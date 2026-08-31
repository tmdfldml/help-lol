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

Stage 2: Live Client Data API integration test.

- League of Legends Live Client Data API connection testing is allowed.
- Local API connection tests based on
  `https://127.0.0.1:2999/liveclientdata/` are allowed.
- Use the built-in .NET `HttpClient` and `System.Text.Json` APIs.
- Do not add Riot Web API integration or a Riot API key yet.
- Do not add a server or database.
- Do not implement matchup analysis, automatic game detection,
  repeated polling, or jungle intervention recommendations yet.
- Do not add external NuGet packages unless explicitly required.
- Keep the existing C#, .NET 10, WPF, Windows-only technology stack.
