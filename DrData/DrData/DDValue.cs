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
using DrOpen.DrCommon.DrData.Exceptions;
using System.Runtime.Serialization;
using System.Globalization;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// Data warehouse
    /// </summary>
    [Serializable]
    public class DDValue : IEquatable<DDValue>, IConvertible, ICloneable, IComparable, ISerializable
    {
        #region DDValue
        /// <summary>
        /// Create empty value
        /// There are nullable type and data.
        /// </summary>
        public DDValue() { }
        /// <summary>
        /// Creates value with data. If type of object isn't supported throw application exception
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double or an array of the above types
        /// </summary>
        /// <param name="value">data</param>
        public DDValue(object value)
        {
            SetValue(value);
        }
        #endregion DDValue
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDValue(SerializationInfo info, StreamingContext context)
        {
            this.type = (Type)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(Type));
            this.data = (byte[])info.GetValue(DDSchema.SERIALIZE_NODE_ARRAY_VALUE_ITEM, typeof(byte[]));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, this.type, typeof(Type));
            info.AddValue(DDSchema.SERIALIZE_NODE_ARRAY_VALUE_ITEM, this.data, typeof(byte[]));
        }
        #endregion ISerializable
        #region Properties
        protected byte[] data;
        protected Type type;
        /// <summary>
        /// Get the type of data stored
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
            if (type == typeof(byte)) return new[] {(byte)value};
            if (type == typeof(string)) return Encoding.UTF8.GetBytes(value.ToString());
            if (type == typeof(DateTime)) return BitConverter.GetBytes(((DateTime)value).ToBinary());
            if (type == typeof(bool)) return BitConverter.GetBytes((bool)value);
            if (type == typeof(char)) return BitConverter.GetBytes((char)value);
            if (type == typeof(float)) return BitConverter.GetBytes((float)value);
            if (type == typeof(decimal)) return  GetByteArrayFromDecimal((decimal)value); 
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
        /// Converts decimal to byte array
        /// </summary>
        /// <param name="dec">decimal number for converting</param>
        /// <returns></returns>
        private static byte[] GetByteArrayFromDecimal(decimal dec)
        {
             var bits = decimal.GetBits(dec);
             var bytes = new byte[bits.Length * sizeof(int)];
             for (int i = 0; i < bits.Length; i++)
                 BitConverter.GetBytes(bits[i]).CopyTo(bytes, i * sizeof(int));
             return bytes;
        }
        /// <summary>
        /// Converts bytes array to decimal
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static decimal GetDecimalFromByteArray(byte[] b)
        {
            var bits = new int [sizeof(int)];
            for(int i = 0; i < sizeof(int); i++)
                bits[i] = BitConverter.ToInt32(b, i * sizeof(int));
            return new decimal(bits);
        }

        /// <summary>
        /// Convert string to byte[] by specified type.
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, decimal, double or an array of the above types
        /// </summary>
        /// <param name="type">convert by specified type</param>
        /// <param name="value">string to convert</param>
        /// <returns>Converted byte[] by specified type</returns>
        protected static byte[] GetByteArrayByTypeFromString(Type type, string value)
        {
            //if (type == typeof(byte[])) return HEX(value);
            //if (type == typeof(byte)) return new[] { Convert.ToByte(value) };
            return GetByteArray(ConvertFromStringTo(type, value));
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
        public static implicit operator DDValue(decimal value)
        {
            return new DDValue(value);
        }
        public static implicit operator DDValue(decimal[] value)
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
        public static implicit operator decimal(DDValue value)
        {
            return value.GetValueAsDecimal();
        }
        public static implicit operator decimal[](DDValue value)
        {
            return value.GetValueAsDecimalArray();
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
        /// Get value by current type
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue()
        {
            return GetValue(this.Type);
        }
        /// <summary>
        /// Returns value by specified type
        /// </summary>
        /// <param name="t">type</param>
        /// <returns></returns>
        protected virtual object GetValue(Type t)
        {
            if (t == typeof(string)) return GetValueAsString();
            if (t== typeof(string[])) return GetValueAsStringArray();
            if (t== typeof(DateTime)) return GetValueAsDateTime();
            if (t== typeof(DateTime[])) return GetValueAsDateTimeArray();
            if (t== typeof(byte)) return GetValueAsByte();
            if (t== typeof(byte[])) return GetValueAsByteArray();
            if (t== typeof(short)) return GetValueAsShort();
            if (t== typeof(short[])) return GetValueAsShortArray();
            if (t== typeof(ushort)) return GetValueAsUShort();
            if (t== typeof(ushort[])) return GetValueAsUShortArray();
            if (t== typeof(int)) return GetValueAsInt();
            if (t== typeof(int[])) return GetValueAsIntArray();
            if (t== typeof(uint)) return GetValueAsUInt();
            if (t== typeof(uint[])) return GetValueAsUIntArray();
            if (t== typeof(long)) return GetValueAsLong();
            if (t== typeof(long[])) return GetValueAsLongArray();
            if (t== typeof(ulong)) return GetValueAsULong();
            if (t== typeof(ulong[])) return GetValueAsULongArray();
            if (t== typeof(char)) return GetValueAsChar();
            if (t== typeof(char[])) return GetValueAsCharArray();
            if (t== typeof(float)) return GetValueAsFloat();
            if (t== typeof(float[])) return GetValueAsFloatArray();
            if (t== typeof(decimal)) return GetValueAsDecimal();
            if (t== typeof(decimal[])) return GetValueAsDecimalArray();
            if (t== typeof(double)) return GetValueAsDouble();
            if (t== typeof(double[])) return GetValueAsDoubleArray();
            if (t== typeof(bool)) return GetValueAsBool();
            if (t== typeof(bool[])) return GetValueAsBoolArray();
            if (t== typeof(Guid)) return GetValueAsGuid();
            if (t== typeof(Guid[])) return GetValueAsGuidArray();
            if (t== null) return null;
            throw new DDTypeIncorrectException(Type.ToString());
        }
        /// <summary>
        /// Get value as array by specified type
        /// </summary>
        /// <typeparam expected="T">The type of return array value.</typeparam>
        /// <returns>Returns array by specified type</returns>
        public virtual T[] GetValueAsArray<T>()
        {
            if (this.data == null) return null;
            if (this.Size == 0) return new T[] { };// returns empty array of requested type

            int sizePerElementsSource = 0;
            long length = 0;
            Type sourceElementType = (this.Type.IsArray ? this.Type.GetElementType() : this.Type);
            T[] result;
            // casts to string array
            if (typeof(T) == typeof(string))
            {
                //  --- if both requested and current types are string or string array return it
                if (sourceElementType == typeof(string)) return (T[])((object)GetValueAsString().Split('\0'));
                // ***  --- if requested type is string and current type is byte array return string HEX array 
                // if (this.Type == typeof(byte[])) return (T[]) (object) HEX(data) ;
                // convert premitivies
                sizePerElementsSource = GetPrimitiveSize(sourceElementType);
                length = this.Size / sizePerElementsSource;
                result = new T[length];
                for (int i = 0; i < length; i++)
                {
                    var tmp = new byte[sizePerElementsSource];
                    Array.Copy(data, i * sizePerElementsSource, tmp, 0, sizePerElementsSource);
                    result[i] = (T)(object)GetObjAsStringByType(sourceElementType, GetValueObjByType(sourceElementType, tmp));
                }
                return result;
            }

            // casts string to array of primitive 
            // if (this.Type == typeof(string)) return new T[] {GetValueAs<T>()};
            // casts string array to array of primitive
            if (sourceElementType == typeof(string))
            {
                var tmp = GetValueAsString().Split('\0');
                result = new T[tmp.Length];
                for (var i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Length != 0) // if string is empty saves the default value of type  
                        result[i] = (T)ConvertFromStringTo(typeof(T), tmp[i]); //(T)GetValueObjByType(typeof(T), Encoding.UTF8.GetBytes(tmp[i]));
                }
                return result;
            }

            // casts primitivies to primitivies
            var sizePerElementsTarget = GetPrimitiveSize(typeof(T));
            sizePerElementsSource = GetPrimitiveSize(sourceElementType);
            var lenght = this.Size / sizePerElementsSource;
            result = new T[lenght];
            for (int i = 0; i < lenght; i++)
            {
                var tmp = new byte[(sizePerElementsSource > sizePerElementsTarget ? sizePerElementsSource : sizePerElementsTarget)];
                Array.Copy(data, i * sizePerElementsSource, tmp, 0, sizePerElementsSource);
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
            if (data == null) return default(T); // check Nullable type
            Type t = typeof(T);
            if (Nullable.GetUnderlyingType(t) != null) 
            {
                t = Nullable.GetUnderlyingType(t); // Returns the underlying type argument of the specified nullable type. 
            }
            if (this.Size == 0) return GetDefaultValueByType<T>();// returns empty array of requested type

            int sizePerElementsSource = 0;
            long length = 0;
            Type sourceElementType = (this.Type.IsArray ? this.Type.GetElementType() : this.Type);
            // casts to string 
            if (t == typeof(string))
            {
                //  --- if both requested and current types are string or string array return it
                if (sourceElementType == typeof(string)) return (T)GetValueObjByType(t, data);
                // convert premitivies
                sizePerElementsSource = GetPrimitiveSize(sourceElementType);
                var tmp = new byte[sizePerElementsSource];
                Array.Copy(data, 0, tmp, 0, sizePerElementsSource);
                return (T)(object)GetObjAsStringByType(sourceElementType, GetValueObjByType(sourceElementType, tmp));
            }

            // casts string to primitive 
            if (sourceElementType == typeof(string))
            {
                var tmp = (string)GetValueObjByType(sourceElementType, data);
                if (tmp.Length != 0) // if string is empty saves the default value of type  
                    return (T)ConvertFromStringTo(t, tmp);
                else
                    return GetDefaultValueByType<T>();
            }

            // casts primitivies to primitivies
            var sizePerElementsTarget = GetPrimitiveSize(t);
            sizePerElementsSource = GetPrimitiveSize(sourceElementType);
            var lenght = this.Size / sizePerElementsSource;
            var b = new byte[(sizePerElementsSource > sizePerElementsTarget ? sizePerElementsSource : sizePerElementsTarget)];
            Array.Copy(data, 0, b, 0, sizePerElementsSource);
            return (T)GetValueObjByType(t, b);

        }
        /// <summary>
        /// Returns String.Empty for string and default value for all others supported types.
        /// </summary>
        /// <typeparam name="T">supported types</typeparam>
        /// <returns></returns>
        protected static T GetDefaultValueByType<T>()
        {
            if (!typeof(T).IsArray)
            {
                if (typeof(T) == typeof(string)) return (T)(object)String.Empty;
                if (ValidateType(typeof(T))) return default(T);
              /*
                if (typeof(T) == typeof(DateTime)) return (T)(object)new DateTime();
                if (typeof(T) == typeof(byte)) return (T)(object)new Byte();
                if (typeof(T) == typeof(char)) return (T)(object)new Char();
                if (typeof(T) == typeof(short)) return (T)(object)new short();
                if (typeof(T) == typeof(ushort)) return (T)(object)new ushort();
                if (typeof(T) == typeof(int)) return (T)(object)new int();
                if (typeof(T) == typeof(uint)) return (T)(object)new uint();
                if (typeof(T) == typeof(long)) return (T)(object)new long();
                if (typeof(T) == typeof(ulong)) return (T)(object)new ulong();
                if (typeof(T) == typeof(float)) return (T)(object)new float();
                if (typeof(T) == typeof(double)) return (T)(object)new double();
                if (typeof(T) == typeof(bool)) return (T)(object)new bool();
                if (typeof(T) == typeof(Guid)) return (T)(object)new Guid();
               */
            }
            throw new DDTypeIncorrectException(typeof(T).ToString());
        }

        protected static object GetValueObjByType(Type type, byte[] data)
        {
            if (Nullable.GetUnderlyingType(type) != null)
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
            if (type == typeof(decimal)) return GetDecimalFromByteArray(data);  //Convert.ToDecimal(BitConverter.ToDouble(data, 0));
            if (type == typeof(bool)) return BitConverter.ToBoolean(data, 0);
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

        public virtual decimal GetValueAsDecimal()
        {
            return GetValueAs<decimal>();//BitConverter.Decimaldata, 0);
        }
        public virtual decimal[] GetValueAsDecimalArray()
        {
            return GetValueAsArray<decimal>();
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
        /// Supports the following types: string, char, bool, byte, DateTime, short, int, float, long, ushort, uint, ulong, double and Guid or an array of the above types
        /// </summary>
        /// <param name="value">object whose type will be validate</param>
        /// <returns>true if type of object is supported, otherwise false</returns>
        public static bool ValidateType(object value)
        {
            return ValidateType(value.GetType());
        }

        /// <summary>
        /// Checks the type.
        /// Supports the following types: string, char, bool, byte, sbye, DateTime, short, int, float, long, ushort, uint, ulong, double, decimal and Guid or an array of the above types. 
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
                    type == typeof(decimal) ||
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
            if (type == typeof(decimal)) return sizeof(decimal);
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
            if (type == typeof(DateTime)) return ((DateTime)value).ToString(DDSchema.StringDateTimeFormat);
            // Workarround for MS ToString() issue for Single, Double, and BigInteger types.
            // for example: Single.MaxValue -> 3.40282347E+38, after ToString() -> 3.402823E+38, - lost data
            // The round-trip ("R") format: http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx#RFormatString
            if (type == typeof(Single)) return ((Single)value).ToString(DDSchema.StringRoundTripFormat);
            if (type == typeof(Double)) return ((Double)value).ToString(DDSchema.StringRoundTripFormat);
            if (type == typeof(float)) return ((float)value).ToString(DDSchema.StringRoundTripFormat);
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
            return GetValueAsArray<string>();
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
        #region Convert
        /// <summary>
        /// Converts current value to specified array type. Specify elemnt type 'int' not array type 'int[]'
        /// </summary>
        /// <typeparam name="T">Element type, for example, 'bool'. Don't specify array type 'bool[]'.</typeparam>
        public void ConvertToArray<T>()
        {
            if (this.Type == null) throw new DDTypeNullException(Msg.CANNOT_TRANSFORM_NULL_TYPE);
            DDValue tmp = new DDValue(GetValueAsArray<T>());
            this.data = tmp.data;
            this.type = tmp.type;
        }
        /// <summary>
        /// Converts current value to specify type. Don't specify array type
        /// </summary>
        /// <typeparam name="T">New type</typeparam>
        public void ConvertTo<T>()
        {
            if (this.Type == null) throw new DDTypeNullException(Msg.CANNOT_TRANSFORM_NULL_TYPE);
            DDValue tmp = new DDValue(GetValueAs<T>());
            this.data = tmp.data;
            this.type = tmp.type;
        }
        public void ConvertTo(Type t)
        {
            DDValue tmp = new DDValue(GetValue(t));
            this.data = tmp.data;
            this.type = tmp.type;
        }
        /*
        public void ConvertTo(Type newType)
        {
            if ((this.Type != typeof(string) && (this.Type != typeof(string[])))) throw new DDTypeConvertException(this.type.FullName, newType.FullName, string.Format(Msg.CANNOT_CONVERT_FROM_NONE_STRING_OR_STRING_ARRAY_TYPE, newType.Name, this.type.Name));
            if (this.Type.IsArray != newType.IsArray) throw new DDTypeConvertException(this.type.FullName, newType.FullName, string.Format(Msg.CANNOT_TRANSFORM_ARRAY_TYPE_TO_NOT_ARRAY, this.type.FullName, newType.FullName));
            if ((newType == typeof(string) || (newType == typeof(string[])))) return; // nothing to do
            if (data.Length > 0)
            {
                if (this.Type.IsArray)
                {
                    var items = GetValueAsStringArray();
                    var objArray = new object[items.Length];
                    int i = 0;
                    var elType = newType.GetElementType();
                    foreach (var item in items)
                    {
                        objArray[i] = ConvertFromStringTo(elType, item);
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
        }
         */
        #endregion Convert
        #region Convert Static
        /// <summary>
        /// Convert string to specified type.
        /// Supports the following types: byte, byte[], string, DateTime, bool, char, double, short, int, float, long, ushort, uint, ulong, double, Guid
        /// </summary>
        /// <param name="type">convert to specified type</param>
        /// <param name="value">string to convert</param>
        /// <returns>Converted object to specified type</returns>
        public static object ConvertFromStringTo(Type type, string value)
        {
            try
            {
                if (type == typeof(byte[])) return HEX(value);
                if (type == typeof(byte)) return Convert.ToByte(value);
                if (type == typeof(string)) return value.ToString();

                if (type == typeof(DateTime))
                {
                    DateTime res;
                    if (DateTime.TryParseExact(value, DDSchema.StringDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out res))
                        return res;
                    else
                        return Convert.ToDateTime(value);
                }
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
                if (type == typeof(decimal)) return Convert.ToDecimal(value);
                if (type == typeof(Guid)) return new Guid(value);
            }
            catch (Exception e)
            {
                throw new DDValueConvertException(value, type, e);
            }
            throw new DDTypeIncorrectException(type.ToString());
        }
        /// <summary>
        /// Convert string[] to specified type.
        /// Supports the following types: byte[], string[], DateTime[], bool[], char[], double[], short[], int[], float[], long[], ushort[], uint[], ulong[], double[], Guid[]
        /// </summary>
        /// <param name="type">convert to specified type</param>
        /// <param name="value">string array to convert</param>
        /// <returns>Converted object to specified type</returns>
        public static object ConvertFromStringArrayTo(Type type, string[] value)
        {
            try
            {
                var elType = type.GetElementType();
                var objArray = GetStubArrayByType(type, value.Length);

                int i = 0;
                foreach (var item in value)
                {
                    objArray.SetValue(ConvertFromStringTo(elType, item), i);
                    i++;
                }
                return Convert.ChangeType(objArray, type);
            }
            catch (Exception e)
            {
                throw new DDValueConvertException(value, type, e);
            }
        }
        /// <summary>
        /// Returns specified type of array and lenght
        /// Supports the following types: byte[], string[], DateTime[], bool[], char[], double[], short[], int[], float[], long[], ushort[], uint[], ulong[], double[], Guid[]
        /// </summary>
        /// <param name="type">create array of this type</param>
        /// <param name="lenght">total number of elements</param>
        /// <returns>Returns specified type of array and lenght</returns>
        private static Array GetStubArrayByType(Type type, int lenght)
        {
            if (type.IsArray)
            {
                var t = type.GetElementType();
                if (t == typeof(byte)) return new byte[lenght];
                if (t == typeof(string)) return new string[lenght];
                if (t == typeof(DateTime)) return new DateTime[lenght];
                if (t == typeof(bool)) return new bool[lenght];
                if (t == typeof(char)) return new char[lenght];
                if (t == typeof(float)) return new float[lenght];
                if (t == typeof(double)) return new double[lenght];
                if (t == typeof(short)) return new Int16[lenght];
                if (t == typeof(int)) return new Int32[lenght];
                if (t == typeof(long)) return new Int64[lenght];
                if (t == typeof(ushort)) return new ushort[lenght];
                if (t == typeof(uint)) return new uint[lenght];
                if (t == typeof(ulong)) return new ulong[lenght];
                if (t == typeof(decimal)) return new decimal[lenght];
                if (t == typeof(Guid)) return new Guid[lenght];

            }
            throw new DDTypeIncorrectException(type.ToString());
        }
        #endregion Convert Static

        #region IConvertible
        /// <summary>
        /// Returns the TypeCode for this instance.
        /// </summary>
        /// <returns>The enumerated constant that is the TypeCode of the class or value type that implements this interface.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object; 
        }
        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return GetValueAsBool();
        }
        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return GetValueAsByte();
        }
        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            return GetValueAsChar();
        }
        /// <summary>
        /// returns value as Date Time format
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return GetValueAsDateTime();
        }
        /// <summary>
        /// returns value as decimal
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return GetValueAsDecimal();    
        }
        /// <summary>
        /// returns value as double
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return GetValueAsDouble();
        }
        /// <summary>
        /// returns value as short
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return GetValueAsShort();
        }
        /// <summary>
        /// returns value as int
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return GetValueAsInt();
        }
        /// <summary>
        /// returns value as long
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return GetValueAsLong();
        }
        /// <summary>
        /// returns value as sbyte
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte)(GetValueAsByte()-128);
        }
        /// <summary>
        /// returns value as float
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return GetValueAsFloat();
        }
        /// <summary>
        /// returns value as string
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        string IConvertible.ToString(IFormatProvider provider)
        {
            return GetValueAsString();
        }
        /// <summary>
        /// Converts the value of this instance to an Object of the specified Type that has an equivalent value, using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="conversionType">The Type to which the value of this instance is converted.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An Object instance of type conversionType whose value is equivalent to the value of this instance.</returns>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(GetValue(), conversionType);
        }
        /// <summary>
        /// returns value as ushort
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return GetValueAsUShort();
        }
        /// <summary>
        /// returns value as uint
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return GetValueAsUInt();
        }
        /// <summary>
        /// Return value as ulong
        /// </summary>
        /// <param name="provider">this parametr will be ignored</param>
        /// <returns></returns>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return GetValueAsULong();
        }
        #endregion IConvertible
    }
}