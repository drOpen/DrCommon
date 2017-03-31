/*
  DDTypeSe.cs -- stored type of the 'DrDataSe' general purpose Data abstraction layer 1.1, November 27, 2016
 
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DrOpen.DrCommon.DrDataSe
{
    /// <summary>
    /// the type of the object
    /// </summary>
    [Serializable]
    public class DDTypeSe : DrData.DDType, ISerializable
    {
        #region Constructor
        public DDTypeSe(Enum name): base(name) { }
        public DDTypeSe(string name): base(name) { }
        public DDTypeSe(Type type) : base(type) { }
        /// <summary>
        /// The special constructor is used to deserialize values.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public DDTypeSe(SerializationInfo info, StreamingContext context): base((String)info.GetValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, typeof(String)))
        { }
        #endregion Constructor
        /// <summary>
        /// Method to serialize data. The method is called on serialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DDSchema.SERIALIZE_ATTRIBUTE_TYPE, this.Name, typeof(String));
        }
    }
}
