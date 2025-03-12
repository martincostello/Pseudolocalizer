using Humanizer.Configuration;
using Humanizer.Localisation.CollectionFormatters;
using PseudoLocalizer.Core;

namespace PseudoLocalizer.Humanizer
{
    /// <summary>
    /// A class representing an implementation of <see cref="ICollectionFormatter"/> that applies
    /// pseudo-localization using a specified <see cref="ITransformer"/>. This class cannot be inherited.
    /// </summary>
    public sealed class PseudoCollectionFormatter : ICollectionFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoCollectionFormatter"/> class.
        /// </summary>
        /// <param name="pseudoHumanizer">The <see cref="PseudoHumanizer"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pseudoHumanizer"/> is <see langword="null"/>.
        /// </exception>
        public PseudoCollectionFormatter(PseudoHumanizer pseudoHumanizer)
        {
            Transformer = pseudoHumanizer ?? throw new ArgumentNullException(nameof(pseudoHumanizer));
            Inner = Configurator.CollectionFormatters.ResolveForCulture(pseudoHumanizer.BaseLocale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoCollectionFormatter"/> class.
        /// </summary>
        /// <param name="transformer">The <see cref="ITransformer"/> to use.</param>
        /// <param name="inner">The inner <see cref="ICollectionFormatter"/> to use before applying transforms.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer"/> or <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public PseudoCollectionFormatter(ITransformer transformer, ICollectionFormatter inner)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets the inner <see cref="ICollectionFormatter"/> to use.
        /// </summary>
        private ICollectionFormatter Inner { get; }

        /// <summary>
        /// Gets the <see cref="ITransformer"/> to use.
        /// </summary>
        private ITransformer Transformer { get; }

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection)
            => Transformer.Transform(Inner.Humanize(collection));

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection, Func<T, string> objectFormatter)
            => Transformer.Transform(Inner.Humanize(collection, objectFormatter));

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection, Func<T, object> objectFormatter)
            => Transformer.Transform(Inner.Humanize(collection, objectFormatter));

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection, string separator)
            => Transformer.Transform(Inner.Humanize(collection, separator));

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection, Func<T, string> objectFormatter, string separator)
            => Transformer.Transform(Inner.Humanize(collection, objectFormatter, separator));

        /// <inheritdoc />
        public string Humanize<T>(IEnumerable<T> collection, Func<T, object> objectFormatter, string separator)
            => Transformer.Transform(Inner.Humanize(collection, objectFormatter, separator));
    }
}
