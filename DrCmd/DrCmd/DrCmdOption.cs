/*
  DrCmdOption.cs -- object of options. 1.0.0, May 10, 2014
 
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
using System.Collections.Generic;
using DrCmd.Res;
using DrOpen.DrCommon.DrCmd;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrData.Exceptions;
using DrOpen.DrCommon.DrExt;

namespace DrOpen.DrCommon.DrCmd
{
    /// <summary>
    /// class of options
    /// </summary>
    internal class DrCmdOption
    {
        /// <summary>
        /// Create object of option by specified node with settings of option
        /// </summary>
        /// <param name="command">reference to parent command</param>
        /// <param name="option">node with settings of option</param>
        internal DrCmdOption(DrCmdCommand command, DDNode option)
        {
            option.Type.ValidateExpectedNodeType(DrCmdConst.TypeOption);
            Option = option;
            Command = command;
            name = GetOptionName();
            type = GetOptionType();
            valueType = GetValueType();
        }
        /// <summary>
        /// reference to parent command
        /// </summary>
        internal DrCmdCommand Command { get; private set; }
        /// <summary>
        /// node with settings of this option
        /// </summary>
        public DDNode Option { get; private set; }
        /// <summary>
        /// Parent command name
        /// </summary>
        public string CommandName
        {
            get { return Command.Name; }
        }
        /// <summary>
        /// Option name
        /// </summary>
        private readonly string name;
        /// <summary>
        /// Option name
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// Option type
        /// </summary>
        private readonly DrCmdOptionType type;
        /// <summary>
        /// Option type
        /// </summary>
        public DrCmdOptionType Type
        {
            get { return type; }
        }
        /// <summary>
        /// Value type
        /// </summary>
        private readonly DrCmdValueType valueType;
        /// <summary>
        /// Value type
        /// </summary>
        public DrCmdValueType ValueType
        {
            get { return valueType; }
        }
        /// <summary>
        /// Retruns parametr value of this option from <see cref="DrCmdOptionSettings.Value"/>
        /// </summary>
        public DDValue Value
        {
            get { return GetAttributeValue(DrCmdOptionSettings.Value, string.Empty); }
        }

        /// <summary>
        /// Gets the value from attribute collection associated with the specified name. When this method returns, 
        /// contains the value associated with the specified name, if the name is found; 
        /// otherwise, the default value for the type of the value parameter.
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="defaultValue">the default value for the type of the value parameter.</param>
        /// <returns>When this method returns, contains the value associated with the specified name, if the nameis found; 
        /// otherwise, the default value for the type of the value parameter.</returns>
        public DDValue GetAttributeValue(object name, object defaultValue)
        {
            return Option.Attributes.GetValue(name.ToString(), defaultValue);
        }

        #region Default
        /// <summary>
        /// Returns default option type <see cref="DrCmdOptionType.Optional"/>
        /// </summary>
        /// <returns></returns>
        public static DrCmdOptionType GetDefaultOptionType()
        {
            return DrCmdOptionType.Optional;
        }
        /// <summary>
        /// Returns default option value type <see cref="DrCmdValueType.Optional"/>
        /// </summary>
        /// <returns></returns>
        public static DrCmdValueType GetDefaultValueType()
        {
            return DrCmdValueType.Optional;
        }

        #endregion Default
        /// <summary>
        /// Returns aliases as string arrray from attribute <see cref="DrCmdOptionSettings.Aliases"/>
        /// </summary>
        /// <returns></returns>
        public string[] GetAliases()
        {
            return GetAttributeValue(DrCmdOptionSettings.Aliases, string.Empty).GetValueAsStringArray();
        }
        /// <summary>
        /// Returns true if this option has been specified in the arguments, otherwise - false from attribute <see cref="DrCmdOptionSettings.ResultIsOptionSpecified"/>
        /// </summary>
        /// <returns></returns>
        public bool GetResultIsOptionSpecified()
        {
            return GetAttributeValue(DrCmdOptionSettings.ResultIsOptionSpecified, false);
        }
        /// <summary>
        /// Return true if option is enables, otherwise - false.
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return GetAttributeValue(DrCmdOptionSettings.Enabled, true);
        }
        #region restrictionList
        /// <summary>
        /// Returns restriction list as single string separetedby by specified string for option from attribute <see cref="DrCmdOptionSettings.RestrictionList"/>
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetRestrictionListAsString(string separator)
        {
            return GetAttributeValue(DrCmdOptionSettings.RestrictionList, string.Empty).ToString().Replace("\0", separator);
        }
        /// <summary>
        /// Returns restriction list and numeric value as single string separated by specified separator
        /// If the Length of the lists do not match a <exception cref="FormatException">FormatException</exception>will be thrown.
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetRestrictionListWithValueAsString(string separator)
        {
            var restrictionList = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();
            var restrictionListAsNumeric = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { }).GetValueAsIntArray();
            if (restrictionList.Length != restrictionListAsNumeric.Length) return (Msg.CANNOT_BUILD_RESTRICTION_LIST_AND_NUMERIC_VALUE); // Consistency settings is checked when an instance of the class is created. Here is "control shot"
            string text = string.Empty;
            for (int i = 0; i < restrictionList.Length; i++)
            {
                if (text.Length > 0) text += separator ;
                text += restrictionList[i] + "=" + restrictionListAsNumeric[i].ToString();
            }
            return text;
        }
        /// <summary>
        /// Returns restriction list and description value as single string separated by specified separator
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetRestrictionListWithDescriptionAsString(string separator)
        {
            var restrictionList = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();
            var restrictionListDescription = GetAttributeValue(DrCmdOptionSettings.RestrictionListDescription, new string[] { }).GetValueAsStringArray();
            if (restrictionList.Length != restrictionListDescription.Length) return (Msg.CANNOT_BUILD_RESTRICTION_LIST_AND_DESCRIPTION); 
            string text = string.Empty;
            for (int i = 0; i < restrictionList.Length; i++)
            {
                if (text.Length > 0) text += separator ;
                text += restrictionList[i] + " - " + restrictionListDescription[i].ToString();
            }
            return text;
        }
        /// <summary>
        /// Returns restriction list with value  and description as single string separated by specified separator
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetRestrictionListWithValueAndDescriptionAsString(string separator)
        {
            var restrictionList = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();
            var restrictionListDescription = GetAttributeValue(DrCmdOptionSettings.RestrictionListDescription, new string[] { }).GetValueAsStringArray();
            var restrictionListAsNumeric = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { }).GetValueAsIntArray();
            if ((restrictionList.Length != restrictionListDescription.Length) || (restrictionList.Length != restrictionListAsNumeric.Length)) return (Msg.CANNOT_BUILD_RESTRICTION_LIST_AND_NUMERIC_WITH_DESCRIPTION); 
            string text = string.Empty;
            for (int i = 0; i < restrictionList.Length; i++)
            {
                if (text.Length > 0) text += separator;
                text += restrictionList[i] + "=" + restrictionListAsNumeric[i].ToString();
                text +=  " - " + restrictionListDescription[i].ToString();
            }
            return text;
        }
        #endregion restrictionList
        #region GetTerm
        /// <summary>
        /// Returns term of dependancy list as single string separetedby by specified string for option from attribute <see cref="DrCmdOptionSettings.TermsOfDependency"/>
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetTermsOfDependencyAsString(string separator)
        {
            return GetAttributeValue(DrCmdOptionSettings.TermsOfDependency, string.Empty).ToString().Replace("\0", separator);
        }
        /// <summary>
        /// Returns term of Incongruous list as single string separetedby by specified string for option from attribute <see cref="DrCmdOptionSettings.TermsOfIncongruous"/>
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetTermsOfIncongruousAsString(string separator)
        {
            return GetAttributeValue(DrCmdOptionSettings.TermsOfIncongruous, string.Empty).ToString().Replace("\0", separator);
        }
        #endregion GetTerm
        /// <summary>
        /// Returns aliaseslist as single string separetedby by specified string for option from attribute <see cref="DrCmdOptionSettings.Aliases"/>
        /// <param name="separator">item separator</param>
        /// </summary>
        private string GetAliasesAsString(string separator)
        {
            return GetAttributeValue(DrCmdOptionSettings.Aliases, string.Empty).ToString().Replace("\0", separator);
        }
        /// <summary>
        /// Returns name and aliaseslist as single string separetedby by specified string for option from attributes <see cref="DrCmdOptionSettings.Name"/> and <see cref="DrCmdOptionSettings.Aliases"/>
        /// <param name="separator">item separator</param>
        /// </summary>
        internal string GetNameAndAliasesAsString(string separator)
        {
            var al = GetAliasesAsString(separator);
            if (al.Length > 0) return Name + separator + al;
            return Name; // if aliases is not specified
        }
        #region Build Type
        /// <summary>
        /// Build options type from string array. Ignore case.
        /// </summary>
        /// <returns></returns>
        public DrCmdOptionType GetOptionType()
        {
            return GetOptionType(true);
        }
        /// <summary>
        /// Build options type from string array from attribute <see cref="DrCmdOptionSettings.Type"/>.
        /// If cannot convert string representation to the OptionType <exception cref="FormatException">FormatException</exception> will be thrown.
        /// </summary>
        /// <param name="ignoreCase">if true ignore case, otherwise regard case</param>
        /// <returns></returns>
        public DrCmdOptionType GetOptionType(bool ignoreCase)
        {

            var typeOption = GetAttributeValue(DrCmdOptionSettings.Type, GetDefaultOptionType().ToString()).GetValueAsStringArray();
            var type = typeof(DrCmdOptionType);
            DrCmdOptionType typeOptionAsEnum = 0;
            foreach (var item in typeOption)
            {
                try
                {
                    typeOptionAsEnum |= (DrCmdOptionType)Enum.Parse(type, item, ignoreCase);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format(Msg.CANNOT_PARSE_VALUE_AS_TYPE_OF_OPTION, item), e);
                }
            }
            return typeOptionAsEnum;
        }
        /// <summary>
        /// Build values type from string array. Ignore case
        /// </summary>
        /// <returns></returns>
        public DrCmdValueType GetValueType()
        {
            return GetValueType(true);
        }

        /// <summary>
        /// Build values type from string array from attribute <see cref="DrCmdOptionSettings.ValueType"/>
        /// If cannot convert string representation to the ValueType <exception cref="FormatException">FormatException</exception> will be threw.
        /// </summary>
        /// <param name="ignoreCase">if true ignore case, otherwise regard case</param>
        /// <returns></returns>
        public DrCmdValueType GetValueType(bool ignoreCase)
        {

            var typeValue = GetAttributeValue(DrCmdOptionSettings.ValueType, GetDefaultValueType().ToString()).GetValueAsStringArray();
            var type = typeof(DrCmdValueType);
            DrCmdValueType typeValueAsEnum = 0;
            foreach (var item in typeValue)
            {
                try
                {
                    typeValueAsEnum |= (DrCmdValueType)Enum.Parse(type, item, ignoreCase);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format(Msg.CANNOT_PARSE_VALUE_AS_TYPE_OF_VALUE, item), e);
                }
            }
            return typeValueAsEnum;
        }
        #endregion Build Type

        /// <summary>
        /// Return option name from attribute <see cref="DrCmdOptionSettings.Name"/>.
        /// Return empty string if name isn't specified
        /// </summary>
        /// <returns>return commnd name</returns>
        private string GetOptionName()
        {
            return GetAttributeValue(DrCmdOptionSettings.Name, string.Empty);
        }
        /// <summary>
        /// Return option aliases  from attribute <see cref="DrCmdOptionSettings.Aliases"/>.
        /// Return empty string if name isn't specified
        /// </summary>
        /// <returns>return commnd name</returns>
        public string[] GetOptionAliases()
        {
            return GetAttributeValue(DrCmdOptionSettings.Aliases, new string[] { });
        }

        /// <summary>
        /// Returns string array from all values in collections of attributes
        /// </summary>
        /// <returns></returns>
        private static string[] GetOptionValueAsStringArray(DDNode value)
        {
            var result = new string[value.Attributes.Values.Count];
            int i = 0;
            foreach (var item in value.Attributes.Values)
            {
                result[i] = item;
                i++;
            }
            return result;
        }

        /// <summary>
        /// Set option result fields
        /// </summary>
        /// <param name="item">KeyValuePair contains parameters for this option</param>
        public void SetOptionResults(KeyValuePair<string, DDNode> item)
        {
            Option.Attributes.Add(DrCmdOptionSettings.ResultIsOptionSpecified, true, ResolveConflict.OVERWRITE);                                        // option is specified
            Option.Attributes.Add(DrCmdOptionSettings.ResultSpecifiedOptionName, item.Key, ResolveConflict.OVERWRITE);                                  // specified option name
            var optValues = GetOptionValueAsStringArray(item.Value);
            Option.Attributes.Add(DrCmdOptionSettings.ResultValue, optValues, ResolveConflict.OVERWRITE);                                               // specified option value as string array
            Option.Attributes.Add(DrCmdOptionSettings.ResultIsOptionSpecifiedValue, optValues.Length != 0, ResolveConflict.OVERWRITE);                  // specified option value as string array            
        }

        /// <summary>
        /// Removes attributes  filled after parsing parameters. 
        /// Cleaning must be done before the parsing of the parameters.
        /// </summary>
        public void Clear()
        {
            Option.Attributes.Remove(DrCmdOptionSettings.ResultIsOptionSpecified);
            Option.Attributes.Remove(DrCmdOptionSettings.ResultSpecifiedOptionName);
            Option.Attributes.Remove(DrCmdOptionSettings.ResultIsOptionSpecifiedValue);
            Option.Attributes.Remove(DrCmdOptionSettings.ResultValue);
            Option.Attributes.Remove(DrCmdOptionSettings.Value);
  
        }

        /// <summary>
        /// Verify the following restrictions: <see cref="VerifyRestrictionListAsNumericType"/>, <see cref="VerifyOptionRestrictionsType"/>, <see cref="VerifyValueRestrictionsType"/>
        /// </summary>
        internal void VerifyOptionSettings()
        {
            VerifyRestrictionListAsNumericType();
            VerifyOptionRestrictionsType();
            VerifyValueRestrictionsType();
        }
        /// <summary>
        /// Checks type of attributes <see cref="DrCmdOptionSettings.RestrictionListAsNumeric"/>. 
        /// </summary>
        public void VerifyRestrictionListAsNumericType()
        {
            var value = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { });
            if (value.Type != typeof(int[])) throw new FormatException(string.Format(Msg.INCORRECT_TYPE_OF_RESTRICTION_LIST_AS_NUMERIC, value.Type.Name));
        }

        /// <summary>
        /// Verify type of option. 
        /// In the case of the incongruous flags are detected ​​the <exception cref="FormatException">FormatException</exception>will be thrown
        /// </summary>
        public void VerifyOptionRestrictionsType()
        {
            if ((Type.HasFlag(DrCmdOptionType.Optional)) & (Type.HasFlag(DrCmdOptionType.Required)))
            {
                throw new FormatException(string.Format(Msg.OPTION_TYPE_IS_WRONG_BOTH_TYPES, Name, Type.ToString(), DrCmdOptionType.Optional, DrCmdOptionType.Required));
            }
        }
        /// <summary>
        /// Verify type of value for option. 
        /// In the case of the incongruous flags are detected ​​the <exception cref="FormatException">FormatException</exception>will be thrown
        /// </summary>
        public void VerifyValueRestrictionsType()
        {
            try
            {
                #region Forbidden
                if ((ValueType.HasFlag(DrCmdValueType.Forbidden)) && (ValueType != DrCmdValueType.Forbidden))
                    throw new FormatException(string.Format(Msg.WITH_THIS_TYPE_CANNOT_BE_SPECIFIED_MORE_THAN_OTHER_TYPES, DrCmdValueType.Forbidden));
                #endregion Forbidden
                if ((ValueType.HasFlag(DrCmdValueType.AllowNumeric)) && (!ValueType.HasFlag(DrCmdValueType.ListOfRestriction)))
                    throw new FormatException(string.Format(Msg.CANNOT_SPECIFY_ONE_TYPE_WITHOUT_SECOND_TYPE, DrCmdValueType.AllowNumeric, DrCmdValueType.ListOfRestriction)); // AllowNumeric without ListOfRestriction
                var restrictionList = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();
                var restrictionListDescription = GetAttributeValue(DrCmdOptionSettings.RestrictionListDescription, new string[] { }).GetValueAsStringArray();
                if (ValueType.HasFlag(DrCmdValueType.AllowNumeric)) // if AllowNumeric is true need to check both list element count -> RestrictionList and RestrictionListAsNumeric
                {
                    var restrictionListAsNumeric = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { }).GetValueAsIntArray();
                    if (restrictionList.Length != restrictionListAsNumeric.Length)
                        throw new FormatException(string.Format(Msg.ELEMENT_COUNT_IN_RESTRICTION_LIST_IS_NOT_EQUALS_OF_NUMERIC_LIST_COUNT, Name, restrictionList.Length.ToString(), restrictionListAsNumeric.Length.ToString()));
                }
                if ((restrictionList.Length>0) && (restrictionListDescription.Length>0) && (restrictionList.Length!=restrictionListDescription.Length))
                    throw new FormatException(string.Format(Msg.ELEMENT_COUNT_IN_RESTRICTION_LIST_IS_NOT_EQUALS_OF_RESTRICTIOM_DESCRIPTION_LIST, Name, restrictionList.Length.ToString(), restrictionListDescription.Length.ToString()));
                CheckIncongruousTypeOfValue(ValueType, DrCmdValueType.Optional, DrCmdValueType.Required);                                                             // Optioanl & Required
                CheckIncongruousTypeOfValue(ValueType, DrCmdValueType.Single, DrCmdValueType.List);                                                                   // Single & List
            }
            catch (ApplicationException e)
            {
                throw new FormatException(string.Format(Msg.OPTION_TYPE_IS_WRONG, Name, ValueType.ToString()), e);
            }
        }
        /// <summary>
        /// Checks the current value of the two incongruous flags simultaneously. 
        /// If the two flags are detected ​​at the same time the <exception cref="FormatException">FormatException</exception> will be thrown.
        /// </summary>
        /// <param name="current">Current type of value</param>
        /// <param name="first">the first incongruous flag to detect</param>
        /// <param name="second">the second  incongruous flag to detect</param>
        private static void CheckIncongruousTypeOfValue(DrCmdValueType current, DrCmdValueType first, DrCmdValueType second)
        {
            if (((current & first) == first) & ((current & second) == second))
            {
                throw new FormatException(string.Format(Msg.OPTION_TYPE_HAS_INCONGRUOUS_TYPES, first.ToString(), second.ToString()));
            }
        }

        /// <summary>
        /// Checking this option specified arguments. 
        /// If arguments are incongruous the <exception cref="ArgumentException">ArgumentException</exception> will be thrown.
        /// </summary>
        public void ValidateOptionParameter()
        {
            var isSpecifiedOption = GetAttributeValue(DrCmdOptionSettings.ResultIsOptionSpecified, false);
            if ((Type.HasFlag(DrCmdOptionType.Required)) & (isSpecifiedOption == false)) throw new ArgumentException(string.Format(Msg.REQUIRED_OPTION_IS_NOT_SPECIFIED, Name, CommandName));
            // value type
            var valueAsStringArray = GetAttributeValue(DrCmdOptionSettings.ResultValue, new string[] { }).GetValueAsStringArray();
            var valueAsString = GetAttributeValue(DrCmdOptionSettings.ResultValue, string.Empty).GetValueAsString();
            // Single
            if ((ValueType.HasFlag(DrCmdValueType.Single)) & (valueAsStringArray.Length > 1)) throw new ArgumentException(string.Format(Msg.OPTION_CANNOT_CONTAINS_MORE_THAN_ONE_VALUE, Name, valueAsString, valueAsStringArray.Length, DrCmdValueType.Single, CommandName));
            // Forbidden
            if ((ValueType.HasFlag(DrCmdValueType.Forbidden)) & (valueAsStringArray.Length > 0)) throw new ArgumentException(string.Format(Msg.OPTION_CANNOT_CONTAIN_VALUE, Name, valueAsString, DrCmdValueType.Forbidden, CommandName));
            // Required
            if ((isSpecifiedOption) && (ValueType.HasFlag(DrCmdValueType.Required)) & (valueAsStringArray.Length == 0)) throw new ArgumentException(string.Format(Msg.OPTION_MUST_CONTAIN_VALUE, Name, DrCmdValueType.Required, CommandName));
            if ((ValueType.HasFlag(DrCmdValueType.ListOfRestriction)) && (valueAsStringArray.Length > 0)) ValidateOptionValueByRestrictionList(valueAsStringArray); // RestrictionList and AllowNumeric

        }

        /// <summary>
        /// Checks the value of the options on their compliance with the list of allowed values ​​or, if enabled, their numerical conform
        /// If the option value is incorrect by restriction list from attribute <see cref="DrCmdOptionSettings.RestrictionList"/> the <exception cref="ArgumentException">ArgumentException</exception> will be thrown.
        /// </summary>
        /// <param name="valueAsStringArray">option values as string array</param>
        private void ValidateOptionValueByRestrictionList(IEnumerable<string> valueAsStringArray)
        {
            var restrictionListArray = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();

            var ignoreCase = !Command.Settings.GetSettingsCaseSensitive();
            foreach (var val in valueAsStringArray)
            {
                if ((ValueType.HasFlag(DrCmdValueType.AllowNumeric)) && (val.IsPositiveNumber())) continue;
                var isMatched = false;
                foreach (var restrictionItem in restrictionListArray)
                {
                    if (string.Compare(restrictionItem, val, ignoreCase) == 0)
                    {
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                {
                    var text = string.Format(Msg.OPTION_HAS_INCORRECT_VALUE, Name, val, CommandName) + " ";
                    if (ValueType.HasFlag(DrCmdValueType.AllowNumeric))
                        text += string.Format(Msg.OPTION_SUPPORT_ONLY_FOLLOWING_VALUES_OR_NUMERIC, GetRestrictionListWithValueAsString(", "));
                    else
                        text += string.Format(Msg.OPTION_SUPPORT_ONLY_FOLLOWING_VALUES, GetRestrictionListAsString(", "));
                    throw new ArgumentException(text);
                }
            }
        }
        /// <summary>
        /// Checks enabled options in the command reference values from specified term attribute
        /// If option has reference to non-existen option the <exception cref="FormatException">FormatException</exception> will be thrown.
        /// </summary>
        /// <param name="hashSetNames"></param>
        /// <param name="termOfName"></param>
        public void VerifyLinksFromTermToOption(HashSet<string> hashSetNames, DrCmdOptionSettings termOfName)
        {
            var termsOfArray = GetAttributeValue(termOfName, string.Empty).GetValueAsStringArray();
            foreach (var item in termsOfArray)
            {
                if (!hashSetNames.Contains(item)) throw new FormatException(string.Format(Msg.REFERENCE_TO_NON_EXISTEN_OPTION, Command.Name, Name, termOfName, item));
            }
        }
        /// <summary>
        /// Save value ​​in the attribute Value based on the parameters passed, and / or using default values
        /// </summary>
        public void ApplyDefaultValue()
        {
            var value = new DDValue();
            if (GetResultIsOptionSpecified())
            {
                if (GetAttributeValue(DrCmdOptionSettings.ResultIsOptionSpecifiedValue, false))
                {
                    value = GetAttributeValue(DrCmdOptionSettings.ResultValue, string.Empty);                                  // if option value is specified
                }
                else
                {
                    value = GetAttributeValue(DrCmdOptionSettings.DefaultValueIfSpecifiedWithoutValue, string.Empty);          // if option is specified without value
                }
            }
            else
            {
                value = GetAttributeValue(DrCmdOptionSettings.DefaultValueIfNoneSpecified, string.Empty);                       // if option is not specified
            }
            Option.Attributes.Add(DrCmdOptionSettings.Value, value);
        }

        #region Help
        /// <summary>
        /// Returns the synopsis for the option and her valuedepending on the type of option and value type
        /// If value for this option can be optional, her value type is not <see cref="DrCmdValueType.Required"/> the function will be return option synopsis in square brackets <para> </para>
        /// How synopsis for the value will look of the function, see <see cref="GetOptionValueSynopsis"/><para> </para>
        /// <example>Example: OptName, [OptName], OptName [OptValue], [OptName [OptValue]]</example>
        /// </summary>
        /// <returns></returns>
        internal string GetOptionSynopsis()
        {
            var text = DrCmdConst.OptionStartSymbolName + Name;
            var valueSynopsis = GetOptionValueSynopsis();
            if (valueSynopsis.Length > 0) text += " " + valueSynopsis;

            if ( ! Type.HasFlag(DrCmdOptionType.Required))
            {
                text = "[" + text + "]"; // this option is optional 
            }
            return text;
        }

        /// <summary>
        /// Returns the synopsis for the value of this option depending on the type of value. <para> </para>
        /// If value for this option can be optional, her value type is not <see cref="DrCmdValueType.Required"/> the function will be return value synopsis in square brackets <para> </para>
        /// If this option cannot contains value, her value type is <see cref="DrCmdValueType.Forbidden"/> the function will be return empty string 
        /// </summary>
        /// <returns></returns>
        internal string GetOptionValueSynopsis()
        {
            var text = string.Empty;
            if (!ValueType.HasFlag(DrCmdValueType.Forbidden)) // this option can have value
            {
                text = GetAttributeValue(DrCmdOptionSettings.SynopsisValue,string.Empty);
                if (!ValueType.HasFlag(DrCmdValueType.Required)) text = "[" + text + "]"; // this value is optional 
            }
            return text;
        }

        /// <summary>
        /// Returns description of the option. This parameter is used to build option help.<para> </para>
        /// You can use the below listed substitutions:<para> </para>
        /// {0}  - application name<para> </para>
        /// {1}  - command name<para> </para>
        /// {2}  - option name<para> </para>
        /// {3}  - option aliases as string: alias1, alias2, alias3<para> </para>
        /// {4}  - default value for none specified optional option<para> </para>
        /// {5}  - default value for specified optional with optional value and value for this option is not specified<para> </para>
        /// {6}  - restriction list of values as string: item1 | item2 | item3<para> </para>
        /// {7}  - restriction list of values and their numeric values as string: item1=1 | item2=2 | item3=3<para> </para>
        /// {8}  - restriction list of values and their descriptions as string: item1 - descrption for item1 | item2 - descrption for item3 | item3 - descrption for item3<para> </para>
        /// {9}  - restriction list of values, their numeric value and descriptions as string: item1=1 - descrption for item1 | item2=2 - descrption for item3 | item3=3 - descrption for item3<para> </para>
        /// {10} - list of dependency options as string: dep1, dep2, dep3<para> </para>
        /// {11} - list of incongruous options as string: incongr1, incongr2, incongr3<para> </para>
        /// </summary>
        internal string GetOptionDescription()
        {
            return string.Format(GetAttributeValue(DrCmdOptionSettings.Description, string.Empty),
                                 Command.Settings.GetSettingsApplicationName(),                                             // application name
                                 Command.Name,                                                                              // command name
                                 Option.Name,                                                                               // option name
                                 GetAliasesAsString(", "),                                                                  // option aliases as string: alias1, alias2, alias3
                                 GetAttributeValue(DrCmdOptionSettings.DefaultValueIfNoneSpecified, string.Empty),          // default value for none specified optional option
                                 GetAttributeValue(DrCmdOptionSettings.DefaultValueIfSpecifiedWithoutValue, string.Empty),  // default value for specified optional with optional value and value for this option is not specified
                                 GetRestrictionListAsString(" | "),                                                         // restriction list of values as string: item1 | item2 | item3
                                 GetRestrictionListWithValueAsString(" | "),                                                // restriction list of values and their numeric values as string: item1=1 | item2=2 | item3=3
                                 GetRestrictionListWithDescriptionAsString(" | "),                                          // restriction list of values and their descriptions as string: item1 - descrption for item1 | item2 - descrption for item3 | item3 - descrption for item3
                                 GetRestrictionListWithValueAndDescriptionAsString(" | "),                                  // restriction list of values, their numeric value and descriptions as string: item1=1 - descrption for item1 | item2=2 - descrption for item3 | item3=3 - descrption for item3
                                 GetTermsOfDependencyAsString(", "),                                                        // list of dependency options as string: dep1, dep2, dep3
                                 GetTermsOfIncongruousAsString(", "));                                                      // list of incongruous options as string: incongr1, incongr2, incongr3

        }

        #endregion Help
    }
}
