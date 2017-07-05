/*
  LoggerST.cs -- single tone for data of the 'Logger' client for DrLog, July 02, 2017
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
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

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
        /// <summary>
    /// singletone of Logger
    /// </summary>
    public class LoggerST : Logger
    {
                #region Singleton
        /// <summary>
        /// static instance of logger
        /// </summary>
        static volatile Dictionary<string, Logger> sm_instance;
        /// <summary>
        /// object for lock
        /// </summary>
        private static object lockLogger = new Object();
        /// <summary>
        /// private static constructor
        /// </summary>
        static LoggerST()
        {
        }
        /// <summary>
        /// private constructor
        /// </summary>
        LoggerST()
        {

        }
        /// <summary>
        /// return logger
        /// </summary>
        /// <returns></returns>
        public static Logger GetInstance()
        {
            return GetInstance(string.Empty);
        }

        public static Logger GetInstance(Enum name)
        {
            return GetInstance(name.ToString());
        }
        /// <summary>
        /// return named instance
        /// </summary>
        /// <param name="name">name of child node. Empty for root node</param>
        public static Logger GetInstance(string name)
        {
            Logger logger;
            if (sm_instance == null)
            {
                lock (lockLogger)
                {
                    if (sm_instance == null) sm_instance = new Dictionary<string, Logger>();
                }
            }
            lock (lockLogger) // lock here must have -) UTest
            {
                if (sm_instance.ContainsKey(name))
                    logger = sm_instance[name]; // return exist named node
                else
                {
                    logger = new Logger();
                    sm_instance.Add(name, logger);
                }
            }          
            return logger;
        }
        #endregion Singleton
    }
}
