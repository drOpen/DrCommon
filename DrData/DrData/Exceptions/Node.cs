/*
  Node.cs -- stored node exceptions for DrData, January 24, 2016
  
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
    /// DrData -- node exception
    /// </summary>
    public class DDNodeExceptions : Exception
    {
        /// <summary>
        /// DrData node exception
        /// </summary>
        public DDNodeExceptions()
            : base() { }
        /// <summary>
        /// DrData node exception
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeExceptions(string message)
            : base(message) { }
        /// <summary>
        /// DrData node exception
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeExceptions(string message, Exception innerException)
            : base(message, innerException) { }

    }

    #region Add
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when you try to add node which has another parent node.
    /// </summary>
    public class DDNodeAddNodeWithParent : DDNodePathExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNodeWithParent class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodeAddNodeWithParent(string path)
            : base(path, string.Format(Res.Msg.CANNOT_ADD_NODE_BELONG_TO_ANOTHER_PARENT_NODE, path))
        {  }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNodeWithParent class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodeAddNodeWithParent(string path, string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNodeWithParent class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddNodeWithParent(string path, Exception innerException)
            : base(string.Format(Res.Msg.CANNOT_ADD_NODE_BELONG_TO_ANOTHER_PARENT_NODE, path), innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNodeWithParent class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddNodeWithParent(string path, string message, Exception innerException)
            : base(message, innerException)
        { }
    }
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when you try to add yourself as child node.
    /// </summary>
    public class DDNodeAddSelf : DDNodePathExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeAddSelf class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodeAddSelf(string path)
            : base(path, string.Format(Res.Msg.CANNOT_ADD_YOURSELF_AS_CHILD, path))
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddSelf class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodeAddSelf(string path, string message)
            : base(path, message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddSelf class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddSelf(string path, Exception innerException)
            : base(path, string.Format(Res.Msg.CANNOT_ADD_YOURSELF_AS_CHILD, path), innerException)
        {}
        /// <summary>
        /// Initializes a new instance of the DDNodeAddSelf class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddSelf(string path, string message, Exception innerException)
            : base(path, message, innerException)
        {}
    }
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the try add null node.
    /// </summary>
    public class DDNodeAddNullExceptions : DDNodeExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullExceptions class with the default error message.
        /// </summary>
        public DDNodeAddNullExceptions()
            : base(Res.Msg.CANNOT_NULL_NODE)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullExceptions class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public DDNodeAddNullExceptions(string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddNullExceptions(Exception innerException)
            : base(Res.Msg.CANNOT_NULL_NODE, innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddNullExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    #endregion Add
    #region Path
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node path is null.
    /// </summary>
    public class DDNodeNullPathExceptions : DDNodeExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathExceptions class with the default error message.
        /// </summary>
        public DDNodeNullPathExceptions()
            : base(Res.Msg.PATH_IS_NULL)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathExceptions class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public DDNodeNullPathExceptions(string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullPathExceptions(Exception innerException)
            : base(Res.Msg.PATH_IS_NULL, innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullPathExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when you try to rise above the root node.
    /// </summary>
    public class DDNodePathAboveRootExceptions : DDNodePathExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootExceptions class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodePathAboveRootExceptions(string path)
            : base(path, string.Format(Res.Msg.RISE_ABOVE_ROOT_NODE, path))
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootExceptions class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodePathAboveRootExceptions(string path, string message)
            : base(path, message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathAboveRootExceptions(string path, Exception innerException)
            : base(path, string.Format(Res.Msg.RISE_ABOVE_ROOT_NODE, path), innerException)
        {  }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathAboveRootExceptions(string path, string message, Exception innerException)
            : base(path , message, innerException)
        {  }
    }

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node path is incorrect.
    /// </summary>
    public class DDNodePathExceptions : DDNodeExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodePathExceptions class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodePathExceptions(string path)
            : base(string.Format(Res.Msg.INCORRECT_PATH, path))
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathExceptions class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodePathExceptions(string path, string message)
            : base(message)
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathExceptions(string path, Exception innerException)
            : base(string.Format(Res.Msg.INCORRECT_PATH, path), innerException)
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathExceptions(string path, string message, Exception innerException)
            : base(message, innerException)
        { this.Path = path; }
        /// <summary>
        /// path to node
        /// </summary>
        public virtual string Path { get; private set; }
    }
    #endregion Path

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node name is incorrect.
    /// </summary>
    public class DDNodeMergeNameExceptions : DDNodeIncorrectNameExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameExceptions class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        public DDNodeMergeNameExceptions(string name, string destinationPath)
            : base(name, string.Format(Res.Msg.CANNOT_MERGE_NODE_WITH_EXIST_NAME, name, destinationPath))
        {
            this.DestinationPath = destinationPath;
        }
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameExceptions class with a specified error message.
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeMergeNameExceptions(string name,  string destinationPath ,string message)
            : base(name, message)
        {
            this.DestinationPath = destinationPath;

        }
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeMergeNameExceptions(string name, string destinationPath, Exception innerException)
            : base(name, string.Format(Res.Msg.INCORRECT_NODE_NAME, name), innerException)
        { this.DestinationPath = destinationPath; }
        /// <summary>
        ///  Initializes a new instance of the DDNodeMergeNameExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeMergeNameExceptions(string name, string destinationPath, string message, Exception innerException)
            : base(name, message, innerException)
        { this.DestinationPath = destinationPath; }
        /// <summary>
        /// destination path to node
        /// </summary>
        public virtual string DestinationPath { get; private set; }
    }

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node name is incorrect.
    /// </summary>
    public class DDNodeIncorrectNameExceptions : DDNodeExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameExceptions class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        public DDNodeIncorrectNameExceptions(string name)
            : base(string.Format(Res.Msg.INCORRECT_NODE_NAME, name))
        { this.Name = name; }
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameExceptions class with a specified error message.
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeIncorrectNameExceptions(string name, string message)
            : base(message)
        { this.Name = name; }
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameExceptions class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeIncorrectNameExceptions(string name, Exception innerException)
            : base(string.Format(Res.Msg.INCORRECT_NODE_NAME, name), innerException)
        { this.Name = name; }
        /// <summary>
        ///  Initializes a new instance of the DDNodeIncorrectNameExceptions class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeIncorrectNameExceptions(string name, string message, Exception innerException)
            : base(message, innerException)
        { this.Name = name; }
        /// <summary>
        /// Node name
        /// </summary>
        public virtual string Name { get; private set; }
    }
}