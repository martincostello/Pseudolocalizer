using System;
using System.Text;
using Karambolo.PO;

namespace PseudoLocalizer.Core
{
    /// <summary>
    /// The exception that is thrown when a PO file format error is detected.
    /// </summary>
    [Serializable]
#pragma warning disable CS3003 // Type is not CLS-compliant
    public class POFileFormatException : Exception
    {
        /// <summary>
        /// Gets the diagnostics information about the file format error(s).
        /// </summary>
        /// <value>
        /// The diagnostics.
        /// </value>
        public IDiagnostics Diagnostics { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException"/> class.
        /// </summary>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public POFileFormatException(IDiagnostics diagnostics)
            : base(FormatDiagnostics(diagnostics))
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            Diagnostics = diagnostics;
        }

        /// <summary>
        /// Formats the specified diagnostics collection as a user-friendly message.
        /// </summary>
        /// <param name="diagnostics">The diagnostics collection.</param>
        /// <returns>A string containing a user-friendly message.</returns>
        private static string FormatDiagnostics(IDiagnostics diagnostics)
        {
            if (diagnostics == null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            var sb = new StringBuilder();
            foreach (var diagnostic in diagnostics)
            {
                sb.AppendFormat(diagnostic.Code, diagnostic.Args);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
