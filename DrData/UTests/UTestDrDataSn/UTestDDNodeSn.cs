using System;
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
            var a = new DDNode(Guid.Empty.ToString(), String.Empty);
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
            return a;
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {
            var a = new DDNode(Guid.Empty.ToString(), String.Empty);
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

        #endregion IXmlSerializable

        public static void ValidateDeserialization(DDNode original, DDNode deserialized)
        {
            Assert.IsTrue(original == deserialized, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialized, "Deserialized object should not be same as original object.");
        }
    }
}
