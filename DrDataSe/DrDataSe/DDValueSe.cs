/*
  DDValueSe.cs -- stored data of the 'DrDataSe' general purpose Data abstraction layer 1.1, November 27, 2016
 
  Copyright (c) 2013-2016 Kudryashov Andrey aka Dr
 
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DrOpen.DrCommon.DrDataSe
{
    public class DDValueSe : DrData.DDValue, ISerializable, IXmlSerializable
    {
        #region DDValueSe
        /// <summary>
        /// Create empty value
        /// There are nullable type and data.
        /// </summary>
        public DDValueSe() : base() { }
        /// <summary>
        /// Create value with data. If type of object isn't supported throw application exception
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">data</param>
        public DDValueSe(object value) : base(value) { }
        #endregion DDValueSe
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDValueSe(SerializationInfo info, StreamingContext context)
        {
            base.type = (Type)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(Type));
            base.data = (byte[])info.GetValue(DDSchema.SERIALIZE_NODE_ARRAY_VALUE_ITEM, typeof(byte[]));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, this.type, typeof(Type));
            info.AddValue(DDSchema.SERIALIZE_NODE_ARRAY_VALUE_ITEM, this.data, typeof(byte[]));
        }
        #endregion ISerializable
        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, 
        /// if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Converts an object into its XML representation. The name of value is specified 
        /// </summary>
        /// <param name="writer">xml writer stream</param>
        /// <param name="name">value name</param>
        public virtual void WriteXml(XmlWriter writer, string name)
        {
            writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, name);
            this.WriteXml(writer);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">xml writer stream</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (Type == null) return; // if data is null

            writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, Type.ToString());
            if (Size != 0) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_SIZE, Size.ToString()); // write size only for none empty objects
            if (IsThisTypeXMLSerialyzeAsArray(type))
            {
                foreach (var element in ToStringArray())
                {
                    writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM);
                    writer.WriteString(element);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(ToString());
            }
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {

            reader.MoveToContent();
            type = null;

            var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            if (string.IsNullOrEmpty(t))
            {
                data = null;
                return; // null object
            }
            data = new byte[] { };
            this.type = Type.GetType(t);
            if (IsThisTypeXMLSerialyzeAsArray(type) == false)
            {
                this.data = GetByteArrayByTypeFromString(type, GetXmlElementValue(reader)); // read node value for none array types
            }
            else
            {
                var value = ReadXmlValueArray(reader);
                if (value != null) this.data = GetByteArray(Type, typeof(string[]) == Type ? ConvertObjectArrayToStringArray(value) : value);
            }

            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_VALUE)) reader.ReadEndElement(); // Need to close the opened element </DDValue>, only self
        }

        /// <summary>
        /// Return XML Element value.
        /// Open XML Element if needed, read value, close element and return value 
        /// </summary>
        /// <param name="reader">Xml stream reder</param>
        /// <returns>XML Element value</returns>
        protected static string GetXmlElementValue(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            var value = reader.Value; // read node value for none array types
            if (reader.HasValue) // read value of element if there is
            {
                reader.Read(); // read value of element
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            }
            return value;
        }

        /// <summary>
        /// Read XML Subling Nodes for array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual object[] ReadXmlValueArray(XmlReader reader)
        {

            int i = 0;
            object[] value = null;
            var elementType = type.GetElementType();

            reader.Read();
            var initialDepth = reader.Depth;
            if (reader.NodeType == XmlNodeType.None) return value; // Exit for element without child <DDvalue Type="String[]"/>

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <Value> elements with childs and subchilds <Value> elements 'Deep proptection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    Array.Resize(ref value, i + 1);
                    value[i] = ConvertStringToSpecifiedTypeObject(elementType, GetXmlElementValue(reader));
                    i++;
                }
                reader.MoveToContent();
            }
            return value;
        }

        /// <summary>
        /// Returns new string [] from object [].
        /// This function call ToString() for each element for new array
        /// </summary>
        /// <param name="array">object[]</param>
        /// <returns>Retrun new string[]</returns>
        protected string[] ConvertObjectArrayToStringArray(Array array)
        {
            var result = new string[array.Length];
            var i = 0;
            foreach (var item in array)
            {
                result[i] = item.ToString();
                i++;
            }
            return result;
        }

        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param name="type">Type to serialyze</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return value is false for this type, all other arrays should be serialized per elements</example>
        protected static bool IsThisTypeXMLSerialyzeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }


        #endregion IXmlSerializable

    }
}
