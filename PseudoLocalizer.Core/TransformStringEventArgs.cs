namespace PseudoLocalizer.Core
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="IProcessor.TransformString"/> event.
    /// </summary>
    public class TransformStringEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current value to transform.
        /// </summary>
        public string Value { get; set; }
    }
}
