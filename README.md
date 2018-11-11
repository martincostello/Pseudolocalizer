# Pseudolocalizer

[![NuGet](https://buildstats.info/nuget/PseudoLocalize?includePreReleases=false)](https://www.nuget.org/packages/PseudoLocalize "Download PseudoLocalize from NuGet")

[![Build Status](https://dev.azure.com/martincostello/Pseudolocalizer/_apis/build/status/CI)](https://dev.azure.com/martincostello/Pseudolocalizer/_build/latest?definitionId=72)

[![Build history](https://buildstats.info/azurepipelines/chart/martincostello/Pseudolocalizer/72?branch=master&includeBuildsFromPullRequest=false)](https://dev.azure.com/martincostello/Pseudolocalizer/_build?definitionId=72)

## Introduction

Pseudolocalizer is a [.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools ".NET Core Global Tools overview") for testing internationalization aspects of software. Specifically, it reads string values from resource files in the Resx or Xlf format and generates fake translations for the "qps-Ploc" pseudo-locale ([MSDN](https://docs.microsoft.com/en-gb/windows/desktop/Intl/using-pseudo-locales-for-localization-testing "Window pseudo-localization "Using pseudo-locales for localizability testing")).

The tool is run from the command line and provides the following options for the fake translation:

  * Add accents on all letters so that non-localized text can be spotted - but without making the text unreadable.
  * Make all words 30% longer, to ensure that there is room for translations.
  * Add brackets to show the start and end of each localized string.
  This makes it possible to spot strings that have been cut off.
  * Reverse all words ("mirror"), to simulate right-to-left locales.
  * Replace all characters with underscores so that non-localized text can be spotted.

### See also
  * [WPF Localization Guidance Whitepaper by Rick Strahl and Michele Leroux Bustamante](https://archive.codeplex.com/?p=wpflocalization "WPF Localization Guidance")
  * [Stack Overflow: How to use enable pseudo-locale in Windows for testing?](https://stackoverflow.com/questions/7042920/how-to-use-enable-pseudo-locale-in-windows-for-testing/ "How to use enable pseudo-locale in Windows for testing?")

## Installation

To install the tool from [NuGet](https://www.nuget.org/packages/MartinCostello.Logging.XUnit/ "MartinCostello.Logging.XUnit on NuGet.org") using the .NET SDK run:

```
dotnet tool install -g PseudoLocalize
```

## Usage

```
Usage: pseudo-localize [/l] [/a] [/b] [/m] [/u] file [file...]
Generates pseudo-localized versions of the specified input file(s).

The input files must be resource files in Resx or Xlf file format.
The output will be written to a file next to the original, with .qps-Ploc
appended to its name. For example, if the input file is X:\Foo\Bar.resx,
then the output file will be X:\Foo\Bar.qps-Ploc.resx.

Options:
  /h, --help         Show command line help.
  /l, --lengthen     Make all words 30% longer, to ensure that there is room for translations.
  /a, --accents      Add accents on all letters so that non-localized text can be spotted.
  /b, --brackets     Add brackets to show the start and end of each localized string.
                     This makes it possible to spot cut off strings.
  /m, --mirror       Reverse all words ("mirror").
  /u, --underscores  Replace all characters with underscores.
  /o, --overwrite    Overwrites the input file(s) with the pseudo-localized version.
  /f, --force        Suppresses the confirmation prompt for the --overwrite option.

The default options, if none are given, are: /l /a /b.
```

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/Pseudolocalizer/issues "Issues for this project on GitHub.com").

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/Pseudolocalizer "This project on GitHub.com"): https://github.com/martincostello/Pseudolocalizer.git

## License

This project is licensed under the [MIT](https://github.com/martincostello/Pseudolocalizer/blob/master/LICENSE "The MIT license") license.

## Building and Testing

Compiling the tool yourself requires Git and the [.NET Core SDK](https://www.microsoft.com/net/download/core "Download the .NET Core SDK") to be installed (version `2.1.403` or later).

To build and test the tool locally from a terminal/command-line, run one of the following set of commands:

**Windows**

```powershell
git clone https://github.com/martincostello/Pseudolocalizer.git
cd Pseudolocalizer
.\Build.ps1
```

**Linux/macOS**

```sh
git clone https://github.com/martincostello/Pseudolocalizer.git
cd Pseudolocalizer
./build.sh
```
