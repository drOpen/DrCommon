using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrVar;
using DrOpen.DrCommon.DrVar.Exceptions;

namespace UTestDrVar
{
    [TestClass]
    public class UTestDrVarResolve
    {

        const string UTestProjectCategory = "DrVar";
        const string UTestClassCategory = "DrVarResolver";

        const string VAR_TYPE = "DrTest.VarType";


        private static DrVarPage GetVarPage()
        {
            return new DrVarPage(new DrOpen.DrCommon.DrVar.Resolver.DrVarTokenMaster());
        }

        private static DDNode getVarsNode()
        {
            var v = new DDNode("root", VAR_TYPE);
            v.Attributes.Add("a", "a");
            v.Attributes.Add("b", int.MaxValue);
            var ch = v.Add("child", VAR_TYPE);
            ch.Attributes.Add("c", "%a%%b%");
            var nn = v.Add("without type");
            nn.Attributes.Add("d", "%a%");
            return v;
        }


        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
        public void UTestDrVarResolver_1()
        {
            var p = GetVarPage();
            p.Add(getVarsNode(), VAR_TYPE);

        }
    }
}
