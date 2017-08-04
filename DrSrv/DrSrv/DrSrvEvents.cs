/*
  DrSrvEvents.cs -- Provides events from the DrSrvMgr, July 23, 2017
 
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

namespace DrOpen.DrCommon.DrSrv
{
    public abstract class DrSrvEventArgService : EventArgs
    {
        public DrSrvEventArgService(IntPtr hService)
        {
            this.HService = hService;
        }
        /// <summary>
        /// Contians a handle to the current service.
        /// </summary>
        public virtual IntPtr HService { get; private set; }
    }
    #region OpenSCM

    public abstract class DrSrvEventArgsOpenSCM : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DrSrvEventArgsOpenSCM
        /// </summary>
        /// <param name="serverName">Connect to specified computer name</param>
        /// <param name="access">The access to the service control manager.</param>
        public DrSrvEventArgsOpenSCM(string serverName, DrSrvHelper.SC_MANAGER access)
        {
            this.ServerName = serverName;
            this.Access = access;
        }
        /// <summary>
        /// Contians computer name for establishing a connection to the service control manager
        /// </summary>
        public virtual string ServerName { get; private set; }
        /// <summary>
        /// The access to the service control manager.
        /// </summary>
        public virtual DrSrvHelper.SC_MANAGER Access { get; private set; }
    }

    /// <summary>
    /// DrSrvEventArgsBeforeOpenSCM contains event data about before connection will be established to the service control manager on the specified computer
    /// </summary>
    public class DrSrvEventArgsBeforeOpenSCM : DrSrvEventArgsOpenSCM
    {
        /// <summary>
        /// Initializes a new instance of the DrSrvEventArgsBeforeOpenSCM
        /// </summary>
        /// <param name="serverName">Connect to specified computer name</param>
        /// <param name="access">The access to the service control manager.</param>
        public DrSrvEventArgsBeforeOpenSCM(string serverName, DrSrvHelper.SC_MANAGER access)
            : base(serverName, access)
        {
            this.Cancel = false;
        }
        /// <summary>
        /// Set value true for Cancel this operation
        /// </summary>
        public virtual bool Cancel { get; set; }
    }

    /// <summary>
    /// DrSrvEventArgsAfterOpenSCM contains event data about before connection will be established to the service control manager on the specified computer
    /// </summary>
    public class DrSrvEventArgsAfterOpenSCM : DrSrvEventArgsOpenSCM
    {
        /// <summary>
        /// Initializes a new instance of the DrSrvEventArgsAfterOpenSCM
        /// </summary>
        /// <param name="serverName">Connect to specified computer name</param>
        /// <param name="hSCM">A handle to the service control manager database. The OpenSCManager function returns this handle.</param>
        public DrSrvEventArgsAfterOpenSCM(string serverName, DrSrvHelper.SC_MANAGER access, IntPtr hSCM)
            : base(serverName, access)
        {
            this.HSCM = hSCM;
        }
        /// <summary>
        /// Contians a handle to the service control manager database.
        /// </summary>
        public virtual IntPtr HSCM { get; private set; }
    }
    #endregion OpenSCM
    #region OpenService
    public abstract class DrSrvEventArgsOpenService : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DrDrvEventArgsOpenService
        /// </summary>
        /// <param name="serviceName">Open the specified service name</param>
        /// <param name="access">The access to the service</param>
        /// <param name="hSCM">a handle to SCM</param>
        public DrSrvEventArgsOpenService(string serviceName, DrSrvHelper.SERVICE_ACCESS access, IntPtr hSCM)
        {
            this.ServiceName = serviceName;
            this.Access = access;
            this.HSCM = hSCM;
        }
        /// <summary>
        /// Contians computer name for establishing a connection to the service control manager
        /// </summary>
        public virtual string ServiceName { get; private set; }
        /// <summary>
        /// The access to the service control manager.
        /// </summary>
        public virtual DrSrvHelper.SERVICE_ACCESS Access { get; private set; }
        /// <summary>
        /// Contians a handle to the service control manager database.
        /// </summary>
        public virtual IntPtr HSCM { get; private set; }
    }

    /// <summary>
    /// DrSrvEventArgsBeforeOpenService contains event data about before connection will be established to the service control manager on the specified computer
    /// </summary>
    public class DrSrvEventArgsBeforeOpenService : DrSrvEventArgsOpenService
    {
        /// <summary>
        /// Initializes a new instance of the DrSrvEventArgsBeforeOpenSCM
        /// </summary>
        /// <param name="serverName">Connect to specified computer name</param>
        /// <param name="access">The access to the service control manager.
        public DrSrvEventArgsBeforeOpenService(string serviceName, DrSrvHelper.SERVICE_ACCESS access, IntPtr hSCM)
            : base(serviceName, access, hSCM)
        {
            this.Cancel = false;
        }
        /// <summary>
        /// Set value true for Cancel this operation
        /// </summary>
        public virtual bool Cancel { get; set; }
    }

    /// <summary>
    /// DrSrvEventArgsAfterOpenService contains event data about before connection will be established to the service control manager on the specified computer
    /// </summary>
    public class DrSrvEventArgsAfterOpenService : DrSrvEventArgsOpenService
    {
        /// <summary>
        /// Initializes a new instance of the DrSrvEventArgsAfterOpenSCM
        /// </summary>
        /// <param name="serverName">Connect to specified computer name</param>
        /// <param name="hSCM">A handle to the service control manager database. The OpenSCManager function returns this handle.</param>
        public DrSrvEventArgsAfterOpenService(string serviceName, DrSrvHelper.SERVICE_ACCESS access, IntPtr hSCM, IntPtr hService)
            : base(serviceName, access, hSCM)
        {
            this.HService = hService;
        }
        /// <summary>
        /// Contians a handle to the current service.
        /// </summary>
        public virtual IntPtr HService { get; private set; }
    }
    #endregion OpenService

    #region WaitExpectedServiceState
    public abstract class DrSrvEventArgsWaitServiceState : DrSrvEventArgService
    {
        public DrSrvEventArgsWaitServiceState(IntPtr hService, int remainigTimeOut)
            : base(hService)
        {
            this.RemainigTimeOut = remainigTimeOut;
        }
        /// <summary>
        /// Remaining timeout
        /// </summary>
        public int RemainigTimeOut { get; set; }

    }
    public class DrSrvEventArgsAfterWaitExpectedServiceState : DrSrvEventArgService
    {
        public DrSrvEventArgsAfterWaitExpectedServiceState(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE currentSrvState, int spentTimeOnWait)
            : base(hService)
        {
            this.SpentOnTimeWait = spentTimeOnWait;
            this.CurrentSrvState = currentSrvState;

        }
        public int SpentOnTimeWait { get; private set; }
        /// <summary>
        /// current service status
        /// </summary>
        public DrSrvHelper.SERVICE_CURRENT_STATE CurrentSrvState { get; private set; }
    }
    public class DrSrvEventArgsBeforeWaitExpectedServiceState : DrSrvEventArgsWaitServiceState
    {
        public DrSrvEventArgsBeforeWaitExpectedServiceState(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int remainigTimeOut)
            : base(hService, remainigTimeOut)
        {
            this.Cancel = false;
            this.ExpectedSrvState = expectedSrvState;
            this.RemainigTimeOut = remainigTimeOut;
        }
        /// <summary>
        /// set true to Cancel operation
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// expected service status
        /// </summary>
        public DrSrvHelper.SERVICE_CURRENT_STATE ExpectedSrvState { get; private set; }
    }

    public class DrSrvEventArgsWaitExpectedServiceState : DrSrvEventArgsBeforeWaitExpectedServiceState
    {
        public DrSrvEventArgsWaitExpectedServiceState(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int remainigTimeOut)
            : base(hService, expectedSrvState, remainigTimeOut) { }
        public DrSrvEventArgsWaitExpectedServiceState(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, DrSrvHelper.SERVICE_CURRENT_STATE currentSrvState, int remainigTimeOut)
            : base(hService, expectedSrvState, remainigTimeOut)
        {
            this.CurrentSrvState = currentSrvState;
            this.SpentOnTimeWait = 0;
        }
        /// <summary>
        /// current service status
        /// </summary>
        public DrSrvHelper.SERVICE_CURRENT_STATE CurrentSrvState { get; set; }
        public int SpentOnTimeWait { get; set; }
    }
    #endregion WaitExpectedServiceState
    #region ServiceControl
    public class DrSrvEventArgsAfterServiceControl : DrSrvEventArgService
    {

        public DrSrvEventArgsAfterServiceControl(IntPtr hService, DrSrvHelper.SERVICE_CONTROL serviceControl)
            : base(hService)
        {
            this.ServiceControl = serviceControl;
        }
        public DrSrvHelper.SERVICE_CONTROL ServiceControl { get; private set; }

    }

    public class DrSrvEventArgsBeforeServiceControl : DrSrvEventArgsAfterServiceControl
    {

        public DrSrvEventArgsBeforeServiceControl(IntPtr hService, DrSrvHelper.SERVICE_CONTROL serviceControl, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState, int timeOut)
            : base(hService, serviceControl)
        {
            this.ExpectedSrvState = expectedSrvState;
            this.TimeOut = timeOut;
            this.Cancel = false;
        }
        /// <summary>
        /// Set value true for Cancel this operation
        /// </summary>
        public virtual bool Cancel { get; set; }
        public DrSrvHelper.SERVICE_CURRENT_STATE ExpectedSrvState { get; private set; }
        public int TimeOut { get; private set; }
    }

    #endregion ServiceControl

    #region ServiceStart
    public class DrSrvEventArgsAfterServiceStart : DrSrvEventArgService
    {

        public DrSrvEventArgsAfterServiceStart(IntPtr hService)
            : base(hService)
        {
        }
    }

    public class DrSrvEventArgsBeforeServiceStart : DrSrvEventArgsAfterServiceStart
    {

        public DrSrvEventArgsBeforeServiceStart(IntPtr hService, int timeOut)
            : base(hService)
        {
            this.TimeOut = timeOut;
            this.Cancel = false;
        }
        /// <summary>
        /// Set value true for Cancel this operation
        /// </summary>
        public virtual bool Cancel { get; set; }
        public DrSrvHelper.SERVICE_CURRENT_STATE ExpectedSrvState { get; private set; }
        public int TimeOut { get; private set; }
    }

    #endregion ServiceStart
    #region CloseHandle
    public class DrSrvEventArgsHandle : EventArgs
    {
        public DrSrvEventArgsHandle(IntPtr h)
        {
            this.Handle = h;
        }
        public IntPtr Handle { get; private set; }
    }
    #endregion CloseHandle
    #region ServiceDelete
    public class DrSrvEventArgsBeforeServiceDelete : DrSrvEventArgService
    {
        public DrSrvEventArgsBeforeServiceDelete(IntPtr hService) : base(hService) { this.Cancel = false; }
        public bool Cancel { get; set; }
    }
    #endregion ServiceDelete
    #region CreateService
    public abstract class DrSrvEventArgsCreateService : EventArgs
    {
        public DrSrvEventArgsCreateService(string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string loadOrderGroup,
                                  string dependencies,
                                  string userName)
        {
            this.ServiceName = serviceName;
            this.DisplayName = displayName;
            this.Description = description;
            this.DesiredAccess = desiredAccess;
            this.ServiceType = serviceType;
            this.StartType = startType;
            this.ErrorControl = errorControl;
            this.BinaryPathName = binaryPathName;
            this.LoadOrderGroup = loadOrderGroup;
            this.Dependencies = dependencies;
            this.UserName = userName;
        }
        public string ServiceName { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public DrSrvHelper.SERVICE_ACCESS DesiredAccess { get; private set; }
        public DrSrvHelper.SERVICE_TYPE ServiceType { get; private set; }
        public DrSrvHelper.SERVICE_START_TYPE StartType { get; private set; }
        public DrSrvHelper.SERVICE_ERROR_TYPE ErrorControl { get; private set; }
        public string BinaryPathName { get; private set; }
        public string LoadOrderGroup { get; private set; }
        public string Dependencies { get; private set; }
        public string UserName { get; private set; }
    }
    public class DrSrvEventArgsAfterCreateService : DrSrvEventArgsCreateService
    {
        public DrSrvEventArgsAfterCreateService(IntPtr hService,
                                  string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string loadOrderGroup,
                                  string dependencies,
                                  string userName)
            : base(serviceName,
                displayName,
                description,
                desiredAccess,
                serviceType,
                startType,
                errorControl,
                binaryPathName,
                loadOrderGroup,
                dependencies,
                userName)
        {
            this.HService = hService;
        }
        public IntPtr HService { get; private set; }
    }
    public class DrSrvEventArgsBeforeCreateService : DrSrvEventArgsCreateService
    {
        public DrSrvEventArgsBeforeCreateService(IntPtr hSCManager,
                                  string serviceName,
                                  string displayName,
                                  string description,
                                  DrSrvHelper.SERVICE_ACCESS desiredAccess,
                                  DrSrvHelper.SERVICE_TYPE serviceType,
                                  DrSrvHelper.SERVICE_START_TYPE startType,
                                  DrSrvHelper.SERVICE_ERROR_TYPE errorControl,
                                  string binaryPathName,
                                  string loadOrderGroup,
                                  string dependencies,
                                  string userName)
            : base(serviceName,
                displayName,
                description,
                desiredAccess,
                serviceType,
                startType,
                errorControl,
                binaryPathName,
                loadOrderGroup,
                dependencies,
                userName)
        {
            this.HSCManager = hSCManager;
            this.Cancel = false;
        }
        public IntPtr HSCManager { get; set; }
        public bool Cancel { get; set; }
    }
    #endregion CreateService

    #region Win32Error
    public class DrSrvEventArgBeforeThrowWin32Error : EventArgs
    {

        public DrSrvEventArgBeforeThrowWin32Error(Win32Exception e, bool cancel)
        {
            this.Win32Exception = e;
            this.Cancel = cancel;
        }
        public DrSrvEventArgBeforeThrowWin32Error(Win32Exception e) : this(e, false) { }
        /// <summary>
        /// Set value true for Cancel this operation
        /// </summary>
        public virtual bool Cancel { get; set; }
        public Win32Exception Win32Exception { get; private set; }
    }

    #endregion Win32Error

    ///// <summary>
    ///// Provides SCM event
    ///// </summary>
    //public class DrSCMEventArgs : EventArgs
    //{
    //    public DrSCMEventArgs(IntPtr hSCM)
    //    {
    //        this.HSCM = hSCM;
    //    }
    //    /// <summary>
    //    /// a handle to the openned service control manager database
    //    /// </summary>
    //    public IntPtr HSCM { private get; set; }
    //}

    ///// <summary>
    ///// Provides openning service control manager database
    ///// </summary>
    //public class DrSCMOpenEventArgs : EventArgs
    //{
    //    public DrSCMOpenEventArgs(string serverName)
    //    {
    //        this.ServerName = serverName;
    //    }
    //    /// <summary>
    //    /// server name
    //    /// </summary>
    //    public string ServerName { get; private set; }
    //}

    ///// <summary>
    ///// Provides openning service event
    ///// </summary>
    //public class DrSrvOpenEventArgs : EventArgs
    //{
    //    public DrSrvOpenEventArgs(string serviceName)
    //    {
    //        this.ServiceName = serviceName;
    //    }
    //    /// <summary>
    //    /// service name
    //    /// </summary>
    //    public string ServiceName { get; private set; }
    //}

    ///// <summary>
    ///// Provides creating service event
    ///// </summary>
    //public class DrSrvCreateEventArgs : EventArgs
    //{
    //    public DrSrvCreateEventArgs(string serviceName)
    //    {
    //        this.ServiceName = serviceName;
    //    }
    //    /// <summary>
    //    /// service name
    //    /// </summary>
    //    public string ServiceName { get; private set; }
    //}

    ///// <summary>
    ///// Provides service event
    ///// </summary>
    //public class DrSrvEventArgs : EventArgs
    //{
    //    public DrSrvEventArgs(IntPtr hService)
    //    {
    //        this.HService = hService;
    //    }
    //    /// <summary>
    //    /// a handle to the openned service.
    //    /// </summary>
    //    public IntPtr HService { get; private set; }
    //}

    ///// <summary>
    ///// Service action event
    ///// </summary>
    //public class DrSrvActionEventArgs : DrSrvEventArgs
    //{
    //    public DrSrvActionEventArgs(IntPtr hService, string action)
    //        : base(hService)
    //    {
    //        this.Action = action;
    //    }
    //    /// <summary>
    //    /// current service status
    //    /// </summary>
    //    public string Action { get; private set; }
    //}

    ///// <summary>
    ///// Service status event
    ///// </summary>
    //public class DrSrvStatusEventArgs : DrSrvEventArgs
    //{
    //    public DrSrvStatusEventArgs(IntPtr hService) : base(hService) { }

    //    public DrSrvStatusEventArgs(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE currentSrvState)
    //        : base(hService)
    //    {
    //        this.CurrentSrvState = currentSrvState;
    //    }
    //    /// <summary>
    //    /// current service status
    //    /// </summary>
    //    public DrSrvHelper.SERVICE_CURRENT_STATE CurrentSrvState { get; set; }
    //}



}
