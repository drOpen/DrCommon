using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrVar;
using DrOpen.DrCommon.DrVar.Resolver;
using DrOpen.DrCommon.DrVar.Resolver.Item;
using DrOpen.DrCommon.DrVar.Resolver.Token;
using DrOpen.DrCommon.DrVar.Exceptions;

namespace UTestDrVar
{
    [TestClass]
    public class UTestDrVarExceptions
    {
        const string UTestProjectCategory = "DrVar";
        const string UTestClassCategory = "DrVarException";

        private static DrVarPage GetVarPage(RESOLVE_LEVEL resLevel = RESOLVE_LEVEL.ROOT_AND_CHILDREN , 
                                            RESOLVE_AMBIGUITY_OPTION resAmbiguity = RESOLVE_AMBIGUITY_OPTION.RES_UNRESOLVED_KEEP_TEXT)
        {
            return new DrVarPage(new DrOpen.DrCommon.DrVar.Resolver.DrVarTokenMaster(), resLevel, resAmbiguity);
        }
        /// <summary>
        /// Get an exception incorrect variable name. Every variable name has not contain a variable
        /// </summary>
        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
        public void UTestDrVarException_IncorrectVarName_ContainVar()
        {
            var n = new DDNode();
            var vars = new Dictionary<string, string> { { "%var%", "%var%" }, { "% %", "% %" }, { "v%a%r", "%a%" }, { "va%r%", "%r%" } };
            foreach (var v in vars)
            {
                n.Attributes.Clear();
                n.Attributes.Add(v.Key, 1, ResolveConflict.OVERWRITE);
                CheckException_IncorrectVarName_ContainsVar(v.Value, n);
            }

        }

        private void CheckException_IncorrectVarName_ContainsVar(string varName, DDNode n)
        {
            try
            {
                var p = GetVarPage(); 
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
        /// Get an exception loop variable. Variable references to herself
        /// </summary>
        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
                public void UTestDrVarException_LoopVar()
        {
            var n = new DDNode();
            var vars = new Dictionary<string, string> { { "a", "%a%" }, { "A", "bb%A%" }, { "b", "%b%bbb" } };
            foreach (var v in vars)
            {
                n.Attributes.Clear();
                n.Attributes.Add(v.Key, v.Value, ResolveConflict.OVERWRITE);
                CheckException_LoopVar(v.Key, v.Value, n);
            }

        }

        private void CheckException_LoopVar(string varName, string varValue,  DDNode n)
        {
            try
            {
                var p = GetVarPage();
                p.Add(n);
                p.Resolve("");
                Assert.Fail("Variable loop '{0}' has been allowed.", varName);
            }
            catch (DrVarExceptionLoop e)
            {
                Assert.IsTrue(varName == e.Name, "Incorrect exception variable name '{0}'. Expected '{1}'.", e.Name, varName);
                Assert.IsTrue(varValue == e.Value, "Incorrect exception variable value '{0}'. Expected '{1}'.", e.Value, varValue);
            }
            catch (Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message);
            }
        }

        /// <summary>
        /// Get an exception loop variable. Variable references to herself
        /// </summary>
        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
        public void UTestDrVarException_UnresolveVar()
        {
            var n = new DDNode();
            var vars = new Dictionary<string, string> { { "a", "%A%" }, { "A", "AA%a%" }, { "b", "%bb%bbb" } };
            var res = new Dictionary<string, string> { { "a", "A" }, { "A", "a" }, { "b", "bb" } };

            foreach (var v in vars)
            {
                n.Attributes.Clear();
                n.Attributes.Add(v.Key, v.Value, ResolveConflict.OVERWRITE);
                CheckException_UnresolveVar(res[v.Key], n);
            }

        }

        private void CheckException_UnresolveVar(string varName, DDNode n)
        {
            try
            {
                var p = GetVarPage(RESOLVE_LEVEL.ROOT_AND_CHILDREN  ,RESOLVE_AMBIGUITY_OPTION.RES_UNRESOLVED_EXCEPTION);

                p.Add(n);
                p.Resolve("");
                Assert.Fail("Unresolved variable is forbited by rule but '{0}' has been allowed by checker.", varName);
            }
            catch (DrVarExceptionResolve e)
            {
                Assert.IsTrue(varName == e.Name, "Incorrect exception variable name '{0}'. Expected '{1}'.", e.Name, varName);
            }
            catch (Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message);
            }
        }
        /// <summary>
        /// Get an exception if there isn't ending variable sign in a varible name
        /// </summary>
        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
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

        private void CheckException_IncorrectVarName(string varName, DDNode n)
        {
            try
            {
                var p = GetVarPage(); 
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
        [TestMethod, TestCategory(UTestProjectCategory), TestCategory(UTestClassCategory)]
        public void UTestDrVarException_MissEnd()
        {
            var n = new DDNode();
            var aName = "var";
            string[] vars = { "%var", "%", "v%ar", "var%" };
            foreach (var v in vars)
            {
                n.Attributes.Add(aName, v, ResolveConflict.OVERWRITE);
                CheckException_MissEnd(aName, n);
            }

        }

        private void CheckException_MissEnd(string aName, DDNode n)
        {
            var p = GetVarPage(); 
            try
            {
                p.Add(n);
                //p.Compile();
                Assert.Fail("The incorrect variable '{0}' has been allowed.", n.Attributes[aName]);
            }
            catch (DrVarExceptionMissVarEnd e)
            {
                Assert.IsTrue(DrVarSign.varSign.ToString() == e.MissSign, "Incorrect exception sign value '{0}'", e.MissSign);
                Assert.IsTrue(n.Attributes[aName] == e.Value, "Incorrect exception sign value '{0}'", e.MissSign);

            }
            catch (Exception e)
            {
                Assert.Fail("The is incorrect exception type '{0}'. The exception message is '{1}'.", e.GetType(), e.Message);
            }
        }
    }
}
