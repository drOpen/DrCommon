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
        public static void Serialyze(this DDAttributesCollection ac, TextWriter tw)
        {
            using (JsonWriter writer = new JsonTextWriter(tw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                ac.Serialyze(writer);
            }
        }

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
        public static DDAttributesCollection Deserialyze(TextReader tr)
        {
            using (JsonReader reader = new JsonTextReader(tr))
            {
                return Deserialyze(reader);
            }
        }
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
