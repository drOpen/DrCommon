using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public class LogConsole : LogProvider
    {

        public LogConsole(DrData.DDNode config)
            : base(config)
        { }

        public override void Write(DrData.DDNode msg)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get 
            { 
                throw new NotImplementedException(); 
            }
        }
    }
}
