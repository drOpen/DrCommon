/*
  DrCmdParser.cs -- object of command. 1.0.0, May 2, 2014
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation is required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>

 */
using System;
using System.Collections.Generic;
using System.Text;
using DrCmd.Res;
using DrOpen.DrCommon.DrCmd;
using DrOpen.DrData.DrDataObject;
using DrOpen.DrData.DrDataObject.Exceptions;
using DrOpen.DrCommon.DrExt;

namespace DrOpen.DrCommon.DrCmd
{
    /// <summary>
    /// class of command
    /// </summary>
    internal class DrCmdCommand : IComparable, IComparable<DrCmdCommand>
    {
        /// <summary>
        /// Create object of command by specified node with settings of command
        /// </summary>
        /// <param name="settings">reference to settings of parser</param>
        /// <param name="command">node with settings of command</param>
        internal DrCmdCommand(DrCmdParser settings, DDNode command)
        {
            command.Type.ValidateExpectedNodeType(DrCmdConst.TypeCommand);
            Settings = settings;
            Command = command;
            name = GetCommandName();
        }
        /// <summary>
        /// reference to settings of parser
        /// </summary>
        public DrCmdParser Settings { get; private set; }
        /// <summary>
        /// Settings of command specified in the constructor
        /// </summary>
        public DDNode Command { get; private set; }
        /// <summary>
        /// Command name
        /// </summary>
        private readonly string name;
        /// <summary>
        /// Command name
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// Return command name from node name
        /// Return empty string if name isn't specified
        /// </summary>
        /// <returns>return commnd name</returns>
        public string GetCommandName()
        {
            //return command.Attributes.GetValue(DrCmdCommandSettings.Name, string.Empty);
            return Command.Name;
        }
        /// <summary>
        /// Returns parameter <see cref="DrCmdCommandSettings.Description"/> as command description
        /// </summary>
        /// <returns></returns>
        public string GetCommandDescription()
        {
            return Command.Attributes.GetValue(DrCmdCommandSettings.Description, string.Empty);
        }
        /// <summary>
        /// Return true if command is enables, otherwise - false.
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return Command.Attributes.GetValue(DrCmdCommandSettings.Enabled, true);
        }
        /// <summary>
        /// Checks the parameters of active command. If any parameter is invalid throws an appropriate exception.
        /// <exception cref="ArgumentException">In case of error or not be able to choose a command throws an exception.</exception>
        /// </summary>
        public void ValidateCommandSettings()
        {
            if (Command == null) throw new NullReferenceException(Msg.COMMAND_IS_NULL);

            if ((Command.Attributes.GetValue(DrCmdCommandSettings.Enabled, true) != true)) // if command Disabled
            {
                throw new ArgumentException(string.Format(Msg.COMMAND_IS_DISABLED, GetCommandName()));
            }
        }
        /// <summary>
        /// The function validates the correctness of the settings for all options in this command.
        /// </summary>
        public void ValidateCommandParameters()
        {
            foreach (var option in Options)
            {
                option.ValidateOptionParameter();
            }
        }

        /// <summary>
        /// Returns options
        /// </summary>
        public IEnumerable<DrCmdOption> Options
        {
            get
            {
                foreach (var option in Command.Values)
                {
                    yield return new DrCmdOption(this, option);
                }
            }
        }


        /// <summary>
        /// Returns option of active command using search by name and aliases
        /// If the option with the same name or alias does not exist it returns null
        /// </summary>
        /// <param name="optionName">Name for search</param>
        /// <returns></returns>
        public DrCmdOption GetOptionByNameOrAlias(string optionName)
        {
            var stringComparison = Settings.GetSettingsStringComparison();
            foreach (var option in Options)
            {
                if (String.Compare(option.Name, optionName, stringComparison) == 0) return option;
                foreach (var alias in option.GetAliases())
                {
                    if (String.Compare(alias, optionName, stringComparison) == 0) return option;
                }
            }
            return null;
        }

