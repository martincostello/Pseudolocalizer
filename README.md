Pseudolocalizer
===============
The Pseudolocalizer is a tool for testing internationalization aspects of software. Specifically, it reads string values from resource files in the Resx or Xlf format and generates fake translations for the "qps-ploc" pseudo-locale.

The tool is run from the command line and provides the following options for the fake translation:

- Add accents on all letters so that non-localized text can be spotted -- but without making the text unreadable.
- Make all words 30% longer, to ensure that there is room for translations.
- Add brackets to show the start and end of each localized string.
  This makes it possible to spot strings that have been cut off.
- Reverse all words ("mirror"), to simulate right-to-left locales.
- Replace all characters with underscores so that non-localized text can be spotted.

See also
--------
- [WPF Localization Guidance Whitepaper by Rick Strahl and Michele Leroux Bustamante](https://archive.codeplex.com/?p=wpflocalization)
- [Stack Overflow: How to use enable pseudo-locale in Windows for testing?](https://stackoverflow.com/questions/7042920/how-to-use-enable-pseudo-locale-in-windows-for-testing/)

License (MIT)
-------------
Copyright (C) 2012, Anders Kaplan.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished 
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
