using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DrOpen.DrCommon.DrSrv
{
    public class DrSrvMgr : IDisposable
    {
        /// <summary>
        /// Inits new DrSrvMgr object allows throw win32 exception
        /// </summary>
        public DrSrvMgr()
            : this(true)
        { }
        /// <summary>
        /// Inits new DrSrvMgr object
        /// </summary>
        /// <param name="allowThrowWin32Exception">allow throw win32 exception when function return win32 error</param>
        public DrSrvMgr(bool allowThrowWin32Exception)
        {
            this.AllowThrowWin32Exception = allowThrowWin32Exception;
        }
        /// <summary>
        /// Service change status event
        /// </summary>
        public event EventHandler<DrSrvChangeStatusEventArgs> ChangeStatusEvent;
        /// <summary>
        /// allow throw win32 exception when function return win32 error
        /// </summary>
        public bool AllowThrowWin32Exception { get; set; }
        /// <summary>
        /// Returns last Win32Exception
        /// </summary>
        public Win32Exception LastError { get; private set; }

        /// <summary>
        /// a handle to the openned service control manager database
        /// </summary>
        public IntPtr HSCManager { get; private set; }
        /// <summary>
        /// a handle to the openned service.
        /// </summary>
        public IntPtr HService { get; private set; }
        /// <summary>
        /// Sets Win32Exception to property <paramref name="LastError"/> and throws this exception if value of property <paramref name="AllowThrowWin32Exception"/> is true
        /// </summary>
        /// <param name="error">win 32 error code</param>
        /// <returns>always returns false or throws Win32Exception depends on  property <paramref name="AllowThrowWin32Exception"/></returns>
        private bool win32ErrorHandling(int error)
        {
            LastError = new Win32Exception(error);
            if (AllowThrowWin32Exception) throw LastError;
            return false;
        }

        #region OpenSCM
        /// <summary>
        ///  Establishes a connection to the local service control manager with generic read access
        /// </summary>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenSCM()
        {
            return OpenSCM("", DrSrvHelper.SC_MANAGER.SC_GENERIC_READ);
        }
        /// <summary>
        ///  Establishes a connection to the local service control manager with specified access rights
        /// </summary>
        /// <param name="access">The access to the service control manager. For a list of access rights, see Service Security and Access Rights.
        /// Before granting the requested access rights, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service control manager.
        /// The SC_MANAGER_CONNECT access right is implicitly specified by calling this function.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenSCM(DrSrvHelper.SC_MANAGER access)
        {
            return OpenSCM("", access);
        }
        /// <summary>
        /// Establishes a connection to the service control manager on the specified computer with specified access rights.
        /// </summary>
        /// <param name="serverName">The name of the target computer. If the pointer is NULL or points to an empty string, the function connects to the service control manager on the local computer.</param>
        /// <param name="access">The access to the service control manager. For a list of access rights, see Service Security and Access Rights.
        /// Before granting the requested access rights, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service control manager.
        /// The SC_MANAGER_CONNECT access right is implicitly specified by calling this function.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenSCM(string serverName, DrSrvHelper.SC_MANAGER access)
        {
            if (String.IsNullOrEmpty(serverName)) serverName = "."; // set localhost for empty or null server name
            // close all openned handles
            CloseHandle(this.HService);
            this.HService = IntPtr.Zero;
            CloseHandle(this.HSCManager);
            this.HSCManager = IntPtr.Zero; ;
            this.HSCManager = DrSrvHelper.OpenSCManager(serverName, null, (int)access);
            if (this.HSCManager.ToInt32() <= 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            return true;
        }
        #endregion OpenSCM
        #region CloseHandle
        /// <summary>
        /// Close opened service or SCM handle
        /// </summary>
        /// <param name="ptr">pointer</param>
        /// <returns></returns>
        public bool CloseHandle(IntPtr ptr) //close openned SCM
        {
            if (IntPtr.Zero != ptr) return DrSrvHelper.CloseServiceHandle(ptr); //close openned SCM
            return true;
        }
        /// <summary>
        /// закрываем все открытые хендлы
        /// </summary>
        public void Dispose()
        {
            CloseHandle(this.HService);
            CloseHandle(this.HSCManager);
        }
        #endregion CloseHandle
        #region OpenService
        /// <summary>
        /// Opens an existing service in openned SCManager with generic read access
        /// </summary>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenService(string serviceName)
        {
            return OpenService(this.HSCManager, serviceName, DrSrvHelper.SERVICE_ACCESS.SERVICE_GENERIC_READ);
        }
        /// <summary>
        /// Opens an existing service in openned SCManager
        /// </summary>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="access">The access to the service. For a list of access rights, see Service Security and Access Rights.
        /// Before granting the requested access, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service object.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenService(string serviceName, DrSrvHelper.SERVICE_ACCESS access)
        {
            return OpenService(this.HSCManager, serviceName, access);
        }
        /// <summary>
        /// Opens an existing service.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database. The OpenSCManager function returns this handle. For more information, see Service Security and Access Rights.</param>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="access">The access to the service. For a list of access rights, see Service Security and Access Rights.
        /// Before granting the requested access, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service object.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool OpenService(IntPtr hSCManager, string serviceName, DrSrvHelper.SERVICE_ACCESS access)
        {
            CloseHandle(this.HService);
            this.HService = IntPtr.Zero;
            this.HService = DrSrvHelper.OpenService(hSCManager, serviceName, (int)access);
            if (this.HService.ToInt32() <= 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            return true;
        }
        #endregion OpenService
        #region GetServiceCurrentStatus
        /// <summary>
        /// Retrieves the current status of the specified service.
        /// </summary>     
        /// <param name="status">A SERVICE_STATUS structure that receives the status information.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool GetServiceCurrentStatus(out DrSrvHelper.SERVICE_STATUS status)
        {
            return GetServiceCurrentStatus(this.HService, out status);
        }
        /// <summary>
        /// Retrieves the current status of the specified service.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or the CreateService function, and it must have the SERVICE_QUERY_STATUS access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="status">A SERVICE_STATUS structure that receives the status information.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool GetServiceCurrentStatus(IntPtr hService, out DrSrvHelper.SERVICE_STATUS status)
        {
            status = new DrSrvHelper.SERVICE_STATUS();
            if (DrSrvHelper.QueryServiceStatus(hService, ref status) == 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            return true;
        }
        #endregion GetServiceCurrentStatus
        #region WaitServiceCurrentStatus

        protected virtual void OnChangeStatusEvent(DrSrvChangeStatusEventArgs e)
        {
            EventHandler<DrSrvChangeStatusEventArgs> handler = ChangeStatusEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual bool ServiceWaiteStatus(DrSrvHelper.SERVICE_CURRENT_STATE state, int timeOut)
        {
            return ServiceWaiteStatus(this.HService, state, timeOut);
        }

        public virtual bool ServiceWaiteStatus(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE state, int timeOut)
        {
            DrSrvHelper.SERVICE_STATUS currentServiceStatus;
            var eventArgs = new DrSrvChangeStatusEventArgs(hService, state);
            eventArgs.Cancel = false;
            eventArgs.RemainigTimeOut = timeOut;
            do
            {
                GetServiceCurrentStatus(hService, out currentServiceStatus);
                eventArgs.CurrentSrvState = currentServiceStatus.serviceState;
                if (currentServiceStatus.serviceState == state)
                {
                    Thread.Sleep(100);
                    return true;
                }
                OnChangeStatusEvent(eventArgs);
                Thread.Sleep(1000);
                if (eventArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
                eventArgs.RemainigTimeOut--;
            } while (0 <= eventArgs.RemainigTimeOut);
            return win32ErrorHandling(DrSrvHelper.ERROR_SERVICE_REQUEST_TIMEOUT);
        }
        #endregion WaitServiceCurrentStatus
        #region GetServiceDependencies
        /// <summary>
        /// Returns service dependencies for curent service
        /// </summary>
        /// <param name="serviceState">the state of the services to be enumerated</param>
        /// <param name="dependentServices">output array of services depends on specified service</param>
        /// <returns></returns>
        public bool GetServiceDependencies(DrSrvHelper.SERVICE_STATE serviceState, out DrSrvHelper.ENUM_SERVICE_STATUS[] dependentServices)
        {
            return GetServiceDependencies(this.HService, serviceState, out dependentServices);
        }
        /// <summary>
        /// Returns service dependencies for specified service
        /// </summary>
        /// <param name="hService">handle of service</param>
        /// <param name="serviceState">the state of the services to be enumerated</param>
        /// <param name="dependentServices">output array of services depends on specified service</param>
        /// <returns></returns>
        public bool GetServiceDependencies(IntPtr hService, DrSrvHelper.SERVICE_STATE serviceState, out DrSrvHelper.ENUM_SERVICE_STATUS[] dependentServices)
        {
            int bytesNeeded = 0;
            int servicesReturned = 0;
            dependentServices = null;
            // gets buffer size
            DrSrvHelper.EnumDependentServices(hService, serviceState, IntPtr.Zero, 0, ref bytesNeeded, ref servicesReturned);
            // alloc buffer
            IntPtr lpServices = Marshal.AllocHGlobal((int)bytesNeeded);
            // gets service dependencies
            if (!DrSrvHelper.EnumDependentServices(hService, serviceState, lpServices, bytesNeeded, ref bytesNeeded, ref servicesReturned)) return win32ErrorHandling(Marshal.GetLastWin32Error());

            DrSrvHelper.ENUM_SERVICE_STATUS depService;
            int iDepPtr = lpServices.ToInt32();
            if (servicesReturned != 0)
            {
                dependentServices = new DrSrvHelper.ENUM_SERVICE_STATUS[servicesReturned];
                for (int i = 0; i < servicesReturned; i++)
                {
                    depService = (DrSrvHelper.ENUM_SERVICE_STATUS)Marshal.PtrToStructure(new IntPtr(iDepPtr), typeof(DrSrvHelper.ENUM_SERVICE_STATUS));
                    dependentServices[i] = depService;
                    iDepPtr += Marshal.SizeOf(depService);
                }
            }
            // free buffer
            Marshal.FreeCoTaskMem(lpServices);
            return true;
        }
        #endregion GetServiceDependencies
        #region ServiceControl
        /// <summary>
        /// Sends a control code to current service. Use the following commands from <paramref name="DrSrvHelper.SERVICE_CONTROL"/> , for example, STOP, START etc
        /// </summary>
        /// <param name="serviceControl">service control code <paramref name="DrSrvHelper.SERVICE_CONTROL"/></param>
        /// <param name="expectedSrvState">expected service state<paramref name="DrSrvHelper.SERVICE_CURRENT_STATE"/></param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceControl(DrSrvHelper.SERVICE_CONTROL serviceControl, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int timeOut)
        {
            return ServiceControl(this.HService, serviceControl, expectedSrvState, timeOut);
        }
        /// <summary>
        /// Sends a control code to a specified service. Use the following commands from <paramref name="DrSrvHelper.SERVICE_CONTROL"/> , for example, STOP, START etc
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_START access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="serviceControl">service control code <paramref name="DrSrvHelper.SERVICE_CONTROL"/></param>
        /// <param name="expectedSrvState">expected service state<paramref name="DrSrvHelper.SERVICE_CURRENT_STATE"/></param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceControl(IntPtr hService, DrSrvHelper.SERVICE_CONTROL serviceControl, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int timeOut)
        {
            DrSrvHelper.SERVICE_STATUS currentSrvStatus;
            if (!GetServiceCurrentStatus(hService, out currentSrvStatus)) return false;
            if (currentSrvStatus.serviceState == expectedSrvState) return true; // exit if current service status equals expected service status. nothing to do
            // send service commond
            if (DrSrvHelper.ControlService(hService, serviceControl, ref currentSrvStatus) != true) return win32ErrorHandling(Marshal.GetLastWin32Error());

            if (timeOut == 0) return true;
            return ServiceWaiteStatus(hService, expectedSrvState, timeOut);
        }
        #region Stop
        /// <summary>
        /// Stops current service
        /// </summary>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <param name="stopDependences">If value is 'true' automatically stops dependent services, otherwise, stops only specified service</param>
        /// <returns></returns>
        public bool ServiceStop( int timeOut, bool stopDependences)
        {
            return ServiceStop(this.HService, timeOut, stopDependences);
        }
        /// <summary>
        /// Stops specified service
        /// </summary>
        /// <param name="hService">a service handle</param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <param name="stopDependences">If value is 'true' automatically stops dependent services, otherwise, stops only specified service</param>
        /// <returns></returns>
        public bool ServiceStop(IntPtr hService, int timeOut, bool stopDependences)
        {
            if (stopDependences)
            {
                DrSrvHelper.ENUM_SERVICE_STATUS[] depServices;
                if (!GetServiceDependencies(hService, DrSrvHelper.SERVICE_STATE.SERVICE_ACTIVE, out depServices)) return false;
                // stops all dependent services
                if (depServices != null)
                {
                    IntPtr hDService;
                    foreach (var serviceStatus in depServices)
                    {
                        hDService = DrSrvHelper.OpenService(this.HSCManager, serviceStatus.pServiceName, (int)(DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS));
                        if (!this.ServiceStop(hDService, timeOut, stopDependences)) return false;
                    }
                }
            }
            // stops specified service
            return ServiceControl(hService, DrSrvHelper.SERVICE_CONTROL.SERVICE_CONTROL_STOP, DrSrvHelper.SERVICE_CURRENT_STATE.SERVICE_STOPPED, timeOut);
        }
        #endregion Stop
        #region Start
        /// <summary>
        /// Starts current service
        /// </summary>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceStart(int timeOut)
        {
            return ServiceStart(this.HService, timeOut);
        }
        /// <summary>
        /// Starts specified service
        /// </summary>
        /// <param name="hService">a service handle</param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceStart(IntPtr hService, int timeOut)
        {
            if (DrSrvHelper.StartService(hService, 0, null) != true) return win32ErrorHandling(Marshal.GetLastWin32Error());
            if (timeOut == 0) return true;
            return ServiceWaiteStatus(hService, DrSrvHelper.SERVICE_CURRENT_STATE.SERVICE_RUNNING, timeOut);
        }
        #endregion Start
        #endregion ServiceControl

    }
}
