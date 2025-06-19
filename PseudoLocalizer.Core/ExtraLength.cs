namespace PseudoLocalizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A transform which makes all words approximately one third longer. This class cannot be inherited.
    /// </summary>
    public sealed class ExtraLength : ITransformer
    {
        private readonly bool _isReadOnly;
        private char _lengthenChar;

        /// <summary>
        /// Gets the singleton instance of <see cref="ExtraLength"/>.
        /// </summary>
        public static ExtraLength Instance { get; } = new ExtraLength(isReadOnly: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraLength"/> class.
        /// </summary>
        public ExtraLength()
            : this(isReadOnly: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraLength"/> class.
        /// </summary>
        /// <param name="isReadOnly">Whether the properties of the instance are read-only.</param>
        private ExtraLength(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
            _lengthenChar = 'x';
        }

        /// <summary>
        /// Gets or sets the character used to lengthen strings.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the instance is equal to the value of <see cref="Instance"/>.
        /// </exception>
        public char LengthenCharacter
        {
            get => _lengthenChar;
            set
            {
                if (_isReadOnly)
                {
                    throw new InvalidOperationException("This property's value cannot be changed for the default instance.");
                }

                _lengthenChar = value;
            }
        }

        /// <inheritdoc />
        public string Transform(string value)
        {
            IEnumerable<string> words;

            // Slower path to not break formatting strings by removing their digits or break HTML tags
            if (EscapeHelpers.MayNeedEscaping(value))
            {
                char[] src = [.. value];

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

        private string Lengthen(StringBuilder builder)
            => string.Join(" ", builder.ToString().Split(' ').Select(Lengthen));

        private string Lengthen(string word)
        {
            var count = (word.Length + 2) / 3;
            return word + new string(_lengthenChar, count);
        }
    }
}
