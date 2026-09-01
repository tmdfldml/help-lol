# LoL Matchup Project Instructions

## Project

LoL Matchup is a Windows-only desktop application for
League of Legends jungle players.

The application will analyze lane matchups when a game starts.

## Technology

- C#
- .NET 10
- WPF
- Windows only

## Development Rules

- Keep the project simple.
- Do not add unnecessary dependencies.
- Do not add a server unless explicitly requested.
- Do not add a database unless explicitly requested.
- Do not change the main technology stack without discussion.
- Never hard-code API keys or secrets.
- Build the project after code changes.
- Fix build errors before considering a task complete.
- Prefer built-in .NET functionality when possible.
- Do not implement unrelated features.

## Current Stage

Stage 5: Match Statistics Collector.

### Objective and Project Boundary

- Build a separate .NET 10 console application under `Tools/StatsCollector/`.
- Keep statistics collection separate from the existing WPF application.
- Use Riot's official Web API to validate a small end-to-end collection,
  analysis, and aggregation pipeline before attempting large-scale collection.
- Preserve the existing C#, .NET 10, WPF, Windows-only application structure.
- Prefer built-in .NET functionality and avoid unnecessary dependencies.

### Data Scope

- Region: KR.
- Queue: Ranked Solo/Duo with `queueId` 420.
- Maintain `MASTER_PLUS` and `EMERALD_PLUS` as separate datasets.
- `MASTER_PLUS` includes Master, Grandmaster, and Challenger and is the primary
  dataset.
- `EMERALD_PLUS` includes Emerald and above and may be used when the primary
  dataset has insufficient samples.
- Do not merge the two datasets into one aggregate implicitly.
- Separate data by patch version.

### Matchup Metrics

Analyze only these metrics during Stage 5:

1. Game wins.
2. First to level 2.
3. First to level 3.
4. First to level 6.
5. Solo kills against the matched opposing-role player before 15:00.

Do not analyze CS, gold, experience differences, KDA, damage, gank deaths,
or turret statistics yet.

### Aggregation and Storage

- Preserve raw counts where possible instead of storing only percentages.
- Support counts such as `sampleSize`, `winsForA`, level milestone outcomes
  for A, B, and unknown/tie, and `soloKillsBefore15ForA/B`.
- Derive win rates and other ratios from raw counts when needed.
- Keep the final statistics model and file format independent of a specific
  data provider.
- Allow future source metadata such as `SELF_RIOT`, `OPGG`, `LOLPS`, or
  `OTHER`, while using Riot official API data only during this stage.
- Do not finalize favorable/unfavorable score thresholds yet.

### Champion and Role Rules

- Continue using the standard roles `TOP`, `JUNGLE`, `MID`, `ADC`, and
  `SUPPORT` through one shared structure.
- Use stable Riot/Data Dragon-compatible English champion IDs, not localized
  display names.
- Keep valid off-meta champion-role samples instead of discarding them solely
  because the pick is uncommon.
- Preserve champion-role occurrence counts and matchup sample sizes so sample
  scarcity can be evaluated later.
- Do not implement a complex confidence algorithm during the initial Stage 5
  pipeline.

### Collection Accuracy

- Deduplicate Match IDs and filter strictly to `queueId` 420.
- Validate complete and plausible role assignments for both teams.
- Match opposing players across all five standard roles.
- Exclude incomplete or malformed matches safely.
- Store each champion pair in one normalized direction without duplicate
  reverse records.
- Persist collection progress so collection can resume after restart.
- Never process an already completed Match ID again.

### Timeline Definitions

- Compare when the two matched opposing-role participants reach levels 2, 3,
  and 6 using Match Timeline data.
- Record simultaneous or indeterminate milestones as unknown/tie rather than
  forcing a winner.
- Define a pre-15-minute solo kill initially as a Champion Kill before 15:00
  where the killer directly kills the matched opposing-role participant and
  no assisting participant is present.
- Verify the actual Riot Match Timeline field structure before finalizing the
  implementation.

### Riot API and Security

- Read the Riot API key from the `RIOT_API_KEY` environment variable.
- Never hard-code, commit, or log the Riot API key.
- Respect Riot API rate limits.
- Handle HTTP 429 using `Retry-After` and handle transient network and 5xx
  failures safely.

### Not Allowed Yet

- Do not immediately collect 50,000 to 100,000 matches; start with a small
  validation sample.
- Do not implement OP.GG, LOL.PS, or other external-site crawlers or automatic
  external statistics collection.
- Before any future external-site integration, separately review terms of
  service, crawling policies, attribution, and request limits.
- Do not modify the WPF UI.
- Do not build a database server or web backend.
- Do not implement automatic statistics deployment or automatic updates on
  other computers.
- Do not implement jungle intervention recommendations or detailed
  position-specific tips.
