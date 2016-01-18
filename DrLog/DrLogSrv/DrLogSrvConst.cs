/*
  DrLogSrvConst.cs -- constants for DrLog 1.1.0, 4 January 30, 2016
 
  Copyright (c) 2013-2016 Kudryashov Andrey aka Dr
 
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

namespace DrOpen.DrCommon.DrLog.DrLogSrv
{
    #region Exception
    /// <summary>
    /// Log Exception
    /// </summary>
    [Flags]
    public enum LogExceptionLevel : int
    {
        /// <summary>
        /// Exceptions will not be logged
        /// </summary>
        NONE=0,
        /// <summary>
        /// Log Stack Trace of Exception
        /// </summary>
        STACK_TRACE=1,
        /// <summary>
        /// Log Source of Exception
        /// </summary>
        SOURCE=2,
        /// <summary>
        /// Log Help Link of Exception
        /// </summary>
        HELP_LINK=4,
        /// <summary>
        /// Log Exception Type
        /// </summary>
        TYPE=8,
        /// <summary>
        /// Log Data of Exception
        /// </summary>
        DATA=16,
        /// <summary>
        /// Log Exception Message
        /// </summary>
        MESSAGE=32,
        /// <summary>
        /// Inner Exceptions will be logged
        /// </summary>
        INNERT_EXCEPTION=512,
        /// <summary>
        /// All exceptions fields include inner exceptions will be logged
        /// </summary>
        ALL = STACK_TRACE | SOURCE | HELP_LINK | TYPE | DATA | MESSAGE | INNERT_EXCEPTION
    }
    #endregion Exception

    #region Level
    /// <summary>
    /// Log Level
    /// </summary>
    [Flags]
    public enum LogLevel : int
    {
        /// <summary>
        /// Exclude all messages
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Error
        /// </summary>
        ERR = 1,
        /// <summary>
        /// Warning
        /// </summary>
        WAR = 2,
        /// <summary>
        /// Information
        /// </summary>
        INF = 4,
        /// <summary>
        /// Trace
        /// </summary>
        TRC = 8,
        /// <summary>
        /// Debug
        /// </summary>
        DBG = 16,
        /// <summary>
        /// Only inforamtion mesages: <see cref="LogLevel.ERR"/>, <see cref="LogLevel.WAR"/> and <see cref="LogLevel.INF"/>
        /// </summary>
        INFO = (ERR | WAR | INF),
        /// <summary>
        /// Extands <see cref="INFO"/> adds <see cref="LogLevel.TRC"/>
        /// </summary>
        TRACE = INFO | TRC,
        /// <summary>
        /// Extands <see cref="TRACE"/> adds <see cref="LogLevel.DBG"/>. All messages including debug level
        /// </summary>
        ALL = TRACE | DBG
    }
    #endregion Level

    /// <summary>
    /// Common constants for DrLogProvider
    /// </summary> 
    public static class DrLogProviderConst
    {
        /// <summary>
        /// Log level of provider
        /// </summary>
        public const string AttLevel = "Level";
        /// <summary>
        /// Log Exception level of provider
        /// </summary>
        public const string AttExceptionLevel = "ExceptionLevel";

    }


    /// <summary>
    /// Constants for DrLogMsg
    /// </summary> 
    public static class DrLogMsgConst
    {
        /// <summary>
        /// DDNode type for messages
        /// </summary>
        public const string MessageType = "DrLogMessage";

        #region basic attributes for messages
        /// <summary>
        /// creation time of the message 
        /// </summary>
        public const string AttDateTime = "DateTime";
        /// <summary>
        /// body of message
        /// </summary>
        public const string AttBody = "Body";
        /// <summary>
        /// Log level of message
        /// </summary>
        public const string AttLevel = "Level";
        /// <summary>
        /// Log exception of message
        /// </summary>
        public const string AttException = "Exception";
        /// <summary>
        /// Who created the message 
        /// </summary>
        public const string AttSource = "Source";
        /// <summary>
        /// The list of providers who will be read this message. by default all providers
        /// </summary>
        public const string AttProviders = "Providers";
        /// <summary>
        /// The list of recipients who will be receive this message. by default all recipients
        /// </summary>
        public const string AttRecipients = "Recipients";
        #endregion basic attributes for messages
    }
}
