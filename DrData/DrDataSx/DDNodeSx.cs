/*
  DDNodeSx.cs -- provides XML formating serialization and deserialization for DDNode of the 'DrData'  1.0, May 8, 2017
 
  Copyright (c) 2013-2017 Kudryashov Andrey aka Dr
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation would be
      appreciated but is not required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>

*/
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrDataSx
{
    /// <summary>
    /// provides XML formating serialization and deserialization for DDNode of the 'DrData'
    /// </summary>
    [XmlRoot(ElementName = "n")]
    public class DDNodeSx : IXmlSerializable
    {

        private DDNodeSx()
        { }
        private DDNodeSx(DDNode n)
        {
            this.n = n;
        }
        /// <summary>
        /// returns/unboxes DDNode 
        /// </summary>
        /// <returns></returns>
        public DDNode GetDDNode()
        {
            return this.n;
        }

        private DDNode n;

        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">XML writer</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            DDNodeSxe.XMLSerialize(this.n, writer);
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">XML reader</param>
        public virtual void ReadXml(XmlReader reader)
        {
            this.n = DDNodeSxe.Deserialize(reader);
        }

        #endregion IXmlSerializable

        #region explicit operator
        /// <summary>
        /// boxes DDNode to for XML formating serialization and deserialization
        /// </summary>
        /// <param name="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDNodeSx(DDNode n)
        {
            return (n == null ? null : new DDNodeSx(n));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static implicit operator DDNode(DDNodeSx n)
        {
            return (n == null ? null : n.n);
        }

        #endregion explicit operator
    }

    /// <summary>
    /// provides XML formating serialization and deserialization for DDNode of the 'DrData'
    /// </summary>
    public static class DDNodeSxe
    {
        #region Serialize
        /// <summary>
        /// Serializes the specified DDNode into its XML representation and writes to a text writer.
        /// The parent XML element &lt;n&gt; will be writed&lt;/n&gt;
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="tw">text writer used to write the XML document.</param>
        public static void Serialize(this DDNode n, TextWriter tw)
        {
            using (XmlWriter writer = new XmlTextWriter(tw))
            {
                n.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDNode into its XML representation and writes to a string builder.
        /// The parent XML element &lt;n&gt; will be writed&lt;/n&gt;
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="sb">string builder used to write the XML document.</param>
        public static void Serialize(this DDNode n, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    n.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDNode into its XML representation and writes to a stream.
        /// The parent XML element &lt;n&gt; will be writed&lt;/n&gt;
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="s">stream used to write the XML document.</param>
        public static void Serialize(this DDNode n, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    n.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Converts an object into its XML representation.
        /// The parent XML element &lt;n&gt; will be writed&lt;/n&gt;
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        public static void Serialize(this DDNode n, XmlWriter writer)
        {
            writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE);
            XMLSerialize(n, writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation. 
        /// The parent node must exist, for example, use IXmlSerializable interface
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        internal static void XMLSerialize(DDNode n, XmlWriter writer)
        {
            if (n.Name != null) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, n.Name);
            if (String.IsNullOrEmpty(n.Type) == false) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, n.Type); // write none empty type
            if (n.Attributes != null) DDAttributesCollectionSxe.XMLSerialize(n.Attributes, writer);

            if (n.HasChildNodes)
            {
                foreach (var keyValuePair in n)
                {
                    if (keyValuePair.Value != null) keyValuePair.Value.Serialize(writer);
                }
            }
        }


        #endregion Serialize
        #region Serialize
        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(Stream s)
        {
            using (StreamReader sr = new StreamReader(s))
            {
                using (XmlReader r = new XmlTextReader(sr))
                {
                    return Deserialize(r);
                }
            }
        }
        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="s">stream</param>
        public static void Deserialize(this DDNode n, Stream s)
        {
            n.Deserialize(s);
        }

        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="tr">text reader</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(TextReader tr)
        {
            using (XmlReader r = XmlReader.Create(tr))
            {
                return Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="tr">text reader</param>
        public static void Deserialize(this DDNode n, TextReader tr)
        {
            n.Deserialize(tr);
        }
        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(String s)
        {
            using (XmlReader r = XmlReader.Create(new StringReader(s)))
            {
                return Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDNode from its XML representation.
        /// </summary>
        /// <param name="v">The deserialized node.</param>
        /// <param name="s">string</param>
        public static void Deserialize(this DDNode n, String s)
        {
            n.Deserialize(s);
        }
        /// <summary>
        /// Generates an node from its XML representation.
        /// </summary>
        /// <param name="reader">XML reader</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(XmlReader reader)
        {
            DDNode n = null;
            reader.MoveToContent();

            var sDDAttributeCollection = new XmlSerializer(typeof(DDAttributesCollectionSx));
            var sDDNode = new XmlSerializer(typeof(DDNodeSx));

            var name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            var type = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            if (type == null) type = string.Empty;

            if (name == null)
                n = new DDNode(new DDType(type));
            else
                n = new DDNode(name, new DDType(type));

            if (isEmptyXMLElement(reader)) return n; // skip empty node <n/>

            var initialDepth = reader.Depth;

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if (reader.Depth > initialDepth)
                    reader.Skip(); // 'Deep protection'
                else if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE)) || (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION)))
                    n.Attributes.Deserialize(reader);
                else if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE))
                    n.Add(Deserialize(reader));
                else
                    reader.Skip(); // Skip none <n>,<a>,<n> elements with childs and subchilds. 

                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element

                if (reader.HasValue) // read value of element if there is
                {
                    reader.Read(); // read value of element
                    if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // need to close the opened element, only self type
                }
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // Need to close the opened element, only self type
            return n;
        }

        /// <summary>
        /// read element and return true if it's empty
        /// </summary>
        /// <param name="reader">Generates an object from its XML representation.</param>
        /// <returns>returns true if element is empty</returns>
        private static bool isEmptyXMLElement(XmlReader reader)
        {
            var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
            reader.Read(); // read root element
            if ((isEmptyElement) | (reader.NodeType == XmlNodeType.EndElement)) return true;  // Exit if element without child '</n>' or is empty node '<n></n>'
            return false;
        }
        #endregion Serialize
    }
}

