namespace PseudoLocalizer.Core
{
    /// <summary>
    /// Defines a method that transforms a <see cref="string"/>.
    /// </summary>
    public interface ITransformer
    {
        /// <summary>
        /// Transforms the specified value.
        /// </summary>
        /// <param name="value">The string to transform.</param>
        /// <returns>
        /// The value of <paramref name="value"/> after a transformation is applied.
        /// </returns>
        string Transform(string value);
    }
}
