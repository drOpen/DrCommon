/*
  Logger.cs -- client for DrLog 1.0.0, August 30, 2015
 
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
using System;
using System.Diagnostics;
using DrOpen.DrCommon.DrData;
using DrLogClient.Res;

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
    /// <summary>
    /// Create client logging messages
    /// </summary>
    public class Logger : DrLogClient.ILogger
    {
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

        /// <summary>
        /// return message as DDNode with basic attributes
        /// </summary>
        /// <param name="createdDateTime">message creation date time</param>
        /// <param name="logLevel">log level</param>
        /// <param name="source">source</param>
        /// <param name="exception">exception with inner exception. This attribute support null for message without exception</param>
        /// <param name="body">body of message</param>
        /// <param name="providers">array of supported providers. All connected providers will be get this message if array is empty</param>
        /// <param name="recipients">array of supported recipients. All connected recipients will be get this message if array is empty</param>
        /// <returns></returns>
        public static DDNode MessageItem(DateTime createdDateTime, LogLevel logLevel, string source, Exception exception, string body, string[] providers, string[] recipients)
        {
            var node = new DDNode(new DDType(SchemaMsg.MessageType)) { Type = SchemaMsg.MessageType };

            if (exception != null) node.Add(exception); // add exception
            node.Attributes.Add(SchemaMsg.AttDateTime, createdDateTime);
            node.Attributes.Add(SchemaMsg.AttLevel, logLevel.ToString());
            if (!string.IsNullOrEmpty(body)) node.Attributes.Add(SchemaMsg.AttBody, body);
            if (!string.IsNullOrEmpty(source)) node.Attributes.Add(SchemaMsg.AttSource, source);
            if ((providers != null) && (providers.Length > 0)) node.Attributes.Add(SchemaMsg.AttProviders, providers);
            if ((recipients != null) && (recipients.Length > 0)) node.Attributes.Add(SchemaMsg.AttRecipients, recipients);
            return node;
        }

        #region write

        /// <summary>
        /// Send MessageItem to transport for logging. Temporary transport doesn't support
        /// </summary>
        /// <param name="msg">message</param>
        public virtual void Write(DDNode msg)
        {
            try
            {
               //this.logSrv
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
