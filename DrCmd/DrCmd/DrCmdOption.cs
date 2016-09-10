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
            valueFlags = GetValueFlags();
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
        /// Option flag
        /// </summary>
        private readonly DrCmdOptionType type;
        /// <summary>
        /// Option flag
        /// </summary>
        public DrCmdOptionType Type
        {
            get { return type; }
        }
        /// <summary>
        /// Value flag
        /// </summary>
        private readonly DrCmdValueFlags valueFlags;
        /// <summary>
        /// Value flag
        /// </summary>
        public DrCmdValueFlags ValueFlags
        {
            get { return valueFlags; }
        }
        /// <summary>
        /// Retruns parametr ac of this option from <see cref="DrCmdOptionSettings.Value"/>
        /// </summary>
        public DDValue Value
        {
            get { return GetAttributeValue(DrCmdOptionSettings.Value, string.Empty); }
        }

        /// <summary>
        /// Gets the ac from attribute collection associated with the specified name. When this method returns, 
        /// contains the associated with the specified name, if the name is found; 
        /// otherwise, the default ac for the flag of the ac parameter.
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="defaultValue">the default ac for the flag of the ac parameter.</param>
        /// <returns>When this method returns, contains the associated with the specified name, if the nameis found; 
        /// otherwise, the default ac for the flag of the ac parameter.</returns>
        public DDValue GetAttributeValue(object name, object defaultValue)
        {
            return Option.Attributes.GetValue(name.ToString(), defaultValue);
        }

        #region Default
        /// <summary>
        /// Returns default option flag <see cref="DrCmdOptionType.Optional"/>
        /// </summary>
        /// <returns></returns>
        public static DrCmdOptionType GetDefaultOptionType()
        {
            return DrCmdOptionType.Optional;
        }
        /// <summary>
        /// Returns default option ac flag <see cref="DrCmdValueFlags.Optional"/>
        /// </summary>
        /// <returns></returns>
        public static DrCmdValueFlags GetDefaultValueFlags()
        {
            return DrCmdValueFlags.Optional;
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
        /// Returns restriction list and numeric as single string separated by specified separator
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
        /// Returns restriction list and description as single string separated by specified separator
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
        /// Returns restriction list with ac  and description as single string separated by specified separator
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
        /// Build options flag from string array. Ignore case.
        /// </summary>
        /// <returns></returns>
        public DrCmdOptionType GetOptionType()
        {
            return GetOptionType(true);
        }
        /// <summary>
        /// Build options flag from string array from attribute <see cref="DrCmdOptionSettings.Type"/>.
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
        /// Build values flags from string array. Ignore case
        /// </summary>
        /// <returns></returns>
        public DrCmdValueFlags GetValueFlags()
        {
            return GetValueFlags(true);
        }

        /// <summary>
        /// Build values flags from string array from attribute <see cref="DrCmdOptionSettings.ValueFlags"/>
        /// If cannot convert string representation to the ValueFlags <exception cref="FormatException">FormatException</exception> will be threw.
        /// </summary>
        /// <param name="ignoreCase">if true ignore case, otherwise regard case</param>
        /// <returns></returns>
        public DrCmdValueFlags GetValueFlags(bool ignoreCase)
        {

            var flagValue = GetAttributeValue(DrCmdOptionSettings.ValueFlags, GetDefaultValueFlags().ToString()).GetValueAsStringArray();
            var flag = typeof(DrCmdValueFlags);
            DrCmdValueFlags flagValueAsEnum = 0;
            foreach (var item in flagValue)
            {
                try
                {
                    flagValueAsEnum |= (DrCmdValueFlags)Enum.Parse(flag, item, ignoreCase);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format(Msg.CANNOT_PARSE_VALUE_AS_FLAG_OF_VALUE, item), e);
                }
            }
            return flagValueAsEnum;
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
            return GetOptionValueAsStringArray(value.Attributes);
        }

        /// <summary>
        /// Returns string array from all values in collections of attributes
        /// </summary>
        /// <returns></returns>
        private static string[] GetOptionValueAsStringArray(DDAttributesCollection ac)
        {
            var result = new string[ac.Values.Count];
            int i = 0;
            foreach (var item in ac.Values)
            {
                result[i] = item;
                i++;
            }
            return result;
        }


        private DDValue ConvertToDDValue(DDAttributesCollection ac)
        {
            if (ac == null) return new DDValue(null);
            if (ac.Count == 0) return new DDValue();
            if (ac.Count == 1)  
            {
                var en = ac.GetEnumerator();
                en.MoveNext();
                return en.Current.Value;
            }
            return new DDValue(GetOptionValueAsStringArray(ac));
        }

        /// <summary>
        /// Set option result fields
        /// </summary>
        /// <param name="item">KeyValuePair contains parameters for this option</param>
        public void SetOptionResults(KeyValuePair<string, DDNode> item)
        {
            Option.Attributes.Add(DrCmdOptionSettings.ResultIsOptionSpecified, true, ResolveConflict.OVERWRITE);                                        // option is specified
            Option.Attributes.Add(DrCmdOptionSettings.ResultSpecifiedOptionName, item.Key, ResolveConflict.OVERWRITE);                                  // specified option name
            
            Option.Attributes.Add(DrCmdOptionSettings.ResultValueAsStringArray, GetOptionValueAsStringArray(item.Value), ResolveConflict.OVERWRITE);                                               // specified option as string array
            Option.Attributes.Add(DrCmdOptionSettings.ResultIsOptionSpecifiedValue, item.Value.HasAttributes, ResolveConflict.OVERWRITE);               // specified option as string array            

            Option.Attributes.Add(DrCmdOptionSettings.ResultValue, ConvertToDDValue(item.Value.Attributes), ResolveConflict.OVERWRITE);                 // specified option DDValue
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
            Option.Attributes.Remove(DrCmdOptionSettings.ResultValueAsStringArray);
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
        /// Checks flag of attributes <see cref="DrCmdOptionSettings.RestrictionListAsNumeric"/>. 
        /// </summary>
        public void VerifyRestrictionListAsNumericType()
        {
            var value = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { });
            if (value.Type != typeof(int[])) throw new FormatException(string.Format(Msg.INCORRECT_TYPE_OF_RESTRICTION_LIST_AS_NUMERIC, value.Type.Name));
        }

        /// <summary>
        /// Verify flag of option. 
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
        /// Verify flag of ac for option. 
        /// In the case of the incongruous flags are detected ​​the <exception cref="FormatException">FormatException</exception>will be thrown
        /// </summary>
        public void VerifyValueRestrictionsType()
        {
            try
            {
                #region Forbidden
                if ((ValueFlags.HasFlag(DrCmdValueFlags.Forbidden)) && (ValueFlags != DrCmdValueFlags.Forbidden))
                    throw new FormatException(string.Format(Msg.WITH_THIS_TYPE_CANNOT_BE_SPECIFIED_MORE_THAN_OTHER_TYPES, DrCmdValueFlags.Forbidden));
                #endregion Forbidden
                if ((ValueFlags.HasFlag(DrCmdValueFlags.AllowNumeric)) && (!ValueFlags.HasFlag(DrCmdValueFlags.ListOfRestriction)))
                    throw new FormatException(string.Format(Msg.CANNOT_SPECIFY_ONE_TYPE_WITHOUT_SECOND_TYPE, DrCmdValueFlags.AllowNumeric, DrCmdValueFlags.ListOfRestriction)); // AllowNumeric without ListOfRestriction
                var restrictionList = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();
                var restrictionListDescription = GetAttributeValue(DrCmdOptionSettings.RestrictionListDescription, new string[] { }).GetValueAsStringArray();
                if (ValueFlags.HasFlag(DrCmdValueFlags.AllowNumeric)) // if AllowNumeric is true need to check both list element count -> RestrictionList and RestrictionListAsNumeric
                {
                    var restrictionListAsNumeric = GetAttributeValue(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { }).GetValueAsIntArray();
                    if (restrictionList.Length != restrictionListAsNumeric.Length)
                        throw new FormatException(string.Format(Msg.ELEMENT_COUNT_IN_RESTRICTION_LIST_IS_NOT_EQUALS_OF_NUMERIC_LIST_COUNT, Name, restrictionList.Length.ToString(), restrictionListAsNumeric.Length.ToString()));
                }
                if ((restrictionList.Length>0) && (restrictionListDescription.Length>0) && (restrictionList.Length!=restrictionListDescription.Length))
                    throw new FormatException(string.Format(Msg.ELEMENT_COUNT_IN_RESTRICTION_LIST_IS_NOT_EQUALS_OF_RESTRICTIOM_DESCRIPTION_LIST, Name, restrictionList.Length.ToString(), restrictionListDescription.Length.ToString()));
                CheckIncongruousTypeOfValue(ValueFlags, DrCmdValueFlags.Optional, DrCmdValueFlags.Required);                                                             // Optioanl & Required
                CheckIncongruousTypeOfValue(ValueFlags, DrCmdValueFlags.Single, DrCmdValueFlags.List);                                                                   // Single & List
            }
            catch (ApplicationException e)
            {
                throw new FormatException(string.Format(Msg.OPTION_TYPE_IS_WRONG, Name, ValueFlags.ToString()), e);
            }
        }
        /// <summary>
        /// Checks the current ac of the two incongruous flags simultaneously. 
        /// If the two flags are detected ​​at the same time the <exception cref="FormatException">FormatException</exception> will be thrown.
        /// </summary>
        /// <param name="current">Current flag of ac</param>
        /// <param name="first">the first incongruous flag to detect</param>
        /// <param name="second">the second  incongruous flag to detect</param>
        private static void CheckIncongruousTypeOfValue(DrCmdValueFlags current, DrCmdValueFlags first, DrCmdValueFlags second)
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
            // start validate ac Type
            if (Option.Attributes.Contains(DrCmdOptionSettings.ValueType))  // validate specified Type for convert
            {
                var valueType = System.Type.GetType(Option.Attributes[DrCmdOptionSettings.ValueType].GetValueAsString());
                if (!DDValue.ValidateType(valueType)) throw new ArgumentException(string.Format(Msg.SPECIFIED_VALUE_TYPE_IS_NOT_SUPPORTED_BY_DDVALUE, valueType.ToString()));
            }
            // end validate ac Type
            var isSpecifiedOption = GetAttributeValue(DrCmdOptionSettings.ResultIsOptionSpecified, false);
            if ((Type.HasFlag(DrCmdOptionType.Required)) & (isSpecifiedOption == false)) throw new ArgumentException(string.Format(Msg.REQUIRED_OPTION_IS_NOT_SPECIFIED, Name, CommandName));
            // ac flag
            var valueAsStringArray = GetAttributeValue(DrCmdOptionSettings.ResultValueAsStringArray, new string[] { }).GetValueAsStringArray();
            var valueAsString = GetAttributeValue(DrCmdOptionSettings.ResultValueAsStringArray, string.Empty).GetValueAsString();
            // Single
            if ((ValueFlags.HasFlag(DrCmdValueFlags.Single)) & (valueAsStringArray.Length > 1)) throw new ArgumentException(string.Format(Msg.OPTION_CANNOT_CONTAINS_MORE_THAN_ONE_VALUE, Name, valueAsString, valueAsStringArray.Length, DrCmdValueFlags.Single, CommandName));
            // Forbidden
            if ((ValueFlags.HasFlag(DrCmdValueFlags.Forbidden)) & (valueAsStringArray.Length > 0)) throw new ArgumentException(string.Format(Msg.OPTION_CANNOT_CONTAIN_VALUE, Name, valueAsString, DrCmdValueFlags.Forbidden, CommandName));
            // Required
            if ((isSpecifiedOption) && (ValueFlags.HasFlag(DrCmdValueFlags.Required)) & (valueAsStringArray.Length == 0)) throw new ArgumentException(string.Format(Msg.OPTION_MUST_CONTAIN_VALUE, Name, DrCmdValueFlags.Required, CommandName));
            if ((ValueFlags.HasFlag(DrCmdValueFlags.ListOfRestriction)) && (valueAsStringArray.Length > 0)) ValidateOptionValueByRestrictionList(valueAsStringArray); // RestrictionList and AllowNumeric

        }

        /// <summary>
        /// Checks the ac of the options on their compliance with the list of allowed values ​​or, if enabled, their numerical conform
        /// If the option ac is incorrect by restriction list from attribute <see cref="DrCmdOptionSettings.RestrictionList"/> the <exception cref="ArgumentException">ArgumentException</exception> will be thrown.
        /// </summary>
        /// <param name="valueAsStringArray">option values as string array</param>
        private void ValidateOptionValueByRestrictionList(IEnumerable<string> valueAsStringArray)
        {
            var restrictionListArray = GetAttributeValue(DrCmdOptionSettings.RestrictionList, new string[] { }).GetValueAsStringArray();

            var ignoreCase = !Command.Settings.GetSettingsCaseSensitive();
            foreach (var val in valueAsStringArray)
            {
                if ((ValueFlags.HasFlag(DrCmdValueFlags.AllowNumeric)) && (val.IsPositiveNumber())) continue;
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
                    if (ValueFlags.HasFlag(DrCmdValueFlags.AllowNumeric))
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
        /// Save ac ​​in the attribute Value based on the parameters passed, and / or using default values
        /// </summary>
        public void ApplyDefaultValue()
        {
            var value = new DDValue();
            if (GetResultIsOptionSpecified())
            {
                if (GetAttributeValue(DrCmdOptionSettings.ResultIsOptionSpecifiedValue, false))
                {
                    value = GetAttributeValue(DrCmdOptionSettings.ResultValue, string.Empty);                                  // if option ac is specified
                }
                else
                {
                    value = GetAttributeValue(DrCmdOptionSettings.DefaultValueIfSpecifiedWithoutValue, string.Empty);          // if option is specified without ac
                }
            }
            else
            {
                value = GetAttributeValue(DrCmdOptionSettings.DefaultValueIfNoneSpecified, string.Empty);                       // if option is not specified
            }
            if (Option.Attributes.Contains(DrCmdOptionSettings.ValueType)) // if ValueType is specified ac will be converted to specified Type
            {
                if ((value.Type == typeof(System.String)) || (value.Type == typeof(System.String[]))) value.ConvertFromStringTo(Option.Attributes[DrCmdOptionSettings.ValueType].GetValueAsString());
            }
            Option.Attributes.Add(DrCmdOptionSettings.Value, value);
        }

        #region Help
        /// <summary>
        /// Returns the synopsis for the option and her valuedepending on the flag of option and ac flag
        /// If ac for this option can be optional, her ac flag is not <see cref="DrCmdValueFlags.Required"/> the function will be return option synopsis in square brackets <para> </para>
        /// How synopsis for the ac will look of the function, see <see cref="GetOptionValueSynopsis"/><para> </para>
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
        /// Returns the synopsis for the ac of this option depending on the flag of ac. <para> </para>
        /// If ac for this option can be optional, her ac flag is not <see cref="DrCmdValueFlags.Required"/> the function will be return ac synopsis in square brackets <para> </para>
        /// If this option cannot contains ac, her ac flag is <see cref="DrCmdValueFlags.Forbidden"/> the function will be return empty string 
        /// </summary>
        /// <returns></returns>
        internal string GetOptionValueSynopsis()
        {
            var text = string.Empty;
            if (!ValueFlags.HasFlag(DrCmdValueFlags.Forbidden)) // this option can have ac
            {
                text = GetAttributeValue(DrCmdOptionSettings.SynopsisValue,string.Empty);
                if (!ValueFlags.HasFlag(DrCmdValueFlags.Required)) text = "[" + text + "]"; // this ac is optional 
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
        /// {4}  - default ac for none specified optional option<para> </para>
        /// {5}  - default ac for specified optional with optional ac and ac for this option is not specified<para> </para>
        /// {6}  - restriction list of values as string: item1 | item2 | item3<para> </para>
        /// {7}  - restriction list of values and their numeric values as string: item1=1 | item2=2 | item3=3<para> </para>
        /// {8}  - restriction list of values and their descriptions as string: item1 - descrption for item1 | item2 - descrption for item3 | item3 - descrption for item3<para> </para>
        /// {9}  - restriction list of values, their numeric ac and descriptions as string: item1=1 - descrption for item1 | item2=2 - descrption for item3 | item3=3 - descrption for item3<para> </para>
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
                                 GetAttributeValue(DrCmdOptionSettings.DefaultValueIfNoneSpecified, string.Empty),          // default ac for none specified optional option
                                 GetAttributeValue(DrCmdOptionSettings.DefaultValueIfSpecifiedWithoutValue, string.Empty),  // default ac for specified optional with optional ac and ac for this option is not specified
                                 GetRestrictionListAsString(" | "),                                                         // restriction list of values as string: item1 | item2 | item3
                                 GetRestrictionListWithValueAsString(" | "),                                                // restriction list of values and their numeric values as string: item1=1 | item2=2 | item3=3
                                 GetRestrictionListWithDescriptionAsString(" | "),                                          // restriction list of values and their descriptions as string: item1 - descrption for item1 | item2 - descrption for item3 | item3 - descrption for item3
                                 GetRestrictionListWithValueAndDescriptionAsString(" | "),                                  // restriction list of values, their numeric ac and descriptions as string: item1=1 - descrption for item1 | item2=2 - descrption for item3 | item3=3 - descrption for item3
                                 GetTermsOfDependencyAsString(", "),                                                        // list of dependency options as string: dep1, dep2, dep3
                                 GetTermsOfIncongruousAsString(", "));                                                      // list of incongruous options as string: incongr1, incongr2, incongr3

        }

        #endregion Help
    }
}
