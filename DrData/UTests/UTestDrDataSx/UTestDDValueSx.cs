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
    public class UTestDDValueSx
    {
        #region IXmlSerializable

        [TestMethod]
        public void TestDDValueXmlSerializationGetSchemaNull()
        {
            var ddValueSe = (DDValueSx)new DDValue(new[] { "test1", "test2" });
            Assert.IsNull(ddValueSe.GetSchema(), "XML schema should be null.");
        }

        [TestMethod]
        public void TestDDValueXmlSerializationNullFromXML()
        {
            ValidateXMLDeserialization(new DDValue(), UTestDrDataCommon.GetMemoryStreamFromString("<" + DrOpen.DrCommon.DrDataSx.DDSchema.XML_SERIALIZE_NODE_VALUE + "/>"));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationNull()
        {
            ValidateXMLDeserialization(new DDValue());
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyString()
        {
            ValidateXMLDeserialization(new DDValue(string.Empty));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationUint()
        {
            ValidateXMLDeserialization(new DDValue((uint)uint.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationShort()
        {
            ValidateXMLDeserialization(new DDValue((short)short.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationUShort()
        {
            ValidateXMLDeserialization(new DDValue((ushort)ushort.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationDouble()
        {
            ValidateXMLDeserialization(new DDValue((double)double.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationLong()
        {
            ValidateXMLDeserialization(new DDValue((long)long.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationULong()
        {
            ValidateXMLDeserialization(new DDValue((ulong)ulong.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationULongArray()
        {
            ValidateXMLDeserialization(new DDValue(new[] { ulong.MaxValue, ulong.MinValue }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFloat()
        {
            ValidateXMLDeserialization(new DDValue((float)float.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFloatArray()
        {
            ValidateXMLDeserialization(new DDValue(new[] { float.MaxValue, float.MaxValue, float.MinValue }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationSingle()
        {
            ValidateXMLDeserialization(new DDValue((Single)Single.MaxValue));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationSingleArray()
        {
            ValidateXMLDeserialization(new DDValue(new Single[] { Single.MaxValue, Single.MinValue, Single.MaxValue }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationGuid()
        {
            ValidateXMLDeserialization(new DDValue(Guid.NewGuid()));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationGuidArray()
        {
            ValidateXMLDeserialization(new DDValue(new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationChar()
        {
            ValidateXMLDeserialization(new DDValue('c'));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationCharArray()
        {
            ValidateXMLDeserialization(new DDValue("char".ToCharArray()));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFromFileEmptyString()
        {
            ValidateXMLDeserialization(new DDValue(string.Empty), UTestDrDataCommon.GetMemoryStreamFromFile());
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFromFileSkippIncorrectedNodeTypeStringEmpty()
        {
            ValidateXMLDeserialization(new DDValue(string.Empty), UTestDrDataCommon.GetMemoryStreamFromFile());
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyStringArray()
        {
            ValidateXMLDeserialization(new DDValue(new string[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFromFileSkippIncorrectedNodeTypeStringArray()
        {
            ValidateXMLDeserialization(new DDValue(new string[] { }), UTestDrDataCommon.GetMemoryStreamFromFile());
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFromFileSkippIncorrectedNodeTypeStringArrayDeep()
        {
            ValidateXMLDeserialization(new DDValue(new string[] { }), UTestDrDataCommon.GetMemoryStreamFromFile());
        }
        [TestMethod]
        public void TestDDValueXmlSerializationFromFileSkippIncorrectedNodeTypeStringArrayDeepWithValue()
        {
            ValidateXMLDeserialization(new DDValue(new string[] { "TestValue", "TheLastValue" }), UTestDrDataCommon.GetMemoryStreamFromFile());
        }

        [TestMethod]
        public void TestDDValueXmlSerializationStringArrayThreeEmptyElements()
        {
            ValidateXMLDeserialization(new DDValue(new string[] { string.Empty, string.Empty, string.Empty }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationStringArrayThreeElements()
        {
            ValidateXMLDeserialization(new DDValue(new[] { "test1", string.Empty, "test3" }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationUnicodeStringArrayThreeElements()
        {
            ValidateXMLDeserialization(new DDValue(new[] { "test1", "Юникод", "test3" }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyByteArray()
        {
            ValidateXMLDeserialization(new DDValue(new byte[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyDateTimeArray()
        {
            ValidateXMLDeserialization(new DDValue(new DateTime[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationDateTimeArrayThreeElements()
        {
            ValidateXMLDeserialization(new DDValue(new DateTime[] { DateTime.Now, DateTime.Now.AddMinutes(1), DateTime.Now.AddYears(1) }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyBoolArray()
        {
            ValidateXMLDeserialization(new DDValue(new bool[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyGuidArray()
        {
            ValidateXMLDeserialization(new DDValue(new Guid[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationEmptyIntArray()
        {
            ValidateXMLDeserialization(new DDValue(new int[] { }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationIntArrayThreeElements()
        {
            ValidateXMLDeserialization(new DDValue(new[] { -1, 0, 1 }));
        }
        [TestMethod]
        public void TestDDValueXmlSerializationByteArrayThreeElements()
        {
            //var ddValueSe = new DDValue(new[] { "test1", "test2" });
            ValidateXMLDeserialization(new DDValue(new byte[] { 0x1, 0x2, 0x3 }));
        }




        private void ValidateXMLDeserialization(DDValue original)
        {

            var xml = XMLSerialyze(original);
            ValidateXMLDeserialization(original, xml);

        }

        private void ValidateXMLDeserialization(DDValue original, MemoryStream xml)
        {
            xml.Position = 0;
            var deserialyzed = XMLDeserialyze(xml);
            UTestDrDataCommon.WriteMemmoryStreamToXmlFile(xml);
            ValidateDeserialization(original, (DDValue)deserialyzed);
        }


        private MemoryStream XMLSerialyze(DDValue value)
        {

            var v = (DDValueSx)value;

            var ms = new MemoryStream();
            var s = new XmlSerializer(v.GetType());
            s.Serialize(ms, v);
            return ms;
        }

        private DDValueSx XMLDeserialyze(MemoryStream stream)
        {
            var serializer = new XmlSerializer(typeof(DDValueSx));
            return (DDValueSx)serializer.Deserialize(stream);
        }

        [TestMethod]
        public void TestDDValueXmlDirectSerialization()
        {
            var v = new DDValue("string");
            StringBuilder sb = new StringBuilder();
            v.Serialize(sb);
            var dv = DDValueSxe.Deserialize(sb.ToString());
            ValidateDeserialization(v, dv);
        }

        [TestMethod]
        public void TestDDValueArrayXmlDirectSerialization()
        {
            var v = new DDValue(new [] {1, 0, -1});
            StringBuilder sb = new StringBuilder();
            v.Serialize(sb);
            var dv = DDValueSxe.Deserialize(sb.ToString());
            ValidateDeserialization(v, dv);
        }

        #endregion IXmlSerializable

        #region ISerializable
        [TestMethod]
        public void TestDDValueISerializableNullData()
        {
            var dDValue = new DDValue();
            ValidateDeserialization(dDValue, new BinaryFormatter());
        }
        [TestMethod]
        public void TestDDValueISerializableBool()
        {
            var dDValue = new DDValue(false);
            ValidateDeserialization(dDValue, new BinaryFormatter());
        }
        [TestMethod]
        public void TestDDValueISerializableByteArray()
        {
            var dDValue = new DDValue(new byte[] { 0x1, 0x2, 0x3, 0x4, 0xff });
            ValidateDeserialization(dDValue, new BinaryFormatter());
        }
        [TestMethod]
        public void TestDDValueISerializableStringArray()
        {
            var dDValue = new DDValue(new string[] { "Test", string.Empty, "Юникод" });
            ValidateDeserialization(dDValue, new BinaryFormatter());
        }
        public static Stream SerializeItem(DDValue iSerializable, IFormatter formatter)
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

        private void ValidateDeserialization(DDValue original, IFormatter iFormatter)
        {
            var stream = SerializeItem(original, iFormatter);
            ValidateDeserialization(original, iFormatter, stream);

        }

        private void ValidateDeserialization(DDValue original, IFormatter iFormatter, Stream stream)
        {
            stream.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToBinFile((MemoryStream)stream);
            var deserialyzed = (DDValue)DeserializeItem(stream, iFormatter);
            ValidateDeserialization(original, deserialyzed);
        }

        private void ValidateDeserialization(DDValueSx original, DDValueSx deserialyzed)
        {
            ValidateDeserialization((DDValue)original, (DDValue)deserialyzed);
        }

        private void ValidateDeserialization(DDValue original, DDValue deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
            deserialyzed = true; // change type no true
            Assert.IsFalse(original == deserialyzed, "Changed deserialized object should not be equal to the original object.");
        }
        #endregion ISerializable
    }
}