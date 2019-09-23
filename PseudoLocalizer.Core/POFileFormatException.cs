using System;
using System.Collections.Generic;
using System.Text;

namespace PseudoLocalizer.Core
{
    /// <summary>
    /// The exception that is thrown when a PO file format error is detected.
    /// </summary>
    public class POFileFormatException : Exception
    {
        /// <summary>
        /// Gets the diagnostics messages that describe the file format errors.
        /// </summary>
        /// <value>
        /// The diagnostics messages that describe the file format errors.
        /// </value>
        public ICollection<string> DiagnosticsMessages { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="diagnosticsMessages">The diagnostics messages that describe the file format errors.</param>
        public POFileFormatException(ICollection<string> diagnosticsMessages)
            : base(GetMessage(diagnosticsMessages))
        {
            DiagnosticsMessages = diagnosticsMessages;
        }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <param name="diagnosticMessages">The diagnostic messages.</param>
        /// <returns>A string that represents the diagnostic messages.</returns>
        private static string GetMessage(ICollection<string> diagnosticMessages)
        {
            var sb = new StringBuilder();
            foreach (var message in diagnosticMessages)
            {
                sb.AppendLine(message);
            }

            return sb.ToString();
        }
    }
}
