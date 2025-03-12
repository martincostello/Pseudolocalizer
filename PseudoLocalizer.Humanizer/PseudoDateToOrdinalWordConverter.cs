using Humanizer;
using Humanizer.Configuration;
using Humanizer.Localisation.DateToOrdinalWords;
using PseudoLocalizer.Core;

namespace PseudoLocalizer.Humanizer
{
    /// <summary>
    /// A class representing an implementation of <see cref="IDateToOrdinalWordConverter"/> that applies
    /// pseudo-localization using a specified <see cref="ITransformer"/>. This class cannot be inherited.
    /// </summary>
    public sealed class PseudoDateToOrdinalWordConverter : IDateToOrdinalWordConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoDateToOrdinalWordConverter"/> class.
        /// </summary>
        /// <param name="pseudoHumanizer">The <see cref="PseudoHumanizer"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pseudoHumanizer"/> is <see langword="null"/>.
        /// </exception>
        public PseudoDateToOrdinalWordConverter(PseudoHumanizer pseudoHumanizer)
        {
            Transformer = pseudoHumanizer ?? throw new ArgumentNullException(nameof(pseudoHumanizer));
            Inner = Configurator.DateToOrdinalWordsConverters.ResolveForCulture(pseudoHumanizer.BaseLocale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoDateToOrdinalWordConverter"/> class.
        /// </summary>
        /// <param name="transformer">The <see cref="ITransformer"/> to use.</param>
        /// <param name="inner">The inner <see cref="IDateToOrdinalWordConverter"/> to use before applying transforms.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer"/> or <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public PseudoDateToOrdinalWordConverter(ITransformer transformer, IDateToOrdinalWordConverter inner)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets the inner <see cref="IDateToOrdinalWordConverter"/> to use.
        /// </summary>
        private IDateToOrdinalWordConverter Inner { get; }

        /// <summary>
        /// Gets the <see cref="ITransformer"/> to use.
        /// </summary>
        private ITransformer Transformer { get; }

        /// <inheritdoc />
        public string Convert(DateTime date)
            => Transformer.Transform(Inner.Convert(date));

        /// <inheritdoc />
        public string Convert(DateTime date, GrammaticalCase grammaticalCase)
            => Transformer.Transform(Inner.Convert(date, grammaticalCase));
    }
}
