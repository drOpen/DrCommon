using DrOpen.DrCommon.DrData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace DrOpen.DrCommon.DrDataSx
{
    [Serializable]
    [XmlRoot(ElementName = "v")]
    public class DDValueSx : IXmlSerializable
    {

        private DDValueSx()
        { }
        private DDValueSx(DDValue v)
        {
            this.ddValue = v;
        }
        public DDValue GetDDValue()
        {
            return this.ddValue;
        }

        private DDValue ddValue;

        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, 
        /// if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (this.ddValue.Type == null)
            {
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, DDSchema.XML_SERIALIZE_VALUE_TYPE_NULL);
            }
            else
            {
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, this.ddValue.Type.ToString());
                if (this.ddValue.Size != 0) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_SIZE, this.ddValue.Size.ToString()); // write size only for none empty objects
                if (IsThisTypeXMLSerialyzeAsArray(this.ddValue.Type))
                {
                    foreach (var element in this.ddValue.ToStringArray())
                    {
                        writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM);
                        writer.WriteString(element);
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    writer.WriteString(this.ddValue.ToString());
                }
            }
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            
            var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            if ((string.IsNullOrEmpty(t)) || (t == DDSchema.XML_SERIALIZE_VALUE_TYPE_NULL))
            {
                this.ddValue = new DDValue();                // data = null;
                if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            }
            else
            {
                //data = new byte[] { };
                var type = Type.GetType(t);
                if (IsThisTypeXMLSerialyzeAsArray(type) )
                {
                    var value = ReadXmlValueArray(reader);
                    if (value == null)  value = new string[]{}; // support empty array
                    this.ddValue = new DDValue(DDValue.ConvertFromStringArrayTo(type, value));                    
                }
                else
                {
                    this.ddValue = new DDValue(DDValue.ConvertFromStringTo(type, GetXmlElementValue(reader)));
                }
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_VALUE)) reader.ReadEndElement(); // Need to close the opened element </v>, only self
        }

        /// <summary>
        /// Return XML Element v.
        /// Open XML Element if needed, read v, close element and return v 
        /// </summary>
        /// <param name="reader">Xml stream reder</param>
        /// <returns>XML Element v</returns>
        protected static string GetXmlElementValue(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            var value = reader.Value; // read node v for none array types
            if (reader.HasValue) // read v of element if there is
            {
                reader.Read(); // read v of element
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            }
            return value;
        }

        /// <summary>
        /// Read XML Subling Nodes for array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual string[] ReadXmlValueArray(XmlReader reader)
        {
            int i = 0;
            string[] v = null;

            reader.Read();
            var initialDepth = reader.Depth;
            if (reader.NodeType == XmlNodeType.None) return v; // Exit for element without child <v t="System.String[]"/>

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <v> elements with childs and subchilds <v> elements 'Deep proptection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    Array.Resize(ref v, i + 1);
                    v[i] = GetXmlElementValue(reader);
                    i++;
                }
                reader.MoveToContent();
            }
            return v;
        }
        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param name="type">Type to serialyze</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return v is false for this type, all other arrays should be serialized per elements</example>
        protected static bool IsThisTypeXMLSerialyzeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }


        #endregion IXmlSerializabl

        #region explicit operator
        public static explicit operator DDValueSx(DDValue v)
        {
            return new DDValueSx(v);
        }
        public static explicit operator DDValue(DDValueSx v)
        {
            return v.ddValue;
        }

        #endregion explicit operator
    }
}

