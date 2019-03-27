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

        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
        public void UTestDrVarResolver_1()
        {
            var p = new DrVarPage();
            //p.Add(n);
        }
    }
}
