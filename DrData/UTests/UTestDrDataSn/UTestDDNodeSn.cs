﻿using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSn;
using DrOpen.DrCommon.DrData.Exceptions;

namespace UTestDrDataSn
{
    [TestClass]
    public class UTestDDNodeSn
    {
        private enum TEST_ENUM
        {
            TEST_ENUM_A,
            TEST_ENUM_a,
            TEST_ENUM_B,
            TEST_ENUM_NULL,
        }

        public const string attStock1Name = "value a->a";

        static private DDNode GetStockHierarhy()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var a = new DDNode("a");
            a.Attributes.Add(attStock1Name, "string");
            a.Attributes.Add("value a->b", true);
            var a_b = a.Add("a.b");
            var a_c = a.Add("a.c");
            a_c.Attributes.Add("value a.c->a", "string");
            a_c.Attributes.Add("value a.c->b", true);
            var a_b_d = a_b.Add("a.b.d");
            var a_b_d_e = a_b_d.Add("a.b.d.e");
            a_b_d_e.Attributes.Add("value a.b.d.e->a", 1);
            a_b_d_e.Attributes.Add("value a.b.d.e->b", null);
            root.Add(a);
            return root;
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var a = new DDNode("a");
            a.Attributes.Add("value a->a", new[] { true, false });

            var a_a = a.Add("a.a");
            a_a.Attributes.Add("value a.a->a", new[] { true, false });
            a_a.Attributes.Add("value a.a->b", new[] { true, false });

            var a_a_a = a_a.Add("a.a.a");
            a_a_a.Attributes.Add("Value", new[] { true, false });

            var a_a_a_a = a_a_a.Add("a.a.a.a");
            a_a_a_a.Attributes.Add("Value", new[] { true, false });

            var a_a_a_b = a_a_a.Add("a.a.a.b");
            a_a_a_b.Attributes.Add("Value", new[] { true, false });

            var a_a_a_c = a_a_a.Add("a.a.a.c");
            a_a_a_c.Attributes.Add("Value", new[] { true, false });


            var a_b = a.Add("a.b");
            a_b.Attributes.Add("value a.b->a", new[] { true, false });
            a_b.Attributes.Add("value a.b->b", new[] { true, false });

            root.Add(a);
            return root;
        }

        #region IXmlSerializable

