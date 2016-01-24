/*
  SchemaProvider.cs -- configuration schema of provider for log for DrLog 1.0.0, January 24, 2016
 
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

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    /// <summary>
    /// Common constants for configuration schema of provider
    /// </summary> 
    public static class SchemaProvider
    {
        /// <summary>
        /// Log level of provider
        /// </summary>
        public const string AttLevel = "Level";
        /// <summary>
        /// Log Exception level of provider
        /// </summary>
        public const string AttExceptionLevel = "ExceptionLevel";

    }

}
