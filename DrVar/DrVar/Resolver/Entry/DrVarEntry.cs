/*
  DrVarEntry.cs -- variable entry for 'DrVar' general purpose variables builder 1.1.0, February 09, 2019
  
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
using DrOpen.DrCommon.DrVar.Exceptions;
using DrOpen.DrCommon.DrVar.Resolver.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrOpen.DrCommon.DrVar.Resolver.Entry
{
    internal class DrVarEntry : ICloneable
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public DrVarTokenList Items { get; private set; }

        public DrVarEntry(string name, string value)
        {
            var DynNames = DrVarTokenList.GetItemsList(name);
            if (DynNames.Count() != 0) throw new DrVarExceptionMissName(DynNames.First<DrVarToken>().FullName);
            this.Name = name;
            this.Value = value;
            this.Items = DrVarTokenList.GetItemsList(value);
        }

        public DrVarEntry(DrVarEntry entry)
        {
            this.Name = entry.Name;
            this.Value = entry.Value;
            this.Items = entry.Items.Clone();
        }

        internal int Resolve(DrVarToken item, string value)
        {
            if (item.FullName == value) // replaces with the same value ! loop protection
            {
                return (item.EndIndex + 1 >= this.Value.Length) ? Items.Parse(String.Empty) : Items.Parse(this.Value.Substring(item.EndIndex , this.Value.Length - item.EndIndex)); // skip this item 
            }
            else
            {
                var pref = this.Value.Substring(0, item.StartIndex);
                var suff = (item.EndIndex + 1 >= this.Value.Length) ? String.Empty : this.Value.Substring(item.EndIndex , this.Value.Length - item.EndIndex );
                this.Value = pref + value + suff;
                return Items.Parse(this.Value);
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


        public DrVarEntry Clone()
        {
            return new DrVarEntry(this);
        }

        object ICloneable.Clone()
        {
            return (DrVarEntry)Clone();
        }

    }
}
