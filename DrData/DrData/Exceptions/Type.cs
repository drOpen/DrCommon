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
    public class DDTypeException : Exception
    {
        public const string NullTypeName = "null";
        /// <summary>
        /// DrData type exception
        /// <param name="name">Type name</param>
        /// </summary>
        public DDTypeException(string name)
            : base() { this.TypeName = name; }
        /// <summary>
        /// DrData type exception
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeException(string name, string message)
            : base(message) { this.TypeName = name; }
        /// <summary>
        /// DrData type exception
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeException(string name, string message, Exception innerException)
            : base(message, innerException) { this.TypeName = name; }
        /// <summary>
        /// Name of type
        /// </summary>
        public virtual string TypeName { get; private set; }
    }


    public class DDTypeNullException : DDTypeException
    {
        /// <summary>
        /// Initializes a new instance of the DDTypeNullException class without error message. The type name is "null".
        /// </summary>
        public DDTypeNullException()
            : base(NullTypeName) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeNullException class with the specified error message. The type name is "null".
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeNullException(string message)
            : base(NullTypeName, message) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeNullException class with the specified error message. The type name is "null".
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeNullException(string message, Exception innerException)
            : base(NullTypeName, message, innerException) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DDTypeConvertException : DDTypeException
    {
        /// <summary>
        /// Initializes a new instance of the DDTypeConvertExceptions class without error message.
        /// <param name="currentType">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// </summary>
        public DDTypeConvertException(string currentType, string requestedTypeName)
            : base(currentType) { this.RequestedTypeName = requestedTypeName; }
        /// <summary>
        /// Initializes a new instance of the DDTypeConvertExceptions class with the specified error message.
        /// <param name="currentType">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// </summary>
        public DDTypeConvertException(string currentType, string requestedTypeName, string message)
            : base(currentType, message) { this.RequestedTypeName = requestedTypeName; }
        /// <summary>
        /// Initializes a new instance of the DDTypeConvertExceptions class with the specified error message.
        /// </summary>
        /// <param name="currentType">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeConvertException(string currentType, string requestedTypeName, string message, Exception innerException)
            : base(currentType, message, innerException) { this.RequestedTypeName = requestedTypeName; }
        /// <summary>
        /// requested type name
        /// </summary>
        public virtual string RequestedTypeName { get; private set; }
    }
 


    /// <summary>
    /// DrData -- type exception
    /// </summary>
    public class DDTypeIncorrectException : DDTypeException
    {
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the default error message.
        /// </summary>
        /// <param name="name">Type name</param>
        public DDTypeIncorrectException(string name)
            : base(name, string.Format(Res.Msg.OBJ_TYPE_IS_INCORRECT, name)) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the default error message.
        /// </summary>
        /// <param name="type">Type</param>
        public DDTypeIncorrectException(Type type)
            : this((type == null ? NullTypeName : type.Name)) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the specified error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeIncorrectException(string name, string message)
            : base(name, message) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the specified error message.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="message">A message that describes the error.</param>
        public DDTypeIncorrectException(Type type, string message)
            : this((type == null ? NullTypeName : type.Name), message) { }

        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the default error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectException(string name, Exception innerException)
            : base(name, string.Format(Res.Msg.OBJ_TYPE_IS_INCORRECT, name), innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the default error message.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectException(Type type, Exception innerException)
            : this((type == null ? NullTypeName : type.Name), innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the specified error message.
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectException(string name, string message, Exception innerException)
            : base(name, message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeIncorrectException class with the specified error message.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeIncorrectException(Type type, string message, Exception innerException)
            : this((type == null ? NullTypeName : type.Name), message, innerException) { }
    }

    /// <summary>
    /// DrData -- type exception
    /// </summary>
    public class DDTypeExpectedException : DDTypeConvertException
    {
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the default error message.
        /// <param name="currentTypeName">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// </summary>
        public DDTypeExpectedException(string currentTypeName, string requestedTypeName)
            : base(currentTypeName, requestedTypeName , string.Format(Res.Msg.EXPECTED_NODE_TYPE_IS_INCORRECT, currentTypeName, requestedTypeName)) { }

        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the default error message.
        /// <param name="currentType">Current type</param>
        /// <param name="requestedType">requested type</param>
        /// </summary>
        public DDTypeExpectedException(Type currentType, Type requestedType)
            : this((currentType == null ? NullTypeName : currentType.Name), (requestedType == null ? NullTypeName : requestedType.Name)) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the specified error message.
        /// </summary>
        /// <param name="currentType">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// </summary>
        public DDTypeExpectedException(string currentTypeName, string requestedTypeName, string message)
            : base(currentTypeName, requestedTypeName, message) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the specified error message.
        /// </summary>
        /// <param name="currentType">Current type</param>
        /// <param name="requestedTypeName">requested type</param>
        /// <param name="message">A message that describes the error.</param>
        /// </summary>
        public DDTypeExpectedException(Type currentType, Type requestedType, string message)
            : this((currentType == null ? NullTypeName : currentType.Name), (requestedType == null ? NullTypeName : requestedType.Name), message) { }

        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the default error message.
        /// </summary>
        /// <param name="currentTypeName">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeExpectedException(string currentTypeName, string requestedTypeName,  Exception innerException)
            : base(currentTypeName, requestedTypeName, string.Format(Res.Msg.EXPECTED_NODE_TYPE_IS_INCORRECT, currentTypeName, requestedTypeName), innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the default error message.
        /// </summary>
        /// <param name="currentType">Current type</param>
        /// <param name="requestedType">requested type</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeExpectedException(Type currentType, Type requestedType, Exception innerException)
            : this((currentType == null ? NullTypeName : currentType.Name), (requestedType == null ? NullTypeName : requestedType.Name), innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the specified error message.
        /// </summary>
        /// <param name="currentTypeName">current type name</param>
        /// <param name="requestedTypeName">requested type name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeExpectedException(string currentTypeName, string requestedTypeName, string message, Exception innerException)
            : base(currentTypeName, requestedTypeName, message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the DDTypeExpectedException class with the specified error message.
        /// </summary>
        /// <param name="currentType">Current type</param>
        /// <param name="requestedType">requested type</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDTypeExpectedException(Type currentType, Type requestedType, string message, Exception innerException)
            : this((currentType == null ? NullTypeName : currentType.Name), (requestedType == null ? NullTypeName : requestedType.Name), message, innerException) { }
    }

}
