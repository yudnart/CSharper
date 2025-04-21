# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added
- Base type for `Entity` class.

## [1.0.0] - 2025-04-21

### Added
- Initial stable release.
- **Mediator**: Implemented `IMediator` with support for global behavior (`IBehavior`) and request-specific behavior (`IBehavior<TRequest>`).
- **Results**: Added `Result` and `Result<T>` types.
- **Functional Extensions**: Added functional extension methods for `Result` and `Result<T>`.
