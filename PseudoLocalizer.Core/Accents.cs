namespace PseudoLocalizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A transform which adds accents to all letters. This class cannot be inherited.
    /// </summary>
    public sealed class Accents : ITransformer
    {
        // Character mappings gratefully borrowed from the Google pseudolocalization-tool.
        private readonly Dictionary<char, char> _replacements = new Dictionary<char, char>()
        {
            { ' ', '\u2003' },
            { '!', '\u00a1' },
            { '"', '\u2033' },
            { '#', '\u266f' },
            { '$', '\u20ac' },
            { '%', '\u2030' },
            { '&', '\u214b' },
            { '\'', '\u00b4' },
            { '*', '\u204e' },
            { '+', '\u207a' },
            { ',', '\u060c' },
            { '-', '\u2010' },
            { '.', '\u00b7' },
            { '/', '\u2044' },
            { '0', '\u24ea' },
            { '1', '\u2460' },
            { '2', '\u2461' },
            { '3', '\u2462' },
            { '4', '\u2463' },
            { '5', '\u2464' },
            { '6', '\u2465' },
            { '7', '\u2466' },
            { '8', '\u2467' },
            { '9', '\u2468' },
            { ':', '\u2236' },
            { ';', '\u204f' },
            { '<', '\u2264' },
            { '=', '\u2242' },
            { '>', '\u2265' },
            { '?', '\u00bf' },
            { '@', '\u055e' },
            { 'A', '\u00c5' },
            { 'B', '\u0181' },
            { 'C', '\u00c7' },
            { 'D', '\u00d0' },
            { 'E', '\u00c9' },
            { 'F', '\u0191' },
            { 'G', '\u011c' },
            { 'H', '\u0124' },
            { 'I', '\u00ce' },
            { 'J', '\u0134' },
            { 'K', '\u0136' },
            { 'L', '\u013b' },
            { 'M', '\u1e40' },
            { 'N', '\u00d1' },
            { 'O', '\u00d6' },
            { 'P', '\u00de' },
            { 'Q', '\u01ea' },
            { 'R', '\u0154' },
            { 'S', '\u0160' },
            { 'T', '\u0162' },
            { 'U', '\u00db' },
            { 'V', '\u1e7c' },
            { 'W', '\u0174' },
            { 'X', '\u1e8a' },
            { 'Y', '\u00dd' },
            { 'Z', '\u017d' },
            { '[', '\u2045' },
            { '\\', '\u2216' },
            { ']', '\u2046' },
            { '^', '\u02c4' },
            { '_', '\u203f' },
            { '`', '\u2035' },
            { 'a', '\u00e5' },
            { 'b', '\u0180' },
            { 'c', '\u00e7' },
            { 'd', '\u00f0' },
            { 'e', '\u00e9' },
            { 'f', '\u0192' },
            { 'g', '\u011d' },
            { 'h', '\u0125' },
            { 'i', '\u00ee' },
            { 'j', '\u0135' },
            { 'k', '\u0137' },
            { 'l', '\u013c' },
            { 'm', '\u0271' },
            { 'n', '\u00f1' },
            { 'o', '\u00f6' },
            { 'p', '\u00fe' },
            { 'q', '\u01eb' },
            { 'r', '\u0155' },
            { 's', '\u0161' },
            { 't', '\u0163' },
            { 'u', '\u00fb' },
            { 'v', '\u1e7d' },
            { 'w', '\u0175' },
            { 'x', '\u1e8b' },
            { 'y', '\u00fd' },
            { 'z', '\u017e' },
            { '|', '\u00a6' },
            { '~', '\u02de' },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Accents"/> class.
        /// </summary>
        public Accents()
            : this(isReadOnly: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Accents"/> class.
        /// </summary>
        /// <param name="isReadOnly">A value indicating whether the replacements are read-only.</param>
        private Accents(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="Accents"/>.
        /// </summary>
        public static Accents Instance { get; } = new Accents(isReadOnly: true);

        /// <summary>
        /// Gets a value indicating whether the replacements are read-only.
        /// </summary>
        private bool IsReadOnly { get; }

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

                    array[i] = Transform(ch);
                }

                return new string(array);
            }
            else
            {
                return new string(
                    value.ToCharArray()
                        .Select(x => Transform(x))
                        .ToArray());
            }
        }

        /// <summary>
        /// Adds a new replacement character for the specified value.
        /// </summary>
        /// <param name="original">The character to add a replacement for.</param>
        /// <param name="substitute">The character to use as a replacement for <paramref name="original"/>.</param>
        /// <returns>
        /// The current instance of <see cref="Accents"/>.
        /// </returns>
        public Accents AddReplacement(char original, char substitute)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException($"This instance of {nameof(Accents)} is read-only.");
            }

            _replacements[original] = substitute;
            return this;
        }

        private char Transform(char value)
        {
            if (_replacements.TryGetValue(value, out char x))
            {
                return x;
            }
            else
            {
                return value;
            }
        }
    }
}
