using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSj;

namespace UTestDrDataSj
{
    /// <summary>
    /// Summary description for UTestDDValueSj
    /// </summary>
    [TestClass]
    public class UTestDDValueSj
    {
        public UTestDDValueSj()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestDDValueJsonDirectSerialization()
        {
            var v = new DDValue("string");
            StringBuilder sb = new StringBuilder();
            v.Serialize(sb);
            var dv = DDValueSje.Deserialize(sb.ToString());
            ValidateDeserialization(v, dv);
        }

        [TestMethod]
        public void TestDDValueArrayJsonDirectSerialization()
        {
            var v = new DDValue(new[] { 1, 0, -1 });
            StringBuilder sb = new StringBuilder();
            v.Serialize(sb);
            var dv = DDValueSje.Deserialize(sb.ToString());
            ValidateDeserialization(v, dv);
        }

        private void ValidateDeserialization(DDValue original, DDValue deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
            deserialyzed = true; // change type no true
            Assert.IsFalse(original == deserialyzed, "Changed deserialized object should not be equal to the original object.");
        }
    }
}
