using System.ComponentModel;

namespace PseudoLocalizer.Core
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ITransformer"/> interface.  This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ITransformerExtensions
    {
        /// <summary>
        /// Applies a string transformation to the specified value.
        /// </summary>
        /// <param name="transformer">The <see cref="ITransformer"/> to use.</param>
        /// <param name="args">The <see cref="TransformStringEventArgs"/> to apply the transform to.</param>
        public static void Apply(this ITransformer transformer, TransformStringEventArgs args)
            => args.Value = transformer.Transform(args.Value);
    }
}
