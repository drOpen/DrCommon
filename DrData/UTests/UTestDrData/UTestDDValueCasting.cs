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
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [TestCategory(CLASS_CATEGORY)]
        public void TestCasting2StringFromInt()
        {
            string[] ss = new string[] { "-1", "0", "1" };  

            var vv = new DDValue(ss);
            var s = vv.GetValueAsIntArray();
            
            var arr = new int[] { Int32.MinValue, -1, 0, 1, Int32.MaxValue };

            var r = new DDValue(arr).GetValueAsArray<bool>();


            foreach (var exp in arr)
            {
                var v = new DDValue(exp);
                var u = v.ToStringArray();
                a(v);
                CheckCasting2String(v, exp.ToString());
            }
        }


        private void a(params object[] p)
        {
            var r = String.Format("{0}", p);
        }

        private void CheckCasting2String(DDValue v,  string exp)
        {
            var r = v.ToString();
            Assert.AreEqual(exp, r, "The casting from {0} to {1} doesn't work properly. Unfortunately the result {2} doesn't much expectation value {3}.",
                                    v.Type.Name, exp.GetType().Name, r, exp);
            Assert.IsInstanceOfType(r, typeof(string)); 
        }

        #endregion 2 string
    }
}
