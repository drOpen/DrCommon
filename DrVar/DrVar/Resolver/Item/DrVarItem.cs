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
using System.Text;

namespace DrOpen.DrCommon.DrVar.Resolver.Item
{
    internal class DrVarItem : ICloneable, IResolved
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public DrVarTokenStack Token { get; private set; }

        public DrVarItem(string name, string value, DrVarTokenStack token)
        {
            //var DynNames = DrVarTokenStack.GetItemsList(name);
            //if (DynNames.Count() != 0) throw new DrVarExceptionMissName(DynNames.First<DrVarToken>().FullName);
            this.Name = name;
            this.Value = value;
            this.Token = token; 
        }

        public DrVarItem(DrVarItem varItem)
        {
            this.Name = varItem.Name;
            this.Value = varItem.Value;
            this.Token = varItem.Token.Clone();
        }
/*
        internal int Resolve(DrVarToken item, string value)
        {
            if (item.FullName == value) // replaces with the same value ! loop protection
            {
                return (item.EndIndex + 1 >= this.Value.Length) ? Token.Parse(String.Empty) : Token.Parse(this.Value.Substring(item.EndIndex , this.Value.Length - item.EndIndex)); // skip this item 
            }
            else
            {
                var pref = this.Value.Substring(0, item.StartIndex);
                var suff = (item.EndIndex + 1 >= this.Value.Length) ? String.Empty : this.Value.Substring(item.EndIndex , this.Value.Length - item.EndIndex );
                this.Value = pref + value + suff;
                return Token.Parse(this.Value);
            }
        }
*/
        /// <summary>
        /// Detects self var loop and if it exists will throw the DrVarExceptionLoop
        /// </summary>
        /// <param name="en">var entry for analyze</param>
        /// <exception cref="DrVarExceptionLoop" />
        public void CheckLoopAndThrowException()
        {
            foreach (var it in Token)
            {
                if (it.Name == Name) // self reference detected
                    throw new DrVarExceptionLoop(Name, Value);
            }
        }


        public DrVarItem Clone()
        {
            return new DrVarItem(this);
        }

        object ICloneable.Clone()
        {
            return (DrVarItem)Clone();
        }


        /// <summary>
        /// returns true if this variable doesn't have a referebce to another
        /// </summary>
        /// <returns>returns true if this variable doesn't have a referebce to another</returns>
        public bool IsResolved
        {
            get { return (this.Token.Count() == 0); }
        }
    }
}
