using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public class LogConsole : Provider
    {

        public LogConsole(DDNode config)
            : base(config)
        { }
        public LogConsole(DDNode config, bool mergeWithDefault)
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
            var n = new DDNode(new DDType(typeof(LogConsole).AssemblyQualifiedName));
            n.Attributes.Add(AttDateTimeFormat, new DDValue(""));
            n.Attributes.Add(AttColumnSeparator, new DDValue("\t"));
            //n.Attributes.Add();
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
        #endregion basic attributes for config

    }
}
