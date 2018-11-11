using System;
using System.Collections.Generic;
using System.Linq;

namespace PseudoLocalizer.Core
{
    /// <summary>
    /// A class representing a chain of transformations. This class cannot be inherited.
    /// </summary>
    public sealed class Pipeline : ITransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="transformers">The <see cref="ITransformer"/> implementations to chain together.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformers"/> is <see langword="null"/>.
        /// </exception>
        public Pipeline(params ITransformer[] transformers)
            : this(transformers as IEnumerable<ITransformer>)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="transformers">The <see cref="ITransformer"/> implementations to chain together.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformers"/> is <see langword="null"/>.
        /// </exception>
        public Pipeline(IEnumerable<ITransformer> transformers)
        {
            if (transformers == null)
            {
                throw new ArgumentNullException(nameof(transformers));
            }

            Impl = transformers
                .Select((p) => new Func<string, string>(p.Transform))
                .DefaultIfEmpty(NoOp)
                .Aggregate((x, y) => Pipe(x, y));
        }

        private Func<string, string> Impl { get; }

        /// <inheritdoc />
        public string Transform(string value)
            => Impl(value);

        private static Func<string, string> Pipe(Func<string, string> first, Func<string, string> second)
            => (value) => second(first(value));

        private static string NoOp(string value)
            => value;
    }
}
