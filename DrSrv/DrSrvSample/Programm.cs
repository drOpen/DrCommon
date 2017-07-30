using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrOpen.DrCommon.DrSrv;

namespace DrSrvSample
{
    class Programm
    {
        static void Main(string[] args)
        {

            var srvMgr = new DrSrvMgr();
            srvMgr.OpenSCM(DrSrvHelper.SC_MANAGER.SC_MANAGER_ALL_ACCESS);
            srvMgr.OpenService("LanmanServer", DrSrvHelper.SERVICE_ACCESS.SERVICE_START | DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS);

            srvMgr.ChangeStatusEvent += DoChangeStatusEventArgs;

            srvMgr.ServiceStop(10, true);
            srvMgr.ServiceStart(10);

            DrSrvHelper.SERVICE_STATUS status;
            srvMgr.GetServiceCurrentStatus(out status);
            
            Console.Write("Waiting service status stopped");
            srvMgr.ServiceWaiteStatus(DrSrvHelper.SERVICE_CURRENT_STATE.SERVICE_STOPPED, 10);

        }


        static void DoChangeStatusEventArgs(object sender, DrSrvChangeStatusEventArgs e)
        {
            Console.Write(".");
            if (e.RemainigTimeOut < 5)  e.Cancel = true;
        }
    }
}
