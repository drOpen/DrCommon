using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSx;
using System.IO;
using UTestDrData;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace UTestDrDataSe
{
    [TestClass]
    public class UTestDDAttributesCollectionSx
    {

        private enum TEST_ENUM
        {
            TEST_ENUM_A,
            TEST_ENUM_a,
            TEST_ENUM_B,
            TEST_ENUM_NULL,
            TEST_ENUM_DONT_ADD_WITH_NAME
        }
        private DDAttributesCollection GetStockAttributesCollection()
        {
            var attrs = new DDAttributesCollection();
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"), ResolveConflict.THROW_EXCEPTION) == TEST_ENUM.TEST_ENUM_A.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_B, new DDValue("B"), ResolveConflict.OVERWRITE) == TEST_ENUM.TEST_ENUM_B.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_a, new DDValue("a"), ResolveConflict.OVERWRITE) == TEST_ENUM.TEST_ENUM_a.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("Skipped"), ResolveConflict.SKIP) == null, "Skipped attribute name should be null.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_NULL, null) == TEST_ENUM.TEST_ENUM_NULL.ToString(), "Incorrect attribute value.");

            return attrs;
        }

        #region ISerializable
        [TestMethod]
        public void TestDDAttributesCollectionISerializableNullData()
        {
            var ddAttributesCollection = new DDAttributesCollection();
            ValidateDeserialization(ddAttributesCollection, new BinaryFormatter());
        }
        [TestMethod]
        public void TestDDAttributesCollectionISerializableData()
        {
            var ddAttributesCollection = GetStockAttributesCollection();
            ValidateDeserialization(ddAttributesCollection, new BinaryFormatter());
        }
        public static Stream SerializeItem(DDAttributesCollection iSerializable, IFormatter formatter)
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

        private void ValidateDeserialization(DDAttributesCollection original, IFormatter iFormatter)
        {
            var stream = SerializeItem(original, iFormatter);
            ValidateDeserialization(original, iFormatter, stream);

        }

        private void ValidateDeserialization(DDAttributesCollection original, IFormatter iFormatter, Stream stream)
        {
            stream.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToBinFile((MemoryStream)stream);
            var deserialized = (DDAttributesCollection)DeserializeItem(stream, iFormatter);

            ValidateDeserialization(original, deserialized);
        }

        private void ValidateDeserialization(DDAttributesCollection original, DDAttributesCollection deserialized)
        {
            Assert.IsTrue(original == deserialized, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialized, "Deserialized object should not be same as original object.");
        }
        #endregion ISerializable

        #region IXmlSerializable

        [TestMethod]
        public void TestDDAttributesCollectionXmlDirectSerialization()
        {
            var n = new DDNode("name", "type");
            n.Attributes.Add("bool", false);
            n.Attributes.Add("int", -1);
            n.Add("ChildNode").Add("SubChildNode").Attributes.Add("string", "string");
            StringBuilder sb = new StringBuilder();
            n.Attributes.Serialize(sb);
            var ac = DDAttributesCollectionSxe.Deserialize(sb.ToString());
            ValidateDeserialization(n.Attributes, ac);
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationGetSchemaNull()
        {
            var a = GetStockAttributesCollection();
            Assert.IsNull(((DDAttributesCollectionSx)a).GetSchema(), "XML schema should be null.");
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationAttributeStrongName1()
        {

    

            /*
              var a = GetStockAttributesCollection();
            a.Add(UTestDrDataCommon.ElementNameStrong1, new DDValue());
            ValidateXMLDeserialization(a);
             * */
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationEmpty()
        {
            ValidateXMLDeserialization(new DDAttributesCollection());
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationFromFileEmpty()
        {
            ValidateXMLDeserialization(new DDAttributesCollection(), UTestDrDataCommon.GetMemoryStreamFromFile());
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationFromFileSkipIncorrectValue()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, "AttributeValue");
            a.Add("2", String.Empty);
            a.Add("3", null);
            a.Add("A", null);
            a.Add("B", new DDValue(new string[] { "", "Value_A", "Value_B" }));
            ValidateXMLDeserialization(a, UTestDrDataCommon.GetMemoryStreamFromFile());
        }



        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationEmptyName()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, "Empty");
            ValidateXMLDeserialization(a);
        }



        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationNullValue()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, null);
            ValidateXMLDeserialization(a);
        }

        private void ValidateXMLDeserialization(DDAttributesCollection original)
        {
            var xml = XMLSerialize(original);
            ValidateXMLDeserialization(original, xml);
        }

        private void ValidateXMLDeserialization(DDAttributesCollection original, MemoryStream xml)
        {
            xml.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToXmlFile(xml);
            var deserialized = XMLDeserialize(xml);
            ValidateDeserialization(original, (DDAttributesCollection)deserialized);

        }

        private MemoryStream XMLSerialize(DDAttributesCollection value)
        {
            var v = (DDAttributesCollectionSx)value;
            var memoryStream = new MemoryStream();
            var serializer = new XmlSerializer(v.GetType());
            serializer.Serialize(memoryStream, v);
            return memoryStream;

        }

        private DDAttributesCollectionSx XMLDeserialize(MemoryStream stream)
        {
            var serializer = new XmlSerializer(typeof(DDAttributesCollectionSx));
            return (DDAttributesCollectionSx)serializer.Deserialize(stream);
        }


        #endregion IXmlSerializable
    }
}
