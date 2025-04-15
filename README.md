# CSharper

CSharper is a C# class library distributed as a NuGet package, targeting .NET Standard 2.0 and .NET 8.0. It provides common types and services, such as Mediator, Results, and functional extensions, to help developers quickly build robust .NET applications. Currently in its early stages, CSharper is designed to streamline development with reusable, well-structured utilities.

## Features

- **Multi-Target Support**: Compatible with .NET Standard 2.0 (.NET Framework 4.6.1+, .NET Core 2.0+) and .NET 8.0.
- **NuGet Distribution**: Packaged for easy inclusion in your projects.
- **Planned Utilities**: Foundation for Mediator patterns, Result types, and functional programming extensions.
- **Cross-Platform**: Runs on Windows, macOS, and Linux with the .NET SDK.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or any SDK supporting .NET Standard 2.0)
- An IDE like [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/)

### Installation

CSharper is not yet published to NuGet, but you can build and use it locally:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yudnart/CSharper.git
   cd CSharper
   ```

2. **Build the library**:
   ```bash
   dotnet build
   ```

3. **Create a NuGet package**:
   ```bash
   dotnet pack -c Release
   ```

   The package will be in `src/CSharper/bin/Release`.

4. **Add to your project**:
   - Install the local NuGet package:
     ```bash
     dotnet add package CSharper --source ./src/CSharper/bin/Release
     ```
   - Or reference the project directly:
     ```bash
     dotnet add reference ./src/CSharper/CSharper.csproj
     ```

### Usage

CSharper is in its initial phase with no public APIs yet. Future releases will include:

- **Mediator**: For decoupled request handling.
- **Result Types**: For robust error handling.
- **Functional Extensions**: For cleaner, expressive code.

Example (placeholder for future APIs):

```csharp
// Coming soon:
// using CSharper.Results;
// var result = Result.Success("Operation completed");
```

## Project Structure

```
CSharper/
├── src/
│   └── CSharper/       # Class library targeting .NET Standard 2.0 and .NET 8.0
├── tests/
│   └── CSharper.Tests/ # Unit tests for the library
└── README.md
```

- `src/CSharper`: The core library with dependency injection support.
- `tests/CSharper.Tests`: A test project to validate utilities (empty for now).

## Compatibility

CSharper supports:
- **.NET Standard 2.0**: .NET Framework 4.6.1+, .NET Core 2.0+, Mono, Xamarin.
- **.NET 8.0**: Modern .NET applications with the latest features.

## Roadmap

CSharper will expand to include:
- Mediator pattern implementation.
- Result and Option types for functional error handling.
- Extensions for common tasks (e.g., LINQ utilities, string manipulation).
- XML documentation for all public APIs.

## Contributing

Want to contribute to CSharper? We’d love your help!

1. Fork the repository.
2. Create a branch (`git checkout -b feature/YourFeature`).
3. Commit changes (`git commit -m "Add YourFeature"`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact