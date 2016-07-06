/*
  DDValue.cs -- stored data of the 'DrData' general purpose Data abstraction layer 1.0.1, October 5, 2013
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
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
using DrOpen.DrCommon.DrData.Res;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using DrOpen.DrCommon.DrData.Exceptions;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// Data warehouse
    /// </summary>
    [Serializable]
    public class DDValue : IEquatable<DDValue>, ICloneable, IComparable, ISerializable, IXmlSerializable
    {
        #region const
        public const string SerializePropNameValue = "Value";
        public const string SerializePropNameType = "Type";
        public const string SerializePropNameSize = "Size";
        public const string SerializeDateTimeFormat = "o"; //ISO 8601 format
        public const string SerializeRoundTripFormat = "r"; //round-trip format for Single, Double, and BigInteger types.
        #endregion const
        #region DDValue
        /// <summary>
        /// Create empty value
        /// There are nullable type and data.
        /// </summary>
        public DDValue() { }
        /// <summary>
        /// Create value with data. If type of object isn't supported throw application exception
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">data</param>
        public DDValue(object value)
        {
            SetValue(value);
        }
        #endregion DDValue
        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, 
        /// if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (Type == null) return; // if data is null

            writer.WriteAttributeString(SerializePropNameType, Type.ToString());
            if (Size != 0) writer.WriteAttributeString(SerializePropNameSize, Size.ToString()); // write size only for none empty objects
            if (IsThisTypeXMLSerialyzeAsArray(type))
            {
                foreach (var element in ToStringArray())
                {
                    writer.WriteStartElement(SerializePropNameValue);
                    writer.WriteString(element);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(ToString());
            }
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {

            var typeNameSelf = this.GetType().Name;

            reader.MoveToContent();
            type = null;

            var t = reader.GetAttribute(SerializePropNameType);
            if (string.IsNullOrEmpty(t))
            {
                data = null;
                return; // null object
            }
            data = new byte[] { };
            this.type = Type.GetType(t);
            if (IsThisTypeXMLSerialyzeAsArray(type) == false)
            {
                this.data = GetByteArrayByTypeFromString(type, GetXmlElementValue(reader)); // read node value for none array types
            }
            else
            {
                var value = ReadXmlValueArray(reader);
                if (value != null) this.data = GetByteArray(Type, typeof(string[]) == Type ? ConvertObjectArrayToStringArray(value) : value);
            }

            if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == typeNameSelf)) reader.ReadEndElement(); // Need to close the opened element </DDValue>, only self
        }

        /// <summary>
        /// Return XML Element value.
        /// Open XML Element if needed, read value, close element and return value 
        /// </summary>
        /// <param name="reader">Xml stream reder</param>
        /// <returns>XML Element value</returns>
        protected static string GetXmlElementValue(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element) reader.ReadStartElement();
            var value = reader.Value; // read node value for none array types
            if (reader.HasValue) // read value of element if there is
            {
                reader.Read(); // read value of element
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element
            }
            return value;
        }

        /// <summary>
        /// Read XML Subling Nodes for array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual object[] ReadXmlValueArray(XmlReader reader)
        {
            var dDValueTypeName = typeof(DDValue).Name;
            int i = 0;
            object[] value = null;
            var elementType = type.GetElementType();

            reader.Read();
            var initialDepth = reader.Depth;
            if (reader.NodeType == XmlNodeType.None) return value; // Exit for element without child <DDvalue Type="String[]"/>

            while ((reader.Depth >= initialDepth)) // do all childs
            {
                if ((reader.IsStartElement(SerializePropNameValue) == false) || (reader.Depth > initialDepth))
                {
                    reader.Skip(); // Skip none <Value> elements with childs and subchilds <Value> elements 'Deep proptection'
                    if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement(); // need to close the opened element after deep protection
                }
                else
                {
                    Array.Resize(ref value, i + 1);
                    value[i] = ConvertStringToSpecifiedTypeObject(elementType, GetXmlElementValue(reader));
                    i++;
                }
                reader.MoveToContent();
            }
            return value;
        }

        /// <summary>
        /// Returns new string [] from object [].
        /// This function call ToString() for each element for new array
        /// </summary>
        /// <param name="array">object[]</param>
        /// <returns>Retrun new string[]</returns>
        protected string[] ConvertObjectArrayToStringArray(Array array)
        {
            var result = new string[array.Length];
            var i = 0;
            foreach (var item in array)
            {
                result[i] = item.ToString();
                i++;
            }
            return result;
        }

        /// <summary>
        /// Return true if this type should be serialization per each array element
        /// </summary>
        /// <param name="type">Type to serialyze</param>
        /// <returns>Return true if this type should be serialization per each array element, otherwise: false</returns>
        /// <example>For example: byte[] should be serialize as HEX single string therefore return value is false for this type, all other arrays should be serialized per elements</example>
        protected static bool IsThisTypeXMLSerialyzeAsArray(Type type)
        {
            return ((type.IsArray) && (type != typeof(byte[])));
        }


        #endregion IXmlSerializable
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDValue(SerializationInfo info, StreamingContext context)
        {
            this.type = (Type)info.GetValue(SerializePropNameType, typeof(Type));
            this.data = (byte[])info.GetValue(SerializePropNameValue, typeof(byte[]));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(SerializePropNameType, this.type, typeof(Type));
            info.AddValue(SerializePropNameValue, this.data, typeof(byte[]));
        }
        #endregion ISerializable
        #region Properties
        protected byte[] data;
        protected Type type;
        /// <summary>
        /// the type of data stored
        /// </summary>
        public virtual Type Type
        {
            get { return type; }
            private set
            {
                if (ValidateType(value)) type = value;
            }
        }

        /// <summary>
        /// size in bytes of the stored data
        /// </summary>
        public virtual long Size
        {
            get
            {
                if (data == null) return 0;
                return data.Length;
            }
        }
        #endregion Properties
        #region SetValue
        /// <summary>
        /// Set value by object type
        /// </summary>
        /// <param name="value">object</param>
        public void SetValue(object value)
        {
            if (value == null) // support null value
            {
                this.data = null;
                this.Type = null;
            }
            else
            {
                var t = value.GetType();
                try
                {
                    data = GetByteArray(value);
                }
                catch
                {
                    t = null; // set null because cannot set value
                    throw;
                }
                finally
                {
                    this.Type = t;
                }
            }
        }
        /// <summary>
        /// Set value from HEX string with specified type
        /// </summary>
        /// <param name="t">type of data</param>
        /// <param name="hex">HEX</param>
        public virtual void SetHEXValue(Type t, string hex)
        {
            Type = t;
            data = HEX(hex);
        }
        #endregion SetValue
        #region GetByteArray
        /// <summary>
        /// Convert oject supported type to byte array (byte []).
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <returns>byte []</returns>
        public static byte[] GetByteArray(object value)
        {
            var t = value.GetType();
            if (!ValidateType(t)) throw new DDTypeIncorrectException(t.ToString());
            return GetByteArray(t, value);
        }
        /// <summary>
        /// Convert object by specified type to byte array (byte []).
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="type">convert object as the specified data type</param>
        /// <returns>byte []</returns>
        public static byte[] GetByteArray(Type type, object value)
        {
            // if (type == typeof(byte[])) return (byte[])value; // it does not work
            if (type == typeof(byte)) return new[] { (byte)value };

            if (type == typeof(string)) return Encoding.UTF8.GetBytes(value.ToString());
            if (type == typeof(DateTime)) return BitConverter.GetBytes(((DateTime)value).ToBinary());
            if (type == typeof(bool)) return BitConverter.GetBytes((bool)value);
            if (type == typeof(char)) return BitConverter.GetBytes((char)value);
            if (type == typeof(float)) return BitConverter.GetBytes((float)value);
            if (type == typeof(double)) return BitConverter.GetBytes((double)value);
            if (type == typeof(short)) return BitConverter.GetBytes((short)value);
            if (type == typeof(int)) return BitConverter.GetBytes((int)value);
            if (type == typeof(long)) return BitConverter.GetBytes((long)value);
            if (type == typeof(ushort)) return BitConverter.GetBytes((ushort)value);
            if (type == typeof(uint)) return BitConverter.GetBytes((uint)value);
            if (type == typeof(ulong)) return BitConverter.GetBytes((ulong)value);
            if (type == typeof(Guid)) return ((Guid)value).ToByteArray();

            if (type == typeof(string[])) return Encoding.UTF8.GetBytes(string.Join("\0", (string[])value));
            if (type.IsArray) return JoinByteArray((Array)value);

            throw new DDTypeIncorrectException(type.ToString());
        }

        /// <summary>
        /// Convert string to byte[] by specified type.
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="type">convert by specified type</param>
        /// <param name="value">string to convert</param>
        /// <returns>Converted byte[] by specified type</returns>
        protected static byte[] GetByteArrayByTypeFromString(Type type, string value)
        {
            //if (type == typeof(byte[])) return HEX(value);
            //if (type == typeof(byte)) return new[] { Convert.ToByte(value) };
            return GetByteArray(ConvertStringToSpecifiedTypeObject(type, value));
        }

        /// <summary>
        /// Convert string to specified type.
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="type">convert to specified type</param>
        /// <param name="value">string to convert</param>
        /// <returns>Converted object by specified type</returns>
        protected static object ConvertStringToSpecifiedTypeObject(Type type, string value)
        {

            if (type == typeof(byte[])) return HEX(value);
            if (type == typeof(byte)) return Convert.ToByte(value);

            if (type == typeof(string)) return value.ToString();
            if (type == typeof(DateTime)) return Convert.ToDateTime(value);
            if (type == typeof(bool)) return Convert.ToBoolean(value);
            if (type == typeof(char)) return Convert.ToChar(value);
            if (type == typeof(float)) return Convert.ToSingle(value);
            if (type == typeof(double)) return Convert.ToDouble(value);
            if (type == typeof(short)) return Convert.ToInt16(value);
            if (type == typeof(int)) return Convert.ToInt32(value);
            if (type == typeof(long)) return Convert.ToInt64(value);
            if (type == typeof(ushort)) return Convert.ToUInt16(value);
            if (type == typeof(uint)) return Convert.ToUInt32(value);
            if (type == typeof(ulong)) return Convert.ToUInt64(value);
            if (type == typeof(Guid)) return new Guid(value);
            throw new DDTypeIncorrectException(type.ToString());
        }

        /// <summary>
        /// Returns an array of data as a byte[]. 
        /// Array of the bellow types  char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double
        /// </summary>
        /// <param name="value"></param>
        /// <returns>byte []</returns>
        protected static byte[] JoinByteArray(Array value)
        {
            if ((value).Length == 0) return new byte[] { };
            byte[] result = null;
            int iElement = 0;
            foreach (var item in value)
            {
                var tmp = GetByteArray(item);
                if (iElement == 0) result = new byte[GetArraySize(value)]; // allocate memmory by first element of array
                Array.Copy(tmp, 0, result, iElement * tmp.Length, tmp.Length);
                iElement += 1;
            }
            return result;
        }
        #endregion GetByteArray
        #region implicit operator
        #region -> DDValue
        public static implicit operator DDValue(int value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(int[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(uint value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(uint[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(long value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(long[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(ulong value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(ulong[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(short value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(short[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(ushort value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(ushort[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(double value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(double[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(float value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(float[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(bool value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(bool[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(Guid value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(Guid[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(char value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(char[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(DateTime value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(DateTime[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(string value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(string[] value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(byte value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(byte[] value)
        {
            return new DDValue(value);
        }
        #endregion -> DDValue
        #region  DDValue ->
        public static implicit operator int(DDValue value)
        {
            return value.GetValueAsInt();
        }
        public static implicit operator int[](DDValue value)
        {
            return value.GetValueAsIntArray();
        }
        public static implicit operator uint(DDValue value)
        {
            return value.GetValueAsUInt();
        }
        public static implicit operator uint[](DDValue value)
        {
            return value.GetValueAsUIntArray();
        }
        public static implicit operator long(DDValue value)
        {
            return value.GetValueAsLong();
        }
        public static implicit operator long[](DDValue value)
        {
            return value.GetValueAsLongArray();
        }
        public static implicit operator ulong(DDValue value)
        {
            return value.GetValueAsULong();
        }
        public static implicit operator ulong[](DDValue value)
        {
            return value.GetValueAsULongArray();
        }
        public static implicit operator short(DDValue value)
        {
            return value.GetValueAsShort();
        }
        public static implicit operator short[](DDValue value)
        {
            return value.GetValueAsShortArray();
        }
        public static implicit operator ushort(DDValue value)
        {
            return value.GetValueAsUShort();
        }
        public static implicit operator ushort[](DDValue value)
        {
            return value.GetValueAsUShortArray();
        }
        public static implicit operator double(DDValue value)
        {
            return value.GetValueAsDouble();
        }
        public static implicit operator double[](DDValue value)
        {
            return value.GetValueAsDoubleArray();
        }
        public static implicit operator float(DDValue value)
        {
            return value.GetValueAsFloat();
        }
        public static implicit operator float[](DDValue value)
        {
            return value.GetValueAsFloatArray();
        }
        public static implicit operator bool(DDValue value)
        {
            return value.GetValueAsBool();
        }
        public static implicit operator bool[](DDValue value)
        {
            return value.GetValueAsBoolArray();
        }
        public static implicit operator Guid(DDValue value)
        {
            return value.GetValueAsGuid();
        }
        public static implicit operator Guid[](DDValue value)
        {
            return value.GetValueAsGuidArray();
        }
        public static implicit operator char(DDValue value)
        {
            return value.GetValueAsChar();
        }
        public static implicit operator char[](DDValue value)
        {
            return value.GetValueAsCharArray();
        }
        public static implicit operator DateTime(DDValue value)
        {
            return value.GetValueAsDateTime();
        }
        public static implicit operator DateTime[](DDValue value)
        {
            return value.GetValueAsDateTimeArray();
        }
        public static implicit operator string(DDValue value)
        {
            return value.GetValueAsString();
        }
        public static implicit operator string[](DDValue value)
        {
            return value.GetValueAsStringArray();
        }
        public static implicit operator byte(DDValue value)
        {
            return value.GetValueAsByte();
        }
        public static implicit operator byte[](DDValue value)
        {
            return value.GetValueAsByteArray();
        }
        #endregion  DDValue ->
        #endregion implicit operator
        #region GetValue

        /// <summary>
        /// Get value by type
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue()
        {

            
           /* 
            if (Type.IsArray )
                return GetValueAsArray<Type>();
            else
                return GetValueAs<Type>();
            */
            

            if (Type == typeof(string)) return GetValueAsString();
            if (Type == typeof(string[])) return GetValueAsStringArray();
            if (Type == typeof(DateTime)) return GetValueAsDateTime();
            if (Type == typeof(DateTime[])) return GetValueAsDateTimeArray();
            if (Type == typeof(byte)) return GetValueAsByte();
            if (Type == typeof(byte[])) return GetValueAsByteArray();
            if (Type == typeof(short)) return GetValueAsShort();
            if (Type == typeof(short[])) return GetValueAsShortArray();
            if (Type == typeof(ushort)) return GetValueAsUShort();
            if (Type == typeof(ushort[])) return GetValueAsUShortArray();
            if (Type == typeof(int)) return GetValueAsInt();
            if (Type == typeof(int[])) return GetValueAsIntArray();
            if (Type == typeof(uint)) return GetValueAsUInt();
            if (Type == typeof(uint[])) return GetValueAsUIntArray();
            if (Type == typeof(long)) return GetValueAsLong();
            if (Type == typeof(long[])) return GetValueAsLongArray();
            if (Type == typeof(ulong)) return GetValueAsULong();
            if (Type == typeof(ulong[])) return GetValueAsULongArray();
            if (Type == typeof(char)) return GetValueAsChar();
            if (Type == typeof(char[])) return GetValueAsCharArray();
            if (Type == typeof(float)) return GetValueAsFloat();
            if (Type == typeof(float[])) return GetValueAsFloatArray();
            if (Type == typeof(double)) return GetValueAsDouble();
            if (Type == typeof(double[])) return GetValueAsDoubleArray();
            if (Type == typeof(bool)) return GetValueAsBool();
            if (Type == typeof(bool[])) return GetValueAsBoolArray();
            if (Type == typeof(Guid)) return GetValueAsGuid();
            if (Type == typeof(Guid[])) return GetValueAsGuidArray();
            if (Type == null) return null;

            throw new DDTypeIncorrectException(Type.ToString());
        }
        /// <summary>
        /// Get value as array by specified type
        /// </summary>
        /// <typeparam expected="T">The type of return array value.</typeparam>
        /// <returns>Returns array by specified type</returns>
        public virtual T[] GetValueAsArray<T>()
        {
            if (typeof(T) == typeof(string)) //string array convertion
            {
                if (Size == 0) return new T[] { };
                return (T[])((object)GetValueAsString().Split('\0'));
            }

            var sizePerElements = GetPrimitiveSize(typeof(T));
            var result = new T[this.Size / sizePerElements];
            for (int i = 0; i < this.Size / sizePerElements; i++)
            {
                var tmp = new byte[sizePerElements];
                Array.Copy(data, i * sizePerElements, tmp, 0, sizePerElements);
                result[i] = (T)GetValueObjByType(typeof(T), tmp);
            }
            return result;
        }
        /// <summary>
        /// Get value by specified type
        /// </summary>
        /// <typeparam expected="T">The type of return value.</typeparam>
        /// <returns>Returns value by specified type</returns>
        public virtual T GetValueAs<T>()
        {
            return (T)GetValueObjByType(typeof(T), data);
        }

        protected static object GetValueObjByType(Type type, byte[] data)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (data == null) return null; // check Nullable type
                type = Nullable.GetUnderlyingType(type); // Returns the underlying type argument of the specified nullable type. 
            }
            
            if (type == typeof(string)) return Encoding.UTF8.GetString(data);

            if (type == typeof(DateTime)) return DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            if (type == typeof(byte)) return data[0];
            if (type == typeof(byte[])) return data;

            if (type == typeof(short)) return BitConverter.ToInt16(data, 0);
            if (type == typeof(ushort)) return BitConverter.ToUInt16(data, 0);
            if (type == typeof(int)) return BitConverter.ToInt32(data, 0);
            if (type == typeof(uint)) return BitConverter.ToUInt32(data, 0);
            if (type == typeof(long)) return BitConverter.ToInt64(data, 0);
            if (type == typeof(ulong)) return BitConverter.ToUInt64(data, 0);
            if (type == typeof(char)) return BitConverter.ToChar(data, 0);
            if (type == typeof(float)) return BitConverter.ToSingle(data, 0);
            if (type == typeof(double)) return BitConverter.ToDouble(data, 0);
            if (type == typeof(bool))  return BitConverter.ToBoolean(data, 0);
            if (type == typeof(Guid)) return new Guid(data);

            throw new DDTypeIncorrectException(type.ToString());
        }

        public virtual byte[] GetValueAsByteArray()
        {
            return data;
        }
        public virtual byte GetValueAsByte()
        {
            return data[0];
        }

        public virtual string GetValueAsString()
        {
            return GetValueAs<string>(); //Encoding.UTF8.GetString(data);
        }
        public virtual string[] GetValueAsStringArray()
        {
            return GetValueAsArray<string>();
        }
        public virtual DateTime GetValueAsDateTime()
        {
            return GetValueAs<DateTime>();  //DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }
        public virtual DateTime[] GetValueAsDateTimeArray()
        {
            return GetValueAsArray<DateTime>();
        }

        public virtual bool GetValueAsBool()
        {
            return GetValueAs<bool>();
        }
        public virtual bool[] GetValueAsBoolArray()
        {
            return GetValueAsArray<bool>();
        }

        public virtual Guid GetValueAsGuid()
        {
            return GetValueAs<Guid>();
        }
        public virtual Guid[] GetValueAsGuidArray()
        {
            return GetValueAsArray<Guid>();
        }

        public virtual char GetValueAsChar()
        {
            return GetValueAs<char>(); //BitConverter.ToChar(data, 0);
        }
        public virtual char[] GetValueAsCharArray()
        {
            return GetValueAsArray<char>();
        }

        public virtual double GetValueAsDouble()
        {
            return GetValueAs<double>(); //BitConverter.ToDouble(data, 0);
        }
        public virtual double[] GetValueAsDoubleArray()
        {
            return GetValueAsArray<double>();
        }

        public virtual float GetValueAsFloat()
        {
            return GetValueAs<float>();// BitConverter.ToSingle(data, 0);
        }
        public virtual float[] GetValueAsFloatArray()
        {
            return GetValueAsArray<float>();
        }

        public virtual short GetValueAsShort()
        {
            return GetValueAs<short>(); //BitConverter.ToInt16(data, 0);
        }
        public virtual short[] GetValueAsShortArray()
        {
            return GetValueAsArray<short>();
        }

        public virtual ushort GetValueAsUShort()
        {
            return GetValueAs<ushort>();// BitConverter.ToUInt16(data, 0);
        }
        public virtual ushort[] GetValueAsUShortArray()
        {
            return GetValueAsArray<ushort>();
        }

        public virtual int GetValueAsInt()
        {
            return GetValueAs<int>(); // BitConverter.ToInt32(data, 0);
        }
        public virtual int[] GetValueAsIntArray()
        {
            return GetValueAsArray<int>();
        }

        public virtual uint GetValueAsUInt()
        {
            return GetValueAs<uint>();// BitConverter.ToUInt32(data, 0);
        }
        public virtual uint[] GetValueAsUIntArray()
        {
            return GetValueAsArray<uint>();
        }

        public virtual long GetValueAsLong()
        {
            return GetValueAs<long>();//BitConverter.ToInt64(data, 0);
        }
        public virtual long[] GetValueAsLongArray()
        {
            return GetValueAsArray<long>();
        }

        public virtual ulong GetValueAsULong()
        {
            return GetValueAs<ulong>();  //BitConverter.ToUInt64(data, 0);
        }
        public virtual ulong[] GetValueAsULongArray()
        {
            return GetValueAsArray<ulong>();
        }
        /// <summary>
        /// Return value as string as HEX
        /// </summary>
        /// <returns></returns>
        public virtual string GetValueAsHEX()
        {
            return HEX(data);
        }
        #endregion GetValue
        #region ValidateType
        /// <summary>
        /// Checks the type of object.
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">object whose type will be validate</param>
        /// <returns>true if type of object is supported, otherwise false</returns>
        public static bool ValidateType(object value)
        {
            return ValidateType(value.GetType());
        }

        /// <summary>
        /// Checks the type.
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types. 
        /// Nullable array type is not supported.
        /// </summary>
        /// <param name="type">type for validation</param>
        /// <returns>rue if type is supported, otherwise false</returns>
        public static bool ValidateType(Type type)
        {

            if (type == null) return false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (type.IsArray) return false; // nullable array type is not supported
                type = Nullable.GetUnderlyingType(type); // Returns the underlying type argument of the specified nullable type. 
            }
            if (type.IsArray) type = type.GetElementType(); // returns element type

            return (
                    type == typeof(int) ||
                    type == typeof(string) ||
                    type == typeof(DateTime) ||
                    type == typeof(char) ||
                    type == typeof(bool) ||
                    type == typeof(byte) ||
                    type == typeof(short) ||
                    type == typeof(float) ||
                    type == typeof(long) ||
                    type == typeof(ushort) ||
                    type == typeof(uint) ||
                    type == typeof(ulong) ||
                    type == typeof(double) ||
                    type == typeof(Guid)
                );
        }
        #endregion ValidateType
        #region ObjectSize
        /// <summary>
        /// Return the sizePerElements of occupied space in the memory of the object.
        /// <exception cref="ApplicationException">If the object type is not supported throw application exception</exception>
        /// </summary>
        /// <param name="obj">object for analyze</param>
        /// <returns>Size in value</returns>
        public static int GetObjSize(object obj)
        {
            var type = obj.GetType();
            if (type == typeof(string)) return Encoding.UTF8.GetBytes(obj.ToString()).Length;
            if (type == typeof(byte[])) return ((byte[])obj).Length;
            if (type == typeof(Guid)) return ((Guid)obj).ToByteArray().Length;
            if (type.IsArray) return GetArraySize((Array)obj);

            return GetPrimitiveSize(type);
        }
        /// <summary>
        /// Returns the sizePerElements of occupied space in the memory of the primitive.
        /// <exception cref="ApplicationException">If the object type is not supported throw application exception</exception>
        /// </summary>
        /// <param name="type">Type for analyze</param>
        /// <returns>Size for this type</returns>
        protected static int GetPrimitiveSize(Type type)
        {
            if (type == typeof(byte)) return sizeof(byte);
            if (type == typeof(short)) return sizeof(short);
            if (type == typeof(ushort)) return sizeof(ushort);
            if (type == typeof(int)) return sizeof(int);
            if (type == typeof(uint)) return sizeof(uint);
            if (type == typeof(long)) return sizeof(long);
            if (type == typeof(ulong)) return sizeof(ulong);
            if (type == typeof(char)) return sizeof(char);
            if (type == typeof(float)) return sizeof(float);
            if (type == typeof(double)) return sizeof(double);
            if (type == typeof(bool)) return sizeof(bool);
            if (type == typeof(DateTime)) return sizeof(Int64);
            if (type == typeof(Guid)) return Guid.Empty.ToByteArray().Length;
            throw new DDTypeIncorrectException(type.ToString());

        }
        /// <summary>
        /// Return the sizePerElements of occupied space in the memory of the array.
        /// <exception cref="ApplicationException">If the object type is not supported throw application exception.</exception>
        /// </summary>
        /// <param name="value">array to analyze</param>
        /// <returns>Size in value</returns>
        public static int GetArraySize(Array value)
        {
            if (value.GetType() == typeof(string[])) return GetByteArray(value).Length;
            foreach (var item in value)
            {
                return GetObjSize(item) * value.Length;
            }
            return 0;
        }

        #endregion ObjectSize
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance. The resulting clone must be of the same type as, or compatible with, the original instance.
        /// The resulting clone must be of the same type as, or compatible with, the original instance.
        /// </summary>
        /// <returns>A new DDAtribute that is a copy of this instance.</returns>
        public virtual DDValue Clone()
        {
            var a = new DDValue();
            if (Type != null) a.type = Type;
            if (data != null) a.data = (byte[])data.Clone();
            return a;
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new DDAtribute that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion ICloneable
        #region GetHashCode
        /// <summary>
        /// Return HashCode from blob -> byte[]
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (data == null) return 0;
            return data.GetHashCode();
        }
        #endregion GetHashCode
        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// The type of comparison between the current instance and the obj parameter depends on whether the current instance is a reference type or a value type.
        /// </summary>
        /// <param name="other">The object to compare with the current object. </param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        override public bool Equals(object other)
        {
            if (other.GetType() != typeof(DDValue)) return false;
            return Equals((DDValue)other);

        }
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// The type of comparison between the current instance and the obj parameter depends on whether the current instance is a reference type or a value type.
        /// </summary>
        /// <param name="other">The object to compare with the current object. </param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public virtual bool Equals(DDValue other)
        {
            return base.Equals(other);
            // ***********************
            // ToDo
            //return(Compare(this, other) == 0);
        }
        #endregion IEquatable
        #region ==, != operators
        /// <summary>
        /// Compare both values and return true if type and data are same otherwise return false.
        /// If both values are null - return true, if only one of them are null, return false. if the data types are different - return false
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if type and data are same otherwise return false</returns>
        public static bool operator ==(DDValue value1, DDValue value2)
        {
            return (Compare(value1, value2) == 0);
        }
        public static bool operator !=(DDValue value1, DDValue value2)
        {
            return (!(value1 == value2));
        }
        #endregion ==, != operators
        #region IComparable
        /// <summary>
        /// Compares the two DDValue of the same type and returns an integer that indicates whether the current instance precedes, follows, 
        /// or occurs in the same position in the sort order as the other object.
        /// The both null object is equal and return value will be Zero.
        /// </summary>
        /// <param name="value1">First DDValue to compare</param>
        /// <param name="value2">Second DDValue to compare</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - the both DDValue have some type and value.
        /// One - type or value is not equal.</returns>
        public static int Compare(DDValue value1, DDValue value2)
        {
            if (((object)value1 == null) && ((object)value2 == null)) return 0; // if both are null -> return true
            if (((object)value1 == null) || ((object)value2 == null)) return 1; // if only one of them are null ->  return false
            if ((value1.Type == null) || (value2.Type == null))
            {
                if (!((value1.Type == null) && (value2.Type == null))) return 1;
            }
            else
            {
                if (value1.Type != value2.Type) return 1; // if the data types are different ->  return false
            }

            if (value1.Size != value2.Size) return 1; // if the sizes are different ->  return false

            if ((value1.data == null) || (value2.data == null))
            {
                if (!((value1.data == null) && (value2.data == null))) return 1;
            }
            else
            {
                for (int i = 0; i < value1.data.Length; i++)
                {
                    if (value1.data[i] != value2.data[i]) return 1; // it is faster (~10%) than:
                    // if (value1.data[i].Equals(value2.data[i]) == false) return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Compares the current DDValue instance with another object of the same type and returns an integer that indicates whether the current instance precedes, 
        /// follows, or occurs in the same position in the sort order as the other object.
        /// The both null object is equal and return value will be Zero.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - This instance occurs in the same position in the sort order as obj.
        /// One - This instance follows obj in the sort order.</returns>
        public virtual int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DDValue)) return 1;
            return Compare(this, (DDValue)obj);
        }

        #endregion
        #region ToString
        /// <summary>
        /// Convert object to string with some tricks. 
        /// <see cref="DateTime "/> - ISO 8601 format; round-trip format for Single, Double, and BigInteger types.
        /// </summary>
        /// <param name="type">type of object</param>
        /// <param name="value">value to convert</param>
        /// <returns>value as string</returns>
        protected static string GetObjAsStringByType(Type type, object value)
        {
            if (type == typeof(DateTime)) return ((DateTime)value).ToString(SerializeDateTimeFormat);
            // Workarround for MS ToString() issue for Single, Double, and BigInteger types.
            // for example: Single.MaxValue -> 3.40282347E+38, after ToString() -> 3.402823E+38, - lost data
            // The round-trip ("R") format: http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx#RFormatString
            if (type == typeof(Single)) return ((Single)value).ToString(SerializeRoundTripFormat);
            if (type == typeof(Double)) return ((Double)value).ToString(SerializeRoundTripFormat);
            if (type == typeof(float)) return ((float)value).ToString(SerializeRoundTripFormat);
            return value.ToString();
        }

        /// <summary>
        /// Return data as string.
        /// <example>Sample: DateTime -> ISO 8601 format, bool: true -> "True"; int: 123 -> "123"; string: "test" -> "test"; byte: 255 -> "255"; byte[]: 128 15 -> "800F"</example>
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            if (data == null) return string.Empty;
            if (Type == typeof(byte[])) return HEX(data);
            if (Type == typeof(string[])) return string.Join("\0", (string[])GetValue());

            return GetObjAsStringByType(Type, GetValue());
        }
        /// <summary>
        /// Return data as string array.
        /// <example>Sample: DateTime -> ISO 8601 format, bool: true -> "True"; int: 123 -> "123"; string: "test" -> "test"; byte: 255 -> "255"; byte[]: 128 15 -> "800F"</example>
        /// </summary>
        /// <returns></returns>
        public virtual string[] ToStringArray()
        {
            if (data == null) return new string[] { };
            if (Type == typeof(byte[])) return new string[] { HEX(data) };
            if (Type.IsArray)
            {
                var array = (Array)GetValue();
                var result = new string[array.Length];
                var elementType = Type.GetElementType();
                int i = 0;
                foreach (var item in array)
                {
                    result[i] = GetObjAsStringByType(elementType, item);
                    i++;
                }
                return result;
            }
            return new string[] { GetValue().ToString() };
        }
        #endregion  ToString
        #region HEX
        /// <summary>
        /// Convert byte[] to HEX string
        /// </summary>
        /// <param name="byteArray">byte array to convertion</param>
        /// <returns></returns>
        public static string HEX(byte[] byteArray)
        {
            char[] charArray = new char[byteArray.Length * 2];
            int bite;
            for (int i = 0; i < byteArray.Length; i++)
            {
                bite = byteArray[i] >> 4;
                charArray[i * 2] = (char)(55 + bite + (((bite - 10) >> 31) & -7));
                bite = byteArray[i] & 0xF;
                charArray[i * 2 + 1] = (char)(55 + bite + (((bite - 10) >> 31) & -7));
            }
            return new string(charArray);
        }
        /// <summary>
        /// Convert byte[] to HEX string
        /// </summary>
        /// <param name="hex">byte array to convertion</param>
        /// <returns></returns>
        public static byte[] HEX(string hex)
        {
            if (hex.Length % 2 > 0) throw new DDValueException(hex, string.Format(Msg.INCORRECT_HEX, hex));
            int iSize = hex.Length / 2;
            var bytes = new byte[iSize];
            for (int i = 0; i < iSize; i++)
            {
                bytes[i] = Convert.ToByte(hex[i * 2].ToString() + hex[i * 2 + 1].ToString(), 16);

            }
            return bytes;
        }
        #endregion HEX
        #region Transformation
        /// <summary>
        /// Self transformation from string or string array type to specified type. Change themselves and their data type. Retruns itself after covertion.<para> </para>
        /// If original type is not string or string array the <exception cref="DDTypeConvertException">DDTypeConvertExceptions</exception> will be thrown.<para> </para>
        /// If original type is null the <exception cref="DDTypeNullException">DDTypeNullException</exception> will be thrown.<para> </para>
        /// </summary>
        /// <param name="newType">convert to specified type</param>
        /// <returns></returns>
        public DDValue SelfTransformFromStringTo(Type newType)
        {
            if (this.Type == null) throw new DDTypeNullException(Msg.CANNOT_TRANSFORM_NULL_TYPE);
            if ((this.Type != typeof(string) && (this.Type != typeof(string[])))) throw new DDTypeConvertException(this.type.Name, newType.Name, string.Format(Msg.CANNOT_CONVERT_FROM_NONE_STRING_OR_STRING_ARRAY_TYPE, newType.Name, this.type.Name));
            if (this.Type.IsArray != newType.IsArray) throw new DDTypeConvertException(this.type.Name, newType.Name, string.Format(Msg.CANNOT_TRANSFORM_ARRAY_TYPE_TO_NOT_ARRAY, this.type.Name, newType.Name));
            if ((newType == typeof(string) || (newType == typeof(string[])))) return this; // nothing to do
            if (data.Length > 0)
            {
                if (this.Type.IsArray)
                {
                    var items = GetValueAsStringArray();
                    var objArray = new object[items.Length];
                    int i = 0;
                    foreach (var item in items)
                    {
                        objArray[i] = ConvertStringToSpecifiedTypeObject(newType.GetElementType(), item);
                        i++;
                    }
                    this.data = GetByteArray(newType, objArray);
                }
                else
                {
                    this.data = GetByteArrayByTypeFromString(newType, GetValueAsString());
                }
            }
            this.type = newType;
            return this;
        }
        #endregion Transformation
    }
}