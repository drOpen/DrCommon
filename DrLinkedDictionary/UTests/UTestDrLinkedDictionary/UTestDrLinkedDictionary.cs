using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrLinkedDictionary;

using System.Diagnostics;
using System.Collections.Generic;

namespace UTestDrLinkedDictionary
{
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
            this.CompareKeyLinkedDictionary(dic, list, new DrLinkedDictonary<int, string>.DrEnumerationRules());
        }

        public void CompareKeyLinkedDictionary(DrLinkedDictonary<int, string> dic, List<string> list, DrLinkedDictonary<int, string>.DrEnumerationRules eRules)
        {
            var i = (eRules.Direction == EDirection.FORWARD ? 0 : list.Count - 1);
            foreach (var item in dic.GetEnumerator(eRules))
            {
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", eRules.Direction.ToString(), item.Key, list[i]));
                if (eRules.Direction == EDirection.FORWARD)
                    i++;
                else
                    i--;
            }
        }

        #endregion #region TestData

        #region Test Enumeration
        [TestMethod]
        public void TestRulesCloneChangeDirection()
        {

            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);


            var i = 0;
            var eRules = dic.GetDrEnumerationRules(EDirection.FORWARD);

            foreach (var item in (dic.GetEnumerator(eRules)))
            {
                Debug.Print(string.Format("Key: '{0}', Value: '{1}'.",  item.Key , item.Value));
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", EDirection.FORWARD.ToString(), item.Key, list[i]));
                eRules.Direction = EDirection.BACKWARD; // current foreach is not changed
                i++;
            }

            i = list.Count - 1;
            foreach (var item in (dic.GetEnumerator(eRules)))
            {
                Debug.Print(string.Format("Key: '{0}', Value: '{1}'.", item.Key, item.Value));
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", EDirection.BACKWARD.ToString(), item.Key, list[i]));
                eRules.Direction = EDirection.FORWARD; // current foreach is not changed
                i--;
            }
        }

        [TestMethod]
        public void TestRulesCloneChangeStartFrom()
        {

            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);

            var startFrom = 5;
            var i = 4;
            var eRules = dic.GetDrEnumerationRules(startFrom);
            
            foreach (var item in (dic.GetEnumerator(eRules)))
            {
                Debug.Print(string.Format("Key: '{0}', Value: '{1}'.", item.Key, item.Value));
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", EDirection.FORWARD.ToString(), item.Key, list[i]));
                eRules.StartFromKey = startFrom; // current foreach is not changed
                i++;
            }

            eRules.Direction = EDirection.BACKWARD;
            i = startFrom-1;

            foreach (var item in (dic.GetEnumerator(eRules)))
            {
                Debug.Print(string.Format("Key: '{0}', Value: '{1}'.", item.Key, item.Value));
                Assert.AreEqual(item.Key.ToString(), list[i], false, string.Format("The '{0}' direction doesn't work correctly item '{1}' is not equals '{2}'.", EDirection.BACKWARD.ToString(), item.Key, list[i]));
                eRules.Direction = EDirection.FORWARD; // current foreach is not changed
                eRules.StartFromKey = startFrom; // current foreach is not changed
                i--;
            }
        }
        #endregion Test Enumeration

        [TestMethod]
        public void TestForEachForStaticStockDictionary()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);

            CompareKeyLinkedDictionary(dic, list);
            var eRules = dic.GetDrEnumerationRules();
            eRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, list);
            eRules.Direction = EDirection.FORWARD;
            CompareKeyLinkedDictionary(dic, list);
        }
        [TestMethod]
        public void TestStartFromKeyStaticStockDictionary()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var listFirst = new List<string> { "1", "2", "3", "4", "5" };
            var listLast = new List<string> { "5", "6", "7", "8", "9", "10" };
            var list = GetStockList(elements);

            var eRules = dic.GetDrEnumerationRules(5);

            CompareKeyLinkedDictionary(dic, listLast, eRules);
            eRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, listFirst, eRules);

            eRules.Reset();

            CompareKeyLinkedDictionary(dic, list, eRules);
            eRules.Direction = EDirection.BACKWARD;
            CompareKeyLinkedDictionary(dic, list, eRules);
        }

        [TestMethod]
        public void TestEnumeratorWithModifyDirection()
        {
            var elements = 10;
            var dic = GetStockDictonary(elements);
            var list = GetStockList(elements);

            int i = 1;

            var eRules = dic.GetDrEnumerationRules();
            var enumerator = dic.GetEnumerator(eRules);

            while (enumerator.MoveNext())
            {

                Assert.AreEqual(enumerator.Current.Value, list[i - 1],  "The forward direction doesn't work correctly.");
                i++;
            }

            i = 10;
            eRules.Direction = EDirection.BACKWARD;
            enumerator = dic.GetEnumerator(eRules);
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

    }
}
