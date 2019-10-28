/*
  DrCmdParser.cs -- library to parse arguments. 1.0.0, May 2, 2014
 
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
using DrOpen.DrCommon.DrExt.Win32;
using DrOpen.DrData.DrDataObject;
using DrOpen.DrData.DrDataObject.Exceptions;

namespace DrOpen.DrCommon.DrCmd
{

    /// <summary>
    /// class to parse arguments
    /// </summary>
    public class DrCmdParser
    {
        /// <summary>
        /// Create instance of DrCmdParser by specified settings and validate their. 
        /// If settings are incorrect the <exception cref="FormatException">FormatException</exception>will be threw
        /// </summary>
        /// <param name="settings">settings for command and their options</param>
        public DrCmdParser(DDNode settings)
        {
            settings.Type.ValidateExpectedNodeType(DrCmdConst.TypeSettings);
            Settings = settings;
            VerifySettings();   // verify parser, commands and options settings
            SaveHelp();         // build and save help to application and commands
        }
        /// <summary>
        /// Settings specified in the constructor
        /// </summary>
        public DDNode Settings { get; private set; }
        /// <summary>
        /// The command selected on the basis of arguments specified. If the command is not able to identify contains is null.
        /// </summary>
        private DrCmdCommand activeCommand;
        /// <summary>
        /// The command selected on the basis of arguments specified. If the command is not able to identify contains is null.
        /// </summary>
        public DDNode ActiveCommnand
        {
            get { return (activeCommand == null ? null : activeCommand.Command); }
        }
        /// <summary>
        /// Reads the specified settings arguments determines the specified command and substitutes the value of its options for it. 
        /// Returns the node containing collection of attributes with options names and their values. 
        /// If parse and compare this arguments are not possible, or are contrary to the values ​​specified in the configuration rules - throws an exception.
        /// </summary>
        /// <returns></returns>
        public DDNode Parse()
        {
            Clear();                                                                    // Removes attributes for all options in all commands filled after parsing parameters. 
            activeCommand = GetActiveCommand();                                         // Get active command by first argument ToDo should be 'if else' for support arguments without command
            activeCommand.ValidateCommandSettings();                                    // Check commands settings before parse
            var paramsNode = SplitOptionsAndTheirValuesByArguments(1);                  // Get parameters from arguments
            activeCommand.SetParamsByOptionsForActiveCommand(paramsNode);               // Set parameters to active command
            activeCommand.ValidateCommandParameters();                                  // Check commands parameters after parse
            activeCommand.ValidateOptionsDependency();                                  // Check options dependency - TermsOfDependency and TermsOfIncongruous
            activeCommand.ApplyDefaultValue();                                          // Apply default value if need it
            return activeCommand.TransformCommandToResult();                            // Transform to result format
        }

        #region Properties
        /// <summary>
        /// Returns specified argumets form the Setting. 
        /// If parameter <see cref="DrCmdSettings.RemoveStartEndQuotas"/> is true the quotation marks at the beginning and at the end arguments will be removed
        /// </summary>
        /// <returns></returns>
        internal string[] GetSettingsArguments()
        {
            var args = Settings.Attributes.GetValue(DrCmdSettings.Arguments, new string[] { }).GetValueAsStringArray();
            if (GetSettingsRemoveStartEndQuotas())
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if ((args[i].Length > 1) && (args[i].StartsWith("\"")) && (args[i].EndsWith("\""))) args[i] = args[i].Substring(1, args[i].Length - 2);
                }
            }
            return args;
        }
        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.ApplicationName"/> value .
        /// If it is not specified, returns the FriendlyName value from <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <returns></returns>
        internal string GetSettingsApplicationName()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.ApplicationName, AppDomain.CurrentDomain.FriendlyName);
        }
        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.ApplicationDescription"/> as application description
        /// </summary>
        /// <returns></returns>
        internal string GetSettingsApplicationDescription()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.ApplicationDescription, string.Empty);
        }
        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.HelpMaxLineLength"/> as the maximum number of characters in a single line in the help.
        /// If it is not specified, returns the BufferWidth-1 value from <see cref="Console"/> 
        /// Returns <see cref="DrCmdConst.SYMBOLS_LEIGHT_FOR_NONE_CONSOLE_OUT"/> value if StdOut is redirectred from console to something else
        /// </summary>
        /// <returns></returns>
        internal int GetSettingsHelpMaxLineLength()
        {
            int consoleBuffer = DrCmdConst.SYMBOLS_LEIGHT_FOR_NONE_CONSOLE_OUT;
            uint cMode;

            if (WinNT.GetConsoleMode(WinNT.GetStdHandle(WinNT.STD_OUTPUT_HANDLE), out cMode)) 
            {
                try   { consoleBuffer = Console.BufferWidth - 1; }
                catch { consoleBuffer = DrCmdConst.DEFAULT_CONSOLE_BUFFER; }
            }

            return Settings.Attributes.GetValue(DrCmdSettings.HelpMaxLineLength, consoleBuffer);
        }
        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.HelpTabSize"/> as number of space characters used as a tab for help. By default, this value is equal 3.
        /// </summary>
        /// <returns></returns>
        internal int GetSettingsHelpTabSize()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.HelpTabSize, 3);
        }

        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.RemoveStartEndQuotas"/>ac, If it is true, the quotation marks at the beginning and at the end arguments will be removed
        /// </summary>
        /// <returns></returns>
        internal bool GetSettingsRemoveStartEndQuotas()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.RemoveStartEndQuotas, false);
        }

        /// <summary>
        /// Returns a string comparer operation that uses specific case and culture-based or ordinal comparison rules command and option name in dependency from case sensitive setting.
        /// </summary>
        /// <returns></returns>
        internal StringComparer GetSettingsStringComparer()
        {
            return (GetSettingsCaseSensitive() ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a string comparison operation that uses specific case and culture-based or ordinal comparison rules command  and option name in dependency from case sensitive setting.
        /// </summary>
        /// <returns></returns>
        internal StringComparison GetSettingsStringComparison()
        {
            return (GetSettingsCaseSensitive() ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.CaseSensitive"/>ac, If it is false command and arguments ignoring the case of the strings being compared.
        /// </summary>
        /// <returns></returns>
        internal bool GetSettingsCaseSensitive()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.CaseSensitive, false);
        }

        /// <summary>
        /// Returns parameter <see cref="DrCmdSettings.IgnoreUnknowArguments"/>ac, 
        /// If it is true - the parser will be ignore the unknow arguments, otherwise the <exception cref="ArgumentException">ArgumentException</exception>will be thrown
        /// </summary>
        /// <returns></returns>
        internal bool GetSettingsIgnoreUnknowArguments()
        {
            return Settings.Attributes.GetValue(DrCmdSettings.IgnoreUnknowArguments, false);
        }
        #endregion Properties

        /// <summary>
        /// Removes attributes for all options in all commands filled after parsing parameters. 
        /// Cleaning must be done before the parsing of the parameters.
        /// </summary>
        public void Clear()
        {
            foreach (var command in Commands)
            {
                command.Clear();
            }
        }

        /// <summary>
        /// Returns the name of the command analyzing the first element in the arguments. 
        /// If first argument is empty the <exception cref="ArgumentException">ArgumentException</exception> will be thrown with inner exception.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedCommandNameFromArguments()
        {
            try
            {
                var args = GetSettingsArguments();
                if (args.Length == 0) throw new ArgumentNullException(Msg.COMMAND_NAME_IS_NOT_SPECIFIED);
                var commandName = args[0];
                if (commandName.Length == 0) throw new ArgumentException(Msg.COMMAND_NAME_IS_EMPTY);            // redundancy
                return commandName;
            }
            catch (Exception exc)
            {
                throw new ArgumentException(Msg.CANNOT_GET_COMMAND, exc);
            }
        }

        /// <summary>
        /// Analyzes the available commands and chooses the command from specified arguments.
        /// <exception cref="ArgumentException">In case of error or not be able to choose a command throws an ArgumentException.</exception>
        /// </summary>
        /// <returns>Returns specified command</returns>
        internal DrCmdCommand GetActiveCommand()
        {
            var selectedCommandName = GetSelectedCommandNameFromArguments();
            DrCmdCommand result = null;
            foreach (var command in Commands)
            {
                if (String.Compare(selectedCommandName, command.Name, GetSettingsStringComparison()) == 0)
                {
                    result = command;
                    break;
                }
            }
            if (result == null) throw new ArgumentException(string.Format(Msg.COMMAND_IS_NOT_SUPPORTED, selectedCommandName));
            return result;
        }
        /// <summary>
        /// Returns commands
        /// </summary>
        internal IEnumerable<DrCmdCommand> Commands
        {
            get
            {
                foreach (var command in Settings.Values)
                {
                    yield return new DrCmdCommand(this, command);
                }
            }
        }


        #region Arguments Parser
        /// <summary>
        /// Analyzes the arguments skipping a parameter with command name. 
        /// Returns a structure with the names of specified parameters and their values​​, if any were specified.
        /// If options is incorrect an <exception cref="ArgumentException">ArgumentException</exception> will be thrown
        /// </summary>
        /// <param name="startFrom">Index number at which to begin analyzing the parameters. Designed for skip a parameter with command name.</param>
        /// <returns>Returns a structure with the names of specified parameters and their values​​, if any were specified.</returns>
        internal DDNode SplitOptionsAndTheirValuesByArguments(int startFrom)
        {
            var enableScaningMode = true;
            var result = new DDNode();
            DDNode currentOption = null; // create after find option (-)
            var args = GetSettingsArguments();
            for (int i = startFrom; i < args.Length; i++)
            {
                if (args[i].StartsWith(DrCmdConst.OptionStartSymbolName)) // Is this Option?
                {
                    if (((args[i]).Equals(DrCmdConst.EndOptionScaning)) || ((args[i]).Equals(DrCmdConst.StartOptionScaning))) // scaning mode options
                    {
                        if ((((args[i]).Equals(DrCmdConst.EndOptionScaning)) && enableScaningMode) || (((args[i]).Equals(DrCmdConst.StartOptionScaning)) && !enableScaningMode))
                        {
                            enableScaningMode = !enableScaningMode; // revert scaning mode
                            continue;
                        }
                    }
                    else if (enableScaningMode)
                    {
                        var optionName = args[i].Substring(1);
                        if (optionName.Length == 0) throw new ArgumentException(string.Format(Msg.OPTION_SYMBOL_IS_SPECEFIED, DrCmdConst.OptionStartSymbolName, DrCmdConst.EndOptionScaning, DrCmdConst.StartOptionScaning));
                        if (result.Contains(optionName)) throw new ArgumentException(string.Format(Msg.OPTION_ALREADY_SPECIFIED_EARLY, optionName, DrCmdConst.EndOptionScaning, DrCmdConst.StartOptionScaning));
                        currentOption = result.Add(optionName);                      // add new option, name is guid for exclude collision 
                        //currentOption.Attributes.Add(DrCmdCommandSettings.Name, ); // add option name without '-' to attribute 'Name'
                        continue;
                    }
                }
                // add value 
                if (currentOption == null) throw new ArgumentException(string.Format(Msg.OPTION_IS_NOT_SPECIFIED, args[i], DrCmdConst.OptionStartSymbolName));
                currentOption.Attributes.Add(args[i]); // it's option value 
            }
            return result;
        }

        #endregion Arguments Parser

        #region Settings Restrictions
        /// <summary>
        /// The function checks the correctness of the settings for all options in all specified commands.
        /// </summary>
        private void VerifySettings()
        {
            foreach (var command in Commands)
            {
                if (command.IsEnabled()) // skip disabled command
                {
                    command.VerifyComandSettings();
                }
            }
        }
        #endregion Settings Restrictions

        #region Help
        /// <summary>
        /// Save application help to attribute <see cref="DrCmdSettings.Help"/> and enum all commands and save their help to attribute <see cref="DrCmdCommandSettings.Help"/>
        /// </summary>
        internal void SaveHelp()
        {
            Settings.Attributes.Add(DrCmdSettings.Help, GetHelp(false), ResolveConflict.OVERWRITE);
            foreach (var command in Commands)
            {
                command.SaveHelp();
            }
        }

        /// <summary>
        /// If there is active command - returns help. Otherwise, returns an empty string.
        /// </summary>
        /// <returns></returns>
        public string GetHelpForActiveCommand()
        {
            return (activeCommand == null ? string.Empty : activeCommand.GetHelp());
        }


        /// <summary>
        /// Returns help for application: application description and list of commands with descriptions
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return GetHelp(false);
        }

        /// <summary>
        /// Returns help for application: application description, synopsys and descriptions for all enabled commands
        /// </summary>
        /// <param name="withSynopsis">describe options synopsis for all enabled commands</param>
        /// <returns></returns>
        public string GetHelp(bool withSynopsis)
        {
            var text = GetHelpApplicationName();
            text += "\r\n" + Msg.HELP_SYNOPSIS + "\r\n\r\n";
            text += GetHelpSynopsis();
            text += "\r\n" + Msg.HELP_DESCRIPTION + "\r\n\r\n";
            text += GetHelpCommandsDescriptions();
            if (withSynopsis)
            {
                foreach (var command in Commands)
                {
                    text += "\r\n" + command.GetHelp();
                }
            }
            return text;
        }
        /// <summary>
        /// Returns help lines contains command description like
        /// COMMAND1 - it's command 1
        /// COMMAND2 - it's command 2
        /// </summary>
        /// <returns></returns>
        private string GetHelpCommandsDescriptions()
        {
            var text = string.Empty;
            var tab = GetHelpTab();
            var maxCommandNameLength = this.GetMaximumCommandNameLength() + 1;
            var leftMargin = tab.Length + maxCommandNameLength;
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(leftMargin, GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            foreach (var command in Commands)
            {
                if (command.IsEnabled())
                {
                    text += tab + command.Name + (new string(' ', maxCommandNameLength - command.Name.Length)) +
                            builder.BuildText(command.GetCommandDescription());

                }
            }
            return text;
        }
        /// <summary>
        /// Returns help line contains application synopsis like 'Application.exe - {COMMAND1 | COMMAND2}'
        /// </summary>
        /// <returns></returns>
        private string GetHelpSynopsis()
        {
            var tab = GetHelpTab();
            var text = tab + GetSettingsApplicationName() + " ";
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(text.Length, GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            text += builder.BuildText("{" + GetCommandsNamesForHelp() + "}");
            return text;
        }
        /// <summary>
        /// Returns list of enabled commands as single string separeted by vertical bar '|'
        /// </summary>
        private string GetCommandsNamesForHelp()
        {
            var text = String.Empty;
            foreach (var command in Commands)
            {
                if (command.IsEnabled())
                {
                    if (text.Length > 0) text += " | ";
                    text += command.Name;
                }
            }
            return text;
        }

        /// <summary>
        /// Returns the maximum length of the name of the enabled commands
        /// </summary>
        /// <returns></returns>
        internal int GetMaximumCommandNameLength()
        {
            var maxLength = 0;
            foreach (var command in Commands)
            {
                if ((command.IsEnabled()) && (command.Name.Length > maxLength)) maxLength = command.Name.Length;
            }
            return maxLength;
        }
        /// <summary>
        /// Returns help line contains application name
        /// </summary>
        /// <returns></returns>
        private string GetHelpApplicationName()
        {
            var text = GetSettingsApplicationName() + " - ";
            var frmt = new TextBuilder.DrCmdTextBuilderFormat(0, GetSettingsHelpMaxLineLength(), true, true);
            var builder = new TextBuilder.DrCmdTextBuilder(frmt);
            text += builder.BuildText(GetSettingsApplicationDescription());
            return text;
        }

        /// <summary>
        /// Returns string specified space character repeated a specified number of times by setting <see cref="DrCmdSettings.HelpTabSize"/>.
        /// </summary>
        /// <returns></returns>
        internal string GetHelpTab()
        {
            return GetHelpTab(1);
        }
        /// <summary>
        /// Returns string specified space character repeated a specified number of times by setting <see cref="DrCmdSettings.HelpTabSize"/> and multiplied by a specified number of times
        /// </summary>
        /// <param name="i">Multiply the tabs to the specified number of times</param>
        /// <returns></returns>
        internal string GetHelpTab(int i)
        {
            return new string(' ', GetSettingsHelpTabSize() * i);
        }
        #endregion Help
    }
}
