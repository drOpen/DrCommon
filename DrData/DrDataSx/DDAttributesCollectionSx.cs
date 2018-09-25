/*
  DDAttributesCollectionSx.cs -- provides XML formating serialization and deserialization for DDAttributesCollection of the 'DrData'  1.0, May 8, 2017
 
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
    /// provides XML formating serialization and deserialization for DDAttributesCollection of the 'DrData'
    /// </summary>
    [XmlRoot(ElementName = "ac")]
    public class DDAttributesCollectionSx : IXmlSerializable
    {

        private DDAttributesCollectionSx()
        { }
        private DDAttributesCollectionSx(DDAttributesCollection v)
        {
            this.ac = v;
        }
        /// <summary>
        /// returns/unboxes DDAttributesCollection 
        /// </summary>
        /// <returns></returns>
        public DDAttributesCollection GetDDAttributesCollection()
        {
            return this.ac;
        }

        private DDAttributesCollection ac;


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
            DDAttributesCollectionSxe.XMLSerialize(this.ac, writer);
        }
        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {
            this.ac = DDAttributesCollectionSxe.Deserialize(reader);
        }


        #endregion IXmlSerializable
        #region explicit operator
        /// <summary>
        /// boxes DDAttributesCollection to for XML formating serialization and deserialization
        /// </summary>
        /// <param name="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDAttributesCollectionSx(DDAttributesCollection ac)
        {
            return (ac == null ? null : new DDAttributesCollectionSx(ac));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static implicit operator DDAttributesCollection(DDAttributesCollectionSx ac)
        {
            return (ac == null ? null : ac.ac);
        }

        #endregion explicit operator
    }

    /// <summary>
    /// provides XML formating serialization and deserialization for DDAttributesCollection of the 'DrData'
    /// </summary>
    public static class DDAttributesCollectionSxe
    {
        #region Serialize
        /// <summary>
        /// Serializes the specified DDAttributesCollection into its XML representation and writes to a text writer.
        /// The parent XML element &lt;ac&gt; will be writed&lt;/ac&gt;
        /// </summary>
        /// <param name="n">the attributes collection to serialize</param>
        /// <param name="tw">text writer used to write the XML document.</param>
        public static void Serialize(this DDAttributesCollection ac, TextWriter tw)
        {
            using (XmlWriter writer = new XmlTextWriter(tw))
            {
                ac.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection into its XML representation and writes to a string builder.
        /// The parent XML element &lt;ac&gt; will be writed&lt;/ac&gt;
        /// </summary>
        /// <param name="n">the attributes collection to serialize</param>
        /// <param name="sb">string builder used to write the XML document.</param>
        public static void Serialize(this DDAttributesCollection ac, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    ac.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection into its XML representation and writes to a stream.
        /// The parent XML element &lt;ac&gt; will be writed&lt;/ac&gt;
        /// </summary>
        /// <param name="n">the attributes collection to serialize</param>
        /// <param name="s">stream used to write the XML document.</param>
        public static void Serialize(this DDAttributesCollection ac, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    ac.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection into its XML representation and writes to a XML writer.
        /// The parent XML element &lt;ac&gt; will be writed&lt;/ac&gt;
        /// </summary>
        /// <param name="n">the attributes collection to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        public static void Serialize(this DDAttributesCollection ac, XmlWriter writer)
        {
            writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION);
            XMLSerialize(ac, writer);
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection into its XML representation and writes to a XML writer.
        /// The parent node must exist, for example, use IXmlSerializable interface
        /// </summary>
        /// <param name="n">the attributes collection to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        internal static void XMLSerialize(DDAttributesCollection ac, XmlWriter writer)
        {
            if (ac == null) return; // if attributes is null

            foreach (var a in ac)
            {
                writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE);
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, a.Key);
                //if (a.Value != null) DDValueSxe.XMLSerialize(a.Value, writer);
                DDValueSxe.XMLSerialize(a.Value, writer);
                writer.WriteEndElement();
            }
        }
        #endregion Serialize
        #region Deserialize

        /// <summary>
        /// Generates an new DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(Stream s)
        {
            var ac = new DDAttributesCollection();
            ac.Deserialize(s);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">stream</param>
        public static void Deserialize(this DDAttributesCollection ac, Stream s)
        {
            using (StreamReader sr = new StreamReader(s))
            {
                using (XmlReader r = new XmlTextReader(sr))
                {
                    ac.Deserialize(r);
                }
            }
        }
        /// <summary>
        /// Generates an new DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="tr">text reader</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(TextReader tr)
        {
            var ac = new DDAttributesCollection();
            ac.Deserialize(tr);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="tr">text reader</param>
        public static void Deserialize(this DDAttributesCollection ac, TextReader tr)
        {
            using (XmlReader r = XmlReader.Create(tr))
            {
                ac.Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(String s)
        {
            var ac = new DDAttributesCollection();
            ac.Deserialize(s);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">string</param>
        public static void Deserialize(this DDAttributesCollection ac, String s)
        {
            using (XmlReader r = XmlReader.Create(new StringReader(s)))
            {
                ac.Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="reader">XML reader stream</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(XmlReader reader)
        {
            var ac = new DDAttributesCollection();
            ac.Deserialize(reader);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its XML representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="reader">XML reader stream</param>
        public static void Deserialize(this DDAttributesCollection ac, XmlReader reader)
        {
            if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE))
            {
                AddDeserializedAttribute(ac, reader);
            }
            else if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION))
            {
                AddDeserializedAttributesCollection(ac, reader);
            }
        }
        /// <summary>
        /// Generates an attribute from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        private static void AddDeserializedAttribute(DDAttributesCollection ac, XmlReader reader)
        {
            var name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);

            //if (name != null)
            //{
                DDValue v = null;
                //if (t != null) v = DDValueSxe.Deserialize(reader);
                v = DDValueSxe.Deserialize(reader);
                if (name != null)
                    ac.Add(name, v);
                else
                    ac.Add(v);

            //}

            //else
            //{
            //    if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            //    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            //}
        }

        /// <summary>
        /// Generates an attributes collection from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        private static void AddDeserializedAttributesCollection(DDAttributesCollection ac, XmlReader reader)
        {
            reader.MoveToContent();

            var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
            reader.Read(); // read root element
            if (isEmptyElement) return; // Exit for element without child <n />

            var initialDepth = reader.Depth;

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <a> elements with childs and subchilds <a> elements 'Deep proptection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    AddDeserializedAttribute(ac, reader); // deserializes attribute
                }
                reader.MoveToContent();
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION)) reader.ReadEndElement(); // need to close the opened element, only self type
        }
        #endregion Deserialize
    }
}
