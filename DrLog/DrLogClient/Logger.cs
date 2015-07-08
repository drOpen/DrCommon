using System;
using System.Diagnostics;
using DrOpen.DrCommon.DrData;
using DrLogClient.Res;

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
    /// <summary>
    /// Create client logging messages
    /// </summary>
    public class Logger
    {

        #region Singleton
        /// <summary>
        /// Stored Logger instance
        /// </summary>
        private static volatile Logger stInstance;
        /// <summary>
        /// lock object for creating Logger
        /// </summary>
        private static object lockLogger = new Object();
        static Logger()
        {
        }

        Logger()
        {
            this.LogFullSourceName = false;
            this.LogThreadName = true;
            // Source FullName for current class
            currentSourceFullName = new StackTrace().GetFrame(0).GetMethod().ReflectedType.FullName;
        }

        /// <summary>
        /// returns existing instance or create new Logger instance
        /// </summary>
        public static Logger GetInstance
        {
            get
            {
                if (stInstance == null)
                {
                    lock (lockLogger)
                    {
                        if (stInstance == null) stInstance = new Logger(); // double check 'stInstance' must have
                    }
                }
                return stInstance;
            }
        }

        #endregion Singleton
        /// <summary>
        /// Source FullName for current class
        /// </summary>
        private readonly string currentSourceFullName = string.Empty;
        /// <summary>
        /// Log FullSourceName: Logger or DrOpen.DrCommon.DrLog.DrLogClient.Logger<para> </para>
        /// By default - false;
        /// </summary>
        public bool LogFullSourceName { get; set; }
        /// <summary>
        /// Log current thread name: Logger or DrOpen.DrCommon.DrLog.DrLogClient.Logger<para> </para>
        ///  By default - true;
        /// </summary>
        public bool LogThreadName { get; set; }

        #region Buildsource

        private string GetSource()
        {
            if (LogThreadName)
                return GetThreadInfo() + ">>" + GetSourceNameFromStack();
            else
                return GetSourceNameFromStack();
        }

        private string GetSourceNameFromStack()
        {
            var frames = new StackTrace().GetFrames();
            string strName = string.Empty;
            if (frames != null)
                foreach (var frame in frames)
                {
                    var type = frame.GetMethod().ReflectedType;
                    if ((type.FullName != null) && (type.FullName.Equals(currentSourceFullName) == false))
                    {
                        strName = LogFullSourceName ? type.FullName : type.UnderlyingSystemType.Name;
                        return strName + "::" + frame.GetMethod().Name;
                    }
                }
            return strName;
        }



        private string GetThreadInfo()
        {
            if (System.Threading.Thread.CurrentThread.Name != null)
                return System.Threading.Thread.CurrentThread.Name.Trim() + "(" + System.Threading.Thread.CurrentThread.ManagedThreadId + ")";
            else
                return String.Empty;
        }
        #endregion Buildsource

        public static DDNode MessageItem(DateTime createdDateTime, LogLevel logLevel, string source, Exception exception, string body, string[] providers, string[] recipients)
        {
            var node = new DDNode(new DDType(DrLogClientConst.MessageType)) {Type = DrLogClientConst.MessageType};

            if (exception != null) node.Add(exception); // add exception
            node.Attributes.Add(MessageAttributes.LogLevel, logLevel.ToString());
            if (!string.IsNullOrEmpty(body)) node.Attributes.Add(MessageAttributes.Body, body);
            if (!string.IsNullOrEmpty(source)) node.Attributes.Add(MessageAttributes.Source, source);
            if ((providers != null) && (providers.Length > 0)) node.Attributes.Add(MessageAttributes.Providers, providers);
            if ((recipients != null) && (recipients.Length > 0)) node.Attributes.Add(MessageAttributes.Recipients, recipients);
            return node;
        }

        #region write

        /// <summary>
        /// Send MessageItem to transport for logging
        /// </summary>
        /// <param name="msg">message</param>
        public virtual void Write(DDNode msg)
        {
            try
            {
               
            }
            catch (Exception e)
            {
               
            }
        }

        /// <summary>
        /// Build MessageItem as DDNode and send to LogSrv across PipeTransport.<para> </para>
        /// Exposes the current time for this message. Defines the source name.<para> </para>
        /// Also replaces the format item in a specified body with the string representation of a corresponding object in a specified bodyArgs.
        /// </summary>
        /// <param name="logLevel">associated log level for current log message, which identifies how important/detailed the message is</param>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="providers">The list of providers who will be read this message. by default all providers</param>
        /// <param name="recipients">The list of recipients who will be receive this message. by default all recipients</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void Write(LogLevel logLevel, Exception exception, string[] providers, string[] recipients, string body, params object[] bodyArgs)
        {
            try
            {
                body = ((bodyArgs != null) && (bodyArgs.Length != 0) ? string.Format(body, bodyArgs) : body);
            }
            catch (Exception e)
            {
                var bodyF = body ?? "null";
                WriteError(e, Msg.CANNOT_BUILD_MSG_BODY , bodyF, Args2String(bodyArgs)); 
            }
            Write(MessageItem(DateTime.Now, logLevel, GetSource(), exception, body, providers, recipients));
        }

        /// <summary>
        /// Convert arguments string array to string like "'param1', 'empty', 'null'"
        /// <remarks>Return "null" for null arguments and "empty" for 0 arguments</remarks> 
        /// </summary>
        /// <param name="args">arguments to convert</param>
        /// <returns></returns>
        public static string Args2String(params object[] args)
        {
            if (args==null) return "null";
            if (args.Length ==0) return "empty";
            string res = String.Empty;
            foreach (var item in args)
            {
                if (res.Length != 0) res += ", ";
                var itemF = (item == null) ? "null" : item.ToString();
                if (itemF.Length == 0) itemF = "empty";
                res += "'" + itemF + "'";
            }
            return res;
        }

        /// <summary>
        /// Build MessageItem as DDNode and send to LogSrv across PipeTransport.<para> </para>
        /// Exposes the current time for this message. Defines the source name.<para> </para>
        /// Also replaces the format item in a specified body with the string representation of a corresponding object in a specified bodyArgs.
        /// </summary>
        /// <param name="logLevel">associated log level for current log message, which identifies how important/detailed the message is</param>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void Write(LogLevel logLevel, Exception exception, string body, params object[] bodyArgs)
        {
            Write(logLevel, exception, null, null, body, bodyArgs);
        }

        #region WriteError
        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteError(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.ERR, exception, body, bodyArgs);
        }
        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteError(string body, params object[] bodyArgs)
        {
            Write(LogLevel.ERR, null, body, bodyArgs);
        }
        #endregion WriteError
        #region WriteWarning
        /// <summary>
        /// Write warning message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteWarning(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.WAR, exception, body, bodyArgs);
        }
        /// <summary>
        /// Write warning message
        /// </summary>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteWarning(string body, params object[] bodyArgs)
        {
            Write(LogLevel.WAR, null, body, bodyArgs);
        }
        #endregion WriteWarning
        #region WriteInfo
        /// <summary>
        /// Write information message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteInfo(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.INF, exception, body, bodyArgs);
        }
        /// <summary>
        /// Write information message
        /// </summary>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteInfo(string body, params object[] bodyArgs)
        {
            Write(LogLevel.INF, null, body, bodyArgs);
        }
        #endregion WriteInfo
        #region WriteTrace
        /// <summary>
        /// Write trace message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteTrace(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.TRC, exception, body, bodyArgs);
        }
        /// <summary>
        /// Write trace message
        /// </summary>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteTrace(string body, params object[] bodyArgs)
        {
            Write(LogLevel.TRC, null, body, bodyArgs);
        }
        #endregion WriteTrace
        #region WriteDebug
        /// <summary>
        /// Write debug message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteDebug(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.DBG, exception, body, bodyArgs);
        }
        /// <summary>
        /// Write debug message
        /// </summary>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects to format body. </param>
        public virtual void WriteDebug(string body, params object[] bodyArgs)
        {
            Write(LogLevel.DBG, null, body, bodyArgs);
        }
        #endregion WriteDebug
        #endregion write
    }
}
