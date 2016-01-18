using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public class LogFile : Provider
    {

        public LogFile(DDNode config)
            : base(config)
        { }
        public LogFile(DDNode config, bool mergeWithDefault)
            : base(config, mergeWithDefault)
        { }

        public override void Write(DDNode msg)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }


        public override DDNode DefaultConfig
        {
            get
            {
                return GetDefaultConfig();
            }
        }


        public static DDNode GetDefaultConfig()
        {
            var n = GetCommonConfig(typeof(LogFile).AssemblyQualifiedName);
            n.Attributes.Add(AttDateTimeFormat, String.Empty);
            n.Attributes.Add(AttColumnSeparator, "\t");
            n.Attributes.Add(AttFileName, "Log.log");
            n.Attributes.Add(AttReopenFileHandle, false);
            //n.Attributes.Add();
            return n;
        }

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
        public const string AttReopenFileHandle = "ReopenFileHandle";

        #endregion basic attributes for config

    }
}
