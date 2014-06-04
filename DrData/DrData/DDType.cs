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
        public string Name { get;  set; }



        #region NodeType
        /// <summary>
        /// Throw <exception cref="NodeTypeException">NodeTypeException</exception> if type of current node is not equals expected type
        /// </summary>
        /// <param name="expectedType"></param>
        public void ThrowIsNotExpectedNodeType(DDType expectedType)
        {
            if (CompareTo(expectedType)!=0) throw new NodeTypeException(this.Name, expectedType);
        }

        #endregion NodeType

        #region
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
        /// Retruns base.Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DDType other)
        {
            return base.Equals(other);
        }
    }
}
