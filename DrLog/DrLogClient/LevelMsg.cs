﻿/*
  LevelMsg.cs -- message level for log for DrLog 1.0.0, January 24, 2016
 
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

namespace DrOpen.DrCommon.DrLog.DrLogClient
{

    #region Level
    /// <summary>
    /// Log Level
    /// </summary>
    [Flags]
    public enum LogLevel : int
    {
        /// <summary>
        /// Exclude all messages
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Error
        /// </summary>
        ERR = 1,
        /// <summary>
        /// Warning
        /// </summary>
        WAR = 2,
        /// <summary>
        /// Information
        /// </summary>
        INF = 4,
        /// <summary>
        /// Trace
        /// </summary>
        TRC = 8,
        /// <summary>
        /// Debug
        /// </summary>
        DBG = 16,
        /// <summary>
        /// Only inforamtion mesages: <see cref="LogLevel.ERR"/>, <see cref="LogLevel.WAR"/> and <see cref="LogLevel.INF"/>
        /// </summary>
        INFO = (ERR | WAR | INF),
        /// <summary>
        /// Extands <see cref="INFO"/> adds <see cref="LogLevel.TRC"/>
        /// </summary>
        TRACE = INFO | TRC,
        /// <summary>
        /// Extands <see cref="TRACE"/> adds <see cref="LogLevel.DBG"/>. All messages including debug level
        /// </summary>
        ALL = TRACE | DBG
    }
    #endregion Level
   

}
