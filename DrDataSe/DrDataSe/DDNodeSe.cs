using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrDataSe
{
    [Serializable]
    [XmlRoot(ElementName = "n")]
    public class DDNodeSe: DrData.DDNode , ISerializable, IXmlSerializable
    {
        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (Name != null) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME , Name);
            if (String.IsNullOrEmpty(Type) == false) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, Type); // write none empty type
            if (IsRoot) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_ROOT, IsRoot.ToString());
            if (HasChildNodes) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_COUNT, Count.ToString());

            var serializer = new XmlSerializer(typeof(DDAttributesCollection));
            if (Attributes != null) serializer.Serialize(writer, Attributes);

            if (HasChildNodes)
            {
                serializer = new XmlSerializer(typeof(DDNode));
                foreach (var keyValuePair in this)
                {
                    if (keyValuePair.Value != null) serializer.Serialize(writer, keyValuePair.Value);
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

            this.attributes = new DDAttributesCollection();
            var serializerDDAttributeCollection = new XmlSerializer(typeof(DDAttributesCollection));

            var serializerDDNode = new XmlSerializer(typeof(DDNode));

            this.Name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            this.Type = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);

            if (this.Type.Name == null) this.Type = string.Empty;

            var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
            reader.Read(); // read root element
            if ((isEmptyElement) | (reader.NodeType == XmlNodeType.EndElement)) return; // Exit if element without child '</DDNode>' or is empty node '<DDNode></DDNode>'

            var initialDepth = reader.Depth;

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if (((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION) == false) && (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE) == false)) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <DDAttributesCollection> or <DDNode> elements with childs and subchilds. 'Deep proptection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION)) attributes = ((DDAttributesCollection)serializerDDAttributeCollection.Deserialize(reader));

                    if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE)) Add((DDNode)serializerDDNode.Deserialize(reader));

                    if (reader.HasValue) // read value of element if there is
                    {
                        reader.Read(); // read value of element
                        if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // need to close the opened element, only self type
                    }
                }
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // Need to close the opened element, only self type
        }

        #endregion IXmlSerializable
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDNodeSe(SerializationInfo info, StreamingContext context)
        {
            this.Name = (String)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_NAME, typeof(String));
            this.Type = (DDType)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(DDType));
            this.attributes = (DDAttributesCollection)info.GetValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, typeof(DDAttributesCollection));
            this.childNodes = (Dictionary<string, DDNode>)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_COUNT, typeof(Dictionary<string, DDNode>));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_NAME, Name, typeof(String));
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, Type, typeof(DDType));
            info.AddValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, attributes, typeof(DDAttributesCollection));
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_COUNT, childNodes, typeof(Dictionary<string, DDNode>));
        }
        #endregion ISerializable
    }
}
