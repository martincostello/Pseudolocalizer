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
            // Slower path to no break formatting strings by removing their digits
            if (value.Contains('{') && value.Contains('}'))
            {
                char[] array = value.ToArray();

                for (int i = 0; i < array.Length; i++)
                {
                    char ch = array[i];

                    // Are we at the start of a potential placeholder (e.g. "{?...}")
                    if (ch == '{' && i < array.Length - 2)
                    {
                        int j = i;

                        // Consume all the digits
                        while (j < array.Length - 1 && char.IsDigit(array[++j]))
                        {
                        }

                        if (array[j] == ':')
                        {
                            // Consume all of any format specifier (e.g. "{0:yyyy}" for a DateTime)
                            while (j < array.Length - 1 && array[++j] != '}')
                            {
                            }
                        }

                        if (array[j] == '}')
                        {
                            i = j;
                            continue;
                        }
                    }

                    array[i] = '_';
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
