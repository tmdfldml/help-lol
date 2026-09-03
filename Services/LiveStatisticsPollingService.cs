using System.Diagnostics;
using System.Net.Http;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Services;

public sealed class LiveStatisticsPollingService
{
    public static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(2);

    private readonly LiveStatisticsCoordinator _coordinator;
    private readonly TimeSpan _interval;
    private LiveMatchupSignature? _currentSignature;
    private MatchupPresentationState? _currentState;

    public LiveStatisticsPollingService(
        LiveStatisticsCoordinator coordinator,
        TimeSpan? interval = null)
    {
        _coordinator = coordinator;
        _interval = interval ?? DefaultInterval;
    }

    public async Task RunAsync(
        Func<MatchupStatisticsPresentation, Task> updatePresentationAsync,
        CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_interval);

        do
        {
            try
            {
                var signature = await _coordinator.GetCurrentMatchupAsync(
                    cancellationToken);

                if (signature is null)
                {
                    _currentSignature = null;

                    if (_currentState != MatchupPresentationState.Waiting)
                    {
                        var waiting = new MatchupStatisticsPresenter().PresentStatus(
                            MatchupStatisticsQueryStatus.InvalidQuery);
                        _currentState = waiting.State;
                        await updatePresentationAsync(waiting);
                    }

                    continue;
                }

                if (signature == _currentSignature)
                {
                    continue;
                }

                if (_currentSignature is not null)
                {
                    Debug.WriteLine(
                        $"Matchup changed: {signature.Value.Role} "
                        + $"{signature.Value.QueryChampion} vs "
                        + signature.Value.OpponentChampion);
                }

                var presentation = await _coordinator.GetPresentationAsync(
                    signature.Value);
                _currentSignature = signature;
                _currentState = presentation.State;
                await updatePresentationAsync(presentation);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (HttpRequestException exception)
            {
                Debug.WriteLine($"Live Client polling failed: {exception.Message}");
            }
        }
        while (await timer.WaitForNextTickAsync(cancellationToken));
    }
}
