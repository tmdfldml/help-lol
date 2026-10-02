using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Resources;
using LoLMatchup.Services;
using LoLMatchup.Services.Development;
using LoLMatchup.Models.Presentation;

internal static class Program
{
    [STAThread]
    private static void Main() => RunAsync().GetAwaiter().GetResult();

    private static async Task RunAsync()
    {
        await DiagnosticsRegression.RunAsync();
        Trace.Listeners.Add(new ConsoleTraceListener());
        var metadata = new ChampionMetadataService();
        var validator = typeof(LiveStatisticsCoordinator).Assembly.GetType("LoLMatchup.Services.MatchupStatisticsDebugValidator")!;
        await (Task)validator.GetMethod("ValidateAsync", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [metadata])!;
        Console.WriteLine("Existing WPF DEBUG validators: PASS");
        var dev = new DeveloperTestModeService();
        using var client = new LiveClientService();
        var coordinator = new LiveStatisticsCoordinator(client, developerTestMode: dev);
        var events = 0;
        dev.StateChanged += (_, _) => events++;
        string? lastKey = null;
        for (var cycle = 0; cycle < 20; cycle++)
        {
            dev.StartScenario(DeveloperTestModeService.CreateNormal5v5());
            var signature = await coordinator.GetCurrentMatchupAsync();
            Require(signature is not null && signature.CacheKey != lastKey, "Start unique cache key");
            lastKey = signature!.CacheKey;
            var presentation = await coordinator.GetPresentationAsync(signature);
            Require(presentation.DisplayMatchups.Count == 5
                && presentation.DisplayMatchups.Select(x => x.Role).Distinct().Count() == 5,
                "Exactly five distinct roles");
            dev.Stop();
            Require(!dev.IsActive && dev.CurrentScenario is null, "Stop clears fixture");
        }
        Require(events == 40, "One change notification per start/stop");
        Console.WriteLine("20 start/stop service+coordinator+presentation cycles: PASS (40 notifications, no duplicate role/cache)");
        await Check(DeveloperTestModeService.CreatePartialRoles(), "Partial", 1);
        await Check(DeveloperTestModeService.CreateNotFound(), "NotFound", 4, expectedTopNotFound: true);
        await Check(DeveloperTestModeService.CreateCustom(
            ["Aatrox", null, null, null, null], ["Akali", null, null, null, null]), "Custom common", 4);
        await Check(DeveloperTestModeService.CreateCustom(
            [null, null, null, null, "Camille"], [null, null, null, null, "Alistar"]), "Custom off-meta", 4);
        await Check(DeveloperTestModeService.CreateCustom(
            ["UnknownChampion123", null, null, null, null], ["UnknownChampion456", null, null, null, null]),
            "Custom absent fixture", 4, expectedTopNotFound: true);
        Require(await coordinator.GetCurrentMatchupAsync() is null, "No-game stop returns waiting signature");
        Console.WriteLine("Stop -> no-game Waiting signature: PASS");
        var manifest = System.Text.Json.JsonSerializer.Deserialize<LoLMatchup.Models.Statistics.DatasetUpdateManifest>(
            await File.ReadAllTextAsync("Data/Statistics/manifest.json"))!;
        using (var updater = new LoLMatchup.Services.Statistics.DatasetUpdateService(
            new LoLMatchup.Services.Statistics.DatasetUpdateOptions
            { ManifestUri = LoLMatchup.Services.Statistics.DatasetUpdateOptions.ProductionManifestUri },
            Path.Combine(Path.GetTempPath(), "lolmatchup-readonly-unused", "master_plus.json")))
        {
            var result = await updater.CheckAsync(manifest);
            Require(result.Status == LoLMatchup.Services.Statistics.DatasetUpdateStatus.UpToDate, "Public UpToDate");
            Console.WriteLine("Public endpoint + current packaged manifest: UpToDate PASS (read-only)");
        }

        foreach (var config in new[] { "Debug", "Release" })
        {
            var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", config, "net10.0-windows", "LoLMatchInfo.dll");
            using var stream = File.OpenRead(assemblyPath);
            using var pe = new PEReader(stream);
            var reader = pe.GetMetadataReader();
            var names = reader.TypeDefinitions.Select(handle => reader.GetString(reader.GetTypeDefinition(handle).Name)).ToArray();
            var developerTypes = names.Count(x => x.Contains("DeveloperTest") || x.Contains("SearchableChampionPicker"));
            var crashMethods = reader.MethodDefinitions.Select(handle => reader.GetString(reader.GetMethodDefinition(handle).Name))
                .Count(name => name is "CrashUi_Click" or "CrashTask_Click" or "CreateUnobservedTask" or "PatchMismatch_Click" or "Viewport_Click" or "UpdateFailure_Click" or "ShowUpdateFailureFixture");
            Require(config == "Debug" ? crashMethods == 7 : crashMethods == 0, "Crash/patch fixtures excluded from Release");
            var resourceDirectory = pe.PEHeaders.CorHeader!.ResourcesDirectory;
            var baml = new List<string>();
            foreach (var handle in reader.ManifestResources)
            {
                var resource = reader.GetManifestResource(handle);
                if (!resource.Implementation.IsNil || !reader.GetString(resource.Name).EndsWith(".g.resources")) continue;
                var block = pe.GetSectionData(resourceDirectory.RelativeVirtualAddress + (int)resource.Offset).GetReader();
                using var bytes = new MemoryStream(block.ReadBytes(block.ReadInt32()));
                using var resources = new ResourceReader(bytes);
                foreach (System.Collections.DictionaryEntry item in resources)
                    if (((string)item.Key).Contains("developertest") || ((string)item.Key).Contains("searchablechampionpicker")) baml.Add((string)item.Key);
            }
            Require(config == "Debug" ? developerTypes > 0 && baml.Count == 2 : developerTypes == 0 && baml.Count == 0,
                "Developer PE types and embedded BAML " + config);
            Console.WriteLine($"{config} artifact: developer types={developerTypes}, developer BAML={baml.Count}, diagnostic fixture methods={crashMethods}: PASS");
        }

        async Task Check(LoLMatchup.Models.Development.DeveloperTestScenario scenario,
            string name, int missing, bool expectedTopNotFound = false)
        {
            dev.StartScenario(scenario);
            var signature = await coordinator.GetCurrentMatchupAsync();
            Require(signature is not null, name + " signature");
            var presentation = await coordinator.GetPresentationAsync(signature!);
            Require(presentation.DisplayMatchups.Count == 5
                && presentation.DisplayMatchups.Count(x => x.State == MatchupPresentationState.Waiting) == missing,
                name + " missing slots");
            Require(presentation.DisplayMatchups.Where(x => !x.HasStatistics)
                .All(x => !x.HasOutcomeEvaluation && !x.HasCombatPressure && !x.HasLevelTempo), name + " no absent-data indicators");
            if (expectedTopNotFound) Require(presentation.DisplayMatchups[0].State == MatchupPresentationState.MatchupNotFound, name + " absent");
            dev.Stop();
            Console.WriteLine(name + " pipeline: PASS");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
