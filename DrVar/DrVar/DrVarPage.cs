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
using System.Text;
using System.Collections.Generic;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrVar.Exceptions;
using DrOpen.DrCommon.DrVar.Resolver.Token;
using DrOpen.DrCommon.DrVar.Resolver.Item;
using DrOpen.DrCommon.DrVar.Resolver;

namespace DrOpen.DrCommon.DrVar
{

    [Flags]
    public enum RESOLVE_LEVEL : int
    {
        ROOT = 1,
        ROOT_ONLY = 2,
        CHILDREN = 4,
        ROOT_AND_CHILDREN = ROOT | CHILDREN
    }

    internal class DrVarPage
    {

       
       

        private DrVarItemManager itemManagerRaw;
        private DrVarItemManager  itemManager;
        public DrVarTokenMaster TokenMaster { get; private set; }
        public RESOLVE_LEVEL ResolveLevel { get; private set; }
        #region DrVarPage

        public DrVarPage(DrVarTokenMaster tokenMaster, RESOLVE_LEVEL resLevel = RESOLVE_LEVEL.ROOT_AND_CHILDREN, RESOLVE_AMBIGUITY_OPTION resAmbiguity = RESOLVE_AMBIGUITY_OPTION.RES_UNRESOLVED_KEEP_TEXT)
        {
            this.TokenMaster = tokenMaster;
            this.ResolveLevel = resLevel;
            this.itemManager = new DrVarItemManager(tokenMaster, resAmbiguity);
            this.itemManagerRaw = new DrVarItemManager(tokenMaster, resAmbiguity);

        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        public DrVarPage(DrVarPage page)
        {
            this.TokenMaster = page.TokenMaster;
            this.ResolveLevel = page.ResolveLevel;
            this.itemManagerRaw = page.itemManagerRaw.Clone();
            this.itemManager = page.itemManager.Clone();
        }


        public DrVarPage(DrVarTokenMaster tokenMaster, DDNode node)
            : this(tokenMaster)
        {
            this.Add(node);
        }
        public DrVarPage(DrVarTokenMaster tokenMaster, DDNode node, params DDType[] t)
            : this(tokenMaster)
        {
            this.Add(node, t);
        }
        #endregion DrVarPage


        #region Add
        /// <summary>
        /// Add variables from specified node and children. You can specify node types which contain variables. By default, all nodes will analyze 
        /// </summary>
        /// <param name="node">node contains variables </param>
        /// <param name="t">node types which contains varibles</param>
        public void Add(DDNode node, params DDType[] t)
        {
            foreach (var n in node.Traverse(true, false, true, t))
                foreach (var a in n.Attributes)
                    Add(a);
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
            itemManager.Add(name, value);
        }
        #endregion Add


        public void Resolve(string s)
        {

            var par = new DrVarTokenStack(s, TokenMaster.TokenSign, TokenMaster.TokenEscape);
            if (par.AreThereVars)
            {

            }

            foreach (var en in itemManager.Values)
            {
                while (en.IsResolved == false)
                {
                   // var item = en.Items.First<DrVarItem>();
                   // var value = GetVarItemValue(item);
                   // en.Resolve(item, value); ;
                }
            }

        }

        public void Resolve(DDValue v)
        {
            if (v.Type == typeof(string)) 
            {
                Resolve(v.GetValueAs<string>());
            }

            if (v.Type == (typeof(string[])))
            {
                var tmp = v.GetValueAsArray<string>();
                foreach (var s in v.GetValueAsArray<string>())
                {

                }
            }
        }

        public void Resolve(DDNode n, RESOLVE_LEVEL deep, params DDType[] t)
        {
            //if (!IsResolved) ResolveSelf();
            // process root only
            if ((deep & RESOLVE_LEVEL.ROOT_ONLY) == RESOLVE_LEVEL.ROOT_ONLY)
            {
                foreach (var v in n.Attributes.Values)
                    Resolve(v);
            }
            else
            {
                var root = (deep & RESOLVE_LEVEL.ROOT) == RESOLVE_LEVEL.ROOT;
                var children = (deep & RESOLVE_LEVEL.CHILDREN) == RESOLVE_LEVEL.CHILDREN;

                foreach (var it in n.Traverse(root, (t != null && t.Length != 0), children, t))
                    foreach (var v in it.Attributes.Values)
                        Resolve(v);
            }
        }


        private void ResolveSelf()
        {
            if (itemManager.IsResolved) return;

            itemManager = itemManagerRaw.Clone();
            foreach (var en in itemManager.Values)
            {
                while (en.IsResolved == false)
                {
                    en.CheckLoopAndThrowException();
                    //var item = en.Items.First<DrVarItem>();
                    //var value = GetVarItemValue(item);
                    //en.Resolve(item, value); ;
                }
            }
        }
        /// <summary>
        /// Returns raw variables as attribute collection of a DDNode
        /// </summary>
        /// <returns></returns>
        public DDNode GetVarRaw()
        {
            return GetVarRaw(null);
        }
        /// <summary>
        /// Returns raw variables as attribute collection of a DDNode with specified node type
        /// </summary>
        /// <param name="t">node type</param>
        /// <returns></returns>
        public DDNode GetVarRaw(DDType t)
        {
            return itemManagerRaw.Convert(t);
        }

        /// <summary>
        /// Returns resolved variables as attribute collection of a DDNode
        /// </summary>
        /// <returns></returns>
        public DDNode GetVarResolved()
        {
            return GetVarResolved(null);
        }
        /// <summary>
        /// Returns resolved variables as attribute collection of a DDNode with specified node type
        /// </summary>
        /// <param name="t">node type</param>
        /// <returns></returns>
        public DDNode GetVarResolved(DDType t)
        {
            ResolveSelf();
            return itemManager.Convert(t);
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
