using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSj;
using System.Text;

namespace UTestDrDataSj
{
    [TestClass]
    public class UTestDDNodeSj
    {

        private enum TEST_ENUM
        {
            TEST_ENUM_A,
            TEST_ENUM_a,
            TEST_ENUM_B,
            TEST_ENUM_NULL,
        }

        public const string aName1 = "prevValueString a->a";
        public static DateTime aValueDateTime1 = DateTime.Parse("2013-06-14T16:15:30+00Z");

        static private DDNode GetStockHierarhy()
        {


            var a = new DDNode("a", "Type.Root");
            a.Attributes.Add(aName1, "string");
            a.Attributes.Add("prevValueString a->b", true);
            var a_b = a.Add("a.b", "Type.Child");
            var a_c = a.Add("a.c", "Type.Child");
            a_c.Attributes.Add("prevValueString a.c->a", "string");
            a_c.Attributes.Add("prevValueString a.c->b", true);
            a_c.Attributes.Add("prevValueString a.c->c", aValueDateTime1);
            var a_b_d = a_b.Add("a.b.d");
            var a_b_d_e = a_b_d.Add("a.b.d.e");
            a_b_d_e.Attributes.Add("prevValueString a.b.d.e->a", 1);
            a_b_d_e.Attributes.Add("prevValueString a.b.d.e->b", null);
            return a;
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {

            var a = new DDNode("a");
            a.Attributes.Add("prevValueString a->a", new[] { true, false });

            var a_a = a.Add("a.a");
            a_a.Attributes.Add("prevValueString a.a->a", new[] { "stringA", "stringB" });
            a_a.Attributes.Add("prevValueString a.a->b", new[] { (byte)1, (byte)255, (byte)107 });

            var a_a_a = a_a.Add("a.a.a");
            a_a_a.Attributes.Add("Value", new[] { 1, 0, -1, 1024, Int16.MaxValue, Int16.MinValue });

            var a_a_a_a = a_a_a.Add("a.a.a.a");
            a_a_a_a.Attributes.Add("Value", new bool[] { });

            var a_a_a_b = a_a_a.Add("a.a.a.b");
            a_a_a_b.Attributes.Add("Value", new bool[] { false });

            var a_a_a_c = a_a_a.Add("a.a.a.c");
            a_a_a_c.Attributes.Add("Value", new[] { Int32.MinValue, 0, Int32.MaxValue });


            var a_b = a.Add("a.b");
            a_b.Attributes.Add("prevValueString a.b->a", new[] { true, false });
            a_b.Attributes.Add("prevValueString a.b->b", new[] { true, false });

            return a;
        }

        public static void ValidateDeserialization(DDNode original, DDNode deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
        }
        public static DDNode SerialyzeDeserialyzeValidate(DDNode n)
        {
            var sb = new StringBuilder();
            ((DDNodeSj)n).Serialyze(sb);

            var result = (DDNodeSj)new DDNode();

            var s = sb.ToString();
            result.Deserialyze(s);
            ValidateDeserialization(n, result);
            return result;
        }
        [TestMethod]
        public void TestDDNodeJsonSingleNode()
        {
            var res = SerialyzeDeserialyzeValidate(new DDNode());

        }
        [TestMethod]
        public void TestDDNodeJsonSingleNodeWithType()
        {
            var res = SerialyzeDeserialyzeValidate(new DDNode("NodeName", "NodeType"));
        }
        [TestMethod]
        public void TestDDNodeJsonNodeOneLevelChild()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Add("childA", "childTypeA");
            n.Add("childB");
            n.Add("childC", "childTypeC");
            var res = SerialyzeDeserialyzeValidate(n);
        }

        #region Node + Attributes + Value
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteNull()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(null);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteString()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add("It's string");
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteStringNull()
        {
            var n = new DDNode("NodeName", "NodeType");
            string str = null;
            n.Attributes.Add(str);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteBool()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(true);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteInt()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(11);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteFloat()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(float.MaxValue);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteDate()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(DateTime.Now);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonManualFormatSingleAttributteGUID()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add("a", new DDValue(new Guid("faf77826-1f05-42d2-880f-ca19fa1bc60e")));
            string j = "{\"NodeName\":{\"a\": [{\"a\": {\"v\":\"faf77826-1f05-42d2-880f-ca19fa1bc60e\",\"t\": \"System.Guid\"}}], \"t\":\"NodeType\"}}";
            var result = DDNodeSje.Deserialyze(j);
            ValidateDeserialization(n, result);
        }

        #endregion Node + Attributes + Value

        #region Node + Attribute + Value array

        [TestMethod]
        public void TestDDNodeJsonSingleAttributteStringArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new[] { "valueA", "valueB" });
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteBoolArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new[] { false, true, false});
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteEmptyBoolArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new bool[] { });
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteEmptyIntArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new int[] { });
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteIntArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new int[] { 1, 2 , 3 , 0});
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteFloatArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new float[] { float.MaxValue, 1, 2, 3, 0, float.MinValue, float.NaN });
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteEmptyStringArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new string[] { });
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteEmptyStringArray2()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(new string[] { "", null, String.Empty});
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonSingleAttributteNullStringArray()
        {
            string[] v = null;
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add(v);
            var res = SerialyzeDeserialyzeValidate(n);
        }
        [TestMethod]
        public void TestDDNodeJsonManualFormatSingleAttributteGUIDArray()
        {
            var n = new DDNode("NodeName", "NodeType");
            n.Attributes.Add("a", new DDValue(new [] { new Guid("faf77826-1f05-42d2-880f-ca19fa1bc60e"), new Guid("faf77826-1f05-42d2-880f-ca19fa1bc70e")}));
            string j = "{\"NodeName\":{\"a\": [{\"a\": {\"v\":[\"faf77826-1f05-42d2-880f-ca19fa1bc60e\",\"faf77826-1f05-42d2-880f-ca19fa1bc70e\"],\"t\": \"System.Guid[]\"}}], \"t\":\"NodeType\"}}";
            var result = DDNodeSje.Deserialyze(j);
            ValidateDeserialization(n, result);
        }
        #endregion Node + Attribute + Value array

        [TestMethod]
        public void TestMethod1()
        {
            var res = SerialyzeDeserialyzeValidate(GetStockHierarhy());
        }
        [TestMethod]
        public void TestMethod2()
        {
            var res = SerialyzeDeserialyzeValidate(GetStockHierarhyWithArrayValue());
        }
    }
}
