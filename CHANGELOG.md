# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/).

## [1.0.0] - 2025-04-21

### Added
- Initial stable release.
- Base type for `Entity` and `ValueObject` class.
- **Results**: Added `Result` and `Result<T>` types.
- **Functional Extensions**: Added functional extension methods for `Result` and `Result<T>`.
- **Mediator**: Implemented `IMediator` with support for global behavior (`IBehavior`) and request-specific behavior (`IBehavior<TRequest>`).
