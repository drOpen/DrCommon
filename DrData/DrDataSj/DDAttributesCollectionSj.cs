/*
  DDAttributesCollectionSj.cs -- provides json formating serialization and deserialization for DDAttributesCollection of the 'DrData'  1.0, May 8, 2017
 
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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IO;


namespace DrOpen.DrCommon.DrDataSj
{
    /// <summary>
    /// provides json formating serialization and deserialization for DDAttributesCollection of the 'DrData'
    /// </summary>
    public class DDAttributesCollectionSj
    {

        private DDAttributesCollectionSj()
        { }
        private DDAttributesCollectionSj(DDAttributesCollection v)
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

        public void Serialyze(StringBuilder sb)
        {
            this.ac.Serialyze(sb);
        }

        public void Serialyze(JsonWriter writer)
        {
            this.ac.Serialyze(writer);
        }

        public void Deserialyze(string s)
        {
            this.ac = DDAttributesCollectionSje.Deserialyze(s);
        }

        public void Deserialyze(JsonReader reader)
        {
            this.ac = DDAttributesCollectionSje.Deserialyze(reader);
        }

        //#region IXmlSerializable

        ///// <summary>
        ///// Converts an object into its XML representation.
        ///// </summary>
        ///// <param prevName="reader"></param>
        //public virtual void WriteXml(XmlWriter reader)
        //{
        //    if (this.v == null) return; // if attributes is null
        //    if (this.v.Count != 0) reader.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_CHILDREN_ATTRIBUTE_COUNT, this.v.Count.ToString()); // write element count for none empty collection

        //    foreach (var a in this.v)
        //    {
        //        reader.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE);
        //        reader.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, a.Key);
        //        if (a.Value != null) ((DDValueSx)a.Value).WriteXml(reader);
        //        reader.WriteEndElement();
        //    }
        //}

        ///// <summary>
        ///// Generates an object from its XML representation.
        ///// </summary>
        ///// <param prevName="reader"></param>
        //public virtual void ReadXml(XmlReader reader)
        //{
        //    if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE))
        //    {
        //        DeserializeAttribute(reader);
        //        return;
        //    }
        //    if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION))
        //    {
        //        DeserializeAttributesCollection(reader);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Generates an attribute from its XML representation.
        ///// </summary>
        ///// <param prevName="reader"></param>
        //private void DeserializeAttribute(XmlReader reader)
        //{
        //    var prevName = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
        //    var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);

        //    if (prevName != null)
        //    {
        //        DDValueSx v = null;
        //        if (t != null)
        //        {
        //            v = (DDValueSx)new DDValue();
        //            v.ReadXml(reader);
        //        }
        //        this.v.Add(prevName, v);
        //    }

        //    if ((prevName == null) || (t == null)) // reads and close empty node
        //    {
        //        if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
        //        if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
        //    }
        //}

        ///// <summary>
        ///// Generates an attributes collection from its XML representation.
        ///// </summary>
        ///// <param prevName="reader"></param>
        //private void DeserializeAttributesCollection(XmlReader reader)
        //{
        //    reader.MoveToContent();
        //    this.v = new DDAttributesCollection();
        //    var serializer = new XmlSerializer(typeof(DDValueSx));

        //    var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
        //    reader.Read(); // read root element
        //    if (isEmptyElement) return; // Exit for element without child <v />

        //    var initialDepth = reader.Depth;

        //    while ((reader.Depth >= initialDepth)) // do all childs
        //    {
        //        if ((reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE) == false) || (reader.Depth > initialDepth))
        //        {
        //            reader.Skip(); // Skip none <a> elements with childs and subchilds <a> elements 'Deep proptection'
        //            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
        //        }
        //        else
        //        {
        //            DeserializeAttribute(reader); // deserializes attribute
        //        }
        //        reader.MoveToContent();
        //    }
        //    if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION)) reader.ReadEndElement(); // need to close the opened element, only self type
        //}

        //#endregion IXmlSerializable

        #region explicit operator
        /// <summary>
        /// boxes DDAttributesCollection to for json formating serialization and deserialization
        /// </summary>
        /// <param prevName="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDAttributesCollectionSj(DDAttributesCollection ac)
        {
            return (ac == null ? null : new DDAttributesCollectionSj(ac));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param prevName="v"></param>
        /// <returns></returns>
        public static implicit operator DDAttributesCollection(DDAttributesCollectionSj ac)
        {
            return (ac == null ? null : ac.ac);
        }

        #endregion explicit operator

    }

    /// <summary>
    /// provides json formating serialization and deserialization for DDAttributesCollection of the 'DrData'
    /// </summary>
    public static class DDAttributesCollectionSje
    {
        #region Serialyze
        public static void Serialyze(this DDAttributesCollection ac, StringBuilder sb)
        {
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                ac.Serialyze(writer);
            }
        }

        public static void Serialyze(this DDAttributesCollection ac, JsonWriter writer)
        {
            if (ac.Count != 0)
            {
                writer.WritePropertyName(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE);
                writer.WriteStartArray();

                foreach (var a in ac)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(a.Key);
                    writer.WriteStartObject();
                    if (a.Value != null)
                        ((DDValueSj)a.Value).Serialyze(writer);

                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
        #endregion Serialyze
        #region Deserialyze
        public static DDAttributesCollection Deserialyze(string s)
        {
            var sr = new StringReader(s);

            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Deserialyze(reader);
            }
        }
        public static DDAttributesCollection Deserialyze(JsonReader reader)
        {
            return new DDAttributesCollection().Deserialyze(reader);
        }
        public static DDAttributesCollection Deserialyze(this DDAttributesCollection ac, JsonReader reader)
        {
            string prevValueString = null;
            string prevName = null;

            JsonToken prevTokenType = JsonToken.None;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray) break; // end list of attributes

                if ((reader.TokenType == JsonToken.PropertyName) && (prevTokenType == JsonToken.StartObject) && (reader.Value != null))
                {
                    ac.Add(reader.Value.ToString(),DDValueSje.Deserialyze(reader));
                }
                //  save current values
                prevTokenType = reader.TokenType;
                if (reader.TokenType == JsonToken.None)
                {
                    prevValueString = null;
                    prevName = null;
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    prevName = reader.Value.ToString();
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    prevValueString = reader.Value.ToString();
                }
            }
            return ac;
        }
        #endregion Deserialyze
    }
}
