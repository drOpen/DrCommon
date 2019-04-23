/*
  DDValueSx.cs -- provides XML formating serialization and deserialization for DDValue of the 'DrData'  1.0, May 8, 2017
 
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
    /// provides XML formating serialization and deserialization for DDValue of the 'DrData'
    /// </summary>
    public static class DDValueSne
    {
        public static Type DEFAULT_VALUE_TYPE = typeof(string);

        #region Serialize
        /// <summary>
        /// Serializes the specified DDValue into its XML representation and writes to a text writer.
        /// The parent XML element &lt;v&gt; will be writed&lt;/v&gt;
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="tw">text writer used to write the XML document.</param>
        public static void Serialize(this DDValue v, TextWriter tw)
        {
            using (XmlWriter writer = new XmlTextWriter(tw))
            {
                v.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDValue into its XML representation and writes to a string builder.
        /// The parent XML element &lt;v&gt; will be writed&lt;/v&gt;
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="sb">string builder used to write the XML document.</param>
        public static void Serialize(this DDValue v, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    v.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDValue into its XML representation and writes to a stream.
        /// The parent XML element &lt;v&gt; will be writed&lt;/v&gt;
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="s">stream used to write the XML document.</param>
        public static void Serialize(this DDValue v, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (XmlWriter writer = new XmlTextWriter(sw))
                {
                    v.Serialize(writer);
                }
            }
        }

        /// <summary>
        /// Serializes the specified DDValue into its XML representation and writes to a XML writer.
        /// The parent XML element &lt;v&gt; will be writed&lt;/v&gt;
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        public static void Serialize(this DDValue v, XmlWriter writer)
        {
            writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_VALUE);
            XMLSerialize(v, writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the specified DDValue into its XML representation and writes to a XML writer.
        /// The parent XML element &lt;v&gt; will be writed&lt;/v&gt;
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="writer">XML writer used to write the XML document.</param>
        internal static void XMLSerialize(DDValue v, XmlWriter writer)
        {
            if (v != null)
            {
                if (v.Type != null)
                {
                    writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, ConvertToAcnType(v.Type));

                    if (IsThisTypeXMLSerializeAsArray(v.Type))
                    {
                        foreach (var element in v.ToStringArray())
                        {
                            writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_VALUE);
                            writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_VALUE, element);
                            writer.WriteEndElement();
                        }
                    }
                    else
                    {
                        String attrVal = v.ToString();

                        if (v.Type == typeof(Byte[]))
                            attrVal = "HEX:" + attrVal;

                        writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_VALUE, attrVal);
                        
                    }
                }
                else
                {
                    writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, "");
                    writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_VALUE, "");
                }
            }
            else
            {
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, "");
            }
        }
        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param name="type">Type to serialize</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return n is false for this type, all other arrays should be serialized per elements</example>
        private static bool IsThisTypeXMLSerializeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }
        #endregion Serialize

        #region Deserialize
        /// <summary>
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>an new DDValue </returns>
        public static DDValue Deserialize(Stream s)
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
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="s">stream</param>
        public static void Deserialize(this DDValue v, Stream s)
        {
            v.Deserialize(s);
        }
        /// <summary>
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="tr">text reader</param>
        /// <returns>an new DDValue </returns>
        public static DDValue Deserialize(TextReader tr)
        {
            using (XmlReader r = XmlReader.Create(tr))
            {
                return Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="tr">text reader</param>
        public static void Deserialize(this DDValue v, TextReader tr)
        {
            v.Deserialize(tr);
        }
        /// <summary>
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>an new DDValue </returns>
        public static DDValue Deserialize(String s)
        {
            using (XmlReader r = XmlReader.Create(new StringReader(s)))
            {
                return Deserialize(r);
            }
        }
        /// <summary>
        /// Generates an new DDValue from its XML representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="s">string</param>
        public static void Deserialize(this DDValue v, String s)
        {
            v.Deserialize(s);
        }

        public static DDValue Deserialize(XmlReader reader)
        {
            DDValue v = null;
            reader.MoveToContent();

            var xmlType = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            var attrType = DEFAULT_VALUE_TYPE;
            String xmlVal = reader[DDSchema.XML_SERIALIZE_ATTRIBUTE_VALUE];
            
            if (xmlType == null)
            {
                if (xmlVal == null)
                    v = null;
                else
                    v = new DDValue(DDValue.ConvertFromStringTo(attrType, xmlVal));
            }
            else if (xmlType == String.Empty)
            {
                if (xmlVal == null)
                    v = null;
                else if (xmlVal == String.Empty)
                    v = new DDValue();
                else
                    v = new DDValue(DDValue.ConvertFromStringTo(attrType, xmlVal));
            }
            else
            {
                attrType = ParseAcnType(xmlType);
                if (IsThisTypeXMLSerializeAsArray(attrType))
                {
                    var value = ReadXmlValueArray(reader);
                    if (value == null) value = new string[] { }; // support empty array
                    v = new DDValue(DDValue.ConvertFromStringArrayTo(attrType, value));
                }
                else
                {
                    if (attrType == typeof(Byte[]))
                        xmlVal = xmlVal.Replace("HEX:", "");

                    if (attrType == typeof(String) & xmlVal == null)
                        v = new DDValue(null);
                    else
                        v = new DDValue(DDValue.ConvertFromStringTo(attrType, xmlVal));
                }

            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_VALUE)) reader.ReadEndElement(); // Need to close the opened element </n>, only self
            reader.Read();
            return v;
        }

        /// <summary>
        /// Return XML Element n.
        /// Open XML Element if needed, read n, close element and return n 
        /// </summary>
        /// <param name="reader">Xml stream reder</param>
        /// <returns>XML Element n</returns>
        private static string GetXmlElementValue(XmlReader reader)
        {
            //if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            //var value = reader.Value; // read node n for none array types
            //if (reader.HasValue) // read n of element if there is
            //{
            //    reader.Read(); // read n of element
            //    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            //}
            var value = reader[DDSchema.XML_SERIALIZE_ATTRIBUTE_VALUE];
                
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            return value;
        }

        /// <summary>
        /// Read XML Subling Nodes for array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static string[] ReadXmlValueArray(XmlReader reader)
        {
            int i = 0;
            string[] v = null;
            reader.Read();
            var initialDepth = reader.Depth;
            if (reader.NodeType == XmlNodeType.None) return v; // 

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_VALUE) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <n> elements with childs and subchilds <n> elements 'Deep protection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    Array.Resize(ref v, i + 1);
                    v[i] = GetXmlElementValue(reader);
                    i++;
                }
                reader.MoveToContent();
                reader.Read();
            }
            return v;
        }
        #endregion Deserialize

        private static String ConvertToAcnType(Type attrType)
        {
            if (attrType == typeof(String)) { return "2"; }
            else if (attrType == typeof(String[])) { return "258"; }
            else if (attrType == typeof(Int32)) { return "3"; }
            else if (attrType == typeof(Int32[])) { return "259"; }
            else if (attrType == typeof(UInt32)) { return "4"; }
            else if (attrType == typeof(UInt32[])) { return "260"; }
            else if (attrType == typeof(Int64)) { return "5"; }
            else if (attrType == typeof(Int64[])) { return "261"; }
            else if (attrType == typeof(UInt64)) { return "6"; }
            else if (attrType == typeof(UInt64[])) { return "262"; }
            else if (attrType == typeof(Boolean)) { return "7"; }
            else if (attrType == typeof(Boolean[])) { return "263"; }
            else if (attrType == typeof(Byte[])) { return "12"; }
            else if (attrType == typeof(Guid)) { return "13"; }
            else if (attrType == typeof(Guid[])) { return "269"; }
            else { return ""; }
        }

        private static Type ParseAcnType(String intType)
        {
            switch (intType)
            {
                case "1":
                    return typeof(String);
                case "2":
                    return typeof(String);
                case "258":
                    return typeof(String[]);
                case "3":
                    return typeof(Int32);
                case "259":
                    return typeof(Int32[]);
                case "4":
                    return typeof(UInt32);
                case "260":
                    return typeof(UInt32[]);
                case "5":
                    return typeof(Int64);
                case "261":
                    return typeof(Int64[]);
                case "6":
                    return typeof(UInt64);
                case "262":
                    return typeof(UInt64[]);
                case "7":
                    return typeof(Boolean);
                case "263":
                    return typeof(Boolean[]);
                case "12":
                    return typeof(Byte[]);
                case "13":
                    return typeof(Guid);
                case "269":
                    return typeof(Guid[]);
                default:
                    return typeof(String);
            }
        }
    }
}
