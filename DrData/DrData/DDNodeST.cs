/*
  DDNodeST.cs -- single tone for data of the 'DrData' general purpose Data abstraction layer 1.0.1, April 19, 2014
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
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

namespace DrOpen.DrCommon.DrData
{
    /// <summary>
    /// singletone of hierarchy data warehouse
    /// </summary>
    public class DDNodeST
    {
        #region Singleton
        /// <summary>
        /// static instance of root node
        /// </summary>
        static volatile DDNode sm_instance;
        /// <summary>
        /// object for lock
        /// </summary>
        private static object lockDDNode = new Object();
        /// <summary>
        /// private static constructor
        /// </summary>
        static DDNodeST()
        {
        }
        /// <summary>
        /// private constructor
        /// </summary>
        DDNodeST()
        {

        }
        /// <summary>
        /// return root
        /// </summary>
        /// <returns></returns>
        public static DDNode GetInstance()
        {
            return GetInstance(string.Empty);
        }

        public static DDNode GetInstance(Enum name)
        {
            return GetInstance(name.ToString());
        }

        /// <summary>
        /// return named instance
        /// </summary>
        /// <param name="name">name of child node. Empty for root node</param>
        public static DDNode GetInstance(string name)
        {
            if (sm_instance == null)
            {
                lock (lockDDNode)
                {
                    if (sm_instance == null) sm_instance = new DDNode();
                }
            }
            if (name.Length == 0) return sm_instance; // return root
            if (sm_instance.Contains(name))
            {
                lock (lockDDNode) // lock here must have -) UTest
                {
                    return sm_instance.GetNode(name); // return exist named node
                }
            }
            DDNode newNode;
            lock (lockDDNode)
            {
                newNode = sm_instance.Contains(name) ? sm_instance.GetNode(name) : sm_instance.Add(name); // additional verification for multithreading
            }
            return newNode;
        }

        #endregion Singleton
    }
}
