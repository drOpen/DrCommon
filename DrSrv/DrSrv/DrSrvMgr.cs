/*
  DrSrvMgr.cs -- manager of services 1.0, July 23, 2017
 
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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace DrOpen.DrCommon.DrSrv
{
    public class DrSrvMgr : IDisposable
    {
        #region DrSrvMgr
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
        #endregion DrSrvMgr
        #region Fields
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
        #endregion Fields
        #region Events
        #region OpenSCM
        public event EventHandler<DrSrvEventArgsBeforeOpenSCM> EventBeforeOpenSCM;
        protected virtual void OnBeforeOpenSCM(DrSrvEventArgsBeforeOpenSCM e)
        {
            EventHandler<DrSrvEventArgsBeforeOpenSCM> handler = EventBeforeOpenSCM;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgsAfterOpenSCM> EventAfterOpenSCM;
        protected virtual void OnAfterOpenSCM(DrSrvEventArgsAfterOpenSCM e)
        {
            EventHandler<DrSrvEventArgsAfterOpenSCM> handler = EventAfterOpenSCM;
            if (handler != null) handler(this, e);
        }
        #endregion OpenSCM
        #region OpenService
        public event EventHandler<DrSrvEventArgsBeforeOpenService> EventBeforeOpenService;
        protected virtual void OnBeforeOpenService(DrSrvEventArgsBeforeOpenService e)
        {
            EventHandler<DrSrvEventArgsBeforeOpenService> handler = EventBeforeOpenService;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgsAfterOpenService> EventAfterOpenService;
        protected virtual void OnAfterOpenService(DrSrvEventArgsAfterOpenService e)
        {
            EventHandler<DrSrvEventArgsAfterOpenService> handler = EventAfterOpenService;
            if (handler != null) handler(this, e);
        }
        #endregion OpenService
        #region WaitExpectedServiceState
        public event EventHandler<DrSrvEventArgsBeforeWaitExpectedServiceState> EventBeforeWaitExpectedServiceState;
        protected virtual void OnBeforeWaitExpectedServiceState(DrSrvEventArgsBeforeWaitExpectedServiceState e)
        {
            EventHandler<DrSrvEventArgsBeforeWaitExpectedServiceState> handler = EventBeforeWaitExpectedServiceState;
            if (handler != null) handler(this, e);
        }
        public event EventHandler<DrSrvEventArgsWaitExpectedServiceState> EventWaitExpectedServiceState;
        protected virtual void OnWaitExpectedServiceState(DrSrvEventArgsWaitExpectedServiceState e)
        {
            EventHandler<DrSrvEventArgsWaitExpectedServiceState> handler = EventWaitExpectedServiceState;
            if (handler != null) handler(this, e);
        }
        public event EventHandler<DrSrvEventArgsAfterWaitExpectedServiceState> EventAfterWaitExpectedServiceState;
        protected virtual void OnAfterWaitExpectedServiceState(DrSrvEventArgsAfterWaitExpectedServiceState e)
        {
            EventHandler<DrSrvEventArgsAfterWaitExpectedServiceState> handler = EventAfterWaitExpectedServiceState;
            if (handler != null) handler(this, e);
        }
        #endregion WaitExpectedServiceState
        #region ServiceControl
        public event EventHandler<DrSrvEventArgsBeforeServiceControl> EventBeforeServiceControl;
        protected virtual void OnBeforeServiceControl(DrSrvEventArgsBeforeServiceControl e)
        {
            EventHandler<DrSrvEventArgsBeforeServiceControl> handler = EventBeforeServiceControl;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgsAfterServiceControl> EventAfterServiceControl;
        protected virtual void OnAfterServiceControl(DrSrvEventArgsAfterServiceControl e)
        {
            EventHandler<DrSrvEventArgsAfterServiceControl> handler = EventAfterServiceControl;
            if (handler != null) handler(this, e);
        }
        #endregion ServiceControl
        #region ServiceStart
        public event EventHandler<DrSrvEventArgsBeforeServiceStart> EventBeforeServiceStart;
        protected virtual void OnBeforeServiceStart(DrSrvEventArgsBeforeServiceStart e)
        {
            EventHandler<DrSrvEventArgsBeforeServiceStart> handler = EventBeforeServiceStart;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgsAfterServiceStart> EventAfterServiceStart;
        protected virtual void OnAfterServiceStart(DrSrvEventArgsAfterServiceStart e)
        {
            EventHandler<DrSrvEventArgsAfterServiceStart> handler = EventAfterServiceStart;
            if (handler != null) handler(this, e);
        }
        #endregion ServiceStart
        #region CloseHandle
        public event EventHandler<DrSrvEventArgsHandle> EventBeforeCloseHandle;
        protected virtual void OnBeforeCloseHandle(DrSrvEventArgsHandle e)
        {
            EventHandler<DrSrvEventArgsHandle> handler = EventBeforeCloseHandle;
            if (handler != null) handler(this, e);
        }
        #endregion CloseHandle
        #region CreateService
        public event EventHandler<DrSrvEventArgsBeforeCreateService> EventBeforeCreateService;
        protected virtual void OnBeforeCreateService(DrSrvEventArgsBeforeCreateService e)
        {
            EventHandler<DrSrvEventArgsBeforeCreateService> handler = EventBeforeCreateService;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgsAfterCreateService> EventAfterCreateService;
        protected virtual void OnAfterCreateService(DrSrvEventArgsAfterCreateService e)
        {
            EventHandler<DrSrvEventArgsAfterCreateService> handler = EventAfterCreateService;
            if (handler != null) handler(this, e);
        }
        #endregion CreateService
        #region ServiceDelete
        public event EventHandler<DrSrvEventArgsBeforeServiceDelete> EventBeforeServiceDelete;
        protected virtual void OnBeforeServiceDelete(DrSrvEventArgsBeforeServiceDelete e)
        {
            EventHandler<DrSrvEventArgsBeforeServiceDelete> handler = EventBeforeServiceDelete;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DrSrvEventArgService> EventAfterServiceDelete;
        protected virtual void OnAfterServiceDelete(DrSrvEventArgService e)
        {
            EventHandler<DrSrvEventArgService> handler = EventAfterServiceDelete;
            if (handler != null) handler(this, e);
        }
        #endregion ServiceDelete
        #region Win32Error
        public event EventHandler<DrSrvEventArgBeforeThrowWin32Error> EventBeforeThrowWin32Error;
        protected virtual void OnBeforeThrowWin32Error(DrSrvEventArgBeforeThrowWin32Error e)
        {
            EventHandler<DrSrvEventArgBeforeThrowWin32Error> handler = EventBeforeThrowWin32Error;
            if (handler != null) handler(this, e);
        }
        #endregion Win32Error
        #endregion Events
        /// <summary>
        /// Sets Win32Exception to property <paramref name="LastError"/> and throws this exception if value of property <paramref name="AllowThrowWin32Exception"/> is true
        /// </summary>
        /// <param name="error">win 32 error code</param>
        /// <returns>always returns false or throws Win32Exception depends on  property <paramref name="AllowThrowWin32Exception"/></returns>
        private bool win32ErrorHandling(int error)
        {
            LastError = new Win32Exception(error);
            var eBeforeArgs = new DrSrvEventArgBeforeThrowWin32Error(LastError, this.AllowThrowWin32Exception);

            OnBeforeThrowWin32Error(eBeforeArgs);

            if (!eBeforeArgs.Cancel) throw LastError;
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
            var eBeforeArgs = new DrSrvEventArgsBeforeOpenSCM(serverName, access);
            OnBeforeOpenSCM(eBeforeArgs); // raise event before open SCM
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED); // Canceled by user
            // close all openned handles
            CloseHandle(this.HService);
            this.HService = IntPtr.Zero;
            CloseHandle(this.HSCManager);
            this.HSCManager = IntPtr.Zero;
            this.HSCManager = DrSrvHelper.OpenSCManager(serverName, null, (int)access);
            if (this.HSCManager.ToInt32() <= 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            OnAfterOpenSCM(new DrSrvEventArgsAfterOpenSCM(serverName, access, this.HSCManager)); // raise event after open SCM
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
            if (IntPtr.Zero != ptr)
            {
                OnBeforeCloseHandle(new DrSrvEventArgsHandle(ptr));
                return DrSrvHelper.CloseServiceHandle(ptr); //close openned SCM
            }
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
            var eBeforeArgs = new DrSrvEventArgsBeforeOpenService(serviceName, access, hSCManager);
            OnBeforeOpenService(eBeforeArgs); // raise event before open service
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED); // Canceled by user
            CloseHandle(this.HService);
            this.HService = IntPtr.Zero;
            this.HService = DrSrvHelper.OpenService(hSCManager, serviceName, (int)access);
            if (this.HService.ToInt32() <= 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            OnAfterOpenService(new DrSrvEventArgsAfterOpenService(serviceName, access, this.HSCManager, this.HService)); // raise event after open service
            return true;
        }
        #endregion OpenService
        #region GetServiceConfig
        /// <summary>
        /// Retrieves the configuration parameters of the current service. Optional configuration parameters are available using the GetServiceConfig2 function.
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <param name="config">out class contains configuration information for an installed service. It is used by the QueryServiceConfig function.</param>
        /// <returns></returns>
        public bool GetServiceConfig(string serviceName, out DrSrvHelper.QUERY_SERVICE_CONFIG config)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG)))
            {
                return GetServiceConfig(this.HService, out config);
            }
            config = new DrSrvHelper.QUERY_SERVICE_CONFIG();
            return false;
        }
        /// <summary>
        /// Retrieves the configuration parameters of the current service. Optional configuration parameters are available using the GetServiceConfig2 function.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights</param>
        /// <param name="config">out class contains configuration information for an installed service. It is used by the QueryServiceConfig function.</param>
        /// <returns></returns>
        public bool GetServiceConfig(out DrSrvHelper.QUERY_SERVICE_CONFIG config)
        {
            return GetServiceConfig(this.HService, out config);
        }
        /// <summary>
        /// Retrieves the configuration parameters of the specified service handle. Optional configuration parameters are available using the GetServiceConfig2 function.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights</param>
        /// <param name="config">out class contains configuration information for an installed service. It is used by the QueryServiceConfig function.</param>
        /// <returns></returns>
        public bool GetServiceConfig(IntPtr hService, out DrSrvHelper.QUERY_SERVICE_CONFIG config)
        {
            int needBuffer = 0;
            var ptr = new IntPtr(0);
            config = new DrSrvHelper.QUERY_SERVICE_CONFIG();
            var res = DrSrvHelper.QueryServiceConfig(hService, ptr, needBuffer, ref needBuffer);
            if (res != 0) return win32ErrorHandling(Marshal.GetLastWin32Error());

            var err = Marshal.GetLastWin32Error();
            // There is more service configuration information than would fit into the lpServiceConfig buffer. The number of bytes required to get all the information is returned in the pcbBytesNeeded parameter. Nothing is written to lpServiceConfig.
            if (err != DrSrvHelper.ERROR_INSUFFICIENT_BUFFER) return win32ErrorHandling(err);
            ptr = Marshal.AllocCoTaskMem(needBuffer);
            res = DrSrvHelper.QueryServiceConfig(hService, ptr, needBuffer, ref needBuffer);
            if (res == 0)
            {
                Marshal.FreeCoTaskMem(ptr);
                return win32ErrorHandling(Marshal.GetLastWin32Error());
            }
            var c = (DrSrvHelper.QUERY_SERVICE_CONFIG_PTR)Marshal.PtrToStructure(ptr, typeof(DrSrvHelper.QUERY_SERVICE_CONFIG_PTR));
            config = new DrSrvHelper.QUERY_SERVICE_CONFIG(c);
            Marshal.FreeCoTaskMem(ptr);
            return true;
        }
        #endregion GetServiceConfig
        #region GetServiceCurrentStatus
        /// <summary>
        /// Retrieves the current status of the specified service.
        /// </summary>     
        /// <param name="serviceName">service name</param>
        /// <param name="status">A SERVICE_STATUS structure that receives the status information.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool GetServiceCurrentStatus(string serviceName, out DrSrvHelper.SERVICE_STATUS status)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS)))
            {
                return GetServiceCurrentStatus(this.HService, out status);
            }
            status = new DrSrvHelper.SERVICE_STATUS();
            return false;
        }
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
        /// <summary>
        /// Wait expected specified service status. Raise event <paramref name="WaitExpectedStatus"/>
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <param name="state">expected status</param>
        /// <param name="timeOut">time out period is sec.</param>
        /// <returns></returns>
        public virtual bool ServiceWaitStatus(string serviceName, DrSrvHelper.SERVICE_CURRENT_STATE state, int timeOut)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS)))
            {
                return ServiceWaitStatus(this.HService, state, timeOut);
            }
            return false;
        }
        /// <summary>
        /// Wait expected service status. Raise event <paramref name="WaitExpectedStatus"/>
        /// </summary>
        /// <param name="state">expected status</param>
        /// <param name="timeOut">time out period is sec.</param>
        /// <returns></returns>
        public virtual bool ServiceWaitStatus(DrSrvHelper.SERVICE_CURRENT_STATE state, int timeOut)
        {
            return ServiceWaitStatus(this.HService, state, timeOut);
        }
        /// <summary>
        /// Wait expected service status. This function raises event <paramref name="WaitExpectedStatus"/>
        /// </summary>
        /// <param name="hService">a service handle</param>
        /// <param name="state">expected status</param>
        /// <param name="timeOut">time out period is sec.</param>
        /// <returns></returns>
        public virtual bool ServiceWaitStatus(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE state, int timeOut)
        {
            DrSrvHelper.SERVICE_STATUS currentServiceStatus;
            var eventArgs = new DrSrvEventArgsWaitExpectedServiceState(hService, state, timeOut);
            OnBeforeWaitExpectedServiceState((DrSrvEventArgsBeforeWaitExpectedServiceState)eventArgs);
            if (eventArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
            do
            {
                GetServiceCurrentStatus(hService, out currentServiceStatus);
                eventArgs.CurrentSrvState = currentServiceStatus.serviceState;
                if (currentServiceStatus.serviceState == state)
                {
                    Thread.Sleep(100);
                    OnAfterWaitExpectedServiceState(new DrSrvEventArgsAfterWaitExpectedServiceState(hService, state, timeOut - eventArgs.RemainigTimeOut));
                    return true;
                }
                OnWaitExpectedServiceState(eventArgs);
                if (eventArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
                Thread.Sleep(1000);
                eventArgs.RemainigTimeOut--;
                eventArgs.SpentOnTimeWait++;
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
        /// Sends a control code to a specified service. Use the following commands from <paramref name="DrSrvHelper.SERVICE_CONTROL"/> , for example, STOP, START etc. This function raises event <paramref name="WaitExpectedStatus"/>
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_START access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="serviceControl">service control code <paramref name="DrSrvHelper.SERVICE_CONTROL"/></param>
        /// <param name="expectedSrvState">expected service state<paramref name="DrSrvHelper.SERVICE_CURRENT_STATE"/></param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceControl(IntPtr hService, DrSrvHelper.SERVICE_CONTROL serviceControl, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int timeOut)
        {
            DrSrvHelper.SERVICE_STATUS currentSrvStatus;
            var eBeforeArgs = new DrSrvEventArgsBeforeServiceControl(hService, serviceControl, expectedSrvState, timeOut);
            OnBeforeServiceControl(eBeforeArgs);
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
            if (!GetServiceCurrentStatus(hService, out currentSrvStatus)) return false;
            if (currentSrvStatus.serviceState == expectedSrvState) return true; // exit if current service status equals expected service status. nothing to do
            // send service commond
            if (DrSrvHelper.ControlService(hService, serviceControl, ref currentSrvStatus) != true) return win32ErrorHandling(Marshal.GetLastWin32Error());
            OnAfterServiceControl((DrSrvEventArgsAfterServiceControl)eBeforeArgs);
            if (timeOut == 0) return true;
            return ServiceWaitStatus(hService, expectedSrvState, timeOut);
        }
        #region Stop
        /// <summary>
        /// Stops the specified service by name. If curent service handle is oppened this function will close openned service handle and open specified service with the following access:
        /// <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP"/> | <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS"/> | <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS"/>
        /// </summary>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="timeOut">time wait period expected service status in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <param name="stopDependences">If value is 'true' automatically stops dependent services, otherwise, stops only specified service</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceStop(string serviceName, int timeOut, bool stopDependences)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS)))
            {
                return ServiceStop(this.HService, timeOut, stopDependences);
            }
            return false;
        }
        /// <summary>
        /// Stops current service
        /// </summary>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <param name="stopDependences">If value is 'true' automatically stops dependent services, otherwise, stops only specified service</param>
        /// <returns></returns>
        public bool ServiceStop(int timeOut, bool stopDependences)
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
                    var access = (DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS);
                    foreach (var serviceStatus in depServices)
                    {
                        var eBeforeArgs = new DrSrvEventArgsBeforeOpenService(serviceStatus.pServiceName, access, this.HSCManager);
                        OnBeforeOpenService(eBeforeArgs);
                        if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED); // Canceled by user
                        hDService = DrSrvHelper.OpenService(this.HSCManager, serviceStatus.pServiceName, (int)access);
                        OnAfterOpenService(new DrSrvEventArgsAfterOpenService(serviceStatus.pServiceName, access, this.HSCManager, hDService));
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
        /// Starts the specified service by name. If current service handle is oppened this function will close openned service handle and open specified service with the following access:
        /// <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_START"/> | <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS"/>
        /// </summary>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="timeOut">time wait period expected service status in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceStart(string serviceName, int timeOut)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_START | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS)))
            {
                return ServiceStart(this.HService, timeOut);
            }
            return false;
        }

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
        /// Starts specified service. This function raises event <paramref name="WaitExpectedStatus"/>
        /// </summary>
        /// <param name="hService">a service handle</param>
        /// <param name="timeOut">time wait period in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <returns></returns>
        public bool ServiceStart(IntPtr hService, int timeOut)
        {
            var eBeforeArgs = new DrSrvEventArgsBeforeServiceStart(hService, timeOut);
            OnBeforeServiceStart(eBeforeArgs);
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
            if (DrSrvHelper.StartService(hService, 0, null) != true) return win32ErrorHandling(Marshal.GetLastWin32Error());
            OnAfterServiceStart((DrSrvEventArgsAfterServiceStart)eBeforeArgs);
            if (timeOut == 0) return true;
            return ServiceWaitStatus(hService, DrSrvHelper.SERVICE_CURRENT_STATE.SERVICE_RUNNING, timeOut);
        }
        #endregion Start
        #endregion ServiceControl
        #region ServiceDelete
        /// <summary>
        /// Marks the specified service for deletion from the service control manager database. If current service handle is oppened this function will close openned service handle and open specified service with the following access:
        /// <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_DELETE"/> 
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// </summary>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceDelete(string serviceName)
        {
            return ServiceDelete(serviceName, false, 0, false);
        }
        /// <summary>
        /// Marks the specified service for deletion from the service control manager database. If current service handle is oppened this function will close openned service handle and open specified service with the following access:
        /// <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_DELETE"/>  or <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP"/> | <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS"/> | <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS"/> depend on <paramref name="stopBeforeDelete"/>
        /// <param name="serviceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="stopBeforeDelete">stops service before marks for deleteion</param>
        /// <param name="timeOut">time wait period expected service status in sec. If parameter is 0 function will not wait expected service status.</param>
        /// <param name="stopDependences">If value is 'true' automatically stops dependent services, otherwise, stops only specified service</param>
        /// </summary>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceDelete(string serviceName, bool stopBeforeDelete, int timeOut, bool stopDependences)
        {
            var access = (stopBeforeDelete ? (DrSrvHelper.SERVICE_ACCESS.SERVICE_STOP | DrSrvHelper.SERVICE_ACCESS.SERVICE_DELETE | DrSrvHelper.SERVICE_ACCESS.SERVICE_ENUMERATE_DEPENDENTS | DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_STATUS) : DrSrvHelper.SERVICE_ACCESS.SERVICE_DELETE);

            if (!OpenService(serviceName, access)) return false;
            if (stopBeforeDelete)
            {
                if (ServiceStop(this.HService, timeOut, stopDependences)) return false;
            }
            return ServiceDelete(this.HService);
        }
        /// <summary>
        /// Marks the current service for deletion from the service control manager database.
        /// </summary>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceDelete()
        {
            return ServiceDelete(this.HService);
        }
        /// <summary>
        /// Marks the specified service for deletion from the service control manager database.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the DELETE access right. For more information, see Service Security and Access Rights.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool ServiceDelete(IntPtr hService)
        {
            var eBeforeArgs = new DrSrvEventArgsBeforeServiceDelete(hService);
            OnBeforeServiceDelete(eBeforeArgs);
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);
            if (!DrSrvHelper.DeleteService(hService)) return win32ErrorHandling(Marshal.GetLastWin32Error());
            OnAfterServiceDelete((DrSrvEventArgService)eBeforeArgs);
            return true;
        }
        #endregion Service Delete
        #region SetServiceDescription
        /// <summary>
        /// Sets description to specified service. If current service handle is oppened this function will close openned service handle and open specified service with the following access:
        /// <paramref name="DrSrvHelper.SERVICE_ACCESS.SERVICE_CHANGE_CONFIG"/>
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <param name="description">The description of the service. If this member is NULL, the description remains unchanged. If this value is an empty string (""), the current description is deleted. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool SetServiceDescription(string serviceName, string description)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_CHANGE_CONFIG)))
            {
                return SetServiceDescription(description);
            }
            return false;
        }
        /// <summary>
        /// Sets description to current service 
        /// </summary>
        /// <param name="description">The description of the service. If this member is NULL, the description remains unchanged. If this value is an empty string (""), the current description is deleted. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool SetServiceDescription(string description)
        {
            return SetServiceDescription(this.HService, description);
        }
        /// <summary>
        /// Sets description to specified service 
        /// </summary>
        /// <param name="hService">hadle of service</param>
        /// <param name="description">The description of the service. If this member is NULL, the description remains unchanged. If this value is an empty string (""), the current description is deleted. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool SetServiceDescription(IntPtr hService, string description)
        {
            DrSrvHelper.SERVICE_DESCRIPTION info;
            info.lpDescription = description;
            return SetServiceConfig2<DrSrvHelper.SERVICE_DESCRIPTION>(DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION, info);
        }
        #endregion SetServiceDescription
        #region SetServiceDelayAutostartInfo
        /// <summary>
        /// Sets the delayed auto-start setting of an auto-start service.
        /// </summary>
        /// <param name="hService">a service name</param>
        /// <param name="delayedAutostart">If this member is TRUE, the service is started after other auto-start services are started plus a short delay. Otherwise, the service is started during system boot. This setting is ignored unless the service is an auto-start service.</param>
        /// <returns></returns>
        public bool SetServiceDelayAutostartInfo(string serviceName, bool delayedAutostart)
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_CHANGE_CONFIG)))
            {
                return SetServiceDelayAutostartInfo(delayedAutostart);
            }
            return false;
        }
        /// <summary>
        /// Sets the delayed auto-start setting of an auto-start current service.
        /// </summary>
        /// <param name="delayedAutostart">If this member is TRUE, the service is started after other auto-start services are started plus a short delay. Otherwise, the service is started during system boot. This setting is ignored unless the service is an auto-start service.</param>
        /// <returns></returns>
        public bool SetServiceDelayAutostartInfo(bool delayedAutostart)
        {
            return SetServiceDelayAutostartInfo(this.HService, delayedAutostart);
        }
        /// <summary>
        /// Sets the delayed auto-start setting of an auto-start service.
        /// </summary>
        /// <param name="hService">a handle of service</param>
        /// <param name="delayedAutostart">If this member is TRUE, the service is started after other auto-start services are started plus a short delay. Otherwise, the service is started during system boot. This setting is ignored unless the service is an auto-start service.</param>
        /// <returns></returns>
        public bool SetServiceDelayAutostartInfo(IntPtr hService, bool delayedAutostart)
        {
            DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO info;
            info.fDelayedAutostart = delayedAutostart;
            return SetServiceConfig2<DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO>(DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO, info);
        }
        #endregion SetServiceDelayAutostartInfo
        #region SetServiceConfig2
        /// <summary>
        /// Changes the optional configuration parameters of a service.
        /// </summary>
        /// <typeparam name="T">structure of service info.</typeparam>
        /// <param name="serviceName">a service name</param>
        /// <param name="level">The configuration information to be changed. This parameter can be one of the following values <paramref name="DrSrvHelper.INFO_LEVEL"/> </param>
        /// <param name="info">supported structure, for example, SERVICE_DELAYED_AUTO_START_INFO</param>
        /// <returns></returns>
        public bool SetServiceConfig2<T>(string serviceName, DrSrvHelper.INFO_LEVEL level, T info) where T : new()
        {
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_CHANGE_CONFIG)))
            {
                return SetServiceConfig2<T>(level, info);
            }
            return false;
        }
        /// <summary>
        /// Changes the optional configuration parameters of the current service.
        /// </summary>
        /// <typeparam name="T">structure of service info.</typeparam>
        ///  If the service controller handles the SC_ACTION_RESTART action, hService must have the SERVICE_START access right.</param>
        /// <param name="level">The configuration information to be changed. This parameter can be one of the following values <paramref name="DrSrvHelper.INFO_LEVEL"/> </param>
        /// <param name="info">supported structure, for example, SERVICE_DELAYED_AUTO_START_INFO</param>
        /// <returns></returns>
        public bool SetServiceConfig2<T>(DrSrvHelper.INFO_LEVEL level, T info) where T : new()
        {
            return SetServiceConfig2<T>(this.HService, level, info);
        }
        /// <summary>
        /// Changes the optional configuration parameters of a service.
        /// </summary>
        /// <typeparam name="T">structure of service info.</typeparam>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function and must have the SERVICE_CHANGE_CONFIG access right. For more information, see Service Security and Access Rights.
        ///  If the service controller handles the SC_ACTION_RESTART action, hService must have the SERVICE_START access right.</param>
        /// <param name="level">The configuration information to be changed. This parameter can be one of the following values <paramref name="DrSrvHelper.INFO_LEVEL"/> </param>
        /// <param name="info">supported structure, for example, SERVICE_DELAYED_AUTO_START_INFO</param>
        /// <returns></returns>
        public bool SetServiceConfig2<T>(IntPtr hService, DrSrvHelper.INFO_LEVEL level, T info) where T : new()
        {
            IntPtr ptrInfo = Marshal.AllocHGlobal(Marshal.SizeOf(info.GetType()));
            Marshal.StructureToPtr(info, ptrInfo, false);
            if (!DrSrvHelper.ChangeServiceConfig2(hService, level, ptrInfo)) return win32ErrorHandling(Marshal.GetLastWin32Error());
            Marshal.FreeHGlobal(ptrInfo);
            return true;
        }
        #endregion SetServiceConfig2
        #region GetServiceConfig2
        /// <summary>
        /// Retrieves the optional configuration parameters of the specified service.
        /// </summary>
        /// <typeparam name="T">optional configuration parametrs. 
        /// <paramref name="DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_DESCRIPTION" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS_FLAG" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PREFERRED_NODE_INFO" />   - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PRESHUTDOWN_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_REQUIRED_PRIVILEGES_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_SERVICE_SID_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_TRIGGER_INFO" />  - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_LAUNCH_PROTECTED" />  - Note  This value is supported starting with Windows 8.1.
        /// </typeparam>
        /// <param name="serviceName">a name of a service</param>
        /// <param name="level">The configuration information to be queried. This parameter can be one of the following values
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO" /> = 3,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION" /> = 1,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS" /> = 2,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG" /> = 4,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PREFERRED_NODE" /> = 9,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PRESHUTDOWN_INFO" /> = 7,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO" /> = 6,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_SERVICE_SID_INFO" /> = 5,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_TRIGGER_INFO" /> = 8,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_LAUNCH_PROTECTED" /> = 12
        /// </param>
        /// <param name="info">out configuration information</param>
        /// <returns></returns>
        public bool GetServiceConfig2<T>(string serviceName, DrSrvHelper.INFO_LEVEL level, out T info) where T : new()
        {
            info = new T();
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG)))
            {
                return GetServiceConfig2<T>(this.HService, level, out info);
            }
            return false;
        }
        /// <summary>
        /// Retrieves the optional configuration parameters of the current service.
        /// </summary>
        /// <typeparam name="T">optional configuration parametrs. 
        /// <paramref name="DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_DESCRIPTION" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS_FLAG" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PREFERRED_NODE_INFO" />   - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PRESHUTDOWN_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_REQUIRED_PRIVILEGES_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_SERVICE_SID_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_TRIGGER_INFO" />  - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_LAUNCH_PROTECTED" />  - Note  This value is supported starting with Windows 8.1.
        /// </typeparam>
        /// <param name="level">The configuration information to be queried. This parameter can be one of the following values
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO" /> = 3,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION" /> = 1,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS" /> = 2,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG" /> = 4,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PREFERRED_NODE" /> = 9,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PRESHUTDOWN_INFO" /> = 7,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO" /> = 6,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_SERVICE_SID_INFO" /> = 5,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_TRIGGER_INFO" /> = 8,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_LAUNCH_PROTECTED" /> = 12
        /// </param>
        /// <param name="info">out configuration information</param>
        /// <returns></returns>
        public bool GetServiceConfig2<T>(DrSrvHelper.INFO_LEVEL level, out T info) where T : new()
        {
            return GetServiceConfig2<T>(this.HService, level, out info);
        }
        /// <summary>
        /// Retrieves the optional configuration parameters of the specified service.
        /// </summary>
        /// <typeparam name="T">optional configuration parametrs. 
        /// <paramref name="DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_DESCRIPTION" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS" /> ,
        /// <paramref name="DrSrvHelper.SERVICE_FAILURE_ACTIONS_FLAG" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PREFERRED_NODE_INFO" />   - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_PRESHUTDOWN_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_REQUIRED_PRIVILEGES_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_SERVICE_SID_INFO" />  - Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_TRIGGER_INFO" />  - Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported,
        /// <paramref name="DrSrvHelper.SERVICE_LAUNCH_PROTECTED" />  - Note  This value is supported starting with Windows 8.1.
        /// </typeparam>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function and must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="level">The configuration information to be queried. This parameter can be one of the following values
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO" /> = 3,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION" /> = 1,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS" /> = 2,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG" /> = 4,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PREFERRED_NODE" /> = 9,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_PRESHUTDOWN_INFO" /> = 7,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO" /> = 6,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_SERVICE_SID_INFO" /> = 5,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_TRIGGER_INFO" /> = 8,
        /// <paramref name="DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_LAUNCH_PROTECTED" /> = 12
        /// </param>
        /// <param name="info">out configuration information</param>
        /// <returns></returns>
        public bool GetServiceConfig2<T>(IntPtr hService, DrSrvHelper.INFO_LEVEL level, out T info) where T : new()
        {
            IntPtr ptr;
            info = new T();
            if (!getServiceConfig2(hService, level, out ptr)) return false;
            info = (T)Marshal.PtrToStructure(ptr, info.GetType());
            Marshal.FreeCoTaskMem(ptr);
            return true;
        }
        /// <summary>
        /// Retrieves the optional configuration parameters of the specified service. Returns out pointer to structure of DrSrvHelper.INFO_LEVEL. You should call Marshal.FreeCoTaskMem(ptr) 
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function and must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="level">The configuration information to be queried. This parameter can be one of the following values from <paramref name="DrSrvHelper.INFO_LEVEL"/></param>
        /// <param name="ptr">A pointer to the buffer that receives the service configuration information. The format of this data depends on the value of the dwInfoLevel parameter.
        /// The maximum size of this array is 8K bytes. To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. The function fails and GetLastError returns ERROR_INSUFFICIENT_BUFFER. The pcbBytesNeeded parameter receives the needed size.</param>
        /// <returns></returns>
        private bool getServiceConfig2(IntPtr hService, DrSrvHelper.INFO_LEVEL level, out IntPtr ptr)
        {
            int needBuffer = 0;
            ptr = new IntPtr(0);
            var res = DrSrvHelper.QueryServiceConfig2(hService, level, ptr, needBuffer, ref needBuffer);
            if (res != 0) return win32ErrorHandling(Marshal.GetLastWin32Error());

            var err = Marshal.GetLastWin32Error();

            if (err != DrSrvHelper.ERROR_INSUFFICIENT_BUFFER) return win32ErrorHandling(err);
            ptr = Marshal.AllocCoTaskMem(needBuffer);
            res = DrSrvHelper.QueryServiceConfig2(hService, level, ptr, needBuffer, ref needBuffer);
            if (res == 0)
            {
                Marshal.FreeCoTaskMem(ptr);
                return win32ErrorHandling(Marshal.GetLastWin32Error());
            }
            return true;
        }
        #endregion GetServiceConfig2
        #region GetServiceDelayAutostartInfo()
        /// <summary>
        /// Returns the delayed auto-start setting of an auto-start specified service.
        /// </summary>
        /// <param name="serviceName">a service name</param>
        /// <param name="delayedAutostart">returns the delayed auto-start setting of an auto-start service.</param>
        /// <returns></returns>
        public bool GetServiceDelayAutostartInfo(string serviceName, out bool delayedAutostart)
        {
            delayedAutostart = false;
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG)))
            {
                return GetServiceDelayAutostartInfo(out delayedAutostart);
            }
            return false;
        }
        /// <summary>
        /// Returns the delayed auto-start setting of an auto-start current service
        /// </summary>
        /// <param name="delayedAutostart">returns the delayed auto-start setting of an auto-start service.</param>
        /// <returns></returns>
        public bool GetServiceDelayAutostartInfo(out bool delayedAutostart)
        {
            return GetServiceDelayAutostartInfo(this.HService, out delayedAutostart);
        }
        /// <summary>
        /// Returns the delayed auto-start setting of an auto-start service.
        /// </summary>
        /// <param name="hService">a handle of service</param>
        /// <param name="delayedAutostart">returns the delayed auto-start setting of an auto-start service.</param>
        /// <returns></returns>
        public bool GetServiceDelayAutostartInfo(IntPtr hService, out bool delayedAutostart)
        {
            DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO info;
            delayedAutostart = false;
            if (!GetServiceConfig2<DrSrvHelper.SERVICE_DELAYED_AUTO_START_INFO>(hService, DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DELAYED_AUTO_START_INFO, out info)) return false;
            delayedAutostart = info.fDelayedAutostart;
            return true;
        }
        #endregion GetServiceDelayAutostartInfo()
        #region GetServiceDescription()
        /// <summary>
        /// Returns a service description.
        /// </summary>
        /// <param name="serviceName">a name of service</param>
        /// <param name="description">returns a service description.</param>
        /// <returns></returns>
        public bool GetServiceDescription(string serviceName, out string description)
        {
            description = string.Empty;
            if (OpenService(serviceName, (DrSrvHelper.SERVICE_ACCESS.SERVICE_QUERY_CONFIG)))
            {
                return GetServiceDescription(out description);
            }
            return false;
        }
        /// <summary>
        /// Returns a current service description.
        /// </summary>
        /// <param name="description">returns a service description.</param>
        /// <returns></returns>
        public bool GetServiceDescription(out string description)
        {
            return GetServiceDescription(this.HService, out description);
        }
        /// <summary>
        /// Returns a service description.
        /// </summary>
        /// <param name="hService">a handle of service</param>
        /// <param name="description">returns a service description.</param>
        /// <returns></returns>
        public bool GetServiceDescription(IntPtr hService, out string description)
        {
            DrSrvHelper.SERVICE_DESCRIPTION info;
            description = string.Empty;
            if (!GetServiceConfig2<DrSrvHelper.SERVICE_DESCRIPTION>(hService, DrSrvHelper.INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION, out info)) return false;
            description = info.lpDescription;
            return true;
        }
        #endregion GetServiceDescription()
        #region CreateService
        /// <summary>
        /// Creates a service object and adds it to the current service control manager database. This service will run as LOCAL_SYSTEM service. Sets handle of new service to current service handle field <paramref name="HService"/>. If <paramref name="HService"/> has an openned service handle this handle will closed.
        /// </summary>
        /// <param name="serviceName">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="displayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="description">The description of the service. If value is not empty or null function will set description for new service. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <param name="desiredAccess">The access to the service. Before granting the requested access, the system checks the access token of the calling process. For a list of values, see Service Security and Access Rights. <paramref name="SERVICE_ACCESS"/></param>
        /// <param name="serviceType">The service type. This parameter can be one of the following values. <paramref name="SERVICE_TYPE"/></param>
        /// <param name="startType">The service start options. This parameter can be one of the following values. <paramref name="SERVICE_START_TYPE"/></param>
        /// <param name="errorControl">The severity of the error, and action taken, if this service fails to start. This parameter can be one of the following values. <paramref name="SERVICE_ERROR_TYPE"/> </param>
        /// <param name="binaryPathName">The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).
        /// If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</param>
        /// <param name="dependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service. Specify NULL or an empty string if the service has no dependencies. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
        /// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, because services and service groups share the same name space.</param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool CreateService(string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string dependencies)
        {
            return this.CreateService(serviceName,
                                        displayName,
                                        description,
                                        desiredAccess,
                                        serviceType,
                                        startType,
                                        errorControl,
                                        binaryPathName,
                                        null,
                                        IntPtr.Zero,
                                        dependencies,
                                        null,
                                        null);
        }
        /// <summary>
        /// Creates a service object and adds it to the current service control manager database. Sets handle of new service to current service handle field <paramref name="HService"/>. If <paramref name="HService"/> has an openned service handle this handle will closed.
        /// </summary>
        /// <param name="serviceName">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="displayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="description">The description of the service. If value is not empty or null function will set description for new service. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <param name="desiredAccess">The access to the service. Before granting the requested access, the system checks the access token of the calling process. For a list of values, see Service Security and Access Rights. <paramref name="SERVICE_ACCESS"/></param>
        /// <param name="serviceType">The service type. This parameter can be one of the following values. <paramref name="SERVICE_TYPE"/></param>
        /// <param name="startType">The service start options. This parameter can be one of the following values. <paramref name="SERVICE_START_TYPE"/></param>
        /// <param name="errorControl">The severity of the error, and action taken, if this service fails to start. This parameter can be one of the following values. <paramref name="SERVICE_ERROR_TYPE"/> </param>
        /// <param name="binaryPathName">The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).
        /// If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</param>
        /// <param name="loadOrderGroup">The names of the load ordering group of which this service is a member. Specify NULL or an empty string if the service does not belong to a group.
        /// The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder</param>
        /// <param name="tagId">A pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.
        /// You can use a tag for ordering service startup within a load ordering group by specifying a tag order vector in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
        /// Tags are only evaluated for driver services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.</param>
        /// <param name="dependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service. Specify NULL or an empty string if the service has no dependencies. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
        /// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, because services and service groups share the same name space.</param>
        /// <param name="userName">The name of the account under which the service should run. If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. The service process will be logged on as this user. If the account belongs to the built-in domain, you can specify .\UserName.
        /// If this parameter is NULL, CreateService uses the LocalSystem account. If the service type specifies SERVICE_INTERACTIVE_PROCESS, the service must run in the LocalSystem account.
        /// If this parameter is NT AUTHORITY\LocalService, CreateService uses the LocalService account. If the parameter is NT AUTHORITY\NetworkService, CreateService uses the NetworkService account.
        /// A shared process can run as any user.
        /// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, the name is the driver object name that the system uses to load the device driver. Specify NULL if the driver is to use a default object name created by the I/O system.
        /// A service can be configured to use a managed account or a virtual account. If the service is configured to use a managed service account, the name is the managed service account name. If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName. For more information about managed service accounts and virtual accounts, see the Service Accounts Step-by-Step Guide.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  Managed service accounts and virtual accounts are not supported until Windows 7 and Windows Server 2008 R2.</param>
        /// <param name="password">The password to the account name specified by the lpServiceStartName parameter. Specify an empty string if the account has no password or if the service runs in the LocalService, NetworkService, or LocalSystem account. For more information, see Service Record List.
        /// If the account name specified by the lpServiceStartName parameter is the name of a managed service account or virtual account name, the lpPassword parameter must be NULL.
        /// Passwords are ignored for driver services.</param>
        /// <param name="password"></param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool CreateService(string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string loadOrderGroup,
                                  IntPtr tagId,
                                  string dependencies,
                                  string userName,
                                  string password)
        {
            return this.CreateService(this.HSCManager,
                                        serviceName,
                                        displayName,
                                        description,
                                        desiredAccess,
                                        serviceType,
                                        startType,
                                        errorControl,
                                        binaryPathName,
                                        loadOrderGroup,
                                        tagId,
                                        dependencies,
                                        userName,
                                        password);
        }
        /// <summary>
        /// Creates a service object and adds it to the specified service control manager database. Sets handle of new service to current service handle field <paramref name="HService"/>. If <paramref name="HService"/> has an openned service handle this handle will closed.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database. This handle is returned by the OpenSCManager function and must have the SC_MANAGER_CREATE_SERVICE access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="serviceName">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="displayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="description">The description of the service. If value is not empty or null function will set description for new service. The service description must not exceed the size of a registry value of type REG_SZ. This member can specify a localized string using the following format: @[path\]dllname,-strID The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString. Windows Server 2003 and Windows XP/2000:  Localized strings are not supported until Windows Vista.</param>
        /// <param name="desiredAccess">The access to the service. Before granting the requested access, the system checks the access token of the calling process. For a list of values, see Service Security and Access Rights. <paramref name="SERVICE_ACCESS"/></param>
        /// <param name="serviceType">The service type. This parameter can be one of the following values. <paramref name="SERVICE_TYPE"/></param>
        /// <param name="startType">The service start options. This parameter can be one of the following values. <paramref name="SERVICE_START_TYPE"/></param>
        /// <param name="errorControl">The severity of the error, and action taken, if this service fails to start. This parameter can be one of the following values. <paramref name="SERVICE_ERROR_TYPE"/> </param>
        /// <param name="binaryPathName">The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).
        /// If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</param>
        /// <param name="loadOrderGroup">The names of the load ordering group of which this service is a member. Specify NULL or an empty string if the service does not belong to a group.
        /// The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder</param>
        /// <param name="tagId">A pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.
        /// You can use a tag for ordering service startup within a load ordering group by specifying a tag order vector in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
        /// Tags are only evaluated for driver services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.</param>
        /// <param name="dependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service. Specify NULL or an empty string if the service has no dependencies. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
        /// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, because services and service groups share the same name space.</param>
        /// <param name="userName">The name of the account under which the service should run. If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. The service process will be logged on as this user. If the account belongs to the built-in domain, you can specify .\UserName.
        /// If this parameter is NULL, CreateService uses the LocalSystem account. If the service type specifies SERVICE_INTERACTIVE_PROCESS, the service must run in the LocalSystem account.
        /// If this parameter is NT AUTHORITY\LocalService, CreateService uses the LocalService account. If the parameter is NT AUTHORITY\NetworkService, CreateService uses the NetworkService account.
        /// A shared process can run as any user.
        /// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, the name is the driver object name that the system uses to load the device driver. Specify NULL if the driver is to use a default object name created by the I/O system.
        /// A service can be configured to use a managed account or a virtual account. If the service is configured to use a managed service account, the name is the managed service account name. If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName. For more information about managed service accounts and virtual accounts, see the Service Accounts Step-by-Step Guide.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  Managed service accounts and virtual accounts are not supported until Windows 7 and Windows Server 2008 R2.</param>
        /// <param name="password">The password to the account name specified by the lpServiceStartName parameter. Specify an empty string if the account has no password or if the service runs in the LocalService, NetworkService, or LocalSystem account. For more information, see Service Record List.
        /// If the account name specified by the lpServiceStartName parameter is the name of a managed service account or virtual account name, the lpPassword parameter must be NULL.
        /// Passwords are ignored for driver services.</param>
        /// <param name="password"></param>
        /// <returns>if the function succeeds, the return true, otherwise, the return false or throw win32exception depend on <paramref name="AllowThrowWin32Exception"/></returns>
        public bool CreateService(IntPtr hSCManager,
                                  string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string loadOrderGroup,
                                  IntPtr tagId,
                                  string dependencies,
                                  string userName,
                                  string password)
        {

            var eBeforeArgs = new DrSrvEventArgsBeforeCreateService(hSCManager,
                                                                    serviceName,
                                                                    displayName,
                                                                    description,
                                                                    desiredAccess,
                                                                    serviceType,
                                                                    startType,
                                                                    errorControl,
                                                                    binaryPathName,
                                                                    loadOrderGroup,
                                                                    dependencies,
                                                                    userName);

            OnBeforeCreateService(eBeforeArgs);
            if (eBeforeArgs.Cancel) return win32ErrorHandling(DrSrvHelper.ERROR_CANCELLED);

            IntPtr hService = DrSrvHelper.CreateService(hSCManager,
                                                        serviceName,
                                                        displayName,
                                                        desiredAccess,
                                                        serviceType,
                                                        startType,
                                                        errorControl,
                                                        binaryPathName,
                                                        loadOrderGroup,
                                                        tagId,
                                                        dependencies,
                                                        userName,
                                                        password);
            if (hService.ToInt32() <= 0) return win32ErrorHandling(Marshal.GetLastWin32Error());
            CloseHandle(this.HService);
            this.HService = hService;
            OnAfterCreateService(new DrSrvEventArgsAfterCreateService(hService,
                                                                    serviceName,
                                                                    displayName,
                                                                    description,
                                                                    desiredAccess,
                                                                    serviceType,
                                                                    startType,
                                                                    errorControl,
                                                                    binaryPathName,
                                                                    loadOrderGroup,
                                                                    dependencies,
                                                                    userName));
            if (String.IsNullOrEmpty(description)) return true;
            return SetServiceDescription(description);
        }
        #endregion CreateService
    }
}
