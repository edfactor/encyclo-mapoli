# GitHub Copilot Custom Instructions

## Purpose
This file defines workspace-specific guidelines and preferences for GitHub Copilot suggestions in this repository. It is intended to help Copilot generate code and documentation that matches the team’s standards and project requirements.

---

## General Coding Standards
- **Target Frameworks:**
  - Use .NET 9 for new code unless otherwise specified.
  - Support .NET Standard 2.0 for shared libraries if required.
- **Language:**
  - Use C# 13.0 features where appropriate.
- **File Encoding:**
  - Use UTF-8 with BOM for all C# and VB files.
- **Line Endings:**
  - Use CRLF for most files, LF for shell scripts.
- **Indentation:**
  - Use 4 spaces for code files, 2 spaces for XML/config files.
- **Braces:**
  - Always use braces for all control blocks.
- **Namespace:**
  - Use file-scoped namespaces for C# files.
- **Type Declarations:**
  - Prefer explicit types over `var` unless the type is obvious.
- **Constants:**
  - Name constant fields in PascalCase.
- **Static Fields:**
  - Prefix private/internal static fields with `s_`.
- **Private/Internal Fields:**
  - Prefix with `_` and use camelCase.
- **Readonly:**
  - Mark fields as `readonly` where possible.

## Project Structure
- **Startup Project:**
  - Use `Demoulas.ProfitSharing.AppHost` for running/debugging.
- **Secrets:**
  - Store sensitive data in `secrets.json` (never commit secrets).
- **Testing:**
  - Place unit/integration tests in the `tests` directory. Use xUnit and FluentAssertions.

## Code Quality & Analyzers
- **Security:**
  - Treat all security analyzer warnings as errors.
- **Unused Parameters:**
  - Warn on all unused parameters.
- **Accessibility:**
  - Require explicit accessibility modifiers for non-interface members.
- **Prefer Initializers:**
  - Use object/collection initializers where possible.
- **Null Checks:**
  - Prefer null propagation and coalesce expressions.

## Pull Request & Review Guidance
- Ensure code meets requirements and is covered by tests.
- Write clear, maintainable, and well-documented code.
- Avoid inefficient code paths and unnecessary allocations.
- Avoid vulnerabilities (e.g., SQL injection, secrets in code).
- Update README and inline comments as needed.
- Follow SOLID principles and project architecture.
- Ensure backward compatibility and cross-platform support.

## Copilot Usage
- Prefer Copilot suggestions that match these conventions.
- When generating new files, include file headers and XML documentation for public APIs.
- For background services, use `BackgroundService` as the base class.
- For dependency injection, use constructor injection and register services in the appropriate DI container.
- For database access, prefer async/await and EF Core best practices.

---

# End of Copilot Instructions
