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

        public const string aName1 = "value a->a";
        public static DateTime aValueDateTime1 = DateTime.Parse("2013-06-14T16:15:30+00Z");

        static private DDNode GetStockHierarhy()
        {


            var a = new DDNode("a", "Type.Root");
            a.Attributes.Add(aName1, "string");
            a.Attributes.Add("value a->b", true);
            var a_b = a.Add("a.b", "Type.Child");
            var a_c = a.Add("a.c", "Type.Child");
            a_c.Attributes.Add("value a.c->a", "string");
            a_c.Attributes.Add("value a.c->b", true);
            a_c.Attributes.Add("value a.c->c", aValueDateTime1);
            var a_b_d = a_b.Add("a.b.d");
            var a_b_d_e = a_b_d.Add("a.b.d.e");
            a_b_d_e.Attributes.Add("value a.b.d.e->a", 1);
            a_b_d_e.Attributes.Add("value a.b.d.e->b", null);
            return a;
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {

            var a = new DDNode("a");
            a.Attributes.Add("value a->a", new[] { true, false });

            var a_a = a.Add("a.a");
            a_a.Attributes.Add("value a.a->a", new[] { "stringA", "stringB"});
            a_a.Attributes.Add("value a.a->b", new[] { (byte)1, (byte)255, (byte)107});

            var a_a_a = a_a.Add("a.a.a");
            a_a_a.Attributes.Add("Value", new[] { 1, 0, -1, 1024, Int16.MaxValue, Int16.MinValue });

            var a_a_a_a = a_a_a.Add("a.a.a.a");
            a_a_a_a.Attributes.Add("Value", new bool[] { });

            var a_a_a_b = a_a_a.Add("a.a.a.b");
            a_a_a_b.Attributes.Add("Value", new bool[] { false });

            var a_a_a_c = a_a_a.Add("a.a.a.c");
            a_a_a_c.Attributes.Add("Value", new[] { Int32.MinValue, 0 , Int32.MaxValue });


            var a_b = a.Add("a.b");
            a_b.Attributes.Add("value a.b->a", new[] { true, false });
            a_b.Attributes.Add("value a.b->b", new[] { true, false });

            return a;
        }


        [TestMethod]
        public void TestMethod1()
        {
            var  sb = new StringBuilder();
            ((DDNodeSj)GetStockHierarhy()).Serialyze(sb);

        }
        [TestMethod]
        public void TestMethod2()
        {
            var sb = new StringBuilder();
            ((DDNodeSj)GetStockHierarhyWithArrayValue()).Serialyze(sb);

        }
    }
}
