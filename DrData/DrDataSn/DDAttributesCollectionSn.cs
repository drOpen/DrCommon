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

using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrDataSn
{
    /// <summary>
    /// provides XML formating serialization and deserialization for DDAttributesCollection of the 'DrData'
    /// </summary>
    public static class DDAttributesCollectionSne
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
            writer.WriteStartElement("ac");
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
                writer.WriteStartElement("a");
                writer.WriteAttributeString("n", a.Key);
                //if (a.Value != null) DDValueSxe.XMLSerialize(a.Value, writer);
                DDValueSne.XMLSerialize(a.Value, writer);
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
        }
        /// <summary>
        /// Generates an attribute from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        private static void AddDeserializedAttribute(DDAttributesCollection ac, XmlReader reader)
        {
            var name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);

            DDValue v = null;
            v = DDValueSne.Deserialize(reader);
            if (name != null)
                ac.Add(name, v);
        }   

        #endregion Deserialize
    }
}
