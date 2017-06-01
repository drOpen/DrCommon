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
            this.n.Serialyze(sb);
        }

        public void Serialyze(JsonWriter writer)
        {
            this.n.Serialyze(writer);
        }

        public void Deserialyze(string s)
        {
            this.n = DDNodeSje.Deserialyze(s);
        }

        public void Deserialyze(JsonReader reader)
        {
            this.n = DDNodeSje.Deserialyze(reader);
        }

        #region explicit operator
        /// <summary>
        /// boxes DDNode to for json formating serialization and deserialization
        /// </summary>
        /// <param prevName="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDNodeSj(DDNode n)
        {
            return (n == null ? null : new DDNodeSj(n));
        }
        /// <summary>
        /// unbox DDNode
        /// </summary>
        /// <param prevName="n"></param>
        /// <returns></returns>
        public static implicit operator DDNode(DDNodeSj n)
        {
            return (n == null ? null : n.n);
        }

        #endregion explicit operator
    }

    public static class DDNodeSje
    {

        #region Serialyze

        public static void Serialyze(this DDNode n, TextWriter tw)
        {
            using (JsonWriter writer = new JsonTextWriter(tw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                n.Serialyze(writer);
            }
        }
        public static void Serialyze(this DDNode n, StringBuilder sb)
        {
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                n.Serialyze(writer);
            }
        }

        public static void Serialyze(this DDNode n, JsonWriter writer)
        {
            writer.WriteStartObject();

            if (n.Name != null)
            {
                writer.WritePropertyName(n.Name);
            }
            writer.WriteStartObject();
            if (String.IsNullOrEmpty(n.Type) == false)
            {
                writer.WritePropertyName(DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE);
                writer.WriteValue(n.Type);
            }
            if (n.Attributes.Count > 0)
            {
                n.Attributes.Serialyze(writer);
            }
            if (n.HasChildNodes)
            {
                writer.WritePropertyName(DDSchema.XML_SERIALIZE_NODE);
                writer.WriteStartArray();
                foreach (var keyValuePair in n)
                {
                    if (keyValuePair.Value != null) keyValuePair.Value.Serialyze(writer);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
        #endregion Serialyze

        #region Deserialyze
        public static DDNode Deserialyze(TextReader tr)
        {
            using (JsonReader reader = new JsonTextReader(tr))
            {
                return Deserialyze(reader);
            }
        }
        public static DDNode Deserialyze(string s)
        {
            var sr = new StringReader(s);

            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Deserialyze(reader);
            }
        }

        public static DDNode Deserialyze(JsonReader reader)
        {
            DDNode n = null;
            string prevValueString = null;
            string prevName = null;

            JsonToken prevTokenType = JsonToken.None;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                if ((reader.TokenType == JsonToken.PropertyName) && (n == null) && (reader.Value != null))
                {
                    n = new DDNode(reader.Value.ToString());
                }

                if ((reader.TokenType == JsonToken.String) && (n != null) && (prevTokenType == JsonToken.PropertyName) && (prevName == DDSchema.XML_SERIALIZE_ATTRIBUTE_TYPE) && (reader.Value != null))
                {
                    n.Type = reader.Value.ToString();
                }

                if ((reader.TokenType == JsonToken.StartArray) && (prevName == DDSchema.XML_SERIALIZE_NODE_ATTRIBUTE) && (n != null))  // attributes collection
                {
                    n.Attributes.Deserialyze(reader);
                }

                if ((reader.TokenType == JsonToken.StartArray) && (prevName == DDSchema.XML_SERIALIZE_NODE) && (n != null)) // nodes collection
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndArray) break; // end list of nodes
                        if (reader.TokenType == JsonToken.StartObject) n.Add(Deserialyze(reader)); // end list of nodes
                    }
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
            return n;
        }
        #endregion Deserialyze
    }

}
