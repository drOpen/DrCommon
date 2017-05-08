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
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (this.n.Name != null) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME, this.n.Name);
            if (String.IsNullOrEmpty(this.n.Type) == false) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE, this.n.Type); // write none empty type
            if (this.n.IsRoot) writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_ROOT, this.n.IsRoot.ToString());
            writer.WriteAttributeString(DDSchema.XML_SERIALIZE_ATTRIBUTE_CHILDREN_COUNT, this.n.Count.ToString());

            if (this.n.Attributes != null) ((DDAttributesCollectionSx)this.n.Attributes).WriteXml(writer);

            if (this.n.HasChildNodes)
            {
                var serializer = new XmlSerializer(typeof(DDNodeSx));
                foreach (var keyValuePair in this.n)
                {
                    if (keyValuePair.Value != null) serializer.Serialize(writer, (DDNodeSx)keyValuePair.Value);
                }
            }
        }

        /// <summary>
        /// read element and return true if it's empty
        /// </summary>
        /// <param name="reader">Generates an object from its XML representation.</param>
        /// <returns>returns true if element is empty</returns>
        private bool isEmptyXMLElement(XmlReader reader)
        {
            var isEmptyElement = reader.IsEmptyElement; // Save Empty Status of Root Element
            reader.Read(); // read root element
            if ((isEmptyElement) | (reader.NodeType == XmlNodeType.EndElement)) return true;  // Exit if element without child '</n>' or is empty node '<n></n>'
            return false;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var sDDAttributeCollection = new XmlSerializer(typeof(DDAttributesCollectionSx));
            var sDDNode = new XmlSerializer(typeof(DDNodeSx));

            var name = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_NAME);
            var type = reader.GetAttribute(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            if (type== null) type = string.Empty;

            this.n = new DDNode(name, type);

            if (isEmptyXMLElement(reader)) return; // skip empty node <n/>

            var initialDepth = reader.Depth;

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if (reader.Depth > initialDepth)
                    reader.Skip(); // 'Deep proptection'
                else if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION))
                    // stupid solution for backward compatible
                    this.n.Attributes.Merge (((DDAttributesCollectionSx)sDDAttributeCollection.Deserialize(reader)).GetDDAttributesCollection());
                else if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE))
                    ((DDAttributesCollectionSx)this.n.Attributes).ReadXml(reader);
                else if (reader.IsStartElement(DDSchema.XML_SERIALIZE_NODE))
                    this.n.Add(((DDNodeSx)sDDNode.Deserialize(reader)).GetDDNode());
                else
                    reader.Skip(); // Skip none <ac>,<a>,<n> elements with childs and subchilds. 

                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element

                if (reader.HasValue) // read value of element if there is
                {
                    reader.Read(); // read value of element
                    if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // need to close the opened element, only self type
                }
            }
            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == DDSchema.XML_SERIALIZE_NODE)) reader.ReadEndElement(); // Need to close the opened element, only self type
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
}

