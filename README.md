# Pseudolocalizer

[![NuGet](https://img.shields.io/nuget/v/PseudoLocalize?logo=nuget&label=Latest&color=blue)](https://www.nuget.org/packages/PseudoLocalize "Download PseudoLocalize from NuGet")
[![NuGet Downloads](https://img.shields.io/nuget/dt/PseudoLocalize?logo=nuget&label=Downloads&color=blue)](https://www.nuget.org/packages/PseudoLocalize "Download PseudoLocalize from NuGet")

[![Build status](https://github.com/martincostello/Pseudolocalizer/actions/workflows/build.yml/badge.svg?branch=main&event=push)](https://github.com/martincostello/Pseudolocalizer/actions?query=workflow%3Abuild+branch%3Amain+event%3Apush)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/martincostello/Pseudolocalizer/badge)](https://securityscorecards.dev/viewer/?uri=github.com/martincostello/Pseudolocalizer)

## Introduction

Pseudolocalizer is a [.NET Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools ".NET Global Tools overview") for testing internationalization aspects of software. Specifically, it reads string values from resource files in the [resx](https://docs.microsoft.com/en-us/dotnet/framework/resources/creating-resource-files-for-desktop-apps#resources-in-resx-files "Resources in .resx Files
"), [XLIFF](https://en.wikipedia.org/wiki/XLIFF "XLIFF"), or [GetText Portable Object (PO)](https://www.gnu.org/software/gettext/manual/html_node/PO-Files.html) format and generates fake translations for the _"qps-Ploc"_ pseudo-locale ([MSDN](https://docs.microsoft.com/en-gb/windows/desktop/Intl/using-pseudo-locales-for-localization-testing "Using pseudo-locales for localizability testing")).

The tool is run from the command line and provides the following options for the fake translation:

- Add accents on all letters so that non-localized text can be spotted - but without making the text unreadable.
- Make all words 30% longer, to ensure that there is room for translations.
- Add brackets to show the start and end of each localized string.
  This makes it possible to spot strings that have been cut off.
- Reverse all words ("mirror"), to simulate right-to-left locales.
- Replace all characters with underscores so that non-localized text can be spotted.

### See also

[Stack Overflow: How to use enable pseudo-locale in Windows for testing?](https://stackoverflow.com/questions/7042920/how-to-use-enable-pseudo-locale-in-windows-for-testing/ "How to use enable pseudo-locale in Windows for testing?")

## Installation

To install the tool from [NuGet](https://www.nuget.org/packages/PseudoLocalize "PseudoLocalize on NuGet.org") using the .NET SDK run:

```sh
dotnet tool install --global PseudoLocalize
```

## Usage

```console
dotnet pseudo-localize --help
Usage: pseudo-localize [/l] [/a] [/b] [/m] [/u] [/c culture] file [file...]
Generates pseudo-localized versions of the specified input file(s).

The input files must be resource files in Resx, Xlf, or PO file format.
The output will be written to a file next to the original, with .qps-Ploc
(or the output culture you specify) appended to its name. For example, if
the input file is X:\Foo\Bar.resx, then the output file will be
X:\Foo\Bar.qps-Ploc.resx.

Options:
  /h, --help         Show command line help.
  /v, --version      Show the version of the tool.
  /l, --lengthen     Make all words 30% longer, to ensure that there is room for translations.
  /a, --accents      Add accents on all letters so that non-localized text can be spotted.
  /b, --brackets     Add brackets to show the start and end of each localized string.
                     This makes it possible to spot cut off strings.
  /m, --mirror       Reverse all words ("mirror").
  /u, --underscores  Replace all characters with underscores.
  /c, --culture      Use the following string as the culture code in the output file name(s).
  /o, --overwrite    Overwrites the input file(s) with the pseudo-localized version.
  /f, --force        Suppresses the confirmation prompt for the --overwrite option.

The default options, if none are given, are: /l /a /b.
```

## Library

The core functionality is also available as a [NuGet package](https://www.nuget.org/packages/PseudoLocalizer.Core/ "PseudoLocalizer.Core on NuGet.org"), as well as a [NuGet package](https://www.nuget.org/packages/PseudoLocalizer.Humanizer/ "") that plugs into the [Humanizer](https://github.com/Humanizr/Humanizer) library.

These allow you to programmatically pseudo-localize strings for other scenarios not catered for by the PseudoLocalize tool itself.

Took at look at the [source code for PseudoLocalize](https://github.com/martincostello/Pseudolocalizer/blob/3e988d301d1e93e3802434ff8fad7f77e421d266/PseudoLocalize/Program.cs#L316-L354) for examples of how to use the library to pseudo-localize strings and process streams.

| Packages | Latest Version |
|:--|:--|
| `PseudoLocalizer.Core` |  [![NuGet](https://img.shields.io/nuget/v/PseudoLocalizer.Core?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/PseudoLocalizer.Core "Download PseudoLocalizer.Core from NuGet") |
| `PseudoLocalizer.Humanizer` | [![NuGet](https://img.shields.io/nuget/v/PseudoLocalizer.Humanizer?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/PseudoLocalizer.Humanizer "Download PseudoLocalizer.Humanizer from NuGet") |

### Library Installation

```sh
dotnet add package PseudoLocalizer.Core
dotnet add package PseudoLocalizer.Humanizer
```

### Usage with Humanizer

To register pseudo-localization for Humanizer, add the following snippet to your application:

```csharp
new PseudoLocalizer.Humanizer.PseudoHumanizer().Register();
```

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/Pseudolocalizer/issues "Issues for this project on GitHub.com").

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/Pseudolocalizer "This project on GitHub.com"): <https://github.com/martincostello/Pseudolocalizer.git>

## License

This project is licensed under the [MIT](https://github.com/martincostello/Pseudolocalizer/blob/main/LICENSE "The MIT license") license.

## Building and Testing

Compiling the tool yourself requires Git and the [.NET SDK](https://www.microsoft.com/net/download/core "Download the .NET SDK") to be installed.

To build and test the tool locally from a terminal/command-line, run the following set of commands:

```sh
git clone https://github.com/martincostello/Pseudolocalizer.git
cd Pseudolocalizer
./build.sh
```
