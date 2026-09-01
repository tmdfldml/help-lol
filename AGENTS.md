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

Stage 3: Team and Lane Matchup Parsing.

### Allowed

- Identify the current player's Riot ID through the Live Client Data API.
- Determine the current player's team.
- Separate allied and opposing players.
- Normalize player positions.
- Create a TOP, JUNGLE, MID, ADC, SUPPORT lane structure.
- Match allied and opposing champions in the same lane.
- Output team and lane matchup parsing results to the debug console.
- Continue using the local Live Client Data API based on
  `https://127.0.0.1:2999/liveclientdata/`.
- Use the built-in .NET `HttpClient` and `System.Text.Json` APIs.

### Not Allowed Yet

- Do not calculate champion matchup scores.
- Do not add win-rate data or matchup JSON data.
- Do not display actual matchup data in the UI.
- Do not implement automatic game detection or repeated polling.
- Do not implement jungle intervention recommendations.
- Do not add Riot Web API integration or a Riot API key.
- Do not add a server or database.
- Do not add external NuGet packages unless explicitly required.
- Keep the existing C#, .NET 10, WPF, Windows-only technology stack.
