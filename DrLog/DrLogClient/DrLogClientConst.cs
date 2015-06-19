using System;

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
    #region Level
    /// <summary>
    /// Log Level
    /// </summary>
    [Flags]
    public enum LogLevel
    {

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
        INFO = (ERR | WAR| INF),
        /// <summary>
        /// Extands <see cref="INFO"/> adds <see cref="LogLevel.TRC"/>
        /// </summary>
        TRACE = INFO| TRC,
        /// <summary>
        /// Extands <see cref="TRACE"/> adds <see cref="LogLevel.DBG"/>. All messages including debug level
        /// </summary>
        ALL = TRACE | DBG
    }
    #endregion Level
    
    public enum MessageAttributes
    {
        /// <summary>
        /// creation time of the message 
        /// </summary>
        DateTime,
        /// <summary>
        /// body of message
        /// </summary>
        Body,
        /// <summary>
        /// Log level of message
        /// </summary>
        LogLevel,
        /// <summary>
        /// Who created the message 
        /// </summary>
        Source,
        /// <summary>
        /// The list of providers who will be read this message. by default all providers
        /// </summary>
        Providers,
        /// <summary>
        /// The list of recipients who will be receive this message. by default all recipients
        /// </summary>
        Recipients
    }

    public class DrLogClientConst
    {
        public const string MessageType = "DrLogMessage";
    }
}
