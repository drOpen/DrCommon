/*
  DrExtEnum.cs -- Extends functionality of standard types - Enum.  1.0.0, May 9, 2014
 
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
using DrExt.Res;


namespace DrOpen.DrCommon.DrExt
{
    public static class DrExtEnum
    {
        /// <summary>
        /// Verify flags and Enum types. 
        /// If verification flags type has another type than enum <exception cref="ApplicationException">ApplicationException will be threw</exception>.
        /// </summary>
        /// <param name="enumType">type of Enum</param>
        /// <param name="flagsType">type of flags</param>
        private static void VerifyEnumAndFlagsType(Type enumType, Type flagsType)
        {
            if (enumType != flagsType) throw new ApplicationException(String.Format(Msg.FLAG_TYPE_NOT_EQUALS_ENUM_TYPE, flagsType.ToString(), enumType.ToString()));
        }
        /// <summary>
        /// Checks whether the value contains of the specified flag (bit).
        /// Returns true if bit is contained? otherwise - false
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <param name="flags">seeking flag</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum value, Enum flags)
        {
            VerifyEnumAndFlagsType(value.GetType(), flags.GetType());
            var ulongFlags = Convert.ToUInt64(flags);
            return ((Convert.ToUInt64(value) & ulongFlags) == ulongFlags);
        }
        /// <summary>
        /// Add flag (bit) to specified value
        /// </summary>
        /// <typeparam name="T">type of enum</typeparam>
        /// <param name="value">value which will be added a bit</param>
        /// <param name="flags">flag (bit) which will be added</param>
        /// <returns></returns>
        public static T SetFlag<T>(this Enum value, T flags)
        {
            VerifyEnumAndFlagsType(value.GetType(), typeof(T));
            return (T)Enum.ToObject(typeof(T), Convert.ToUInt64(value) | Convert.ToUInt64(flags));
        }

        /// <summary>
        /// returns an integer array of enumeration values 
        /// </summary>
        /// <param name="value">enum for enumeration</param>
        /// <returns></returns>
        public static int[] GetFlags(this Enum value)
        {
            var values = Enum.GetValues(value.GetType());
            var result = new int[values.Length];
            int i = 0;
            foreach (var item in values)
            {
                result[i] = (int)item;
                i++;
            }
            return result;
        }

        /// <summary>
        /// Return string value from 'StringValueAttribute' for enum
        /// </summary>
        /// <param name="value">enum</param>
        /// <returns></returns>
        /// <remarks>Extends the standard set of functions for an enums value</remarks>
        public static string GetValueAsString(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = (StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);
            return attribs.Length > 0 ? attribs[0].StringValue : String.Empty;
        }
    }
}
