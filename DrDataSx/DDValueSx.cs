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
    /// <summary>
    /// provides XML formating serialization and deserialization for DDValue of the 'DrData'
    /// </summary>
    [XmlRoot(ElementName = "v")]
    public class DDValueSx : IXmlSerializable
    {

        private DDValueSx()
        { }
        private DDValueSx(DDValue v)
        {
            this.v = v;
        }
        /// <summary>
        /// returns/unboxes DDValue 
        /// </summary>
        /// <returns></returns>
        public DDValue GetDDValue()
        {
            return this.v;
        }

        private DDValue v;

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
            if (this.v.Type == null)
            {
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, DDSchema.XML_SERIALIZE_VALUE_TYPE_NULL);
            }
            else
            {
                writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, this.v.Type.ToString());
                if (this.v.Size != 0) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_SIZE, this.v.Size.ToString()); // write size only for none empty objects
                if (IsThisTypeXMLSerialyzeAsArray(this.v.Type))
                {
                    foreach (var element in this.v.ToStringArray())
                    {
                        writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM);
                        writer.WriteString(element);
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    writer.WriteString(this.v.ToString());
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
                this.v = new DDValue();                // data = null;
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
                    this.v = new DDValue(DDValue.ConvertFromStringArrayTo(type, value));                    
                }
                else
                {
                    this.v = new DDValue(DDValue.ConvertFromStringTo(type, GetXmlElementValue(reader)));
                }
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_VALUE)) reader.ReadEndElement(); // Need to close the opened element </n>, only self
        }

        /// <summary>
        /// Return XML Element n.
        /// Open XML Element if needed, read n, close element and return n 
        /// </summary>
        /// <param name="reader">Xml stream reder</param>
        /// <returns>XML Element n</returns>
        protected static string GetXmlElementValue(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            var value = reader.Value; // read node n for none array types
            if (reader.HasValue) // read n of element if there is
            {
                reader.Read(); // read n of element
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
            if (reader.NodeType == XmlNodeType.None) return v; // Exit for element without child <n t="System.String[]"/>

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <n> elements with childs and subchilds <n> elements 'Deep proptection'
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
        /// <example>For example: byte[] should be serialize as HEX single string therefore return n is false for this type, all other arrays should be serialized per elements</example>
        protected static bool IsThisTypeXMLSerialyzeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }


        #endregion IXmlSerializabl

        #region explicit operator
        /// <summary>
        /// boxes DDValue to for XML formating serialization and deserialization
        /// </summary>
        /// <param name="n">DDValue for box</param>
        /// <returns></returns>
        public static explicit operator DDValueSx(DDValue v)
        {
            return(v == null ? null : new DDValueSx(v));
        }
        /// <summary>
        /// unbox DDValue
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static implicit operator DDValue(DDValueSx v)
        {
            return(v == null ? null : v.v);
        }
        #endregion explicit operator
    }
}

