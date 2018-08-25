namespace PseudoLocalizer.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines an interface for processing a stream to transform strings.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Event raised when a string value to be transformed is found.
        /// </summary>
        event EventHandler<TransformStringEventArgs> TransformString;

        /// <summary>
        /// Read from an input stream and write to an output stream.
        /// </summary>
        void Transform(Stream inputStream, Stream outputStream);
    }
}