        [TestMethod]
        public void TestDDNodeXmlSerializationGetSchemaNull()
        {
            var dn = GetStockHierarhy();
            Assert.IsNull(((DDNodeSn)dn).GetSchema(), "XML schema should be null.");
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationNodeTwoLevels()
        {
            var n = new DDNode(Guid.Empty.ToString(), String.Empty);
            n.Attributes.Add("_a", "a");
            n.Add("B").Attributes.Add("_b", "b");
            ValidateXMLDeserialization(n);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationNode()
        {
            var root = GetStockHierarhy();
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationSingleNodeAndAtribute()
        {
            var n = new DDNode(Guid.Empty.ToString(), String.Empty);
            n.Attributes.Add(true);
            ValidateXMLDeserialization(n);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationEmpty()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationWithoutAttributeCollection()
        {
            var stream = UTestDrDataCommon.GetMemoryStreamFromFile();
            stream.Position = 0;
            var deserialized = XMLDeserialize(stream); // check looping

        }

        public static void ValidateXMLDeserialization(DDNode original)
        {
            var xml = XMLSerialize(original);
            ValidateXMLDeserialization(original, xml);
        }

        public static void ValidateXMLDeserialization(DDNode original, MemoryStream xml)
        {
            xml.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToXmlFile(xml);

            var deserialized = XMLDeserialize(xml);
            ValidateDeserialization(original, (DDNode)deserialized);
        }

        public static MemoryStream XMLSerialize(DDNode value)
        {

            var n = (DDNodeSn)value;
            var memoryStream = new MemoryStream();

            var serializer = new XmlSerializer(n.GetType());
            serializer.Serialize(memoryStream, n);
            return memoryStream;

        }

        public static DDNodeSn XMLDeserialize(MemoryStream stream)
        {
            stream.Position = 0;
            var serializer = new XmlSerializer(typeof(DDNodeSn));
            return (DDNodeSn)serializer.Deserialize(stream);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationHierarchy()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var child_level_1 = root.Add("Child_Level_1");
            var child_level_2 = child_level_1.Add("Child_Level_2");
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeXmlSerializationFromFileSkipIncorrectedData()
        {
            ValidateXMLDeserialization(GetStockHierarhy(), UTestDrDataCommon.GetMemoryStreamFromFile());
        }

        [TestMethod]
        public void TestDDNodeXmlDirectSerialization()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var n = new DDNode("name", "type");
            n.Attributes.Add("bool", false);
            n.Attributes.Add("int", -1);
            n.Add("ChildNode").Add("SubChildNode").Attributes.Add("string", "string");
            root.Add(n);
            StringBuilder sb = new StringBuilder();
            root.Serialize(sb);
            var d = DDNodeSne.Deserialize(sb.ToString());
            ValidateDeserialization(root, d);
        }

        [TestMethod]
        public void TestDeserializeArrayValue()
        {
            ValidateXMLDeserialization(GetStockHierarhyWithArrayValue());
        }

        [TestMethod]
        public void TestNullSerialization()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var n = new DDNode();
            var a = n.Attributes;
            a.Add(null);
            a.Add(new DDValue());
            a.Add(new DDValue(null));
            a.Add(String.Empty);
            root.Add(n);
            ValidateXMLDeserialization(root);
        }

        #endregion IXmlSerializable
        public static void ValidateDeserialization(DDNode original, DDNode deserialized)
        {
            Assert.IsTrue(original == deserialized, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialized, "Deserialized object should not be same as original object.");
        }

        #region AutoGenerationName

        [TestMethod]
        public void TestDeserializationFromXMLAndAutoNameGeneration()
        {

            var n = DDNodeSne.Deserialize(UTestDrDataCommon.GetMemoryStreamFromFile());
            Assert.IsTrue(n.Type == String.Empty, "Type of nodes are not equals after deserialization with auto node name generation.");
            foreach (var c in n)
            {
                Assert.IsTrue(c.Value.Type == "Type", "Type of nodes are not equals after deserialization with auto node name generation.");
                var en = c.Value.Attributes.GetEnumerator();

                en.MoveNext();
            }

        }

        #endregion AutoGenerationName

        #region Merge
        [TestMethod]
        public void TestMerge()
        {
            var dn = GetStockHierarhy();
        }

        [TestMethod]
        public void TestMergeEmptyNodeWithEmptyNode()
        {
            var nDestination = new DDNode("empty");
            var nSource = new DDNode("empty");
            nDestination.Merge(nSource);
            Assert.IsTrue(nDestination == nSource, "The both nodes must be equals.");
        }

        [TestMethod]
        public void TestMergeEmptyNodeWithStock()
        {
            var nDestination = new DDNode(Guid.Empty.ToString(), String.Empty);
            var nSource = GetStockHierarhy();
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nSource), UTestDrDataCommon.GetTestMethodName() + ".Source.xml");

            nDestination.Merge(nSource);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Actual.xml");
            var nExpected = XMLDeserialize(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + ".Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nExpected), UTestDrDataCommon.GetTestMethodName() + ".Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        [TestMethod]
        public void TestMergeStockCollectionWithEmptyCollection()
        {

            var nDestination = GetStockHierarhy();
            var nSource = new DDNode("Test");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nSource), UTestDrDataCommon.GetTestMethodName() + ".Source.xml");

            nDestination.Merge(nSource);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Actual.xml");
            var nExpected = XMLDeserialize(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + ".Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nExpected), UTestDrDataCommon.GetTestMethodName() + ".Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflictAndChild()
        {
            TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION.ATTRIBUTES, ResolveConflict.THROW_EXCEPTION);
        }

        [TestMethod]
        public void TestMergeConflict()
        {
            var n1 = GetStockHierarhy();
            try
            {
                GetStockHierarhy().Merge(n1, DDNode.DDNODE_MERGE_OPTION.ALL, ResolveConflict.THROW_EXCEPTION);
                Assert.Fail("The DDAttributeExistsException exception isn't raised.");
            }
            catch (DDAttributeExistsException e)
            {
                Assert.AreEqual(attStock1Name, e.Name); // attribute name
            }
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflictAndAttributes()
        {
            TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION.CHILD_NODES, ResolveConflict.THROW_EXCEPTION);
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOutConflict()
        {
            TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION.ALL, ResolveConflict.THROW_EXCEPTION);
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithSkipConflict()
        {
            TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION.ALL, ResolveConflict.SKIP);
        }

        [TestMethod]
        public void TestMergeStockCollectionWithAnotherCollectionWithOverwriteConflict()
        {
            TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION.ALL, ResolveConflict.OVERWRITE);
        }

        private void TestMergeStockCollectionWithAnotherCollection(DDNode.DDNODE_MERGE_OPTION option, ResolveConflict res)
        {
            TestMergeNodeWithAnotherNode(GetStockHierarhy(), option, res);
        }

        private void TestMergeNodeWithAnotherNode(DDNode nDestination, DDNode.DDNODE_MERGE_OPTION option, ResolveConflict res)
        {

            var nSource = XMLDeserialize(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + ".Source.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nSource), UTestDrDataCommon.GetTestMethodName() + ".Source.xml");

            nDestination.Merge(nSource, option, res);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nDestination), UTestDrDataCommon.GetTestMethodName() + ".Actual.xml");
            var nExpected = XMLDeserialize(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + ".Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialize(nExpected), UTestDrDataCommon.GetTestMethodName() + ".Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        #endregion Merge
        #region bugs
        [TestMethod]
        public void TestDDNodeLostNodesAfterDeserializationEmptyStringArray1()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var nSource = new DDNode("Source");
            var attr = nSource.Add("1").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a3", new string[] { });
            attr.Add("a2", new string[] { "1" });
            attr = nSource.Add("2").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a3", new string[] { });
            attr.Add("a2", new string[] { "2" });
            root.Add(nSource);
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeLostNodesAfterDeserializationEmptyStringArray2()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var nSource = new DDNode("Root");
            var attr = nSource.Add("1").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a2", new string[] { "1" });
            attr.Add("a3", new string[] { });
            attr = nSource.Add("2").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a2", new string[] { "2" });
            attr.Add("a3", new string[] { });
            root.Add(nSource);
            ValidateXMLDeserialization(root);
        }

        [TestMethod]
        public void TestDDNodeLostNodesAfterDeserializationEmptyStringArray3IsEmptyElementFalse()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var nExpected = new DDNode("Root");
            var attr = nExpected.Add("1").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a2", new string[] { "1" });
            attr.Add("a3", new string[] { });
            attr = nExpected.Add("2").Attributes;
            attr.Add("a1", new Guid());
            attr.Add("a3", new string[] { });
            attr.Add("a2", new string[] { "2" });
            root.Add(nExpected);

            var nSource = DDNodeSne.Deserialize(@"<?xml version='1.0'?>
                <nr>
                    <n n='Root'>
                    <n n='1'>
                        <a n='a1' t='13' v='00000000-0000-0000-0000-000000000000'></a>
                        <a n='a2' t='258'>
                            <v v='1'></v>
                        </a>
                        <a n='a3' t='258'></a>
                    </n>
                    <n n='2'>
                        <a n='a1' t='13' v='00000000-0000-0000-0000-000000000000'></a>
                        <a n='a3' t='258'></a>
                        <a n='a2' t='258'>
                            <v v='2'></v>
                        </a>
                    </n>
                </n>
            </nr>");
            ValidateDeserialization(nSource, root);
        }

        #endregion bugs

    }
}
