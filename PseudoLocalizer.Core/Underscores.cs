namespace PseudoLocalizer.Core
{
    /// <summary>
    /// A transform which replaces all characters with underscores. This class cannot be inherited.
    /// </summary>
    public sealed class Underscores : ITransformer
    {
        /// <summary>
        /// Gets the singleton instance of <see cref="Underscores"/>.
        /// </summary>
        public static Underscores Instance { get; } = new Underscores();

        /// <inheritdoc />
        public string Transform(string value)
            => new string('_', value.Length);
    }
}
