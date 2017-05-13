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
            //StringWriter sw = new StringWriter(sb);

            //using (JsonWriter writer = new JsonTextWriter(sw))
            //{
            //    writer.Formatting = Newtonsoft.Json.Formatting.Indented;

            //    if (this.ac.Count != 0)
            //    {
            //        writer.WriteStartArray();

            //        writer.WriteEndArray();
            //    }
            //    if (this.ac.Name != null)
            //    {
            //        writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            //        writer.WriteValue(this.n.Name);
            //    }
            //    if (String.IsNullOrEmpty(this.n.Type) == false)
            //    {
            //        writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            //        writer.WriteValue(this.n.Type);
            //    }
            //    if (this.n.IsRoot)
            //    {
            //        writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_ROOT);
            //        writer.WriteValue(this.n.IsRoot);
            //    }
            //    if (this.n.Count != 0)
            //    {
            //        writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_CHILDREN_COUNT);
            //        writer.WriteValue(this.n.Count);
            //    }
            //    if (this.n.HasChildNodes)
            //    {
            //        foreach (var keyValuePair in this.n)
            //        {
            //            if (keyValuePair.Value != null) ((DDNodeSj)keyValuePair.Value).Serialyze(sb);
            //        }
            //    }
            //    writer.WriteEndObject();
            //}
        }


        //#region IXmlSerializable
     
        ///// <summary>
        ///// Converts an object into its XML representation.
        ///// </summary>
        ///// <param name="writer"></param>
        //public virtual void WriteXml(XmlWriter writer)
        //{
        //    if (this.ac == null) return; // if attributes is null
        //    if (this.ac.Count != 0) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_CHILDREN_ATTRIBUTE_COUNT, this.ac.Count.ToString()); // write element count for none empty collection

        //    foreach (var a in this.ac)
        //    {
        //        writer.WriteStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE);
        //        writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, a.Key);
        //        if (a.Value != null) ((DDValueSx)a.Value).WriteXml(writer);
        //        writer.WriteEndElement();
        //    }
        //}

        ///// <summary>
        ///// Generates an object from its XML representation.
        ///// </summary>
        ///// <param name="reader"></param>
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
        ///// <param name="reader"></param>
        //private void DeserializeAttribute(XmlReader reader)
        //{
        //    var name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
        //    var t = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);

        //    if (name != null)
        //    {
        //        DDValueSx v = null;
        //        if (t != null)
        //        {
        //            v = (DDValueSx)new DDValue();
        //            v.ReadXml(reader);
        //        }
        //        this.ac.Add(name, v);
        //    }

        //    if ((name == null) || (t == null)) // reads and close empty node
        //    {
        //        if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
        //        if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
        //    }
        //}

        ///// <summary>
        ///// Generates an attributes collection from its XML representation.
        ///// </summary>
        ///// <param name="reader"></param>
        //private void DeserializeAttributesCollection(XmlReader reader)
        //{
        //    reader.MoveToContent();
        //    this.ac = new DDAttributesCollection();
        //    var serializer = new XmlSerializer(typeof(DDValueSx));

        //    var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
        //    reader.Read(); // read root element
        //    if (isEmptyElement) return; // Exit for element without child <ac />

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
        /// <param name="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDAttributesCollectionSj(DDAttributesCollection ac)
        {
            return (ac == null ? null : new DDAttributesCollectionSj(ac));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static implicit operator DDAttributesCollection(DDAttributesCollectionSj ac)
        {
            return ( ac == null ? null : ac.ac);
        }

        #endregion explicit operator

    }
}
