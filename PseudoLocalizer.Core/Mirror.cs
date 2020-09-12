namespace PseudoLocalizer.Core
{
    using System.Linq;
    using System.Text;

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
        {
            StringBuilder mirrored;

            // Slower path to not break formatting strings by flipping placeholders or breaking HTML
            if (EscapeHelpers.MayNeedEscaping(value))
            {
                char[] src = value.ToArray();

                var builder = new StringBuilder(value.Length);
                var current = new StringBuilder(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    char ch = value[i];
                    int indexBefore = i;

                    if (EscapeHelpers.ShouldTransform(src, ch, ref i))
                    {
                        current.Insert(0, FlipIfSpecial(ch));
                    }
                    else
                    {
                        // Transformation should be skipped due to formatting placeholder or HTML
                        // Add the skipped range
                        for (int j = indexBefore; j < i + 1; j++)
                        {
                            builder.Append(value[j]);
                        }

                        if (current.Length > 0)
                        {
                            builder.Append(current.ToString());
                            current.Clear();
                        }
                    }
                }

                if (current.Length > 0)
                {
                    builder.Insert(0, current.ToString());
                }

                mirrored = builder;
            }
            else
            {
                mirrored = new StringBuilder();

                foreach (char ch in value.Reverse())
                {
                    mirrored.Append(FlipIfSpecial(ch));
                }
            }

            return mirrored.ToString();
        }

        private static char FlipIfSpecial(char ch)
        {
            return ch switch
            {
                '[' => ']',
                ']' => '[',
                '(' => ')',
                ')' => '(',
                '{' => '}',
                '}' => '{',
                _ => ch,
            };
        }
    }
}
