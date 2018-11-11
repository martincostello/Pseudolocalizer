namespace PseudoLocalizer.Core
{
    using System.Linq;

    /// <summary>
    /// A transform which reverses (mirrors) all strings. This class cannot be inherited.
    /// </summary>
    public sealed class Mirror : ITransformer
    {
        /// <summary>
        /// Gets the singleton instance of <see cref="Mirror"/>.
        /// </summary>
        public static Mirror Instance { get; } = new Mirror();

        /// <inheritdoc />
        public string Transform(string value)
            => new string(value.Reverse().ToArray());
    }
}
