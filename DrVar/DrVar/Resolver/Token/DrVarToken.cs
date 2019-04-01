/*
  DrVarToken.cs -- stored variable token for 'DrVar' general purpose variables builder 1.1.0, February 09, 2019
 
  Copyright (c) 2013-2019 Kudryashov Andrey aka dr
 
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

      Kudryashov Andrey <kudryashov dot andrey at gmail dot com>

 */

using System;
namespace DrOpen.DrCommon.DrVar.Resolver.Token
{
    /// <summary>
    /// stored variable token, immutable structure
    /// </summary>
    internal struct DrVarToken : ICloneable

    {
        /// <summary>
        /// Initializes a new instance of the variable item
        /// </summary>
        /// <param name="startIndex">start index of substitution symbol</param>
        /// <param name="endIndex">end index of substitution symbol</param>
        /// <param name="name">name of substitution without substitution symbols</param>
        /// <param name="fullName">name of substitution with substitution symbols</param>
        internal DrVarToken(int startIndex,
                           int endIndex,
                           string name,
                           string fullName): this()
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Name = name;
            FullName = fullName;
        }

        internal DrVarToken(DrVarToken it) : this(it.StartIndex, it.EndIndex, it.Name, it.FullName)
        { }

        /// <summary>
        /// Updates token positions. If <c>StartIndex</c> or <c>EndIndex</c> to the right of <paramref name="fromPosition"/>fromPosition</> they will change on the <paramref name="diffLength"/>
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <param name="diffLength"></param>
        public void ShiftToken(int fromPosition, int diffLength)
        {
            if (this.StartIndex > fromPosition) this.StartIndex += diffLength;
            if (this.EndIndex > fromPosition) this.EndIndex += diffLength;
        }

        /// <summary>
        /// Gets/sets the start index of substitution symbol
        /// </summary>
        internal int StartIndex { private set; get; }
        /// <summary>
        /// Gets/sets the end index of substitution symbol
        /// </summary>
        public int EndIndex { private set; get; }
        /// <summary>
        /// Gets/sets the name of substitution without substitution symbols
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// Gets/sets the name of substitution with substitution symbols
        /// </summary>
        public string FullName { private set; get; }

        public DrVarToken Clone()
        {
            return new DrVarToken(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
