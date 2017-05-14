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
    public class DDValueSj
    {

        private DDValueSj()
        { }
        private DDValueSj(DDValue v)
        {
            this.v = v;
        }
        /// <summary>
        /// returns/unboxes DDAttributesCollection 
        /// </summary>
        /// <returns></returns>
        public DDValue GetDDValue()
        {
            return this.v;
        }

        private DDValue v;

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
            writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
            if (this.v.Type == null)
                writer.WriteNull();
            else
            {
                writer.WriteValue(this.v.Type.ToString());
                writer.WritePropertyName(DDSchema.XML_SERIALIZE_NODE_VALUE);
                var a = IsThisTypeJsonSerialyzeAsArray(this.v.Type);
                if (a)
                {
                    writer.WriteStartArray();
                    foreach (var i in (Array)this.v.GetValue())
                    {
                        writer.WriteValue(i);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    if (this.v.Type == typeof (byte[]))
                        writer.WriteValue(this.v.GetValueAsHEX());
                    else
                        writer.WriteValue(this.v.GetValue());
                }
            }
        }

        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param name="type">Type to serialyze</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return n is false for this type, all other arrays should be serialized per elements</example>
        protected static bool IsThisTypeJsonSerialyzeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }

        #region explicit operator
        /// <summary>
        /// boxes DDValue to for json formating serialization and deserialization
        /// </summary>
        /// <param name="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDValueSj(DDValue v)
        {
            return (v == null ? null : new DDValueSj(v));
        }
        /// <summary>
        /// unbox DDValue
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator DDValue(DDValueSj v)
        {
            return (v == null ? null : v.v);
        }

        #endregion explicit operator

    }
}
