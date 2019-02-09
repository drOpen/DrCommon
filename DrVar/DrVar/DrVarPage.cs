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

namespace DrOpen.DrCommon.DrVar
{
    public class DrVarPage
    {

        private DDNode pageRaw;
        private DDNode page;

        #region DrVarPage
        public DrVarPage()
        {
            pageRaw = new DDNode();
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

        public void Add(DDNode node, params DDType[] t)
        {
            foreach (var n in node.Traverse(true, false, true, t))
            {
                pageRaw.Attributes.Merge(n.Attributes, ResolveConflict.OVERWRITE);
            }
        }

        public void Compile()
        {
            page = new DDNode();
            page = pageRaw.Clone(false);


        }

        public void ValidateVariablesName()
        {
            var item = new Item.DrVarItemsList();
            foreach (var a in page.Attributes.Names)
            {
                if (item.Parse(a) != 0) throw new DrVarExceptionMissName(item.ToArray<Item.DrVarItem>() [0].FullName);
            }
        }

    }
}
