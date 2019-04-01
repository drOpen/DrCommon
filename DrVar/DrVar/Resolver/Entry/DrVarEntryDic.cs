/*
  DrVarEntryDic.cs -- dictonary of variable entry for 'DrVar' general purpose variables builder 1.1.0, February 09, 2019
  
  Copyright (c) 2013-2019 Kudryashov Andrey aka dr
 
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

      Kudryashov Andrey <kudryashov dot andrey at gmail  dot com>

 */
using DrOpen.DrCommon.DrData;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace DrOpen.DrCommon.DrVar.Resolver.Entry
{
    internal class DrVarEntryDic: IEnumerable<KeyValuePair<string, DrVarEntry>>, ICloneable
    {

        public bool IsResolved { get; private set; }
        private Dictionary<string, DrVarEntry> varEntryDic;
        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="vel"></param>
        public DrVarEntryDic(DrVarEntryDic vel)
        {
            foreach (var en in vel)
            {
                varEntryDic.Add(en.Key, en.Value);
            }
            IsResolved = vel.IsResolved;
        }

        public bool Contains(string name)
        {
            return varEntryDic.ContainsKey(name);
        }

        #region Add
        /// <summary>
        /// Add variables from specified node and children. You can specify node types which contain variables. By default, all nodes will analyze 
        /// </summary>
        /// <param name="node">node contains variables </param>
        /// <param name="t">node types which contains varibles</param>
        public void Add(DDNode node, params DDType[] t)
        {
            foreach (var n in node.Traverse(true, false, true, t))
            {
                foreach (var a in n.Attributes)
                {
                    Add(a);
                }
            }
        }
        /// <summary>
        /// Add variable
        /// </summary>
        /// <param name="v">key value pair where key is variable name and value is variable value</param>
        public void Add(KeyValuePair<string, DDValue> v)
        {
            Add(v.Key, v.Value);
        }

        /// <summary>
        /// Add variable
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">variable value</param>
        public void Add(string name, string value)
        {
            if (varEntryDic.ContainsKey(name))
            {
                if (IsResolved) IsResolved = false;
                varEntryDic.Remove(name); // remove previous variable by name
            }
            var entry = new DrVarEntry(name, value);
            if ((IsResolved == true) && (entry.IsResolved() == false)) IsResolved = false;
            varEntryDic.Add(name, entry);
        }

        #endregion Add


        #region Converter
        public DDNode Convert(DDType t = null)
        {
            DDNode v;
            if (t == null)
                v = new DDNode();
            else
                v = new DDNode(t);
            foreach (var en in Values)
                v.Attributes.Add(en.Name, en.Value);
            return v;
        }

        #endregion Converter

        #region Enumerator
        public IEnumerator<KeyValuePair<string, DrVarEntry>> GetEnumerator()
        {
            foreach (var item in varEntryDic)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #region Names/Values
        /// <summary>
        /// Gets a collection containing the names of child
        /// </summary>
        public Dictionary<string, DrVarEntry>.KeyCollection Names
        {
            get { return this.varEntryDic.Keys; }
        }
        /// <summary>
        /// Gets a collection containing the values of child
        /// </summary>
        public virtual Dictionary<string, DrVarEntry>.ValueCollection Values
        {
            get { return this.varEntryDic.Values; }
        }
        #endregion Names/Values
        #endregion Enumerator

        public DrVarEntryDic Clone()
        {
            return new DrVarEntryDic(this);
        }
        /// <summary>
        /// Returns the clone of this class
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

    }
}
