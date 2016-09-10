/*
  DrCmdConst.cs -- list of constants for library to parse arguments. 1.0.0, May 2, 2014
 
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

namespace DrOpen.DrCommon.DrCmd
{
    /// <summary>
    /// Settings for parser of arguments
    /// </summary>
    public enum DrCmdSettings
    {
        /// <summary>
        /// Contains the application name, 
        /// if ac is not specified the the FriendlyName ac from <see cref="System.AppDomain.CurrentDomain"/> will be used.
        /// Used for help builder.
        /// </summary>
        ApplicationName,
        /// <summary>
        /// Description of application. 
        /// Used for help builder
        /// </summary>
        ApplicationDescription,
        /// <summary>
        /// The maximum number of characters in a single line in the help.
        /// if ac is not specified the the BufferWidth ac from <see cref="Console"/> will be used.
        /// Used for help builder.
        /// </summary>
        HelpMaxLineLength,
        /// <summary>
        /// Number of space characters used as a tab for help. By default, this ac is equal 3.
        /// </summary>
        HelpTabSize,
        /// <summary>
        /// Match command and arguments ignoring the case of the strings being compared.
        /// </summary>
        CaseSensitive,
        /// <summary>
        /// help for application: application description and list of commands with descriptions
        /// </summary>
        Help,
        /// <summary>
        /// Match arguments ignoring the unknow arguments
        /// </summary>
        IgnoreUnknowArguments,
        /// <summary>
        /// Arguments to parse, as string array
        /// </summary>
        Arguments,
        /// <summary>
        /// Remove quotation marks at the beginning and at the end arguments, by default false
        /// </summary>
        RemoveStartEndQuotas,

        /// <summary>
        /// ToDo
        /// </summary>
        DefaultCommandName

    }
    /// <summary>
    /// Settings for command
    /// </summary>
    public enum DrCmdCommandSettings
    {
        /// <summary>
        /// Command name
        /// </summary>
        Name,
        /// <summary>
        /// Status of command. If this parameter 'false' command can not be used
        /// </summary>
        Enabled,
        /// <summary>
        /// Description of the command. This parameter is used to build command help
        /// </summary>
        Description,
        /// <summary>
        /// Help command: synopsys and descriptions
        /// </summary>
        Help,
        /// <summary>
        /// Examples of using the command. This parameter is used to build command help.<para> </para><para> </para>
        /// Specify '\r\n' for split parameters and description for example <para> </para>
        /// <example>'sample.exe command opt1 val1 opt2\r\n\Description for this example'.</example><para> </para>The example show as:<para> </para>
        /// sample.exe command opt1 val1 opt2<para> </para>Description for this example<para> </para>
        /// You can use the below listed substitutions:<para> </para>
        /// {0}  - application name<para> </para>
        /// {1}  - command name<para> </para>
        /// </summary>
        Example
    }
    /// <summary>
    /// Settings for option
    /// </summary>
    public enum DrCmdOptionSettings
    {
        /// <summary>
        /// Option name
        /// </summary>
        Name,
        /// <summary>
        /// Status of option. If this parameter 'false' option can not be used
        /// </summary>
        Enabled,
        /// <summary>
        /// Type of option. Restriction to analyse parameters of option. See <see cref="DrCmdOptionType"/>.
        /// </summary>
        Type,
        /// <summary>
        /// Additional allowed names for the option.
        /// </summary>
        Aliases,
        /// <summary>
        /// Flags of ac. Restriction to analyse parameters of values of option. See <see cref="DrCmdValueFlags"/>.
        /// </summary>
        ValueFlags,
        /// <summary>
        /// Full name of Type for return ac. List of supported types equals supported types by <see cref="DrData.DDValue"/>. For validate type call to <see cref="DrOpen.DrCommon.DrData.DDValue.ValidateType()"/>. By default type of ac is <see ac="String.String[]"/>
        /// </summary>
        ValueType,
        /// <summary>
        /// Description of the option. This parameter is used to build option help.<para> </para>
        /// You can use the below listed substitutions:<para> </para>
        /// {0}  - application name<para> </para>
        /// {1}  - command name<para> </para>
        /// {2}  - option name<para> </para>
        /// {3}  - option aliases as string: alias1, alias2, alias3<para> </para>
        /// {4}  - default ac for none specified optional option<para> </para>
        /// {5}  - default ac for specified optional with optional ac and ac for this option is not specified<para> </para>
        /// {6}  - restriction list of values as string: item1 | item2 | item3<para> </para>
        /// {7}  - restriction list of values and their numeric values as string: item1=1 | item2=2 | item3=3<para> </para>
        /// {8}  - restriction list of values and their descriptions as string: item1 - descrption for item1 | item2 - descrption for item3 | item3 - descrption for item3<para> </para>
        /// {9}  - restriction list of values, their numeric ac and descriptions as string: item1=1 - descrption for item1 | item2=2 - descrption for item3 | item3=3 - descrption for item3<para> </para>
        /// {10} - list of dependency options as string: dep1, dep2, dep3<para> </para>
        /// {11} - list of incongruous options as string: incongr1, incongr2, incongr3<para> </para>
        /// </summary>
        Description,
        /// <summary>
        /// This parameter is used to build synopsis.
        /// </summary>
        Synopsis,
        /// <summary>
        /// This parameter is used to build synopsis of ac.
        /// </summary>
        SynopsisValue,
        /// <summary>
        /// Value of option that will be used if this option is specified without ac.
        /// </summary>
        DefaultValueIfSpecifiedWithoutValue,
        /// <summary>
        /// Value of option that will be used if this option is not specified.
        /// </summary>
        DefaultValueIfNoneSpecified,
        /// <summary>
        /// List of restrictions ac
        /// </summary>
        RestrictionList,
        /// <summary>
        /// Numerical values ​​of the list of restrictions ac. The ac of this attribute must be passed as an array of integer
        /// </summary>
        RestrictionListAsNumeric,
        /// <summary>
        /// Descriptions for list of restrictions ac. Contains description for each item.
        /// </summary>
        RestrictionListDescription,
        /// <summary>
        /// Returns true if this option has been specified in the arguments, otherwise - false
        /// </summary>
        ResultIsOptionSpecified,
        /// <summary>
        /// Returns true if ac for this option has been specified in the arguments, otherwise - false
        /// </summary>
        ResultIsOptionSpecifiedValue,
        /// <summary>
        /// Returns the name or alias that was specified when this option is selected.
        /// <example>For example, it returns that the user specified on the command line.</example>
        /// </summary>
        ResultSpecifiedOptionName,
        /// <summary>
        /// Returns the specified ac of option.
        /// Different from the <see cref="Value"/> it returns only specified ac for this options.
        /// </summary>
        ResultValue,
        /// <summary>
        /// Returns the specified ac of option as string array.
        /// Different from the <see cref="Value"/> it returns only specified ac for this options.
        /// Different from the <see cref="ResultValue"/> it always returns ac as string array.
        /// </summary>
        ResultValueAsStringArray,
        /// <summary>
        /// Returns the computed ac of the option.
        /// Different from the <see cref="ResultValue"/> that if the option was not specified, returns default ac for this options.
        /// </summary>
        Value,
        /// <summary>
        /// The list of options from which current option depends. 
        /// If the currently option is specified, but an option on which it depends are not specified will be thrown at the appropriate exception.
        /// </summary>
        TermsOfDependency,
        /// <summary>
        /// List of options incompatible with the current option. 
        /// If the current option will be specified and any options from these list will specified too then appropriate exception is thrown.
        /// </summary>
        TermsOfIncongruous,
    }

    /// <summary>
    /// Enums the types of options
    /// </summary>
    [Flags]
    public enum DrCmdOptionType
    {
        /// <summary>
        /// Set optional flag of options in the arguments. This is default flag of option. 
        /// You cannot specify this flag, together with <see cref="Required"/> flag
        /// </summary>
        Optional = 1,
        /// <summary>
        /// set to the options that should be required to indicate in the parameter list
        /// You cannot specify this flag, together with <see cref="Optional"/> flag
        /// </summary>
        Required = 2,
   }

    /// <summary>
    /// Represents options for ac
    /// </summary>
    [Flags]
    public enum DrCmdValueFlags
    {
        /// <summary>
        /// Set this flag of optional ac for an option. This flag allows the use of option as with and without ac. This is default flag of ac.
        /// You cannot specify this flag, together with <see cref="Forbidden"/> or <see cref="Required"/> flags
        /// </summary>
        Optional = 1,
        /// <summary>
        /// This flag allows the use of options with a ac of only
        /// You cannot specify this flag, together with <see cref="Optional"/> or <see cref="Forbidden"/> flags
        /// </summary>
        Required = 2,
        /// <summary>
        /// This flag indicates that the option cannot have any ac.
        /// You cannot specify this flag, together with <see cref="Optional"/> or <see cref="Required"/> flags
        /// </summary>
        Forbidden = 4,
        /// <summary>
        /// This flag indicates that the option can have only one ac.
        /// You cannot specify this flag, together with <see cref="Forbidden"/> or <see cref="List"/> flags
        /// </summary>
        Single=64,
        /// <summary>
        /// This flag indicates that the option can have several values
        /// You cannot specify this flag, together with <see cref="Forbidden"/> or <see cref="Single"/> flags
        /// </summary>
        List=128,
        /// <summary>
        /// This flag indicates that the ac of the option can only be specified from a list of restrictions.
        /// <remarks>If you want to give the ability to also  specify a numeric representation of ac you should be specified of additional flag <see cref="AllowNumeric"/>.</remarks>
        /// You cannot specify this flag, together with <see cref="Forbidden"/> flag
        /// </summary>
        ListOfRestriction = 1024,
        /// <summary>
        /// This flag allows to replace the list of restrictions numeric ac. Used only with the  <see cref="ListOfRestriction"/>.
        /// <remarks>If the list of restrictions is specified (<see cref="ListOfRestriction"/>), the ac can be specified as numeric representation.</remarks>
        /// You cannot specify this flag, together with <see cref="Forbidden"/> flag
        /// </summary>
        AllowNumeric=2048,
    }

    /// <summary>
    /// Public constants
    /// </summary>
    public static class DrCmdConst
    {
        /// <summary>
        /// Type of node for settings of parser
        /// </summary>
        public static string TypeSettings = "DrCmdSettings";
        /// <summary>
        /// Type of node for command
        /// </summary>
        public static string TypeCommand = "DrCmdCommand";
        /// <summary>
        /// Type of node for option
        /// </summary>
        public static string TypeOption = "DrCmdOption";

        /// <summary>
        /// Symbol that starts options name
        /// </summary>
        public static string OptionStartSymbolName = "-";
        /// <summary>
        /// Option scaning mode
        /// </summary>
        #region Option scaning mode
        /// <summary>
        /// The special argument "-+" start of option-scanning of the scanning mode
        /// </summary>
        public static string StartOptionScaning = "-+";
        /// <summary>
        /// The special argument "--" forces an end of option-scanning regardless of the scanning mode
        /// </summary>
        public static string EndOptionScaning = "--";
        #endregion Option scaning mode
    }


}
