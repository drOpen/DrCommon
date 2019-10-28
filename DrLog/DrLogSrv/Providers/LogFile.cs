/*
  LogFile.cs -- log file provider for DrLog 1.1.0, January 24, 2016
 
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
using System.Text;
using DrOpen.DrData.DrDataObject;
using System.IO;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    /// <summary>
    /// log file provider for DrLog
    /// </summary>
    public class LogFile : Provider
    {

        #region LogFile
        public LogFile(DDNode config)
            : base(config)
        {
            this.RebuildConfiguration();
        }
        public LogFile(DDNode config, bool mergeWithDefault)
            : base(config, mergeWithDefault)
        {
            this.RebuildConfiguration();
        }
        #endregion LogFile

        /// <summary>
        /// stream to file
        /// </summary>
        StreamWriter strWrt;

        public string DateTimeFormat
        {
            get;
            private set;
        }
        /// <summary>
        /// columns separator
        /// </summary>
        public string Separator
        {
            get;
            private set;
        }
        /// <summary>
        /// File name
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Add additional column separator for message with level Trace compared with Info
        /// </summary>
        public bool ExtendedSeparatorForTrace
        {
            get;
            private set;
        }
        /// <summary>
        /// Add additional column separator for message with level Debug compared with Trace
        /// </summary>
        public bool ExtendedSeparatorForDebug
        {
            get;
            private set;
        }

        /// <summary>
        /// Update settings from config
        /// </summary>
        public override void RebuildConfiguration()
        {
            this.Separator = Config.Attributes.GetValue(LogFile.AttColumnSeparator, LogFile.DefaultValueColumnSeparator);
            this.DateTimeFormat = Config.Attributes.GetValue(LogFile.AttDateTimeFormat, LogFile.DefaultValueDateTimeFormat);
            this.ExtendedSeparatorForTrace = Config.Attributes.GetValue(LogFile.AttExtandedSeparatorForTrace, LogFile.DefaultValueExtandedSeparatorForTrace );
            this.ExtendedSeparatorForTrace = Config.Attributes.GetValue(LogFile.AttExtandedSeparatorForDebug, LogFile.DefaultValueExtandedSeparatorForDebug);

            this.FileName = Config.Attributes.GetValue(LogFile.AttFileName, LogFile.DefaultValueFileName);

            InitStream(this.FileName);
            base.RebuildConfiguration();
        }
        /// <summary>
        /// Wtrite log message
        /// </summary>
        /// <param name="msg">msg</param>
        public override void Write(DDNode msg)
        {
            var logLevel = msg.GetLogLevel();
            if ((logLevel & this.Level) != logLevel) return;

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append(msg.GetDateTime().ToString(this.DateTimeFormat));
            sBuilder.Append(this.Separator);

            sBuilder.Append(logLevel.ToString());
            sBuilder.Append(this.Separator);

            sBuilder.Append(msg.GetSource());
            sBuilder.Append(this.Separator);

            if ((this.ExtendedSeparatorForTrace) && (logLevel == LogLevel.TRC)) sBuilder.Append(this.Separator);
            if ((this.ExtendedSeparatorForDebug) && (logLevel == LogLevel.DBG))
            {
                sBuilder.Append(this.Separator);
                if (this.ExtendedSeparatorForTrace) sBuilder.Append(this.Separator);
            }

            sBuilder.Append(msg.GetBody());
            sBuilder.Append(this.Separator);

            if ((this.ExceptionLevel != LogExceptionLevel.NONE) && (msg.ContainsException()))
            {
                sBuilder.Append(msg.GetLogException(this.ExceptionLevel));
            }

            strWrt.WriteLine(sBuilder.ToString());
            strWrt.Flush();
        }

        public override void Dispose()
        {
            CloseStream();
        }

        public override DDNode DefaultConfig
        {
            get
            {
                return GetDefaultConfig();
            }
        }


        private void InitStream(string fileName)
        {
            if (strWrt != null) CloseStream();
            strWrt = new StreamWriter(new FileStream(fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read));
        }

        private void CloseStream()
        {
            if (strWrt != null)
            {
                try
                {
                    strWrt.Flush();
                    strWrt.Dispose();
                }
                catch
                { }
            }
        }

        public static string GetType()
        {
            throw new DrData.DrDataObject.Exceptions.DDTypeNullException();
        }

        public static DDNode GetDefaultConfig()
        {
            var n = GetCommonConfig(typeof(LogFile).AssemblyQualifiedName);
            n.Attributes.Add(AttDateTimeFormat, DefaultValueDateTimeFormat);
            n.Attributes.Add(AttColumnSeparator, DefaultValueColumnSeparator);
            n.Attributes.Add(AttExtandedSeparatorForTrace , DefaultValueExtandedSeparatorForTrace);
            n.Attributes.Add(AttExtandedSeparatorForDebug, DefaultValueExtandedSeparatorForDebug);
            n.Attributes.Add(AttFileName, DefaultValueFileName);
            //n.Attributes.Add(AttReopenFileHandle, false);
            //n.Attributes.Add();
            return n;
        }

        #region default value
        /// <summary>
        /// Default format for date time
        /// </summary>
        public const string DefaultValueDateTimeFormat = "";
        /// <summary>
        /// Default colums separator
        /// </summary>
        public const string DefaultValueColumnSeparator = "\t";
        /// <summary>
        /// Add additional column separator for message with level Trace compared with Info
        /// </summary>
        public const bool DefaultValueExtandedSeparatorForTrace = true;
        /// <summary>
        /// Add additional column separator for message with level Debug compared with Trace
        /// </summary>
        public const bool DefaultValueExtandedSeparatorForDebug = true;
        /// <summary>
        /// default log file name
        /// </summary>
        public const string DefaultValueFileName = "Log.log";

        #endregion default value

        #region basic attributes for config
        /// <summary>
        /// Converts the value of the current DateTime to its equivalent string representation using the specified culture-specific format information.
        /// </summary>
        public const string AttDateTimeFormat = "DateTimeFormat";
        /// <summary>
        /// Column separator
        /// </summary>
        public const string AttColumnSeparator = "ColumnSeparator";
        /// <summary>
        /// Add additional column separator for message with level Trace compared with Info
        /// </summary>
        public const string AttExtandedSeparatorForTrace = "ExtandedSeparatorForTrace";
        /// <summary>
        /// Add additional column separator for message with level Debug compared with Trace
        /// </summary>
        public const string AttExtandedSeparatorForDebug = "ExtandedSeparatorForDebug";
        /// <summary>
        /// File name
        /// </summary>
        public const string AttFileName = "FileName";

        #endregion basic attributes for config

    }
}
