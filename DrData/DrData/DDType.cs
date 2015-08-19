/*
  DDType.cs -- stored type of the 'DrData' general purpose Data abstraction layer 1.0.1, October 5, 2013
 
  Copyright (c) 2013-2015 Kudryashov Andrey aka Dr
 
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
using System.Runtime.Serialization;
using DrData.Res;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// Incorrect type of node exception
    /// </summary>
    public class NodeTypeException : Exception
    {
        public NodeTypeException(string currentType, string expectedType)
            : base(string.Format(Msg.NODE_TYPE_IS_NOT_MATCHED, currentType, expectedType))
        { }
    }
    /// <summary>
    /// the type of the object
    /// </summary>
    [Serializable]
    public class DDType : IComparable, IComparable<DDType>, IEquatable<DDType>, ISerializable
    {

        private const string SerializePropName = "Type";
        #region Constructor
        public DDType(Enum name): this(name.ToString())
        { }

        public DDType(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDType(SerializationInfo info, StreamingContext context)
        {
            this.Name = (String)info.GetValue(SerializePropName, typeof(String));
        }
        #endregion Constructor
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(SerializePropName, this.Name, typeof(String));
        }


        /// <summary>
        /// the type of the object as a string
        /// </summary>
        public string Name { get; set; }



        #region NodeType
        /// <summary>
        /// Throw <exception cref="NodeTypeException">NodeTypeException</exception> if type of current node is not equals expected type
        /// </summary>
        /// <param name="expectedType"></param>
        public void ThrowIsNotExpectedNodeType(DDType expectedType)
        {
            if (CompareTo(expectedType) != 0) throw new NodeTypeException(this.Name, expectedType);
        }

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
        /// Compare type as string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DDType)) return 1;
            return CompareTo((DDType)obj);
        }
        /// <summary>
        /// Compare type as string
        /// </summary>
        /// <param name="other"></param>
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
        /// Retruns base.Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DDType other)
        {
            return base.Equals(other);
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
    }
}
