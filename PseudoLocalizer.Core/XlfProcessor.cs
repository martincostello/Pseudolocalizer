namespace PseudoLocalizer.Core
{
    using System.Xml;

    /// <summary>
    /// Applies transforms to string values in Xlf resource files.
    /// </summary>
    public sealed class XlfProcessor : XmlProcessor
    {
        /// <inheritdoc />
        protected override XmlNodeList SelectNodes(XmlDocument document, XmlNamespaceManager nsmgr)
        {
            if (!nsmgr.HasNamespace("x"))
            {
                nsmgr.AddNamespace("x", document.DocumentElement.NamespaceURI);
            }

            return document.SelectNodes("/x:xliff/x:file/x:body/x:trans-unit", nsmgr);
        }

        /// <inheritdoc />
        protected override void Visit(XmlNode node, XmlNamespaceManager nsmgr)
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

                    if (state == null)
                    {
                        state = document.CreateAttribute("state");
                        target.Attributes.Append(state);
                    }

                    state.Value = "translated";
                    target.InnerText = transformed;
                }
            }
        }
    }
}
