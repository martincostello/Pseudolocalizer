using System;
using Humanizer.Configuration;
using Humanizer.Localisation;
using Humanizer.Localisation.Formatters;
using PseudoLocalizer.Core;

namespace PseudoLocalizer.Humanizer
{
    /// <summary>
    /// A class representing an implementation of <see cref="IFormatter"/> that applies
    /// pseudo-localization using a specified <see cref="ITransformer"/>. This class cannot be inherited.
    /// </summary>
    public sealed class PseudoFormatter : IFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoFormatter"/> class.
        /// </summary>
        /// <param name="pseudoHumanizer">The <see cref="PseudoHumanizer"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pseudoHumanizer"/> is <see langword="null"/>.
        /// </exception>
        public PseudoFormatter(PseudoHumanizer pseudoHumanizer)
        {
            Transformer = pseudoHumanizer ?? throw new ArgumentNullException(nameof(pseudoHumanizer));
            Inner = Configurator.Formatters.ResolveForCulture(pseudoHumanizer.BaseLocale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoFormatter"/> class.
        /// </summary>
        /// <param name="transformer">The <see cref="ITransformer"/> to use.</param>
        /// <param name="inner">The inner <see cref="IFormatter"/> to use before applying transforms.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer"/> or <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public PseudoFormatter(ITransformer transformer, IFormatter inner)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets the inner <see cref="IFormatter"/> to use.
        /// </summary>
        private IFormatter Inner { get; }

        /// <summary>
        /// Gets the <see cref="ITransformer"/> to use.
        /// </summary>
        private ITransformer Transformer { get; }

        /// <inheritdoc />
        public string DateHumanize(TimeUnit timeUnit, Tense timeUnitTense, int unit)
            => Transformer.Transform(Inner.DateHumanize(timeUnit, timeUnitTense, unit));

        /// <inheritdoc />
        public string DateHumanize_Never()
            => Transformer.Transform(Inner.DateHumanize_Never());

        /// <inheritdoc />
        public string DateHumanize_Now()
            => Transformer.Transform(Inner.DateHumanize_Now());

        /// <inheritdoc />
        public string TimeSpanHumanize(TimeUnit timeUnit, int unit, bool toWords = false)
            => Transformer.Transform(Inner.TimeSpanHumanize(timeUnit, unit, toWords));

        /// <inheritdoc />
        public string TimeSpanHumanize_Zero()
            => Transformer.Transform(Inner.TimeSpanHumanize_Zero());
    }
}