        /// <summary>
        /// The function checks the correctness of the settings for specified command.
        /// If the command settings are incorrect a <exception cref="FormatException">FormatException</exception> will be thrown.
        /// </summary>
        public void VerifyComandSettings()
        {
            var hashSetNamesAndAliases = new HashSet<string>(Settings.GetSettingsStringComparer());      // Consider ignoring case
            var hashSetNames = new HashSet<string>(Settings.GetSettingsStringComparer());                // Consider ignoring case

            foreach (var opt in Options)
            {
                if (opt.IsEnabled())                                    // skip disabled option
                {
                    if (hashSetNamesAndAliases.Contains(opt.Name)) throw new FormatException(string.Format(Msg.DUBLICATE_OPTION_NAME, opt.Name, Name));
                    hashSetNamesAndAliases.Add(opt.Name);               // add option name to hashset : name and aliases
                    hashSetNames.Add(opt.Name);                         // add option name to hashset : name
                    foreach (var alias in opt.GetAliases())
                    {
                        if (hashSetNamesAndAliases.Contains(alias)) throw new FormatException(string.Format(Msg.DUBLICATE_OPTION_ALIASES, alias, Name));
                        hashSetNamesAndAliases.Add(alias);              // add alias to hashset : name and aliases
                    }
                    opt.VerifyOptionSettings();
                }
            }
            VerifyLinksFromTermToOption(hashSetNames);
        }


        #region VerifyLinksFromTerm
        /// <summary>
        /// Checks enabled options in the command reference values from TermsOfIncongruous and TermsOfDependency attributes
        /// </summary>
        /// <param name="hashSetNames">hash set with enabled options for specified command</param>
        private void VerifyLinksFromTermToOption(HashSet<string> hashSetNames)
        {
            foreach (var opt in Options)
            {
                opt.VerifyLinksFromTermToOption(hashSetNames, DrCmdOptionSettings.TermsOfIncongruous);
                opt.VerifyLinksFromTermToOption(hashSetNames, DrCmdOptionSettings.TermsOfDependency);
            }
        }

