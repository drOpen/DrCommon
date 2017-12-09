/*
  DrSrvHelper.cs -- helper for service control manager 1.0, July 16, 2017
 
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
using System.Runtime.InteropServices;
using System.Text;

namespace DrOpen.DrCommon.DrSrv
{
    public static class DrSrvHelper
    {
        #region win32 declaration
        #region win 32 struct
        /// <summary>
        /// Contains the name of a service in a service control manager database and information about that service
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS
        {
            /// <summary>
            /// The name of a service in the service control manager database. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. A slash (/), backslash (\), comma, and space are invalid service name characters.
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string pServiceName;
            /// <summary>
            /// A display name that can be used by service control programs, such as Services in Control Panel, to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string pDisplayName;
            /// <summary>
            /// A SERVICE_STATUS structure that contains status information for the lpServiceName service.
            /// </summary>
            public SERVICE_STATUS ServiceStatus;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(ENUM_SERVICE_STATUS));
        }
        public class QUERY_SERVICE_CONFIG
        {
            public QUERY_SERVICE_CONFIG()
            { }

            internal QUERY_SERVICE_CONFIG(QUERY_SERVICE_CONFIG_PTR cfg)
            {
                this.binaryPathName = cfg.binaryPathName;
                this.dependencies = GetStringArrayFromPtrArrayDoublyNullTerminated(cfg.dependencies);
                this.displayName = cfg.displayName;
                this.errorControl = cfg.errorControl;
                this.loadOrderGroup = cfg.loadOrderGroup;
                this.serviceType = cfg.serviceType;
                this.startName = cfg.startName;
                this.startType = cfg.startType;
                this.tagID = cfg.tagID;
            }
            /// <summary>
            /// The type of service. This member can be one of the following values.
            /// </summary>
            public SERVICE_TYPE serviceType;
            /// <summary>
            /// When to start the service. This member can be one of the following values.
            /// </summary>
            public SERVICE_START_TYPE startType;
            /// <summary>
            /// The severity of the error, and action taken, if this service fails to start. This member can be one of the following values.
            /// </summary>
            public SERVICE_ERROR_TYPE errorControl;
            /// <summary>
            /// The fully qualified path to the service binary file. The path can also include arguments for an auto-start service. These arguments are passed to the service entry point (typically the main function).
            /// </summary>
            public string binaryPathName;
            /// <summary>
            /// The name of the load ordering group to which this service belongs. If the member is NULL or an empty string, the service does not belong to a load ordering group.
            /// The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:
            /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder
            /// </summary>
            public string loadOrderGroup;
            /// <summary>
            /// A unique tag value for this service in the group specified by the lpLoadOrderGroup parameter. A value of zero indicates that the service has not been assigned a tag. You can use a tag for ordering service startup within a load order group by specifying a tag order vector in the registry located at:
            /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
            /// Tags are only evaluated for SERVICE_KERNEL_DRIVER and SERVICE_FILE_SYSTEM_DRIVER type services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.
            /// </summary>
            public int tagID;
            /// <summary>
            /// A pointer to an array of null-separated names of services or load ordering groups that must start before this service. The array is doubly null-terminated. If the pointer is NULL or if it points to an empty string, the service has no dependencies. If a group name is specified, it must be prefixed by the SC_GROUP_IDENTIFIER (defined in WinSvc.h) character to differentiate it from a service name, because services and service groups share the same name space. Dependency on a service means that this service can only run if the service it depends on is running. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
            /// </summary>
            public string[] dependencies;
            /// <summary>
            /// If the service type is SERVICE_WIN32_OWN_PROCESS or SERVICE_WIN32_SHARE_PROCESS, this member is the name of the account that the service process will be logged on as when it runs. This name can be of the form Domain\UserName. If the account belongs to the built-in domain, the name can be of the form .\UserName. The name can also be "LocalSystem" if the process is running under the LocalSystem account.
            /// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, this member is the driver object name (that is, \FileSystem\Rdr or \Driver\Xns) which the input and output (I/O) system uses to load the device driver. If this member is NULL, the driver is to be run with a default object name created by the I/O system, based on the service name.
            /// </summary>
            public string startName;
            /// <summary>
            /// The display name to be used by service control programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.
            /// This parameter can specify a localized string using the following format:
            /// @[Path\]DLLName,-StrID
            /// The string with identifier StrID is loaded from DLLName; the Path is optional. For more information, see RegLoadMUIString.
            /// Windows Server 2003 and Windows XP:  Localized strings are not supported until Windows Vista.
            /// </summary>
            public string displayName;
        };
        /// <summary>
        /// Contains configuration information for an installed service. It is used by the QueryServiceConfig function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class QUERY_SERVICE_CONFIG_PTR
        {
            /// <summary>
            /// The type of service. This member can be one of the following values.
            /// </summary>
            public SERVICE_TYPE serviceType;
            /// <summary>
            /// When to start the service. This member can be one of the following values.
            /// </summary>
            public SERVICE_START_TYPE startType;
            /// <summary>
            /// The severity of the error, and action taken, if this service fails to start. This member can be one of the following values.
            /// </summary>
            public SERVICE_ERROR_TYPE errorControl;
            /// <summary>
            /// The fully qualified path to the service binary file. The path can also include arguments for an auto-start service. These arguments are passed to the service entry point (typically the main function).
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string binaryPathName;
            /// <summary>
            /// The name of the load ordering group to which this service belongs. If the member is NULL or an empty string, the service does not belong to a load ordering group.
            /// The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:
            /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string loadOrderGroup;
            /// <summary>
            /// A unique tag value for this service in the group specified by the lpLoadOrderGroup parameter. A value of zero indicates that the service has not been assigned a tag. You can use a tag for ordering service startup within a load order group by specifying a tag order vector in the registry located at:
            /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
            /// Tags are only evaluated for SERVICE_KERNEL_DRIVER and SERVICE_FILE_SYSTEM_DRIVER type services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.
            /// </summary>
            public int tagID;
            /// <summary>
            /// A pointer to an array of null-separated names of services or load ordering groups that must start before this service. The array is doubly null-terminated. If the pointer is NULL or if it points to an empty string, the service has no dependencies. If a group name is specified, it must be prefixed by the SC_GROUP_IDENTIFIER (defined in WinSvc.h) character to differentiate it from a service name, because services and service groups share the same name space. Dependency on a service means that this service can only run if the service it depends on is running. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
            /// </summary>
            // [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public IntPtr dependencies;
            /// <summary>
            /// If the service type is SERVICE_WIN32_OWN_PROCESS or SERVICE_WIN32_SHARE_PROCESS, this member is the name of the account that the service process will be logged on as when it runs. This name can be of the form Domain\UserName. If the account belongs to the built-in domain, the name can be of the form .\UserName. The name can also be "LocalSystem" if the process is running under the LocalSystem account.
            /// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, this member is the driver object name (that is, \FileSystem\Rdr or \Driver\Xns) which the input and output (I/O) system uses to load the device driver. If this member is NULL, the driver is to be run with a default object name created by the I/O system, based on the service name.
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string startName;
            /// <summary>
            /// The display name to be used by service control programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.
            /// This parameter can specify a localized string using the following format:
            /// @[Path\]DLLName,-StrID
            /// The string with identifier StrID is loaded from DLLName; the Path is optional. For more information, see RegLoadMUIString.
            /// Windows Server 2003 and Windows XP:  Localized strings are not supported until Windows Vista.
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string displayName;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(QUERY_SERVICE_CONFIG_PTR));
        };
        /// <summary>
        /// Represents the action the service controller should take on each failure of a service. A service is considered failed when it terminates without reporting a status of SERVICE_STOPPED to the service controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_FAILURE_ACTIONS
        {
            /// <summary>
            /// The time after which to reset the failure count to zero if there are no failures, in seconds. Specify INFINITE to indicate that this value should never be reset.
            /// </summary>
            public int dwResetPeriod;
            /// <summary>
            /// The message to be broadcast to server users before rebooting in response to the SC_ACTION_REBOOT service controller action.
            /// If this value is NULL, the reboot message is unchanged. If the value is an empty string (""), the reboot message is deleted and no message is broadcast.
            /// This member can specify a localized string using the following format:
            /// @[path\]dllname,-strID
            /// The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString.
            /// Windows Server 2003 and Windows XP:  Localized strings are not supported until Windows Vista.
            /// </summary>
            public string lpRebootMsg;
            /// <summary>
            /// The command line of the process for the CreateProcess function to execute in response to the SC_ACTION_RUN_COMMAND service controller action. This process runs under the same account as the service.
            /// If this value is NULL, the command is unchanged. If the value is an empty string (""), the command is deleted and no program is run when the service fails.
            /// </summary>
            public string lpCommand;
            /// <summary>
            /// The number of elements in the lpsaActions array.
            /// If this value is 0, but lpsaActions is not NULL, the reset period and array of failure actions are deleted.
            /// </summary>
            public int cActions;
            /// <summary>
            /// A pointer to an array of SC_ACTION structures.
            /// If this value is NULL, the cActions and dwResetPeriod members are ignored.
            /// </summary>
            public int lpsaActions;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(SERVICE_FAILURE_ACTIONS));
        }
        /// <summary>
        /// Contains status information for a service. The ControlService, EnumDependentServices, EnumServicesStatus, and QueryServiceStatus functions use this structure. A service uses this structure in the SetServiceStatus function to report its current status to the service control manager.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SERVICE_STATUS
        {
            /// <summary>
            /// The type of service. This member can be one of the following values.
            /// </summary>
            public SERVICE_TYPE serviceType;
            /// <summary>
            /// The current state of the service. This member can be one of the following values.
            /// </summary>
            public SERVICE_CURRENT_STATE serviceState;
            /// <summary>
            /// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). A user interface process can control a service by specifying a control command in the ControlService or ControlServiceEx function. By default, all services accept the SERVICE_CONTROL_INTERROGATE value. To accept the SERVICE_CONTROL_DEVICEEVENT value, the service must register to receive device events by using the RegisterDeviceNotification function.
            /// </summary>
            public SERVICE_ACCEPTED controlsAccepted;
            /// <summary>
            /// The error code the service uses to report an error that occurs when it is starting or stopping. To return an error code specific to the service, the service must set this value to ERROR_SERVICE_SPECIFIC_ERROR to indicate that the dwServiceSpecificExitCode member contains the error code. The service should set this value to NO_ERROR when it is running and on normal termination.
            /// </summary>
            public uint win32ExitCode;
            /// <summary>
            /// A service-specific error code that the service returns when an error occurs while the service is starting or stopping. This value is ignored unless the dwWin32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
            /// </summary>
            public uint serviceSpecificExitCode;
            /// <summary>
            /// The check-point value the service increments periodically to report its progress during a lengthy start, stop, pause, or continue operation. For example, the service should increment this value as it completes each step of its initialization when it is starting up. The user interface program that invoked the operation on the service uses this value to track the progress of the service during a lengthy operation. This value is not valid and should be zero when the service does not have a start, stop, pause, or continue operation pending.
            /// </summary>
            public uint checkPoint;
            /// <summary>
            /// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. Before the specified amount of time has elapsed, the service should make its next call to the SetServiceStatus function with either an incremented dwCheckPoint value or a change in dwCurrentState. If the amount of time specified by dwWaitHint passes, and dwCheckPoint has not been incremented or dwCurrentState has not changed, the service control manager or service control program can assume that an error has occurred and the service should be stopped. However, if the service shares a process with other services, the service control manager cannot terminate the service application because it would have to terminate the other services sharing the process as well.
            /// </summary>
            public uint waitHint;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(SERVICE_STATUS));
        }
        /// <summary>
        /// Contains a service description.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_DESCRIPTION
        {
            /// <summary>
            /// The description of the service. If this member is NULL, the description remains unchanged. If this value is an empty string (""), the current description is deleted.
            /// The service description must not exceed the size of a registry value of type REG_SZ.
            /// This member can specify a localized string using the following format:
            /// @[path\]dllname,-strID
            /// The string with identifier strID is loaded from dllname; the path is optional. For more information, see RegLoadMUIString.
            /// Windows Server 2003 and Windows XP:  Localized strings are not supported until Windows Vista.
            /// A description of NULL indicates no service description exists. The service description is NULL when the service is created.
            /// </summary>
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpDescription;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(SERVICE_DESCRIPTION));
        }
        /// <summary>
        /// Contains the delayed auto-start setting of an auto-start service.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_DELAYED_AUTO_START_INFO
        {
            /// <summary>
            /// If this member is TRUE, the service is started after other auto-start services are started plus a short delay. Otherwise, the service is started during system boot. This setting is ignored unless the service is an auto-start service.
            /// </summary>
            public bool fDelayedAutostart;
            /// <summary>
            /// Returns size of this structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(SERVICE_DELAYED_AUTO_START_INFO));
        }
        #endregion win32 struct
        #region win32 constants
        //  The following are masks for the predefined standard access types
        //
        /// <summary>
        /// Required to call the DeleteService function to delete the service.
        /// </summary>
        public const int DELETE = 0x10000;
        /// <summary>
        /// 	Required to call the QueryServiceObjectSecurity function to query the security descriptor of the service object.
        /// </summary>
        public const int READ_CONTROL = 0x20000;
        /// <summary>
        /// Required to call the SetServiceObjectSecurity function to modify the Dacl member of the service object's security descriptor.
        /// </summary>
        public const int WRITE_DAC = 0x40000;
        /// <summary>
        /// 	Required to call the SetServiceObjectSecurity function to modify the Owner and Group members of the service object's security descriptor.
        /// </summary>
        public const int WRITE_OWNER = 0x80000;
        public const int SYNCHRONIZE = 0x100000;
        public const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        public const int STANDARD_RIGHTS_READ = READ_CONTROL;
        public const int STANDARD_RIGHTS_WRITE = READ_CONTROL;
        public const int STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
        public const int STANDARD_RIGHTS_ALL = 0x1F0000;
        public const int SPECIFIC_RIGHTS_ALL = 0xFFFF;
        /// <summary>
        /// The data area passed to a system call is too small.
        /// </summary>
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        /// <summary>
        /// Value to indicate no change to an optional parameter
        /// </summary>
        public const int SERVICE_NO_CHANGE = 0xFFFF;
        //
        // Service object specific access type
        //
        /// <summary>
        /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
        /// </summary>
        public const int SERVICE_QUERY_CONFIG = 0x0001;
        /// <summary>
        /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Because this grants the caller the right to change the executable file that the system runs, it should be granted only to administrators.
        /// </summary>
        public const int SERVICE_CHANGE_CONFIG = 0x0002;
        /// <summary>
        /// Required to call the QueryServiceStatus or QueryServiceStatusEx function to ask the service control manager about the status of the service.
        /// Required to call the NotifyServiceStatusChange function to receive notification when a service changes status.
        /// </summary>
        public const int SERVICE_QUERY_STATUS = 0x0004;
        /// <summary>
        /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
        /// </summary>
        public const int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        /// <summary>
        /// Required to call the StartService function to start the service.
        /// </summary>
        public const int SERVICE_START = 0x0010;
        /// <summary>
        /// Required to call the ControlService function to stop the service.
        /// </summary>
        public const int SERVICE_STOP = 0x0020;
        /// <summary>
        /// Required to call the ControlService function to pause or continue the service.
        /// </summary>
        public const int SERVICE_PAUSE_CONTINUE = 0x0040;
        /// <summary>
        /// Required to call the ControlService function to ask the service to report its status immediately.
        /// </summary>
        public const int SERVICE_INTERROGATE = 0x0080;
        /// <summary>
        /// Required to call the ControlService function to specify a user-defined control code.
        /// </summary>
        public const int SERVICE_USER_DEFINED_CONTROL = 0x0100;
        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
        /// </summary>
        public const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                                               SERVICE_QUERY_CONFIG |
                                               SERVICE_CHANGE_CONFIG |
                                               SERVICE_QUERY_STATUS |
                                               SERVICE_ENUMERATE_DEPENDENTS |
                                               SERVICE_START |
                                               SERVICE_STOP |
                                               SERVICE_PAUSE_CONTINUE |
                                               SERVICE_INTERROGATE |
                                               SERVICE_USER_DEFINED_CONTROL);
        #region errors
        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        public const int ERROR_CANCELLED = 1223;
        /// <summary>
        /// A stop control has been sent to a service that other running services are dependent on.
        /// </summary>
        public const int ERROR_DEPENDENT_SERVICES_RUNNING = 1051;
        /// <summary>
        /// The requested control is not valid for this service.
        /// </summary>
        public const int ERROR_INVALID_SERVICE_CONTROL = 1052;

        /// The service did not respond to the start or control request in a timely fashion.

        public const int ERROR_SERVICE_REQUEST_TIMEOUT = 1053;

        /// <summary>
        /// A thread could not be created for the service.
        /// </summary>
        public const int ERROR_SERVICE_NO_THREAD = 1054;

        /// <summary>
        /// The service database is locked.
        /// </summary>
        public const int ERROR_SERVICE_DATABASE_LOCKED = 1055;

        /// <summary>
        /// An instance of the service is already running.
        /// </summary>
        public const int ERROR_SERVICE_ALREADY_RUNNING = 1056;

        /// <summary>
        /// The account name is invalid or does not exist, or the password is invalid for the account name specified.
        /// </summary>
        public const int ERROR_INVALID_SERVICE_ACCOUNT = 1057;

        /// <summary>
        /// The service cannot be started, either because it is disabled or because it has no enabled devices associated with it.
        /// </summary>
        public const int ERROR_SERVICE_DISABLED = 1058;

        /// <summary>
        /// Circular service dependency was specified.
        /// </summary>
        public const int ERROR_CIRCULAR_DEPENDENCY = 1059;

        /// <summary>
        /// The specified service does not exist as an installed service.
        /// </summary>
        public const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

        /// <summary>
        /// The service cannot accept control messages at this time.
        /// </summary>
        public const int ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061;

        /// <summary>
        /// The service has not been started.
        /// </summary>
        public const int ERROR_SERVICE_NOT_ACTIVE = 1062;

        /// <summary>
        /// The service process could not connect to the service controller.
        /// </summary>
        public const int ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063;

        /// <summary>
        /// An exception occurred in the service when handling the control request.
        /// </summary>
        public const int ERROR_EXCEPTION_IN_SERVICE = 1064;

        /// <summary>
        /// The database specified does not exist.
        /// </summary>
        public const int ERROR_DATABASE_DOES_NOT_EXIST = 1065;

        /// <summary>
        /// The service has returned a service-specific error code.
        /// </summary>
        public const int ERROR_SERVICE_SPECIFIC_ERROR = 1066;

        /// <summary>
        /// The process terminated unexpectedly.
        /// </summary>
        public const int ERROR_PROCESS_ABORTED = 1067;

        /// <summary>
        /// The dependency service or group failed to start.
        /// </summary>
        public const int ERROR_SERVICE_DEPENDENCY_FAIL = 1068;

        /// <summary>
        /// The service did not start due to a logon failure.
        /// </summary>
        public const int ERROR_SERVICE_LOGON_FAILED = 1069;

        /// <summary>
        /// After starting, the service hung in a start-pending state.
        /// </summary>
        public const int ERROR_SERVICE_START_HANG = 1070;

        /// <summary>
        /// The specified service database lock is invalid.
        /// </summary>
        public const int ERROR_INVALID_SERVICE_LOCK = 0x1071;

        /// <summary>
        /// The specified service has been marked for deletion.
        /// </summary>
        public const int ERROR_SERVICE_MARKED_FOR_DELETE = 1072;

        /// <summary>
        /// The specified service already exists.
        /// </summary>
        public const int ERROR_SERVICE_EXISTS = 1073;

        /// <summary>
        /// The system is currently running with the last-known-good configuration.
        /// </summary>
        public const int ERROR_ALREADY_RUNNING_LKG = 1074;

        /// <summary>
        /// The dependency service does not exist or has been marked for deletion.
        /// </summary>
        public const int ERROR_SERVICE_DEPENDENCY_DELETED = 1075;

        /// <summary>
        /// The current boot has already been accepted for use as the last-known-good control set.
        /// </summary>
        public const int ERROR_BOOT_ALREADY_ACCEPTED = 1076;

        /// <summary>
        /// No attempts to start the service have been made since the last boot.
        /// </summary>
        public const int ERROR_SERVICE_NEVER_STARTED = 1077;

        /// <summary>
        /// The name is already in use as either a service name or a service display name.
        /// </summary>
        public const int ERROR_DUPLICATE_SERVICE_NAME = 0x1078;

        /// <summary>
        /// The account specified for this service is different from the account specified for other services running in the same process.
        /// </summary>
        public const int ERROR_DIFFERENT_SERVICE_ACCOUNT = 1079;

        /// <summary>
        /// Failure actions can only be set for Win32 services, not for drivers.
        /// </summary>
        public const int ERROR_CANNOT_DETECT_DRIVER_FAILURE = 1080;

        /// <summary>
        /// This service runs in the same process as the service control manager.
        /// </summary> Therefore, the service control manager cannot take action if this service's process terminates unexpectedly.
        //
        public const int ERROR_CANNOT_DETECT_PROCESS_ABORT = 1081;

        /// <summary>
        /// No recovery program has been configured for this service.
        /// </summary>
        public const int ERROR_NO_RECOVERY_PROGRAM = 1082;

        /// <summary>
        /// The executable program that this service is configured to run in does not implement the service.
        /// </summary>
        public const int ERROR_SERVICE_NOT_IN_EXE = 1083;
        #endregion errors
        #endregion win32 constants
        #region win32 enum

        /// <summary>
        /// The state of the services to be enumerated. This parameter can be one of the following values.
        /// </summary>
        [Flags]
        public enum SERVICE_STATE : int
        {
            /// <summary>
            /// numerates services that are in the following states: SERVICE_START_PENDING, SERVICE_STOP_PENDING, SERVICE_RUNNING, SERVICE_CONTINUE_PENDING, SERVICE_PAUSE_PENDING, and SERVICE_PAUSED.
            /// </summary>
            SERVICE_ACTIVE = 0x00000001,
            /// <summary>
            /// Enumerates services that are in the SERVICE_STOPPED state.
            /// </summary>
            SERVICE_INACTIVE = 0x00000002,
            /// <summary>
            /// Combines the SERVICE_ACTIVE and SERVICE_INACTIVE states.
            /// </summary>
            SERVICE_STATE_ALL = SERVICE_ACTIVE | SERVICE_INACTIVE
        }
        /// <summary>
        /// Access Rights for the Service Control Manager
        /// </summary>
        [Flags]
        public enum SC_MANAGER : int
        {
            /// <summary>
            /// Required to connect to the service control manager.
            /// </summary>
            SC_MANAGER_CONNECT = 0x1,
            /// <summary>
            /// Required to call the CreateService function to create a service object and add it to the database.
            /// </summary>
            SC_MANAGER_CREATE_SERVICE = 0x2,
            /// <summary>
            /// Required to call the EnumServicesStatus or EnumServicesStatusEx function to list the services that are in the database.
            /// Required to call the NotifyServiceStatusChange function to receive notification when any service is created or deleted.
            /// </summary>
            SC_MANAGER_ENUMERATE_SERVICE = 0x4,
            /// <summary>
            /// Required to call the LockServiceDatabase function to acquire a lock on the database.
            /// </summary>
            SC_MANAGER_LOCK = 0x8,
            /// <summary>
            /// Required to call the NotifyBootConfigStatus function.
            /// </summary>
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x20,
            /// <summary>
            /// Required to call the QueryServiceLockStatus function to retrieve the lock status information for the database.
            /// </summary>
            SC_MANAGER_QUERY_LOCK_STATUS = 0x10,
            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights in this table.
            /// </summary>
            SC_MANAGER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS | SC_MANAGER_MODIFY_BOOT_CONFIG),
            /// <summary>
            /// STANDARD_RIGHTS_READ SC_MANAGER_ENUMERATE_SERVICE SC_MANAGER_QUERY_LOCK_STATUS
            /// </summary>
            SC_GENERIC_READ = (STANDARD_RIGHTS_READ | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_QUERY_LOCK_STATUS),
            /// <summary>
            /// STANDARD_RIGHTS_WRITE SC_MANAGER_CREATE_SERVICE SC_MANAGER_MODIFY_BOOT_CONFIG
            /// </summary>
            SC_GENERIC_WRITE = (STANDARD_RIGHTS_WRITE | SC_MANAGER_CREATE_SERVICE),
            /// <summary>
            /// STANDARD_RIGHTS_EXECUTE  SC_MANAGER_CONNECT SC_MANAGER_LOCK
            /// </summary>
            SC_GENERIC_EXECUTE = (STANDARD_RIGHTS_EXECUTE | SC_MANAGER_CONNECT | SC_MANAGER_LOCK),
            /// <summary>
            /// SC_MANAGER_ALL_ACCESS
            /// </summary>
            GENERIC_ALL = 0x1000000
        }

        /// <summary>
        /// Access Rights for a Service
        /// </summary>
        [Flags]
        public enum SERVICE_ACCESS : int
        {
            /// <summary>
            /// STEM_SECURITY	Required to call the QueryServiceObjectSecurity or SetServiceObjectSecurity function to access the SACL. The proper way to obtain this access is to enable the SE_SECURITY_NAMEprivilege in the caller's current access token, open the handle for ACCESS_SYSTEM_SECURITY access, and then disable the privilege.
            /// </summary>
            SERVICE_ACCESS_SYSTEM_SECURITY = 0x1000000,
            /// <summary>
            /// 	Required to call the DeleteService function to delete the service.
            /// </summary>
            SERVICE_DELETE = 0x10000,
            /// <summary>
            /// Required to call the QueryServiceObjectSecurity function to query the security descriptor of the service object.
            /// </summary>
            SERVICE_READ_CONTROL = 0x20000,
            /// <summary>
            /// 	Required to call the SetServiceObjectSecurity function to modify the Dacl member of the service object's security descriptor.
            /// </summary>
            SERVICE_WRITE_DAC = 0x40000,
            /// <summary>
            /// 	Required to call the SetServiceObjectSecurity function to modify the Owner and Group members of the service object's security descriptor.
            /// </summary>
            SERVICE_WRITE_OWNER = 0x80000,
            /// <summary>
            /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
            /// </summary>
            SERVICE_QUERY_CONFIG = 0x1,
            /// <summary>
            /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Because this grants the caller the right to change the executable file that the system runs, it should be granted only to administrators.
            /// </summary>
            SERVICE_CHANGE_CONFIG = 0x2,
            /// <summary>
            /// Required to call the QueryServiceStatus or QueryServiceStatusEx function to ask the service control manager about the status of the service.
            /// Required to call the NotifyServiceStatusChange function to receive notification when a service changes status.
            /// </summary>
            SERVICE_QUERY_STATUS = 0x4,
            /// <summary>
            /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
            /// </summary>
            SERVICE_ENUMERATE_DEPENDENTS = 0x8,
            /// <summary>
            /// Required to call the StartService function to start the service.
            /// </summary>
            SERVICE_START = 0x10,
            /// <summary>
            /// Required to call the ControlService function to stop the service.
            /// </summary>
            SERVICE_STOP = 0x20,
            /// <summary>
            /// Required to call the ControlService function to pause or continue the service.
            /// </summary>
            SERVICE_PAUSE_CONTINUE = 0x40,
            /// <summary>
            /// Required to call the ControlService function to ask the service to report its status immediately.
            /// </summary>
            SERVICE_INTERROGATE = 0x80,
            /// <summary>
            /// Required to call the ControlService function to specify a user-defined control code.
            /// </summary>
            SERVICE_USER_DEFINED_CONTROL = 0x100,
            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
            /// </summary>
            SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL),
            SERVICE_GENERIC_READ = (STANDARD_RIGHTS_READ | SERVICE_QUERY_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS),
            SERVICE_GENERIC_WRITE = (STANDARD_RIGHTS_WRITE | SERVICE_CHANGE_CONFIG),
            SERVICE_GENERIC_EXECUTE = (STANDARD_RIGHTS_EXECUTE | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_USER_DEFINED_CONTROL),
        }
        [Flags]
        public enum SERVICE_TYPE : int
        {
            /// <summary>
            /// Driver services.
            /// </summary>
            SERVICE_KERNEL_DRIVER = 0x1,               // The service is a device driver.
            /// <summary>
            /// File system driver service.
            /// </summary>
            SERVICE_FILE_SYSTEM_DRIVER = 0x2,          // The service is a file system driver.
            /// <summary>
            /// Service that runs in its own process.
            /// </summary>
            SERVICE_WIN32_OWN_PROCESS = 0x10,          // The service runs in its own process.
            /// <summary>
            /// Service that shares a process with one or more other services. For more information, see Service Programs.
            /// </summary>
            SERVICE_WIN32_SHARE_PROCESS = 0x20,        // The service shares a process with other services.
            /// <summary>
            /// The service can interact with the desktop.
            /// </summary>
            SERVICE_INTERACTIVE_PROCESS = 0x100,       // The service can interact with the desktop.
            /// <summary>
            /// Reserved.
            /// </summary>
            SERVICE_ADAPTER = 0x4,
            /// <summary>
            /// Reserved.
            /// </summary>
            SERVICE_RECOGNIZER_DRIVER = 0x8,
            /// <summary>
            /// SERVICE_KERNEL_DRIVER | SERVICE_FILE_SYSTEM_DRIVER | SERVICE_RECOGNIZER_DRIVER
            /// </summary>
            SERVICE_DRIVER = (SERVICE_KERNEL_DRIVER | SERVICE_FILE_SYSTEM_DRIVER | SERVICE_RECOGNIZER_DRIVER),
            /// <summary>
            /// SERVICE_WIN32_OWN_PROCESS | SERVICE_WIN32_SHARE_PROCESS
            /// </summary>
            SERVICE_WIN32 = (SERVICE_WIN32_OWN_PROCESS | SERVICE_WIN32_SHARE_PROCESS),
            /// <summary>
            /// SERVICE_WIN32 | SERVICE_ADAPTER | SERVICE_DRIVER | SERVICE_INTERACTIVE_PROCESS
            /// </summary>
            SERVICE_TYPE_ALL = (SERVICE_WIN32 | SERVICE_ADAPTER | SERVICE_DRIVER | SERVICE_INTERACTIVE_PROCESS)
        }
        /// <summary>
        /// The current state of the service. This member can be one of the following values.
        /// </summary>
        public enum SERVICE_CURRENT_STATE : int
        {
            /// <summary>
            /// The service is not running.
            /// </summary>
            SERVICE_STOPPED = 0x1,
            /// <summary>
            /// The service is starting.
            /// </summary>
            SERVICE_START_PENDING = 0x2,
            /// <summary>
            /// The service is stopping.
            /// </summary>
            SERVICE_STOP_PENDING = 0x3,
            /// <summary>
            /// The service is running.
            /// </summary>
            SERVICE_RUNNING = 0x4,
            /// <summary>
            /// The service continue is pending.
            /// </summary>
            SERVICE_CONTINUE_PENDING = 0x5,
            /// <summary>
            /// The service pause is pending.
            /// </summary>
            SERVICE_PAUSE_PENDING = 0x6,
            /// <summary>
            /// The service is paused.
            /// </summary>
            SERVICE_PAUSED = 0x7
        }
        /// <summary>
        /// The severity of the error, and action taken, if this service fails to start. This parameter can be one of the following values.
        /// </summary>
        public enum SERVICE_ERROR_TYPE : int
        {
            /// <summary>
            /// The startup program ignores the error and continues the startup operation.
            /// </summary>
            SERVICE_ERROR_IGNORE = 0x0,
            /// <summary>
            /// The startup program logs the error in the event log but continues the startup operation.
            /// </summary>
            SERVICE_ERROR_NORMAL = 0x1,
            /// <summary>
            /// The startup program logs the error in the event log. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.
            /// </summary>
            SERVICE_ERROR_SEVERE = 0x2,
            /// <summary>
            /// The startup program logs the error in the event log, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.
            /// </summary>
            SERVICE_ERROR_CRITICAL = 0x3,
        }
        /// <summary>
        /// The service start options. This parameter can be one of the following values.
        /// </summary>
        public enum SERVICE_START_TYPE : int
        {
            /// <summary>
            /// A device driver started by the system loader. This value is valid only for driver services.
            /// </summary>
            SERVICE_BOOT_START = 0x0,
            /// <summary>
            /// A device driver started by the IoInitSystem function. This value is valid only for driver services.
            /// </summary>
            SERVICE_SYSTEM_START = 0x1,
            /// <summary>
            /// A service started automatically by the service control manager during system startup. For more information, see Automatically Starting Services.
            /// </summary>
            SERVICE_AUTO_START = 0x2,
            /// <summary>
            /// A service started by the service control manager when a process calls the StartService function. For more information, see Starting Services on Demand.
            /// </summary>
            SERVICE_DEMAND_START = 0x3,
            /// <summary>
            /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
            /// </summary>
            SERVICE_DISABLED = 0x4,
        }
        /// <summary>
        /// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). A user interface process can control a service by specifying a control command in the ControlService or ControlServiceEx function. By default, all services accept the SERVICE_CONTROL_INTERROGATE value. To accept the SERVICE_CONTROL_DEVICEEVENT value, the service must register to receive device events by using the RegisterDeviceNotification function.
        /// </summary>
        [Flags]
        public enum SERVICE_ACCEPTED : int
        {
            /// <summary>
            /// The service can be stopped.
            /// </summary>
            SERVICE_ACCEPT_STOP = 0x1,
            /// <summary>
            /// The service can be paused and continued.
            /// </summary>
            SERVICE_ACCEPT_PAUSE_CONTINUE = 0x2,
            /// <summary>
            /// The service is notified when system shutdown occurs.
            /// </summary>
            SERVICE_ACCEPT_SHUTDOWN = 0x4,
            /// <summary>
            /// The service can reread its startup parameters without being stopped and restarted.
            /// </summary>
            SERVICE_ACCEPT_PARAMCHANGE = 0x8,
            /// <summary>
            /// The service is a network component that can accept changes in its binding without being stopped and restarted.
            /// This control code allows the service to receive SERVICE_CONTROL_NETBINDADD, SERVICE_CONTROL_NETBINDREMOVE, SERVICE_CONTROL_NETBINDENABLE, and SERVICE_CONTROL_NETBINDDISABLE notifications.
            /// </summary>
            SERVICE_ACCEPT_NETBINDCHANGE = 0x10,
            /// <summary>
            /// The service is notified when the computer's hardware profile has changed.
            /// </summary>
            SERVICE_ACCEPT_HARDWAREPROFILECHANGE = 0x20,
            /// <summary>
            /// The service is notified when the computer's power status has changed. 
            /// </summary>
            SERVICE_ACCEPT_POWEREVENT = 0x40,
            /// <summary>
            /// The service is notified when the computer's session status has changed. 
            /// </summary>
            SERVICE_ACCEPT_SESSIONCHANGE = 0x80,
            /// <summary>
            /// The service is notified when the system time has changed. This enables the system to send SERVICE_CONTROL_TIMECHANGE notifications to the service.
            /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This control code is not supported.
            /// </summary>
            SERVICE_ACCEPT_TIMECHANGE = 0x200,
            /// <summary>
            /// The service is notified when an event for which the service has registered occurs. This enables the system to send SERVICE_CONTROL_TRIGGEREVENT notifications to the service.
            /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This control code is not supported.
            /// </summary>
            SERVICE_ACCEPT_TRIGGEREVENT = 0x400,
            /// <summary>
            /// The services is notified when the user initiates a reboot.
            /// Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This control code is not supported.
            /// </summary>
            SERVICE_ACCEPT_USERMODEREBOOT = 0x800,
        }

        /// <summary>
        /// service control code
        /// </summary>
        public enum SERVICE_CONTROL : int
        {
            /// <summary>
            /// Notifies a service that it should stop. The hService handle must have the SERVICE_STOP access right. After sending the stop request to a service, you should not send other controls to the service.
            /// </summary>
            SERVICE_CONTROL_STOP = 0x1,
            /// <summary>
            /// Notifies a service that it should pause. The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
            /// </summary>
            SERVICE_CONTROL_PAUSE = 0x2,
            /// <summary>
            /// Notifies a paused service that it should resume. The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
            /// </summary>
            SERVICE_CONTROL_CONTINUE = 0x3,
            /// <summary>
            /// Notifies a service that it should report its current status information to the service control manager. The hService handle must have the SERVICE_INTERROGATE access right. Note that this control is not generally useful as the SCM is aware of the current state of the service.
            /// </summary>
            SERVICE_CONTROL_INTERROGATE = 0x4,
            /// <summary>
            /// Notifies a service that its startup parameters have changed. The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
            /// </summary>
            SERVICE_CONTROL_PARAMCHANGE = 0x6,
            /// <summary>
            /// Notifies a network service that there is a new component for binding. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead.
            /// </summary>
            SERVICE_CONTROL_NETBINDADD = 0x7,
            /// <summary>
            /// Notifies a network service that a component for binding has been removed. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead.
            /// </summary>
            SERVICE_CONTROL_NETBINDREMOVE = 0x8,
            /// <summary>
            /// Notifies a network service that a disabled binding has been enabled. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead.
            /// </summary>
            SERVICE_CONTROL_NETBINDENABLE = 0x9,
            /// <summary>
            /// Notifies a network service that one of its bindings has been disabled. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead.
            /// </summary>
            SERVICE_CONTROL_NETBINDDISABLE = 0xA,
        }

        public enum INFO_LEVEL : int
        {
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_DELAYED_AUTO_START_INFO structure. Windows Server 2003 and Windows XP/2000:  This value is not supported.
            ///  </summary>
            SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_DESCRIPTION structure.
            /// </summary>
            SERVICE_CONFIG_DESCRIPTION = 1,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_FAILURE_ACTIONS structure. If the service controller handles the SC_ACTION_REBOOT action, the caller must have the SE_SHUTDOWN_NAME privilege. For more information, see Running with Special Privileges.
            /// </summary>
            SERVICE_CONFIG_FAILURE_ACTIONS = 2,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_FAILURE_ACTIONS_FLAG structure. Windows Server 2003 and Windows XP/2000:  This value is not supported.
            /// </summary>
            SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_PREFERRED_NODE_INFO structure. Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP/2000:  This value is not supported.
            /// </summary>
            SERVICE_CONFIG_PREFERRED_NODE = 9,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_PRESHUTDOWN_INFO structure. Windows Server 2003 and Windows XP/2000:  This value is not supported.
            /// </summary>
            SERVICE_CONFIG_PRESHUTDOWN_INFO = 7,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_REQUIRED_PRIVILEGES_INFO structure. Windows Server 2003 and Windows XP/2000:  This value is not supported.
            /// </summary>
            SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_SID_INFO structure. Windows 2000:  This value is not supported.
            /// </summary>
            SERVICE_CONFIG_SERVICE_SID_INFO = 5,
            /// <summary>
            /// The lpInfo parameter is a pointer to a SERVICE_TRIGGER_INFO structure. This value is not supported by the ANSI version of ChangeServiceConfig2. Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP/2000:  This value is not supported until Windows Server 2008 R2.
            /// </summary>
            SERVICE_CONFIG_TRIGGER_INFO = 8
        }

        #endregion win32 enum
        #region DllImports

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr lstrcpy([Out] StringBuilder lpStr1, string lpStr2);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr lstrcpy([Out] IntPtr lpStr1, IntPtr lpStr2);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr lstrcpy([Out] IntPtr lpStr1, string lpStr2);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr lstrcpy([Out] IntPtr lpStr1, int lpStr2);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int lstrlen(int lpStr);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int lstrlen(IntPtr lpStr);
        /// <summary>
        /// Establishes a connection to the service control manager on the specified computer and opens the specified service control manager database.
        /// </summary>
        /// <param name="machineName">The name of the target computer. If the pointer is NULL or points to an empty string, the function connects to the service control manager on the local computer</param>
        /// <param name="databaseName">The name of the service control manager database. This parameter should be set to SERVICES_ACTIVE_DATABASE. If it is NULL, the SERVICES_ACTIVE_DATABASE database is opened by default.</param>
        /// <param name="desiredAccess">The access to the service control manager. For a list of access rights, see Service Security and Access Rights.
        /// Before granting the requested access rights, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service control manager.
        /// The SC_MANAGER_CONNECT access right is implicitly specified by calling this function.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified service control manager database.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenSCManager([MarshalAs(UnmanagedType.LPTStr)] string machineName, [MarshalAs(UnmanagedType.LPTStr)]string databaseName, int desiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, [MarshalAs(UnmanagedType.LPTStr)] string serviceName, int desiredAccess);
        /// <summary>
        /// Changes the configuration parameters of a service.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceType"></param>
        /// <param name="startType"></param>
        /// <param name="errorControl"></param>
        /// <param name="binaryPathName"></param>
        /// <param name="loadOrderGroup"></param>
        /// <param name="tagID"></param>
        /// <param name="dependencies"></param>
        /// <param name="startName"></param>
        /// <param name="password"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ChangeServiceConfig(IntPtr service, int serviceType, int startType, int errorControl, [MarshalAs(UnmanagedType.LPTStr)] string binaryPathName,
                                                      [MarshalAs(UnmanagedType.LPTStr)] string loadOrderGroup, IntPtr tagID, [MarshalAs(UnmanagedType.LPTStr)] string dependencies,
                                                      [MarshalAs(UnmanagedType.LPTStr)] string startName, [MarshalAs(UnmanagedType.LPTStr)] string password,
                                                      [MarshalAs(UnmanagedType.LPTStr)] string displayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, INFO_LEVEL dwInfoLevel, IntPtr lpInfo);
        /// <summary>
        /// Retrieves the name and status of each service that depends on the specified service; that is, the specified service must be running before the dependent services can run.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_ENUMERATE_DEPENDENTS access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="dwServiceState">The state of the services to be enumerated. This parameter can be one of the following values.<paramref name="SERVICE_STATE"/></param>
        /// <param name="lpServices">A pointer to an array of ENUM_SERVICE_STATUS structures that receives the name and service status information for each dependent service in the database. The buffer must be large enough to hold the structures, plus the strings to which their members point.
        /// The order of the services in this array is the reverse of the start order of the services. In other words, the first service in the array is the one that would be started last, and the last service in the array is the one that would be started first.
        /// The maximum size of this array is 64,000 bytes. To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. The function will fail and GetLastError will return ERROR_MORE_DATA. The pcbBytesNeeded parameter will receive the required size.</param>
        /// <param name="cbBufSize">The size of the buffer pointed to by the lpServices parameter, in bytes.</param>
        /// <param name="pcbBytesNeeded">A pointer to a variable that receives the number of bytes needed to store the array of service entries. The variable only receives this value if the buffer pointed to by lpServices is too small, indicated by function failure and the ERROR_MORE_DATA error; otherwise, the contents of pcbBytesNeeded are undefined.</param>
        /// <param name="lpServicesReturned">A pointer to a variable that receives the number of service entries returned.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", EntryPoint = "EnumDependentServicesW", ExactSpelling = true, SetLastError = true)]
        public static extern bool EnumDependentServices(IntPtr hService,
                               SERVICE_STATE dwServiceState,
                               IntPtr lpServices,
                               int cbBufSize,
                               ref int pcbBytesNeeded,
                               ref int lpServicesReturned);
        /// <summary>
        /// Retrieves the configuration parameters of the specified service. Optional configuration parameters are available using the QueryServiceConfig2 function.
        /// </summary>
        /// <param name="service">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="queryServiceConfig">A pointer to a buffer that receives the service configuration information. The format of the data is a QUERY_SERVICE_CONFIG structure.
        /// The maximum size of this array is 8K bytes. To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. The function will fail and GetLastError will return ERROR_INSUFFICIENT_BUFFER. The pcbBytesNeeded parameter will receive the required size.</param>
        /// <param name="bufferSize">The size of the buffer pointed to by the lpServiceConfig parameter, in bytes.</param>
        /// <param name="bytesNeeded">A pointer to a variable that receives the number of bytes needed to store all the configuration information, if the function fails with ERROR_INSUFFICIENT_BUFFER.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int QueryServiceConfig(IntPtr service, IntPtr queryServiceConfig, int bufferSize, ref int bytesNeeded);
        /// <summary>
        /// Retrieves the optional configuration parameters of the specified service.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function and must have the SERVICE_QUERY_CONFIG access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="dwInfoLevel">The configuration information to be queried.</param>
        /// <param name="buffer">A pointer to the buffer that receives the service configuration information. The format of this data depends on the value of the dwInfoLevel parameter. The maximum size of this array is 8K bytes. To determine the required size, specify NULL for this parameter and 0 for the cbBufSize parameter. The function fails and GetLastError returns ERROR_INSUFFICIENT_BUFFER. The pcbBytesNeeded parameter receives the needed size.</param>
        /// <param name="cbBufSize">The size of the structure pointed to by the lpBuffer parameter, in bytes.</param>
        /// <param name="pcbBytesNeeded">A pointer to a variable that receives the number of bytes required to store the configuration information, if the function fails with ERROR_INSUFFICIENT_BUFFER.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int QueryServiceConfig2(IntPtr hService, INFO_LEVEL dwInfoLevel, IntPtr buffer, int cbBufSize, ref int pcbBytesNeeded);

        /// <summary>
        /// Closes a handle to a service control manager or service object.
        /// </summary>
        /// <param name="hSCObject">A handle to the service control manager object or the service object to close. Handles to service control manager objects are returned by the OpenSCManager function, and handles to service objects are returned by either the OpenService or CreateService function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);
        /// <summary>
        /// Retrieves the current status of the specified service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="lpServiceStatus">A pointer to a SERVICE_STATUS structure that receives the status information.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS lpServiceStatus);
        /// <summary>
        /// Sends a control code to a service.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function. The access rights required for this handle depend on the dwControl code requested.</param>
        /// <param name="dwControl">his parameter can be one of the following control codes. <paramref name="SERVICE_CONTROL"/></param>
        /// <param name="lpServiceStatus">A pointer to a SERVICE_STATUS structure that receives the latest service status information. The information returned reflects the most recent status that the service reported to the service control manager.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(IntPtr hService, SERVICE_CONTROL dwControl, ref  SERVICE_STATUS lpServiceStatus);
        /// <summary>
        /// Starts a service.
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the SERVICE_START access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="dwNumServiceArgs">The number of strings in the lpServiceArgVectors array. If lpServiceArgVectors is NULL, this parameter can be zero.</param>
        /// <param name="lpServiceArgVectors">The null-terminated strings to be passed to the ServiceMain function for the service as arguments. If there are no arguments, this parameter can be NULL. Otherwise, the first argument (lpServiceArgVectors[0]) is the name of the service, followed by any additional arguments (lpServiceArgVectors[1] through lpServiceArgVectors[dwNumServiceArgs-1]).</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);
        /// <summary>
        /// Marks the specified service for deletion from the service control manager database.
        /// </summary>
        /// <param name="hService">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the DELETE access right. For more information, see Service Security and Access Rights.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(IntPtr hService);
        /// <summary>
        /// Creates a service object and adds it to the specified service control manager database.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database. This handle is returned by the OpenSCManager function and must have the SC_MANAGER_CREATE_SERVICE access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="lpServiceName">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="lpDisplayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="dwDesiredAccess">The access to the service. Before granting the requested access, the system checks the access token of the calling process. For a list of values, see Service Security and Access Rights. <paramref name="SERVICE_ACCESS"/></param>
        /// <param name="dwServiceType">The service type. This parameter can be one of the following values. <paramref name="SERVICE_TYPE"/></param>
        /// <param name="dwStartType">The service start options. This parameter can be one of the following values. <paramref name="SERVICE_START_TYPE"/></param>
        /// <param name="dwErrorControl">The severity of the error, and action taken, if this service fails to start. This parameter can be one of the following values. <paramref name="SERVICE_ERROR_TYPE"/> </param>
        /// <param name="lpBinaryPathName">The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).
        /// If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</param>
        /// <param name="lpLoadOrderGroup">The names of the load ordering group of which this service is a member. Specify NULL or an empty string if the service does not belong to a group.
        /// The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder</param>
        /// <param name="lpdwTagId">A pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.
        /// You can use a tag for ordering service startup within a load ordering group by specifying a tag order vector in the following registry value:
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
        /// Tags are only evaluated for driver services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.</param>
        /// <param name="lpDependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service. Specify NULL or an empty string if the service has no dependencies. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
        /// You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, because services and service groups share the same name space.</param>
        /// <param name="lpServiceStartName ">The name of the account under which the service should run. If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. The service process will be logged on as this user. If the account belongs to the built-in domain, you can specify .\UserName.
        /// If this parameter is NULL, CreateService uses the LocalSystem account. If the service type specifies SERVICE_INTERACTIVE_PROCESS, the service must run in the LocalSystem account.
        /// If this parameter is NT AUTHORITY\LocalService, CreateService uses the LocalService account. If the parameter is NT AUTHORITY\NetworkService, CreateService uses the NetworkService account.
        /// A shared process can run as any user.
        /// If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, the name is the driver object name that the system uses to load the device driver. Specify NULL if the driver is to use a default object name created by the I/O system.
        /// A service can be configured to use a managed account or a virtual account. If the service is configured to use a managed service account, the name is the managed service account name. If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName. For more information about managed service accounts and virtual accounts, see the Service Accounts Step-by-Step Guide.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  Managed service accounts and virtual accounts are not supported until Windows 7 and Windows Server 2008 R2.</param>
        /// <param name="lpPassword">The password to the account name specified by the lpServiceStartName parameter. Specify an empty string if the account has no password or if the service runs in the LocalService, NetworkService, or LocalSystem account. For more information, see Service Record List.
        /// If the account name specified by the lpServiceStartName parameter is the name of a managed service account or virtual account name, the lpPassword parameter must be NULL.
        /// Passwords are ignored for driver services.</param>
        /// <returns>If the function succeeds, the return value is a handle to the service.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName,
                                                   SERVICE_ACCESS dwDesiredAccess, SERVICE_TYPE dwServiceType, SERVICE_START_TYPE dwStartType,
                                                   SERVICE_ERROR_TYPE dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup,
                                                   IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        #endregion DllImports
        #endregion win32 declaration
        /// <summary>
        /// Converts from a pointer to an array of null-separated names to string[]
        /// </summary>
        /// <param name="ptr">A pointer to an array of null-separated names The array is doubly null-terminated.</param>
        /// <returns></returns>
        private static string[] GetStringArrayFromPtrArrayDoublyNullTerminated(IntPtr ptr)
        {
            var a = new string[] { };
            IntPtr p = ptr;
            string s = string.Empty;
            while (true)
            {
                s = Marshal.PtrToStringAuto(p);
                if (s.Length == 0) break;

                Array.Resize<string>(ref a, a.Length + 1);
                a[a.Length - 1] = s;
                p = new IntPtr(p.ToInt64() + (s.Length + 1) * Marshal.SystemDefaultCharSize);
            }
            return a;
        }
    }
}
