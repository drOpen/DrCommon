/*
  DrVarException.cs -- exceptions for 'DrVar' general purpose Builder variables. 1.1.0, February 09, 2019
 
  Copyright (c) 2013-2019 Kudryashov Andrey aka Dr
 
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
using System.Text;
using DrOpen.DrCommon.DrVar.Res;

namespace DrOpen.DrCommon.DrVar.Eception
{
    /// <summary>
    /// General DrVar exception. All DrVar exceptions inherited from this exception
    /// </summary>
    public class DrVarException : Exception
    {
        /// <summary>
        /// Creates the general DrVar exception with a message
        /// </summary>
        /// <param name="message">Unformated exception message</param>
        /// <param name="p">Optional insertion strings for exception message</param>
        public DrVarException(string message, params object[] p) : base(string.Format(message, p)) { }
        /// <summary>
        /// Creates the general DrVar exception with a message and an inner exception
        /// </summary>
        /// <param name="message">Unformated exception message</param>
        /// <param name="inner">inner exception</param>
        /// <param name="p">Optional insertion strings for exception message</param>
        public DrVarException(string message, Exception inner, params object[] p) : base(string.Format(message, p), inner) { }
    }

    /// <summary>
    /// Incorrect variable name exception 
    /// </summary>
    public class DrVarExceptionMissName : DrVarException
    {
        /// <summary>
        /// Incorrect variable name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Creates incorrect variable name exception
        /// </summary>
        /// <param name="name">incorrect variable name</param>
        public DrVarExceptionMissName(string name)
            : base(Res.Msg.MISS_VAR_NAME, name)
        {
            this.Name = name;
        }

    }

    /// <summary>
    /// The end of variable sign is missed 
    /// </summary>
    public class DrVarExceptionMissVarEnd: DrVarException
    {
        /// <summary>
        /// Incorrect value
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// missed sign symbol
        /// </summary>
        public string MissSign { get; private set; }
        /// <summary>
        /// Creates incorrect variable name exception
        /// </summary>
        /// <param name="name">incorrect variable name</param>
        /// <param name="missSign">missed sign</param>
        public DrVarExceptionMissVarEnd(string value, string missSign)
            : base(Msg.CANNOT_BUILD_VAR_NOT_CLOSED_SYMBOL, value, missSign)
        {
            this.Value = value;
            this.MissSign = missSign;
        }

    }
    /// <summary>
    /// The variable has reference to itself
    /// </summary>
    public class DrVarExceptionLoop : DrVarException
    {
        /// <summary>
        /// Variable name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Variable value
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// Creates loop variable exception
        /// </summary>
        /// <param name="name">incorrect variable name</param>
        /// <param name="value">incorrect var value</param>
        public DrVarExceptionLoop(string name, string value)
            : base(Msg.LOOP_VAR, name, value)
        {
            this.Value = value;
            this.Name = name;
        }
    }
    /// <summary>
    /// The variable was not resolved
    /// </summary>
    public class DrVarExceptionResolve: DrVarException
    {
        /// <summary>
        /// Variable name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Text contains unresolved var
        /// </summary>
        public string  Text { get; private set; }
        /// <summary>
        /// Creates the unresolved variable exception
        /// </summary>
        /// <param name="name">unresolved variable name</param>
        /// <param name="text">text contains unresolved var</param>
        public DrVarExceptionResolve(string name, string text)
            : base(Msg.UNRESOLED_VAR, name, text)
        {
            this.Text = text;
            this.Name = name;
        }
    }
}
