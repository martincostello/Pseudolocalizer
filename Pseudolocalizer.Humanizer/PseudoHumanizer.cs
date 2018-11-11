﻿using System;
using System.Globalization;
using Humanizer.Configuration;
using PseudoLocalizer.Core;

namespace PseudoLocalizer.Humanizer
{
    /// <summary>
    /// A class representing configuration for applying pseudo-localization for Humanzier.
    /// </summary>
    public class PseudoHumanizer : ITransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoHumanizer"/> class
        /// using the default base locale and pseudo-locale.
        /// </summary>
        public PseudoHumanizer()
            : this(DefaultBaseLocale)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoHumanizer"/> class
        /// using the specified base locale and the default pseudo-locale.
        /// </summary>
        /// <param name="baseLocale">The base locale culture to use to humanize values.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseLocale"/> is <see langword="null"/>.
        /// </exception>
        public PseudoHumanizer(CultureInfo baseLocale)
            : this(baseLocale, DefaultPseudoLocale)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoHumanizer"/> class
        /// using the specified base locale and the pseudo-locale.
        /// </summary>
        /// <param name="baseLocale">The base locale culture to use to humanize values.</param>
        /// <param name="pseudoLocale">The pseudo-locale culture to register usage for.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseLocale"/> or <paramref name="pseudoLocale"/> is <see langword="null"/>.
        /// </exception>
        public PseudoHumanizer(CultureInfo baseLocale, CultureInfo pseudoLocale)
        {
            BaseLocale = baseLocale ?? throw new ArgumentNullException(nameof(baseLocale));
            PseudoLocale = pseudoLocale ?? throw new ArgumentNullException(nameof(pseudoLocale));
        }

        /// <summary>
        /// Gets the default base locale culture. The locale is <c>en</c>.
        /// </summary>
        public static CultureInfo DefaultBaseLocale { get; } = new CultureInfo("en");

        /// <summary>
        /// Gets the default pseudo-locale culture. The locale is <c>qps-Ploc</c>.
        /// </summary>
        public static CultureInfo DefaultPseudoLocale { get; } = new CultureInfo("qps-Ploc");

        /// <summary>
        /// Gets the base locale to use to humanize values.
        /// </summary>
        public CultureInfo BaseLocale { get; }

        /// <summary>
        /// Gets the pseudo-locale culture to register usage for.
        /// </summary>
        public CultureInfo PseudoLocale { get; }

        /// <summary>
        /// Gets the locale code associated with this instance.
        /// </summary>
        public string LocaleCode => PseudoLocale.Name;

        /// <summary>
        /// Gets or sets a value indicating whether to use the default options.
        /// </summary>
        /// <remarks>
        /// The default options are <see cref="EnableExtraLength"/>,
        /// <see cref="EnableAccents"/> and <see cref="EnableBrackets"/>.
        /// </remarks>
        public bool UseDefaultOptions { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to add extra length to strings.
        /// </summary>
        public bool EnableExtraLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable use of accents for substitutions.
        /// </summary>
        public bool EnableAccents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add brackets to strings.
        /// </summary>
        public bool EnableBrackets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to mirror strings.
        /// </summary>
        public bool EnableMirror { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to replace spaces with underscores.
        /// </summary>
        public bool EnableUnderscores { get; set; }

        /// <inheritdoc />
        public virtual string Transform(string value)
        {
            string result = value;

            if (EnableExtraLength || UseDefaultOptions)
            {
                result = ExtraLength.Transform(result);
            }

            if (EnableAccents || UseDefaultOptions)
            {
                result = Accents.Transform(result);
            }

            if (EnableBrackets || UseDefaultOptions)
            {
                result = Brackets.Transform(result);
            }

            if (EnableMirror)
            {
                result = Mirror.Transform(result);
            }

            if (EnableUnderscores)
            {
                result = Underscores.Transform(result);
            }

            return result;
        }

        /// <summary>
        /// Registers the locale associated with this instance with the Humanizer <see cref="Configurator"/>
        /// to enable pseudo-localization for the locale specified by <see cref="LocaleCode"/>.
        /// </summary>
        public void Register()
        {
            Configurator.CollectionFormatters.Register(LocaleCode, new PseudoCollectionFormatter(this));
            Configurator.DateToOrdinalWordsConverters.Register(LocaleCode, new PseudoDateToOrdinalWordConverter(this));
            Configurator.Formatters.Register(LocaleCode, new PseudoFormatter(this));
            Configurator.NumberToWordsConverters.Register(LocaleCode, new PseudoNumberToWordsConverter(this));
            Configurator.Ordinalizers.Register(LocaleCode, new PseudoOrdinalizer(this));
        }
    }
}
