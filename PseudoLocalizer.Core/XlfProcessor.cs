namespace PseudoLocalizer.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Xml;

    /// <summary>
    /// Applies transforms to string values in Xlf resource files.
    /// </summary>
    public sealed class XlfProcessor : XmlProcessor
    {
        private readonly ConcurrentDictionary<XmlDocument, Version> _versionCache = new();

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
                xpath = "/x:xliff/x:file/x:body/x:trans-unit|/x:xliff/x:file/x:body/x:group/x:trans-unit";
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
            var source = node.SelectSingleNode("x:source", nsmgr);

            return TransformSourceToTarget(
                source,
                node.SelectSingleNode("x:target", nsmgr),
                node);
        }

        private bool Visit20(XmlNode node, XmlNamespaceManager nsmgr)
        {
            var source = node.SelectSingleNode("x:segment/x:source", nsmgr);

            return TransformSourceToTarget(
                source,
                node.SelectSingleNode("x:segment/x:target", nsmgr),
                node);
        }

        private static XmlElement CreateTargetElement(XmlNode parent)
        {
            var document = parent.OwnerDocument;
            var newTarget = document.CreateElement("target", document.DocumentElement.NamespaceURI);
            parent.AppendChild(newTarget);
            return newTarget;
        }

        private bool TransformSourceToTarget(
            XmlNode source,
            XmlNode existingTarget,
            XmlNode targetParent)
        {
            if (source == null || source.NodeType != XmlNodeType.Element)
            {
                return false;
            }

            // Collect inline (non-text) child nodes
            var inlineNodes = new System.Collections.Generic.List<XmlNode>();

            foreach (XmlNode child in source.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Text && child.NodeType != XmlNodeType.SignificantWhitespace)
                {
                    inlineNodes.Add(child);
                }
            }

            if (inlineNodes.Count > 0)
            {
                return TransformWithInlineElements(source, existingTarget, targetParent, inlineNodes);
            }

            return TransformTextOnly(source, existingTarget, targetParent);
        }

        private bool TransformTextOnly(
            XmlNode source,
            XmlNode existingTarget,
            XmlNode targetParent)
        {
            var original = source.InnerText;
            var transformed = Transform(original);

            if (transformed == original)
            {
                return false;
            }

            var target = existingTarget ?? CreateTargetElement(targetParent);

            target.Attributes["state"]?.Value = "translated";
            target.InnerText = transformed;
            return true;
        }

        private bool TransformWithInlineElements(
            XmlNode source,
            XmlNode existingTarget,
            XmlNode targetParent,
            System.Collections.Generic.List<XmlNode> inlineNodes)
        {
            var document = source.OwnerDocument;

            // Build a combined string with format-string placeholders for inline elements.
            // Placeholders use {N} syntax which EscapeHelpers preserves through all transforms.
            // Use a high base index to avoid collision with real format strings in the text.
            const int PlaceholderBase = 10_000;

            var combined = new System.Text.StringBuilder();
            int inlineIndex = 0;

            foreach (XmlNode child in source.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.SignificantWhitespace)
                {
                    combined.Append(child.Value);
                }
                else
                {
                    combined.Append("{" + (PlaceholderBase + inlineIndex) + "}");
                    inlineIndex++;
                }
            }

            var originalCombined = combined.ToString();
            var transformedCombined = Transform(originalCombined);

            if (transformedCombined == originalCombined)
            {
                return false;
            }

            var target = existingTarget ?? CreateTargetElement(targetParent);
            target.Attributes["state"]?.Value = "translated";

            // Clear existing children (preserving element attributes)
            while (target.HasChildNodes)
            {
                target.RemoveChild(target.FirstChild);
            }

            // Reconstruct target by splitting transformed text at placeholder positions
            string remaining = transformedCombined;

            while (remaining.Length > 0)
            {
                int earliestPosition = -1;
                int earliestIndex = -1;
                string earliestPlaceholder = null;

                for (int i = 0; i < inlineNodes.Count; i++)
                {
                    string placeholder = "{" + (PlaceholderBase + i) + "}";
                    int pos = remaining.IndexOf(placeholder, StringComparison.Ordinal);

                    if (pos >= 0 && (earliestPosition < 0 || pos < earliestPosition))
                    {
                        earliestPosition = pos;
                        earliestIndex = i;
                        earliestPlaceholder = placeholder;
                    }
                }

                if (earliestPlaceholder != null)
                {
                    if (earliestPosition > 0)
                    {
                        target.AppendChild(document.CreateTextNode(remaining.Substring(0, earliestPosition)));
                    }

                    target.AppendChild(inlineNodes[earliestIndex].CloneNode(deep: true));
                    remaining = remaining.Substring(earliestPosition + earliestPlaceholder.Length);
                }
                else
                {
                    target.AppendChild(document.CreateTextNode(remaining));
                    break;
                }
            }

            return true;
        }
    }
}
