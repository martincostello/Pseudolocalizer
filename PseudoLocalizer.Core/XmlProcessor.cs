namespace PseudoLocalizer.Core
{
    using System.IO;
    using System.Text;
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

            bool modified = false;

            foreach (XmlNode node in SelectNodes(document, nsmgr))
            {
                modified |= Visit(node, nsmgr);
            }

            OnProcessed(document, nsmgr, modified);

            bool encoderShouldEmitUTF8Identifier = ShouldPreserveBOM(inputStream);

            var settings = new XmlWriterSettings()
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier),
            };

            using (var xmlWriter = XmlWriter.Create(outputStream, settings))
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
        /// Called after the nodes are processed to allow post-processing.
        /// </summary>
        /// <param name="document">The XML document that was processed.</param>
        /// <param name="nsmgr">The namespace manager for the document.</param>
        /// <param name="modified">Whether the XML document was modified during processing.</param>
        protected virtual void OnProcessed(XmlDocument document, XmlNamespaceManager nsmgr, bool modified)
        {
            // Do nothing by default
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
        /// <returns>
        /// <see langword="true"/> if the node was modified; otherwise <see langword="false"/>.
        /// </returns>
        protected abstract bool Visit(XmlNode node, XmlNamespaceManager nsmgr);

        private bool ShouldPreserveBOM(Stream inputStream)
        {
            bool preserveBOM = false;

            if (inputStream.CanSeek)
            {
                long offset = inputStream.Position;
                inputStream.Seek(0, SeekOrigin.Begin);

                byte[] preamble = Encoding.UTF8.GetPreamble();

                if (preamble.Length > 0)
                {
                    bool equal = true;

                    for (int i = 0; i < preamble.Length && equal; i++)
                    {
                        int j = inputStream.ReadByte();

                        if (j == -1)
                        {
                            equal = false;
                            break;
                        }

                        equal &= preamble[i] == (byte)j;
                    }

                    preserveBOM = equal;
                }

                inputStream.Seek(offset, SeekOrigin.Begin);
            }

            return preserveBOM;
        }
    }
}
