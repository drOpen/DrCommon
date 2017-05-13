/*
  DDNodeSj.cs -- provides json formating serialization and deserialization for DDNode of the 'DrData'  1.0, May 14, 2017
 
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
    public class DDNodeSj
    {

        private DDNodeSj()
        { }
        private DDNodeSj(DDNode n)
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

        public void Serialyze(StringBuilder sb)
        {
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                this.Serialyze(writer);
            }
        }

        public void Serialyze(JsonWriter writer)
        {
            writer.WriteStartObject();

                if (this.n.Name != null)
                {
                    writer.WritePropertyName(this.n.Name);
                }
                writer.WriteStartObject();
                if (String.IsNullOrEmpty(this.n.Type) == false)
                {
                    writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
                    writer.WriteValue(this.n.Type);
                }
                if (this.n.IsRoot)
                {
                    writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_ROOT);
                    writer.WriteValue(this.n.IsRoot);
                }
                if (this.n.Count != 0)
                {
                    writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_CHILDREN_COUNT);
                    writer.WriteValue(this.n.Count);
                }
                if (this.n.Attributes.Count > 0)
                {
                    //writer.WritePropertyName(DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE);
                    ((DDAttributesCollectionSj)this.n.Attributes).Serialyze(writer);
                    //writer.WriteEnd();
                }
                if (this.n.HasChildNodes)
                {
                   writer.WritePropertyName(DDSchema.XML_SERIALIZE_NODE);
                    writer.WriteStartArray();
                    foreach (var keyValuePair in this.n)
                    {
                        if (keyValuePair.Value != null) ((DDNodeSj)keyValuePair.Value).Serialyze(writer);
                    }
                    writer.WriteEndArray();
                    //writer.WriteEnd();
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
        }

        #region explicit operator
        /// <summary>
        /// boxes DDNode to for XML formating serialization and deserialization
        /// </summary>
        /// <param name="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDNodeSj(DDNode n)
        {
            return (n == null ? null : new DDNodeSj(n));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static implicit operator DDNode(DDNodeSj n)
        {
            return (n == null ? null : n.n);
        }

        #endregion explicit operator
    }
}
