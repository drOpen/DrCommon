/*
  DDMsgQueue.cs -- implements messages queue based on layer of abstraction DDNode 1.0.0, August 30, 2015
 
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
using DrOpen.DrData.DrDataObject;
using DrOpen.DrData.DrDataSx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DrOpen.DrCommon.DrMsgQueue
{

    /// <summary>
    /// implements messages queue based on layer of abstraction DDNode
    /// </summary>
    public class DDMsgQueue : IEnumerable<DDNode>, ICollection, IEnumerable, IXmlSerializable
    {
        /// <summary>
        /// queue of nodes
        /// </summary>
        private Queue<DDNode> qDDNode = new Queue<DDNode>();
        /// <summary>
        /// queue of size of nodes
        /// </summary>
        private Queue<long> qSize = new Queue<long>();
        /// <summary>
        /// Returns size of queue
        /// </summary>
        public long Size
        { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the Queue
        /// </summary>
        public int Count
        {
            get { return qDDNode.Count; }
        }

        /// <summary>
        /// Removes all objects from the Queue<>.
        /// </summary>
        public void Clear()
        {
            this.qDDNode.Clear();
            this.qSize.Clear();
            this.Size = 0;
        }
        /// <summary>
        ///  Determines whether an element is in the Queue&lt;DDNode&gt;.
        /// </summary>
        /// <param name="node">The object to locate in the System.Queue&lt;DDNode&gt;. The value can be null for reference types.</param>
        /// <returns>true if item is found in the Queue&lt;DDNode&gt;; otherwise, false.</returns>
        public bool Contains(DDNode node)
        {
            return qDDNode.Contains(node);
        }
        /// <summary>
        /// Removes and returns the object at the beginning of the Queue&lt;DDNode&gt;.
        /// </summary>
        /// <returns>The object that is removed from the beginning of the Queue&lt;DDNode&gt;.</returns>
        public DDNode Dequeue()
        {
            this.Size -= this.qSize.Dequeue(); // decrease the size of the queue
            return this.qDDNode.Dequeue();
        }
        /// <summary>
        /// Adds an object to the end of the Queue&lt;DDNode&gt;.
        /// </summary>
        /// <param name="item">The object to add to the Queue&lt;DDNode&gt;. The value can be null for reference types.</param>
        public void Enqueue(DDNode item)
        {
            var size = item.GetSize();
            this.Size += size; // increase the size of the queue

            this.qDDNode.Enqueue(item);
            this.qSize.Enqueue(size);
        }
        /// <summary>
        /// Returns the object at the beginning of the Queue&lt;DDNode&gt; without removing it.
        /// </summary>
        /// <returns>The object at the beginning of the Queue&lt;DDNode&gt;.</returns>
        public DDNode Peek()
        {
            return this.qDDNode.Peek();
        }

        /// <summary>
        ///  Sets the capacity to the actual number of elements in the Queue&lt;DDNode&gt;, if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            this.qDDNode.TrimExcess();
            this.qSize.TrimExcess();
        }
        /// <summary>
        ///  Returns an enumerator that iterates through the Queue&lt;DDNode&gt;.<T>.
        /// </summary>
        /// <returns></returns>
        IEnumerator<DDNode> IEnumerable<DDNode>.GetEnumerator()
        {
            return this.qDDNode.GetEnumerator();
        }
        /// <summary>
        ///  Returns an enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.qDDNode.GetEnumerator();
        }
        /// <summary>
        /// Copies the elements of the System.Collections.ICollection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.ICollection. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.qDDNode).CopyTo(array, index);
        }
        /// <summary>
        /// Gets a value indicating whether access to the System.Collections.ICollection is synchronized (thread safe).
        /// Returns: true if access to the System.Collections.ICollection is synchronized (thread safe); otherwise, false.
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((ICollection)this.qDDNode).IsSynchronized; }
        }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the System.Collections.ICollection.
        /// Returns: An object that can be used to synchronize access to the System.Collections.ICollection.
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection)this.qDDNode).SyncRoot; }
        }

        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null) from this method, and instead, if specifying a custom schema is required, apply the XmlSchemaProviderAttribute to the class.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(DDNodeSx ));
            foreach (var item in this.qDDNode)
            {
                if (item != null) serializer.Serialize(writer, (DDNodeSx)item);
            }
        }
        /// <summary>
        /// Generates an object from its XML representation.
        /// Method doesn't support - throw new NotSupportedException();
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(XmlReader reader)
        {
            throw new NotSupportedException();
            //reader.MoveToContent();
            //qDDNode = new Queue<DDNode>();
            //qSize = new Queue<long>();
        }
        #endregion IXmlSerializable
    }
}
