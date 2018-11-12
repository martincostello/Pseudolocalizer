using System.Linq;

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
        {
            // Slower path to not break formatting strings by removing their digits or break HTML tags
            if ((value.Contains('{') && value.Contains('}')) ||
                (value.Contains('<') && value.Contains('>') && value.Contains('/')))
            {
                char[] array = value.ToArray();

                for (int i = 0; i < array.Length; i++)
                {
                    char ch = array[i];

                    if (EscapeHelpers.ShouldTransform(array, ch, ref i))
                    {
                        array[i] = '_';
                    }
                }

                return new string(array);
            }
            else
            {
                return new string('_', value.Length);
            }
        }
    }
}
