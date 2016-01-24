using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrOpen.DrCommon.DrData;
using System.IO;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public class LogFile : Provider
    {

        #region LogFile
        public LogFile(DDNode config)
            : base(config)
        { this.RebuildConfiguration(); }
        public LogFile(DDNode config, bool mergeWithDefault)
            : base(config, mergeWithDefault)
        { this.RebuildConfiguration(); }
        #endregion LogFile

        StreamWriter strWrt;

        public string DateTimeFormat
        {
            get;
            private set;
        }

        public string Separator
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }
        public override void RebuildConfiguration()
        {
            this.Separator = Config.Attributes.GetValue(LogFile.AttColumnSeparator, LogFile.DefaultValueColumnSeparator);
            this.DateTimeFormat = Config.Attributes.GetValue(LogFile.AttDateTimeFormat, LogFile.DefaultValueDateTimeFormat);
            this.FileName= Config.Attributes.GetValue(LogFile.AttFileName, LogFile.DefaultValueFileName);
            InitStream(this.FileName);
            base.RebuildConfiguration();
        }

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

            if (logLevel == LogLevel.TRC) sBuilder.Append(this.Separator);
            if (logLevel == LogLevel.DBG) sBuilder.Append(this.Separator + this.Separator);

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
            strWrt = new StreamWriter (new FileStream(fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read));
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

        public static DDNode GetDefaultConfig()
        {
            var n = GetCommonConfig(typeof(LogFile).AssemblyQualifiedName);
            n.Attributes.Add(AttDateTimeFormat, DefaultValueDateTimeFormat);
            n.Attributes.Add(AttColumnSeparator, DefaultValueColumnSeparator);
            n.Attributes.Add(AttFileName, DefaultValueFileName);
            //n.Attributes.Add(AttReopenFileHandle, false);
            //n.Attributes.Add();
            return n;
        }

        #region default value
        public const string DefaultValueDateTimeFormat = "";
        public const string DefaultValueColumnSeparator = "\t";
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
        /// File name
        /// </summary>
        public const string AttFileName = "FileName";
        /// <summary>
        /// reopen the handle at each message
        /// </summary>
        //public const string AttReopenFileHandle = "ReopenFileHandle";

        #endregion basic attributes for config

    }
}
