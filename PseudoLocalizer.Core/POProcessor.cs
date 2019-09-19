using System;
using System.Collections.Generic;
using System.IO;
using Karambolo.PO;

namespace PseudoLocalizer.Core
{
    /// <summary>
    /// Applies transforms to string values in PO resource files.
    /// </summary>
    public class POProcessor : Processor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="POProcessor"/> class.
        /// </summary>
        public POProcessor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POProcessor"/> class.
        /// </summary>
        /// <param name="culture">The output culture.</param>
        public POProcessor(string culture)
        {
            Culture = culture ?? throw new ArgumentNullException(nameof(culture));;
        }

        /// <summary>
        /// Gets the culture code associated with the processor.
        /// </summary>
        public string Culture { get; }

        /// <inheritdoc />
        public override void Transform(Stream inputStream, Stream outputStream)
        {
            var parser = new POParser(new POParserSettings());

            var result = parser.Parse(inputStream);

            if (result.Success)
            {
                var inputCatalog = result.Catalog;
                var outputCatalog = ProcessCatalog(inputCatalog);

                var generator = new POGenerator(new POGeneratorSettings());

                generator.Generate(outputStream, outputCatalog);
            }
            else
            {
                var diagnostics = result.Diagnostics;
                throw new Exception(diagnostics.ToString());
            }
        }

        private POCatalog ProcessCatalog(POCatalog inputCatalog)
        {
            var outputCatalog = new POCatalog
                {
                    HeaderComments = inputCatalog.HeaderComments,
                    Headers = inputCatalog.Headers,
                    Encoding = inputCatalog.Encoding,
                    Language = Culture,
                    PluralFormCount = inputCatalog.PluralFormCount,
                    PluralFormSelector = inputCatalog.PluralFormSelector
                };

            // Header comments etc.

            // Entries
            foreach (var entry in inputCatalog.Values)
            {
                if (entry is POSingularEntry singularEntry)
                {
                    outputCatalog.Add(TransformSingularEntry(singularEntry));
                }
                else if (entry is POPluralEntry plural)
                {
                    outputCatalog.Add(TransformPluralEntry(plural));
                }
                else
                {
                    throw new NotSupportedException("Unsupported PO entry type " + entry.GetType());
                }
            }

            return outputCatalog;
        }

        private POPluralEntry TransformPluralEntry(POPluralEntry plural)
        {
            var entry = new POPluralEntry(plural.Key) {Comments = plural.Comments};
            foreach (var source in plural)
            {
                entry.Add(Transform(source));
            }

            return entry;
        }

        private POSingularEntry TransformSingularEntry(POSingularEntry singular)
        {
            var entry = new POSingularEntry(singular.Key)
            {
                Translation = Transform(singular.Translation),
                Comments = singular.Comments
            };
            return entry;
        }
    }
}
