﻿/*
  UTestDDAttributesCollection.cs -- Unit Tests of DDAttributesCollection for 'DrData' general purpose Data abstraction layer 1.0.1, January 5, 2014
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation would be
      appreciated but is not required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>

 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using DrOpen.DrCommon.DrData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;

namespace UTestDrData
{

    /// <summary>
    /// Unit tests for collection of DDAttributes that can be accessed by expected or index.
    /// </summary>
    [TestClass]
    public class UTestDDAttributesCollection
    {
        private enum TEST_ENUM
        {
            TEST_ENUM_A,
            TEST_ENUM_a,
            TEST_ENUM_B,
            TEST_ENUM_NULL,
            TEST_ENUM_DONT_ADD_WITH_NAME
        }
        private DDAttributesCollection GetStockAttributesCollection()
        {
            var attrs = new DDAttributesCollection();
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"), ResolveConflict.THROW_EXCEPTION) == TEST_ENUM.TEST_ENUM_A.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_B, new DDValue("B"), ResolveConflict.OVERWRITE) == TEST_ENUM.TEST_ENUM_B.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_a, new DDValue("a"), ResolveConflict.OVERWRITE) == TEST_ENUM.TEST_ENUM_a.ToString(), "Incorrect attribute name.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("Skipped"), ResolveConflict.SKIP) == null, "Skipped attribute name should be null.");
            Assert.IsTrue(attrs.Add(TEST_ENUM.TEST_ENUM_NULL, null) == TEST_ENUM.TEST_ENUM_NULL.ToString(), "Incorrect attribute value.");

            return attrs;
        }
        #region Add
        [TestMethod]
        public void TestAddNewEmptyValueAutoGeneratedName()
        {
            var attrs = new DDAttributesCollection();

            var name = attrs.Add(new DDValue());
            Assert.IsTrue(attrs.Count == 1, "Collection items count is incorrect.");
            Assert.IsTrue(attrs.Contains(name), "Contains for autogenerated name is incorrected.");


        }
        [TestMethod]
        public void TestAddNewValueWithName1()
        {
            var attrs = new DDAttributesCollection();
            var nameValue = "Юникод Name";
            var nameReturn = attrs.Add(nameValue, new DDValue());
            Assert.IsTrue(attrs.Count == 1, "Collection items count is incorrect.");
            Assert.IsTrue(attrs.Contains(nameReturn), "Contains for autogenerated name is incorrected.");
            Assert.IsTrue(nameReturn == nameValue, "Added name and returned must be same.");

        }
        [TestMethod]
        public void TestAddNewValueWithName2()
        {
            var attrs = new DDAttributesCollection();

            var nameReturn_a = attrs.Add(TEST_ENUM.TEST_ENUM_a, new DDValue("a"));
            var nameReturn_A = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"));
            Assert.IsTrue(attrs.Count == 2, "Collection items count is incorrect.");
            Assert.IsTrue(attrs.Contains(nameReturn_a), "Contains for autogenerated name is incorrected.");
            Assert.IsTrue(attrs.Contains(nameReturn_A), "Contains for autogenerated name is incorrected.");
            Assert.IsTrue(nameReturn_a == TEST_ENUM.TEST_ENUM_a.ToString(), "Added name and returned must be same.");
            Assert.IsTrue(nameReturn_A == TEST_ENUM.TEST_ENUM_A.ToString(), "Added name and returned must be same.");
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_a] == "a", "The added value is incorrected.");
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "A", "The added value is incorrected.");
        }


        [TestMethod]
        public void TestAddNewValueTestUniqNameException1()
        {
            var attrs = new DDAttributesCollection();
            try
            {
                var nameReturn_A1 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"));
                var nameReturn_A2 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("B"));

                Assert.Fail("Add new value with not uniq name!");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "A", "Cannot find first value by name.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Assert.Fail("Catch incorrect exception after attempt to add new value with not uniq name!" + e.Message);
            }
        }


        [TestMethod]
        public void TestAddNewValueTestUniqNameException2()
        {
            var attrs = new DDAttributesCollection();
            try
            {
                var nameReturn_A1 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"), ResolveConflict.SKIP);
                var nameReturn_A2 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("B"), ResolveConflict.THROW_EXCEPTION);

                Assert.Fail("Add new value with not uniq name!");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "A", "Cannot find first value by name.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Assert.Fail("Catch incorrect exception after attempt to add new value with not uniq name!" + e.Message);
            }
        }

        [TestMethod]
        public void TestAddNewValueTestUniqNameSkip()
        {
            var attrs = new DDAttributesCollection();
            try
            {
                var nameReturn_A1 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"), ResolveConflict.THROW_EXCEPTION);
                var nameReturn_A2 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("B"), ResolveConflict.SKIP);
                Assert.IsTrue(nameReturn_A2 == null, "Existing value should not be overwritten and return value should be equal null!");
                Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "A", "Existing value should not be overwritten!");
            }
            catch (ArgumentException)
            {
                Assert.Fail("Incorrect ArgumentException, new value should be skipped!");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {
                Assert.Fail("Incorrect Exception, new value should be skipped!");
            }
        }

        [TestMethod]
        public void TestAddNewValueTestUniqNameOverWrite()
        {
            var attrs = new DDAttributesCollection();
            try
            {
                var nameReturn_A1 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("A"), ResolveConflict.THROW_EXCEPTION);
                var nameReturn_A2 = attrs.Add(TEST_ENUM.TEST_ENUM_A, new DDValue("B"), ResolveConflict.OVERWRITE);
                Assert.IsTrue(nameReturn_A2 == TEST_ENUM.TEST_ENUM_A.ToString(), "Existing value should be overwritten and return value should be equal name.");
                Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "B", "Existing value should be overwritten!");
            }
            catch (ArgumentException)
            {
                Assert.Fail("Incorrect ArgumentException, new value should be skipped!");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {
                Assert.Fail("Incorrect Exception, new value should be skipped!");
            }
        }
        #endregion Add
        #region UTests Get/Set

        [TestMethod]
        public void TestGetItemCaseSensitive()
        {
            var attrs = GetStockAttributesCollection();
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == "A", "Incorrect value.");
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_B] == "B", "Incorrect value.");
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_a] == "a", "Incorrect value.");
            try
            {
                var res = attrs[TEST_ENUM.TEST_ENUM_B.ToString().ToLower()];
                Assert.Fail("Where my exception, dude?");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {
                // it's ok
            }
        }

        private void ValidateAttribute(KeyValuePair<string, DDValue> attr, Enum expectedKey, DDValue expectedValue)
        {
            ValidateAttribute(attr, expectedKey.ToString(), expectedValue);
        }

        private void ValidateAttribute(KeyValuePair<string, DDValue> attr, string expectedKey, DDValue expectedValue)
        {

            Assert.IsTrue(attr.Key == expectedKey, "The key is incorrect.");
            Assert.IsTrue(attr.Value == expectedValue, "The value is incorrect.");

        }
        #endregion UTests Get/Set
        #region TryGetValue()
        [TestMethod]
        public void TestTryGetValue()
        {
            var attrs = GetStockAttributesCollection();
            DDValue value;
            Assert.IsTrue(attrs.TryGetValue(TEST_ENUM.TEST_ENUM_A, out value), "Cannot get exist value by Enum.");
            Assert.IsTrue(value == "A", "Value is incorrect.");

            Assert.IsTrue(attrs.TryGetValue(TEST_ENUM.TEST_ENUM_a.ToString(), out value), "Cannot get exist value by String.");
            Assert.IsTrue(value == "a", "Value is incorrect.");

            value = null;

            Assert.IsFalse(attrs.TryGetValue("", out value), "Found ghost value -(.");
            Assert.IsTrue(value == null, "Value should be null.");

            Assert.IsFalse(attrs.TryGetValue(TEST_ENUM.TEST_ENUM_DONT_ADD_WITH_NAME, out value), "Found ghost value -(.");
            Assert.IsTrue(value == null, "Value should be null.");

        }
        #endregion TryGetValue()
        #region Clear()
        [TestMethod]
        public void TestClear()
        {
            var attrs = GetStockAttributesCollection();

            Assert.IsTrue(attrs.Count > 0, "Incorrect collection count.");
            attrs.Clear();
            Assert.IsTrue(attrs.Count == 0, "After Clear() collection should be Empty.");
            foreach (var attr in attrs)
            {
                Assert.Fail("Attributes collection isn't empty.");
            }

        }
        #endregion Clear()
        #region Contains()
        [TestMethod]
        public void TestContains()
        {
            var attrs = GetStockAttributesCollection();

            Assert.IsTrue(attrs.Contains(TEST_ENUM.TEST_ENUM_A), "Contains() should be return True for Exist item.");
            Assert.IsTrue(attrs.Contains(TEST_ENUM.TEST_ENUM_a), "Contains() should be return True for Exist item.");
            Assert.IsTrue(attrs.Contains(TEST_ENUM.TEST_ENUM_B), "Contains() should be return True for Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_DONT_ADD_WITH_NAME), "Contains() should be return False for none Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_B.ToString().ToLower()), "Contains() should be return False for none Exist item.");
        }
        #endregion Contains()
        #region ContainsValue()
        [TestMethod]
        public void TestContainsValue()
        {
            var attrs = GetStockAttributesCollection();
            attrs.Add(new DDValue(null));

            Assert.IsTrue(attrs.ContainsValue(new DDValue("A")), "ContainsValue() should be return True for Exist item.");
            Assert.IsTrue(attrs.ContainsValue(new DDValue("B")), "ContainsValue() should be return True for Exist item.");
            Assert.IsTrue(attrs.ContainsValue(new DDValue("a")), "ContainsValue() should be return True for Exist item.");
            Assert.IsFalse(attrs.ContainsValue(new DDValue("skipped")), "ContainsValue() should be return False for none Exist item.");
            Assert.IsTrue(attrs.ContainsValue(null), "ContainsValue() should be return True for Exist item.");
            Assert.IsTrue(attrs.ContainsValue(new DDValue(null)), "ContainsValue() should be return True for Exist item.");
        }
        #endregion ContainsValue()
        #region Replace()
        [TestMethod]
        public void TestReplaceExistItem()
        {
            var attrs = GetStockAttributesCollection();
            int expectedValue = 123;

            Assert.IsTrue(attrs.Contains(TEST_ENUM.TEST_ENUM_A), "Contains() should be return True for Exist item.");

            attrs.Replace(TEST_ENUM.TEST_ENUM_A, expectedValue);

            Assert.IsTrue(attrs.Contains(TEST_ENUM.TEST_ENUM_A), "Contains() should be return True for Exist item.");
            Assert.IsTrue(attrs[TEST_ENUM.TEST_ENUM_A] == expectedValue, "Expected new value.");

        }
        [TestMethod]
        public void TestReplaceNoneExistItem()
        {
            var attrs = GetStockAttributesCollection();
            int expectedValue = 123;
            string name = "Test";
            Assert.IsFalse(attrs.Contains(name), "Contains() should be return false for none Exist item.");

            attrs.Replace(name, expectedValue);

            Assert.IsTrue(attrs.Contains(name), "Contains() should be return True for Exist item.");
            Assert.IsTrue(attrs[name] == expectedValue, "Expected new value.");
        }
        #endregion Replace()
        #region Remove()
        [TestMethod]
        public void TestRemoveExistItem()
        {
            var attrs = GetStockAttributesCollection();
            Assert.IsTrue(attrs.Remove(TEST_ENUM.TEST_ENUM_A), "Remove() should be return true for Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_A), "Contains() should be return False for none Exist item.");

            Assert.IsTrue(attrs.Remove(TEST_ENUM.TEST_ENUM_B.ToString()), "Remove() should be return true for Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_B.ToString()), "Contains() should be return False for none Exist item.");

        }
        [TestMethod]
        public void TestRemoveNoneExistItem()
        {
            var attrs = GetStockAttributesCollection();
            Assert.IsTrue(attrs.Remove(TEST_ENUM.TEST_ENUM_A), "Remove() should be return true for Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_A), "Contains() should be return False for none Exist item.");
            Assert.IsFalse(attrs.Remove(TEST_ENUM.TEST_ENUM_A), "Remove() should be return False for none Exist item.");

            Assert.IsTrue(attrs.Remove(TEST_ENUM.TEST_ENUM_B.ToString()), "Remove() should be return true for Exist item.");
            Assert.IsFalse(attrs.Contains(TEST_ENUM.TEST_ENUM_B.ToString()), "Contains() should be return False for none Exist item.");
            Assert.IsFalse(attrs.Remove(TEST_ENUM.TEST_ENUM_B.ToString()), "Remove() should be return False for none Exist item.");

            Assert.IsFalse(attrs.Remove("Bazinga"), "Remove() should be return False for none Exist item.");

        }
        #endregion Remove()
        #region GetValue()
        [TestMethod]
        public void TestGetValueByEnum()
        {
            var attrs = GetStockAttributesCollection();
            Assert.IsTrue(attrs.GetValue(TEST_ENUM.TEST_ENUM_A, null) == "A", "GetValue() should be return Exist value.");
            Assert.IsTrue(attrs.GetValue(TEST_ENUM.TEST_ENUM_a, null) == "a", "GetValue() should be return Exist value.");

            Assert.IsTrue(attrs.GetValue(TEST_ENUM.TEST_ENUM_DONT_ADD_WITH_NAME, null) == null, "GetValue() should be return DefaultValue for none Exist value.");
        }
        [TestMethod]
        public void TestGetValueByString()
        {
            var attrs = GetStockAttributesCollection();
            int expectedInt = 123;
            bool expectedBool = true;
            attrs.Add("Int", expectedInt);
            attrs.Add("Bool", true);
            Assert.IsTrue(attrs.GetValue("Int", 0) == expectedInt, "GetValue() should be return Exist value.");
            Assert.IsTrue(attrs.GetValue("Bool", false) == expectedBool, "GetValue() should be return Exist value.");
            Assert.IsTrue(attrs.GetValue("INT", 0) == 0, "GetValue() should be return Exist value.");
            Assert.IsTrue(attrs.GetValue("", false) == false, "GetValue() should be return DefaultValue for none Exist value.");
        }
        [TestMethod]
        public void TestGetValueByEnumDefaultvalueIsDDvalue()
        {
            var attrs = GetStockAttributesCollection();
            Assert.IsTrue(attrs.GetValue("None Exist", new DDValue(DateTime.MinValue)) == (DDValue)DateTime.MinValue, "GetValue() should be return default value for none exist name.");
        }
        #endregion GetValue()
        #region Clone()
        [TestMethod]
        public void TestClone()
        {
            var attrsOriginal = GetStockAttributesCollection();
            var attrsClone = attrsOriginal.Clone();

            foreach (var a in attrsOriginal)
            {
                var aOriginal = attrsOriginal[a.Key];
                if (aOriginal == null) continue; // skip null 
                Assert.IsTrue(aOriginal.Equals(a.Value), "The linked items should be Equals()");
                var aClone = attrsClone[a.Key];
                Assert.IsFalse(aClone.Equals(a.Value), "The cloned items should not be Equals()");
                Assert.IsFalse(aClone.Equals(aOriginal), "The cloned items should not be Equals()");
                Assert.IsTrue(aOriginal == aClone, "The linked items should be have some type and value.");
                Assert.IsTrue(aOriginal.GetValueAsString() == aClone.GetValueAsString(), "The linked items should be return some value.");
            }
        }
        [TestMethod]
        public void TestCloneByInterfaceICloneable()
        {
            ICloneable attrsOriginal = GetStockAttributesCollection();
            var attrsClone = attrsOriginal.Clone();

            foreach (var a in (DDAttributesCollection)attrsOriginal)
            {
                var aOriginal = ((DDAttributesCollection)attrsOriginal)[a.Key];
                if (aOriginal == null) continue; // skip null 
                Assert.IsTrue(aOriginal.Equals(a.Value), "The linked items should be Equals()");
                var aClone = ((DDAttributesCollection)attrsClone)[a.Key];
                Assert.IsFalse(aClone.Equals(a.Value), "The cloned items should not be Equals()");
                Assert.IsFalse(aClone.Equals(aOriginal), "The cloned items should not be Equals()");
                Assert.IsTrue(aOriginal == aClone, "The linked items should be have some type and value.");
                Assert.IsTrue(aOriginal.GetValueAsString() == aClone.GetValueAsString(), "The linked items should be return some value.");
            }
        }
        #endregion Clone()
        #region UTest ForEach
        [TestMethod]
        public void TestForEach()
        {
            var attrs = GetStockAttributesCollection();
            int i = 0;
            foreach (var attr in attrs)
            {
                i++;
                switch (i)
                {
                    case 1:
                        ValidateAttribute(attr, TEST_ENUM.TEST_ENUM_A, "A");
                        break;
                    case 2:
                        ValidateAttribute(attr, TEST_ENUM.TEST_ENUM_B, "B");
                        break;

                    case 3:
                        ValidateAttribute(attr, TEST_ENUM.TEST_ENUM_a, "a");
                        break;

                    case 4:
                        ValidateAttribute(attr, TEST_ENUM.TEST_ENUM_NULL, null);
                        break;
                    default:
                        Assert.Fail("Additional attribute item?");
                        break;
                }

            }
        }

        [TestMethod]
        public void TestForEach2()
        {
            IEnumerable attrs = GetStockAttributesCollection();
            int i = 0;
            foreach (var attr in attrs)
            {
                i++;
                switch (i)
                {
                    case 1:
                        ValidateAttribute((KeyValuePair<string, DDValue>)attr, TEST_ENUM.TEST_ENUM_A, "A");
                        break;
                    case 2:
                        ValidateAttribute((KeyValuePair<string, DDValue>)attr, TEST_ENUM.TEST_ENUM_B, "B");
                        break;

                    case 3:
                        ValidateAttribute((KeyValuePair<string, DDValue>)attr, TEST_ENUM.TEST_ENUM_a, "a");
                        break;
                    case 4:
                        ValidateAttribute((KeyValuePair<string, DDValue>)attr, TEST_ENUM.TEST_ENUM_NULL, null);
                        break;
                    default:
                        Assert.Fail("Additional attribute item?");
                        break;
                }

            }
        }


        [TestMethod]
        public void TestForEachKeys()
        {
            var attrs = GetStockAttributesCollection();
            int i = 0;
            foreach (var name in attrs.Names)
            {
                i++;
                switch (i)
                {
                    case 1:
                        Assert.IsTrue(name == TEST_ENUM.TEST_ENUM_A.ToString(), "The name is incorrect.");
                        break;
                    case 2:
                        Assert.IsTrue(name == TEST_ENUM.TEST_ENUM_B.ToString(), "The name is incorrect.");
                        break;

                    case 3:
                        Assert.IsTrue(name == TEST_ENUM.TEST_ENUM_a.ToString(), "The name is incorrect.");
                        break;
                    case 4:
                        Assert.IsTrue(name == TEST_ENUM.TEST_ENUM_NULL.ToString(), "The name is incorrect.");
                        break;
                    default:
                        Assert.Fail("Additional attribute item?");
                        break;
                }

            }
        }

        [TestMethod]
        public void TestForEachValues()
        {
            var attrs = GetStockAttributesCollection();
            int i = 0;
            foreach (var value in attrs.Values)
            {
                i++;
                switch (i)
                {
                    case 1:
                        Assert.IsTrue(value == "A", "The value is incorrect.");
                        break;
                    case 2:
                        Assert.IsTrue(value == "B", "The value is incorrect.");
                        break;

                    case 3:
                        Assert.IsTrue(value == "a", "The value is incorrect.");
                        break;
                    case 4:
                        Assert.IsTrue(value == null, "The value is incorrect.");
                        break;
                    default:
                        Assert.Fail("Additional attribute item?");
                        break;
                }

            }
        }

        #endregion Foreach Items, Names, Values
        #region CompareTo
        [TestMethod]
        public void TestCompareToIncorrectTypeBool()
        {
            var a = GetStockAttributesCollection();
            var b = true;
            Assert.IsFalse(a.CompareTo(b) == 0, "Objects of different types may not be the same");
        }
        [TestMethod]
        public void TestCompareTo()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            Assert.IsTrue(a.CompareTo(b) == 0, "Compare the same objects should be return 0.");
        }
        #endregion CompareTo
        #region Compare
        [TestMethod]
        public void TestCompareBothNull()
        {
            Assert.IsTrue(DDAttributesCollection.Compare( null, null)==0, "The null objects should be equals.");
        }
        [TestMethod]
        public void TestCompareOneOfNull()
        {
            Assert.IsFalse(DDAttributesCollection.Compare(null, GetStockAttributesCollection()) == 0, "The object should not be equal null.");
        }
        [TestMethod]
        public void TestCompareNull()
        {
            Assert.IsFalse(GetStockAttributesCollection() == null, "The object should not be equal null.");
        }
        [TestMethod]
        public void TestCompareEqualObject()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            Assert.IsTrue(a == b, "The objects should be equals.");
        }
        [TestMethod]
        public void TestCompareEqualObjectAfterModifyToOriginalStatus()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            b.Add(TEST_ENUM.TEST_ENUM_A, "A", ResolveConflict.OVERWRITE);
            Assert.IsTrue(a == b, "The objects should be equals.");
        }
        [TestMethod]
        public void TestCompareDifferentsItemsCount()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            b.Remove(TEST_ENUM.TEST_ENUM_A);
            Assert.IsFalse(a == b, "The objects with different items count should not be equals.");
        }
        [TestMethod]
        public void TestCompareDifferentsItemsKey()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            a.Add("A", "A", ResolveConflict.OVERWRITE);
            b.Add("B", "B", ResolveConflict.OVERWRITE);
            Assert.IsFalse(a == b, "The objects with different items should not be equals.");
        }
        [TestMethod]
        public void TestCompareDifferentsItemsValueNull()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            b.Add(TEST_ENUM.TEST_ENUM_A, null, ResolveConflict.OVERWRITE);
            Assert.IsFalse(a == b, "The objects with different items should not be equals.");
        }
        [TestMethod]
        public void TestCompareDifferentsItemsValueString()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            b.Add(TEST_ENUM.TEST_ENUM_A, "a", ResolveConflict.OVERWRITE);
            Assert.IsFalse(a == b, "The objects with different items should not be equals.");
        }
        #endregion Compare
        #region Equals
        [TestMethod]
        public void TestEqualsObjectNull()
        {
            var a = GetStockAttributesCollection();
            Assert.IsFalse(a.Equals(null), "The object should not be equal null.");
        }
        [TestMethod]
        public void TestEqualsObjectAnotherType()
        {
            var a = GetStockAttributesCollection();
            Assert.IsFalse(a.Equals(true), "The object should not be equal bool.");
        }
        [TestMethod]
        public void TestEqualsObjectWithDifferentData()
        {
            var a = GetStockAttributesCollection();
            var b = GetStockAttributesCollection();
            b.Remove(TEST_ENUM.TEST_ENUM_A);
            Assert.IsFalse(a.Equals(b), "The object should not be equals another object with different data set.");
        }
        [TestMethod]
        public void TestEqualsSelfIt()
        {
            var a = GetStockAttributesCollection();
            Assert.IsTrue(a.Equals(a), "object must be equal to itself.");
        }
        #endregion Equals
        #region GetHashCode
        #endregion GetHashCode
        #region ISerializable
        [TestMethod]
        public void TestDDAttributesCollectionISerializableNullData()
        {
            var ddAttributesCollection = new DDAttributesCollection();
            ValidateDeserialization(ddAttributesCollection, new BinaryFormatter());
        }
        [TestMethod]
        public void TestDDAttributesCollectionISerializableData()
        {
            var ddAttributesCollection = GetStockAttributesCollection();
            ValidateDeserialization(ddAttributesCollection, new BinaryFormatter());
        }
        public static Stream SerializeItem(DDAttributesCollection iSerializable, IFormatter formatter)
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

        private void ValidateDeserialization(DDAttributesCollection original, IFormatter iFormatter)
        {
            var stream = SerializeItem(original, iFormatter);
            ValidateDeserialization(original, iFormatter, stream);

        }

        private void ValidateDeserialization(DDAttributesCollection original, IFormatter iFormatter, Stream stream)
        {
            stream.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToBinFile((MemoryStream)stream);
            var deserialyzed = (DDAttributesCollection)DeserializeItem(stream, iFormatter);

            ValidateDeserialization(original, deserialyzed);
        }

        private void ValidateDeserialization(DDAttributesCollection original, DDAttributesCollection deserialyzed)
        {
            Assert.IsTrue(original == deserialyzed, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialyzed, "Deserialized object should not be same as original object.");
        }
        #endregion ISerializable

        #region IXmlSerializable

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationGetSchemaNull()
        {
            var a = GetStockAttributesCollection();
            Assert.IsNull(a.GetSchema(), "XML schema should be null.");
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationAttributeStrongName1()
        {
            var a = GetStockAttributesCollection();
            a.Add(UTestDrDataCommon.ElementNameStrong1, new DDValue());
            ValidateXMLDeserialization(a);
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationEmpty()
        {
            ValidateXMLDeserialization(new DDAttributesCollection());
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationFromFileEmpty()
        {
            ValidateXMLDeserialization(new DDAttributesCollection(), UTestDrDataCommon.GetMemoryStreamFromFile());
        }

        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationFromFileSkipIncorrectValue()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, null);
            a.Add("A", null);
            a.Add("B", new DDValue(new string[]{"", "Value_A", "Value_B"}));
            ValidateXMLDeserialization(a, UTestDrDataCommon.GetMemoryStreamFromFile());
        }



        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationEmptyName()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, "Empty");
            ValidateXMLDeserialization(a);
        }



        [TestMethod]
        public void TestDDAttributesCollectionXmlSerializationNullValue()
        {
            var a = new DDAttributesCollection();
            a.Add(string.Empty, null);
            ValidateXMLDeserialization(a);
        }

        private void ValidateXMLDeserialization(DDAttributesCollection original)
        {
            var xml = XMLSerialyze(original);
            ValidateXMLDeserialization(original, xml);
        }

        private void ValidateXMLDeserialization(DDAttributesCollection original, MemoryStream xml)
        {
            xml.Position = 0;
            UTestDrDataCommon.WriteMemmoryStreamToXmlFile(xml);
            var deserialyzed = XMLDeserialyze(xml);
            ValidateDeserialization(original, deserialyzed);

        }

        private MemoryStream XMLSerialyze(DDAttributesCollection value)
        {
            var memoryStream = new MemoryStream();
            var serializer = new XmlSerializer(value.GetType());
            serializer.Serialize(memoryStream, value);
            return memoryStream;

        }

        private DDAttributesCollection XMLDeserialyze(MemoryStream stream)
        {
            var serializer = new XmlSerializer(typeof(DDAttributesCollection));
            return (DDAttributesCollection)serializer.Deserialize(stream);
        }


        #endregion IXmlSerializable


        private List<EventArgs> doLogInEventArgs;
        //#region DoLogIn
        //[TestMethod]
        //public void TestDoLogIn()
        //{
        //    doLogInEventArgs = new List<EventArgs>();
        //    var attrsOriginal = GetStockAttributesCollection();
        //    attrsOriginal.DoLogIn +=(EventCatcher);
        //    attrsOriginal.Add(TEST_ENUM.TEST_ENUM_A, "BB", ResolveConflict.OVERWRITE);
        //    attrsOriginal.Add(TEST_ENUM.TEST_ENUM_B, "CC", ResolveConflict.SKIP);
        //    attrsOriginal.Add("New Item", "New", ResolveConflict.SKIP);
        //    attrsOriginal.DoLogIn -= (EventCatcher);
        //    attrsOriginal.Add(TEST_ENUM.TEST_ENUM_A, "EE", ResolveConflict.OVERWRITE);

        //    Assert.IsTrue(doLogInEventArgs.Count==2, "There are not 2 events in the event list.");
        //    var eventDoLog= (LogInEventArgs)doLogInEventArgs[0];
        //    Assert.IsTrue(eventDoLog.Exception == null, "Exception in the event should be null.");
        //    Assert.IsTrue(eventDoLog.Args[0].ToString() == TEST_ENUM.TEST_ENUM_A.ToString(), "The first argument should be expected.");
        //    Assert.IsTrue(eventDoLog.Args[1].ToString() == "BB", "The second argument should be value.");

        //    eventDoLog = (LogInEventArgs)doLogInEventArgs[1];
        //    Assert.IsTrue(eventDoLog.Exception == null, "Exception in the event should be null.");
        //    Assert.IsTrue(eventDoLog.Args[0].ToString() == TEST_ENUM.TEST_ENUM_B.ToString(), "The first argument should be expected.");
        //    Assert.IsTrue(eventDoLog.Args[1].ToString() == "CC", "The second argument should be value.");

        //}

        //private void EventCatcher(object eventSender, EventArgs eventArgs)
        //{
        //    doLogInEventArgs.Add(eventArgs);
        //}

        //#endregion DoLogIn
    }

}
