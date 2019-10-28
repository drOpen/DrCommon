/*
  LogConsole.cs -- system console provider for DrLog 1.1.0, January 24, 2016
 
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

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    /// <summary>
    /// system console log rpovider for DrLog
    /// </summary>
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
