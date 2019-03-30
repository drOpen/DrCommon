/*
  UTestDDValueCasting.cs -- Unit Tests of Casting DDValue for 'DrData' general purpose Data abstraction layer 1.0.1, March 24, 2019
 
  Copyright (c) 2013-2019 Kudryashov Andrey aka Dr
 
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

      Kudryashov Andrey <kudryashov dot andrey at gmail dot com>

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
    [TestClass]
    public class UTestDDValueCasting
    {

        private const string TEST_CATEGORY = "Value Casting";
        private const string CLASS_CATEGORY = "DDValue";

        #region 2 string
        [TestMethod, TestCategory(TEST_CATEGORY), TestCategory(CLASS_CATEGORY)]
        public void TestCastingNull()
        {
            string[] src = new string[] { null, null, null };

            var trg = new int[src.Length]; 
            CheckArrayCast2Array<string, int>(src, trg);
        }

        [TestMethod, TestCategory(TEST_CATEGORY), TestCategory(CLASS_CATEGORY)]
        public void TestCastingInt2String()
        {
            int[] src = new int[] { int.MinValue, 0, int.MaxValue };
            var trg = ConvertArray2StringArray<int>(src);
            CheckItemCast2Array<int, string>(src, trg);
            CheckItemCast2Array<string, int>(trg, src);
        }

        #endregion 2 string

        #region 2 string array
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayInt2String2Int()
        {
            var src = new int[] { Int32.MaxValue, -1, 0, 1, Int32.MaxValue };
            var trg = ConvertArray2StringArray<int>(src);
            CheckArrayCast2Array<int, string>(src, trg);
            CheckItemCast2Array<int, string>(src, trg);
            CheckArrayCast2Array<string, int>(trg, src);
            CheckItemCast2Array<string, int>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayUInt2String2UInt()
        {
            var src = new uint[] { UInt32.MinValue,  0, 1, UInt32.MaxValue };
            var trg = ConvertArray2StringArray<uint>(src);
            CheckArrayCast2Array<uint, string>(src, trg);
            CheckItemCast2Array<uint, string>(src, trg);
            CheckArrayCast2Array<string, uint>(trg, src);
            CheckItemCast2Array<string, uint>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayBool2String2Bool()
        {
            var src = new bool[] { false, true, false };
            var trg = ConvertArray2StringArray<bool>(src);
            CheckArrayCast2Array<bool, string>(src, trg);
            CheckItemCast2Array<bool, string>(src, trg);
            CheckArrayCast2Array<string, bool>(trg, src);
            CheckItemCast2Array<string, bool>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayGuid2String2Guid()
        {
            var src = new Guid[] { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
            var trg = ConvertArray2StringArray<Guid>(src);
            CheckArrayCast2Array<Guid, string>(src, trg);
            CheckItemCast2Array<Guid, string>(src, trg);
            CheckArrayCast2Array<string, Guid>(trg, src);
            CheckItemCast2Array<string, Guid>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayDateTime2String2DateTime()
        {
            var src = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.UtcNow, DateTime.MaxValue };
            var trg = new string[src.Length]; 
            for (var i = 0; i < src.Length; i++)
                trg[i] = src[i].ToString("o");
            CheckArrayCast2Array<DateTime, string>(src, trg);
            CheckItemCast2Array<DateTime, string>(src, trg);
            CheckArrayCast2Array<string , DateTime>(trg, src);
            CheckItemCast2Array<string , DateTime>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayByte2String2Byte()
        {
            var src = new byte[] { Byte.MinValue, 0, Byte.MaxValue };
            var trg = ConvertArray2StringArray<byte>(src);
            CheckArrayCast2Array<byte, string>(src, trg);
            CheckArrayCast2Array<byte, string>(src, trg);
            CheckItemCast2Array<string, byte>(trg, src);
            CheckItemCast2Array<string, byte>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayChar2String2Char()
        {
            var src = new char[254];
            for (int i = 0; i < 254; i++)
                src[i] = (char) (i + 1);

            var trg = ConvertArray2StringArray<char>(src);
            CheckArrayCast2Array<char, string>(src, trg);
            CheckItemCast2Array<char, string>(src, trg);
            CheckArrayCast2Array<string, char>(trg, src);
            CheckItemCast2Array<string, char>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayShort2String2Short()
        {
            var src = new short[] { short.MinValue, 0, short.MaxValue};
            var trg = ConvertArray2StringArray<short>(src);
            CheckArrayCast2Array<short, string>(src, trg);
            CheckItemCast2Array<short, string>(src, trg);
            CheckArrayCast2Array<string, short>(trg, src);
            CheckItemCast2Array<string, short>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayUShort2String2UShort()
        {
            var src = new ushort[] { ushort.MinValue, 0, ushort.MaxValue};
            var trg = ConvertArray2StringArray<ushort>(src);
            CheckArrayCast2Array<ushort, string>(src, trg);
            CheckItemCast2Array<ushort, string>(src, trg);
            CheckArrayCast2Array<string, ushort>(trg, src);
            CheckItemCast2Array<string, ushort>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayLong2String2Long()
        {
            var src = new long[] { long.MinValue, 0, long.MaxValue};
            var trg = ConvertArray2StringArray<long>(src);
            CheckArrayCast2Array<long, string>(src, trg);
            CheckItemCast2Array<long, string>(src, trg);
            CheckArrayCast2Array<string, long>(trg,src);
            CheckItemCast2Array<string, long>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayULong2String2ULong()
        {
            var src = new ulong[] { ulong.MinValue, 0, ulong.MaxValue};
            var trg = ConvertArray2StringArray<ulong>(src);
            CheckArrayCast2Array<ulong, string>(src, trg);
            CheckItemCast2Array<ulong, string>(src, trg);
            CheckArrayCast2Array<string, ulong>(trg, src);
            CheckItemCast2Array<string, ulong>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayFloat2String2Float()
        {
            var src = new float[] { float.MinValue, 0, float.MaxValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon};
            var trg = new string[src.Length]; 
            for (var i = 0; i < src.Length; i++)
                trg[i] = src[i].ToString("r");
            CheckArrayCast2Array<float, string>(src, trg);
            CheckItemCast2Array<float, string>(src, trg);
            CheckArrayCast2Array<string , float>(trg, src);
            CheckItemCast2Array<string, float>(trg, src);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCastingArrayDouble2String2Double()
        {
            var src = new double[] {double.Epsilon, double.MaxValue, double.MinValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity, 0};
            var trg = new string[src.Length]; 
            for (var i = 0; i < src.Length; i++)
                trg[i] = src[i].ToString("r");
            CheckArrayCast2Array<double, string>(src, trg);
            CheckItemCast2Array<double, string>(src, trg);
            CheckArrayCast2Array<string, double>(trg, src);
            CheckItemCast2Array<string, double>(trg, src);
        }

        private void CheckItemCast2Array<S, T>(S[] src, T[] trg) 
        {
            for (var i = 0; i < src.Length; i++)
            {
                var v = new DDValue(src[i]);
                var res = v.GetValueAsArray<T>();
                Assert.IsTrue(res.Length == 1, "There are '{0}' elements in the array. Expected {1}.", res.Length, 1);
                Assert.AreEqual(res[0].ToString(), trg[i].ToString(),
                    "The element of string array '{0}' doesn't much 'DDValue.ToString()' result '{1}'. The first element must be equal 'DDValue.ToString()' result.", res[0].ToString(), trg[i].ToString());
                Assert.AreEqual(res[0], trg[i],
                    "The element value of result '{0}' doesn't equal '{1}'. The value of the first element must be equal '{1}'.", res[0], trg[i]);
                var cast = (T)(new DDValue(src[i]).GetValueAs<T>() );
                Assert.AreEqual(cast, trg[i],
                    "Value '{0} is not equal to '{1}' after casting from '{2}' to '{3}'.", cast.ToString(), trg[i].ToString(), typeof(S).Name, typeof(T).Name);
            }
        }

        private string[] ConvertArray2StringArray<S>(S[] src)
        {
            var res = new string[src.Length];
            for (var i = 0; i < src.Length; i++)
                res[i] = src[i].ToString();
            return res;
        }

        private void CheckArrayCast2Array<S, T>(S[] src, T[] trg)
        {
            var v = new DDValue(src);
            var res = v.GetValueAsArray<T>();

            Assert.IsTrue(res.Length == trg.Length, "There are '{0}' elements in the array. Expected '{1}'.", res.Length, trg.Length);
            for (var i = 0; i < trg.Length; i++)
            {
                Assert.AreEqual<T>(res[i], trg[i], "The element value of result array '{0}' doesn't equal expected value '{1}'.", res[i], trg[i]);
            }
        }
        #endregion 2 string array

    }
}
