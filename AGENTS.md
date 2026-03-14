# Coding Agent Instructions

This file provides guidance to coding agents when working with code in this repository.

## Build, test, and lint

- Preferred full local validation is `.\build.ps1`. This script boots the SDK version from `global.json`, packs `PseudoLocalizer.Core`, `PseudoLocalizer.Humanizer`, and `PseudoLocalize`, then runs `dotnet test -c Release` unless `-SkipTests` is passed.
- Build a single project with `dotnet build .\PseudoLocalizer.Core\PseudoLocalizer.Core.csproj` or `dotnet build .\PseudoLocalize\PseudoLocalize.csproj`.
- Run all tests with `dotnet test -c Release`.
- Run one test project with `dotnet test .\PseudoLocalizer.Core.Tests\PseudoLocalizer.Core.Tests.csproj` or `dotnet test .\PseudoLocalize.Tests\PseudoLocalize.Tests.csproj`.
- Run a single NUnit test with a filter, for example:
  - `dotnet test .\PseudoLocalizer.Core.Tests\PseudoLocalizer.Core.Tests.csproj --filter "FullyQualifiedName~PseudoLocalizer.Core.Tests.TransformTests.TestPipeline"`
  - `dotnet test .\PseudoLocalize.Tests\PseudoLocalize.Tests.csproj --filter "FullyQualifiedName~PseudoLocalizer.EndToEndTest.ShouldRespectXlfInlineInterpolation"`
- CI linting lives in `.github\workflows\lint.yml`. The repo-local lint command it uses for scripts is:

```powershell
$settings = @{
  IncludeDefaultRules = $true
  Severity = @("Error", "Warning")
}
Invoke-ScriptAnalyzer -Path . -Recurse -ReportSummary -Settings $settings
```

## High-level architecture

- `PseudoLocalizer.Core` is the main engine. It contains the transformer pipeline (`Pipeline`) plus the concrete transforms: `ExtraLength`, `Accents`, `Brackets`, `Mirror`, and `Underscores`.
- `PseudoLocalize` is the .NET global tool. `Program` parses CLI flags, builds a `Pipeline`, selects a processor by file extension (`.resx`, `.xlf`, `.po`/`.pot`), and writes either a sibling output file or overwrites the input.
- `PseudoLocalizer.Core` processors are event-driven. `Processor` exposes a `TransformString` event; the CLI wires a single `ITransformer` pipeline into that event with `transformer.Apply(e)`. This is the seam to preserve if you add new file formats or transformation entry points.
- File-format logic is split by resource type:
  - `ResxProcessor` updates `/root/data/value` nodes only.
  - `XlfProcessor` supports both XLIFF 1.2 and 2.0, updates target language metadata, and preserves inline elements by temporarily converting them to placeholder tokens before transformation and reconstructing the XML afterward.
  - `POProcessor` parses and regenerates catalogs with `Karambolo.PO.Compact`, updating singular and plural entries while preserving comments and headers.
- `PseudoLocalizer.Humanizer` adapts the same transformation model to Humanizer. `PseudoHumanizer` mirrors the CLI option set, creates the same ordered pipeline, and registers pseudo-localized Humanizer formatters/converters for `qps-Ploc`.
- Tests are split by scope: `PseudoLocalizer.Core.Tests` covers transformers and processors directly, while `PseudoLocalize.Tests` calls `Program.Main(...)` for end-to-end CLI behavior.

## Key conventions

- Default pseudolocalization behavior is shared across the CLI and Humanizer integration: if no explicit transform flags are chosen, the default pipeline is `ExtraLength -> Accents -> Brackets`. Keep that order aligned when changing options.
- Transformations must preserve placeholders and markup. The existing tests expect format strings, HTML/XML fragments, and XLIFF inline elements to survive pseudolocalization intact while surrounding text changes.
- Processor tests are not just string tests; they verify that only translatable values change and that comments, metadata, and document structure remain valid. Follow that pattern when changing resource-file handling.
- CLI end-to-end tests call `Program.Main(...)` directly and verify both file contents and XML validity. When changing CLI behavior, prefer extending those tests instead of only adding lower-level unit tests.
- The repo uses central package management via `Directory.Packages.props` and shared build settings via `Directory.Build.props`. Add package versions centrally unless a project has a strong reason not to.
- Formatting is repository-specific:
  - C# uses 4 spaces and UTF-8 with BOM.
  - `csproj`, `json`, `props`, and `xlf` files use 2 spaces.
  - The repo uses CRLF line endings.
- `Nullable` is disabled repo-wide in `Directory.Build.props`, so avoid introducing nullable-annotation style that assumes nullable reference types are enabled unless you first align with the existing project setup.

## General guidelines

- Always ensure code compiles with no warnings or errors and tests pass locally before pushing changes.
- Do not change the public API unless specifically requested.
- Do not use APIs marked with `[Obsolete]`.
- Bug fixes should **always** include a test that would fail without the corresponding fix.
- Do not introduce new dependencies unless specifically requested.
- Do not update existing dependencies unless specifically requested.
