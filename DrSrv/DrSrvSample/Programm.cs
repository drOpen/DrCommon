/*
  Programm.cs -- sample of usage manager of services 1.0, July 23, 2017
 
  Copyright (c) 2013-2017 Kudryashov Andrey aka Dr
 
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
using System.Collections.Generic;
using DrOpen.DrCommon.DrSrv;

namespace DrSrvSample
{
    class Programm
    {
        static void Main(string[] args)
        {

            var srvMgr = new DrSrvMgr();
            srvMgr.EventBeforeOpenSCM += DoBeforeOpenSCM;
            srvMgr.EventAfterOpenSCM += DoAfterOpenSCM;
            srvMgr.EventBeforeOpenService += DoBeforeOpenService;
            srvMgr.EventAfterOpenService += DoAfterOpenService;
            srvMgr.EventWaitExpectedServiceState += DoWaitExpectedStatus;
            srvMgr.EventBeforeWaitExpectedServiceState += DoBeforeWaitExpectedStatus;
            srvMgr.EventAfterWaitExpectedServiceState += DoAferWaitExpectedStatus;
            srvMgr.EventBeforeServiceControl += DoBeforeServiceControl;
            srvMgr.EventAfterServiceControl += DoAfterServiceControl;
            srvMgr.EventBeforeServiceStart += DoBeforeServiceStart;
            srvMgr.EventAfterServiceStart += DoAfterServiceStart;
            srvMgr.EventBeforeCreateService += DoBeforeCreateService;
            srvMgr.EventAfterCreateService += DoAfterCreateService;
            srvMgr.EventBeforeServiceDelete += DoBeforeServiceDelete;
            srvMgr.EventAfterServiceDelete += DoAfterServiceDelete;
            srvMgr.EventBeforeThrowWin32Error += DoBeforeThrowWin32Error;

            srvMgr.OpenSCM(DrSrvHelper.SC_MANAGER.SC_GENERIC_READ);
            srvMgr.OpenService("Spooler", DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG);
            DrSrvHelper.QUERY_SERVICE_CONFIG config;
            srvMgr.GetServiceConfig(srvMgr.HService, out config);
            string description;
            bool delayAutostart;
            srvMgr.GetServiceDescription(out description);
            srvMgr.GetServiceDelayAutostartInfo(out delayAutostart);
            srvMgr.OpenSCM(DrSrvHelper.SC_MANAGER.SC_MANAGER_ALL_ACCESS);
            srvMgr.OpenService("wscsvc", DrSrvHelper.SERVICE_ACCESS.SERVICE_CHANGE_CONFIG);
            var info = new DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO();
            info.fDelayedAutostart = true;
            srvMgr.SetServiceConfig2<DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO>(srvMgr.HService, DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO, info);
            //srvMgr.SetServiceDelayAutostartInfo("wscsvc", true);
            srvMgr.OpenService("TimeBroker", DrSrvHelper.SERVICE_ACCESS.SERVICE_START | DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG);
            srvMgr.ServiceStop(10, true);

            DrSrvHelper.SERVICE_STATUS status;
            srvMgr.GetServiceCurrentStatus(out status);

            srvMgr.ServiceWaitStatus(DrSrvHelper.SERVICE_CURRENT_STATE.SERVICE_STOPPED, 10);
            srvMgr.ServiceStart(10);
            
            srvMgr.CreateService("notepad1","NOTEPAD1", "description",  DrSrvHelper.SERVICE_ACCESS.SERVICE_ALL_ACCESS, DrSrvHelper.SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS, DrSrvHelper.SERVICE_START_TYPE.SERVICE_AUTO_START, DrSrvHelper.SERVICE_ERROR_TYPE.SERVICE_ERROR_IGNORE, "notepad.exe", "");
            srvMgr.ServiceDelete("notepad");

        }


        static void DoBeforeServiceDelete(object sender, DrSrvEventArgsBeforeServiceDelete e)
        {
            Console.WriteLine("Service will be deleted by handle '{0}'.", e.HService);
        }
        static void DoAfterServiceDelete(object sender, DrSrvEventArgService e)
        {
            Console.WriteLine("Service by handle '{0}' was successfully marked as deletion.", e.HService);
        }

        static void DoBeforeCreateService(object sender, DrSrvEventArgsBeforeCreateService e)
        {
            Console.WriteLine("Starting create service '{0}'.", e.ServiceName);
        }
        static void DoAfterCreateService(object sender, DrSrvEventArgsAfterCreateService e)
        {
            Console.WriteLine("Service '{0}' successfully created and new service handle is '{1}'.", e.ServiceName, e.HService);
        }

        static void DoBeforeServiceStart(object sender, DrSrvEventArgsBeforeServiceStart e)
        {
            Console.WriteLine("Starting service by handle '{0}' remainig time '{1}'.", e.HService, e.TimeOut);
        }
        static void DoAfterServiceStart(object sender, DrSrvEventArgsAfterServiceStart e)
        {
            Console.WriteLine("Service successfully started by handle '{0}'.", e.HService);
        }
        static void DoBeforeServiceControl(object sender, DrSrvEventArgsBeforeServiceControl e)
        {
            Console.WriteLine("Sending service notification '{0}' and waiting expected state '{1}' remainig time '{2}'. The service handle is '{3}'.", e.ServiceControl, e.ExpectedSrvState, e.TimeOut, e.HService);
        }
        static void DoAfterServiceControl(object sender, DrSrvEventArgsAfterServiceControl e)
        {
            Console.WriteLine("Service received notification '{0}' successfully. The service handle is '{1}'.", e.ServiceControl, e.HService);
        }
        static void DoBeforeOpenSCM(object sender, DrSrvEventArgsBeforeOpenSCM e)
        {
            Console.WriteLine("Openinig SCM on the server '{0}' with access '{1}'.", e.ServerName, e.Access);
        }
        static void DoAfterOpenSCM(object sender, DrSrvEventArgsAfterOpenSCM e)
        {
            Console.WriteLine("SCM on the server '{0}' with access '{1}' is successfully openned. The SCM handle value is '{2}'.", e.ServerName, e.Access, e.HSCM);
        }

        static void DoBeforeOpenService(object sender, DrSrvEventArgsBeforeOpenService e)
        {
            Console.WriteLine("Openning service '{0}' with access '{1}' using SCM handle '{2}'.", e.ServiceName, e.Access, e.HSCM);
        }

        static void DoAfterOpenService(object sender, DrSrvEventArgsAfterOpenService e)
        {
            Console.WriteLine("The service '{0}' is successfully openned with access '{1}'. The service handle is '{2}'.", e.ServiceName, e.Access, e.HService);
        }
        static void DoBeforeWaitExpectedStatus(object sender, DrSrvEventArgsBeforeWaitExpectedServiceState e)
        {
            Console.WriteLine("Waiting service status '{0}' by handle '{1}'.", e.ExpectedSrvState, e.HService);
        }
        static void DoWaitExpectedStatus(object sender, DrSrvEventArgsWaitExpectedServiceState e)
        {
            Console.Write(".");
            if (e.RemainigTimeOut < 5) e.Cancel = true;
        }
        static void DoAferWaitExpectedStatus(object sender, DrSrvEventArgsAfterWaitExpectedServiceState e)
        {
            Console.WriteLine("The service state '{0}' is expected. Spent on wait time is '{1}'. The service handle is '{2}'.", e.CurrentSrvState, e. SpentOnTimeWait, e.HService);
        }
        static void DoBeforeThrowWin32Error(object sender, DrSrvEventArgBeforeThrowWin32Error e)
        {
            Console.WriteLine("{0}.", e.Win32Exception.Message);
        }
    }
}
