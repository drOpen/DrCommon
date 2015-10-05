using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrLinkedDictionary;

using System.Diagnostics;
using System.Collections.Generic;

namespace UTestDrLinkedDictionary
{
    // ToDo - need to overwrite the following tests -)
    [TestClass]
    public class UTestDrLinkedDictionary
    {

        #region TestData
        public static DrLinkedDictonary<int, string> GetStockDictonary(int iElements)
        {
            return GetStockDictonary(1, iElements);
        }

        public static DrLinkedDictonary<int, string> GetStockDictonary(int start, int iElements)
        {
            var d = new DrLinkedDictonary<int, string>();
            for (int i = start; i <= iElements; i++)
            {
                d.Add(i, i.ToString());
            }
            return d;
        }


        public static List<string> GetStockList(int iElements)
        {
            return GetStockList(1, iElements);
        }

        public static List<string> GetStockList(int start, int iElements)
        {
            var d = new List<string>();
            for (int i = start; i <= iElements; i++)
            {
                d.Add(i.ToString());
            }
            return d;
        }

        public void CompareKeyLinkedDictionary(DrLinkedDictonary<int, string> dic, List<string> list)
        {
            
            var i = (dic.EnumerationRules.Direction==EDirection.FORWARD ? 0 : list.Count-1);

            foreach (var item in dic)
            {
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", EDirection.FORWARD, item.Key, list[i]));
                if (dic.EnumerationRules.Direction == EDirection.FORWARD)
                    i++;
                else
                    i--;
            }
        }

        #endregion #region TestData

        [TestMethod]
        public void ContainsTest()
        {
            var d = GetStockDictonary(10);
            try
            {
                Debug.WriteLine("ContainsKey exists", (d.ContainsKey(5) == true) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsKey not exists", (d.ContainsKey(0) == false) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsValue not exists", (d.ContainsValue("0") == false) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsValue exists", (d.ContainsValue("6") == true) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsItem not exists", (d.Contains(new KeyValuePair<int, string>(5, "6")) == false) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsItem exists", (d.Contains(new KeyValuePair<int, string>(5, "5")) == true) ? "PASS" : "FAIL");
                Debug.WriteLine("ContainsItem exists", (d.Contains("ssd") == false) ? "PASS" : "FAIL");
            }
            catch
            {
                Debug.WriteLine("FAIL: ModifyDictionaryTest - got exception");
                throw;
            }
        }


        [TestMethod]
        public void TestForEachForStaticStockDictionary()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);

            CompareKeyLinkedDictionary(dic, list);
            dic.EnumerationRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, list);
        }
        [TestMethod]
        public void TestStartFromKeyStaticStockDictionary()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var listFirst = new List <string> {"1","2","3","4","5"};
            var listLast = new List<string> { "5", "6", "7", "8", "9", "10" };
            var list = GetStockList(elements);

            dic.EnumerationRules.StartFromKey = 5;
            CompareKeyLinkedDictionary(dic, listLast);
            dic.EnumerationRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, listFirst);

            dic.EnumerationRules.Reset();

            CompareKeyLinkedDictionary(dic, list);
            dic.EnumerationRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, list);


        }

        [TestMethod]
        public void TestEnumeratorWithModifyDirection()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);

            int i = 1;

            var enumerator = dic.GetEnumerator();

            while(enumerator.MoveNext())
            {
                Assert.AreEqual(enumerator.Current.Value, list[i - 1], false, "The forward direction doesn't work correctly.");
                i++;
            } 

            i = 10;
            dic.EnumerationRules.Direction = EDirection.BACKWARD;
            enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(enumerator.Current.Value, list[i - 1], false, "The backward direction doesn't work correctly.");
                i--;
            } 
        }

        [TestMethod]
        public void ModifyDictionaryTest()
        {
            var d = GetStockDictonary(4);
            try
            {
                Debug.WriteLine("Modifying dictionary:");
                Debug.WriteLine("InsertAsFirst(15, \"15\")");
                d.InsertAsFirst(15, "15");
                Debug.WriteLine("InsertAsLast(0, \"0\")");
                d.InsertAsLast(0, "0");
                Debug.WriteLine("InsertAfter(0, -1, \" - 1\")");
                d.InsertAfter(0, -1, "-1");
                Debug.WriteLine("InsertBefore(-1, -2, \" - 2\")");
                d.InsertBefore(-1, -2, "-2");
                Debug.WriteLine("Add(10, \"10\")");
                d.Add(10, "10");
                Debug.WriteLine("Remove(new KeyValuePair<int, string>(0, \"3\")");
                d.Remove(new KeyValuePair<int, string>(0, "3")); // has no effect
                Debug.WriteLine("Remove(3)");
                d.Remove(3);
                Debug.WriteLine("\n\nExpected order: 15, 1, 2, 4, 0, -2, -1, 10");
                var intendedResult = new int[] { 15, 1, 2, 4, 0, -2, -1, 10 };
                int i = 0;
                var res = false;
                Debug.Write("Actual order: ");
                foreach (var item in d)
                {
                    Debug.Write(item.ToString() + "   ");
                    if (item.Key != intendedResult[i])
                        break;
                    i++;
                    if (i == intendedResult.Length)
                    { res = true; Debug.Write("\n"); }
                }

                Debug.WriteLine("Insert test", res ? "PASS" : "FAIL");
            }
            catch
            {
                Debug.WriteLine("FAIL: ModifyDictionaryTest - got exception");
                throw;
            }
        }

        [TestMethod]
        public void EnumerationRulesTest()
        {
            var d = GetStockDictonary(5);
            try
            {
                Debug.Write("LinkedDictionary (also - forward default): ");
                foreach (var item in d)
                    Debug.Write(item.ToString() + " ");


                d.EnumerationRules.Direction = EDirection.BACKWARD;
                Debug.Write("\n\nBackward default: ");
                foreach (var item in d)
                    Debug.Write(item.ToString() + " ");

                d.EnumerationRules.StartFromKey = 3;
                Debug.Write("\nBackward from 3: ");
                foreach (var item in d)
                    Debug.Write(item.ToString() + " ");

                d.EnumerationRules.Reset();
                d.InsertAsFirst(0, "0");
                Debug.Write("\nForward default (0 as first): ");
                foreach (var item in d)
                    Debug.Write(item.ToString() + " ");
                Debug.WriteLine("\nPASS: EnumerationRulesTest");
            }
            catch
            {
                Debug.WriteLine("FAIL: EnumerationRulesTest - got exception");
                throw;
            }

        }
    }
}
