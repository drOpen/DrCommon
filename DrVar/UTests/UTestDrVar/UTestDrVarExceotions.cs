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
    public class UTestDrVarExceotions
    {
        /// <summary>
        /// Get an exception incorrect variable name. Every variable name has not contain a variable
        /// </summary>
        [TestMethod]
        public void UTestDrVarException_IncorrectVarName_ContainVar()
        {
            var n = new DDNode();
            var vars = new Dictionary <string, string> { {"%var%", "%var%"}, {"% %", "% %"},  {"v%a%r", "%a%"}, {"va%r%", "%r%"} };
            foreach (var v in vars)
            {
                n.Attributes.Clear();
                n.Attributes.Add(v.Key, 1, ResolveConflict.OVERWRITE);
                CheckException_IncorrectVarName_ContainsVar(v.Value, n);
            }

        }

        private void CheckException_IncorrectVarName_ContainsVar(string varName , DDNode n)
        {
            try
            {
                var p = new DrVarPage();
                p.Add(n);
                Assert.Fail("The incorrect variable name '{0}' has been allowed.", varName);
            }
            catch (DrVarExceptionMissName e)
            {
                Assert.IsTrue(varName == e.Name, "Incorrect exception name value '{0}'. Expected '{1}'.", e.Name, varName);

            }
            catch (Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message);
            }
        }

        /// <summary>
        /// Get an exception if there isn't ending variable sign in a varible name
        /// </summary>
        [TestMethod]
        public void UTestDrVarException_IncorrectVar_NameWithoutEndSign()
        {
            var n = new DDNode();
            string[] vars = { "%var", "%", "v%ar", "var%" };
            foreach (var v in vars)
            {
                n.Attributes.Clear();
                n.Attributes.Add(v, 1, ResolveConflict.OVERWRITE);
                CheckException_IncorrectVarName(v, n);
            }

        }

        private void CheckException_IncorrectVarName(string varName , DDNode n)
        {
            try
            {
                var p = new DrVarPage();
                p.Add(n);
                Assert.Fail("The incorrect variable name '{0}' has been allowed.", varName);
            }
            catch (DrVarExceptionMissVarEnd e)
            {
                Assert.IsTrue(DrVarSign.varSign.ToString() == e.MissSign, "Incorrect exception sign value '{0}' for variable '{1}'.", e.MissSign, varName);
                Assert.IsTrue(varName == e.Value, "Incorrect exception text value '{0}'. Expected '{1}'.", e.Value, varName);

            }
            catch (Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message);
            }
        }

        /// <summary>
        /// Get an exception if there isn't ending variable sign
        /// </summary>
        [TestMethod]
        public void UTestDrVarException_MissEnd()
        {
            var n = new DDNode();
            var aName = "var";
            string[] vars = {"%var", "%", "v%ar", "var%"};
            foreach (var v in vars) {
                n.Attributes.Add(aName, v, ResolveConflict.OVERWRITE);
                CheckException_MissEnd(aName, n);
            }

        }

        private void CheckException_MissEnd(string aName, DDNode n)
        {
            var p = new DrVarPage();
            p.Add(n);
            try
            {
                p.Compile();
                Assert.Fail("The incorrect variable '{0}' has been allowed.", n.Attributes[aName]);
            }
            catch (DrVarExceptionMissVarEnd e)
            {
                Assert.IsTrue(DrVarSign.varSign.ToString() ==  e.MissSign, "Incorrect exception sign value '{0}'", e.MissSign);
                Assert.IsTrue(n.Attributes[aName] ==  e.Value, "Incorrect exception sign value '{0}'", e.MissSign);

            }
            catch(Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message); 
            }
        }
    }
}
