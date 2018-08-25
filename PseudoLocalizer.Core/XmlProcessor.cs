namespace PseudoLocalizer.Core
{
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Applies transforms to string values in XML files.
    /// </summary>
    public abstract class XmlProcessor : Processor
    {
        /// <inheritdoc />
        public override void Transform(Stream inputStream, Stream outputStream)
        {
            var document = LoadDocument(inputStream);
            var nsmgr = new XmlNamespaceManager(document.NameTable);

            foreach (XmlNode node in SelectNodes(document, nsmgr))
            {
                Visit(node, nsmgr);
            }

            using (var xmlWriter = XmlWriter.Create(outputStream))
            {
                document.WriteTo(xmlWriter);
            }
        }

        /// <summary>
        /// Loads an XML document from the specified stream.
        /// </summary>
        /// <param name="inputStream">The stream to load the XML from.</param>
        /// <returns>
        /// The XML document read from <paramref name="inputStream"/>.
        /// </returns>
        protected virtual XmlDocument LoadDocument(Stream inputStream)
        {
            var document = new XmlDocument() { PreserveWhitespace = true };
            document.Load(inputStream);

            return document;
        }

        /// <summary>
        /// Selects the XML nodes to transform.
        /// </summary>
        /// <param name="document">The XML document to select the nodes from.</param>
        /// <param name="nsmgr">The namespace manager for the document.</param>
        /// <returns>
        /// The XML nodes to transform.
        /// </returns>
        protected abstract XmlNodeList SelectNodes(XmlDocument document, XmlNamespaceManager nsmgr);

        /// <summary>
        /// Visits an XML node containing a string to transform.
        /// </summary>
        /// <param name="node">The node to transform.</param>
        /// <param name="nsmgr">The namespace manager for the document.</param>
        protected abstract void Visit(XmlNode node, XmlNamespaceManager nsmgr);
    }
}
