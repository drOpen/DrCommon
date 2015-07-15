/*
  UTestDDNode.cs -- Unit Tests of DDNode for 'DrData' general purpose Data abstraction layer 1.0.1, January 5, 2014
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
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
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace UTestDrData
{
    [TestClass]
    public class UTestDDNode : DDNode
    {
        private enum TEST_ENUM
        {
            TEST_ENUM_A,
            TEST_ENUM_a,
            TEST_ENUM_B,
            TEST_ENUM_NULL,
        }

        static private DDNode GetStockHierarhy()
        {
            var dtNow = DateTime.Parse("2013-06-14T16:15:30+04");

            var a = new DDNode("a");
            a.Attributes.Add("value a->a", "string");
            a.Attributes.Add("value a->b", true);
            var a_b = a.Add("a.b");
            var a_c = a.Add("a.c");
            a_c.Attributes.Add("value a.c->a", "string");
            a_c.Attributes.Add("value a.c->b", true);
            a_c.Attributes.Add("value a.c->c", dtNow);
            var a_b_d = a_b.Add("a.b.d");
            var a_b_d_e = a_b_d.Add("a.b.d.e");
            a_b_d_e.Attributes.Add("value a.b.d.e->a", 1);
            a_b_d_e.Attributes.Add("value a.b.d.e->b", null);
            return a;
        }

        #region GetNextNodeNameByPath

        [TestMethod]
        public void TestGetNextNodeNameByPathNull()
        {
            string path = null;
            try
            {
                var name = DDNode.GetNextNodeNameByPath(ref path);
                Assert.Fail("Cannot catch null reference exception!");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentNullException)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("The incorrect exception type '" + e.ToString() + "'");
                throw;
            }
        }

        [TestMethod]
        public void TestGetNextNodeNameByPathEmpty()
        {
            var path = "";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == string.Empty, "Incorrect path.");
            Assert.IsTrue(name == "", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPathRoot1()
        {
            var path = "/";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == string.Empty, "Incorrect path.");
            Assert.IsTrue(name == "/", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPathRoot2()
        {
            var path = "/ChildName1/ChildName2/";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == "ChildName1/ChildName2/", "Incorrect path.");
            Assert.IsTrue(name == "/", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPath1()
        {
            var path = "ChildName1/ChildName2";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == "ChildName2", "Incorrect path.");
            Assert.IsTrue(name == "ChildName1", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPath2()
        {
            var path = "ChildName1/ChildName2/";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == "ChildName2/", "Incorrect path.");
            Assert.IsTrue(name == "ChildName1", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPath3()
        {
            var path = "ChildName";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == "", "Incorrect path.");
            Assert.IsTrue(name == "ChildName", "Incorrect root.");
        }

        [TestMethod]
        public void TestGetNextNodeNameByPathLoopProtection()
        {
            var path = "ChildName2/";
            var name = DDNode.GetNextNodeNameByPath(ref path);
            Assert.IsTrue(path == string.Empty, "Loop protection if path ends with '/' is incorrected."); // Check  loop protection if path ended by '/' 
            Assert.IsTrue(name == "ChildName2", "Incorrect root.");
        }

        #endregion GetNextNodeNameByPath
        #region GetNode



        [TestMethod]
        public void TestGetNodeByPathNull()
        {
            string path = null;
            try
            {
                var name = GetStockHierarhy().GetNode(path);
                Assert.Fail("Cannot catch null reference exception!");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentNullException)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("The incorrect exception type '" + e.ToString() + "'");
                throw;
            }
        }

        [TestMethod]
        public void TestGetNodeFromRootBySlash()
        {
            var path = "/";
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(root.Path == result.Path, "Get node by path '{0}' is incorrect.", path);
        }

        [TestMethod]
        public void TestGetNodeFromRootByDoubleSlash()
        {
            var path = "//";
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(root.Path == result.Path, "Get node by path '{0}' is incorrect.", path);
        }

        [TestMethod]
        public void TestGetNodeFromRootBySlashPointSlash()
        {
            var path = "/./";
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(root.Path == result.Path, "Get node by path '{0}' is incorrect.", path);
        }

        [TestMethod]
        public void TestGetNodeFromRootByPoint()
        {
            var path = ".";
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(root.Path == result.Path, "Get node by path '{0}' is incorrect.", path);
        }


        [TestMethod]
        public void TestGetChildNodeFromRootByAbsolutePath()
        {
            var path = "/a.b";
            var expectedPath = path;
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(expectedPath == result.Path, "Get node by path '{0}' is incorrect.", path);
        }

        [TestMethod]
        public void TestGetChildNodeFromRootByRelativePath()
        {
            var path = "a.b";
            var expectedPath = "/" + path;
            var root = GetStockHierarhy();
            var result = root.GetNode(path); //get some node
            Assert.IsTrue(expectedPath == result.Path, "Get node by path '{0}' is incorrect.", path);
        }

        [TestMethod]
        public void TestGetNodeFromRootToUpper()
        {
            var path = "..";
            var root = GetStockHierarhy();
            try
            {
                var result = root.GetNode(path); //attempt to rise above root node
                Assert.Fail("Successfull rise above root node!!!");
            }
            catch (ArgumentException)
            {
                // cool throw
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Assert.Fail("Catch incorrect exception after attempt to rise above root node. " + e.Message);
            }

        }

        [TestMethod]
        public void TestGetUpperNode()
        {
            var path = "..";
            var root = GetStockHierarhy();
            var child = root.GetNode("/a.b");
            var getRoot = child.GetNode(path);
            Assert.IsTrue(getRoot.IsRoot, "The root node requirement.");
            Assert.IsTrue(getRoot.Path == "/", "The path for root node should be '/'.");
            Assert.IsTrue(getRoot.Equals(root), "Should be Equals to Root node.");
        }
        [TestMethod]
        public void TestGetNodeByEnum()
        {
            var root = GetStockHierarhy();
            var child1 = root.Add(TEST_ENUM.TEST_ENUM_A);
            var child2 = root.Add(TEST_ENUM.TEST_ENUM_a);

            ValidateNode(child1, TEST_ENUM.TEST_ENUM_A.ToString(), 2);
            ValidateNode(child2, TEST_ENUM.TEST_ENUM_a.ToString(), 2);

            var getChild1 = root.GetNode(TEST_ENUM.TEST_ENUM_A);
            var getChild2 = root[TEST_ENUM.TEST_ENUM_a];

            Assert.AreEqual(getChild1, child1);
            Assert.AreEqual(getChild2, child2);

        }
        #endregion GetNode
        #region GetPath
        [TestMethod]
        public void TestGetPathForRoot()
        {
            var path = "/";
            var root = GetStockHierarhy();
            var child = root.GetNode("/a.b");
            var getRoot = child.GetNode(path);
            Assert.IsTrue(root.GetPath() == path, "The path for root node should be '/'.");
            Assert.IsTrue(getRoot.Path == path, "The path for root node should be '/'.");
        }

        [TestMethod]
        public void TestGetPathForChildNode()
        {
            var path = "/a.b";
            var root = GetStockHierarhy();
            var child = root.GetNode(path);

            Assert.IsTrue(child.GetPath() == path, string.Format("The path for child node should be '{0}'.", path));
            Assert.IsTrue(child.Path == path, string.Format("The path for child node should be '{0}'.", path));
        }
        [TestMethod]
        public void TestGetPathForSubChildNode()
        {
            var path = "/a.b/a.b.d";
            var root = GetStockHierarhy();
            var child = root.GetNode("a.b");
            child = child.GetNode("a.b.d");

            Assert.IsTrue(child.GetPath() == path, string.Format("The path for child node should be '{0}'.", path));
            Assert.IsTrue(child.Path == path, string.Format("The path for child node should be '{0}'.", path));
        }
        #endregion GetPath
        #region Clone
        [TestMethod]
        public void TestCloneSingleNode()
        {
            var root = new DDNode();
            var clone = (DDNode)root.Clone();
            ValidateClone(root, clone, false);
        }

        [TestMethod]
        public void TestCloneNode()
        {
            var root = GetStockHierarhy();
            var deep = false;
            var clone = (DDNode)root.Clone(deep);
            ValidateClone(root, clone, deep);
        }

        [TestMethod]
        public void TestCloneNodesDeep()
        {
            var root = GetStockHierarhy();
            var deep = true;
            var clone = (DDNode)root.Clone(deep);
            ValidateClone(root, clone, deep);
        }

        private void ValidateClone(DDNode sourceNode, DDNode clonedNode, bool deep)
        {
            CompareNodeProperties(sourceNode, clonedNode, deep);
            Assert.AreNotEqual(sourceNode, clonedNode, "The cloned node should not equal to source node.");
            Assert.AreEqual(clonedNode.Path, "/", "The cloned node path should be root '/'.");
            Assert.IsTrue(clonedNode.IsRoot, "The cloned should be root.");
            if (deep)
            {
                Assert.IsTrue(sourceNode.HasChildNodes == clonedNode.HasChildNodes, "The cloned node with child nodes should have child nodes.");
            }
            else
            {
                Assert.IsFalse(clonedNode.HasChildNodes, "The cloned node without child cannot have child nodes.");
            }

        }

        [TestMethod]
        public void TestNodeComporatorForUnitTest()
        {
            var root1 = GetStockHierarhy();
            var root2 = GetStockHierarhy();
            CompareNodeProperties(root1, root2, true);
        }

        private void CompareNodeProperties(DDNode ddNode1, DDNode ddNode2, bool deep)
        {
            Assert.AreEqual(ddNode1.Name, ddNode2.Name, "The node should be have the same name.");
            Assert.AreEqual(ddNode1.HasAttributes, ddNode2.HasAttributes, "The node should be have the attributes.");
            Assert.AreEqual(ddNode1.Attributes.Count, ddNode2.Attributes.Count, "The node should be have the attributes count.");

            CompareAttributeCollection(ddNode1.Attributes, ddNode2.Attributes);

            if (deep)
            {
                foreach (var childNode1 in ddNode1)
                {
                    CompareNodeProperties(childNode1.Value, ddNode2.GetNode(childNode1.Key), deep);
                }


            }

        }

        #endregion Clone
        #region Remove
        [TestMethod]
        public void TestNodeRemoveChild()
        {
            var child_node_name = "a.b";

            var root = GetStockHierarhy();
            var child = root.GetNode(child_node_name);
            var initial_root_child_count = root.Count;
            Assert.IsTrue(root.Count > 0, "Incorrect collection count.");
            ValidateNodeIsNotRoot(child);
            ValidateNodePath(child);

            Assert.IsNotNull(root.Remove(child_node_name), "Cannot remove existent object.");

            ValidateNodeIsRoot(child); // after remove - root
            ValidateNodePath(child);
            ValidateChildNodeCount(root, initial_root_child_count - 1);
            Assert.IsTrue(root.Add(child_node_name).Name == child_node_name, "Cannot add node with same name"); // add new child with same expected 
        }
        [TestMethod]
        public void TestNodeRemoveChildException()
        {
            var child_node_name = "mistake";
            var root = GetStockHierarhy();
            Assert.IsNull(root.Remove(child_node_name), "Removed nonexistent object.");
        }

        #endregion Remove
        #region Clear
        [TestMethod]
        public void TestNodeClear()
        {
            var root = GetStockHierarhy();
            Assert.IsTrue(root.Count > 0, "Incorrect collection count.");
            root.Clear();
            ValidateChildNodeCount(root, 0);
            foreach (var child in root)
            {
                Assert.Fail("there should not  be child objects.");
            }
            root.Add("a.b"); // add child2 
        }

        [TestMethod]
        public void TestNodeClearChild()
        {
            var root = GetStockHierarhy();
            var child = root.GetNode("/a.b/a.b.d");
            Assert.IsTrue(child.Count > 0, "Incorrect collection count.");
            child.Clear();
            Assert.IsTrue(root.Count > 0, "Incorrect collection count.");
            ValidateChildNodeCount(child, 0);
            child.Add("a.b.c.d"); // add child2 
        }
        [TestMethod]
        public void TestNodeClearRootDeepCheckChildReference()
        {
            var root = GetStockHierarhy();
            var child1 = root.GetNode("/a.b/a.b.d");
            var child2 = root.GetNode("/a.b/a.b.d/a.b.d.e");
            root.Clear(true);

            ValidateChildNodeCount(root, 0);

            ValidateChildNodeCount(child1, 0);
            ValidateChildNodeCount(child2, 0);

            ValidateNodeIsRoot(child1);
            ValidateNodeIsRoot(child2);

        }
        [TestMethod]
        public void TestNodeClearRootNotDeepCheckChildReference()
        {
            var root = GetStockHierarhy();
            var child1 = root.GetNode("/a.b");
            var child2 = root.GetNode("/a.b/a.b.d");
            var child3 = root.GetNode("/a.b/a.b.d/a.b.d.e");
            var child3_initial_level = child3.Level;
            root.Clear();

            ValidateChildNodeCount(root, 0);
            ValidateChildNodeCount(child1, 1);
            ValidateChildNodeCount(child2, 1);
            ValidateChildNodeCount(child3, 0);

            ValidateNodeIsRoot(child1);
            ValidateNodeIsNotRoot(child2);
            ValidateNodeIsNotRoot(child3);

            var child3_ref = child1.GetNode("/a.b.d/a.b.d.e");

            ValidateNodeIsRoot(child1);
            ValidateNodeLevel(child3, child3_initial_level - 1);
            CompareNodeProperties(child3, child3_ref, true);
        }
        #endregion Clear
        #region LeaveParent
        [TestMethod]
        public void TestNodeLeaveParentRoot()
        {
            var root = GetStockHierarhy();
            var new_root = root.LeaveParent();
            CompareNodeProperties(root, new_root, true);
            ValidateNodeIsRoot(new_root);
        }
        [TestMethod]
        public void TestNodeLeaveParent()
        {
            var root = GetStockHierarhy();
            var child1 = root.GetNode("/a.b");
            var child2 = root.GetNode("/a.b/a.b.d");
            var child3 = root.GetNode("/a.b/a.b.d/a.b.d.e");

            child3.LeaveParent();
            ValidateNodeIsRoot(child3);
            ValidateNodePath(child3);

            child2.Add(child3); // verify return node back -)
            ValidateNodeIsNotRoot(child3);
            ValidateNodePath(child3);

        }
        #endregion LeaveParent
        #region IsNameCorect
        [TestMethod]
        public void TestIsNameCorectEmpty()
        {
            Assert.IsFalse(DDNode.IsNameCorect(String.Empty), "The empty string is not legal node name.");
        }
        [TestMethod]
        public void TestIsNameCorectNull()
        {
            Assert.IsFalse(DDNode.IsNameCorect(null), "The null is not legal node name.");
        }
        [TestMethod]
        public void TestIsNameCorectDecimalPoint()
        {
            Assert.IsFalse(DDNode.IsNameCorect("."), "The single decimal point '.' is not legal node name.");
            Assert.IsFalse(DDNode.IsNameCorect(".."), "The double decimal points '..' is not legal node name.");
        }
        [TestMethod]
        public void TestIsNameCorectWithSlash()
        {
            AssertFalseNameWithSlash("/");
            AssertFalseNameWithSlash("./.");
            AssertFalseNameWithSlash(" /");
            AssertFalseNameWithSlash("//");
        }

        private void AssertFalseNameWithSlash(string name)
        {
            Assert.IsFalse(DDNode.IsNameCorect(name), String.Format("The name '{0}' with slash '/' is not legal node name.", name));
        }

        #endregion IsNameCorect
        #region TryGetNode
        [TestMethod]
        public void TestTryGetNodeByPathNull()
        {
            string path = null;

            var root = GetStockHierarhy();
            DDNode node = null;
            bool res = false;
            try
            {
                res = root.TryGetNode(path, out node);
            }
            catch
            {
                Assert.Fail("This function should not throw exception.");
            }
            Assert.IsFalse(res, "Return should be 'false'");
        }
        [TestMethod]
        public void TestTryGetNodeByPathEmpty()
        {
            string path = string.Empty;
            var root = GetStockHierarhy();
            DDNode node = null;
            bool res = false;
            try
            {
                res = root.TryGetNode(path, out node);
            }
            catch
            {
                Assert.Fail("This function should not throw exception.");
            }

            Assert.IsTrue(res, "Return should be 'true'");
            Assert.AreEqual(root, node, "Should be same node.");
            ValidateNodeIsRoot(node);
        }
        [TestMethod]
        public void TestTryGetRootNodeByPath()
        {
            string path = "/";
            var root = GetStockHierarhy();
            DDNode node = null;
            bool res = false;
            try
            {
                res = root.TryGetNode(path, out node);
            }
            catch
            {
                Assert.Fail("This function should not throw exception.");
            }

            Assert.IsTrue(res, "Return should be 'true'");
            Assert.AreEqual(root, node, "Should be same node.");
            ValidateNodeIsRoot(node);
        }
        [TestMethod]
        public void TestTryGetNodeByIncorectPath()
        {
            string path = "+++/";
            var root = GetStockHierarhy();
            DDNode node = null;
            bool res = false;
            try
            {
                res = root.TryGetNode(path, out node);
            }
            catch
            {
                Assert.Fail("This function should not throw exception.");
            }
            Assert.IsFalse(res, "Return shold be 'true'");
            Assert.IsNull(node, "Should be null.");
        }
        [TestMethod]
        public void TestTryGeNodeByPath()
        {

            var root = GetStockHierarhy();
            var child1 = root.Add(TEST_ENUM.TEST_ENUM_A);
            var child2 = root.Add(TEST_ENUM.TEST_ENUM_a);

            ValidateNode(child1, TEST_ENUM.TEST_ENUM_A.ToString(), 2);
            ValidateNode(child2, TEST_ENUM.TEST_ENUM_a.ToString(), 2);

            DDNode getChild1;
            DDNode getChild2;
            DDNode getChild3;

            Assert.IsTrue(root.TryGetNode(TEST_ENUM.TEST_ENUM_A, out getChild1));
            Assert.IsTrue(root.TryGetNode(TEST_ENUM.TEST_ENUM_a, out getChild2));
            Assert.IsFalse(root.TryGetNode(TEST_ENUM.TEST_ENUM_NULL, out getChild3));

            Assert.AreEqual(getChild1, child1);
            Assert.AreEqual(getChild2, child2);
            Assert.IsNull(getChild3, "Not existing node must be null.");
        }
        #endregion TryGetNode
        #region Add
        [TestMethod]
        public void TestAddNodeYourSelfAsChildNode()
        {
            var root = GetStockHierarhy();
            try
            {
                root.Add(root);
                Assert.Fail("You can not add yourself as a child node.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* is it ok */}
        }
        [TestMethod]
        public void TestAddNodeWithParent()
        {
            var root = GetStockHierarhy();
            var rootInitialChildCount = root.Count;
            var n1 = root.Add();
            var n2 = n1.Add(n1.Name);

            ValidateNodeIsRoot(root);
            ValidateNodeIsNotRoot(n1, n2);
            ValidateNodeLevel(n1, 2);
            ValidateNodeLevel(n2, 3);
            ValidateChildNodeCount(root, rootInitialChildCount + 1);
            try
            {
                var n3 = root.Add(n2);
                Assert.Fail(String.Format("Forbidden to add node '{0}' as child already having parent node.", n2.Name));
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectNameDot()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                child = root.Add(".");
                Assert.Fail("Forbidden to add node with name '.'.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectNameDoubleDot()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                child = root.Add("..");
                Assert.Fail("Forbidden to add node with name '..'.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectNameWithSlash()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                child = root.Add("ab/c");
                Assert.Fail("Forbidden to add node with name '/'.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectNameSlash()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                child = root.Add("/");
                Assert.Fail("Forbidden to add node with name contains symbol '/'.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectEmptyName()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                child = root.Add("");
                Assert.Fail("Forbidden to add node with empty name.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithIncorrectNameNull()
        {
            var root = GetStockHierarhy();
            DDNode child;
            try
            {
                DDNode node = null;
                child = root.Add(node);
                Assert.Fail("Forbidden to add node as null.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeWithCorrectName()
        {
            var root = GetStockHierarhy();
            DDNode child;
            child = root.Add(".\\.");
            ValidateNodeIsNotRoot(child);
        }
        [TestMethod]
        public void TestAddNodeWithAutogeneratedUniqName()
        {
            var root = GetStockHierarhy();
            var rootInitialChildCount = root.Count;
            var n1 = root.Add();
            var n2 = root.Add();
            var n3 = root.Add();
            ValidateNodeIsRoot(root);
            ValidateNodeIsNotRoot(n1, n2, n3);
            ValidateNodeLevel(n1, 2);
            ValidateNodeLevel(n2, 2);
            ValidateNodeLevel(n3, 2);
            ValidateChildNodeCount(root, rootInitialChildCount + 3);
        }

        [TestMethod]
        public void TestAddNodeValidateUniqName()
        {
            var root = GetStockHierarhy();
            var rootInitialChildCount = root.Count;
            var n1 = root.Add();
            var n2 = n1.Add(n1.Name);

            ValidateNodeIsRoot(root);
            ValidateNodeIsNotRoot(n1, n2);
            ValidateNodeLevel(n1, 2);
            ValidateNodeLevel(n2, 3);
            ValidateChildNodeCount(root, rootInitialChildCount + 1);
            try
            {
                var n3 = root.Add(n1.Name);
                Assert.Fail(String.Format("The name '{0}' is not uniq.", n1.Name));
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {/* it's ok */}
        }
        [TestMethod]
        public void TestAddNodeAsEnum()
        {
            var root = GetStockHierarhy();
            var child = root.Add(TEST_ENUM.TEST_ENUM_a);
            ValidateNodeIsNotRoot(child);
            ValidateNodeLevel(child, 2);
            ValidateNodeName(child, TEST_ENUM.TEST_ENUM_a.ToString());

        }
        [TestMethod]
        public void TestNewNodeAsEnum()
        {
            var root = GetStockHierarhy();
            var child = new DDNode(TEST_ENUM.TEST_ENUM_a);
            ValidateNodeName(child, TEST_ENUM.TEST_ENUM_a.ToString());
            ValidateNodeIsRoot(child);

            root.Add(child);

            ValidateNodeIsNotRoot(child);
            ValidateNodeLevel(child, 2);
            ValidateNodeName(child, TEST_ENUM.TEST_ENUM_a.ToString());

        }
        [TestMethod]
        public void TestCreateRootNodeAutoName()
        {
            var root = new DDNode();
            ValidateNode(root);
        }
        [TestMethod]
        public void TestCreateRootNodeWithName()
        {
            string testName = "name";
            var root = new DDNode(testName);
            ValidateNode(root, testName);
        }
        [TestMethod]
        public void TestAddChild()
        {
            string nameRoot = "Root";
            var root = new DDNode(nameRoot);
            ValidateNode(root, nameRoot);

            string nameChild = "Child";
            var child = new DDNode(nameChild);
            ValidateNode(child, nameChild);

            root.Add(child);
            ValidateNode(root, nameRoot, 1);
            ValidateNode(child, nameChild, 2);
        }

        #endregion Add
        #region transformation
        [TestMethod]
        public void TestTransforException()
        {
            var msg = "Message";
            var msgInner = "InnerMessage";

            var exc = new AccessViolationException(msg, new ExternalException(msgInner));
            try
            {
                throw exc;
            }
            catch (Exception e)
            {
                var node = (DDNode)e;
                CompareNodeWithException(node, e);
            }
        }
        /// <summary>
        /// Recursively compares the values ​​of the fields Exception and attributes of node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="e"></param>
        public void CompareNodeWithException(DDNode node, Exception e)
        {
            Assert.IsTrue(node.Type == "Exception", "Incorect node type. The expected type is 'Exception'.");
            if (e.HelpLink == null)
                Assert.IsTrue(node.Attributes.GetValue("HelpLink", null) == null, "Cannot match 'HelpLink'.");
            else
                Assert.IsTrue(node.Attributes.GetValue("HelpLink", string.Empty) == e.HelpLink, "Cannot match 'HelpLink'.");

            if (e.Message == null)
                Assert.IsTrue(node.Attributes.GetValue("Message", null) == null, "Cannot match 'Message'.");
            else
                Assert.IsTrue(node.Attributes.GetValue("Message", string.Empty) == e.Message, "Cannot match 'Message'.");

            if (e.Source == null)
                Assert.IsTrue(node.Attributes.GetValue("Source", null) == null, "Cannot match 'Source'.");
            else
                Assert.IsTrue(node.Attributes.GetValue("Source", string.Empty) == e.Source, "Cannot match 'Source'.");

            if (e.StackTrace == null)
                Assert.IsTrue(node.Attributes.GetValue("StackTrace", null) == null, "Cannot match 'StackTrace'.");
            else
                Assert.IsTrue(node.Attributes.GetValue("StackTrace", string.Empty) == e.StackTrace, "Cannot match 'StackTrace'.");

            Assert.IsTrue(node.Attributes.GetValue("Type", string.Empty) == e.GetType().Name, "Cannot match 'Type'.");
            if (e.InnerException != null) CompareNodeWithException(node.GetNode("InnerException"), e.InnerException);
        }

        #endregion transformation
        #region IEnumerable.GetEnumerator
        [TestMethod]
        public void TestGetEnumeratorByIEnumerable()
        {
            IEnumerable root = GetStockHierarhy();
            foreach (var node in root)
            {
                ValidateNode(((KeyValuePair<string, DDNode>)node).Value, 2);
            }
        }
        #endregion IEnumerable.GetEnumerator
        #region ValidateNode
        /// <summary>
        /// Validate root node level=1 and expected isn't empty
        /// </summary>
        /// <param expected="node"></param>
        public static void ValidateNode(DDNode node)
        {
            ValidateNode(node, 1);
        }
        /// <summary>
        /// Validate root node level=1 and expected
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="expected"></param>
        public static void ValidateNode(DDNode node, string name)
        {
            ValidateNodeName(node, name);
            ValidateNode(node, 1);
        }
        /// <summary>
        /// validate node
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="expected"></param>
        /// <param expected="level"></param>
        public static void ValidateNode(DDNode node, string name, int level)
        {
            ValidateNodeName(node, name);
            ValidateNode(node, level);
        }
        /// <summary>
        /// Validate node expected isn't empty
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="level"></param>
        public static void ValidateNode(DDNode node, int level)
        {
            ValidateNodeNameIsNotEmpty(node);
            ValidateNodeLevel(node, level);
            if (level == 1)
                ValidateNodeIsRoot(node);
            else
                ValidateNodeIsNotRoot(node);
            ValidateNodePath(node);
        }


        /// <summary>
        /// check that the specified node is a Root
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="count"></param>
        public static void ValidateChildNodeCount(DDNode node, int count)
        {
            Assert.IsTrue(node.Count == count, "The child node count '{0}' is incorrect. The expected value '{1}'.", node.Count, count);
        }

        /// <summary>
        /// check that the specified node is a Root
        /// </summary>
        /// <param expected="node"></param>
        public static void ValidateNodeNameIsNotEmpty(params DDNode[] nodes)
        {
            foreach (var node in nodes)
            {
                Assert.IsFalse(node.Name.Length == 0, "The node name is empty.");
            }
        }

        /// <summary>
        /// check that the specified node expected
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="expected"></param>
        public static void ValidateNodeName(DDNode node, string name)
        {
            Assert.IsTrue(node.Name == name, "The node name '{0}' is incorrect. The correct name is '{1}'.", node.Name, name);
        }

        public static void ValidateNodeLevel(DDNode node, int level)
        {
            ValidateNodeLevel(node, (uint)level);
        }
        /// <summary>
        /// check that the specified node expected
        /// </summary>
        /// <param expected="node"></param>
        /// <param expected="level"></param>
        public static void ValidateNodeLevel(DDNode node, uint level)
        {
            Assert.IsTrue(node.Level == level, "The node '{0}' level is '{1}' is incorrect. The correct level is '{2}'.", node.Name, node.Level, level);
        }
        /// <summary>
        /// check that the specified node is a Root
        /// </summary>
        /// <param expected="node"></param>
        public static void ValidateNodeIsRoot(params DDNode[] nodes)
        {
            foreach (var node in nodes)
            {
                Assert.IsTrue(node.IsRoot, "This node '{0}' isn't root node.", node.Name);
                Assert.IsTrue(node.Parent == null, "This node '{0}' has parent node '{1}'", node.Name, (node.Parent == null ? "" : node.Parent.Name));
                ValidateNodeLevel(node, 1);
            }
        }

        /// <summary>
        /// check that the specified node is a Root
        /// </summary>
        /// <param expected="node"></param>
        public static void ValidateNodeIsNotRoot(params DDNode[] nodes)
        {
            foreach (var node in nodes)
            {
                Assert.IsFalse(node.IsRoot, "This node '{0}' is Root", node.Name);
                Assert.IsFalse(node.Parent == null, "This node '{0}' doesn't have parent node", node.Name);
            }
        }

        /// <summary>
        /// check that the specified node path is Valid
        /// </summary>
        /// <param expected="node"></param>
        public static void ValidateNodePath(params DDNode[] nodes)
        {
            foreach (var node in nodes)
            {
                string path = node.Path;
                string getPath = node.GetPath();
                string validPath = GetNodeAbsolutePath(node);
                Assert.IsTrue(path == validPath, "The absolute node path '{0}' isn't correct. The correct path is '{1}'.", path, validPath);
                Assert.IsTrue(getPath == validPath, "The absolute node path by GetPath() '{0}' isn't correct. The correct path is '{1}'.", path, validPath);
            }
        }

        /// <summary>
        /// Return absolute path
        /// </summary>
        /// <param expected="node"></param>
        /// <returns></returns>
        public static string GetNodeAbsolutePath(DDNode node)
        {
            string result = string.Empty;
            do
            {
                if (node.IsRoot)
                {
                    if (result.Length == 0) result += @"/";
                }
                else
                {
                    result = @"/" + node.Name + result;
                }
                node = node.Parent;
            } while (node != null);
            return result;
        }
        public static void CompareAttributeCollection(DDAttributesCollection at1, DDAttributesCollection at2)
        {
            if ((at1 == null) && (at2 == null)) return;
            Assert.IsTrue(at1.Count == at2.Count, "Number of attributes must be the same in both collections.");
            foreach (var a1 in at1)
            {
                CompareAttribute(a1.Value, at2.GetValue(a1.Key, null));
            }
        }

        public static void CompareAttribute(DDValue v1, DDValue v2)
        {
            if ((v1 == null) && (v2 == null)) return;
            Assert.IsTrue(v1.Size == v2.Size, "The size of value must be the same in both values.");
            Assert.IsTrue(v1.Type == v2.Type, "The type of value must be the same in both values.");
            Assert.IsTrue(v1 == v2, "The value must be the same in both values.");
        }
        #endregion ValidateNode
        #region CompareTo
        [TestMethod]
        public void TestCompareToIncorrectTypeBool()
        {
            var a = GetStockHierarhy();
            var b = true;
            Assert.IsFalse(a.CompareTo(b) == 0, "Objects of different types may not be the same");
        }
        [TestMethod]
        public void TestCompareTo()
        {
            var a = GetStockHierarhy();
            var b = GetStockHierarhy();
            Assert.IsTrue(a.CompareTo(b) == 0, "Compare the same objects should be return 0.");
            Assert.IsTrue(b.CompareTo(a) == 0, "Compare the same objects should be return 0. Revert compare is failed.");
        }
        [TestMethod]
        public void TestCompareToClonnedNodeAfterRemove()
        {
            var a = GetStockHierarhy();
            var b = a.Clone(true);
            b.Add("A.C");
            Assert.IsFalse(a.CompareTo(b) == 0, "Compare the different objects should be return none 0.");
            Assert.IsFalse(b.CompareTo(a) == 0, "Compare the different objects should be return none 0. Revert compare is failed.");
        }
        [TestMethod]
        public void TestCompareToClonnedNodeAfterRemoveRevertCompare()
        {
            // look the TestCompareToClonnedNodeAfterRemove test
            var a = GetStockHierarhy();
            var b = a.Clone(true);
            b.Add("A.C");
            Assert.IsFalse(b.CompareTo(a) == 0, "Compare the different objects should be return none 0. Revert compare is failed.");
            Assert.IsFalse(a.CompareTo(b) == 0, "Compare the different objects should be return none 0.");
        }
        #endregion CompareTo
        #region ISerializable
        [TestMethod]
        public void TestDDNodeISerializableNullData()
        {
            var ddNode = new DDNode();
            ValidateDeserialization(ddNode, new BinaryFormatter());
        }

        public static Stream SerializeItem(DDNode iSerializable, IFormatter formatter)
        {
            var s = new MemoryStream(0);
            SerializeItem(s, iSerializable, formatter);
            return s;
        }
        public static void SerializeItem(Stream stream, ISerializable iSerializable, IFormatter formatter)
        {
            formatter.Serialize(stream, iSerializable);
        }
        public static object DeserializeItem(Stream stream, IFormatter formatter)
        {
            return formatter.Deserialize(stream);
        }

        public static void ValidateDeserialization(DDNode original, IFormatter iFormatter)
        {
            var stream = SerializeItem(original, iFormatter);
            ValidateDeserialization(original, iFormatter, stream);

        }

        public static void ValidateDeserialization(DDNode original, IFormatter iFormatter, Stream stream)
        {
            stream.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToBinFile((MemoryStream)stream);
            var deserialyzed = (DDNode)DeserializeItem(stream, iFormatter);

            ValidateDeserialization(original, deserialyzed);
        }

        public static void ValidateDeserialization(DDNode original, DDNode deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
        }
        #endregion ISerializable
        #region IXmlSerializable

        [TestMethod]
        public void TestDDNodeXmlSerializationGetSchemaNull()
        {
            var dn = GetStockHierarhy();
            Assert.IsNull(dn.GetSchema(), "XML schema should be null.");
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationNode()
        {
            var root = GetStockHierarhy();
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationEmpty()
        {
            var root = new DDNode();
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationWithoutAttributeCollection()
        {
            var stream = UTestDrDataCommon.GetMemoryStreamFromFile();
            stream.Position = 0;
            var deserialyzed = XMLDeserialyze(stream); // check looping

        }

        [TestMethod]
        public void TestDDNodeXmlSerializationHierarchy()
        {
            var root = new DDNode("root", "NodeType");
            var child_level_1 = root.Add("Child_Level_1");
            var child_level_2 = child_level_1.Add("Child_Level_2");
            ValidateXMLDeserialization(root);
        }
        [TestMethod]
        public void TestDDNodeXmlSerializationFromFileSkipIncorrectedData()
        {
            ValidateXMLDeserialization(GetStockHierarhy(), UTestDrDataCommon.GetMemoryStreamFromFile());
        }

        public static void ValidateXMLDeserialization(DDNode original)
        {
            var xml = XMLSerialyze(original);
            ValidateXMLDeserialization(original, xml);
        }

        public static void ValidateXMLDeserialization(DDNode original, MemoryStream xml)
        {
            xml.Position = 0;

            UTestDrDataCommon.WriteMemmoryStreamToXmlFile(xml);
            var deserialyzed = XMLDeserialyze(xml);
            ValidateDeserialization(original, deserialyzed);
        }



        public static MemoryStream XMLSerialyze(DDNode value)
        {

            var memoryStream = new MemoryStream();

            var serializer = new XmlSerializer(value.GetType());
            serializer.Serialize(memoryStream, value);
            return memoryStream;

        }

        public static DDNode XMLDeserialyze(MemoryStream stream)
        {
            stream.Position = 0;
            var serializer = new XmlSerializer(typeof(DDNode));
            return (DDNode)serializer.Deserialize(stream);
        }


        #endregion IXmlSerializable
        #region Merge

        [TestMethod]
        public void TestMerge()
        {
            var dn = GetStockHierarhy();

        }

        [TestMethod]
        public void TestMergeEmptyNodeWithEmptyNode()
        {
            var nDestination = new DDNode ("empty");
            var nSource = new DDNode ("empty");
            nDestination.Merge(nSource);
            Assert.IsTrue(nDestination == nSource, "The both nodes must be equals.");
        }
        [TestMethod]
        public void TestMergeEmptyNodeWithStock()
        {
            var nDestination = new DDNode("Test");
            var nSource = GetStockHierarhy ();
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination== nExpected , "The actual node is not equal expected node. See xml files in the bin folder.");
        }
        [TestMethod]
        public void TestMergeStockCollectionWithEmptyCollection()
        {
            
            var nDestination = GetStockHierarhy();
            var nSource = new DDNode("Test");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }
        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflictAndChild()
        {

            var nDestination = GetStockHierarhy();
            var nSource = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Source.xml"));

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource, DDNODE_MERGE_OPTION.ATTRIBUTES , ResolveConflict.THROW_EXCEPTION);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflictAndAttributes()
        {

            var nDestination = GetStockHierarhy();
            var nSource = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Source.xml"));

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource, DDNODE_MERGE_OPTION.CHILD_NODES, ResolveConflict.THROW_EXCEPTION);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination==nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflict()
        {

            var nDestination = GetStockHierarhy();
            var nSource = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Source.xml"));

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource, DDNODE_MERGE_OPTION.ALL, ResolveConflict.THROW_EXCEPTION);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        #endregion Merge
    }
}
