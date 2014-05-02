/*
  UTestDDNodeST.cs -- Unit Tests of Single Tone for DDNode for 'DrData' general purpose Data abstraction layer 1.0.1, January 5, 2014
 
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

using DrOpen.DrCommon.DrData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UTestDrData
{
    [TestClass]
    public class UTestDDNodeST: UTestDDNode
    {

        enum TEST_ST
        {
            TEST_ST_NAME_DATA,
            TEST_ST_NAME_CONFIG
            
        }

        private DDNode GetSTConfigNode()
        {
            var node = DDNodeST.GetInstance("Config");
            if (node.Attributes.Contains("Param1") == false) node.Attributes.Add("Param1", "Param1_Valu1");
            if (node.Attributes.Contains("Param2") == false) node.Attributes.Add("Param2", "Param2_Valu1");
            return node;
        }

        private DDNode GetSTDataNode()
        {
            var node = DDNodeST.GetInstance(TEST_ST.TEST_ST_NAME_DATA);
            if (node.Attributes.Contains("Data1") == false) node.Attributes.Add("Data1", "Data1_Valu1");
            if (node.Attributes.Contains("Data2") == false) node.Attributes.Add("Data2", "Data2_Valu1");
            return node;
        }
        private DDNode GetSTRootNode()
        {
            return DDNodeST.GetInstance();
        }

        [TestMethod]
        public void TestGetNodeByInstance()
        {
            var configA = GetSTConfigNode();
            var configB = DDNodeST.GetInstance("Config");
            CompareSingleTone(configA, configB);

        }

        [TestMethod]
        public void TestGetNodeByInstanceAfterModify()
        {
            var configA = GetSTConfigNode();
            var configB = DDNodeST.GetInstance("Config");
            CompareSingleTone(configA, configB);
            configA.Attributes.Add(new DDValue(true));
            CompareSingleTone(configA, configB);
        }

        [TestMethod]
        public void TestGeneralParent()
        {
            var configA = GetSTConfigNode();
            var configB = DDNodeST.GetInstance("Config");
            var configC = GetSTDataNode();

            CompareSingleTone(configA.Parent, configB.Parent);
            CompareSingleTone(configA.Parent, configC.Parent);
        }

        private void GetInstance(object sleep)
        {
            Thread.Sleep((int)sleep);
            for (int i = 0; i < 1000; i++)
            {
                DDNodeST.GetInstance();
                DDNodeST.GetInstance(i.ToString());
                DDNodeST.GetInstance(TEST_ST.TEST_ST_NAME_DATA);
            }

        }

        [TestMethod]
        public void TestMultithreadingInstanceAccessTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var th = new Thread(new ParameterizedThreadStart(GetInstance));
                th.Start(1000-i);
            }

        }

        public static void CompareSingleTone(DDNode a, DDNode b)
        {
            Assert.IsTrue(a == b, "SingleTone objects must be mathematically equal to the original object.");
            Assert.IsTrue(a.Equals(b), "SingleTone objects must be equals.");
        }

    }
}
