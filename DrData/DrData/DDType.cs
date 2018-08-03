/*
  DDType.cs -- stored type of the 'DrData' general purpose Data abstraction layer 1.1, October 27, 2016
 
  Copyright (c) 2013-2016 Kudryashov Andrey aka Dr
 
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
using DrOpen.DrCommon.DrData.Res;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrData.Exceptions;
using System.Runtime.Serialization;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// the type of the object
    /// </summary>
    [Serializable]
    public class DDType : IComparable, IComparable<DDType>, IEquatable<DDType>, ISerializable
    {
        #region Constructor
        public DDType(Enum name)
            : this(name.ToString())
        { }

        public DDType(string name)
        {
            this.Name = name;
        }
        public DDType(Type type)
        {
            this.Name = type.Name;
        }
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDType(SerializationInfo info, StreamingContext context)
        {
            this.Name = (String)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(String));
        }
        #endregion Constructor
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, this.Name, typeof(String));
        }

        /// <summary>
        /// Get or set name of type 
        /// </summary>
        public virtual string Name { get; set; }

        #region NodeType
        #endregion NodeType

        #region implicit operator
        public static implicit operator DDType(string value)
        {
            return new DDType(value);
        }
        public static implicit operator string(DDType value)
        {
            return value.Name;
        }
        #endregion
        /// <summary>
        /// Compare type by name
        /// </summary>
        /// <param name="obj">other type for comparison as object</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DDType)) return 1;
            return CompareTo((DDType)obj);
        }
        /// <summary>
        /// Compare type by name
        /// </summary>
        /// <param name="other">other type for comparison</param>
        /// <returns></returns>
        public int CompareTo(DDType other)
        {
            return Name.CompareTo(other.Name);
        }
        /// <summary>
        /// Compares the two DDType of the same type and returns an integer that indicates whether the current instance precedes, follows, 
        /// or occurs in the same position in the sort order as the other object.
        /// The both null object is equal and return value will be Zero.
        /// </summary>
        /// <param name="value1">First DDType to compare</param>
        /// <param name="value2">Second DDType to compare</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - the both DDValue have some type and value.
        /// One - type or value is not equal.</returns>
        public static int Compare(DDType value1, DDType value2)
        {
            if (((object)value1 == null) && ((object)value2 == null)) return 0; // if both are null -> return true
            if (((object)value1 == null) || ((object)value2 == null)) return 1; // if only one of them are null ->  return false
            if ((value1.Name == null) || (value2.Name == null))
            {
                if (!((value1.Name == null) && (value2.Name == null))) return 1;
            }
            return value1.CompareTo(value2);
        }
        /// <summary>
        /// Determines whether the specified DDType is equal to the current DDType. Returns true if the specified DDType is equal to the current DDType otherwise, false.
        /// </summary>
        /// <param name="other">other type for comparison</param>
        /// <returns>true if the specified DDType is equal to the current DDType otherwise, false.</returns>
        public virtual bool Equals(DDType other)
        {
            //return base.Equals(other);
            return this == (DDType)other;
        }
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current DDType. Returns true if the specified System.Object is equal to the current DDType otherwise, false
        /// </summary>
        /// <param name="other">other type for comparison as object</param>
        /// <returns>true if the specified System.Object is equal to the current DDType otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other.GetType() != typeof(DDType)) return false;
            return this.Equals((DDType)other);
        }
        /// <summary>
        /// Returns the hash code for this DDType which get from Name property
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.Name == null) return 0;
            return this.Name.GetHashCode();
        }
        #region ==, != operators
        /// <summary>
        /// Compare both values and return true if type and data are same otherwise return false.
        /// If both values are null - return true, if only one of them are null, return false. if the data types are different - return false
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if type and data are same otherwise return false</returns>
        public static bool operator ==(DDType value1, DDType value2)
        {
            return (Compare(value1, value2) == 0);
        }
        public static bool operator !=(DDType value1, DDType value2)
        {
            return (!(value1 == value2));
        }
        #endregion ==, != operators
        #region validation
        /// <summary>
        /// Validate current node type of with expected node type. If types are not equal throw new <see cref="DDTypeExpectedException"/> otherwise nothing.
        /// </summary>
        /// <param name="currentType">current node type</param>
        /// <param name="expectedType">expected node type</param>
        public static void ValidateExpectedNodeType(DDType currentType, params DDType[] expectedType)
        {
            if (expectedType == null) return;
            foreach (var eType in expectedType)
            {
                if (currentType == eType) return;
            }
            throw new DDTypeExpectedException(currentType.Name, arrayToString<DDType>(expectedType));
        }
        /// <summary>
        /// Validate current node type of with expected node type. If types are not equal throw new <see cref="DDTypeExpectedException"/> otherwise nothing.
        /// </summary>
        /// <param name="currentType">current node type</param>
        /// <param name="expectedType">expected node type</param>
        public static void ValidateExpectedNodeType(DDType currentType, params string[] expectedType)
        {
            if (expectedType == null) return;
            foreach (var eType in expectedType)
            {
                if (currentType.Name == eType) return;
            }
            throw new DDTypeExpectedException(currentType.Name, arrayToString<string>(expectedType));
        }
        /// <summary>
        /// Validate current node type of with expected node type. If types are not equal throw new <see cref="DDTypeExpectedException"/> otherwise nothing.
        /// </summary>
        /// <param name="expectedType">expected node type</param>
        public virtual void ValidateExpectedNodeType(params string[] expectedType)
        {
            ValidateExpectedNodeType(this, expectedType);
        }
        /// <summary>
        /// Validate current node type of with expected node type. If types are not equal throw new <see cref="DDTypeExpectedException"/> otherwise nothing.
        /// </summary>
        /// <param name="expectedType">expected node type</param>
        public virtual void ValidateExpectedNodeType(DDType[] expectedType)
        {
            ValidateExpectedNodeType(this, expectedType);
        }
        #endregion validation

        private static string arrayToString<T>(T[] a)
        {
            string res = string.Empty;
            bool first = true;
            foreach (var item in a)
            {
                if (first)
                {
                    first = false;
                    res += item.ToString();
                }
                else
                    res += ", " + item.ToString();
            }
            return res;
        }
    }
}