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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrDataSj
{
    /// <summary>
    /// provides json formating serialization and deserialization for DDValue of the 'DrData'
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
        /// returns/unboxes DDValue 
        /// </summary>
        /// <returns></returns>
        public DDValue GetDDValue()
        {
            return this.v;
        }

        private DDValue v;

        #region Serialize
        /// Serializes the DDValueSj and writes the Json document to a text writer
        /// </summary>
        /// <param name="tw">text writer used to write the Json document.</param>
        public void Serialize(TextWriter tw)
        {
            this.v.Serialize(tw);
        }
        /// <summary>
        /// Serializes the DDValueSj and writes the Json document to a string builder
        /// </summary>
        /// <param name="sb">string builder used to write the Json document.</param>
        public void Serialize(StringBuilder sb)
        {
            this.v.Serialize(sb);
        }
        /// <summary>
        /// Serializes the DDValueSj and writes the Json document to a stream
        /// </summary>
        /// <param name="s">stream used to write the Json document.</param>
        public void Serialize(Stream s)
        {
            this.v.Serialize(s);
        }
        /// <summary>
        /// Serializes the DDValueSj and writes the Json document to a Json writer
        /// </summary>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public void Serialize(JsonWriter writer)
        {
            this.v.Serialize(writer);
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new item to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public void Deserialize( string s)
        {
            this.v = DDValueSje.Deserialize(s);
        }
        /// <summary>
        /// Generates an new item to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public void Deserialize(TextReader tr)
        {
            this.v = DDValueSje.Deserialize(tr);
        }
        /// <summary>
        /// Adds an new items to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        public void Deserialize(Stream s)
        {
            this.v = DDValueSje.Deserialize(s);
        }
        /// <summary>
        ///  Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="reader">Json stream reader</param>
        /// <returns>an new DDValue</returns>
        public void Deserialize(JsonReader reader)
        {
            this.v = DDValueSje.Deserialize(reader);
        }
        #endregion Deserialize
        #region explicit operator
        /// <summary>
        /// boxes DDValue to for json formating serialization and deserialization
        /// </summary>
        /// <param prevName="n">DDNode for box</param>
        /// <returns></returns>
        public static explicit operator DDValueSj(DDValue v)
        {
            return (v == null ? null : new DDValueSj(v));
        }
        /// <summary>
        /// unbox DDValue
        /// </summary>
        /// <param prevName="v"></param>
        /// <returns></returns>
        public static implicit operator DDValue(DDValueSj v)
        {
            return (v == null ? null : v.v);
        }

        #endregion explicit operator
    }

    /// <summary>
    /// provides json formating serialization and deserialization for DDValue of the 'DrData'
    /// </summary>
    public static class DDValueSje
    {
        #region Serialize
        /// <summary>
        /// Serializes the specified DDValue and writes the Json document to a text writer
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="tw">text writer used to write the Json document.</param>
        public static void Serialize(this DDValue v, TextWriter tw)
        {
            using (JsonWriter writer = new JsonTextWriter(tw))
            {
                v.Serialize(writer);
            }
        }
        /// <summary>
        /// Serializes the specified DDValue and writes the Json document to a string builder
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="sb">string builder used to write the Json document.</param>
        public static void Serialize(this DDValue v, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    v.Serialize(writer);
                }
            }
        }
        /// <summary>
        /// Serializes the specified DDValue and writes the Json document to a stream
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="s">stream used to write the Json document.</param>
        public static void Serialize(this DDValue v, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    v.Serialize(writer);
                }
            }
        }
                /// <summary>
        /// Serializes the specified DDValue and writes the Json document to a Json writer
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="writer">Json writer used to write the Json document.</param>
        public static void Serialize(this DDValue v, JsonWriter writer)
        {
            writer.WriteStartObject();
            JsonSerialize(v, writer);
            writer.WriteEndObject();
        }


        /// <summary>
        /// Serializes the specified DDValue and writes the Json document to a Json writer
        /// </summary>
        /// <param name="v">the value to serialize</param>
        /// <param name="writer">Json writer used to write the Json document.</param>
        internal static void JsonSerialize(DDValue v, JsonWriter writer)
        {
            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            writer.WritePropertyName(DDSchema.JSON_SERIALIZE_ATTRIBUTE_TYPE);
            if (v.Type == null)
                writer.WriteNull();
            else
            {
                writer.WriteValue(v.Type.ToString());
                writer.WritePropertyName(DDSchema.JSON_SERIALIZE_NODE_VALUE);
                var a = IsThisTypeJsonSerializeAsArray(v.Type);
                if (a)
                {
                    writer.WriteStartArray();
                    foreach (var i in (Array)v.GetValue())
                    {
                        writer.WriteValue(i);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    if (v.Type == typeof(byte[]))
                        writer.WriteValue(v.GetValueAsHEX());
                    else
                        writer.WriteValue(v.GetValue());
                }
            }
        }
        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param prevName="type">Type to serialize</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return n is false for this type, all other arrays should be serialized per elements</example>
        private static bool IsThisTypeJsonSerializeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }
        #endregion Serialize
        #region Deserialize
        /// <summary>
        /// Generates an new item to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDValue v, string s)
        {
            v = Deserialize(s);
        }
        /// <summary>
        ///  Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="s">String that contains the Json document to deserialize.</param>
        /// <returns>an new DDValue </returns>
        public static DDValue Deserialize(string s)
        {
            var sr = new StringReader(s);

            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Deserialize(reader);
            }
        }
        /// <summary>
        /// Generates an new item to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDValue v, TextReader tr)
        {
            v = Deserialize(tr);
        }
        /// <summary>
        /// Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="tr">Text reader stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDValue</returns>
        public static DDValue Deserialize(TextReader tr)
        {
            using (JsonReader reader = new JsonTextReader(tr))
            {
                return Deserialize(reader);
            }
        }
        /// <summary>
        /// Adds an new items to specified DDValue from its Json representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        public static void Deserialize(this DDValue v, Stream s)
        {
            v = Deserialize(s);
        }

        /// <summary>
        ///  Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="s">Stream that contains the Json document to deserialize.</param>
        /// <returns>an new DDValue </returns>
        public static DDValue Deserialize(Stream s)
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
        ///  Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="v">The deserialized value.</param>
        /// <param name="s">Json stream reader</param>
        public static void Deserialize(this DDValue v, JsonReader reader)
        {
            v = Deserialize(reader);
        }
        /// <summary>
        ///  Generates an new DDValue from its Json representation.
        /// </summary>
        /// <param name="reader">Json stream reader</param>
        /// <returns>an new DDValue</returns>
        public static DDValue Deserialize(JsonReader reader)
        {
            DDValue v               = null;
            object objValue         = null;
            string[] objValueArray  = null;
            object t                = null;
            string prevValueString  = null;
            string prevName         = null;
            JsonToken prevTokenType = JsonToken.None;

            while (reader.Read())
            {
                if ((reader.TokenType == JsonToken.EndArray) || (reader.TokenType == JsonToken.EndObject))          // end value or array of values
                {
                    if ((t == null) && (objValue == null) && (objValueArray == null)) break;                        // returns null object "{ }"
                    else if ((t == null) && (objValue != null)) v = new DDValue(objValue);                          // ptoperty type isn't specified auto convertion 
                    else if ((t == null) && (objValueArray != null)) v = new DDValue(objValueArray);                // ptoperty type isn't specified auto convertion string array
                    else if ((t != null) &&  (objValueArray != null)) v = new DDValue(DDValue.ConvertFromStringArrayTo(Type.GetType(t.ToString()), objValueArray));  // array
                    else v = new DDValue(DDValue.ConvertFromStringTo(Type.GetType(t.ToString()), objValue.ToString()));
                    break; 
                }
                if ((prevTokenType == JsonToken.PropertyName) && (prevName == DDSchema.JSON_SERIALIZE_ATTRIBUTE_TYPE))
                {
                    if (reader.TokenType == JsonToken.Null) v = new DDValue(); // null type
                    t = reader.Value;
                }
                if ((prevTokenType == JsonToken.PropertyName) && (prevName == DDSchema.JSON_SERIALIZE_NODE_VALUE))
                {
                    if (reader.TokenType == JsonToken.Date)
                        objValue = ((DateTime)reader.Value).ToString(DDSchema.StringDateTimeFormat);
                    else
                        objValue = reader.Value;
                }

                if ((reader.TokenType == JsonToken.StartArray))  // array of values
                {
                    int i = 0;
                    objValueArray = new string[]{};
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndArray) break; // end list of nodes
                        Array.Resize(ref objValueArray, i + 1);
                        objValueArray[i] = reader.Value.ToString();
                        i++;
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
            return v;
        }
        #endregion Deserialize
    }

}
