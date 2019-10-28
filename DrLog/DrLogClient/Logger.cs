/*
  Logger.cs -- client for DrLog 1.0.0, August 30, 2015
 
  Copyright (c) 2013-2017 Kudryashov Andrey aka Dr
 
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
using System.Diagnostics;
using DrOpen.DrData.DrDataObject;
using DrOpen.DrCommon.DrLog.DrLogClient.Res;

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
    /// <summary>
    /// Instance of client logging messages
    /// </summary>
    public abstract class Logger : ILogger
    {
        /// <summary>
        /// Returns instance of logger with message filter <paramref name="LogLevel.TRACE"/>, log short name of source and log thread name
        /// </summary>
        public Logger(): this(LogLevel.TRACE, false, true)
        {  }

        /// <summary>
        /// Returns instance of logger
        /// </summary>
        /// <param name="filterMsg">set message filter by log level</param>
        /// <param name="logFullSourceName">log full source name with name space</param>
        /// <param name="logThreadName">log thread name</param>
        public Logger(LogLevel filterMsg, bool logFullSourceName, bool logThreadName)
        {
            this.FilterMsg = filterMsg;
            this.LogFullSourceName = logFullSourceName;
            this.LogThreadName = logThreadName;
            // Source FullName for current class
            currentSourceFullName = getCurrentSourceFullName();
        }

        /// <summary>
        /// Gets the fully qualified name of the System.Type, including the namespace of the System.Type but not the assembly fro current class
        /// </summary>
        /// <returns></returns>
        private string getCurrentSourceFullName()
        {
            string res = String.Empty;
            try
            {
                res = new StackTrace().GetFrame(0).GetMethod().ReflectedType.FullName;
            }
            catch { }
            return res;
        }

        /// <summary>
        /// filter messages by their level
        /// </summary>
        public LogLevel FilterMsg { get; set; }
        /// <summary>
        /// Source FullName for current class
        /// </summary>
        private readonly string currentSourceFullName = string.Empty;
        /// <summary>
        /// Specify true for log source name with namespace as 'DrOpen.DrCommon.DrLog.DrLogClient.Logger', otherwise log short name as 'Logger', by default - false
        /// </summary>
        public bool LogFullSourceName { get; set; }
        /// <summary>
        /// Specify true for log current thread name with thread id as [112::main]. By default, it's true
        /// </summary>
        public bool LogThreadName { get; set; }

        #region Buildsource
        /// <summary>
        /// Concatinates source 
        /// </summary>
        /// <returns></returns>
        private string GetSource()
        {
            if (LogThreadName)
                return GetThreadInfo() + GetSourceNameFromStack();
            else
                return GetSourceNameFromStack();
        }
        /// <summary>
        /// Returns source name by stack
        /// </summary>
        /// <returns></returns>
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
          /// <summary>
          /// Returns thread name if it is specified, otherwise returns thread id
          /// </summary>
          /// <returns></returns>
        private string GetThreadInfo()
        {
            if (System.Threading.Thread.CurrentThread.Name != null)
                return  System.Threading.Thread.CurrentThread.Name.Trim();
            else
                return System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() ;
        }
        #endregion Buildsource

        /// <summary>
        /// returns message as DDNode with basic attributes
        /// </summary>
        /// <param name="createdDateTime">message creation date time</param>
        /// <param name="msgLevel">level of message</param>
        /// <param name="source">source of messages</param>
        /// <param name="threadInfo">information about thread: id or name</param>
        /// <param name="exception">exception with inner exception. This attribute support null for message without exception</param>
        /// <param name="body">body of message</param>
        /// <param name="providers">array of supported providers. All connected providers will be get this message if array is empty</param>
        /// <param name="recipients">array of supported recipients. All connected recipients will be get this message if array is empty</param>
        /// <returns></returns>
        public static DDNode MessageItem(DateTime createdDateTime, LogLevel msgLevel, string source, string threadInfo, Exception exception, string body, string[] providers, string[] recipients)
        {
            var node = new DDNode(new DDType(SchemaMsg.MessageType));

            if (exception != null) node.Add(exception); // add exception
            node.Attributes.Add(SchemaMsg.AttDateTime, createdDateTime);
            node.Attributes.Add(SchemaMsg.AttLevel, msgLevel.ToString());
            if (!string.IsNullOrEmpty(body)) node.Attributes.Add(SchemaMsg.AttBody, body);
            if (!string.IsNullOrEmpty(source)) node.Attributes.Add(SchemaMsg.AttSource, source);
            if (!string.IsNullOrEmpty(threadInfo)) node.Attributes.Add(SchemaMsg.AttThreadInfo, threadInfo);
            if ((providers != null) && (providers.Length > 0)) node.Attributes.Add(SchemaMsg.AttProviders, providers);
            if ((recipients != null) && (recipients.Length > 0)) node.Attributes.Add(SchemaMsg.AttRecipients, recipients);
            return node;
        }

        #region write

        /// <summary>
        /// Send MessageItem to transport for logging.
        /// </summary>
        /// <param name="msg">message</param>
        public abstract void Write(DDNode msg);

        /// <summary>
        /// Build MessageItem as DDNode
        /// Exposes the current time for this message. Defines the source name.
        /// Also replaces the format item in a specified body with the string representation of a corresponding object in a specified bodyArgs.
        /// </summary>
        /// <param name="msgLevel">level of message, which identifies important of message</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void Write(LogLevel msgLevel, string body, params object[] bodyArgs)
        {
            Write(msgLevel, null, null, null, body, bodyArgs);
        }
        /// <summary>
        /// Build MessageItem as DDNode
        /// Exposes the current time for this message. Defines the source name.
        /// Also replaces the format item in a specified body with the string representation of a corresponding object in a specified bodyArgs.
        /// </summary>
        /// <param name="msgLevel">level of message, which identifies important of message</param>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="providers">The list of providers who will be read this message. by default all providers</param>
        /// <param name="recipients">The list of recipients who will be receive this message. by default all recipients</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void Write(LogLevel msgLevel, Exception exception, string[] providers, string[] recipients, string body, params object[] bodyArgs)
        {
            if ((msgLevel & FilterMsg) != msgLevel) return; // skip messages by filter
            try
            {
                body = ((bodyArgs != null) && (bodyArgs.Length != 0) ? string.Format(body, bodyArgs) : body);
            }
            catch (Exception e)
            {
                var bodyF = body ?? "null";
                WriteError(e, Msg.CANNOT_BUILD_MSG_BODY, bodyF, Args2String(bodyArgs));
            }
            Write(MessageItem(DateTime.Now, msgLevel, GetSourceNameFromStack(), GetThreadInfo(), exception, body, providers, recipients));
        }
        /// <summary>
        /// Build MessageItem as DDNode and send to LogSrv across PipeTransport.<para> </para>
        /// Exposes the current time for this message. Defines the source name.<para> </para>
        /// Also replaces the format item in a specified body with the string representation of a corresponding object in a specified bodyArgs.
        /// </summary>
        /// <param name="msgLevel">level of message, which identifies important of message</param>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void Write(LogLevel msgLevel, Exception exception, string body, params object[] bodyArgs)
        {
            Write(msgLevel, exception, null, null, body, bodyArgs);
        }
        #endregion write
        #region WriteError
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <param name="exception">Exception for this message. Can be null</param>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteError(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.ERR, exception, body, bodyArgs);
        }
        /// <summary>
        /// Writes error message
        /// </summary>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteError(string body, params object[] bodyArgs)
        {
            Write(LogLevel.ERR, null, body, bodyArgs);
        }
        #endregion WriteError
        #region WriteWarning
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <param name="exception">exception for this message. Can be null</param>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteWarning(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.WAR, exception, body, bodyArgs);
        }
        /// <summary>
        /// Writes warning message
        /// </summary>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteWarning(string body, params object[] bodyArgs)
        {
            Write(LogLevel.WAR, null, body, bodyArgs);
        }
        #endregion WriteWarning
        #region WriteInfo
        /// <summary>
        /// Writes information message
        /// </summary>
        /// <param name="exception">Exception for this message. Can be null</param>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteInfo(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.INF, exception, body, bodyArgs);
        }
        /// <summary>
        /// Writes information message
        /// </summary>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteInfo(string body, params object[] bodyArgs)
        {
            Write(LogLevel.INF, null, body, bodyArgs);
        }
        #endregion WriteInfo
        #region WriteTrace
        /// <summary>
        /// Writes trace message
        /// </summary>
        /// <param name="exception">Exception for this message. Can be null</param>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteTrace(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.TRC, exception, body, bodyArgs);
        }
        /// <summary>
        /// Writes trace message
        /// </summary>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteTrace(string body, params object[] bodyArgs)
        {
            Write(LogLevel.TRC, null, body, bodyArgs);
        }
        #endregion WriteTrace
        #region WriteDebug
        /// <summary>
        /// Writes debug message
        /// </summary>
        /// <param name="exception">Exception for this message. Can be null</param>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteDebug(Exception exception, string body, params object[] bodyArgs)
        {
            Write(LogLevel.DBG, exception, body, bodyArgs);
        }
        /// <summary>
        /// Writes debug message
        /// </summary>
        /// <param name="body">Body of this message</param>
        /// <param name="bodyArgs">An object array that contains zero or more objects which formatting body of message.</param>
        public virtual void WriteDebug(string body, params object[] bodyArgs)
        {
            Write(LogLevel.DBG, null, body, bodyArgs);
        }
        #endregion WriteDebug
        #region Static
        /// <summary>
        /// Convert arguments string array to string like "'param1', 'empty', 'null'"
        /// <remarks>Return "null" for null arguments and "empty" for 0 arguments</remarks> 
        /// </summary>
        /// <param name="args">arguments to convert</param>
        /// <returns></returns>
        public static string Args2String(params object[] args)
        {
            if (args == null) return "null";
            if (args.Length == 0) return "empty";
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
        #endregion Static
    }
}
