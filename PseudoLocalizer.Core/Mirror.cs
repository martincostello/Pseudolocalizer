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
            // Slower path to not break formatting strings by flipping placeholders or breaking HTML
            => EscapeHelpers.MayNeedEscaping(value) ? MirrorSlow(value) : MirrorFast(value);

        private static string MirrorFast(string value)
        {
            var result = new StringBuilder(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                result.Insert(0, FlipIfSpecial(value[i]));
            }

            return result.ToString();
        }

        private static string MirrorSlow(string value)
        {
            char[] src = [.. value];

            var result = new StringBuilder(value.Length);
            var current = new StringBuilder(value.Length);
            var startTag = new StringBuilder();

            bool isInsideTag = false;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                int indexBefore = i;

                if (EscapeHelpers.ShouldTransform(src, ch, ref i, out var textType))
                {
                    if (isInsideTag)
                    {
                        // Build the reversed inner text of the HTML tag
                        current.Insert(0, FlipIfSpecial(ch));
                    }
                    else
                    {
                        result.Insert(0, FlipIfSpecial(ch));
                    }
                }
                else
                {
                    // Transformation should be skipped due to formatting placeholder or HTML
                    if (textType == EscapeHelpers.TextType.Format)
                    {
                        if (isInsideTag)
                        {
                            // Add the escaped string to the start of the
                            // inner text for the tag in left-to-right order
                            for (int j = indexBefore, k = 0; j < i + 1; j++, k++)
                            {
                                current.Insert(k, value[j]);
                            }
                        }
                        else
                        {
                            // Add the format string to the end in left-to-right order
                            for (int j = indexBefore; j < i + 1; j++)
                            {
                                current.Append(value[j]);
                            }

                            // Flush to the left-hand-side of the result
                            if (current.Length > 0)
                            {
                                result.Insert(0, current.ToString());
                                current.Clear();
                            }
                        }
                    }
                    else if (textType == EscapeHelpers.TextType.HtmlSelfClosing)
                    {
                        // Add the escaped string to the start
                        // of the result in left-to-right order
                        for (int j = indexBefore, k = 0; j < i + 1; j++, k++)
                        {
                            result.Insert(k, value[j]);
                        }
                    }
                    else if (textType == EscapeHelpers.TextType.HtmlStart)
                    {
                        // Store the start tag for use once it's closed
                        isInsideTag = true;

                        for (int j = indexBefore; j < i + 1; j++)
                        {
                            startTag.Append(value[j]);
                        }
                    }
                    else if (textType == EscapeHelpers.TextType.HtmlEnd)
                    {
                        // Add the tag start as a prefix to the inner text
                        current.Insert(0, startTag.ToString());
                        startTag.Clear();

                        // Add the tag end as a suffix to the inner text
                        for (int j = indexBefore; j < i + 1; j++)
                        {
                            current.Append(value[j]);
                        }

                        // Flush the tag to the result
                        if (current.Length > 0)
                        {
                            for (int j = current.Length - 1; j > -1; j--)
                            {
                                result.Insert(0, current[j]);
                            }

                            current.Clear();
                        }

                        isInsideTag = false;
                    }
                }
            }

            // Flush any remaining characters
            if (current.Length > 0)
            {
                result.Insert(0, current.ToString());
            }

            return result.ToString();
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
