/*
  DrVarPage.cs -- variables page of 'DrVar' general purpose Builder variables. 1.1.0, February 09, 2019
 
  Copyright (c) 2013-2019 Kudryashov Andrey aka Dr
 
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrVar.Eception;
using DrOpen.DrCommon.DrVar.Item;

namespace DrOpen.DrCommon.DrVar
{
    public class DrVarPage
    {

        //private DDNode pageRaw;
        //private DDNode page;
        public enum VAR_RESOLVE
        {
            VAR_UNRESOLVED_EXCEPTION = 1,
            VAR_UNRESOLVED_KEEP_TEXT,
            VAR_UNRESOLVED_PUT_EMPTY
        }
        Dictionary<string, DrVarEntry> rawVarEntry;
        Dictionary<string, DrVarEntry> varEntry;
        public bool IsCompiled { get; private set; }
        public VAR_RESOLVE Resolver { get; set; }

        #region DrVarPage

        public DrVarPage()
        {
            Resolver = VAR_RESOLVE.VAR_UNRESOLVED_KEEP_TEXT;
            rawVarEntry = new Dictionary<string, DrVarEntry>();
            varEntry = new Dictionary<string, DrVarEntry>();
        }
        public DrVarPage(VAR_RESOLVE r)
            : this()
        {
            Resolver = r;
        }
        public DrVarPage(DDNode node)
            : this()
        {
            this.Add(node);
        }
        public DrVarPage(DDNode node, params DDType[] t)
            : this()
        {
            this.Add(node, t);
        }
        #endregion DrVarPage


        #region var
        class DrVarEntry : ICloneable
        {
            public string Name { get; private set; }
            public string Value { get; private set; }
            public DrVarItemsList Items { get; private set; }

            public DrVarEntry(string name, string value)
            {
                var DynNames = DrVarItemsList.GetItemsList(name);
                if (DynNames.Count() != 0) throw new DrVarExceptionMissName(DynNames.First<DrVarItem>().FullName);
                this.Name = name;
                this.Value = value;
                this.Items = DrVarItemsList.GetItemsList(value);
            }

            public DrVarEntry(DrVarEntry entry)
            {
                this.Name = entry.Name;
                this.Value = entry.Value;
                this.Items = (DrVarItemsList)entry.Clone();
            }

            internal int Resolve(DrVarItem item, string value)
            {
                if (item.FullName == value) // replaces with the same value ! loop protection
                {
                    return ( item.EndIndex + 1 >= this.Value.Length ) ? Items.Parse(String.Empty) : Items.Parse( this.Value.Substring(item.EndIndex + 1, this.Value.Length - 1)); // skip this item 
                }
                else
                {
                    var pref = this.Value.Substring(0, item.StartIndex);
                    var suff = (item.EndIndex + 1 >= this.Value.Length ) ? String.Empty : this.Value.Substring(item.EndIndex + 1, this.Value.Length - 1);
                    return Items.Parse(pref + value + suff);
                }
            }
            /// <summary>
            /// Detects self var loop and if it exists will throw the DrVarExceptionLoop
            /// </summary>
            /// <param name="en">var entry for analyze</param>
            /// <exception cref="DrVarExceptionLoop" />
            public void CheckLoopAndThrowException()
            {
                foreach (var it in Items)
                {
                    if (it.Name == Name) // self reference detected
                        throw new DrVarExceptionLoop(Name, Value);
                }
            }

            /// <summary>
            /// returns true if this variable doesn't have a referebce to another
            /// </summary>
            /// <returns>returns true if this variable doesn't have a referebce to another</returns>
            public bool IsResolved()
            {
                return (this.Items.Count() == 0);
            }

            public object Clone()
            {
                return new DrVarEntry(this);
            }

        }
        #endregion var


        #region Add
        /// <summary>
        /// Add variables from specified node and cildren. You can specify node types which contain variables. By default, all nodes will analyze 
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
        /// <param name="a">key value pair where key is variable name and value is variable value</param>
        public void Add(KeyValuePair<string, DDValue> a)
        {
            Add(a.Key, a.Value);
        }

        /// <summary>
        /// Add variable
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">variable value</param>
        public void Add(string name, string value)
        {
            if (rawVarEntry.ContainsKey(name))
            {
                if (IsCompiled) IsCompiled = false;
                rawVarEntry.Remove(name); // remove previously value of the same variable
            }
            var entry = new DrVarEntry(name, value);
            if ((IsCompiled == true) && (entry.IsResolved() == false)) IsCompiled = false;
            rawVarEntry.Add(name, entry);
        }

        #endregion Add

        private static Dictionary<string, DrVarEntry> Clone(Dictionary<string, DrVarEntry> dVarEntry)
        {
            return new Dictionary<string, DrVarEntry>(dVarEntry);
        }

        public void Compile()
        {
            //page = new DDNode();
            //page = pageRaw.Clone(false);
            //ValidateVariablesName();
            varEntry.Clear();
            varEntry = Clone(rawVarEntry);
            CompileSelf();
            IsCompiled = true;
        }

        private void CompileSelf()
        {
            foreach (var en in varEntry.Values)
            {
                while (en.IsResolved() == false)
                {
                    en.CheckLoopAndThrowException();
                    var item = en.Items.First<DrVarItem>();
                    var value = GetVarItemValue(item);
                    en.Resolve(item, value); ;
                }
            }
        }
        /// <summary>
        /// Returns value of variable depends on <see cref="Resolver"/> rule. If <see cref="Resolver"/> equals <see cref="VAR_RESOLVE.VAR_UNRESOLVED_EXCEPTION"/> will throw according exception.
        /// </summary>
        /// <param name="item">var item</param>
        /// <returns></returns>
        private string GetVarItemValue(DrVarItem item)
        {
            if (varEntry.ContainsKey(item.Name) == false)
            {
                if (Resolver == VAR_RESOLVE.VAR_UNRESOLVED_EXCEPTION)
                    throw new DrVarExceptionResolve(item.Name);
                else if (Resolver == VAR_RESOLVE.VAR_UNRESOLVED_PUT_EMPTY)
                    return String.Empty;
                else
                    return item.FullName;
            }
            return varEntry[item.Name].Value;
        }




        /*
        private void ValidateVariablesName()
        {
            var item = new Item.DrVarItemsList();
            foreach (var a in page.Attributes.Names)
            {
                if (item.Parse(a) != 0) throw new DrVarExceptionMissName(item.ToArray<Item.DrVarItem>()[0].FullName);
            }
        }
        */
    }
}
