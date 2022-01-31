using System;
using Humanizer;
using Humanizer.Configuration;
using Humanizer.Localisation.Ordinalizers;
using PseudoLocalizer.Core;

namespace PseudoLocalizer.Humanizer
{
    /// <summary>
    /// A class representing an implementation of <see cref="IOrdinalizer"/> that applies
    /// pseudo-localization using a specified <see cref="ITransformer"/>. This class cannot be inherited.
    /// </summary>
    public sealed class PseudoOrdinalizer : IOrdinalizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoOrdinalizer"/> class.
        /// </summary>
        /// <param name="pseudoHumanizer">The <see cref="PseudoHumanizer"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pseudoHumanizer"/> is <see langword="null"/>.
        /// </exception>
        public PseudoOrdinalizer(PseudoHumanizer pseudoHumanizer)
        {
            Transformer = pseudoHumanizer ?? throw new ArgumentNullException(nameof(pseudoHumanizer));
            Inner = Configurator.Ordinalizers.ResolveForCulture(pseudoHumanizer.BaseLocale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoOrdinalizer"/> class.
        /// </summary>
        /// <param name="transformer">The <see cref="ITransformer"/> to use.</param>
        /// <param name="inner">The inner <see cref="IOrdinalizer"/> to use before applying transforms.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer"/> or <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public PseudoOrdinalizer(ITransformer transformer, IOrdinalizer inner)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets the inner <see cref="IOrdinalizer"/> to use.
        /// </summary>
        private IOrdinalizer Inner { get; }

        /// <summary>
        /// Gets the <see cref="ITransformer"/> to use.
        /// </summary>
        private ITransformer Transformer { get; }

        /// <inheritdoc />
        public string Convert(int number, string numberString)
            => Transformer.Transform(Inner.Convert(number, numberString));

        /// <inheritdoc />
        public string Convert(int number, string numberString, WordForm wordForm)
            => Transformer.Transform(Inner.Convert(number, numberString, wordForm));

        /// <inheritdoc />
        public string Convert(int number, string numberString, GrammaticalGender gender)
            => Transformer.Transform(Inner.Convert(number, numberString, gender));

        /// <inheritdoc />
        public string Convert(int number, string numberString, GrammaticalGender gender, WordForm wordForm)
            => Transformer.Transform(Inner.Convert(number, numberString, gender, wordForm));
    }
}
