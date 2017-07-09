/*
  DDNode.cs -- container for data of the 'DrData' general purpose Data abstraction layer 1.0.1, January 3, 2014
 
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
using DrOpen.DrCommon.DrData.Res;
using DrOpen.DrCommon.DrData.Exceptions;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// Hierarchy data warehouse
    /// </summary>
    [Serializable]
    public class DDNode : IEnumerable<KeyValuePair<string, DDNode>>, ICloneable, IEquatable<DDNode>, IComparable, ISerializable
    {
        #region Constructor
        public DDNode(string name, DDType type)
        {
            if (!IsNameCorect(name)) throw new DDNodeIncorrectNameException(name);
            attributes = new DDAttributesCollection();
            Name = name;
            childNodes = new Dictionary<string, DDNode>();
            if ((type == null) || (type.Name == null))
                Type.Name = new DDType(string.Empty);
            else
                Type = type;
        }

        public DDNode(string name)
            : this(name, new DDType(String.Empty))
        { }

        public DDNode(Enum name)
            : this(name.ToString())
        { }
        public DDNode(Enum name, DDType type)
            : this(name.ToString(), type)
        { }
        public DDNode()
            : this(Guid.NewGuid().ToString())
        { }
        public DDNode(DDType type)
            : this(Guid.NewGuid().ToString(), type)
        { }
        private DDNode(DDNode parent)
            : this()
        {
            Parent = parent;
        }
        private DDNode(Enum name, DDNode parent)
            : this(name.ToString(), parent)
        { }
        private DDNode(string name, DDNode parent)
            : this(name)
        {
            Parent = parent;
        }
        private DDNode(string name, DDNode parent, DDType type)
            : this(name, type)
        {
            Parent = parent;
        }
        #endregion Constructor
        #region ISerializable
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDNode(SerializationInfo info, StreamingContext context)
        {
            this.Name = (String)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_NAME, typeof(String));
            this.Type = (DDType)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(DDType));
            this.attributes = (DDAttributesCollection)info.GetValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, typeof(DDAttributesCollection));
            this.childNodes = (Dictionary<string, DDNode>)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_CHILDREN_COUNT, typeof(Dictionary<string, DDNode>));
        }
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_NAME, Name, typeof(String));
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, Type, typeof(DDType));
            info.AddValue(DDSchema.SERIALIZE_NODE_ATTRIBUTE_COLLECTION, attributes, typeof(DDAttributesCollection));
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_CHILDREN_COUNT, childNodes, typeof(Dictionary<string, DDNode>));
        }
        #endregion ISerializable
        #region ICloneable Members

        /// <summary>
        /// Creates a duplicate of this node.
        /// Cloning an Node copies all attributes and their value
        /// This method recursively clones the node and the subtree underneath it. Clone is equivalent to calling CloneNode(true).
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return Clone(true);
        }
        /// <summary>
        /// Creates a duplicate of the node, when overridden in a derived class.
        /// </summary>
        /// <param name="deep">true to recursively clone the subtree under the specified node; false to clone only the specified node. </param>
        /// <returns>The cloned node.</returns>
        public virtual DDNode Clone(bool deep)
        {
            return Clone(deep, false);
        }
        /// <summary>
        /// Creates a duplicate of the node, when overridden in a derived class.
        /// </summary>
        /// <param name="deep">true to recursively clone the subtree under the specified node; false to clone only the specified node. </param>
        /// <param name="mergeParentAttributes">Merges all attributes from all parent nodes (including root node) for attribute collection of cloned node only. 
        /// The conflicted attributes by same name will be skipped.</param>
        /// <returns>The cloned node.</returns>
        public virtual DDNode Clone(bool deep, bool mergeParentAttributes)
        {
            return Clone(deep, mergeParentAttributes, ResolveConflict.SKIP);
        }
        /// <summary>
        /// Returns attributes collection contains merged all attributes from all parent nodes (including root node) for specific node
        /// </summary>
        /// <param name="n">node</param>
        /// <param name="rc">conflict resolution</param>
        public static DDAttributesCollection GetMergedParentAttributes(DDNode n, ResolveConflict rc)
        {
            var current = new DDAttributesCollection();
            while (n != null)
            {
                current.Merge(n.Attributes, rc);
                n = n.Parent;
            }
            return current;
        }
        /// <summary>
        /// Merges all attributes from all parent nodes (including root node) for attribute collection of specific node
        /// </summary>
        /// <param name="n">node</param>
        /// <param name="mergeParentAttributes">Merges all attributes from all parent nodes (including root node) for attribute collection of cloned node only</param>
        /// <param name="rc">conflict resolution for merge attributes</param>
        /// <returns>The cloned node.</returns>
        public virtual DDNode Clone(bool deep, bool mergeParentAttributes, ResolveConflict rc)
        {
            var newNode = new DDNode(this.Name, this.Type);
            if ((HasAttributes) && (!mergeParentAttributes)) newNode.attributes = (DDAttributesCollection)Attributes.Clone();
            if (mergeParentAttributes) newNode.attributes = GetMergedParentAttributes(this, rc);

            if (deep)
            {
                foreach (var childNode in this)
                {
                    newNode.Add(childNode.Value.Clone(true));
                }
            }
            return newNode;
        }
        #endregion
        #region INotifyPropertyChanged Members



        #endregion
        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the Attribute Collection (IEnumerator&lt;KeyValuePair&lt;string, DDNode&gt;&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;KeyValuePair&lt;string, DDNode&gt;&gt;</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the Attribute Collection (IEnumerator&lt;KeyValuePair&lt;string, DDNode&gt;&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;KeyValuePair&lt;string, DDNode&gt;&gt;</returns>
        public virtual IEnumerator<KeyValuePair<string, DDNode>> GetEnumerator()
        {
            foreach (var child in childNodes)
            {
                yield return child;
            }
        }

        #endregion IEnumerable
        #region Properties
        /// <summary>
        /// Type of node
        /// </summary>
        public DDType Type { get; set; }

        /// <summary>
        /// Dictonary of children nodes
        /// </summary>
        protected Dictionary<string, DDNode> childNodes;
        /// <summary>
        /// Gets the parent of this node (for nodes that can have parents).
        /// </summary>
        public virtual DDNode Parent { get; internal set; }
        /// <summary>
        /// Gets the qualified name of the node
        /// </summary>
        public virtual string Name { get; protected set; }
        /// <summary>
        /// The current level of this node
        /// </summary>
        public virtual uint Level
        {
            get
            {
                if (IsRoot) return 1;
                return Parent.Level + 1;
            }
        }
        #endregion Properties
        #region Children
        #region Add
        /// <summary>
        /// Adds a child node with auto generated name
        /// </summary>
        /// <returns>new node</returns>
        public virtual DDNode Add()
        {
            return Add(new DDNode(this));
        }
        /// <summary>
        /// Adds a child node with specified name
        /// </summary>
        /// <param name="name">child node name</param>
        /// <returns>new node</returns>
        public virtual DDNode Add(string name)
        {
            return Add(new DDNode(name, this));
        }

        /// <summary>
        /// Adds a child node with specified name
        /// </summary>
        /// <param name="name">child node name</param>
        /// <param name="type">node type</param>
        /// <returns>new node</returns>
        public virtual DDNode Add(string name, string type)
        {
            return Add(new DDNode(name, this, new DDType(type)));
        }
        /// <summary>
        /// Adds a child node with specified name
        /// </summary>
        /// <param name="name">child node name</param>
        /// <returns>new node</returns>
        public virtual DDNode Add(Enum name)
        {
            return Add(name.ToString());
        }

        /// <summary>
        /// Adds a child node with specified name
        /// </summary>
        /// <param name="name">child node name</param>
        /// <param name="type">node type</param>
        /// <returns>new node</returns>
        public virtual DDNode Add(Enum name, Enum type)
        {
            return Add(name.ToString(), type.ToString());
        }
        /// <summary>
        /// Adds the specified node as child
        /// </summary>
        /// <param name="node">child node</param>
        /// <returns>added child node</returns>
        public virtual DDNode Add(DDNode node)
        {
            return Add(node, ResolveConflict.THROW_EXCEPTION);
        }
        /// <summary>
        /// Adds the specified node as child
        /// </summary>
        /// <param name="node">child node</param>
        /// <param name="resolve">Rules of behavior in conflict resolution names.
        /// Throw a new exception; <exception cref="DDNodeExistsException"/>
        /// Update the existing value;
        /// Skip this action and preserve exists node
        /// </param>
        /// <returns>added child node</returns>
        /// <exception cref="DDNodeNullException"/>
        public virtual DDNode Add(DDNode node, ResolveConflict resolve)
        {
            if (null == node) throw new DDNodeNullException();
            if (Equals(node)) throw new DDNodeAddSelf(node.Path);
            if (node.Parent == null)
                node.Parent = this;
            else
                if (!this.Equals(node.Parent)) throw new DDNodeAddNodeWithParent(node.Path);


            switch (resolve)
            {
                case ResolveConflict.OVERWRITE:
                    if (Contains(node.Name))
                    {
                        Remove(node.Name); //remove exists node
                    }
                    break;
                case ResolveConflict.SKIP:
                    if (this.Contains(node.Name))
                    {
                        return null; // return null because the node was not added
                    }
                    break;
            }
            try
            {
                childNodes.Add(node.Name, node);
            }
            catch (ArgumentException e)
            {
                throw new DDNodeExistsException(node.Name, e);
            }

            return node;
        }
        #endregion Add
        #region ValidateNodeName
        /// <summary>
        /// Checks whether specified name is used as a DDNode name.
        /// The node name cannot contain '/' character. As not well as having a name equal to one '.' or two '..' points.
        ///  </summary>
        /// <param name="name">node name</param>
        /// <returns>return true if name is well, otherwise, false.</returns>
        public static bool IsNameCorect(string name)
        {
            if (name == null) return false;
            if (name.Length == 0) return false;
            if ((name.Length == 1) && (name == ".")) return false;
            if ((name.Length == 2) && (name == "..")) return false;
            if (name.Contains("/")) return false;
            return true;
        }
        #endregion ValidateNodeName
        #region Item

        /// <summary>
        /// Get child node by name
        /// </summary>
        public virtual DDNode this[string name]
        {
            get { return childNodes[name]; }
        }
        /// <summary>
        /// Get child node by name
        /// </summary>
        public virtual DDNode this[Enum name]
        {
            get { return childNodes[name.ToString()]; }
        }
        #endregion Item
        /// <summary>
        /// Gets the child node associated with the specified name.
        /// </summary>
        /// <param name="path">child node name or node path</param>
        /// <param name="node">When this method returns, contains the child node associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized. </param>
        /// <returns>true if the node contains an child node with the specified name; otherwise, false.</returns>
        public virtual bool TryGetNode(Enum path, out DDNode node)
        {
            return TryGetNode(path.ToString(), out node);
        }
        /// <summary>
        /// Gets the child node associated with the specified name.
        /// </summary>
        /// <param name="path">child node name or node path</param>
        /// <param name="node">When this method returns, contains the child node associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized. </param>
        /// <returns>true if the node contains an child node with the specified name; otherwise, false.</returns>
        public virtual bool TryGetNode(string path, out DDNode node)
        {
            node = null;
            try
            {
                //return childNodes.TryGetValue(name, out node);
                node = GetNode(path);
                return (node != null);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether this node contains the child node with specified name.
        /// </summary>
        /// <param name="name">child node name</param>
        /// <returns></returns>
        public virtual bool Contains(string name)
        {
            return childNodes.ContainsKey(name);
        }
        /// <summary>
        /// Removes all children. The all children node leave the parent.
        /// Removes all children nodes. Clear() is equivalent to calling Clear(false).
        /// </summary>
        /// <returns></returns>
        public virtual void Clear()
        {
            Clear(false);
        }
        /// <summary>
        /// Removes all children. The children node leave the parent
        /// </summary>
        /// <param name="deep">true to recursively clear the subtree under the specified node, this method recursively remove the children nodes and the subtree underneath it; false to clear only the children itself.</param>
        public virtual void Clear(bool deep)
        {
            var iter = childNodes.GetEnumerator();

            while (iter.MoveNext())
            {
                var child = iter.Current.Value.LeaveParent();
                if (deep) child.Clear(deep);
                iter = childNodes.GetEnumerator();
            }
        }
        /// <summary>
        /// Remove child node by name.
        /// The child node leave the parent.
        /// </summary>
        /// <param name="name">node name</param>
        public virtual DDNode Remove(string name)
        {
            if (Contains(name)) return GetNode(name).LeaveParent();
            return null;
        }

        /// <summary>
        /// Gets the number of children
        /// </summary>
        public virtual int Count
        {
            get { return childNodes.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether this node has any child nodes.
        /// </summary>
        public virtual bool HasChildNodes
        {
            get { return ((childNodes != null) && (Count != 0)); }
        }
        #endregion Children
        #region IDDNode Members

        /// <summary>
        /// Gets an DDAttrubutesCollection containing the attributes of this node.
        /// </summary>
        protected DDAttributesCollection attributes;
        /// <summary>
        /// Gets an DDAttrubutesCollection containing the attributes of this node.
        /// </summary>
        public virtual DDAttributesCollection Attributes
        {
            get { return attributes; }
            private set { attributes = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this node has any attributes.
        /// </summary>
        public virtual bool HasAttributes
        {
            get { return ((attributes != null) && (attributes.Count != 0)); }
        }

        // it's not good idea
        //public virtual string HierarchicalID
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #region Path
        /// <summary>
        /// returns the absolute path to the current node
        /// </summary>
        public virtual string Path
        {
            get { return GetPath(); }
        }
        /// <summary>
        /// Absolute path to the current node
        /// </summary>
        /// <returns>Returns the absolute path to the current node</returns>
        public virtual string GetPath()
        {
            if (IsRoot) return "/";
            if (Parent.IsRoot) return "/" + Name; // ignored root name
            return Parent.GetPath() + "/" + Name;
        }
        #endregion Path
        #region Root
        /// <summary>
        /// Returns true if the current node is a root node, so contains no parent node
        /// </summary>
        public virtual bool IsRoot
        {
            get { return (this.Parent == null); }
        }
        /// <summary>
        /// Gets the Root node
        /// </summary>
        public virtual DDNode GetRoot()
        {
            if (IsRoot) return this;
            return Parent.GetRoot();
        }
        #endregion Root

        #region GetNode
        /// <summary>
        /// Return node by path
        /// </summary>
        /// <param name="path">Path to node. Supported relative and absolute paths. For access to root  node specify '/'. Use '.' (dot) for access to yourself and '..' (double dot) for up to parent node. </param>
        /// <returns></returns>
        public virtual DDNode GetNode(Enum path)
        {
            return GetNode(path.ToString());
        }
        /// <summary>
        /// Return node by path
        /// </summary>
        /// <param name="path">Path to node. Supported relative and absolute paths. For access to root  node specify '/'. Use '.' (dot) for access to yourself and '..' (double dot) for up to parent node. </param>
        /// <returns></returns>
        public virtual DDNode GetNode(string path)
        {
            if (path == null) throw new DDNodeNullPathException();
            if (path.Length == 0) return this; // done
            var nextNodeName = GetNextNodeNameByPath(ref path);
            switch (nextNodeName)
            {
                case "/":
                    return this.GetRoot().GetNode(path);
                case "..":
                    if (this.Parent == null) throw new DDNodePathAboveRootException("..");
                    return this.Parent.GetNode(path);
                case ".":
                    return this.GetNode(path);
                default:
                    return this[nextNodeName].GetNode(path);
            }
        }

        /// <summary>
        /// Return next node name by path
        /// </summary>
        /// <param name="path">Path to node. For access to root  node specify '/'. Use '.' (dot) for access to yourself and '..' (double dot) for up to parent node. </param>
        /// <returns></returns>
        protected static string GetNextNodeNameByPath(ref string path)
        {
            string name = string.Empty;
            if (path == null) throw new DDNodeNullPathException();
            var iRes = path.IndexOf('/');
            if (iRes == -1) //not found
            {
                name = path;
                path = string.Empty;
                return name;
            }
            if (iRes == 0)
                name = path.Substring(0, iRes + 1); // if path none ends with '/' 
            else
                name = path.Substring(0, iRes);
            path = path.Substring(iRes + 1);
            if (path == "/") path = string.Empty; // loop protection if path ends with '/'
            return name;
        }

        #endregion GetNode

        #endregion
        #region Functions
        /// <summary>
        /// Leave the parent node.
        /// This node will be leave from current parent and node became a independent parent
        /// </summary>
        public virtual DDNode LeaveParent()
        {
            if (Parent != null)
            {
                Parent.childNodes.Remove(Name); // don't use Parent.Remove to prevent stupid looping (Stack overflow)
                Parent = null;
            }
            //Level = 1;
            return this;
        }

        #endregion Functions
        #region Equals
        /// <summary>
        /// Determines whether the specified DDNode is equal to the current DDNode. (Inherited from Object.)
        /// </summary>
        /// <param name="other">The DDNode to compare with the current DDNode. </param>
        /// <returns>true if the specified DDNode is equal to the current DDNode otherwise, false.</returns>
        public virtual bool Equals(DDNode other)
        {
            return base.Equals(other);
        }
        /// <summary>
        /// Determines whether the specified object is equal to the current DDNode. (Inherited from Object.)
        /// </summary>
        /// <param name="other">The object to compare with the current DDNode. </param>
        /// <returns>true if the specified object is equal to the current DDNode otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other.GetType() != typeof(DDNode)) return false;
            return base.Equals(other);
        }
        #endregion Equals
        #region GetHashCode
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current DDNode.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion GetHashCode
        #region ==, != operators
        /// <summary>
        /// Compare both values and return true if type and data are same otherwise return false.
        /// The both null object is equal and return value will be true. Very slow
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if type and data are same otherwise return false</returns>
        public static bool operator ==(DDNode value1, DDNode value2)
        {
            return (Compare(value1, value2) == 0);
        }
        public static bool operator !=(DDNode value1, DDNode value2)
        {
            return (!(value1 == value2));
        }
        #endregion ==, != operators
        #region IComparable
        /// <summary>
        /// The result of the comparison of two notes
        /// </summary>
        public enum NODE_COMPARISION_RESULT : int
        {
            /// <summary>
            /// The both nodes are equal. (Including if both nodes are null)
            /// </summary>
            NODE_EQUAL = 0,
            /// <summary>
            /// Attributes nodes or attributes of her children are not equal
            /// </summary>
            NODE_ATTRIBUTES_NOT_EQUAL = 1,
            /// <summary>
            /// Only one of the nodes is equal to zero
            /// </summary>
            NODE_NULL = 2,
            /// <summary>
            /// Type of node or the type of her children are not equal
            /// </summary>
            NODE_TYPE_MISMATCH = 3,
            /// <summary>
            /// Child nodes are not equal
            /// </summary>
            NODE_CHILD_NODES_NOT_EQUAL = 4,
        }

        /// <summary>
        /// Compares the two DDNode of the same values and returns an integer that indicates whether the current instance precedes. Very slow. 
        /// Return value that indicates the relative order of the objects being compared. The return value can be one of the <typeparamref name="NODE_COMPARISION_RESULT"/>
        /// </summary>
        /// <param name="value1">First DDNode to compare</param>
        /// <param name="value2">Second DDNode to compare</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value can be one of the <typeparamref name="NODE_COMPARISION_RESULT"/></returns>>
        /// Zero - the both DDNode have some items and their values otherwise nodes are not equals
        /// <remarks>Very slow. The following properties are not involved in the comparison:
        /// - IsRoot; - Path; - Level
        /// </remarks>
        public static int Compare(DDNode value1, DDNode value2)
        {


            if (((object)value1 == null) && ((object)value2 == null)) return (int)NODE_COMPARISION_RESULT.NODE_EQUAL; // if both are null -> return true
            if (((object)value1 == null) || ((object)value2 == null)) return (int)NODE_COMPARISION_RESULT.NODE_NULL; // if only one of them are null ->  return false

            if ((value1.Name != value2.Name)) return 1; // if Name is not Equals->  return false
            if (value1.Type.CompareTo(value2.Type) != 0) return (int)NODE_COMPARISION_RESULT.NODE_TYPE_MISMATCH; // node type is not matched
            if ((value1.HasAttributes != value2.HasAttributes)) return (int)NODE_COMPARISION_RESULT.NODE_ATTRIBUTES_NOT_EQUAL; // if HasAttributes is not Equals->  return false
            if ((value1.HasChildNodes != value2.HasChildNodes)) return 1; // if HasChildNodes is not Equals->  return false
            // [-] fix revert comparison
            if ((value1.Count != value2.Count)) return (int)NODE_COMPARISION_RESULT.NODE_CHILD_NODES_NOT_EQUAL; // if Count of ChildNodes is not Equals->  return false

            int compareResult = DDAttributesCollection.Compare(value1.Attributes, value2.Attributes);
            if (compareResult != 0) return compareResult;

            foreach (var keyValue1 in value1)
            {
                if (!value2.Contains(keyValue1.Key)) return (int)NODE_COMPARISION_RESULT.NODE_CHILD_NODES_NOT_EQUAL; // 
                var valueCompareResult = DDNode.Compare(keyValue1.Value, value2[keyValue1.Key]);
                if (valueCompareResult != 0) return valueCompareResult;
            }
            return 0;
        }
        /// <summary>
        /// Compares the current DDNode instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has two meanings: 
        /// Zero - the both DDNode have some items and their values.
        /// The difference between the number of elements of the first and second DDNode objects
        /// One - values of collection is not equal.</returns>
        /// <remarks>Very slow. The following properties are not involved in the comparison:
        /// - IsRoot; - Path; - Level
        /// </remarks>
        public virtual int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DDNode)) return 1;
            return Compare(this, (DDNode)obj);
        }
        #endregion CompareTo
        #region Names/Values
        /// <summary>
        /// Gets a collection containing the names of child
        /// </summary>
        public virtual Dictionary<string, DDNode>.KeyCollection Names
        {
            get { return this.childNodes.Keys; }
        }
        /// <summary>
        /// Gets a collection containing the values of child
        /// </summary>
        public virtual Dictionary<string, DDNode>.ValueCollection Values
        {
            get { return this.childNodes.Values; }
        }
        #endregion Names/Values
        #region Exception
        /// <summary>
        /// Type of node which describes Exception
        /// </summary>
        public const string TpException = "Exception";
        /// <summary>
        /// Node name which contains Data for current Exception
        /// </summary>
        public const string NdData = "Data";
        /// <summary>
        /// Node name which contains InnerException for current Exception
        /// </summary>
        public const string NdInnerException = "InnerException";
        /// <summary>
        /// Attribute name with stack trace for node which describes Exception
        /// </summary>
        public const string AttStackTrace = "StackTrace";
        /// <summary>
        /// Attribute name with Source for node which describes Exception
        /// </summary>
        public const string AttSource = "Source";
        /// <summary>
        /// Attribute name with HelpLink for node which describes Exception
        /// </summary>
        public const string AttHelpLink = "HelpLink";
        /// <summary>
        /// Attribute name with Type of Exception
        /// </summary>
        public const string AttType = "Type";
        /// <summary>
        /// Attribute name with Message of Exception
        /// </summary>
        public const string AttMessage = "Message";

        /// <summary>
        /// transformation exception to node
        /// </summary>
        /// <param name="e">exception to transform</param>
        /// <returns></returns>
        static public implicit operator DDNode(Exception e)
        {
            var n = new DDNode();
            SetNodeAttributeFromException(n, e);
            return n;
        }
        /// <summary>
        /// Set exception field to node attributes
        /// </summary>
        /// <param name="n"></param>
        /// <param name="e"></param>
        static private void SetNodeAttributeFromException(DDNode n, Exception e)
        {
            n.Type = new DDType(TpException);
            if (e.StackTrace != null) n.Attributes.Add(AttStackTrace, e.StackTrace);
            if (e.Source != null) n.Attributes.Add(AttSource, e.Source);
            if (e.HelpLink != null) n.Attributes.Add(AttHelpLink, e.HelpLink);
            n.Attributes.Add(AttType, e.GetType().Name);
            if (e.Data != null)
            {
                var data = n.Add(NdData);
                foreach (var dKey in e.Data.Keys)
                {
                    data.attributes.Add(dKey.ToString(), e.Data[dKey].ToString());
                }
            }
            n.Attributes.Add(AttMessage, e.Message);
            if (e.InnerException != null)
            {
                var nodeInner = n.Add(NdInnerException);
                SetNodeAttributeFromException(nodeInner, e.InnerException);
            }
        }

        #endregion Exception
        #region Size
        /// <summary>
        /// size in bytes of the stored data for all attributes in the current node and her children
        /// </summary>
        /// <returns></returns>
        public long GetDataSize()
        {
            long size = Attributes.GetDataSize();
            foreach (var value in childNodes.Values)
            {
                if (value != null) size += value.GetDataSize();
            }
            return size;
        }
        /// <summary>
        /// size in bytes of the node name, stored data and names for all attributes in the current node and her children
        /// </summary>
        /// <returns></returns>
        public long GetSize()
        {
            long size = Attributes.GetSize() + Encoding.UTF8.GetBytes(Name ?? String.Empty).LongLength + Encoding.UTF8.GetBytes(Type.Name ?? String.Empty).LongLength; ;
            foreach (var value in childNodes.Values)
            {
                if (value != null) size += value.GetSize();
            }
            return size;
        }
        #endregion Size
        #region Merge
        /// <summary>
        /// Options of merge nodes
        /// </summary>
        [Flags]
        public enum DDNODE_MERGE_OPTION : int
        {
            /// <summary>
            /// Merge attributes collection
            /// </summary>
            ATTRIBUTES = 1,
            /// <summary>
            /// Merge child nodes. It's equals <paramref name="HIERARCHY_ONLY"/>
            /// </summary>
            CHILD_NODES = 2,
            /// <summary>
            /// Merge hierarchy only.
            /// </summary>
            HIERARCHY_ONLY = (CHILD_NODES ^ ATTRIBUTES),
            /// <summary>
            /// Merge hierarchy and attributes
            /// </summary>
            ALL = (ATTRIBUTES | CHILD_NODES)
        }
        /// <summary>
        /// Merge data with source node. The hierarchy and attributes will be merged. In case of conflict, an appropriate exception is thrown
        /// </summary>
        /// <param name="node">Source node. Node will be taken to transfer the data to destination node</param>
        public void Merge(DDNode node)
        {
            Merge(node, DDNODE_MERGE_OPTION.ALL, ResolveConflict.THROW_EXCEPTION);
        }
        /// <summary>
        /// Merge data with source node
        /// </summary>
        /// <param name="node">Source node. Node will be taken to transfer the data to destination node</param>
        /// <param name="option">Options of merge nodes</param>
        /// <param name="res">The option determine the resolution of conflicts of merge nodes</param>
        public void Merge(DDNode node, DDNODE_MERGE_OPTION option, ResolveConflict res)
        {
            var bMergeAttributes = ((option & DDNODE_MERGE_OPTION.ATTRIBUTES) == DDNODE_MERGE_OPTION.ATTRIBUTES);
            if (bMergeAttributes) Attributes.Merge(node.Attributes, res); // merge attributes
            if ((option & DDNODE_MERGE_OPTION.CHILD_NODES) == DDNODE_MERGE_OPTION.CHILD_NODES)
            {
                foreach (var item in node)
                {
                    if (!Contains(item.Key)) // if child node doesn't exists
                    {
                        DDNode newNode;
                        if (bMergeAttributes) // if need to copy attributes -> copy all: hierarchy and attributes
                        {
                            newNode = item.Value.Clone(true);
                            Add(newNode);
                        }
                        else // if need to copy only hierarchy 
                        {
                            newNode = Add(item.Value.Name);
                            newNode.Merge(item.Value, option, res);
                        }
                    }
                    else
                    {
                        if (res == ResolveConflict.THROW_EXCEPTION) throw new DDNodeMergeNameException(item.Value.Name, Path);
                        GetNode(item.Value.Name).Merge(item.Value, option, res);
                    }
                }
            }
        }
        #endregion Merge
        #region Copy
        #endregion Copy
        public DDNode Copy(DDNode destinationNode, ResolveConflict resolve)
        {
            throw new NotImplementedException();
        }
        #region Move
        /// <summary>
        /// Moves current node as child to destination node. The current and the destination nodes should have the same root.
        /// </summary>
        /// <param name="destinationNode">destination node. New parent for current node</param>
        public DDNode Move(DDNode destinationNode, ResolveConflict resolve)
        {
            throw new NotImplementedException();
            //if (destinationNode == null) throw new DDNodeNullException();
            //if (IsRoot) throw new DDNodeException("Cannot move root node.");
            ////****
            //// need protect move to child node
            //if (AreTheseNodesGrowFromATree(this, destinationNode) == false) throw new DDNodesBelongDifferentTrees(Name, destinationNode.Name);

            //switch (resolve)
            //{
            //    case ResolveConflict.OVERWRITE:
            //        if (destinationNode.Contains(Name))
            //        {
            //            Parent.Remove(Name); //remove self
            //        }
            //        break;
            //    case ResolveConflict.SKIP:
            //        if (destinationNode.Contains(Name))
            //        {
            //            return null; // return null because the node was not added
            //        }
            //        break;
            //}
            //try
            //{
            //    Parent = destinationNode;
            //}
            //catch (ArgumentException e)
            //{
            //    throw new DDNodeExistsException(Name, e);
            //}

        }
        /// <summary>
        /// This function returns true if nodes belong to a same tree, otherwise returns false if the current and specified node don't have the same root node.
        /// </summary>
        /// <param name="n">node to check root</param>
        /// <returns></returns>
        public static bool AreTheseNodesGrowFromATree(DDNode n1, DDNode n2)
        {
            return n1.GetRoot().Equals(n2.GetRoot());
        }
        #endregion Move

    }
}