        #endregion VerifyLinksFromTerm
        /// <summary>
        /// Returns HashSet which contains options names which specified only for with command.
        /// </summary>
        /// <returns></returns>
        private HashSet<string> GetHashSetSpecifiedOptionName()
        {
            var hashSet = new HashSet<string>(Settings.GetSettingsStringComparer());
            foreach (var option in Options)
            {
                if (option.GetResultIsOptionSpecified() && !hashSet.Contains(option.Name)) hashSet.Add(option.Name);
            }
            return hashSet;
        }
        /// <summary>
        /// Checks dependency of options. If the terms are not met dependencies throws an <exception cref="ArgumentException">ArgumentException</exception>
        /// </summary>
        public void ValidateOptionsDependency()
        {
            var hashSet = GetHashSetSpecifiedOptionName();
            foreach (var option in Options)
            {
                if (option.GetResultIsOptionSpecified()) // for specified options only
                {
                    var termsOfDependency = option.GetAttributeValue(DrCmdOptionSettings.TermsOfDependency, new string[] { }).GetValueAsStringArray();
                    foreach (var itemDependency in termsOfDependency)
                    {
                        if (!hashSet.Contains(itemDependency))
                        {
                            throw new ArgumentException(string.Format(Msg.OPTION_DEPENDS_ANOTHER_NOT_SPECIFIED_OPTION, Name, option.Name, itemDependency));
                        }
                    }
                    var termsOfIncongruous = option.GetAttributeValue(DrCmdOptionSettings.TermsOfIncongruous, new string[] { }).GetValueAsStringArray();
                    foreach (var itemIncongruous in termsOfIncongruous)
                    {
                        if (hashSet.Contains(itemIncongruous))
                        {
                            throw new ArgumentException(string.Format(Msg.OPTION_IS_INCONGRUOUS_WTH_ANOTHER_OPTION, Name, option.Name, itemIncongruous));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyze and set results for matched option for each specified parameters. 
        /// If it isn't possible throws an <exception cref="ArgumentException">ArgumentException</exception>.
        /// </summary>
        /// <param name="paramsNode">node contains parameters from arguments</param>
        public void SetParamsByOptionsForActiveCommand(DDNode paramsNode)
        {
            foreach (var item in paramsNode)
            {
                var option = GetOptionByNameOrAlias(item.Key);
                if (option == null) // unknow option
                {
                    if (Settings.GetSettingsIgnoreUnknowArguments())
                        continue; //skip unknow option
                    else
                        throw new ArgumentException(string.Format(Msg.SPECIFIED_OPTION_IS_NOT_ALLOWED, item.Key, Name)); // unknown options are illegal
                }
                if (option.IsEnabled() == false) throw new ArgumentException(string.Format(Msg.SPECIFIED_OPTION_IS_DISABLED, item.Key, Name)); // disabled options are illegal
                if (option.GetResultIsOptionSpecified()) // if option is specified
                {
                    var specifiedName = option.GetAttributeValue(DrCmdOptionSettings.ResultSpecifiedOptionName, string.Empty);
                    throw new ArgumentException(string.Format(Msg.OPTION_ALREADY_SPECIFIED, item.Key, specifiedName, Name));
                }
                option.SetOptionResults(item); // set option result fields
            }
        }

        /// <summary>
        /// For each options in this command and save value ​​in their attribute Value based on the parameters passed, and / or using default values
        /// </summary>
        public void ApplyDefaultValue()
        {
            foreach (var option in Options)
            {
                option.ApplyDefaultValue();
            }
        }
        /// <summary>
        /// Removes attributes for all options filled after parsing parameters. 
        /// Cleaning must be done before the parsing of the parameters.
        /// </summary>
        public void Clear()
        {
            foreach (var option in Options)
            {
                option.Clear();
            }
        }

        /// <summary>
        ///  Transforms the current command and returns the node containing collection of attributes with options names and their values.
        /// </summary>
        /// <returns></returns>
        public DDNode TransformCommandToResult()
        {
            var ddNode = new DDNode(Name);
            foreach (var option in Options)
            {
                if ((option.GetValueFlags() & DrCmdValueFlags.Forbidden) == DrCmdValueFlags.Forbidden)
                    ddNode.Attributes.Add(option.Name, option.GetResultIsOptionSpecified()); // set true - false for DrCmdValueFlags.Forbidden value flag
                else
                    ddNode.Attributes.Add(option.Name, option.Value);
            }
            return ddNode;
        }
        #region CompareTo
        /// <summary>
        /// Compare command and its options
        /// </summary>
        /// <param name="obj">compare with</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DrCmdCommand)) return 1;
            return this.CompareTo((DrCmdCommand)obj);
        }
        /// <summary>
        /// Compare command and its options
        /// </summary>
        /// <param name="other">compare with</param>
        /// <returns></returns>
        public int CompareTo(DrCmdCommand other)
        {
            return DDNode.Compare(Command, other.Command);
        }
        #endregion CompareTo
        #region Help
        /// <summary>
        /// Save command help to attribute <see cref="DrCmdCommandSettings.Help"/>
        /// </summary>
        internal void SaveHelp()
        {
            Command.Attributes.Add(DrCmdCommandSettings.Help, GetHelp(), ResolveConflict.OVERWRITE);
        }
        /// <summary>
        /// Returns help for command: command description, synopsys and descriptions for all enabled options
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            var text = new StringBuilder();
            text.Append(Msg.HELP_COMMAND);
            text.Append("\r\n\r\n");
            text.Append(GetHelpCommandDescription());
            //text.Append("\r\n");
            text.Append(Settings.GetHelpTab());
            text.Append(Msg.HELP_SYNOPSIS);
            text.Append("\r\n\r\n");

            text.Append(GetHelpCommandSynopsis());
            text.Append("\r\n");
            text.Append(GetHelpOptionDescription(DrCmdOptionType.Required));
            text.Append(GetHelpOptionDescription(DrCmdOptionType.Optional));
            text.Append(GetHelpExamples());

            return text.ToString();
        }

        /// <summary>
        /// Returns help string with option description for specified option flag.
        /// </summary>
        /// <param name="flag">option flag to describe</param>
        /// <returns></returns>
        private string GetHelpOptionDescription(DrCmdOptionType type)
        {
            string text = string.Empty;
            var optionDescription = GetOptionDescription(type);
            if (optionDescription.Length > 0)
            {
                text = Settings.GetHelpTab(2) + (type == DrCmdOptionType.Required ? Msg.HELP_REQUIRED : Msg.HELP_OPTIONAL) + "\r\n\r\n" + optionDescription +  "\r\n";
            }
            return text;
        }

        /// <summary>
        /// Returns option description for command help
        /// </summary>
        /// <param name="typeOption">option flag to describe</param>
        /// <returns></returns>
        private string GetOptionDescription(DrCmdOptionType typeOption)
        {
            var tabOption = Settings.GetHelpTab(3);
            var tabRelativeOptionDescription = GetOptionDescriptionRelativeTab();
            var tabOptionDescription = tabRelativeOptionDescription + tabOption;

            var text = string.Empty;
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(tabOptionDescription.Length, Settings.GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            foreach (var option in Options)
            {
                if ((option.IsEnabled()) && (option.Type.HasFlag(typeOption)))
                {
                    var nameAndAliases = DrCmdConst.OptionStartSymbolName + option.GetNameAndAliasesAsString(", -");
                    builder.Format.SetFormat(tabOptionDescription.Length, Settings.GetSettingsHelpMaxLineLength(), (nameAndAliases.Length < tabRelativeOptionDescription.Length)); // change format
                    text += tabOption + nameAndAliases;
                    if (nameAndAliases.Length >= tabRelativeOptionDescription.Length)
                        text += "\r\n"; // for long option name with aliases - new line
                    else
                        text += (new string(' ', tabRelativeOptionDescription.Length - nameAndAliases.Length));
                    text += builder.BuildText(option.GetOptionDescription()) ;
                }
            }
            return text;
        }
        /// <summary>
        /// Returns help string with examples fro command 
        /// </summary>
        private string GetHelpExamples()
        {
            var examples = GetExamples();
            if (examples.Length == 0) return String.Empty;

            var tabOption = Settings.GetHelpTab(3);
            var tabRelativeOptionDescription = GetOptionDescriptionRelativeTab();
            var tabOptionDescription = tabRelativeOptionDescription + tabOption;

            var text = Settings.GetHelpTab(2) + Msg.HELP_EXAMPLE + "\r\n\r\n";
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(tabOptionDescription.Length, Settings.GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            int i = 0; // counter
            foreach (var example in examples)
            {
                if ((example.Length > 0))
                {
                    i++;
                    var id = i.ToString() + ")";
                    text += tabOption + id;
                    text += (new string(' ', tabRelativeOptionDescription.Length - id.Length));
                    var firstLine = true;
                    foreach  ( var line  in example.Split(new[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!firstLine) text += tabOptionDescription;
                        text += builder.BuildText(line) ;
                        firstLine = false;
                    }
                }
                text += "\r\n";
            }
            return text;
        }

        /// <summary>
        /// Returns a string tab for the option name and its aliases. 
        /// Limiting the length of two standard tabs. 
        /// If the listing of the option name and its aliases more than two tabs, the description for this option should go on the next line.
        /// </summary>
        /// <returns></returns>
        private string GetOptionDescriptionRelativeTab()
        {
            return new string(' ', GetMaxOptionNameWithAliases());
        }

        /// <summary>
        /// Returns the maximum length of the name with aliases of the enabled options
        /// </summary>
        /// <returns></returns>
        private int GetMaxOptionNameWithAliases()
        {
            var maxLenght = 0;
            const int maxLenghtAllowed = 7;
            foreach (var option in Options)
            {
                var nameAndAliasesAsString = DrCmdConst.OptionStartSymbolName + option.GetNameAndAliasesAsString(", -") + " ";
                if ((option.IsEnabled()) && (nameAndAliasesAsString.Length > maxLenght) && (maxLenghtAllowed >= nameAndAliasesAsString.Length)) maxLenght = nameAndAliasesAsString.Length;
            }
            return (maxLenght == 0 ? maxLenghtAllowed : maxLenght);
        }
        /// <summary>
        /// Returns description for this command.<para> </para>
        /// <example>Example: COMMAND description for this command</example>
        /// </summary>
        /// <returns></returns>
        private string GetHelpCommandDescription()
        {
            var text = string.Empty;
            var tab = Settings.GetHelpTab();
            var maxCommandNameLength = Settings.GetMaximumCommandNameLength() + 1;
            var leftMargin = tab.Length + maxCommandNameLength;
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(leftMargin, Settings.GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            return tab + Name + (new string(' ', maxCommandNameLength - Name.Length)) + builder.BuildText(GetCommandDescription()) + "\r\n";
        }

        /// <summary>
        /// Returns synopsis for this command with synopsis for all options for this command.<para> </para>
        /// <example>Example: Application.exe COMMAND -Opt1 Opt1Value [-Opt2 Opt2Value] [-Opt3 [Opt2Value]]</example>
        /// </summary>
        /// <returns></returns>
        private string GetHelpCommandSynopsis()
        {
            var tab = Settings.GetHelpTab(2);
            var text = tab + Settings.GetSettingsApplicationName() + " " + Name;
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(text.Length + 1, Settings.GetSettingsHelpMaxLineLength(), false, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            text += " " + builder.BuildWorlds(GetOptionsSynopsisAsSingleLine());
            return text;
        }

        /// <summary>
        /// Returns synopsis as string array for all enabled options for this command like: {"-opt1", "-opt2 specify file name", "[-opt3 [value3]]"}. 
        /// See details in <see cref="DrCmdOption.GetOptionSynopsis"/><para> </para>
        /// Required options are specified at the beginning.
        /// </summary>
        /// <returns></returns>
        private string[] GetOptionsSynopsisAsSingleLine()
        {
            var textRequired = string.Empty;
            var textOptional = string.Empty;
            foreach (var option in Options)
            {
                if (option.IsEnabled())
                {
                    if (option.Type.HasFlag(DrCmdOptionType.Required))
                    {
                        if (textRequired.Length > 0) textRequired += "\0";
                        textRequired += option.GetOptionSynopsis();
                    }
                    else
                    {
                        if (textOptional.Length > 0) textOptional += "\0";
                        textOptional += option.GetOptionSynopsis();
                    }
                }
            }
            if ((textRequired.Length > 0) && (textOptional.Length > 0)) textRequired += "\0"; // add space separator beetwen Required and Optional options
            return (textRequired + textOptional).Split(new[] { '\0' });
        }
        /// <summary>
        /// Returns examples of the command. This parameter is used to build command help.<para> </para>
        /// You can use the below listed substitutions:<para> </para>
        /// {0}  - application name<para> </para>
        /// {1}  - command name<para> </para>
        /// </summary>
        private string[] GetExamples()
        {
            var examples = Command.Attributes.GetValue(DrCmdCommandSettings.Example, string.Empty).GetValueAsStringArray();
            var result = new string[examples.Length];
            int i = 0;
            foreach (var example in examples)
            {
                result[i] = string.Format(example, Settings.GetSettingsApplicationName(), Command.Name);
                i++;
            }
            return result;
        }
        #endregion Help

    }
}
