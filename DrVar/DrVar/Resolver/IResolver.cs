using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrOpen.DrCommon.DrVar.Resolver
{


    public enum RESOLVE_AMBIGUITY_OPTION
    {
        RES_UNRESOLVED_EXCEPTION = 1,
        RES_UNRESOLVED_KEEP_TEXT,
        RES_UNRESOLVED_PUT_EMPTY
    }



    internal interface IResolved
    {
        bool IsResolved{get;}
    }
    internal interface IResolver
    {
        void Resolve();
    } 

    internal interface IVarManager
    {
        /// <summary>
        /// Gets the value of specified variable name.
        /// </summary>
        /// <param name="varName">variable name</param>
        /// <returns></returns>
        string GetValue(string varName);

    }
}
