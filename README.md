# CSharper

[![NuGet Version](https://img.shields.io/nuget/vpre/dht.csharper?label=nuget&style=flat-square&color=blue)](https://www.nuget.org/packages/dht.csharper)
[![NuGet Downloads](https://img.shields.io/nuget/dt/dht.csharper?label=downloads&style=flat-square&color=teal)](https://www.nuget.org/stats/packages/dht.csharper?groupby=Version)
[![NuGet Version](https://img.shields.io/github/actions/workflow/status/yudnart/CSharper/publish-nuget.yml?style=flat-square&color=green)](https://github.com/yudnart/CSharper/actions/workflows/publish-nuget.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-orange.svg)](LICENSE)

> :heart: [Request features or give feedback](https://github.com/yudnart/CSharper/issues)

CSharper provides utilities for .NET development, distributed as the `dht.csharper` NuGet package, targeting .NET Standard 2.0 and .NET 8.0. 
It includes Mediator for decoupled request handling, Results for error management, and Functional extensions for task composition, supporting 
modular and reliable applications.

## Overview

The CSharper library offers components to enhance .NET projects. Its Mediator enables request processing with commands, queries, and behaviors,
Results provides robust error handling with typed outcomes, and Functional extensions support expressive, composable workflows. These utilities 
integrate seamlessly, fitting contexts like web controllers or service layers, and promote clean, testable code.

## Installation

To use CSharper, install the `dht.csharper` package:
1. Run the following command in your project directory
   ```bash
   dotnet add package dht.csharper
   ```
2. Alternatively, use the Package Manager Console in Visual Studio:
   ```powershell
   Install-Package dht.csharper
   ```
3. Or, add it via your IDE’s package manager (e.g., Visual Studio, Rider).

## Features

- [**Types**](docs/CSharper.Types.md): Contains based types for modeling your app domain.
- [**Results**](docs/CSharper.Results.md): Provides `Result` and `Result<T>` for functional error handling, with `Error` objects detailing 
issues via `Message`, `Code`, and `Path`.
- [**Functional Extensions**](docs/CSharper.Functional.md): Enhances `Result` and `Result<T>` with methods like `Bind`, `Map`, and `Ensure`
for synchronous and asynchronous task composition.
- [**Mediator**](docs/CSharper.Mediator.md): Supports commands for actions, queries for data retrieval, and behaviors for shared logic like 
validation or logging, enabling decoupled request handling across application layers.

## Inspirations

CSharper draws inspiration from several established projects in the .NET and functional programming communities:
- [**CSharpFunctionalExtensions**](https://github.com/vkhorikov/CSharpFunctionalExtensions): A functional programming library for C#, influencing
CSharper’s Functional extensions with monadic operations and result handling.
- [**FluentResults**](https://github.com/altmann/FluentResults): A result-handling library that shaped CSharper’s Results module, emphasizing 
typed outcomes and error management.
- [**MediatR**](https://github.com/jbogard/MediatR): A widely-used .NET library for in-process messaging and mediator patterns, inspiring 
CSharper’s Mediator component for decoupled request handling.

These projects provided valuable patterns and ideas, adapted to fit CSharper’s goals of simplicity and integration in .NET applications.

## Contributing

Contributions are welcome! Submit issues or pull requests to the [CSharper repository](https://github.com/yudnart/CSharper). Ensure code follows 
the existing style and includes tests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
