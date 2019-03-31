using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrVar;
using DrOpen.DrCommon.DrVar.Eception;

namespace UTestDrVar
{
    [TestClass]
    public class UTestDrVarResolve
    {

        const string UTestProjectCategory = "DrVar";
        const string UTestClassCategory = "DrVarResolver";

        const string VAR_TYPE = "DrTest.VarType";

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
            var p = new DrVarPage();
            p.Add(getVarsNode(), VAR_TYPE);

        }
    }
}
