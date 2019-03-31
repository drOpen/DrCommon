/*
  UTestDDValue.cs -- Unit Tests of DDValue for 'DrData' general purpose Data abstraction layer 1.0.1, October 6, 2013
 
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
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrData;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DrOpen.DrCommon.DrData.Exceptions;

namespace UTestDrData
{
    /// <summary>
    /// Test of DDValue
    /// </summary>
    [TestClass]
    public class UTestDDValue
    {
        [Flags]
        private enum TEST_VALUE_ENUM
        {
            TEST_VALUE_ENUM_A,
            TEST_VALUE_ENUM_B,
            TEST_VALUE_ENUM_A_AND_B = TEST_VALUE_ENUM_A | TEST_VALUE_ENUM_B,
        }

        #region GetObjSize
        [TestMethod]
        public void TestGetObjSize()
        {
            try
            {
                DDValue.GetObjSize(new object());
                Assert.Fail("a.GetObjSize() - cannot catch exception after try GetObjSize() for unsupported Type;");
            }
            catch (Exception)
            {
            }
        }
        #endregion GetObjSize
        #region GetValue
        [TestMethod]
        public void TestGetValue()
        {
            var a = new DDValue();
            try
            {
                var value = a.GetValue();
                Assert.IsTrue(null == value, "GetValue from null should be return null.");      // Uncomment it if null type is supported
                //Assert.Fail("a.GetValue() - cannot catch exception after try GetValue from null Data.Type;");     // Uncomment it if null type is unsupported
            }
            catch (Exception)
            {
            }
        }
        #endregion GetValue
        #region Equals
        [TestMethod]
        public void TestEqualsObject()
        {
            var a = new DDValue();
            object objClone = a.Clone();
            object objA = a;
            DDValue ddvalueA = a;

            Assert.IsFalse(a.Equals(objClone), "Equals() must return false for cloned object.");
            Assert.IsTrue(a.Equals(objA), "Equals() must return true for linked object.");
            Assert.IsTrue(a.Equals(ddvalueA), "Equals() must return false for linked DDValue object.");
        }
        [TestMethod]
        public void TestEqualsDifferentObject()
        {
            var a = new DDValue();
            object obj = new object();

            Assert.IsFalse(a.Equals(obj), "Equals() must return false for different object types.");
        }
        #endregion Equals
        #region Compare()
        [TestMethod]
        public void TestCompareNullObjects()
        {
            Assert.IsTrue(DDValue.Compare(null, null) == 0, "Compare() must return 0 for both null params.");
        }
        [TestMethod]
        public void TestCompareOneOfObjectIsNull()
        {
            Assert.IsTrue(DDValue.Compare(null, new DDValue(1)) == 1, "Compare() must return 1 if one of object is null.");
            Assert.IsTrue(DDValue.Compare(new DDValue(true), null) == 1, "Compare() must return 1 if one of object is null.");
        }
        [TestMethod]
        public void TestCompareNullObjectsData()
        {
            Assert.IsTrue(DDValue.Compare(new DDValue(), new DDValue()) == 0, "Compare() must return 0 if both DDValue objects data are null.");
        }
        [TestMethod]
        public void TestCompareNullObjectsDataWithBooltr()
        {
            Assert.IsFalse(DDValue.Compare(new DDValue(true), new DDValue()) == 0, "Compare() must return 1 for null and bool data object.");
        }
        [TestMethod]
        public void TestCompareOneOfObjectTypeIsNull()
        {
            Assert.IsTrue(DDValue.Compare(new DDValue(), new DDValue(1)) == 1, "Compare() must return 1 if one of object type is null.");
            DDValue v = new DDValue("a");
            Assert.IsTrue(DDValue.Compare(new DDValue(""), v) == 1, "Compare() must return 1 if one of object data is null.");
            Assert.IsTrue(DDValue.Compare(new DDValue(true), new DDValue()) == 1, "Compare() must return 1 if one of object type is null.");
        }
        [TestMethod]
        public void TestCompareObjectsWithDifferentSize()
        {
            Assert.IsTrue(DDValue.Compare(new DDValue(new byte[] { 1, 2 }), new DDValue(new byte[] { 1 })) == 1, "Compare() must return 1 if DDValue objects have different size.");
        }

        //[TestMethod]
        public void TestCompareBigObjects()
        {
            byte[] myBytesA = new byte[10 * 10000];
            byte[] myBytesB = new byte[10 * 10000];
            var valA = new DDValue(myBytesA);
            var valB = new DDValue(myBytesB);
            var start = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                DDValue.Compare(valA, valB);
            }
            var stop = DateTime.Now;
            Assert.Fail((stop.Ticks - start.Ticks).ToString());
        }


        #endregion Compare()
        #region CompareTo()
        [TestMethod]
        public void TestDifferentObjectTypeCompareTo()
        {
            var o1 = new DDValue(123);
            var o2 = new object();
            Assert.IsTrue(o1.CompareTo(o2) == 1, "CompareTo() different object type must be return 1.");

        }
        #endregion CompareTo()
        #region ToString()
        [TestMethod]
        public void TestNullDataToString()
        {
            var v = new DDValue();
            Assert.IsTrue(v.ToString() == string.Empty, "ToString() must return String.Empty if DDValue has null data.");
        }
        #endregion ToString()
        #region test GetHashCode()
        [TestMethod]
        public void TestGetHashCodeEmptyData()
        {
            var a = new DDValue();
            Assert.IsTrue(a.GetHashCode() == 0, "GetHashCode() must return 0 for null data object.");
        }
        [TestMethod]
        public void TestGetHashCode()
        {
            var a = new DDValue(123);
            Assert.IsTrue(a.GetHashCode() == a.GetValueAsByteArray().GetHashCode(), "GetHashCode() must equal Byte[].GetHashCode() for none null data object.");
        }
        #endregion test GetHashCode()
        #region ValidateType


        [TestMethod]
        public void TestValidateAllSupportedTypes()
        {

            var t = new Type[] 
                                {
                                typeof(int), typeof(int[]),typeof(int?),
                                typeof(string), typeof(string[]),
                                typeof(DateTime), typeof(DateTime[]),typeof(DateTime?),
                                typeof(char), typeof(char[]),typeof(char?), 
                                typeof(bool), typeof(bool[]),typeof(bool?), 
                                typeof(byte), typeof(byte[]),typeof(byte?), 
                                typeof(short), typeof(short[]),typeof(short?),
                                typeof(float), typeof(float[]),typeof(float?),
                                typeof(long), typeof(long[]),typeof(long?),
                                typeof(ushort), typeof(ushort[]),typeof(ushort?),
                                typeof(char), typeof(char[]),typeof(char?),
                                typeof(uint), typeof(uint[]),typeof(uint?),
                                typeof(ulong), typeof(ulong[]),typeof(ulong?),
                                typeof(char), typeof(char[]),typeof(char?),
                                typeof(double), typeof(double[]),typeof(double?),
                                typeof(Guid), typeof(Guid[]),typeof(Guid?)
                                };

            foreach (var item in t)
            {
                Assert.IsTrue(DDValue.ValidateType(item), String.Format("The correct type '{0}' catched as unssuported.", item.ToString()));
            }
        }

        [TestMethod]
        public void TestValidateUnsupportedTypes()
        {
            var t = new Type[] 
                                {
                                typeof(int?[]),
                                typeof(DateTime?[]),
                                typeof(char?[]),
                                typeof(bool?[]),
                                typeof(byte?[]),
                                typeof(short?[]),
                                typeof(float?[]),
                                typeof(long?[]),
                                typeof(ushort?[]),
                                typeof(char?[]),
                                typeof(uint?[]),
                                typeof(ulong?[]),
                                typeof(char?[]),
                                typeof(double?[]),
                                typeof(Guid?[])
                                };

            foreach (var item in t)
            {
                Assert.IsFalse(DDValue.ValidateType(item), String.Format("The incorrect type '{0}' catched as ssuported.", item.ToString()));
            }
        }

        [TestMethod]
        public void TestValidateType()
        {
            Assert.IsFalse(DDValue.ValidateType(new object()), "ValidateType() -> the object type is incorrect.");
            Assert.IsFalse(DDValue.ValidateType(typeof(Object)), "ValidateType() -> the object type is incorrect.");
            Assert.IsFalse(DDValue.ValidateType(new DDNode()), "ValidateType() -> the DDNode type is incorrect.");
            object i = new int();
            Assert.IsTrue(DDValue.ValidateType(i), "ValidateType() -> the int type is correct.");
            Assert.IsTrue(DDValue.ValidateType(typeof(bool)), "ValidateType() -> the bool type is correct.");
        }
        #endregion ValidateType
        #region test Incorrect DataType
        [TestMethod]
        public void TestIncorrectDataType()
        {
            var obj = new object[] 
                                {
                                        new object[1]{null},
                                        new object(),
                                        new int?[6] { null, 1, 2, null, 3, null }
                                };

            foreach (var item in obj)
            {
                CatchIncorrectDataType(item);
            }

        }

        public void CatchIncorrectDataType(object value)
        {
            try
            {
                var d = new DDValue(value);
                Assert.Fail("Allow set incorrect data type - sbyte");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (DDTypeIncorrectException)
            {
                // valid exception
            }
        }

        


        #endregion test Incorrect DataType
        #region Test Empty object
        [TestMethod]
        public void TestCreate()
        {
            var a = new DDValue();
            Assert.IsTrue(a.Size == 0, "The initial size is incorrect");
            Assert.IsTrue(a.Type == null, "The initial type is incorrect");
        }
        #endregion Test Empty object
        #region test ==/!=
        [TestMethod]
        public void TestEqualityNewEmptyObject()
        {
            var a = new DDValue();
            var b = new DDValue();
            Assert.IsTrue(a == b, "== doesn't work.");
            Assert.IsFalse(a != b, "!= doesn't work.");
            var c = a;
            Assert.IsTrue(c == a, "== doesn't work.");
        }

        [TestMethod]
        public void TestEqualityNewObject()
        {
            var a = new DDValue("aaa");
            var b = new DDValue();
            b.SetValue("aaa");
            Assert.IsTrue(a == b, "== doesn't work.");
            Assert.IsFalse(a != b, "!= doesn't work.");
            b.SetValue("AAA");
            Assert.IsFalse(a == b, "== doesn't work.");
            Assert.IsTrue(a != b, "!= doesn't work.");
        }

        [TestMethod]
        public void TestEqualityNewChangedObject()
        {
            var a = new DDValue("aaa");
            var b = new DDValue();
            b.SetValue("aaa");
            Assert.IsTrue(a == b, "== doesn't work.");
            Assert.IsFalse(a != b, "!= doesn't work.");
            b.SetValue("AAA");
            Assert.IsFalse(a == b, "== doesn't work.");
            Assert.IsTrue(a != b, "!= doesn't work.");
        }
        #endregion
        #region test Equal
        [TestMethod]
        public void TestEqualNewEmptyObject()
        {
            var a = new DDValue();
            var b = new DDValue();
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            var c = a;
            Assert.IsTrue(a.Equals(c), "Equal doesn't work.");
        }

        [TestMethod]
        public void TestEqualNewObjectWithData()
        {
            string text = "A";
            var a = new DDValue(text);
            var b = new DDValue(text);
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            var c = a;
            Assert.IsTrue(a.Equals(c), "Equal doesn't work.");
        }
        #endregion Test Equal
        #region test Compare
        [TestMethod]
        public void TestCompareNewEmptyObject()
        {
            var a = new DDValue();
            var b = new DDValue();
            Assert.IsTrue(DDValue.Compare(a, b) == 0, "Compare doesn't work.");
            var c = a;
            Assert.IsTrue(DDValue.Compare(a, c) == 0, "Compare doesn't work.");
        }

        [TestMethod]
        public void TestCompareNewObjectWithData()
        {
            string text = "AЫ";
            var a = new DDValue(text);
            var b = new DDValue(text);
            Assert.IsTrue(DDValue.Compare(a, b) == 0, "Compare doesn't work.");
            var c = a;
            Assert.IsTrue(DDValue.Compare(a, c) == 0, "Compare doesn't work.");
        }
        #endregion Compare
        #region test Clone

        [TestMethod]
        public void TestCloneInterface()
        {
            ICloneable a = new DDValue();
            object b = a.Clone();
            Assert.IsTrue(DDValue.Compare((DDValue)a, (DDValue)b) == 0, "Cloned object isn't compared.");
        }

        [TestMethod]
        public void TestCloneNewEmptyObject()
        {
            var a = new DDValue();
            var b = (DDValue)a.Clone();
            Assert.IsTrue(DDValue.Compare(a, b) == 0, "Cloned object isn't compared.");
            Assert.IsFalse(a.Equals(b), "Cloned object is equals.");
            a.SetValue("test");
            //todo
        }

        [TestMethod]
        public void TestCloneNewObjectWithData()
        {
            string text = "AЫ";
            var a = new DDValue(text);
            var b = (DDValue)a.Clone();
            Assert.IsTrue(DDValue.Compare(a, b) == 0, "Cloned object isn't compared.");
            Assert.IsFalse(a.Equals(b), "Cloned object is equals.");
            a.SetValue("test");
            //todo
        }
        #endregion test Clone
        #region test HEX
        [TestMethod]
        public void TestHEXParityCheck()
        {
            const string hexValue = "123";
            try
            {
                DDValue.HEX(hexValue);
                Assert.Fail("The parity check for HEX string doesn't work.");
            }

            catch (DDValueException e)
            {
                Assert.AreEqual(e.Value, hexValue, "Exception value is incorrect.");
            }
        }

        [TestMethod]
        public void TestByteArrayToString()
        {
            string expected = "800F";
            var a = new DDValue(new byte[] { 128, 15 });
            Assert.IsTrue(a.ToString() == expected, "Incorrect HEX format {0}. The expected - '{1}.'", a.ToString(), expected);
        }

        [TestMethod]
        public void TestHEXStringUTF8()
        {
            string text = "test string текстовая строка";
            TestHEX(text);
        }
        [TestMethod]
        public void TestHEXStringUTF8SingleChar()
        {
            string text = "Ы";
            TestHEX(text);
        }
        [TestMethod]
        public void TestHEXStringANSI_Even()
        {
            string text = "test string";
            TestHEX(text);
        }

        [TestMethod]
        public void TestHEXStringANSI_Odd()
        {
            string text = "_test_string";
            TestHEX(text);
        }

        [TestMethod]
        public void TestHEXString_SpaceOnly()
        {
            string text = " ";
            TestHEX(text);
        }

        private void TestHEX(string text)
        {
            var aSource = new DDValue(text);
            var hexString = aSource.GetValueAsHEX();
            var aTarget = new DDValue();
            aTarget.SetHEXValue(aSource.Type, hexString);
            Assert.IsTrue(aTarget.GetValueAsString() == text, "Incorrect convertion ToString() -> HEX -> ToString() {0}. The expected - '{1}.'", aTarget.ToString(), text);
        }
        #endregion test HEX
        #region test Enum
        //[TestMethod]
        //public void TestCreateWithEnumValue()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A_AND_B;
        //    var a = new DDValue(test);
        //    ValidateEnum(test, a);
        //}

        //[TestMethod]
        //public void TestCreateSetEnumValue()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A;
        //    var a = new DDValue();
        //    a.SetValue(test);
        //    ValidateEnum(test, a);
        //}


        //[TestMethod]
        //public void TestCreateSetImpicitEnumValue()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A;
        //    var a = new DDValue();
        //    a = test;
        //    ValidateEnum(test, a);
        //}

        //[TestMethod]
        //public void TestChangeEnumValue()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A;
        //    var a = new DDValue();
        //    a = test;
        //    ValidateEnum(test, a);
        //    test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_B;
        //    a = test;
        //    ValidateEnum(test, a);
        //}

        //[TestMethod]
        //public void TestChangeTypeFromEnumToBool()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A;
        //    var a = new DDValue();
        //    a = test;
        //    ValidateEnum(test, a);
        //    var b = true;
        //    a = b;
        //    ValidateBool(b, a);
        //    CommonChangeObjectTypeValidation(a, test);
        //}
        //[TestMethod]
        //public void TestEnumToFromHex()
        //{
        //    Enum test = TEST_VALUE_ENUM.TEST_VALUE_ENUM_A;
        //    var a = new DDValue(test);
        //    ValidateEnum(test, a);
        //    var hexValue = a.GetValueAsHEX();
        //    var b = new DDValue();
        //    b.SetHEXValue(test.GetType(), hexValue);
        //    ValidateEnum(test, b);
        //    Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
        //    Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        //}
        #endregion test Enum
        #region test string
        [TestMethod]
        public void TestCreateWithStringValue()
        {
            string test = "Тест Unicode";
            var a = new DDValue(test);
            ValidateString(test, a);
        }

        [TestMethod]
        public void TestCreateSetStringValue()
        {
            string test = "Тест Unicode\r\n2\t";
            var a = new DDValue();
            a.SetValue(test);
            ValidateString(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitStringValue()
        {
            string test = "Тест Unicode\r\n2\t";
            var a = new DDValue();
            a = test;
            ValidateString(test, a);
        }

        [TestMethod]
        public void TestChangeStringValue()
        {
            string test = "Тест Unicode\r\n2\t";
            var a = new DDValue();
            a = test;
            ValidateString(test, a);
            test = "1";
            a = test;
            ValidateString(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromStringToBool()
        {
            string test = "Тест Unicode\r\n2\t";
            var a = new DDValue();
            a = test;
            ValidateString(test, a);
            var b = true;
            a = b;
            ValidateBool(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestStringToFromHex()
        {
            string test = "UTF-8 строка\\\r\n2\t\v";
            var a = new DDValue(test);
            ValidateString(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateString(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test string
        #region test string []
        [TestMethod]
        public void TestCreateWithStringArray()
        {
            string[] test = new string[] { };
            var a = new DDValue(test);
            ValidateStringArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithStringArraySingleValue()
        {
            string[] test = new string[] { "Тест Unicode" };
            var a = new DDValue(test);
            ValidateStringArray(test, a);
        }

        [TestMethod]
        public void TestCreateWithStringArrayWithNullValue()
        {
            var test = new string[] { "1", null, "2" };
            var a = new DDValue(test);
            try
            {
                ValidateStringArray(test, a); // catch well know issue https://github.com/drOpen/DrCommon/issues/1
            }
            catch
            {
                throw new AssertInconclusiveException(); // Warning!
            }
        }
        [TestMethod]
        public void TestCreateWithStringArrayMultipleValue()
        {
            string[] test = new string[] { "", "Тест Unicode", "", "A\t\n2\rA", "" };
            var a = new DDValue(test);
            ValidateStringArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetStringArrayValue()
        {
            string[] test = { "Тест Unicode\r\n2\t" };
            var a = new DDValue();
            a.SetValue(test);
            ValidateStringArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitStringArrayValue()
        {
            string[] test = { "Тест Unicode\r\n2\t" };
            var a = new DDValue();
            a = test;
            ValidateStringArray(test, a);
        }

        [TestMethod]
        public void TestChangeStringArrayValueIncrease()
        {
            string[] test = { "Тест Unicode\r\n2\t" };
            var a = new DDValue();
            a = test;
            ValidateStringArray(test, a);
            test = new string[] { "1", "2", "" };
            a = test;
            ValidateStringArray(test, a);
        }
        [TestMethod]
        public void TestChangeStringArrayValueDecrease()
        {
            string[] test = { "Тест Unicode\r\n2\t", "", "1", "asdasdasdasd" };
            var a = new DDValue();
            a = test;
            ValidateStringArray(test, a);
            test = new string[] { "1" };
            a = test;
            ValidateStringArray(test, a);
        }
        [TestMethod]
        public void TestChangeStringArrayValueToEmpty()
        {
            string[] test = { "Тест Unicode\r\n2\t", "", "1", "asdasdasdasd" };
            var a = new DDValue();
            a = test;
            ValidateStringArray(test, a);
            test = new string[] { };
            a = test;
            ValidateStringArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromStringArrayToString()
        {
            string[] test = { "Тест Unicode\r\n2\t", "", "1", "asdasdasdasd" };
            var a = new DDValue();
            a = test;
            ValidateStringArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestStringArrayToFromHex()
        {
            string[] test = { ", ", "UTF-8 строка\\\r\n2\t\v", "" };
            var a = new DDValue(test);
            ValidateStringArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateStringArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }

        #endregion test string []
        #region test char
        [TestMethod]
        public void TestCreateWithCharValue()
        {
            char test = '1';
            var a = new DDValue(test);
            ValidateChar(test, a);
        }

        [TestMethod]
        public void TestCreateSetCharValue()
        {
            char test = 'a';
            var a = new DDValue();
            a.SetValue(test);
            ValidateChar(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitCharValue()
        {
            char test = 'Ы';
            var a = new DDValue();
            a = test;
            ValidateChar(test, a);
        }

        [TestMethod]
        public void TestChangeCharValue()
        {
            
            char test = '1';
            var a = new DDValue();
            a = test;
            ValidateChar(test, a);
            test = '2';
            a = test;
            ValidateChar(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromCharToInt()
        {
            char test = '1';
            var a = new DDValue();
            a = test;
            ValidateChar(test, a);
            var b = 1;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestCharToFromHex()
        {
            char test = 'Ы';
            var a = new DDValue(test);
            ValidateChar(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateChar(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test char
        #region test char[]
        [TestMethod]
        public void TestCreateWithCharArray()
        {
            var test = new char[] { };
            var a = new DDValue(test);
            ValidateCharArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithCharArraySingleValue()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e' };
            var a = new DDValue(test);
            ValidateCharArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithCharArrayMultipleValue()
        {
            var test = new char[] { '\0', 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e', '\v', 'A', '\t', '\n', '\r', 'A', '\0' };
            var a = new DDValue(test);
            ValidateCharArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetCharArrayValue()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e' };
            var a = new DDValue();
            a.SetValue(test);
            ValidateCharArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitCharArrayValue()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e', '\r', '\n', '\t' };
            var a = new DDValue();
            a = test;
            ValidateCharArray(test, a);
        }

        [TestMethod]
        public void TestChangeCharArrayValueIncrease()
        {
            var test = new char[] { '\n', '\t' };
            var a = new DDValue();
            a = test;
            ValidateCharArray(test, a);
            test = new char[] { '1', '2', '\0' };
            a = test;
            ValidateCharArray(test, a);
        }
        [TestMethod]
        public void TestChangeCharArrayValueDecrease()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e', '\r', '\n', '\t' };
            var a = new DDValue();
            a = test;
            ValidateCharArray(test, a);
            test = new char[] { '1' };
            a = test;
            ValidateCharArray(test, a);
        }
        [TestMethod]
        public void TestChangeCharArrayValueToEmpty()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e', '\r', '\n', '\t' };
            var a = new DDValue();
            a = test;
            ValidateCharArray(test, a);
            test = new char[] { };
            a = test;
            ValidateCharArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromCharArrayToString()
        {
            var test = new char[] { 'Т', 'е', 'с', 'т', ' ', 'U', 'n', 'i', 'c', 'o', 'd', 'e', '\r', '\n', '\t' };
            var a = new DDValue();
            a = test;
            ValidateCharArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestCharArrayToFromHex()
        {
            var test = new char[] { ',', '"', 'U', 'T', 'F', '-', '8', ' ', 'с', 'т', 'р', 'о', 'к', 'а', '\\', '\r', '\n', '\t', '\v', '\0' };
            var a = new DDValue(test);
            ValidateCharArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateCharArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }

        #endregion test char[]
        #region test bool
        [TestMethod]
        public void TestCreateWithBoolFalse()
        {
            var test = false;
            var a = new DDValue(test);
            ValidateBool(test, a);
        }
        [TestMethod]
        public void TestCreateWithBoolTrue()
        {
            var test = true;
            var a = new DDValue(test);
            ValidateBool(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitBoolValue()
        {
            var test = true;
            var a = new DDValue();
            a = test;
            ValidateBool(test, a);
        }

        [TestMethod]
        public void TestChangeBoolValue()
        {
            var test = true;
            var a = new DDValue();
            a = test;
            ValidateBool(test, a);
            test = false;
            a = test;
            ValidateBool(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromBoolToInt()
        {
            var test = false;
            var a = new DDValue();
            a = test;
            ValidateBool(test, a);
            var b = -1;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestChangeTypeFromBoolToUInt()
        {
            var test = true;
            var a = new DDValue();
            a = test;
            ValidateBool(test, a);
            var b = 0;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestBoolToFromHex()
        {
            var test = false;
            var a = new DDValue(test);
            ValidateBool(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateBool(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test bool
        #region test bool[]
        [TestMethod]
        public void TestCreateWithBoolArray()
        {
            var test = new bool[] { };
            var a = new DDValue(test);
            ValidateBoolArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithBoolArraySingleValue()
        {
            var test = new bool[] { true };
            var a = new DDValue(test);
            ValidateBoolArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithBoolArrayMultipleValue()
        {
            var test = new bool[] { true, true, false, false };
            var a = new DDValue(test);
            ValidateBoolArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetBoolArrayValue()
        {
            var test = new bool[] { true };
            var a = new DDValue();
            a.SetValue(test);
            ValidateBoolArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitBoolArrayValue()
        {
            var test = new bool[] { false };
            var a = new DDValue();
            a = test;
            ValidateBoolArray(test, a);
        }

        [TestMethod]
        public void TestChangeBoolArrayValueIncrease()
        {
            var test = new bool[] { true };
            var a = new DDValue();
            a = test;
            ValidateBoolArray(test, a);
            test = new bool[] { false, false, true, true };
            a = test;
            ValidateBoolArray(test, a);
        }
        [TestMethod]
        public void TestChangeBoolArrayValueDecrease()
        {
            var test = new bool[] { true, true, false, false, true };
            var a = new DDValue();
            a = test;
            ValidateBoolArray(test, a);
            test = new bool[] { true };
            a = test;
            ValidateBoolArray(test, a);
        }
        [TestMethod]
        public void TestChangeValueArrayValueToEmpty()
        {
            var test = new bool[] { true, false };
            var a = new DDValue();
            a = test;
            ValidateBoolArray(test, a);
            test = new bool[] { };
            a = test;
            ValidateBoolArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromBoolArrayToBool()
        {
            var test = new bool[] { true, false, true };
            var a = new DDValue();
            a = test;
            ValidateBoolArray(test, a);
            var b = false;
            a = b;
            ValidateBool(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestBoolArrayToFromHex()
        {
            var test = new bool[] { true, false, true };
            var a = new DDValue(test);
            ValidateBoolArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateBoolArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }

        #endregion test bool []
        #region test int
        [TestMethod]
        public void TestCreateWithInt0Value()
        {
            int test = 0;
            var a = new DDValue(test);
            ValidateInt(test, a);
        }
        [TestMethod]
        public void TestCreateWithIntMinValue()
        {
            int test = int.MinValue;
            var a = new DDValue(test);
            ValidateInt(test, a);
        }

        [TestMethod]
        public void TestCreateSetIntMaxValue()
        {
            int test = int.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateInt(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpiciIntValue()
        {
            int test = 123;
            var a = new DDValue();
            a = test;
            ValidateInt(test, a);
        }

        [TestMethod]
        public void TestChangeIntValue()
        {
            int test = 123;
            var a = new DDValue();
            a = test;
            ValidateInt(test, a);
            test = -123;
            a = test;
            ValidateInt(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromIntToBool()
        {
            int test = 1;
            var a = new DDValue();
            a = test;
            ValidateInt(test, a);
            var b = false;
            a = b;
            ValidateBool(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestIntToFromHex()
        {
            int test = int.MaxValue;
            var a = new DDValue(test);
            ValidateInt(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateInt(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test int
        #region test int[]
        [TestMethod]
        public void TestCreateWithIntArray()
        {
            var test = new int[] { };
            var a = new DDValue(test);
            ValidateIntArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithIntArraySingleValue()
        {
            var test = new int[] { 1 };
            var a = new DDValue(test);
            ValidateIntArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithIntArrayMultipleValue()
        {
            var test = new int[] { Int16.MinValue, 0, Int16.MaxValue };
            var a = new DDValue(test);
            ValidateIntArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetIntArrayValue()
        {
            var test = new int[] { -1, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateIntArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitIntArrayValue()
        {
            var test = new int[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateIntArray(test, a);
        }

        [TestMethod]
        public void TestChangeIntArrayValueIncrease()
        {
            var test = new int[] { Int16.MinValue };
            var a = new DDValue();
            a = test;
            ValidateIntArray(test, a);
            test = new int[] { Int16.MinValue, Int16.MaxValue };
            a = test;
            ValidateIntArray(test, a);
        }
        [TestMethod]
        public void TestChangeIntArrayValueDecrease()
        {
            var test = new int[] { Int16.MinValue, 0, Int16.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateIntArray(test, a);
            test = new int[] { 0 };
            a = test;
            ValidateIntArray(test, a);
        }
        [TestMethod]
        public void TestChangeIntArrayValueToEmpty()
        {
            var test = new int[] { Int16.MinValue, 0, Int16.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateIntArray(test, a);
            test = new int[] { };
            a = test;
            ValidateIntArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromIntArrayToString()
        {
            int[] test = { Int16.MinValue, 0, Int16.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateIntArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestIntArrayToFromHex()
        {
            int[] test = { Int16.MaxValue, 0, Int16.MinValue };
            var a = new DDValue(test);
            ValidateIntArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateIntArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }

        #endregion test int[]
        #region test DateTime
        [TestMethod]
        public void TestCreateWithDateTimeNowValue()
        {
            DateTime test = DateTime.Now;
            var a = new DDValue(test);
            ValidateDateTime(test, a);
        }
        [TestMethod]
        public void TestCreateWithDateTimeMinValue()
        {
            DateTime test = DateTime.MinValue;
            var a = new DDValue(test);
            ValidateDateTime(test, a);
        }

        [TestMethod]
        public void TestCreateSetDateTimeMaxValue()
        {
            DateTime test = DateTime.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateDateTime(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpiciDateTimeValue()
        {
            DateTime test = DateTime.Now;
            var a = new DDValue();
            a = test;
            ValidateDateTime(test, a);
        }

        [TestMethod]
        public void TestChangeDateTimeValue()
        {
            DateTime test = DateTime.Now;
            var a = new DDValue();
            a = test;
            ValidateDateTime(test, a);
            test = DateTime.UtcNow;
            a = test;
            ValidateDateTime(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromDateTimeToString()
        {
            DateTime test = DateTime.Now;
            var a = new DDValue();
            a = test;
            ValidateDateTime(test, a);
            var b = "TestЫЫЫ";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestDateTimeToFromHex()
        {
            DateTime test = DateTime.Now;
            var a = new DDValue(test);
            ValidateDateTime(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateDateTime(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test DateTime
        #region test DateTime[]
        [TestMethod]
        public void TestCreateWithDateTimeArray()
        {
            var test = new DateTime[] { };
            var a = new DDValue(test);
            ValidateDateTimeArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithDateTimeArraySingleValue()
        {
            var test = new DateTime[] { DateTime.MinValue };
            var a = new DDValue(test);
            ValidateDateTimeArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithDateTimeArrayMultipleValue()
        {
            var test = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
            var a = new DDValue(test);
            ValidateDateTimeArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetDateTimeArrayValue()
        {
            var test = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
            var a = new DDValue();
            a.SetValue(test);
            ValidateDateTimeArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitDateTimeArrayValue()
        {
            var test = new DateTime[] { DateTime.Now };
            var a = new DDValue();
            a = test;
            ValidateDateTimeArray(test, a);
        }

        [TestMethod]
        public void TestChangeDateTimeArrayValueIncrease()
        {
            var test = new DateTime[] { DateTime.MinValue };
            var a = new DDValue();
            a = test;
            ValidateDateTimeArray(test, a);
            test = new DateTime[] { DateTime.MinValue, DateTime.MaxValue };
            a = test;
            ValidateDateTimeArray(test, a);
        }
        [TestMethod]
        public void TestChangeDateTimeArrayValueDecrease()
        {
            var test = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateDateTimeArray(test, a);
            test = new DateTime[] { DateTime.Now };
            a = test;
            ValidateDateTimeArray(test, a);
        }
        [TestMethod]
        public void TestChangeDateTimeArrayValueToEmpty()
        {
            var test = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateDateTimeArray(test, a);
            test = new DateTime[] { };
            a = test;
            ValidateDateTimeArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromDateTimeArrayToString()
        {
            DateTime[] test = { DateTime.MaxValue, DateTime.Now, DateTime.MinValue };
            var a = new DDValue();
            a = test;
            ValidateDateTimeArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestDateTimeArrayToFromHex()
        {
            DateTime[] test = { DateTime.MaxValue, DateTime.Now, DateTime.MinValue };
            var a = new DDValue(test);
            ValidateDateTimeArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateDateTimeArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test DateTime[]
        #region test uint
        [TestMethod]
        public void TestCreateWithUInt0Value()
        {
            uint test = 0;
            var a = new DDValue(test);
            ValidateUInt(test, a);
        }
        [TestMethod]
        public void TestCreateWithUIntMinValue()
        {
            uint test = uint.MinValue;
            var a = new DDValue(test);
            ValidateUInt(test, a);
        }

        [TestMethod]
        public void TestCreateSetUIntMaxValue()
        {
            uint test = uint.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateUInt(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpiciUIntValue()
        {
            uint test = 123;
            var a = new DDValue();
            a = test;
            ValidateUInt(test, a);
        }

        [TestMethod]
        public void TestChangeUIntValue()
        {
            uint test = 123;
            var a = new DDValue();
            a = test;
            ValidateUInt(test, a);
            test = 0;
            a = test;
            ValidateUInt(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromUIntToInt()
        {
            uint test = uint.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateUInt(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestUIntToFromHex()
        {
            uint test = uint.MaxValue;
            var a = new DDValue(test);
            ValidateUInt(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateUInt(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test int
        #region test uint[]
        [TestMethod]
        public void TestCreateWithUIntArray()
        {
            var test = new uint[] { };
            var a = new DDValue(test);
            ValidateUIntArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithUIntArraySingleValue()
        {
            var test = new uint[] { 1 };
            var a = new DDValue(test);
            ValidateUIntArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithUIntArrayMultipleValue()
        {
            var test = new uint[] { UInt16.MinValue, 0, UInt16.MaxValue };
            var a = new DDValue(test);
            ValidateUIntArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetUIntArrayValue()
        {
            var test = new uint[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateUIntArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitUIntArrayValue()
        {
            var test = new uint[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateUIntArray(test, a);
        }

        [TestMethod]
        public void TestChangeUIntArrayValueIncrease()
        {
            var test = new uint[] { UInt16.MinValue };
            var a = new DDValue();
            a = test;
            ValidateUIntArray(test, a);
            test = new uint[] { UInt16.MinValue, UInt16.MaxValue };
            a = test;
            ValidateUIntArray(test, a);
        }
        [TestMethod]
        public void TestChangeUIntArrayValueDecrease()
        {
            var test = new uint[] { UInt16.MinValue, 0, UInt16.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateUIntArray(test, a);
            test = new uint[] { 0 };
            a = test;
            ValidateUIntArray(test, a);
        }
        [TestMethod]
        public void TestChangeUIntArrayValueToEmpty()
        {
            var test = new uint[] { UInt16.MinValue, 0, UInt16.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateUIntArray(test, a);
            test = new uint[] { };
            a = test;
            ValidateUIntArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromUIntArrayToString()
        {
            uint[] test = { UInt16.MaxValue, 0, UInt16.MinValue };
            var a = new DDValue();
            a = test;
            ValidateUIntArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestUIntArrayToFromHex()
        {
            uint[] test = { UInt16.MaxValue, 0, UInt16.MinValue };
            var a = new DDValue(test);
            ValidateUIntArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateUIntArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }

        #endregion test uint[]
        #region test decimal
        [TestMethod]
        public void TestCreateWithDecimal0Value()
        {
            var test = 0M;
            var a = new DDValue(test);
            ValidateDecimal(test, a);
        }
        [TestMethod]
        public void TestCreateWithDecimalMinValue()
        {
            var test = Decimal.MinValue;
            var a = new DDValue(test);
            ValidateDecimal(test, a);
        }

        [TestMethod]
        public void TestCreateSetDecimalMaxValue()
        {
            var test = Decimal.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateDecimal(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitDecimalValue()
        {
            decimal test = 1M / 3M;
            var a = new DDValue();
            a = test;
            ValidateDecimal(test, a);
        }

        [TestMethod]
        public void TestChangeDecimalleValue()
        {
            Decimal test = 10M / 3M;
            var a = new DDValue();
            a = test;
            ValidateDecimal(test, a);
            test = 0M;
            a = test;
            ValidateDecimal(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromDecimalToInt()
        {
            var test = Decimal.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateDecimal(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestDecimalToFromHex()
        {
            var test = Decimal.MaxValue - 1m /3m;
            var a = new DDValue(test);
            ValidateDecimal(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateDecimal(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test decimal
        #region test double[]
        [TestMethod]
        public void TestCreateWithDoubleArray()
        {
            var test = new double[] { };
            var a = new DDValue(test);
            ValidateDoubleArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithDoubleArraySingleValue()
        {
            var test = new double[] { 1 };
            var a = new DDValue(test);
            ValidateDoubleArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithDoubleArrayMultipleValue()
        {
            var test = new double[] { double.MinValue, 0, double.MaxValue };
            var a = new DDValue(test);
            ValidateDoubleArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetDoubleArrayValue()
        {
            var test = new double[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateDoubleArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitDoubleArrayValue()
        {
            var test = new double[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateDoubleArray(test, a);
        }

        [TestMethod]
        public void TestChangeDoubleArrayValueIncrease()
        {
            var test = new double[] { double.MinValue };
            var a = new DDValue();
            a = test;
            ValidateDoubleArray(test, a);
            test = new double[] { double.MinValue, double.MaxValue };
            a = test;
            ValidateDoubleArray(test, a);
        }
        [TestMethod]
        public void TestChangeDoubleArrayValueDecrease()
        {
            var test = new double[] { double.MinValue, 0, double.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateDoubleArray(test, a);
            test = new double[] { 0 };
            a = test;
            ValidateDoubleArray(test, a);
        }
        [TestMethod]
        public void TestChangeDoubleArrayValueToEmpty()
        {
            var test = new double[] { double.MinValue, 0, double.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateDoubleArray(test, a);
            test = new double[] { };
            a = test;
            ValidateDoubleArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromDoubleArrayToString()
        {
            double[] test = { double.MaxValue, 0, double.MinValue };
            var a = new DDValue();
            a = test;
            ValidateDoubleArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestDoubleArrayToFromHex()
        {
            double[] test = { double.MaxValue, 0, double.MinValue };
            var a = new DDValue(test);
            ValidateDoubleArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateDoubleArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test double[]
        #region test Double
        [TestMethod]
        public void TestCreateWithDouble0Value()
        {
            Double test = 0;
            var a = new DDValue(test);
            ValidateDouble(test, a);
        }
        [TestMethod]
        public void TestCreateWithDoubleMinValue()
        {
            Double test = Double.MinValue;
            var a = new DDValue(test);
            ValidateDouble(test, a);
        }

        [TestMethod]
        public void TestCreateSetDoubleMaxValue()
        {
            Double test = Double.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateDouble(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitDoubleValue()
        {
            Double test = 123;
            var a = new DDValue();
            a = test;
            ValidateDouble(test, a);
        }

        [TestMethod]
        public void TestChangeDoubleValue()
        {
            Double test = 123;
            var a = new DDValue();
            a = test;
            ValidateDouble(test, a);
            test = 0;
            a = test;
            ValidateDouble(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromDoubleToInt()
        {
            Double test = Double.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateDouble(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestDoubleToFromHex()
        {
            Double test = Double.MaxValue;
            var a = new DDValue(test);
            ValidateDouble(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateDouble(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test double
        #region test Float
        [TestMethod]
        public void TestCreateWithFloat0Value()
        {
            float test = 0;
            var a = new DDValue(test);
            ValidateFloat(test, a);
        }
        [TestMethod]
        public void TestCreateWithFloatMinValue()
        {
            float test = float.MinValue;
            var a = new DDValue(test);
            ValidateFloat(test, a);
        }

        [TestMethod]
        public void TestCreateSetFloatMaxValue()
        {
            float test = float.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateFloat(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitFloatValue()
        {
            float test = 123;
            var a = new DDValue();
            a = test;
            ValidateFloat(test, a);
        }

        [TestMethod]
        public void TestChangeFloatValue()
        {
            float test = 123;
            var a = new DDValue();
            a = test;
            ValidateFloat(test, a);
            test = 0;
            a = test;
            ValidateFloat(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromFloatToInt()
        {
            float test = float.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateFloat(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestFloatToFromHex()
        {
            float test = float.MaxValue;
            var a = new DDValue(test);
            ValidateFloat(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateFloat(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test Float
        #region test float[]
        [TestMethod]
        public void TestCreateWithFloatArray()
        {
            var test = new float[] { };
            var a = new DDValue(test);
            ValidateFloatArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithFloatArraySingleValue()
        {
            var test = new float[] { 1 };
            var a = new DDValue(test);
            ValidateFloatArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithFloatArrayMultipleValue()
        {
            var test = new float[] { float.MinValue, 0, float.MaxValue };
            var a = new DDValue(test);
            ValidateFloatArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetFloatArrayValue()
        {
            var test = new float[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateFloatArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitFloatArrayValue()
        {
            var test = new float[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateFloatArray(test, a);
        }

        [TestMethod]
        public void TestChangeFloatArrayValueIncrease()
        {
            var test = new float[] { float.MinValue };
            var a = new DDValue();
            a = test;
            ValidateFloatArray(test, a);
            test = new float[] { float.MinValue, float.MaxValue };
            a = test;
            ValidateFloatArray(test, a);
        }
        [TestMethod]
        public void TestChangeFloatArrayValueDecrease()
        {
            var test = new float[] { float.MinValue, 0, float.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateFloatArray(test, a);
            test = new float[] { 0 };
            a = test;
            ValidateFloatArray(test, a);
        }
        [TestMethod]
        public void TestChangeFloatArrayValueToEmpty()
        {
            var test = new float[] { float.MinValue, 0, float.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateFloatArray(test, a);
            test = new float[] { };
            a = test;
            ValidateFloatArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromFloatArrayToString()
        {
            float[] test = { float.MaxValue, 0, float.MinValue };
            var a = new DDValue();
            a = test;
            ValidateFloatArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestFloatArrayToFromHex()
        {
            float[] test = { float.MaxValue, 0, float.MinValue };
            var a = new DDValue(test);
            ValidateFloatArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateFloatArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test float[]
        #region test long
        [TestMethod]
        public void TestCreateWithLong0Value()
        {
            long test = 0;
            var a = new DDValue(test);
            ValidateLong(test, a);
        }
        [TestMethod]
        public void TestCreateWithLongMinValue()
        {
            long test = long.MinValue;
            var a = new DDValue(test);
            ValidateLong(test, a);
        }

        [TestMethod]
        public void TestCreateSetLongMaxValue()
        {
            long test = long.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateLong(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitLongValue()
        {
            long test = 123;
            var a = new DDValue();
            a = test;
            ValidateLong(test, a);
        }

        [TestMethod]
        public void TestChangeLongValue()
        {
            long test = 123;
            var a = new DDValue();
            a = test;
            ValidateLong(test, a);
            test = 0;
            a = test;
            ValidateLong(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromLongToInt()
        {
            long test = long.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateLong(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestLongToFromHex()
        {
            long test = long.MaxValue;
            var a = new DDValue(test);
            ValidateLong(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateLong(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test long
        #region test long[]
        [TestMethod]
        public void TestCreateWithLongArray()
        {
            var test = new long[] { };
            var a = new DDValue(test);
            ValidateLongArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithLongArraySingleValue()
        {
            var test = new long[] { 1 };
            var a = new DDValue(test);
            ValidateLongArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithLongArrayMultipleValue()
        {
            var test = new long[] { long.MinValue, 0, long.MaxValue };
            var a = new DDValue(test);
            ValidateLongArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetLongArrayValue()
        {
            var test = new long[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateLongArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitLongArrayValue()
        {
            var test = new long[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateLongArray(test, a);
        }

        [TestMethod]
        public void TestChangeLongArrayValueIncrease()
        {
            var test = new long[] { long.MinValue };
            var a = new DDValue();
            a = test;
            ValidateLongArray(test, a);
            test = new long[] { long.MinValue, long.MaxValue };
            a = test;
            ValidateLongArray(test, a);
        }
        [TestMethod]
        public void TestChangeLongArrayValueDecrease()
        {
            var test = new long[] { long.MinValue, 0, long.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateLongArray(test, a);
            test = new long[] { 0 };
            a = test;
            ValidateLongArray(test, a);
        }
        [TestMethod]
        public void TestChangeLongArrayValueToEmpty()
        {
            var test = new long[] { long.MinValue, 0, long.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateLongArray(test, a);
            test = new long[] { };
            a = test;
            ValidateLongArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromLongArrayToString()
        {
            long[] test = { long.MaxValue, 0, long.MinValue };
            var a = new DDValue();
            a = test;
            ValidateLongArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestLongArrayToFromHex()
        {
            long[] test = { long.MaxValue, 0, long.MinValue };
            var a = new DDValue(test);
            ValidateLongArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateLongArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test long[]
        #region test ulong
        [TestMethod]
        public void TestCreateWithULong0Value()
        {
            ulong test = 0;
            var a = new DDValue(test);
            ValidateULong(test, a);
        }
        [TestMethod]
        public void TestCreateWithULongMinValue()
        {
            ulong test = ulong.MinValue;
            var a = new DDValue(test);
            ValidateULong(test, a);
        }

        [TestMethod]
        public void TestCreateSetULongMaxValue()
        {
            ulong test = ulong.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateULong(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitULongValue()
        {
            ulong test = 123;
            var a = new DDValue();
            a = test;
            ValidateULong(test, a);
        }

        [TestMethod]
        public void TestChangeULongValue()
        {
            ulong test = 123;
            var a = new DDValue();
            a = test;
            ValidateULong(test, a);
            test = 0;
            a = test;
            ValidateULong(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromULongToInt()
        {
            ulong test = ulong.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateULong(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestULongToFromHex()
        {
            ulong test = ulong.MaxValue;
            var a = new DDValue(test);
            ValidateULong(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateULong(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test ulong
        #region test ulong[]
        [TestMethod]
        public void TestCreateWithULongArray()
        {
            var test = new ulong[] { };
            var a = new DDValue(test);
            ValidateULongArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithULongArraySingleValue()
        {
            var test = new ulong[] { 1 };
            var a = new DDValue(test);
            ValidateULongArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithULongArrayMultipleValue()
        {
            var test = new ulong[] { ulong.MinValue, 0, ulong.MaxValue };
            var a = new DDValue(test);
            ValidateULongArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetULongArrayValue()
        {
            var test = new ulong[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateULongArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitULongArrayValue()
        {
            var test = new ulong[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateULongArray(test, a);
        }

        [TestMethod]
        public void TestChangeULongArrayValueIncrease()
        {
            var test = new ulong[] { ulong.MinValue };
            var a = new DDValue();
            a = test;
            ValidateULongArray(test, a);
            test = new ulong[] { ulong.MinValue, ulong.MaxValue };
            a = test;
            ValidateULongArray(test, a);
        }
        [TestMethod]
        public void TestChangeULongArrayValueDecrease()
        {
            var test = new ulong[] { ulong.MinValue, 0, ulong.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateULongArray(test, a);
            test = new ulong[] { 0 };
            a = test;
            ValidateULongArray(test, a);
        }
        [TestMethod]
        public void TestChangeULongArrayValueToEmpty()
        {
            var test = new ulong[] { ulong.MinValue, 0, ulong.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateULongArray(test, a);
            test = new ulong[] { };
            a = test;
            ValidateULongArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromULongArrayToString()
        {
            ulong[] test = { ulong.MaxValue, 0, ulong.MinValue };
            var a = new DDValue();
            a = test;
            ValidateULongArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestULongArrayToFromHex()
        {
            ulong[] test = { ulong.MaxValue, 0, ulong.MinValue };
            var a = new DDValue(test);
            ValidateULongArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateULongArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test ulong[]
        #region test short
        [TestMethod]
        public void TestCreateWithShort0Value()
        {
            short test = 0;
            var a = new DDValue(test);
            ValidateShort(test, a);
        }
        [TestMethod]
        public void TestCreateWithShortMinValue()
        {
            short test = short.MinValue;
            var a = new DDValue(test);
            ValidateShort(test, a);
        }

        [TestMethod]
        public void TestCreateSetShortMaxValue()
        {
            short test = short.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateShort(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitShortValue()
        {
            short test = 123;
            var a = new DDValue();
            a = test;
            ValidateShort(test, a);
        }

        [TestMethod]
        public void TestChangeShortValue()
        {
            short test = 123;
            var a = new DDValue();
            a = test;
            ValidateShort(test, a);
            test = 0;
            a = test;
            ValidateShort(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromShortToInt()
        {
            short test = short.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateShort(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestShortToFromHex()
        {
            short test = short.MaxValue;
            var a = new DDValue(test);
            ValidateShort(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateShort(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test short
        #region test short[]
        [TestMethod]
        public void TestCreateWithShortArray()
        {
            var test = new short[] { };
            var a = new DDValue(test);
            ValidateShortArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithShortArraySingleValue()
        {
            var test = new short[] { 1 };
            var a = new DDValue(test);
            ValidateShortArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithShortArrayMultipleValue()
        {
            var test = new short[] { short.MinValue, 0, short.MaxValue };
            var a = new DDValue(test);
            ValidateShortArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetShortArrayValue()
        {
            var test = new short[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateShortArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitShortArrayValue()
        {
            var test = new short[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateShortArray(test, a);
        }

        [TestMethod]
        public void TestChangeShortArrayValueIncrease()
        {
            var test = new short[] { short.MinValue };
            var a = new DDValue();
            a = test;
            ValidateShortArray(test, a);
            test = new short[] { short.MinValue, short.MaxValue };
            a = test;
            ValidateShortArray(test, a);
        }
        [TestMethod]
        public void TestChangeShortArrayValueDecrease()
        {
            var test = new short[] { short.MinValue, 0, short.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateShortArray(test, a);
            test = new short[] { 0 };
            a = test;
            ValidateShortArray(test, a);
        }
        [TestMethod]
        public void TestChangeShortArrayValueToEmpty()
        {
            var test = new short[] { short.MinValue, 0, short.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateShortArray(test, a);
            test = new short[] { };
            a = test;
            ValidateShortArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromShortArrayToString()
        {
            short[] test = { short.MaxValue, 0, short.MinValue };
            var a = new DDValue();
            a = test;
            ValidateShortArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestShortArrayToFromHex()
        {
            short[] test = { short.MaxValue, 0, short.MinValue };
            var a = new DDValue(test);
            ValidateShortArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateShortArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test short[]
        #region test ushort
        [TestMethod]
        public void TestCreateWithUShort0Value()
        {
            ushort test = 0;
            var a = new DDValue(test);
            ValidateUShort(test, a);
        }
        [TestMethod]
        public void TestCreateWithUShortMinValue()
        {
            ushort test = ushort.MinValue;
            var a = new DDValue(test);
            ValidateUShort(test, a);
        }

        [TestMethod]
        public void TestCreateSetUShortMaxValue()
        {
            ushort test = ushort.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateUShort(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitUShortValue()
        {
            ushort test = 123;
            var a = new DDValue();
            a = test;
            ValidateUShort(test, a);
        }

        [TestMethod]
        public void TestChangeUShortValue()
        {
            ushort test = 123;
            var a = new DDValue();
            a = test;
            ValidateUShort(test, a);
            test = 0;
            a = test;
            ValidateUShort(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromUShortToInt()
        {
            ushort test = ushort.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateUShort(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestUShortToFromHex()
        {
            ushort test = ushort.MaxValue;
            var a = new DDValue(test);
            ValidateUShort(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateUShort(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test ushort
        #region test ushort[]
        [TestMethod]
        public void TestCreateWithUShortArray()
        {
            var test = new ushort[] { };
            var a = new DDValue(test);
            ValidateUShortArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithUShortArraySingleValue()
        {
            var test = new ushort[] { 1 };
            var a = new DDValue(test);
            ValidateUShortArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithUShortArrayMultipleValue()
        {
            var test = new ushort[] { ushort.MinValue, 0, ushort.MaxValue };
            var a = new DDValue(test);
            ValidateUShortArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetUShortArrayValue()
        {
            var test = new ushort[] { 2, 0, 1 };
            var a = new DDValue();
            a.SetValue(test);
            ValidateUShortArray(test, a);
        }


        [TestMethod]
        public void TestCreateSetImpicitUShortArrayValue()
        {
            var test = new ushort[] { 123 };
            var a = new DDValue();
            a = test;
            ValidateUShortArray(test, a);
        }

        [TestMethod]
        public void TestChangeUShortArrayValueIncrease()
        {
            var test = new ushort[] { ushort.MinValue };
            var a = new DDValue();
            a = test;
            ValidateUShortArray(test, a);
            test = new ushort[] { ushort.MinValue, ushort.MaxValue };
            a = test;
            ValidateUShortArray(test, a);
        }
        [TestMethod]
        public void TestChangeUShortArrayValueDecrease()
        {
            var test = new ushort[] { ushort.MinValue, 0, ushort.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateUShortArray(test, a);
            test = new ushort[] { 0 };
            a = test;
            ValidateUShortArray(test, a);
        }
        [TestMethod]
        public void TestChangeUShortArrayValueToEmpty()
        {
            var test = new ushort[] { ushort.MinValue, 0, ushort.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateUShortArray(test, a);
            test = new ushort[] { };
            a = test;
            ValidateUShortArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromUShortArrayToString()
        {
            ushort[] test = { ushort.MaxValue, 0, ushort.MinValue };
            var a = new DDValue();
            a = test;
            ValidateUShortArray(test, a);
            var b = "Юникод";
            a = b;
            ValidateString(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }

        [TestMethod]
        public void TestUShortArrayToFromHex()
        {
            ushort[] test = { ushort.MaxValue, 0, ushort.MinValue };
            var a = new DDValue(test);
            ValidateUShortArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateUShortArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
            Assert.IsTrue(CompareStringArray(a, b), "After HEX convertion new data is not equals oroginal data.");
        }
        #endregion test ushort[]
        #region test byte
        [TestMethod]
        public void TestCreateWithByte0Value()
        {
            byte test = 0;
            var a = new DDValue(test);
            ValidateByte(test, a);
        }
        [TestMethod]
        public void TestCreateWithByteMinValue()
        {
            byte test = byte.MinValue;
            var a = new DDValue(test);
            ValidateByte(test, a);
        }

        [TestMethod]
        public void TestCreateSetByteMaxValue()
        {
            byte test = byte.MaxValue;
            var a = new DDValue();
            a.SetValue(test);
            ValidateByte(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitByteValue()
        {
            byte test = 123;
            var a = new DDValue();
            a = test;
            ValidateByte(test, a);
        }

        [TestMethod]
        public void TestChangeByteValue()
        {
            byte test = 123;
            var a = new DDValue();
            a = test;
            ValidateByte(test, a);
            test = 0;
            a = test;
            ValidateByte(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromByteToInt()
        {
            byte test = byte.MaxValue;
            var a = new DDValue();
            a = test;
            ValidateByte(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestByteToFromHex()
        {
            byte test = byte.MaxValue;
            var a = new DDValue(test);
            ValidateByte(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateByte(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test byte
        #region test byte[]
        [TestMethod]
        public void TestCreateWithByteArrayNullValue()
        {
            byte[] test = { };
            var a = new DDValue(test);
            ValidateByteArray(test, a);
        }
        public void TestCreateWithByteArray0Value()
        {
            byte[] test = { 0 };
            var a = new DDValue(test);
            ValidateByteArray(test, a);
        }
        [TestMethod]
        public void TestCreateWithByteArrayMinValue()
        {
            byte[] test = { byte.MinValue };
            var a = new DDValue(test);
            ValidateByteArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetByteArrayMaxValue()
        {
            byte[] test = { byte.MaxValue };
            var a = new DDValue();
            a.SetValue(test);
            ValidateByteArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitByteArrayValue()
        {
            byte[] test = { 1, 2, 3 };
            var a = new DDValue();
            a = test;
            ValidateByteArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromByteArrayToInt()
        {
            byte[] test = { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MaxValue };
            var a = new DDValue();
            a = test;
            ValidateByteArray(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestByteArrayToFromHex()
        {
            byte[] test = { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MaxValue };
            var a = new DDValue(test);
            ValidateByteArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateByteArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test byte[]
        #region test guid

        [TestMethod]
        public void TestCreateWithGuidNulablleNull()
        {
            Guid? test = null;
            var value = new DDValue(test);
            ValidateGuid(test, value);
        }

        [TestMethod]
        public void TestCreateWithGuidNulablleGuidValue()
        {
            Guid? test = Guid.NewGuid();
            var value = new DDValue(test);
            ValidateGuid(test, value);
        }

        [TestMethod]
        public void TestCreateWithGuidNulablleGuidEmpty()
        {
            Guid? test = Guid.Empty;
            var value = new DDValue(test);
            ValidateGuid(test, value);
        }

        [TestMethod]
        public void TestCreateWithGuidEmptyValue()
        {
            var test = Guid.Empty;
            var a = new DDValue(test);
            ValidateGuid(test, a);
        }

        [TestMethod]
        public void TestCreateWithNewGuid()
        {
            var test = Guid.NewGuid();
            var a = new DDValue(test);
            ValidateGuid(test, a);
        }
        [TestMethod]
        public void TestCreateSetImpicitGuidValue()
        {
            Guid test = Guid.NewGuid();
            var a = new DDValue();
            a = test;
            ValidateGuid(test, a);
        }

        [TestMethod]
        public void TestChangeGuidValue()
        {
            Guid test = Guid.NewGuid();
            var a = new DDValue();
            a = test;
            ValidateGuid(test, a);
            test = Guid.Empty;
            a = test;
            ValidateGuid(test, a);
        }
        [TestMethod]
        public void TestChangeTypeFromGuidToInt()
        {
            var test = Guid.Empty;
            var a = new DDValue();
            a = test;
            ValidateGuid(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestGuidToFromHex()
        {
            var test = Guid.NewGuid();
            var a = new DDValue(test);
            ValidateGuid(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateGuid(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test guid
        #region test guid[]

        [TestMethod]
        public void TestCreateWithEmptyGuidArray()
        {
            Guid[] test = { };
            var a = new DDValue(test);
            ValidateGuidArray(test, a);
        }
        public void TestCreateWithGuidArrayEmptyValue()
        {
            Guid[] test = { Guid.Empty };
            var a = new DDValue(test);
            ValidateGuidArray(test, a);
        }

        [TestMethod]
        public void TestCreateSetImpicitGuidArrayValue()
        {
            Guid[] test = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var a = new DDValue();
            a = test;
            ValidateGuidArray(test, a);
        }

        [TestMethod]
        public void TestChangeTypeFromGuidArrayToInt()
        {
            Guid[] test = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var a = new DDValue();
            a = test;
            ValidateGuidArray(test, a);
            var b = int.MinValue;
            a = b;
            ValidateInt(b, a);
            CommonChangeObjectTypeValidation(a, test);
        }
        [TestMethod]
        public void TestGuidArrayToFromHex()
        {
            Guid[] test = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var a = new DDValue(test);
            ValidateGuidArray(test, a);
            var hexValue = a.GetValueAsHEX();
            var b = new DDValue();
            b.SetHEXValue(test.GetType(), hexValue);
            ValidateGuidArray(test, b);
            Assert.IsTrue(a == b, "The to/from HEX convertion doesn't work.");
            Assert.IsFalse(a.Equals(b), "Equal doesn't work.");
        }
        #endregion test guid[]

        #region ToStringArray
        [TestMethod]
        public void TestNullDataToStringArray()
        {
            var d = new DDValue();
            Assert.IsTrue(CompareStringArray(new string[] { }, d.ToStringArray()), "ToStringArray() should be return null as empty string array");
        }
        [TestMethod]
        public void TestByteArrayDataToStringArray()
        {
            var b = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0xff}; 
            var d = new DDValue(b);
            var s = new string[b.Length];
            for (var i = 0; i < b.Length; i++)
                s[i] = b[i].ToString(DDSchema.StringByteFormat);
            Assert.IsTrue(CompareStringArray(s, d.ToStringArray()), "ToStringArray() should be return byte array as hex string array");
        }
        [TestMethod]
        public void TestBoolDataToStringArray()
        {
            var d = new DDValue(true);
            Assert.IsTrue(CompareStringArray(new string[] { true.ToString() }, d.ToStringArray()), "ToStringArray() should be return bool as true string array");

        }
        #endregion ToStringArray
        #region Convert

        /// <summary>
        /// compares to value and change one of them to validate equals
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        private void CompareDDValue(DDValue v1, DDValue v2)
        {
            Assert.IsTrue(v1 == v2, "Values must be mathematically equal.");
            Assert.AreNotEqual(v1, v2, "Values should not be same.");
            v2 = true; // change type no true
            Assert.IsFalse(v1 == v2, "Changed value should not be equal to the another value.");
        }

        [TestMethod]
        public void TestSelfTransformFromStringTo_FromNotString()
        {
            var v = new DDValue(0);
            try
            {
                v.ConvertTo<object>();
                Assert.Fail("Can transfrom from not string type.");
            }
            catch (DDTypeIncorrectException)
            {/* it's ok*/}
        }
        [TestMethod]
        public void TestSelfTransformFromStringTo_Null()
        {
            var v = new DDValue();
            try
            {
                v.ConvertTo<string>();
                Assert.Fail("Can transfrom from null.");
            }

            catch (DDTypeNullException)
            {/* it's ok*/}
        }
        [TestMethod]
        public void TestSelfTransformFromStringTo_Array()
        {
            var v = new DDValue("0");
            try
            {
                v.ConvertTo<int[]>();
                Assert.Fail("Can transfrom from string to string array.");
            }
            catch (DDTypeIncorrectException)
            {/* it's ok*/}
        }
        [TestMethod]
        public void TestSelfTransformFromStringArrayTo_Bool()
        {
            var v = new DDValue(new[] { "true", "false", "true" });
            try
            {
                v.ConvertTo<bool>();
                Assert.Fail("Can transfrom from string to string array.");
            }
            catch (DDValueConvertException)
            {/* it's ok*/}
        }
        [TestMethod]
        public void TestSelfTransformFromStringTo_Empty()
        {
            var v = new DDValue(String.Empty);
            v.ConvertTo<bool>(); // incorrect data = 0 ToDo
            ValidateBool(false, v);

        }
        [TestMethod]
        public void TestSelfTransformFromStringTo_BoolTrue()
        {
            var v = new DDValue("true");
            v.ConvertTo<bool>();
            ValidateBool(true, v);
        }
        [TestMethod]
        public void TestSelfTransformFromStringTo_BoolFalse()
        {
            var v = new DDValue("false");
            v.ConvertToArray<bool>();
            ValidateBoolArray(new bool[] {false}, v);
        }
        [TestMethod]
        public void TestSelfTransformFromStringArrayTo_BoolArray()
        {
            var v = new DDValue(new[] { "true", "false", "true" });

            v.ConvertToArray<bool>();
            ValidateBoolArray(new bool[] { true, false, true }, v);

        }
        [TestMethod]
        public void TestSelfTransformFromStringArrayTo_ByteArray()
        {
            var v = new DDValue(new[] { "1", "2", "3", "255" });

            v.ConvertToArray<byte>();
            ValidateByteArray(new byte[] { 0x1, 0x2, 0x3 , 0xff}, v);

        }



        #endregion Convert
        #region ValidationType
        private void ValidateDateTime(DateTime dt, DDValue data)
        {
            CommonObjectValidation(dt, data);

            DateTime resImpicit = data;
            Assert.IsTrue(resImpicit == dt, "The implicit DateTime conversion is not matched expected bool.");
            var resGetValue = (DateTime)data.GetValue();
            Assert.IsTrue(resGetValue == dt, "The implicit DateTime conversion is not matched expected text.");
            Assert.IsTrue((DateTime)data == dt, "The implicit int conversion is not matched expected text.");
            Assert.IsTrue(data.GetValueAsDateTime() == dt, "The explicit DateTime conversion is not matched expected text.");
            Assert.IsTrue(data.ToString() == dt.ToString("o"), "The DateTime convert ToString() should me return DateTime in ISO 8601 format.");
        }
        private void ValidateByteArray(byte[] i, DDValue data)
        {
            CommonObjectValidation(i, data);
            byte[] resImpicit = data;
            Assert.IsTrue(CompareByteArray(resImpicit, i), "The implicit byte[]  conversion is not matched expected bool.");
            var resGetValue = (byte[])data.GetValue();
            Assert.IsTrue(CompareByteArray(resGetValue, i), "The implicit byte[]  conversion is not matched expected text.");
            Assert.IsTrue(data == i, "The implicit ByteArray conversion is not matched expected text.");
            Assert.IsTrue(CompareByteArray(data.GetValueAsByteArray(), i), "The explicit ByteArray conversion is not matched expected text.");
            Assert.IsTrue(data.ToString() == DDValue.HEX(i), "The byte[] convert ToString() should me return HEX string.");
        }
        private void ValidateByte(byte i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            byte resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit byte conversion is not matched expected bool.");
            var resGetValue = (byte)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit byte conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit byte conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsByte() == i, "The explicit byte conversion is not matched expected text.");
        }
        private void ValidateUShort(ushort i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            ushort resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit ushort conversion is not matched expected bool.");
            var resGetValue = (ushort)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit ushort conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit ushort conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsUShort() == i, "The explicit ushort conversion is not matched expected text.");
        }
        private void ValidateShort(short i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            short resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit short conversion is not matched expected bool.");
            var resGetValue = (short)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit short conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit short conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsShort() == i, "The explicit short conversion is not matched expected text.");
        }
        private void ValidateULong(ulong i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            ulong resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit ulong conversion is not matched expected bool.");
            var resGetValue = (ulong)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit ulong conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit ulong conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsULong() == i, "The explicit ulong conversion is not matched expected text.");
        }
        private void ValidateLong(long i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            long resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit long conversion is not matched expected bool.");
            var resGetValue = (long)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit long conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit uint conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsLong() == i, "The explicit uint conversion is not matched expected text.");
        }
        private void ValidateFloat(float i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            float resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit float conversion is not matched expected bool.");
            var resGetValue = (float)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit float conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit long conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsFloat() == i, "The explicit long conversion is not matched expected text.");
        }
        private void ValidateDouble(double i, DDValue data)
        {
            CommonObjectValidation(i, data);
            double resImpicit = data;
            Assert.IsTrue(resImpicit == i, "The implicit double conversion is not matched expected double.");
            var resGetValue = (double)data.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit double conversion is not matched expected text.");
            Assert.IsTrue(data == i, "The implicit double conversion is not matched expected text.");
            Assert.IsTrue(data.GetValueAsDouble() == i, "The explicit double conversion is not matched expected text.");
        }
        private void ValidateDecimal(decimal i, DDValue data)
        {
            CommonObjectValidation(i, data);
            decimal resImpicit = data;
            Assert.IsTrue(resImpicit == i, "The implicit double conversion is not matched expected double.");
            var resGetValue = (decimal)data.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit double conversion is not matched expected text.");
            Assert.IsTrue(data == i, "The implicit double conversion is not matched expected text.");
            Assert.IsTrue(data.GetValueAsDecimal() == i, "The explicit double conversion is not matched expected text.");
        }
        private void ValidateUInt(uint i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            uint resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit uint conversion is not matched expected bool.");
            var resGetValue = (uint)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit uint conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit uint conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsUInt() == i, "The explicit uint conversion is not matched expected text.");
        }
        private void ValidateInt(int i, DDValue attr)
        {
            CommonObjectValidation(i, attr);
            int resImpicit = attr;
            Assert.IsTrue(resImpicit == i, "The implicit int conversion is not matched expected bool.");
            var resGetValue = (int)attr.GetValue();
            Assert.IsTrue(resGetValue == i, "The implicit int conversion is not matched expected text.");
            Assert.IsTrue(attr == i, "The implicit int conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsInt() == i, "The explicit int conversion is not matched expected text.");
        }
        private void ValidateChar(char c, DDValue attr)
        {
            CommonObjectValidation(c, attr);
            char resImpicit = attr;
            Assert.IsTrue(resImpicit == c, "The implicit char conversion is not matched expected bool.");
            var resGetValue = (char)attr.GetValue();
            Assert.IsTrue(resGetValue == c, "The implicit char conversion is not matched expected text.");
            Assert.IsTrue(attr == c, "The implicit char conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsChar() == c, "The explicit char conversion is not matched expected text.");
        }
        private void ValidateBool(bool b, DDValue attr)
        {
            CommonObjectValidation(b, attr);
            bool resImpicit = attr;
            Assert.IsTrue(resImpicit == b, "The implicit bool conversion is not matched expected bool.");
            var resGetValue = (bool)attr.GetValue();
            Assert.IsTrue(resGetValue == b, "The implicit boolean conversion is not matched expected text.");
            Assert.IsTrue(attr == b, "The implicit boolean conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsBool() == b, "The explicit boolean conversion is not matched expected text.");
        }

        private void ValidateNull(DDValue value)
        {
            Assert.IsNotNull(value, "The value can not be null.");
            Assert.IsNull(value.GetValue(), "The value is not null.");
            Assert.IsTrue(value.Type == null, "The type is not null");
            Assert.IsTrue(value.Size == 0, "The size is not '0'");
        }

        private void ValidateGuid(Guid? g, DDValue value)
        {
            if (g == null)
                ValidateNull(value);
            else
            {
                CommonObjectValidation(g, value);
                Guid resImpicit = value;
                Assert.IsTrue(resImpicit == g, "The implicit Guid conversion is not matched expected bool.");
                var resGetValue = (Guid)value.GetValue();
                Assert.IsTrue(resGetValue == g, "The implicit Guid conversion is not matched expected text.");
                Assert.IsTrue((Guid)value == g, "The implicit byte conversion is not matched expected text.");
                Assert.IsTrue(value.GetValueAsGuid() == g, "The explicit byte conversion is not matched expected text.");
                Assert.IsTrue(value.GetValueAs<Guid>() == g, "The explicit byte conversion is not matched expected text.");
                Assert.IsTrue(value.GetValueAs<Guid?>() == g, "The explicit byte conversion is not matched expected text.");
            }
        }
        private void ValidateStringArray(string[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            string[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, "The implicit text[] conversion is not matched expected text[].");
            Assert.IsTrue(CompareStringArray(data.GetValueAsStringArray(), array), "The explicit text [] conversion is not matched expected text[].");
            Assert.IsTrue(data.ToString() == string.Join("\0", array), "The ToString() text[] conversion is not matched expected text.");
            Assert.IsTrue(CompareStringArray(data.ToStringArray(), array), "The explicit ToStringArray() conversion is not matched expected text[].");
        }
        private void ValidateCharArray(char[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            char[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (char[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<char>(data.GetValueAsCharArray(), array), "The explicit char[] conversion is not matched expected char[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() char[] conversion is not matched expected text.");
        }
        private void ValidateBoolArray(bool[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            bool[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (bool[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<bool>(data.GetValueAsBoolArray(), array), "The explicit bool[] conversion is not matched expected bool[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() bool[] conversion is not matched expected text.");

        }
        private void ValidateGuidArray(Guid[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            Guid[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (Guid[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<Guid>(data.GetValueAsGuidArray(), array), "The explicit Guid[] conversion is not matched expected bool[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() Guid[] conversion is not matched expected text.");

        }
        private void ValidateIntArray(int[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            int[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (int[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<int>(data.GetValueAsIntArray(), array), "The explicit int[] conversion is not matched expected int[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() int[] conversion is not matched expected text.");
        }
        private void ValidateUIntArray(uint[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            uint[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (uint[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<uint>(data.GetValueAsUIntArray(), array), "The explicit uint[] conversion is not matched expected uint[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() uint[] conversion is not matched expected text.");
        }
        private void ValidateDateTimeArray(DateTime[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            DateTime[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (DateTime[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<DateTime>(data.GetValueAsDateTimeArray(), array), "The explicit DateTime[] conversion is not matched expected DateTime[].");
            Assert.IsTrue(data.ToString() == array.ToString(), "The ToString() DateTime[] conversion is not matched expected text.");
        }
        private void ValidateShortArray(short[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            short[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (short[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<short>(data.GetValueAsShortArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateUShortArray(ushort[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            ushort[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (ushort[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<ushort>(data.GetValueAsUShortArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateLongArray(long[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            long[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (long[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<long>(data.GetValueAsLongArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateULongArray(ulong[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            ulong[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (ulong[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<ulong>(data.GetValueAsULongArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateFloatArray(float[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            float[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (float[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<float>(data.GetValueAsFloatArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateDecimalArray(decimal[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            decimal[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (decimal[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<decimal>(data.GetValueAsDecimalArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        private void ValidateDoubleArray(double[] array, DDValue data)
        {
            CommonObjectValidation(array, data);
            double[] resImpicit = data;
            Assert.IsTrue(resImpicit == data, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data == array, string.Format("The implicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            var resGetValue = (double[])data.GetValue();
            Assert.IsTrue(resGetValue == data, string.Format("The implicit '{0}' conversion is not matched expected text.", array.GetType().Name));
            Assert.IsTrue(CompareArray<double>(data.GetValueAsDoubleArray(), array), String.Format("The explicit '{0}' conversion is not matched expected '{0}'.", array.GetType().Name));
            Assert.IsTrue(data.ToString() == array.ToString(), String.Format("The ToString() '{0}' conversion is not matched expected text.", array.GetType().Name));
        }
        #region CompareArray
        private bool CompareArray<T>(T[] a, T[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i])) return false;
            }
            return true;
        }

        private bool CompareByteArray(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private bool CompareSByteArray(sbyte[] a, sbyte[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private bool CompareStringArray(string[] a, string[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
        #endregion CompareArray
        private void ValidateString(string text, DDValue attr)
        {
            CommonObjectValidation(text, attr);
            string resImpicit = attr;
            Assert.IsTrue(resImpicit == text, "The implicit text conversion is not matched expected text.");
            Assert.IsTrue(attr == text, "The implicit text conversion is not matched expected text.");
            Assert.IsTrue(attr.GetValueAsString() == text, "The explicit text conversion is not matched expected text.");
            Assert.IsTrue(attr.ToString() == text, "The ToString() text conversion is not matched expected text.");
        }
        //private void ValidateEnum(Enum text, DDValue value)
        //{
        //    CommonObjectValidation(text, value);
        //    Enum resImpicit = value;
        //    Assert.IsTrue(resImpicit == text, "The implicit enum conversion is not matched expected enum.");
        //    Assert.IsTrue(value == text, "The implicit enum conversion is not matched expected enum.");
        //    Assert.IsTrue(value.GetValueAsEnum() == text, "The explicit enum conversion is not matched expected enum.");
        //    Assert.IsTrue(value.ToString() == text.ToString(), "The ToString() enum conversion is not matched expected enum.");
        //    Assert.IsTrue(value.GetValueAsEnum().GetTypeCode() == text.GetTypeCode(), "The GetTypeCode() enum conversion is not matched expected enum.");
        //}
        private void CommonObjectValidation(object obj, DDValue attr)
        {
            Assert.IsTrue(attr.Type == obj.GetType(), "The object type is incorrect");
            Assert.IsTrue(attr.Size == DDValue.GetObjSize(obj), string.Format("The object size '{0}' is incorrect. Expected size '{1}'.", attr.Size, DDValue.GetObjSize(obj)));
        }
        private void CommonChangeObjecValueValidation(DDValue a1, DDValue a2)
        {
            Assert.IsFalse(a1 == a2, "Object data is ==");
            Assert.IsFalse(a1.Equals(a2), "Object a1 is equal a2");
            Assert.IsFalse(DDValue.Compare(a1, a2) == 0, "Object a1 is Compare a2");
            Assert.IsFalse(a1.CompareTo(a2) == 0, "Object a1 is CompareTo a2");
        }
        private void CommonChangeObjectTypeValidation(DDValue a1, DDValue a2)
        {
            Assert.IsFalse(a1.Type == a2.Type, "The object type is equals.");
            Assert.IsFalse(a1 == a2, "Object data is ==");
            Assert.IsFalse(a1.Equals(a2), "Object a1 is equal a2");
            Assert.IsFalse(DDValue.Compare(a1, a2) == 0, "Object a1 is Compare a2");
            Assert.IsFalse(a1.CompareTo(a2) == 0, "Object a1 is CompareTo a2");
        }
        #endregion ValidationType

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
            var deserialized = (DDValue)DeserializeItem(stream, iFormatter);
            ValidateDeserialization(original, deserialized);
        }

        private void ValidateDeserialization(DDValue original, DDValue deserialized)
        {
            Assert.IsTrue(original == deserialized, "Deserialized object must be mathematically equal to the original object.");
            Assert.AreNotEqual(original, deserialized, "Deserialized object should not be same as original object.");
            deserialized = true; // change type no true
            Assert.IsFalse(original == deserialized, "Changed deserialized object should not be equal to the original object.");
        }
        #endregion ISerializable
    }
}