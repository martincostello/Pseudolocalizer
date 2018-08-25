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
        protected override void Visit(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var version = GetXliffVersion(node.OwnerDocument);

            if (version.Major == 1)
            {
                Visit10(node, nsmgr);
            }
            else if (version.Major == 2)
            {
                Visit20(node, nsmgr);
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

        private void Visit10(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var document = node.OwnerDocument;
            var rootNamespace = document.DocumentElement.NamespaceURI;
            var source = node.SelectSingleNode("x:source", nsmgr);

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
                }
            }
        }

        private void Visit20(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var document = node.OwnerDocument;
            var rootNamespace = document.DocumentElement.NamespaceURI;
            var source = node.SelectSingleNode("x:segment/x:source", nsmgr);

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
                }
            }
        }
    }
}
