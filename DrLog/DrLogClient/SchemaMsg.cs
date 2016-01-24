/*
  SchemaMsg.cs -- schema of message for log for DrLog 1.0.0, January 24, 2016
 
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
    /// <summary>
    /// Constants for schema of message for log
    /// </summary> 
    public static class SchemaMsg
    {
        /// <summary>
        /// DDNode type for messages
        /// </summary>
        public const string MessageType = "DrLogMessage";
        #region basic attributes for messages
        /// <summary>
        /// creation time of the message 
        /// </summary>
        public const string AttDateTime = "DateTime";
        /// <summary>
        /// body of message
        /// </summary>
        public const string AttBody = "Body";
        /// <summary>
        /// Log level of message
        /// </summary>
        public const string AttLevel = "Level";
        /// <summary>
        /// Log exception of message
        /// </summary>
        public const string AttException = "Exception";
        /// <summary>
        /// Who created the message 
        /// </summary>
        public const string AttSource = "Source";
        /// <summary>
        /// The list of providers who will be read this message. by default all providers
        /// </summary>
        public const string AttProviders = "Providers";
        /// <summary>
        /// The list of recipients who will be receive this message. by default all recipients
        /// </summary>
        public const string AttRecipients = "Recipients";
        #endregion basic attributes for messages
    }
}
