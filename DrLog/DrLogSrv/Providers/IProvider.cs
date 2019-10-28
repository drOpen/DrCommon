﻿/*
  ILogProvider.cs -- interface for DrLog provider 1.0, August 8, 2015
 
  Copyright (c) 2013-2015 Kudryashov Andrey aka Dr
 
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

using DrOpen.DrData.DrDataObject;
using System;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    /// <summary>
    /// interface define contract for log provider
    /// </summary>
    public interface IProvider : IDisposable
    {
        /// <summary>
        /// write messages to provider
        /// </summary>
        /// <param name="msg"></param>
        void Write(DDNode msg);
        /// <summary>
        /// return name of current provider
        /// </summary>
        string Name { get; }
        /// <summary>
        /// return Type of provider by configuration node
        /// </summary>
        DDType Type { get; }
        /// <summary>
        /// get/set configuration of current provider
        /// </summary>
        DDNode Config { get; set; }
        /// <summary>
        /// return default configuration of current provider
        /// </summary>
        DDNode DefaultConfig { get; }
        /// <summary>
        /// returns the log level filter for current provider
        /// </summary>
        DrLogSrv.LogLevel Level { get;}
        /// <summary>
        /// returns the log exception level filter for current provider
        /// </summary>
        DrLogSrv.LogExceptionLevel ExceptionLevel { get; }
        /// <summary>
        /// Update settings to current config
        /// </summary>
        void RebuildConfiguration();
        /// <summary>
        /// Update settings from config
        /// </summary>
        void RebuildConfiguration(DDNode config);
    }
}
