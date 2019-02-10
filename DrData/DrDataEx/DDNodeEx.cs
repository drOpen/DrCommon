/*
  DDNodeEx.cs -- extensions for 'DrData.DDNode' 1.0, June 1, 2017
 
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
using System.Collections.Generic;

namespace DrOpen.DrCommon.DrData
{
    public static class DDNodeEx
    {

        #region GetAttributeValue

        /// <summary>
        /// Returns attribute value by attribute name for specified path to the node . If the node by path or attribute does not exist return the default value.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="path">The path to the node at which it is necessary to take the value of an attribute</param>
        /// <param name="name">Attribute name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Returns attribute value by attribute name for specified path to the node. 
        /// If the attribute does not exist returns the default value.</returns>
        public static DDValue GetAttributeValue(this DDNode node, string path, string name, object defaultValue)
        {
            try
            {
                return GetAttributeValue(node.GetNode(path), name, defaultValue);
            }
            catch
            {
                return new DDValue(defaultValue);
            }
        }
        /// <summary>
        /// Returns attribute value by attribute name for current node. If the attribute does not exist return the default value.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="name">Attribute name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Returns attribute value by attribute name for current node. If the attribute does not exist returns the default value.</returns>
        public static DDValue GetAttributeValue(this DDNode node, string name, object defaultValue)
        {
            try
            {
                return node.Attributes.GetValue(name, defaultValue);
            }
            catch
            {
                return new DDValue(defaultValue);
            }
        }
        #endregion GetAttributeValue

        #region Traversal
        /// <summary>
        /// Traversal all node from a root. The root will be excluded.
        /// </summary>
        /// <param name="n">root node for traversal</param>
        /// <returns></returns>
        public static IEnumerable<DDNode> Traverse(this DDNode n)
        {
            return Traverse(n, false, false, true);
        }
        /// <summary>
        /// Traversal all node from a root. The root can be included.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="returnRoot">set true if you would like recieve the root</param>
        /// <returns></returns>
        public static IEnumerable<DDNode> Traverse(this DDNode n, bool returnRoot)
        {
            return Traverse(n, returnRoot, true, false);
        }
        /// <summary>
        /// Traversal all node from root
        /// </summary>
        /// <param name="returnRoot">set true for traverse the root</param>
        /// <param name="skipByType">set 'true' for skip nodes by specified types of nodes. Overwise, set 'false, for process only specified types of nodes </param>
        /// <param name="processChildNodeForSkippedNode">set 'true' for enumerate child nodes for skipped nodes by type. Overwise, set 'false, for skip node with children by type.</param>
        /// <param name="types">array of types of nodes to skip or process depends by <paramref name="skipByType"/></param>
        /// <returns></returns>
        public static IEnumerable<DDNode> Traverse(this DDNode n, bool returnRoot, bool skipByType, bool processChildNodeForSkippedNode, params DDType[] types)
        {
            if ((returnRoot == false) && (n.HasChildNodes == false)) yield break;
            var nCurrent = n;
            var queueNodes = new Queue<DDNode>(new[] { n });
            var isRootNode = true;
            while (queueNodes.Count > 0)
            {
                nCurrent = queueNodes.Dequeue();
                bool needSkip = ((types.Length != 0) && (mustSkipByNodeType(nCurrent.Type, skipByType, types)));
                if (((isRootNode == false) || (returnRoot)) && (needSkip == false)) yield return nCurrent;

                if ((isRootNode) && (returnRoot) && (needSkip) && (processChildNodeForSkippedNode == false)) yield break; // skip root and all child nodes
                
                foreach (var nChild in nCurrent)
                {
                    if ((types.Length == 0) || (processChildNodeForSkippedNode))
                        queueNodes.Enqueue(nChild.Value);
                    else
                        if ((processChildNodeForSkippedNode == false) && (mustSkipByNodeType(nChild.Value.Type, skipByType, types) == false)) queueNodes.Enqueue(nChild.Value);
                }
                if (isRootNode) isRootNode = false;
            }
        }
        /// <summary>
        /// Returns true if specified type of node should be skip
        /// </summary>
        /// <param name="type">type of node to analyze</param>
        /// <param name="skipByType">set 'true' for skip nodes by specified types of nodes. Overwise, set 'false, for process only specified types of nodes </param>
        /// <param name="types">array of types of nodes to skip or process depends by <paramref name="skipByType"/></param>
        /// <returns></returns>
        private static bool mustSkipByNodeType(DDType type, bool skipByType, params DDType[] types)
        {
            bool bMustSkip = !skipByType;
            foreach (var t in types)
            {
                if (type == t)
                {
                    bMustSkip = skipByType;
                    break;
                }
            }
            return bMustSkip;
        }

        #endregion Traversal
    }
}
