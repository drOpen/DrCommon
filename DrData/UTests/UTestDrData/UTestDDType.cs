/*
  UTestDDType.cs -- Unit Tests of Type for DDNode for 'DrData' general purpose Data abstraction layer 1.0.1, July 19, 2015
 
  Copyright (c) 2013-2015 Kudryashov Andrey aka Dr
 
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

using DrOpen.DrCommon.DrData;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTestDrData
{
    [TestClass]
    public class UTestDDType
    {
        [TestMethod]
        public void TestCreateType()
        {
            var expected ="type";
            var ddType = new DDType(expected);
            Assert.IsTrue(ddType == expected, "The type is incorrect.");
        }

        [TestMethod]
        public void TestTypeImplicitConvertToString()
        {
            var expected = "tYpE";
            var ddType = new DDType(expected);
            Assert.IsTrue(ddType == (DDType)expected, "Implicit convertion from string is incorrect.");
            Assert.IsTrue(expected == (string)ddType, "Implicit convertion to string is incorrect.");
        }

        [TestMethod]
        public void TestTypeEqual()
        {
            var expected = "tYpE";
            var ddType1 = new DDType(expected);
            var ddType2 = ddType1 ;
            Assert.IsTrue(ddType1.Equals(ddType2), "Objects must be equals.");
            Assert.IsTrue(ddType2.Equals(ddType1), "Objects must be equals.");
        }

        [TestMethod]
        public void TestTypeValueEqual()
        {
            var expected = "tYpE";
            var ddType1 = new DDType(expected);
            var ddType2 = new DDType(expected);
            Assert.AreEqual(ddType1, ddType2);
            Assert.IsTrue(ddType2.Equals(ddType2), "Objects have to equal.");
            Assert.IsTrue(ddType2.Equals(ddType1), "Objects have to equal.");
        }
        /// <summary>
        /// check fix StackOverflow issue
        /// </summary>
        [TestMethod]
        public void TestTypeNotEqualByObject()
        {
            var expected = "type";
            var ddType1 = new DDType(expected);
            var ddType2 = new DDType(expected);

            Assert.AreEqual(ddType1, ddType2, "Objects have to be equal.");
        }
        /// <summary>
        /// check fix StackOverflow issue
        /// </summary>
        [TestMethod]
        public void TestTypeEqualByObject()
        {
            var expected = "type";
            var ddType1 = new DDType(expected);

            Assert.AreEqual(ddType1, (object)ddType1, "Objects should be equals.");
        }
        /// <summary>
        /// DDType is working fine as a key in the Dictonaryh or hash
        /// </summary>
        [TestMethod]
        public void TestDDTypeEqualIsValuable()
        {
            var d = new System.Collections.Generic.Dictionary<DDType, DDType>();
            var t0 = new DDType("");
            var t1 = new DDType("t1");
            var t2 = new DDType("t2");
            var t3 = new DDType("T2");

            d.Add(t0, t0);
            d.Add(t1, t1);
            d.Add(t2, t2);
            d.Add("T3", "T3");

            Assert.AreEqual(d[t0], t0);
            Assert.AreEqual(d[t1], t1);
            Assert.AreEqual(d[t2], t2);
            Assert.AreEqual(d["T3"], new DDType("T3"));
        }
    }
}