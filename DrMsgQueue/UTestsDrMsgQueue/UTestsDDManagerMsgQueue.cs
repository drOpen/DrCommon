using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrData.DrDataObject ;
using DrOpen.DrCommon.DrMsgQueue;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace UTestsDrMsgQueue
{
    [TestClass]
    public class UTestsDDManagerMsgQueue
    {
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine("Start test");
            var ddQ = new DDManagerMsgQueue();
            for (int i = 1; i < 1000; i++)
            {
                ddQ.Put(new DDNode());
            }

        }
    }
}
