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
    public class DDNodeException : Exception
    {
        /// <summary>
        /// DrData node exception
        /// </summary>
        public DDNodeException()
            : base() { }
        /// <summary>
        /// DrData node exception
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeException(string message)
            : base(message) { }
        /// <summary>
        /// DrData node exception
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeException(string message, Exception innerException)
            : base(message, innerException) { }

    }

    #region Add
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when you try to add node which has another parent node.
    /// </summary>
    public class DDNodeAddNodeWithParent : DDNodePathException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNodeWithParent class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodeAddNodeWithParent(string path)
            : base(path, string.Format(Res.Msg.CANNOT_ADD_NODE_BELONG_TO_ANOTHER_PARENT_NODE, path))
        { }
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
    public class DDNodeAddSelf : DDNodePathException
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
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddSelf class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeAddSelf(string path, string message, Exception innerException)
            : base(path, message, innerException)
        { }
    }
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node is null.
    /// </summary>
    public class DDNodeNullException : DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullException class with the default error message.
        /// </summary>
        public DDNodeNullException()
            : base(Res.Msg.CANNOT_NULL_NODE)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullException class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public DDNodeNullException(string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullException(Exception innerException)
            : base(Res.Msg.CANNOT_NULL_NODE, innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeAddNullException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    #endregion Add
    #region Path
    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node path is null.
    /// </summary>
    public class DDNodeNullPathException : DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathException class with the default error message.
        /// </summary>
        public DDNodeNullPathException()
            : base(Res.Msg.PATH_IS_NULL)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathException class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public DDNodeNullPathException(string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullPathException(Exception innerException)
            : base(Res.Msg.PATH_IS_NULL, innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodeNullPathException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeNullPathException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when you try to rise above the root node.
    /// </summary>
    public class DDNodePathAboveRootException : DDNodePathException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootException class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodePathAboveRootException(string path)
            : base(path, string.Format(Res.Msg.RISE_ABOVE_ROOT_NODE, path))
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootException class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodePathAboveRootException(string path, string message)
            : base(path, message)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathAboveRootException(string path, Exception innerException)
            : base(path, string.Format(Res.Msg.RISE_ABOVE_ROOT_NODE, path), innerException)
        { }
        /// <summary>
        /// Initializes a new instance of the DDNodePathAboveRootException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathAboveRootException(string path, string message, Exception innerException)
            : base(path, message, innerException)
        { }
    }

    /// <summary>
    /// DrData -- node exception -- the exception that is thrown when the node path is incorrect.
    /// </summary>
    public class DDNodePathException : DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodePathException class with the default error message.
        /// </summary>
        /// <param name="path">Path to node</param>
        public DDNodePathException(string path)
            : base(string.Format(Res.Msg.INCORRECT_PATH, path))
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathException class with a specified error message.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error</param>
        public DDNodePathException(string path, string message)
            : base(message)
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathException(string path, Exception innerException)
            : base(string.Format(Res.Msg.INCORRECT_PATH, path), innerException)
        { this.Path = path; }
        /// <summary>
        /// Initializes a new instance of the DDNodePathException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="path">path to node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodePathException(string path, string message, Exception innerException)
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
    public class DDNodeMergeNameException : DDNodeIncorrectNameException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameException class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        public DDNodeMergeNameException(string name, string destinationPath)
            : base(name, string.Format(Res.Msg.CANNOT_MERGE_NODE_WITH_EXIST_NAME, name, destinationPath))
        {
            this.DestinationPath = destinationPath;
        }
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameException class with a specified error message.
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeMergeNameException(string name, string destinationPath, string message)
            : base(name, message)
        {
            this.DestinationPath = destinationPath;

        }
        /// <summary>
        /// Initializes a new instance of the DDNodeMergeNameException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeMergeNameException(string name, string destinationPath, Exception innerException)
            : base(name, string.Format(Res.Msg.INCORRECT_NODE_NAME, name), innerException)
        { this.DestinationPath = destinationPath; }
        /// <summary>
        ///  Initializes a new instance of the DDNodeMergeNameException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="destinationPath">path to destination node</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeMergeNameException(string name, string destinationPath, string message, Exception innerException)
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
    public class DDNodeIncorrectNameException : DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameException class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        public DDNodeIncorrectNameException(string name)
            : base(string.Format(Res.Msg.INCORRECT_NODE_NAME, name))
        { this.Name = name; }
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameException class with a specified error message.
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeIncorrectNameException(string name, string message)
            : base(message)
        { this.Name = name; }
        /// <summary>
        /// Initializes a new instance of the DDNodeIncorrectNameException class with the default message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeIncorrectNameException(string name, Exception innerException)
            : base(string.Format(Res.Msg.INCORRECT_NODE_NAME, name), innerException)
        { this.Name = name; }
        /// <summary>
        ///  Initializes a new instance of the DDNodeIncorrectNameException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeIncorrectNameException(string name, string message, Exception innerException)
            : base(message, innerException)
        { this.Name = name; }
        /// <summary>
        /// Node name
        /// </summary>
        public virtual string Name { get; private set; }
    }
    /// <summary>
    /// DrData -- node with same name already exist exception
    /// </summary>
    public class DDNodeExistsException : DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodeExistsException class with the default error message.
        /// <param name="name">Node name</param>
        /// </summary>
        public DDNodeExistsException(string name)
            : base(string.Format(Res.Msg.NODE_EXISTS, name)) { this.Name = name; }
        /// <summary>
        /// Initializes a new instance of the DDNodeExistsException class with the specified error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodeExistsException(string name, string message)
            : base(message) { this.Name = name; }

        /// <summary>
        /// Initializes a new instance of the DDNodeExistsException class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeExistsException(string name, Exception innerException)
            : base(string.Format(Res.Msg.NODE_EXISTS, name), innerException) { this.Name = name; }

        /// <summary>
        /// Initializes a new instance of the DDNodeExistsException class with the specified error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodeExistsException(string name, string message, Exception innerException)
            : base(message, innerException) { this.Name = name; }

        /// <summary>
        /// Node name
        /// </summary>
        public virtual string Name { get; private set; }
    }
    /// <summary>
    /// DrData -- nodes belong to the different trees
    /// </summary>
    public class DDNodesBelongDifferentTrees: DDNodeException
    {
        /// <summary>
        /// Initializes a new instance of the DDNodesBelongDifferentTrees class with the default error message.
        /// <param name="name">Node name</param>
        /// </summary>
        public DDNodesBelongDifferentTrees(params string[] names)
            : base(string.Format(Res.Msg.NODE_EXISTS, String.Join(", ",  names))) { this.Names = names; }
        /// <summary>
        /// Initializes a new instance of the DDNodesBelongDifferentTrees class with the specified error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="message">A message that describes the error.</param>
        public DDNodesBelongDifferentTrees(string[] names, string message)
            : base(message) { this.Names = names; }

        /// <summary>
        /// Initializes a new instance of the DDNodesBelongDifferentTrees class with the default error message.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodesBelongDifferentTrees(string[] names, Exception innerException)
            : base(string.Format(Res.Msg.NODE_EXISTS, String.Join(", ",  names)), innerException) { this.Names = names; }

        /// <summary>
        /// Initializes a new instance of the DDNodesBelongDifferentTrees class with the specified error message.
        /// </summary>
        /// <param name="name">Nodes names</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public DDNodesBelongDifferentTrees(string[] names, string message, Exception innerException)
            : base(message, innerException) { this.Names = names; }

        /// <summary>
        /// Node name
        /// </summary>
        public virtual string[] Names { get; private set; }
    }
}