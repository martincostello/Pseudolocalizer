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

                var startTag = new StringBuilder();
                var builder = new StringBuilder(value.Length);
                var current = new StringBuilder(value.Length);

                bool insideTag = false;

                for (int i = 0; i < value.Length; i++)
                {
                    char ch = value[i];
                    int indexBefore = i;

                    if (EscapeHelpers.ShouldTransform(src, ch, ref i, out var textType))
                    {
                        if (insideTag)
                        {
                            current.Insert(0, FlipIfSpecial(ch));
                        }
                        else
                        {
                            builder.Insert(0, FlipIfSpecial(ch));
                        }
                    }
                    else
                    {
                        // Transformation should be skipped due to formatting placeholder or HTML
                        if (textType == EscapeHelpers.TextType.Format)
                        {
                            // Add the skipped range
                            if (insideTag)
                            {
                                for (int j = indexBefore, k = 0; j < i + 1; j++, k++)
                                {
                                    current.Insert(k, value[j]);
                                }
                            }
                            else
                            {
                                for (int j = indexBefore; j < i + 1; j++)
                                {
                                    current.Append(value[j]);
                                }
                            }

                            if (current.Length > 0 && !insideTag)
                            {
                                builder.Insert(0, current.ToString());
                                current.Clear();
                            }
                        }
                        else if (textType == EscapeHelpers.TextType.HtmlSelfClosing)
                        {
                            // Add the skipped range
                            for (int j = indexBefore, k = 0; j < i + 1; j++, k++)
                            {
                                builder.Insert(k, value[j]);
                            }
                        }
                        else if (textType == EscapeHelpers.TextType.HtmlStart)
                        {
                            for (int j = indexBefore; j < i + 1; j++)
                            {
                                startTag.Append(value[j]);
                            }

                            insideTag = true;
                        }
                        else if (textType == EscapeHelpers.TextType.HtmlEnd)
                        {
                            for (int j = startTag.Length - 1; j > -1; j--)
                            {
                                current.Insert(0, startTag[j]);
                            }

                            startTag.Clear();

                            // Add the skipped range
                            for (int j = indexBefore; j < i + 1; j++)
                            {
                                current.Append(value[j]);
                            }

                            if (current.Length > 0)
                            {
                                for (int j = current.Length - 1; j > -1; j--)
                                {
                                    builder.Insert(0, current[j]);
                                }

                                current.Clear();
                            }

                            insideTag = false;
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
