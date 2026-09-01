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

Stage 4: Champion Matchup Data.

### Allowed

- Add local JSON-based champion matchup test data.
- Support TOP, JUNGLE, MID, ADC, and SUPPORT through the same structure.
- Look up champion A versus champion B matchups by position.
- Use a simple matchup score system.
- Classify matchups with results such as favorable, even, or unfavorable.
- Handle missing matchup data safely.
- Continue using the existing Live Client Data API, current-player,
  team-separation, position-normalization, and role-matchup structures.
- Design the local data format so it can later be extended with `patch`,
  `tier`, `sampleSize`, `wins`, `losses`, and `winRate` fields.
- Use only test data and simple scores during Stage 4; do not implement
  real statistical processing yet.
- Keep internal matchup structures independent of a single position even
  while jungle-user functionality remains the current development priority.
- Use built-in .NET functionality, including `System.Text.Json`.

### Not Allowed Yet

- Do not add Riot Web API integration or a Riot API key.
- Do not implement a StatsCollector or collect real match data.
- Do not crawl OP.GG or any other website.
- Do not add a server or database.
- Do not implement automatic statistics updates.
- Do not display matchup results in the UI.
- Do not implement automatic game detection or repeated polling.
- Do not add detailed position-specific tips.
- Do not add external NuGet packages.
- Keep the existing C#, .NET 10, WPF, Windows-only technology stack.
