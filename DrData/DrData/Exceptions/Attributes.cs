/*
  Attributes.cs -- stored attribute exceptions for DrData, January 24, 2016
  
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
    /// DrData -- attributes exception
    /// </summary>
    public class DDAttributeException : Exception
    {
        /// <summary>
        /// DrData attributes exception
        /// </summary>
        public DDAttributeException(string name)
            : base() { this.Name = name; }
        /// <summary>
        /// DrData attributes exception
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDAttributeException(string name, string message)
            : base(message) { this.Name = name; }
        /// <summary>
        /// DrData attributes exception
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDAttributeException(string name, string message, Exception innerException)
            : base(message, innerException) { this.Name = name; }
        /// <summary>
        /// Name of type
        /// </summary>
        public virtual string Name { get; private set; }

    }
    /// <summary>
    /// DrData -- attribute exception -- attribute doesn't exist
    /// </summary>
    public class DDMissingAttributeException : DDAttributeException
    {
        /// <summary>
        /// attribute doesn't exist
        /// </summary>
        /// <param name="name">attribute name</param>
        public DDMissingAttributeException(string name)
            : base(name, string.Format(Res.Msg.ATTR_DOESNT_EXIST, name))
        { }
        /// <summary>
        /// attribute doesn't exist
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="innerException">inner exception</param>
        public DDMissingAttributeException(string name, Exception innerException)
            : base(name, string.Format(Res.Msg.ATTR_DOESNT_EXIST, name), innerException)
        { }
    }

    /// <summary>
    /// DrData -- attributes exception -- attributes don't exist
    /// </summary>
    public class DDMissingSomeOfAttributesException : DDAttributeException
    {
        /// <summary>
        /// attributes don't exist
        /// </summary>
        /// <param name="name">attributes name<s/param>
        public DDMissingSomeOfAttributesException(params string[] names)
            : base(String.Empty, string.Format(Res.Msg.ATTRIBUTES_DONT_EXIST, String.Join(", ", names)))
        {
            this.Names = names;
        }
        /// <summary>
        /// attributes don't exist
        /// </summary>
        /// <param name="name">attributes names</param>
        /// <param name="innerException">inner exception</param>
        public DDMissingSomeOfAttributesException(Exception innerException, params string[] names)
            : base(String.Empty, string.Format(Res.Msg.ATTRIBUTES_DONT_EXIST, String.Join(", ", names)), innerException)
        {
            this.Names = names;
        }
        /// <summary>
        /// attributes names
        /// </summary>
        public virtual string[] Names { get; private set; }
        /// <summary>
        /// gets attributes name as string
        /// </summary>
        public override string Name
        {
            get
            {
                return String.Join(", ", Names);
            }
        }
    }

    /// <summary>
    /// DrData -- attribute already exist exception
    /// </summary>
    public class DDAttributeExistsException : DDAttributeException
    {
        /// <summary>
        /// Initializes a new instance of the DDAttributeExistsException class with the default error message.
        /// <param name="name">Attribute name</param>
        /// </summary>
        public DDAttributeExistsException(string name)
            : base(name, string.Format(Res.Msg.ATTRIBUTE_EXISTS, name)) { }
        /// <summary>
        /// Initializes a new instance of the DDAttributeExistsException class with the specified error message.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDAttributeExistsException(string name, string message)
            : base(name, message) { }

        /// <summary>
        /// Initializes a new instance of the DDAttributeExistsException class with the default error message.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDAttributeExistsException(string name, Exception innerException)
            : base(name, string.Format(Res.Msg.ATTRIBUTE_EXISTS, name), innerException) { }

        /// <summary>
        /// nitializes a new instance of the DDAttributeExistsException class with the specified error message.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDAttributeExistsException(string name, string message, Exception innerException)
            : base(name, message, innerException) { }
    }
}
