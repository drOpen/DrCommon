/*
  DDEventArgs.cs -- common events argument based on the general purpose Data abstraction layer 1.0.0, August 30, 2015
 
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
using System.Collections;
using System.Collections.Generic;

namespace DrOpen.DrCommon.DrData
{
    public class DDEventArgs : EventArgs, IDictionary<string, DDNode>, IEnumerable<KeyValuePair<string, DDNode>>, ICollection<KeyValuePair<string, DDNode>>
    {
        IDictionary<string, DDNode> args;

        #region DDEventArgs
        public DDEventArgs()
        {
            this.args = new Dictionary<string, DDNode>();
        }
        public DDEventArgs(DDNode value): this((Guid.NewGuid().ToString()), value)
        { }
        public DDEventArgs(string key, DDNode value) : this()
        {
            this.args.Add(key, value);
        }
        public DDEventArgs(params KeyValuePair<string, DDNode>[] args)
        {
            foreach (KeyValuePair<string, DDNode> a in args)
            {
                this.args.Add(a);
            }
        }
        #endregion DDEventArgs

        public DDNode EventData { get; set; }


        public void Add(string key, DDNode value)
        {
            this.args.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this.args.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return this.args.Keys; }
        }

        public bool Remove(string key)
        {
            return this.args.Remove(key);
        }

        public bool TryGetValue(string key, out DDNode value)
        {
            return this.args.TryGetValue(key, out value);
        }

        public ICollection<DDNode> Values
        {
            get { return this.args.Values; }
        }

        public DDNode this[string key]
        {
            get
            {
                return this.args[key];
            }
            set
            {
                this.args[key] = value;
            }
        }

        public void Clear()
        {
            this.args.Clear();
        }

        public int Count
        {
            get { return this.args.Count; }
        }

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
            foreach (var arg in this.args)
            {
                yield return arg;
            }
        }

        #endregion IEnumerable


        public void Add(KeyValuePair<string, DDNode> item)
        {
            this.args.Add(item);
        }

        public bool Contains(KeyValuePair<string, DDNode> item)
        {
            return this.args.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, DDNode>[] array, int arrayIndex)
        {
            this.args.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return this.args.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, DDNode> item)
        {
            return this.args.Remove(item);
        }
    }
}
