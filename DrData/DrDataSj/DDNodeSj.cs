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

using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DrOpen.DrCommon.DrData;

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
        #region Serialize
        /// <summary>
        /// Serializes the DDNode and writes the Json document to a text writer
        /// </summary>
        /// <param name="tw">text writer used to write the Json document.</param>
        public void Serialize(TextWriter tw)
        {
            this.n.Serialize(tw);
        }
        /// <summary>
        /// Serializes the DDNode and writes the Json document to a string builder
        /// </summary>
        /// <param name="sb">string builder used to write the Json document.</param>
        public void Serialize(StringBuilder sb)
        {
            this.n.Serialize(sb);
        }
        /// <summary>
        /// Serializes the DDNode and writes the Json document to a stream
        /// </summary>
        /// <param name="s">stream used to write the Json document.</param>
        public void Serialize(Stream s)
        {
            this.n.Serialize(s);
        }
        /// <summary>
        /// Serializes the DDNode and writes the Json document to a Json writer
        /// </summary>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public void Serialize(JsonWriter writer)
        {
            this.n.Serialize(writer);
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public void Deserialize(TextReader tr)
        {
            this.n = DDNodeSje.Deserialize(tr);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public void Deserialize(string s)
        {
            this.n = DDNodeSje.Deserialize(s);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        public void Deserialize(Stream s)
        {
            this.n = DDNodeSje.Deserialize(s);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="s">Json stream reader</param>
        public void Deserialize(JsonReader reader)
        {
            this.n = DDNodeSje.Deserialize(reader);
        }
        #endregion Deserialize
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
        #region Serialize
        /// <summary>
        /// Serializes the specified DDNode and writes the Json document to a text writer
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="tw">text writer used to write the Json document.</param>
        public static void Serialize(this DDNode n, TextWriter tw)
        {
            using (JsonWriter writer = new JsonTextWriter(tw))
            {
                n.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDNode and writes the Json document to a string builder
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="sb">string builder used to write the Json document.</param>
        public static void Serialize(this DDNode n, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    n.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDNode and writes the Json document to a stream
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="s">stream used to write the Json document.</param>
        public static void Serialize(this DDNode n, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    n.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDNode and writes the Json document to a Json writer
        /// </summary>
        /// <param name="n">the node to serialize</param>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public static void Serialize(this DDNode n, JsonWriter writer)
        {
            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            writer.WriteStartObject();

            if (n.Name != null)
            {
                writer.WritePropertyName(n.Name);
            }
            writer.WriteStartObject();
            if (String.IsNullOrEmpty(n.Type) == false)
            {
                writer.WritePropertyName(DDSchema.JSON_SERIALIZE_ATTRIBUTE_TYPE);
                writer.WriteValue(n.Type);
            }
            if (n.Attributes.Count > 0)
            {
                DDAttributesCollectionSje.JsonSerialize(n.Attributes, writer);
            }
            if (n.HasChildNodes)
            {
                writer.WritePropertyName(DDSchema.JSON_SERIALIZE_NODE);
                writer.WriteStartArray();
                foreach (var keyValuePair in n)
                {
                    if (keyValuePair.Value != null) keyValuePair.Value.Serialize(writer);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDNode n, TextReader tr)
        {
            n = Deserialize(tr);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(TextReader tr)
        {
            using (JsonReader reader = new JsonTextReader(tr))
            {
                return Deserialize(reader);
            }
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDNode n, string s)
        {
            n = Deserialize(s);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="tr">String that contains the Json document to deserialize.</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(string s)
        {
            var sr = new StringReader(s);

            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Deserialize(reader);
            }
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDNode n, Stream s)
        {
            n = Deserialize(s);
        }
        /// <summary>
        ///  Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDNode </returns>
        public static DDNode Deserialize(Stream s)
        {
            using (StreamReader sr = new StreamReader(s))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    return Deserialize(reader);
                }
            }
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="n">The deserialized node.</param>
        /// <param name="s">Json stream reader</param>
        public static void Deserialize(this DDNode n, JsonReader reader)
        {
            n = Deserialize(reader);
        }
        /// <summary>
        /// Generates an new DDNode from its Json representation.
        /// </summary>
        /// <param name="reader">Json stream reader</param>
        /// <returns>an new DDNode</returns>
        public static DDNode Deserialize(JsonReader reader)
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

                if ((reader.TokenType == JsonToken.String) && (n != null) && (prevTokenType == JsonToken.PropertyName) && (prevName == DDSchema.JSON_SERIALIZE_ATTRIBUTE_TYPE) && (reader.Value != null))
                {
                    n.Type = reader.Value.ToString();
                }

                if ((reader.TokenType == JsonToken.StartArray) && (prevName == DDSchema.JSON_SERIALIZE_NODE_ATTRIBUTE_COLLECTION) && (n != null))  // attributes collection
                {
                    //n.Attributes.Deserialize(reader);
                    DDAttributesCollectionSje.JsonDeserialize(n.Attributes, reader);
                }

                if ((reader.TokenType == JsonToken.StartArray) && (prevName == DDSchema.JSON_SERIALIZE_NODE) && (n != null)) // nodes collection
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndArray) break; // end list of nodes
                        if (reader.TokenType == JsonToken.StartObject) n.Add(Deserialize(reader)); // end list of nodes
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
        #endregion Deserialize
    }

}
