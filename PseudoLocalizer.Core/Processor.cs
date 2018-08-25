namespace PseudoLocalizer.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// Applies transforms to string values.
    /// </summary>
    public abstract class Processor : IProcessor
    {
        /// <inheritdoc />
        public event EventHandler<TransformStringEventArgs> TransformString;

        /// <inheritdoc />
        public abstract void Transform(Stream inputStream, Stream outputStream);

        /// <summary>
        /// Applies a transformation to the specified string value.
        /// </summary>
        /// <param name="value">The string to transform.</param>
        /// <returns>
        /// The transformation of <paramref name="value"/>.
        /// </returns>
        protected virtual string Transform(string value)
        {
            var args = new TransformStringEventArgs { Value = value };

            OnTransformString(args);

            return args.Value;
        }

        /// <summary>
        /// Raises the <see cref="TransformString"/> event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnTransformString(TransformStringEventArgs args)
            => TransformString?.Invoke(this, args);
    }
}
