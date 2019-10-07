using System;
using System.Collections.Generic;
using System.Text;

namespace PseudoLocalizer.Core
{
    /// <summary>
    ///     The exception that is thrown when a PO file format error is detected.
    /// </summary>
    public class POFileFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException"/> class.
        /// </summary>
        public POFileFormatException()
            : base(GetMessage(null, null))
        {
            
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public POFileFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="diagnosticMessages">The diagnostic messages that describe the file format errors.</param>
        public POFileFormatException(string message, ICollection<string> diagnosticMessages)
            : base(GetMessage(message, diagnosticMessages))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="diagnosticMessages">The diagnostic messages that describe the file format errors.</param>
        public POFileFormatException(ICollection<string> diagnosticMessages)
            : base(GetMessage(null, diagnosticMessages))
        {
            DiagnosticMessages = diagnosticMessages;
        }

        /// <summary>
        ///     Gets the diagnostic messages that describe the file format errors.
        /// </summary>
        /// <value>
        ///     The diagnostic messages that describe the file format errors.
        /// </value>
        public ICollection<string> DiagnosticMessages { get; }

        /// <summary>
        ///     Gets the exception message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="diagnosticMessages">The diagnostic messages.</param>
        /// <returns>The error message.</returns>
        private static string GetMessage(string message, ICollection<string> diagnosticMessages)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(message))
            {
                if (diagnosticMessages == null)
                {
                    sb.AppendLine("The input file was not in a valid format");
                }
            }
            else
            {
                sb.AppendLine(message);
            }
            
            if (diagnosticMessages != null)
            {
                foreach (var msg in diagnosticMessages)
                {
                    sb.AppendLine(msg);
                }
            }

            return sb.ToString();
        }
    }
}
