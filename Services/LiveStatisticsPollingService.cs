using System.Diagnostics;
using System.Net.Http;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Services;

public sealed class LiveStatisticsPollingService
{
    public static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(2);

    private readonly LiveStatisticsCoordinator _coordinator;
    private readonly TimeSpan _interval;
    private string? _currentSignatureKey;
    private MatchupPresentationState? _currentState;

    public LiveStatisticsPollingService(
        LiveStatisticsCoordinator coordinator,
        TimeSpan? interval = null)
    {
        _coordinator = coordinator;
        _interval = interval ?? DefaultInterval;
    }

    public async Task RunAsync(
        Func<TeamMatchupStatisticsPresentation, bool, Task> updatePresentationAsync,
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
                    _currentSignatureKey = null;

                    if (_currentState != MatchupPresentationState.Waiting)
                    {
                        var waitingStatus = new MatchupStatisticsPresenter().PresentStatus(
                            MatchupStatisticsQueryStatus.InvalidQuery);
                        var waiting = new TeamMatchupStatisticsPresentation(
                            waitingStatus.State,
                            waitingStatus.StatusText,
                            StatusDetailText: waitingStatus.StatusDetailText);
                        _currentState = waiting.State;
                        await updatePresentationAsync(waiting, false);
                    }

                    continue;
                }

                if (signature.CacheKey == _currentSignatureKey)
                {
                    continue;
                }

                if (_currentSignatureKey is not null)
                {
                    Debug.WriteLine("Team matchup changed.");
                }

                var presentation = await _coordinator.GetPresentationAsync(
                    signature);
                _currentSignatureKey = signature.CacheKey;
                _currentState = presentation.State;
                await updatePresentationAsync(presentation, true);
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
