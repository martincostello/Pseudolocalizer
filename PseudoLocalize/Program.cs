namespace PseudoLocalizer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;
    using PseudoLocalizer.Core;

    /// <summary>
    /// Main class for the pseudo-localizer console application.
    /// </summary>
    public class Program
    {
        private List<string> _inputFiles = new List<string>();

        public Program()
        {
            UseDefaultOptions = true;
        }

        public bool UseDefaultOptions { get; set; }

        public bool EnableExtraLength { get; set; }

        public bool EnableAccents { get; set; }

        public bool EnableBrackets { get; set; }

        public bool EnableMirror { get; set; }

        public bool EnableUnderscores { get; set; }

        public bool Force { get; set; }

        public bool HasInputFiles
        {
            get { return _inputFiles.Count > 0; }
        }

        public bool Overwrite { get; set; }

        public static void Main(string[] args)
        {
            var instance = new Program();
            if (ParseArguments(args, instance))
            {
                instance.Run();
            }
            else
            {
                Console.WriteLine("Usage: pseudo-localize [/l] [/a] [/b] [/m] [/u] file [file...]");
                Console.WriteLine("Generates pseudo-localized versions of the specified input file(s).");
                Console.WriteLine();
                Console.WriteLine("The input files must be resource files in Resx or Xlf file format.");
                Console.WriteLine("The output will be written to a file next to the original, with .qps-Ploc");
                Console.WriteLine("appended to its name. For example, if the input file is X:\\Foo\\Bar.resx,");
                Console.WriteLine("then the output file will be X:\\Foo\\Bar.qps-Ploc.resx.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("  /h, --help         Show command line help.");
                Console.WriteLine("  /l, --lengthen     Make all words 30% longer, to ensure that there is room for translations.");
                Console.WriteLine("  /a, --accents      Add accents on all letters so that non-localized text can be spotted.");
                Console.WriteLine("  /b, --brackets     Add brackets to show the start and end of each localized string.");
                Console.WriteLine("                     This makes it possible to spot cut off strings.");
                Console.WriteLine("  /m, --mirror       Reverse all words (\"mirror\").");
                Console.WriteLine("  /u, --underscores  Replace all characters with underscores.");
                Console.WriteLine("  /o, --overwrite    Overwrites the input file(s) with the pseudo-localized version.");
                Console.WriteLine("  /f, --force        Suppresses the confirmation prompt for the --overwrite option.");
                Console.WriteLine();
                Console.WriteLine("The default options, if none are given, are: /l /a /b.");
            }
        }

        public void ClearInputFiles()
        {
            _inputFiles.Clear();
        }

        public void AddInputFile(string filePath)
        {
            _inputFiles.Add(filePath);
        }

        private static bool ParseArguments(string[] args, Program instance)
        {
            instance.ClearInputFiles();
            instance.UseDefaultOptions = true;
            instance.EnableAccents = false;
            instance.EnableExtraLength = false;
            instance.EnableBrackets = false;
            instance.EnableMirror = false;
            instance.EnableUnderscores = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("/", StringComparison.Ordinal) ||
                    arg.StartsWith("-", StringComparison.Ordinal))
                {
                    string name = arg.TrimStart('-', '/');

                    switch (name.ToUpperInvariant())
                    {
                        case "L":
                        case "LENGTHEN":
                            instance.EnableExtraLength = true;
                            instance.UseDefaultOptions = false;
                            break;

                        case "A":
                        case "ACCENTS":
                            instance.EnableAccents = true;
                            instance.UseDefaultOptions = false;
                            break;

                        case "B":
                        case "BRACKETS":
                            instance.EnableBrackets = true;
                            instance.UseDefaultOptions = false;
                            break;

                        case "F":
                        case "FORCE":
                            instance.Force = true;
                            break;

                        case "M":
                        case "MIRROR":
                            instance.EnableMirror = true;
                            instance.UseDefaultOptions = false;
                            break;

                        case "O":
                        case "OVERWRITE":
                            instance.Overwrite = true;
                            break;

                        case "U":
                        case "UNDERSCORES":
                            instance.EnableUnderscores = true;
                            instance.UseDefaultOptions = false;
                            break;

                        case "H":
                        case "HELP":
                            return false;

                        default:
                            Console.WriteLine("ERROR: Unknown option \"{0}\".", arg);
                            return false;
                    }
                }
                else
                {
                    instance.AddInputFile(arg);
                }
            }

            return instance.HasInputFiles;
        }

        private void Run()
        {
            foreach (var filePath in _inputFiles)
            {
                var processor = GetProcessor(filePath);
                ProcessFile(filePath, processor);
            }
        }

        private IProcessor GetProcessor(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (string.Equals(".xlf", extension, StringComparison.OrdinalIgnoreCase))
            {
                return new XlfProcessor();
            }
            else
            {
                return new ResxProcessor();
            }
        }

        private void ProcessFile(string inputFileName, IProcessor processor)
        {
            try
            {
                string writtenFileName;

                if (Overwrite)
                {
                    if (!Force)
                    {
                        Console.WriteLine("The file {0} will be overwritten.", inputFileName);
                        Console.Write("Are you sure? [y/n]: ", inputFileName);

                        switch (Console.ReadLine().ToUpperInvariant())
                        {
                            case "Y":
                            case "YES":
                                break;

                            default:
                                return;
                        }

                        Console.WriteLine();
                    }

                    using (var inputStream = File.Open(inputFileName, FileMode.Open, FileAccess.ReadWrite))
                    using (var outputStream = new MemoryStream())
                    {
                        Transform(processor, inputStream, outputStream);

                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.SetLength(0);

                        outputStream.Seek(0, SeekOrigin.Begin);
                        outputStream.CopyTo(inputStream);
                        outputStream.Flush();
                    }

                    writtenFileName = inputFileName;
                }
                else
                {
                    var outputFileName = Path.Combine(Path.GetDirectoryName(inputFileName), Path.GetFileNameWithoutExtension(inputFileName) + ".qps-Ploc" + Path.GetExtension(inputFileName));

                    using (var inputStream = File.OpenRead(inputFileName))
                    using (var outputStream = File.OpenWrite(outputFileName))
                    {
                        Transform(processor, inputStream, outputStream);
                    }

                    writtenFileName = outputFileName;
                }

                Console.WriteLine("The file {0} was written successfully.", writtenFileName);
            }
            catch (Exception ex)
            {
                if (ex is PathTooLongException ||
                    ex is FileNotFoundException ||
                    ex is DirectoryNotFoundException ||
                    ex is IOException ||
                    ex is SecurityException)
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        private void Transform(IProcessor processor, Stream inputStream, Stream outputStream)
        {
            if (EnableExtraLength || UseDefaultOptions)
            {
                processor.TransformString += Transform(ExtraLength.Transform);
            }

            if (EnableAccents || UseDefaultOptions)
            {
                processor.TransformString += Transform(Accents.Transform);
            }

            if (EnableBrackets || UseDefaultOptions)
            {
                processor.TransformString += Transform(Brackets.Transform);
            }

            if (EnableMirror)
            {
                processor.TransformString += Transform(Mirror.Transform);
            }

            if (EnableUnderscores)
            {
                processor.TransformString += Transform(Underscores.Transform);
            }

            processor.Transform(inputStream, outputStream);
        }

        private EventHandler<TransformStringEventArgs> Transform(Func<string, string> transformer)
            => (_, e) => e.Value = transformer(e.Value);
    }
}
