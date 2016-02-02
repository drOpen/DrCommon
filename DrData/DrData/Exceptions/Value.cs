/*
  Vaqlue.cs -- stored value exceptions for DrData, January 31, 2016
  
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

namespace DrOpen.DrCommon.DrData.Exceptions
{
    /// <summary>
    /// DrData value exception
    /// </summary>
    public class DDValueExceptions : Exception
    {
        public const string NullValue = "null";
        /// <summary>
        /// DrData value exception
        /// <param name="value">value</param>
        /// </summary>
        public DDValueExceptions(string value)
            : base() { this.Value = value; }
        /// <summary>
        ///DrData value exception
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="message">A message that describes the error.</param>
        public DDValueExceptions(string value, string message)
            : base(message) { this.Value = value; }
        /// <summary>
        /// DrData value exception
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDValueExceptions(string value, string message, Exception innerException)
            : base(message, innerException) { this.Value = value; }
        /// <summary>
        /// Value
        /// </summary>
        public virtual string Value { get; private set; }
    }
}
