/*
  ILogger.cs -- interface provides the DrLogger, July 02, 2017
 
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
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrLog.DrLogClient
{
    public interface ILogger
    {
        bool LogFullSourceName { get; set; }
        bool LogThreadName { get; set; }
        void Write(DDNode msg);
        void Write(LogLevel logLevel, string body, params object[] bodyArgs);
        void Write(LogLevel logLevel, Exception exception, string body, params object[] bodyArgs);
        void Write(LogLevel logLevel, Exception exception, string[] providers, string[] recipients, string body, params object[] bodyArgs);
        void WriteDebug(Exception exception, string body, params object[] bodyArgs);
        void WriteDebug(string body, params object[] bodyArgs);
        void WriteError(Exception exception, string body, params object[] bodyArgs);
        void WriteError(string body, params object[] bodyArgs);
        void WriteInfo(Exception exception, string body, params object[] bodyArgs);
        void WriteInfo(string body, params object[] bodyArgs);
        void WriteTrace(Exception exception, string body, params object[] bodyArgs);
        void WriteTrace(string body, params object[] bodyArgs);
        void WriteWarning(Exception exception, string body, params object[] bodyArgs);
        void WriteWarning(string body, params object[] bodyArgs);
    }
}
