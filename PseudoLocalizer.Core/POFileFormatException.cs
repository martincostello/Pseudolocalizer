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
        /// Initializes a new instance of the <see cref="POFileFormatException"/> class.
        /// </summary>
        public POFileFormatException()
            : this(null, Array.Empty<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public POFileFormatException(string message)
            : this(message, Array.Empty<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="diagnosticMessages">The diagnostic messages that describe the file format errors.</param>
        public POFileFormatException(ICollection<string> diagnosticMessages)
            : this(null, diagnosticMessages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="diagnosticMessages">The diagnostic messages that describe the file format errors.</param>
        public POFileFormatException(string message, ICollection<string> diagnosticMessages)
            : base(GetMessage(message, diagnosticMessages))
        {
            DiagnosticMessages = diagnosticMessages ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POFileFormatException"/> class with
        /// the specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public POFileFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
            DiagnosticMessages = Array.Empty<string>();
        }

        /// <summary>
        /// Gets the diagnostic messages that describe the file format errors, if any.
        /// </summary>
        /// <value>
        /// The diagnostic messages that describe the file format errors.
        /// </value>
        public ICollection<string> DiagnosticMessages { get; }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="diagnosticMessages">The diagnostic messages.</param>
        /// <returns>The error message.</returns>
        private static string GetMessage(string message, ICollection<string> diagnosticMessages)
        {
            var builder = new StringBuilder();

            if (string.IsNullOrEmpty(message))
            {
                if (diagnosticMessages == null || diagnosticMessages.Count < 1)
                {
                    builder.AppendLine("The input file was not in a valid format");
                }
            }
            else
            {
                builder.AppendLine(message);
            }
            
            if (diagnosticMessages != null)
            {
                foreach (string diagnostic in diagnosticMessages)
                {
                    builder.AppendLine(diagnostic);
                }
            }

            return builder.ToString();
        }
    }
}
