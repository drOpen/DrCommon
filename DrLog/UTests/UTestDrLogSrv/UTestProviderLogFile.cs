using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrLog.DrLogSrv.Providers;
using DrOpen.DrData.DrDataObject;
using DrOpen.DrCommon.DrLog.DrLogClient;


namespace UTestDrLogSrv
{
    [TestClass]
    public class UTestProviderLogFile
    {
        [TestMethod]
        [Ignore]
        public void TestMethod1()
        {
            var logFile = new LogFile(new DDNode(new DDType (typeof(LogFile).Name)), true);
            logFile.Write(Logger.MessageItem(DateTime.Now, LogLevel.INF , "Source", null, null, "Start", null , null));
            try
            {
                throw new DivideByZeroException("Divide by zero");
            }
            catch (Exception e)
            {
                logFile.Write(Logger.MessageItem(DateTime.Now, DrOpen.DrCommon.DrLog.DrLogClient.LogLevel.ERR, "source", null, new Exception("ExceptionBody", e), "body", null, null));
            }
            logFile.Write(Logger.MessageItem(DateTime.Now, LogLevel.TRC, "Source", null, null, "Trace", null, null));
            logFile.Write(Logger.MessageItem(DateTime.Now, LogLevel.DBG, "Source", null, null, "Debug", null, null));
            logFile.Write(Logger.MessageItem(DateTime.Now, LogLevel.INF, "Source", null, null, "Stop", null, null));
        }
    }
}
