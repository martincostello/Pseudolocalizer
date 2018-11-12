namespace PseudoLocalizer.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Applies transforms to string values in Xlf resource files.
    /// </summary>
    public sealed class XlfProcessor : XmlProcessor
    {
        private readonly IDictionary<XmlDocument, Version> _versionCache = new ConcurrentDictionary<XmlDocument, Version>();

        /// <summary>
        /// Initializes a new instance of the <see cref="XlfProcessor"/> class.
        /// </summary>
        public XlfProcessor()
            : this("qps-Ploc")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XlfProcessor"/> class.
        /// </summary>
        /// <param name="culture">The name of the culture to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="culture"/> is <see langword="null"/>.
        /// </exception>
        public XlfProcessor(string culture)
        {
            Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        }

        /// <summary>
        /// Gets the culture code associated with the processor.
        /// </summary>
        public string Culture { get; }

        /// <inheritdoc />
        protected override void OnProcessed(XmlDocument document, XmlNamespaceManager nsmgr, bool modified)
        {
            if (!modified)
            {
                return;
            }

            if (!nsmgr.HasNamespace("x"))
            {
                nsmgr.AddNamespace("x", document.DocumentElement.NamespaceURI);
            }

            var version = GetXliffVersion(document);

            string xpath;
            string name;

            if (version.Major == 1)
            {
                xpath = "/x:xliff/x:file";
                name = "target-language";
            }
            else if (version.Major == 2)
            {
                xpath = "/x:xliff";
                name = "trgLang";
            }
            else
            {
                throw new NotSupportedException($"XLIFF version {version} is not supported.");
            }

            var file = document.SelectSingleNode(xpath, nsmgr);
            var attribute = file.Attributes[name];

            if (attribute != null)
            {
                attribute.Value = Culture;
            }
        }

        /// <inheritdoc />
        protected override XmlNodeList SelectNodes(XmlDocument document, XmlNamespaceManager nsmgr)
        {
            if (!nsmgr.HasNamespace("x"))
            {
                nsmgr.AddNamespace("x", document.DocumentElement.NamespaceURI);
            }

            var version = GetXliffVersion(document);

            string xpath;

            if (version.Major == 1)
            {
                xpath = "/x:xliff/x:file/x:body/x:trans-unit";
            }
            else if (version.Major == 2)
            {
                xpath = "/x:xliff/x:file/x:unit";
            }
            else
            {
                throw new NotSupportedException($"XLIFF version {version} is not supported.");
            }

            return document.SelectNodes(xpath, nsmgr);
        }

        /// <inheritdoc />
        protected override bool Visit(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var version = GetXliffVersion(node.OwnerDocument);

            if (version.Major == 1)
            {
                return Visit10(node, nsmgr);
            }
            else if (version.Major == 2)
            {
                return Visit20(node, nsmgr);
            }
            else
            {
                throw new NotSupportedException($"XLIFF version {version} is not supported.");
            }
        }

        private Version GetXliffVersion(XmlDocument document)
        {
            if (_versionCache.TryGetValue(document, out Version version))
            {
                return version;
            }

            var versionString = document.DocumentElement.Attributes["version"]?.Value ?? string.Empty;

            if (!Version.TryParse(versionString, out version))
            {
                version = new Version(1, 0);
            }

            _versionCache[document] = version;

            return version;
        }

        private bool Visit10(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var document = node.OwnerDocument;
            var rootNamespace = document.DocumentElement.NamespaceURI;
            var source = node.SelectSingleNode("x:source", nsmgr);

            bool modified = false;

            if (source != null && source.NodeType == XmlNodeType.Element)
            {
                var original = source.InnerText;
                var transformed = Transform(original);

                if (transformed != original)
                {
                    var target = node.SelectSingleNode("x:target", nsmgr);

                    if (target == null)
                    {
                        target = document.CreateElement("target", rootNamespace);
                        node.AppendChild(target);
                    }

                    var state = target.Attributes["state"];

                    if (state != null)
                    {
                        state.Value = "translated";
                    }

                    target.InnerText = transformed;
                    modified = true;
                }
            }

            return modified;
        }

        private bool Visit20(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var document = node.OwnerDocument;
            var rootNamespace = document.DocumentElement.NamespaceURI;
            var source = node.SelectSingleNode("x:segment/x:source", nsmgr);

            bool modified = false;

            if (source != null && source.NodeType == XmlNodeType.Element)
            {
                var original = source.InnerText;
                var transformed = Transform(original);

                if (transformed != original)
                {
                    var target = node.SelectSingleNode("x:segment/x:target", nsmgr);

                    if (target == null)
                    {
                        target = document.CreateElement("target", rootNamespace);
                        source.ParentNode.AppendChild(target);
                    }

                    var state = target.Attributes["state"];

                    if (state != null)
                    {
                        state.Value = "translated";
                    }

                    target.InnerText = transformed;
                    modified = true;
                }
            }

            return modified;
        }
    }
}
