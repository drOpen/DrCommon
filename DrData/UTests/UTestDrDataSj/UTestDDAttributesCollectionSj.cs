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
    /// Summary description for UTestDDattributesCollectionSj
    /// </summary>
    [TestClass]
    public class UTestDDAttributesCollectionSj
    {
        public UTestDDAttributesCollectionSj()
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
        public void TestDDAttributesCollectionJsonDirectSerialization()
        {
            var n = new DDNode("name", "type");
            n.Attributes.Add("bool", false);
            n.Attributes.Add("int", -1);
            n.Add("ChildNode").Add("SubChildNode").Attributes.Add("string", "string");
            StringBuilder sb = new StringBuilder();
            n.Attributes.Serialize(sb);
            var ac = DDAttributesCollectionSje.Deserialize(sb.ToString());
            ValidateDeserialization(n.Attributes, ac);
        }

        private void ValidateDeserialization(DDAttributesCollection original, DDAttributesCollection deserialized)
        {
            Assert.IsTrue(original == deserialized, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialized, "Deserialized object should not be same as original object.");
        }
    }
}
