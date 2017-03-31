/*
  IDDType.cs -- interface for 'DrType', November 12, 2016
 
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

namespace DrOpen.DrCommon.IDrData
{
    public interface IDDType
    {
        /// <summary>
        /// Compare type by name
        /// </summary>
        /// <param name="other">other type for comparison</param>
        /// <returns></returns>
        int CompareTo(IDDType other);
        /// <summary>
        /// Compare type by name
        /// </summary>
        /// <param name="obj">other type for comparison as object</param>
        /// <returns></returns>
        int CompareTo(object obj);
        /// <summary>
        /// Determines whether the specified DDType is equal to the current DDType. Returns true if the specified DDType is equal to the current DDType otherwise, false.
        /// </summary>
        /// <param name="other">other type for comparison</param>
        /// <returns>true if the specified DDType is equal to the current DDType otherwise, false.</returns>
        bool Equals(IDDType other);
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current DDType. Returns true if the specified System.Object is equal to the current DDType otherwise, false
        /// </summary>
        /// <param name="other">other type for comparison as object</param>
        /// <returns>true if the specified System.Object is equal to the current DDType otherwise, false.</returns>
        bool Equals(object other);
        /// <summary>
        /// Returns the hash code for this DDType which get from Name property
        /// </summary>
        /// <returns></returns>
        int GetHashCode();

        /// <summary>
        /// Get or set name of type 
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Validate current node type of with expected node type.
        /// </summary>
        /// <param name="expectedType">expected node type</param>
        void ValidateExpectedNodeType(IDDType expectedType);
        /// <summary>
        /// Validate current node type of with expected node type.
        /// </summary>
        /// <param name="expectedType">expected node type</param>
        void ValidateExpectedNodeType(string expectedType);
    }
}
