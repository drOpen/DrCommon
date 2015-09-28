using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrLinkedDictionary;

namespace UTestDrLinkedDictionary
{
    [TestClass]
    public class UTestDrLinkedDictionary
    {

        public static DrLinkedDictonary<int, string> GetStockDictonary(int iElements)
        {
            var d = new DrLinkedDictonary<int, string>();
            for (int i = 1; i <= iElements; i++)
            {
                d.Add(i, i.ToString());
            }
            return d;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var d = GetStockDictonary(10);

        }
    }
}
