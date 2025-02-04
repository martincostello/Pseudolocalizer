# Pseudolocalizer

[![NuGet](https://img.shields.io/nuget/v/PseudoLocalize?logo=nuget&label=Latest&color=blue)](https://www.nuget.org/packages/PseudoLocalize "Download PseudoLocalize from NuGet")
[![NuGet Downloads](https://img.shields.io/nuget/dt/PseudoLocalize?logo=nuget&label=Downloads&color=blue)](https://www.nuget.org/packages/PseudoLocalize "Download PseudoLocalize from NuGet")

[![Build status](https://github.com/martincostello/Pseudolocalizer/actions/workflows/build.yml/badge.svg?branch=main&event=push)](https://github.com/martincostello/Pseudolocalizer/actions?query=workflow%3Abuild+branch%3Amain+event%3Apush)

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

## Feedback

Any feedback or issues for this package can be added to the issues in [GitHub](https://github.com/martincostello/Pseudolocalizer/issues "This package's issues on GitHub.com").

## License

This package is licensed under the [MIT](https://github.com/martincostello/Pseudolocalizer/blob/main/LICENSE "The MIT license") license.
