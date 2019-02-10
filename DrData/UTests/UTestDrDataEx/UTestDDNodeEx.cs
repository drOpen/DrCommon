using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using System.Diagnostics;
using System.Collections.Generic;

namespace UTestDrDataEx
{
    [TestClass]
    public class UTestDDNodeEx
    {
        private DDNode getStockNode()
        {
            var r = new DDNode("root", "rootType");
            var c1 = r.Add("child1");
            var c2 = r.Add("child2", "child2Type");
            var c3 = r.Add("child3");
            var c31 = c3.Add("child31", "child31Type");
            var c311 = c31.Add("child311", "child311Type");
            var c312 = c31.Add("child312", "child312Type");
            var c313 = c31.Add("child313");
            var c32 = c3.Add("child32", "child32Type");
            return r;
        }

        #region Traverse

        [TestMethod]
        public void TestDDNodeExTraverseEmptyNode()
        {
            var r = new DDNode();
            foreach (var n in r.Traverse())
            {
                Assert.Fail("The root only node was traversed but option process root node wasn't enebled.");
            }
            
        }
        [TestMethod]
        public void TestDDNodeExTraverseNodeWithoutChildAndReturnRoot()
        {
            var name = "Root";
            var r = new DDNode(name);
            int done = 0;
            foreach (var n in r.Traverse(true))
            {
                if (n.Name == name) 
                    done ++;
                else
                    Assert.Fail("The incorrect node '{0}' has been found.", n.Name);
            }
            if (done == 0) Assert.Fail("The root node wasn't traversed.");
            if (done > 1) Assert.Fail("Travers has processed root node more than ones");
        }
        [TestMethod]
        public void TestDDNodeExTraverse()
        {
            var r = getStockNode();
            var names = new string [] {"child1", "child2", "child3", "child31", "child32", "child311", "child312", "child313"};
            int i = 0;
            foreach (var n in r.Traverse())
            {
                Assert.AreEqual(n.Name, names[i++]); 
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes."); 
        }

        [TestMethod]
        public void TestDDNodeExTraverseWithRoot()
        {
            var r = getStockNode();
            var names = new string[] { "root", "child1", "child2", "child3", "child31", "child32", "child311", "child312", "child313" };
            int i = 0;
            foreach (var n in r.Traverse(true))
            {
                Assert.AreEqual(n.Name, names[i++]);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }

        [TestMethod]
        public void TestDDNodeExTraverseSkipWithChild()
        {
            var r = getStockNode();
            var names = new string[] { "child1", "child2", "child3",  "child32"};
            var types = new DDType[] { "child31Type", "child313Type"};
            int i = 0;
            foreach (var n in r.Traverse(false, true, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual( names.Length, i, "Count of returned nodes should be equal count of expected nodes."); 
        }

        [TestMethod]
        public void TestDDNodeExTraverseRootSkipWithChild()
        {
            var r = getStockNode();
            var names = new string[] { "root", "child1", "child2", "child3", "child32" };
            var types = new DDType[] { "child31Type", "child313Type" };
            int i = 0;
            foreach (var n in r.Traverse(true, true, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }

        [TestMethod]
        public void TestDDNodeExTraverseProcessWithOutChild1()
        {
            var r = getStockNode();
            var names = new string[] { };
            var types = new DDType[] { "child31Type", "child313Type" };
            int i = 0;
            foreach (var n in r.Traverse(false, false, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }

        [TestMethod]
        public void TestDDNodeExTraverseRootProcessWithOutChild1()
        {
            var r = getStockNode();
            var names = new string[] {};
            var types = new DDType[] { "child2Type" };
            int i = 0;
            foreach (var n in r.Traverse(true, false, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseRootProcessWithOutChild2()
        {
            var r = getStockNode();
            var names = new string[] { "root", "child2"};
            var types = new DDType[] { "rootType", "child2Type" };
            int i = 0;
            foreach (var n in r.Traverse(true, false, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseProcessWithOutChild2()
        {
            var r = getStockNode();
            var names = new string[] { "child1", "child2", "child3" };
            var types = new DDType[] { "", "child2Type" };
            int i = 0;
            foreach (var n in r.Traverse(false, false, false, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseProcessWithChild1()
        {
            var r = getStockNode();
            var names = new string[] { "child1", "child2", "child3", "child313" };
            var types = new DDType[] { "", "child2Type" };
            int i = 0;
            foreach (var n in r.Traverse(false, false, true, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseProcessRootWithChild1()
        {
            var r = getStockNode();
            var names = new string[] { "child1", "child2", "child3", "child313" };
            var types = new DDType[] { "", "child2Type" };
            int i = 0;
            foreach (var n in r.Traverse(true, false, true, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseProcessWithChild2()
        {
            var r = getStockNode();
            var names = new string[] { "child32" };
            var types = new DDType[] { "child32Type" };
            int i = 0;
            foreach (var n in r.Traverse(false, false, true, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }
        [TestMethod]
        public void TestDDNodeExTraverseSkip()
        {
            var r = getStockNode();
            var names = new string[] { "child1", "child2", "child3", "child32", "child311", "child312", "child313"};
            var types = new DDType[] { "child31Type", "child313Type" }; // child313 - doesn't have a type
            int i = 0;
            foreach (var n in r.Traverse(false, true, true, types))
            {
                Assert.AreEqual(n.Name, names[i++]);
                AssertIncorrectNodeType(n.Type, types);
            }
            Assert.AreEqual(names.Length, i, "Count of returned nodes should be equal count of expected nodes.");
        }

        private static void AssertIncorrectNodeType(DDType type, DDType[] dDTypes)
        {
            foreach (var t in dDTypes)
            {
                if (t.Equals(t)) return;
            }
            Assert.Fail("The node type '{0}' is not matched here.", type);
        }
        #endregion Traverse

    }
}
