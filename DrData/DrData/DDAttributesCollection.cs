using DrOpen.DrCommon.DrData.Exceptions;
/*
  DDAttributesCollection.cs -- collection of data for data of the 'DrData' general purpose Data abstraction layer 1.0.1, January 3, 2014
 
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// Represents a collection of DDValue that can be accessed by name.
    /// </summary>
    [Serializable]
    public class DDAttributesCollection : IEnumerable<KeyValuePair<string, DDValue>>, ICloneable, IComparable, ISerializable
    {
        public DDAttributesCollection()
        {
            attributes = new Dictionary<string, DDValue>();
        }
        protected Dictionary<string, DDValue> attributes;
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize attributes collection.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDAttributesCollection(SerializationInfo info, StreamingContext context)
        {
            this.attributes = (Dictionary<string, DDValue>)info.GetValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, typeof(Dictionary<string, DDValue>));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, attributes, typeof(Dictionary<string, DDValue>));
        }
        #endregion ISerializable
        #region Enumerator
        /// <summary>
        /// Returns an enumerator that iterates through the Attribute Collection (IEnumerator&lt;KeyValuePair&lt;string, IDDValue&gt;&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;KeyValuePair&lt;string, IDDValue&gt;&gt;</returns>
        public virtual IEnumerator<KeyValuePair<string, DDValue>> GetEnumerator()
        {
            foreach (var a in attributes)
            {
                yield return a;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the Attribute Collection (IEnumerator&lt;KeyValuePair&lt;string, IDDValue&gt;&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;KeyValuePair&lt;string, IDDValue&gt;&gt;</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion Enumerator
        #region Add
        /// <summary>
        /// Add the new value and assign automatically generated name to this value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>Return automatically generated name for this value</returns>
        /// <remarks>The automatically generated name will be GUID</remarks>
        public virtual string Add(DDValue value)
        {
            var name = Guid.NewGuid().ToString();
            return Add(name, value, ResolveConflict.THROW_EXCEPTION);
        }
        /// <summary>
        /// Add the new value by name
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the new application exception will be thrown</param>
        /// <param name="value">value</param>
        /// <returns>Return name for this value</returns>
        public string Add(Enum name, DDValue value)
        {
            return Add(name.ToString(), value);
        }
        /// <summary>
        /// Add the new value by name
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the new application exception will be thrown</param>
        /// <param name="value">value</param>
        /// <returns>Return name for this value</returns>
        public string Add(string name, DDValue value)
        {
            Add(name, value, ResolveConflict.THROW_EXCEPTION);
            return name;
        }
        /// <summary>
        /// Add the new value by name.  
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the subsequent behavior depends of the specified rules</param>
        /// <param name="value">value</param>
        /// <param name="resolve">Rules of behavior in conflict resolution names.
        /// Throw a new exception;
        /// Update the existing value;
        /// Skip this action and preserve exists value
        /// </param>
        /// <returns>If the value was successfully added or overwritten - returns name of this value, otherwise, for example, when used ResolveConflict.SKIP, returns null</returns>
        /// <remarks>Generates events when overwriting or saving the current value</remarks>
        public virtual string Add(Enum name, DDValue value, ResolveConflict resolve)
        {
            return Add(name.ToString(), value, resolve);
        }
        /// <summary>
        /// Add the new value with name.  
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the subsequent behavior depends of the specified rules</param>
        /// <param name="value">value</param>
        /// <param name="resolve">Rules of behavior in conflict resolution names.
        /// Throw a new exception;
        /// Update the existing value;
        /// Skip this action and preserve exists value
        /// </param>
        /// <returns>If the value was successfully added or overwritten - returns name of this value, otherwise, for example, when used ResolveConflict.SKIP, returns null</returns>
        /// <remarks>Generates events when overwriting or saving the current value</remarks>
        public virtual string Add(string name, DDValue value, ResolveConflict resolve)
        {
            switch (resolve)
            {
                case ResolveConflict.OVERWRITE:
                    if (Contains(name))
                    {
                        //this.OnDoLogIn(Res.Msg.OVERWRITE_EXISTS_VALUE, name, value);
                        Remove(name); //remove exists name
                    }
                    break;
                case ResolveConflict.SKIP:
                    if (this.Contains(name))
                    {
                        //this.OnDoLogIn(Res.Msg.PRESERVE_EXISTS_VALUE, name, value);
                        return null; // return null because the item was not added
                    }
                    break;
            }
            try
            {
                attributes.Add(name, value);
            }
            catch (ArgumentException e)
            {
                throw new DDAttributeExistsException(name, e);
            }
            return name; // return name of new value
        }
        #endregion Add
        #region Replace
        /// <summary>
        /// Add the new or replace exists value by name.  
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the value will be updated</param>
        /// <param name="value">value</param>
        /// <returns>If the value was successfully added or updatted - returns name of this value, otherwise, returns null</returns>
        /// <remarks>this Method call Add with flag ResolveConflict.OVERWRITE</remarks>
        public virtual string Replace(Enum name, DDValue value)
        {
            return Replace(name.ToString(), value);
        }
        /// <summary>
        /// Add the new or replace exists value by name.
        /// </summary>
        /// <param name="name">uniq name for with value. If specified name exists in this collection the value will be updated</param>
        /// <param name="value">value</param>
        /// <returns>If the value was successfully added or updatted - returns name of this value, otherwise, returns null</returns>
        /// <remarks>this Method call Add with flag ResolveConflict.OVERWRITE</remarks>
        public virtual string Replace(string name, DDValue value)
        {
            return Add(name, value, ResolveConflict.OVERWRITE);
        }
        #endregion Replace
        #region  Contains
        /// <summary>
        /// Determines whether the Attribute Collection contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the Attribute Collection </param>
        /// <returns>true if the Attribute Collection contains an element with the specified name; otherwise, false.</returns>
        public virtual bool Contains(Enum name)
        {
            return Contains(name.ToString());
        }

        /// <summary>
        /// Determines whether the Attribute Collection contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the Attribute Collection </param>
        /// <returns>true if the Attribute Collection contains an element with the specified name; otherwise, false.</returns>
        public virtual bool Contains(string name)
        {
            return attributes.ContainsKey(name);
        }
        #endregion  Contains
        #region  ContainsValue
        /// <summary>
        /// Determines whether the Attribute Collection contains the specified value. Warning! This is a very slow function!
        /// </summary>
        /// <param name="value">The value to locate in the Attribute Collection. The value can be null for reference types.</param>
        /// <returns>true if the Attribute Collection contains an element with the specified value; otherwise, false.</returns>
        /// <remarks>This method determines equality using the default comparer Compare(DDValue value1, DDValue value2);. Default for DDValue, the type of values in the dictionary.
        /// This method performs a linear search; therefore, the average execution time is proportional to Count. That is, this method is an O(n) operation, where n is Count.</remarks>
        public virtual bool ContainsValue(DDValue value)
        {
            //return attributes.ContainsValue(value); // it doesn't work, bacuase Equls dosn't work for byte[]
            foreach (var item in Values)
            {
                if (DDValue.Compare(value, item) == 0) return true;
            }
            return false;
        }
        #endregion  ContainsValue
        #region Remove
        /// <summary>
        /// Removes the value with the specified name
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the  Attribute Collection</returns>
        public virtual bool Remove(Enum name)
        {
            return Remove(name.ToString());
        }
        /// <summary>
        /// Removes the value with the specified name
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the  Attribute Collection</returns>
        public virtual bool Remove(string name)
        {
            return attributes.Remove(name);
        }
        #endregion Remove
        #region Clear
        /// <summary>
        /// Removes all attributes from collection
        /// </summary>
        public virtual void Clear()
        {
            attributes.Clear();
        }
        #endregion Clear
        #region Clone
        /// <summary>
        /// Creates a copy of the current Attribute Collection
        /// </summary>
        /// <returns>A copy of the current Attribute Collection</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        /// <summary>
        /// Creates a copy of the current Attribute Collection
        /// </summary>
        /// <returns>A copy of the current Attribute Collection</returns>
        public virtual DDAttributesCollection Clone()
        {
            var aDic = new DDAttributesCollection();
            foreach (var a in this)
            {
                if (a.Value == null)
                    aDic.Add(a.Key, null);
                else
                    aDic.Add(a.Key, a.Value.Clone());
            }
            return aDic;
        }
        #endregion Clone
        #region Names/Values
        /// <summary>
        /// Gets a collection containing the names of values in this Attribute Collection
        /// </summary>
        public virtual Dictionary<string, DDValue>.KeyCollection Names
        {
            get { return attributes.Keys; }
        }
        /// <summary>
        /// Gets a collection containing the values in this Attribute Collection
        /// </summary>
        public virtual Dictionary<string, DDValue>.ValueCollection Values
        {
            get { return attributes.Values; }
        }
        #endregion Names/Values
        #region GetValue
        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified name, 
        /// if the name is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the Attribute Collection contains an element with the specified name; otherwise, false.</returns>
        /// <remarks> This method combines the functionality of the Contains method and the Item property.
        /// If the name is not found, then the value parameter gets the appropriate default value for the type IDDValue
        /// Use the TryGetValue method if your code frequently attempts to access name that are not in the Attribute Collection. 
        /// Using this method is more efficient than catching the KeyNotFoundException thrown by the Item property.
        /// This method approaches an O(1) operation.</remarks>
        public virtual bool TryGetValue(Enum name, out DDValue value)
        {
            return TryGetValue(name.ToString(), out value);
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified name, 
        /// if the name is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the Attribute Collection contains an element with the specified name; otherwise, false.</returns>
        /// <remarks> This method combines the functionality of the Contains method and the Item property.
        /// If the name is not found, then the value parameter gets the appropriate default value for the type DDValue
        /// Use the TryGetValue method if your code frequently attempts to access name that are not in the Attribute Collection. 
        /// Using this method is more efficient than catching the KeyNotFoundException thrown by the Item property.
        /// This method approaches an O(1) operation.</remarks>
        public virtual bool TryGetValue(string name, out DDValue value)
        {
            return attributes.TryGetValue(name, out value);
        }
        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        public DDValue this[Enum name]
        {
            get { return attributes[name.ToString()]; }
            private set { attributes[name.ToString()] = value; }
        }
        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        public DDValue this[string name]
        {
            get { return attributes[name]; }
            private set { attributes[name] = value; }
        }

        /// <summary>
        /// Gets the value associated with the specified name. 
        /// When this method returns, contains the value associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter.
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="defaultValue">the default value for the type of the value parameter.</param>
        /// <returns>When this method returns, contains the value associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter.</returns>
        public virtual DDValue GetValue(Enum name, object defaultValue)
        {
            return GetValue(name.ToString(), defaultValue);
        }
        /// <summary>
        /// Gets the value associated with the specified name. When this method returns, 
        /// contains the value associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter.
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="defaultValue">the default value for the type of the value parameter.</param>
        /// <returns>When this method returns, contains the value associated with the specified name, if the nameis found; 
        /// otherwise, the default value for the type of the value parameter.</returns>
        public virtual DDValue GetValue(string name, object defaultValue)
        {
            DDValue newValue;
            if (TryGetValue(name, out newValue)) return newValue; // return new value
            if (defaultValue == null) return null;
            if (defaultValue.GetType() == typeof(DDValue)) return (DDValue)defaultValue; // return default Value as DDValue
            return new DDValue(defaultValue); // create and return new DDValue
        }
        #endregion GetValue
        #region Count
        /// <summary>
        /// Returns the number of elements of attributes collection.
        /// </summary>
        public virtual int Count
        {
            get { return attributes.Count; }
        }
        #endregion Count
        #region ==, != operators
        /// <summary>
        /// Compare both values and return true if type and data are same otherwise return false. Very slow
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if type and data are same otherwise return false</returns>
        /// <remarks>The both null object is equal and return value will be true. Very slow</remarks>
        public static bool operator ==(DDAttributesCollection value1, DDAttributesCollection value2)
        {
            return (Compare(value1, value2) == 0);
        }
        public static bool operator !=(DDAttributesCollection value1, DDAttributesCollection value2)
        {
            return (!(value1 == value2));
        }
        #endregion ==, != operators
        #region IComparable
        /// <summary>
        /// Compares the two DDAttributesCollection of the same values and returns an integer that indicates whether the current instance precedes. Very slow
        /// </summary>
        /// <param name="value1">First DDAttributesCollection to compare</param>
        /// <param name="value2">Second DDAttributesCollection to compare</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - the both DDAttributesCollection have some items and their values.
        /// The difference between the number of elements of the first and second DDAttributes Collection
        /// One - values of collection is not equal.</returns>
        /// <remarks>The both null object is equal and return value will be Zero. Very slow</remarks>
        public static int Compare(DDAttributesCollection value1, DDAttributesCollection value2)
        {
            if (((object)value1 == null) && ((object)value2 == null)) return 0; // if both are null -> return true
            if (((object)value1 == null) || ((object)value2 == null)) return 1; // if only one of them are null ->  return false
            if ((value1.Count != value2.Count)) return value1.Count - value2.Count; // The difference between the number of elements of the first and second DDAttributes Collection

            foreach (var keyValue1 in value1)
            {
                if (!value2.Contains(keyValue1.Key)) return -1; // 
                var valueCompareResult = DDValue.Compare(keyValue1.Value, value2[keyValue1.Key]);
                if (valueCompareResult != 0) return valueCompareResult;
            }
            return 0;
        }
        /// <summary>
        /// Compares the current DDAtribute instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - This instance occurs in the same position in the sort order as obj.
        /// One - This instance follows obj in the sort order.</returns>
        /// <remarks>The both null object is equal and return value will be Zero</remarks>
        public virtual int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DDAttributesCollection)) return 1;
            return Compare(this, (DDAttributesCollection)obj);
        }
        #endregion CompareTo
        #region Equals
        protected bool Equals(DDAttributesCollection other)
        {
            return Equals(attributes, other.attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DDAttributesCollection)obj);
        }
        #endregion Equals
        #region GetHashCode
        public override int GetHashCode()
        {
            return (attributes != null ? attributes.GetHashCode() : 0);
        }
        #endregion GetHashCode
        #region Size
        /// <summary>
        /// size in bytes of the stored data for all attributes
        /// </summary>
        /// <returns></returns>
        public long GetDataSize()
        {
            long size = 0;
            foreach (var value in Values)
            {
                if (value != null) size += value.Size;
            }
            return size;
        }
        /// <summary>
        /// size in bytes of the stored data and names for all attributes
        /// </summary>
        /// <returns></returns>
        public long GetSize()
        {
            long size = 0;
            foreach (var item in this)
            {
                if (item.Value != null) size += item.Value.Size;
                if (item.Key != null) size += Encoding.UTF8.GetBytes(item.Key).LongLength;
            }
            return size;
        }
        #endregion Size
        #region Merge
        /// <summary>
        /// Merge attributes with source attribute collection. In case of conflict, an appropriate exception is thrown
        /// </summary>
        /// <param name="coll">Source attribute collection.</param>
        public void Merge(DDAttributesCollection coll)
        {
            Merge(coll, ResolveConflict.THROW_EXCEPTION);
        }
        /// <summary>
        /// Merge attributes with source attribute collection.
        /// </summary>
        /// <param name="coll">Source attribute collection.</param>
        /// <param name="res">The parameters determine the resolution of attributes name conflicts</param>
        public void Merge(DDAttributesCollection coll, ResolveConflict res)
        {
            foreach (var item in coll)
            {
                Add(item.Key, item.Value, res);
            }
        }
        #endregion Merge
    }
}