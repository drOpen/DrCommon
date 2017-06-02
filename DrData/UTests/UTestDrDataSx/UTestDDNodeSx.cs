using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSx;
using System.IO;
using UTestDrData;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DrOpen.DrCommon.DrData.Exceptions;

namespace UTestDrDataSe
{
    [TestClass]
    public class UTestDDNodeSx
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
            var dtNow = DateTime.Parse("2013-06-14T16:15:30+04");

            var a = new DDNode("a");
            a.Attributes.Add(attStock1Name, "string");
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

        static private DDNode GetStockHierarhyWithArrayValue()
        {
            var dtNow = DateTime.Parse("2013-06-14T16:15:30+04");

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

            return a;
        }

        #region IXmlSerializable

        [TestMethod]
        public void TestDDNodeXmlSerializationGetSchemaNull()
        {
            var dn = GetStockHierarhy();
            Assert.IsNull(((DDNodeSx)dn).GetSchema(), "XML schema should be null.");
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
            var n = new DDNode();
            n.Attributes.Add(true);
            ValidateXMLDeserialization(n);
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

        [TestMethod]
        public void TestDeserialyzeArrayValue()
        {
            ValidateXMLDeserialization(GetStockHierarhyWithArrayValue());
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
            ValidateDeserialization(original, (DDNode)deserialyzed);
        }



        public static MemoryStream XMLSerialyze(DDNode value)
        {

            var n = (DDNodeSx)value;
            var memoryStream = new MemoryStream();

            var serializer = new XmlSerializer(n.GetType());
            serializer.Serialize(memoryStream, n);
            return memoryStream;

        }

        public static DDNodeSx XMLDeserialyze(MemoryStream stream)
        {
            stream.Position = 0;
            var serializer = new XmlSerializer(typeof(DDNodeSx));
            return (DDNodeSx)serializer.Deserialize(stream);
        }


        #endregion IXmlSerializable

        public static void ValidateDeserialization(DDNode original, DDNode deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
        }

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
            var nDestination = new DDNode("Test");
            var nSource = GetStockHierarhy();
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
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
            var nSource = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Source.xml"));

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Destination.xml");
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nSource), UTestDrDataCommon.GetTestMethodName() + "Source.xml");

            nDestination.Merge(nSource, option, res);

            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nDestination), UTestDrDataCommon.GetTestMethodName() + "Actual.xml");
            var nExpected = XMLDeserialyze(UTestDrDataCommon.GetMemoryStreamFromFile(".\\XML\\" + UTestDrDataCommon.GetTestMethodName() + "Expected.xml"));
            UTestDrDataCommon.WriteMemmoryStreamToFile(XMLSerialyze(nExpected), UTestDrDataCommon.GetTestMethodName() + "Expected.xml");

            Assert.IsTrue(nDestination == nExpected, "The actual node is not equal expected node. See xml files in the bin folder.");
        }

        #endregion Merge


    }
}
