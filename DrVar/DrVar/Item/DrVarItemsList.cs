/*
  DrVarItemsList.cs -- list of variables for 'DrVar' general purpose Builder variables 1.1.0, February 09, 2019
  
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

      Kudryashov Andrey <kudryashov dot andrey at gmail  dot com>

 */

using System;
using System.Collections;
using System.Collections.Generic;
using DrOpen.DrCommon.DrVar.Eception;

namespace DrOpen.DrCommon.DrVar.Item
{
    /// <summary>
    /// list of variables
    /// </summary>
    internal class DrVarItemsList : IEnumerable<DrVarItem>
    {
        internal DrVarItemsList()
        {
            varItems = new List<DrVarItem>();
        }
        /// <summary>
        /// Initializes a new instance of the list of variables
        /// </summary>
        internal DrVarItemsList(string value) : this ()
        {
            Parse(value);
        }
        /// <summary>
        /// list of variables
        /// </summary>
        private List<DrVarItem> varItems;

        #region GetVarSymbol
        public string VarSymbol
        {
            get { return DrVar.DrVarSign.varSign.ToString(); }
        }

        public string EscapeVarSymbol
        {
            get { return DrVarSign.escapeVarSign; }
        }

        public bool AreThereVars
        {
            get { return varItems.Count != 0; }
        }

        #endregion GetVarSymbol


        /// <summary>
        /// Stored the number of characters beginning variables
        /// </summary>
        public int OpenedVarCounter { get; private set; }
        /// <summary>
        /// Stored the number of characters closure variables
        /// </summary>
        public int ClosedVarCounter { get; private set; }
        /// <summary>
        /// Stored the number of characters the escape of variables
        /// </summary>
        public int EscapeVarSymbolCounter { get; private set; }
        /// <summary>
        /// Parses the string for the presence of variables and makes list of variables and returns quantity of variables
        /// </summary>
        /// <param name="value">string to parse</param>
        /// <returns>Returns quantity of variables</returns>
        internal int Parse(string value)
        {

            if (value.Contains(DrVarSign.varSign.ToString())) //Exit if the string does not contain a variable indicating symbol.
            {

                int iCurrentPosition = 0;
                
                int opennedVarFlag = 0; // Stored number of not closed characters of variables
                bool isPreviosVarEscaped = false;
                bool isPreviosCharVar = false; // Stored status about previous character of variable
                bool isVarNameStarted = false; // Keeps the status of that now analyzes the variable name

                string varName = String.Empty;

                foreach (char ch in value) // enumerating all characters in the string
                {
                    iCurrentPosition++; // current position
                    if (ch == DrVarSign.varSign)
                    {
                        if (isVarNameStarted)
                        {
                            varItems.Add(new DrVarItem(iCurrentPosition - varName.Length - 2, iCurrentPosition, varName, DrVarSign.varSign + varName + DrVarSign.varSign)); //add new variable item to list
                            varName = String.Empty; // clear name of variable
                            ClosedVarCounter++; // Increase the number of characters closure variables
                        }
                        else
                        {
                            if ((isPreviosCharVar) && (!isPreviosVarEscaped))
                            {
                                EscapeVarSymbolCounter++; // Increase the number of characters escape variables
                                isPreviosVarEscaped = true;
                            }
                            else
                                isPreviosVarEscaped = false;
                            OpenedVarCounter++; // Increase the number of characters beginning variables 
                            opennedVarFlag++; // Increase the number of not closed characters of variables
                            isPreviosCharVar = true; // Follow up about previous character of variable
                        }
                        isVarNameStarted = false; // Clears the status of that now analyzes the variable name
                    }
                    else
                    {
                        // Sets the status of that now analyzes the variable name
                        if ((isPreviosCharVar) && (opennedVarFlag % 2 != 0)) isVarNameStarted = true;  // if the previous character is starting variable and if the number was not even
                        if (isVarNameStarted) varName += ch.ToString(); // build name of variable
                        opennedVarFlag = 0;
                        isPreviosCharVar = false; // Clears status about previous character of variable
                    }
                }
                if (OpenedVarCounter != (ClosedVarCounter + EscapeVarSymbolCounter * 2)) throw new DrVarExceptionMissVarEnd(value, DrVarSign.varSign.ToString());
            }
            return varItems.Count;
        }


        public static DrVarItemsList GetItemsList(string value) {
            return new DrVarItemsList(value);
        }

        #region Enumerator
        /// <summary>
        /// Returns an enumerator that iterates through the DrVarItemsList (IEnumerator&lt;DrVarItem&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;DrVarItem&gt;</returns>
        public IEnumerator<DrVarItem> GetEnumerator()
        {
            foreach (var item in varItems)
            {
                yield return item;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the DrVarItemsList (IEnumerator&lt;DrVarItem&gt;).
        /// </summary>
        /// <returns>IEnumerator&lt;DrVarItem&gt;</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion Enumerator
    }

}
