namespace PseudoLocalizer.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
            IEnumerable<string> words;

            // Slower path to not break formatting strings by removing their digits or break HTML tags
            if (EscapeHelpers.MayNeedEscaping(value))
            {
                char[] src = value.ToArray();

                var builder = new StringBuilder(value.Length * 2);
                var current = new StringBuilder(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    char ch = value[i];
                    int indexBefore = i;

                    if (EscapeHelpers.ShouldTransform(src, ch, ref i))
                    {
                        current.Append(ch);
                    }
                    else
                    {
                        // Transformation should be skipped due to formatting placeholder or HTML
                        if (current.Length > 0)
                        {
                            builder.Append(Lengthen(current));
                            current.Clear();
                        }

                        // Add the skipped range
                        for (int j = indexBefore; j < i + 1; j++)
                        {
                            builder.Append(value[j]);
                        }
                    }
                }

                if (current.Length > 0)
                {
                    builder.Append(Lengthen(current));
                }

                return builder.ToString();
            }
            else
            {
                words = value.Split(' ').Select(Lengthen);
            }

            return string.Join(" ", words);
        }

        private static string Lengthen(StringBuilder builder)
            => string.Join(" ", builder.ToString().Split(' ').Select(Lengthen));

        private static string Lengthen(string word)
        {
            var count = (word.Length + 2) / 3;
            return word + new string('x', count);
        }
    }
}
