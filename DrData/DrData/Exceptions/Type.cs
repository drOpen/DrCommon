/*
  Type.cs -- stored node type exceptions for DrData, January 24, 2016
  
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
    /// DrData -- type exception
    /// </summary>
    public class DDTypeExceptions : Exception
    {
        /// <summary>
        /// DrData type exception
        /// <param name="name">Type name</param>
        /// </summary>
        public DDTypeExceptions(string name)
            : base() { this.Name = name; }
        /// <summary>
        /// DrData type exception
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeExceptions(string name, string message)
            : base(message) { this.Name = name; }
        /// <summary>
        /// DrData type exception
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeExceptions(string name, string message, Exception innerException)
            : base(message, innerException) { this.Name = name; }
        /// <summary>
        /// Name of type
        /// </summary>
        public virtual string Name { get; private set; }
    }

    /// <summary>
    /// DrData -- type exception
    /// </summary>
    public class DDTypeIncorrectExceptions : DDTypeExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectExceptions class with the default error message.
        /// <param name="name">Type name</param>
        /// </summary>
        public DDTypeIncorrectExceptions(string name)
            : base(name, string.Format(Res.Msg.OBJ_TYPE_IS_INCORRECT, name)) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectExceptions class with the specified error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeIncorrectExceptions(string name, string message)
            : base(name, message) { }

        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectExceptions class with the default error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectExceptions(string name, Exception innerException)
            : base(name, string.Format(Res.Msg.OBJ_TYPE_IS_INCORRECT, name), innerException) { }

        /// <summary>
        /// nitializes a new instance of the DDTypeIncorrectExceptions class with the specified error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectExceptions(string name, string message, Exception innerException)
            : base(name, message, innerException) { }
    }
}
