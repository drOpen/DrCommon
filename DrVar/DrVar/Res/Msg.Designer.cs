﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrOpen.DrCommon.DrVar.Res {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Msg {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Msg() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DrOpen.DrCommon.DrVar.Res.Msg", typeof(Msg).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t build list of variables by the string &apos;{0}&apos; because there is not closed &apos;{1}&apos; symbol of the substitution..
        /// </summary>
        internal static string CANNOT_BUILD_VAR_NOT_CLOSED_SYMBOL {
            get {
                return ResourceManager.GetString("CANNOT_BUILD_VAR_NOT_CLOSED_SYMBOL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{1}&apos; of variable name &apos;{0}&apos; has reference to itself..
        /// </summary>
        internal static string LOOP_VAR {
            get {
                return ResourceManager.GetString("LOOP_VAR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The variable name &apos;{0}&apos; contains the variable. Dynamic variable name is restricted..
        /// </summary>
        internal static string MISS_VAR_NAME {
            get {
                return ResourceManager.GetString("MISS_VAR_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot resolve variable &apos;{0}&apos;..
        /// </summary>
        internal static string UNRESOLED_VAR {
            get {
                return ResourceManager.GetString("UNRESOLED_VAR", resourceCulture);
            }
        }
    }
}
