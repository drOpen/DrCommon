using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrLog.DrLogClient;

namespace UTestsDrLogClient
{
    [TestClass]
    public class UTestLogger
    {
        #region Args2String
        [TestMethod]
        public void Args2StringNull()
        {
            var res = Logger.Args2String(null);
            var expected = "null";
            Assert.AreEqual(expected, res , String.Format("Incorrect result '{0}'. Expected '{1}'." , res, expected));
        }
        [TestMethod]
        public void Args2StringEmpty()
        {
            var res = Logger.Args2String();
            var expected = "empty";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        [TestMethod]
        public void Args2StringEmptyArgs1()
        {
            var res = Logger.Args2String(new string[] { });
            var expected = "empty";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        [TestMethod]
        public void Args2StringEmptyArgs2()
        {
            var res = Logger.Args2String(new string[] {  });
            var expected = "empty";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        [TestMethod]
        public void Args2StringNullArgs()
        {
            var res = Logger.Args2String(new string[] {null });
            var expected = "'null'";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        [TestMethod]
        public void Args2StringNullEmptyArgs()
        {
            var res = Logger.Args2String(new string[] { null, "", null, "", String.Empty  });
            var expected = "'null', 'empty', 'null', 'empty', 'empty'";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        [TestMethod]
        public void Args2String()
        {
            var res = Logger.Args2String("Test Юникод");
            var expected = "'Test Юникод'";
            Assert.AreEqual(expected, res, String.Format("Incorrect result '{0}'. Expected '{1}'.", res, expected));
        }
        #endregion Args2String
    }
}
