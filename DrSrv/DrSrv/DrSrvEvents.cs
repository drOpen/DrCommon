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

namespace DrOpen.DrCommon.DrSrv
{
    /// <summary>
    /// Provides SCM event
    /// </summary>
    public class DrSCMEventArgs : EventArgs
    {
        public DrSCMEventArgs(IntPtr hSCM)
        {
            this.HSCM = hSCM;
        }
        /// <summary>
        /// a handle to the openned service control manager database
        /// </summary>
        public IntPtr HSCM { private get; set; }
    }

    /// <summary>
    /// Provides service event
    /// </summary>
    public class DrSrvEventArgs : EventArgs 
    {
        public DrSrvEventArgs(IntPtr hService)
        {
            this.HService = hService;
        }
        /// <summary>
        /// a handle to the openned service.
        /// </summary>
        public IntPtr HService { private get; set;  }
    }

    /// <summary>
    /// Service status event
    /// </summary>
    public class DrSrvStatusEventArgs : DrSrvEventArgs
    {
        public DrSrvStatusEventArgs(IntPtr hService) : base (hService) {}
        /// <summary>
        /// current service status
        /// </summary>
        public DrSrvHelper.SERVICE_CURRENT_STATE CurrentSrvState { get; set; }
    }

    /// <summary>
    /// Expected service status event
    /// </summary>
    public class DrSrvChangeStatusEventArgs : DrSrvStatusEventArgs
    {
        public DrSrvChangeStatusEventArgs(IntPtr hService, DrSrvHelper.SERVICE_CURRENT_STATE expectedSrvState) : base(hService) 
        {
            this.ExpectedSrvState = expectedSrvState;
        }
        /// <summary>
        /// expected service status
        /// </summary>
        public DrSrvHelper.SERVICE_CURRENT_STATE ExpectedSrvState { private get; set; }
        /// <summary>
        /// Remaining timeout
        /// </summary>
        public int RemainigTimeOut { get; set; }
        /// <summary>
        /// set true to Cancel operation
        /// </summary>
        public bool Cancel { get; set; }
    }

}
