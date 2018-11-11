namespace PseudoLocalizer.Core
{
    /// <summary>
    /// A transform which adds brackets to all strings. This class cannot be inherited.
    /// </summary>
    public sealed class Brackets : ITransformer
    {
        /// <summary>
        /// Gets the singleton instance of <see cref="Brackets"/>.
        /// </summary>
        public static Brackets Instance { get; } = new Brackets();

        /// <inheritdoc />
        public string Transform(string value)
            => "[" + value + "]";
    }
}
