using System;
using System.IO;
using System.Text;
using System.Xml;

namespace KeyHub.Common.Serializers
{
    /// <summary>
    /// Overrides the default XML test writer to include full end elements for all elements.
    /// This provides a more readible and editable XML file
    /// </summary>
    public class XmlTextWriterFull : XmlTextWriter
    {
        public XmlTextWriterFull(TextWriter sink) : base(sink) { SetFormatting(); }

        public XmlTextWriterFull(Stream stream) : base(stream, null) { SetFormatting(); }

        public XmlTextWriterFull(Stream stream, Encoding enc) : base(stream, enc) { SetFormatting(); }

        public XmlTextWriterFull(String str, Encoding enc) : base(str, enc) { SetFormatting(); }

        private void SetFormatting()
        {
            this.Formatting = System.Xml.Formatting.Indented;
        }

        public override XmlWriterSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new XmlWriterSettings();

                    settings.Indent = true;
                    settings.OmitXmlDeclaration = false;
                    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                }

                return settings;
            }
        }

        private XmlWriterSettings settings;

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }
}