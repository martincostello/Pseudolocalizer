using System.Linq;

namespace PseudoLocalizer.Core
{
    internal static class EscapeHelpers
    {
        internal static bool MayNeedEscaping(string value)
        {
            return (value.Contains('{') && value.Contains('}')) ||
                   (value.Contains('<') && value.Contains('>'));
        }

        internal static bool ShouldTransform(char[] array, char ch, ref int i)
        {
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
                    return false;
                }
            }
            else if (ch == '<' && i < array.Length - 2)
            {
                // Are we at the start of a potential HTML tag (e.g. "<a/>")
                int j = i;

                char next = array[i + 1];

                if ((next >= 'a' && next <= 'z') || (next >= 'A' && next <= 'Z') || next == '/')
                {
                    // Consume all of the tag
                    while (j < array.Length - 1 && array[++j] != '>')
                    {
                    }

                    if (array[j] == '>')
                    {
                        i = j;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
