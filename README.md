# Snake — Modern C# Game Using Clean Architecture and Async

[![Releases](https://img.shields.io/github/v/release/rusello25/Snake?label=Releases&logo=github&color=007ec6)](https://github.com/rusello25/Snake/releases)

🟩🐍 A modern take on the classic Snake game. Built with C# 12, .NET 8, and a clean, testable architecture that showcases DDD, dependency injection, async programming, and SOLID design.

![Snake Game Preview](https://upload.wikimedia.org/wikipedia/commons/3/3c/Snake_on_old_cell_phone.png)

Table of contents
- About this repo
- Key features
- Architectural overview
- Domain model and patterns
- Async, performance, and threading
- Getting started
  - Download release (recommended)
  - Build from source
  - Run and play
- Project layout
- Tests and quality
- Contributing
- License
- Links and resources

About this repo
This repository demonstrates a real-world approach to a small game while applying enterprise-grade patterns. It targets C# 12 and .NET 8. It mixes game logic with robust architecture. Engineers use it to learn how to structure a small game using domain-driven design (DDD), dependency injection (DI), and asynchronous patterns.

Key features
- Core game loop implemented with async/await and cooperative scheduling.
- Clear separation between Domain, Application, and Infrastructure layers.
- Dependency Injection via the built-in DI container.
- DDD-style aggregates for Snake and Board.
- Testable input and rendering adapters.
- Small, deterministic game engine for reliable unit tests.
- Sample high-performance renderer using console or simple GUI.
- Configurable difficulty, grid size, and seedable RNG for reproducible runs.

Architectural overview
This project follows a clean architecture style:
- Domain: Entities, value objects, domain services, and domain events. No framework references.
- Application: Use cases (commands and queries), DTOs, and interfaces.
- Infrastructure: Rendering, input, persistence, and platform adapters.
- Composition root: Wire up DI and host the game.

The design keeps the game loop and rules in the Domain and Application layers. The Infrastructure layer only adapts the domain to the platform (console or window). Tests target Domain and Application layers only.

Domain model and patterns
The domain focuses on clear, small models and predictable rules.

Core aggregates and objects
- Snake: Holds body segments, direction, movement rules, and collisions.
- Board: Grid size, walls, and apple placement.
- GameSession: Encapsulates the current round, score, and game state.
- Position and Direction: Small value objects.

Design patterns used
- Repository (in-memory) for game session persistence during play or replays.
- Factory for board and snake initializers.
- Observer / event dispatcher for input and UI updates.
- Mediator-style command handlers for user actions and system events.
- Strategy for different movement and collision rules (classic, wrap-around, no-walls).

SOLID application
- Single Responsibility: Small classes perform clearly defined jobs.
- Open/Closed: Rules and renderers extend via abstractions.
- Liskov Substitution: Interfaces use narrow contracts.
- Interface Segregation: Keep small adapter interfaces.
- Dependency Inversion: High-level modules depend on interfaces.

Async, performance, and threading
The game uses async primitives to keep the UI responsive and maintain testable timing.

Key techniques
- Game loop runs on a cooperative scheduler using async delays.
- Input uses a non-blocking adapter that raises events into the Application layer.
- Renderers implement fast diff-based updates to avoid full redraws.
- Cancellation tokens handle graceful shutdowns.
- The domain rules run synchronously for determinism; timing and I/O run async.

This design keeps game rules deterministic for tests while letting the UI and input operate asynchronously.

Getting started

Download release (recommended)
Download the latest release package and run the executable. Visit the Releases page and download the release asset, then run the file.

- Releases: https://github.com/rusello25/Snake/releases  
- Download the release asset (for example `Snake-win-x64.zip` or `Snake-x64.exe`) and execute the binary to run the game on your machine.

If the releases link does not work in your environment, check the Releases section of this repository on GitHub. The release asset contains a self-contained executable or zip with a runnable binary and a small README with platform specifics.

Build from source
If you prefer to build locally, follow these steps.

Prerequisites
- .NET 8 SDK
- A terminal or IDE (Visual Studio, VS Code, Rider)

Steps
1. Clone the repo:
   - `git clone https://github.com/rusello25/Snake.git`
2. Restore packages:
   - `dotnet restore`
3. Build the solution:
   - `dotnet build -c Release`
4. Run the app from the sample host:
   - `dotnet run --project src/Snake.Host.Console`

Run and play
- Keyboard input maps:
  - Arrow keys or WASD to change direction.
  - P to pause/resume.
  - R to restart a session.
- Options:
  - Adjust grid size and difficulty via `appsettings.json` or command-line switches.
  - Use a seed option for reproducible apple placement and deterministic runs.

Project layout
- src/
  - Snake.Domain/ — Entities, value objects, domain events, rules.
  - Snake.Application/ — Commands, handlers, DTOs, interfaces.
  - Snake.Infrastructure.Rendering/ — Console, minimal GUI rendering adapters.
  - Snake.Infrastructure.Input/ — Keyboard adapters, test doubles.
  - Snake.Host.Console/ — Console host and composition root.
  - Snake.Host.Gui/ — Optional simple GUI host (WPF/WinUI sample).
- tests/
  - Snake.Domain.Tests/ — Unit tests for domain rules.
  - Snake.Application.Tests/ — Integration-like tests for use cases.

Tests and quality
The repo includes a full suite of unit tests that assert game rules and expected behavior.

- Domain tests assert collisions, apple placement, growth rules, and wrap/no-wrap behavior.
- Use case tests exercise DI, handlers, and the game loop interactions using deterministic fake clocks.
- CI pipeline runs `dotnet test` across projects and runs static analysis.

Typical test commands
- `dotnet test tests/Snake.Domain.Tests`
- `dotnet test`

Examples and code snippets
App command handler example (pseudocode style)

- Create a command to move the snake:
  - `MoveSnakeCommand { Direction }`
- The handler:
  - Validate direction.
  - Compute next position.
  - Apply collision and growth checks.
  - Emit domain events for score and state changes.

Simple host wiring (conceptual)
- Register domain services and repositories.
- Register input and render adapters.
- Start the game loop:
  - `await host.RunGameAsync(ct);`

Design trade-offs
- Keep domain synchronous for testability and deterministic behavior.
- Place timing and delays in the host layer.
- Use simple in-memory repos to avoid external dependencies in tests.

Contributing
This repo welcomes contributions that improve clarity, add tests, or extend patterns.

How to contribute
- Fork the repository.
- Create a feature branch.
- Add tests for new behavior.
- Open a pull request with a clear description and small changesets.

Code style
- Follow simple, readable code.
- Prefer explicit names.
- Keep methods short and focused.
- Use DI and interfaces for platform concerns.

Roadmap ideas
- Add more render backends (SDL2, OpenGL, WebAssembly).
- Add replay support and deterministic recording.
- Add AI opponents and scripted agents.
- Add a benchmark harness for performance measurements.

Acknowledgements and resources
- .NET and C# documentation for async/await and performance guidance.
- Clean Architecture and DDD resources for domain modeling patterns.
- Community examples of simple game engines in managed languages.

Badges and topics
- Topics used to help discovery: asynchronous-programming, clean-architecture, csharp12, ddd-architecture, dependency-injection, design-patterns, enterprise-solutions, game-development, net8, solid

Contact and support
Open an issue or PR on GitHub for bugs or feature ideas. Use the Discussions tab for design questions and learning-focused threads.

License
- The repository uses a permissive license. Check the LICENSE file for details.

Releases (again)
Download the latest packaged build from the Releases page and run the delivered executable or unpack the zip and run the included binary: https://github.com/rusello25/Snake/releases

Use the release asset to try the game quickly or follow the build instructions above to run from source.

Enjoy exploring a small game built with strong architectural ideas.