namespace PseudoLocalizer.Core
{
    using System.Linq;

    /// <summary>
    /// A transform which makes all words approximately one third longer. This class cannot be inherited.
    /// </summary>
    public sealed class ExtraLength : ITransformer
    {
        /// <summary>
        /// Gets the singleton instance of <see cref="ExtraLength"/>.
        /// </summary>
        public static ExtraLength Instance { get; } = new ExtraLength();

        /// <inheritdoc />
        public string Transform(string value)
        {
            return string.Join(
                " ", 
                value.Split(' ')
                    .Select(word => Lengthen(word)));
        }

        private static string Lengthen(string word)
        {
            var count = (word.Length + 2) / 3;
            return word + new string('x', count);
        }
    }
}
