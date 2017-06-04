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

using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DrOpen.DrCommon.DrData;

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

        #region Serialize
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a text writer
        /// </summary>
        /// <param name="tw">text writer used to write the Json document.</param>
        public void Serialize(TextWriter tw)
        {
            this.ac.Serialize(tw);
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a string builder
        /// </summary>
        /// <param name="sb">string builder used to write the Json document.</param>
        public void Serialize(StringBuilder sb)
        {
            this.ac.Serialize(sb);
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a stream
        /// </summary>
        /// <param name="s">stream used to write the Json document.</param>
        public void Serialize(Stream s)
        {
            this.ac.Serialize(s);
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a Json writer
        /// </summary>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public void Serialize(JsonWriter writer)
        {
            this.ac.Serialize(writer);
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public void Deserialize(TextReader tr)
        {
            DDAttributesCollectionSje.Deserialize(this.ac, tr);
        }
        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public void Deserialize(string s)
        {
            this.ac = DDAttributesCollectionSje.Deserialize(s);
        }

        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDAttributesCollection </returns>
        public void Deserialize(Stream s)
        {
            this.ac = DDAttributesCollectionSje.Deserialize(s);
        }
        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="reader">Json stream reader</param>
        /// <returns>an new DDAttributesCollection</returns>
        public void Deserialize(JsonReader reader)
        {
            this.ac = DDAttributesCollectionSje.Deserialize(reader);
        }
        #endregion Deserialize

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
        #region Serialize
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a text writer
        /// </summary>
        /// <param name="ac">the attributes collection to serialize</param>
        /// <param name="tw">text writer used to write the Json document.</param>
        public static void Serialize(this DDAttributesCollection ac, TextWriter tw)
        {
            using (JsonWriter writer = new JsonTextWriter(tw))
            {
                ac.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a string builder
        /// </summary>
        /// <param name="ac">the attributes collection to serialize</param>
        /// <param name="sb">string builder used to write the Json document.</param>
        public static void Serialize(this DDAttributesCollection ac, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ac.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a stream
        /// </summary>
        /// <param name="ac">the attributes collection to serialize</param>
        /// <param name="s">stream used to write the Json document.</param>
        public static void Serialize(this DDAttributesCollection ac, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ac.Serialize(writer);
                }
            }
        }
                /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a Json writer
        /// </summary>
        /// <param name="ac">the attributes collection to serialize</param>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public static void Serialize(this DDAttributesCollection ac, JsonWriter writer)
        {
            writer.WriteStartObject();
            JsonSerialize(ac, writer);
            writer.WriteEndObject();
        }
        /// <summary>
        /// Serializes the specified DDAttributesCollection and writes the Json document to a Json writer
        /// </summary>
        /// <param name="ac">the attributes collection to serialize</param>
        /// <param name="writer">Json writer used to write the Json document.</param>
        internal static void JsonSerialize(DDAttributesCollection ac, JsonWriter writer)
        {
            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            if (ac.Count != 0)
            {
                writer.WritePropertyName(DDSchema.JSON_SERIALIZE_NODE_ATTRIBUTE_COLLECTION);
                writer.WriteStartArray();

                foreach (var a in ac)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(a.Key);
                    writer.WriteStartObject();
                    if (a.Value != null) DDValueSje.JsonSerialize(a.Value, writer);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(TextReader tr)
        {
            var ac = new DDAttributesCollection();
            Deserialize(ac, tr);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDAttributesCollection ac, TextReader tr)
        {
            using (JsonReader reader = new JsonTextReader(tr))
            {
                Deserialize(ac, reader);
            }
        }
        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(string s)
        {
            var ac = new DDAttributesCollection();
            Deserialize(ac, s);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDAttributesCollection ac, string s)
        {
            var sr = new StringReader(s);
            using (JsonReader reader = new JsonTextReader(sr))
            {
                Deserialize(ac, reader);
            }
        }
        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDAttributesCollection </returns>
        public static DDAttributesCollection Deserialize(Stream s)
        {
            var ac = new DDAttributesCollection();
            Deserialize(ac, s);
            return ac;
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDAttributesCollection ac, Stream s)
        {
            using (StreamReader sr = new StreamReader(s))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    ac.Deserialize(reader);
                }
            }
        }
        /// <summary>
        ///  Generates an new DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="reader">Json stream reader</param>
        /// <returns>an new DDAttributesCollection</returns>
        public static DDAttributesCollection Deserialize(JsonReader reader)
        {
            var ac = new DDAttributesCollection();
            Deserialize(ac, reader);
            return ac;
        }
                /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">Json stream reader</param>
        public static void Deserialize(this DDAttributesCollection ac, JsonReader reader)
        {
            string prevName = null;
            while (reader.Read())
            {
                if ((reader.TokenType == JsonToken.StartArray) && (prevName == DDSchema.JSON_SERIALIZE_NODE_ATTRIBUTE_COLLECTION))  // attributes collection
                {
                    JsonDeserialize(ac, reader);
                    break;
                }
                //  save current values
                if (reader.TokenType == JsonToken.PropertyName) prevName = reader.Value.ToString();
            }
        }
        /// <summary>
        /// Adds an new items to specified DDAttributesCollection from its Json representation.
        /// </summary>
        /// <param name="ac">The deserialized attributes collection.</param>
        /// <param name="s">Json stream reader</param>
        internal static void JsonDeserialize(this DDAttributesCollection ac, JsonReader reader)
        {
            string prevValueString = null;
            string prevName = null;

            JsonToken prevTokenType = JsonToken.None;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray) break; // end list of attributes

                if ((reader.TokenType == JsonToken.PropertyName) && (prevTokenType == JsonToken.StartObject) && (reader.Value != null))
                {
                    ac.Add(reader.Value.ToString(), DDValueSje.Deserialize(reader));
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
        }
        #endregion Deserialize
    }
}
