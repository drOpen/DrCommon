using DrOpen.DrCommon.DrCmd;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrExt;
using DrOpen.DrCommon.DrLog.DrLogClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrSignSample
{
    class Program
    {

        public const int ExitCodeOK = 0;
        public const int ExitCodeInvalidCommand = 1;
        public const int ExitCodeInvalidCommandOptions = 2;
        public const int ExitCodeHelpInfo = 4;

        public const string CmdCommandNameHelp = "HELP";
        public const string CmdCommandNameTest = "TEST";
        public const string CmdCommandNameSign = "SIGN";
        public const string CmdCommandNameSignFile = "SIGNFILE";
        public const string CmdCommandNameStat = "STAT";

        static int Main(string[] args)
        {

            var ddNode = new DDNode(DrCmdConst.TypeSettings, DrCmdConst.TypeSettings);
            ddNode.Attributes.Add(DrCmdSettings.ApplicationDescription, "It's test application.");
            ddNode.Add(GetCommandHelp());
            ddNode.Add(GetCommandTest());
            ddNode.Add(GetCommandSignFile());

            var cmd = new DrCmdParser(ddNode);

            ddNode.Attributes.Add(DrCmdSettings.Arguments, args, ResolveConflict.OVERWRITE);

            try
            {
                var res = cmd.Parse();
                if (string.Compare(res.Name, CmdCommandNameHelp, true) == 0)
                {
                    Console.Write(cmd.GetHelp(true)); // show full help
                    return ExitCodeHelpInfo;
                }

            }
            catch (Exception e)
            {
                if (cmd.ActiveCommnand == null)
                {
                    if ((args != null) && (args.Length > 0))
                    {
                        Console.WriteLine(GetExceptionAsString(e)); // if command line is empty
                        Console.WriteLine();
                    }
                    Console.Write(cmd.GetHelp(false)); // show command help only
                    return ExitCodeInvalidCommand;
                }

                bool isSpecifiedCmdHelp = (cmd.ActiveCommnand.Contains("?") ? cmd.ActiveCommnand.GetNode("?").Attributes.GetValue(DrCmdOptionSettings.ResultIsOptionSpecified, false).GetValueAsBool() : false); // if specified option '-?' for active command

                if (!isSpecifiedCmdHelp)
                {
                    Console.WriteLine(GetExceptionAsString(e));
                    Console.WriteLine();
                    Console.Write(cmd.GetHelpForActiveCommand()); // show active command help only
                    return ExitCodeInvalidCommandOptions;
                }
                Console.Write(cmd.GetHelpForActiveCommand()); // show active command help only
                return ExitCodeHelpInfo;
            }
            return ExitCodeOK;
        }

        private static string GetExceptionAsString(Exception e)
        {
            if (e.InnerException != null) return e.Message + " (" + GetExceptionAsString(e.InnerException) + ")";
            return e.Message;
        }

        #region commands

        public static DDNode GetCommandTest()
        {
            var cmd = new DDNode(CmdCommandNameTest, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, CmdCommandNameTest);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "Validates the availability of a list of signature servers.");
            #region RequiredOptions
            cmd.Add(GetOptionUrls());
            #endregion RequiredOptions
            #region OptionalOptions

            cmd.Add(GetOptionTimeOut());
            cmd.Add(GetOptionConsoleLogLevel());
            cmd.Add(GetOptionFileLog());
            cmd.Add(GetOptionFileLogLevel());

            cmd.Add(GetOptionHelp());

            #endregion OptionalOptions
            return cmd;
        }

        public static DDNode GetCommandSignFile()
        {
            var cmd = new DDNode(CmdCommandNameSignFile, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, CmdCommandNameTest);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "Sign a single file.");
            cmd.Attributes.Add(DrCmdCommandSettings.Example,
                new[] { 
                    "{0} {1} -u https://srv1/page.asm -sf MyProject.exe -a /a\r\nconnect to sign server 'srv1' and sign the single file 'MyProject.exe'. Select option '/a' for the best signing cert automatically. The source file 'MyProject.exe' will be overwritten after the file will be signed.", 
                    "{0} {1} -u https://srv1/page.asm -sf MyProject.exe -tf MyProjectSigned.exe -odf -lf log.txt -fll ALL -a /a /t http://timespamp.com \r\nconnect to sign server 'srv1' and sign the single file 'MyProject.exe' and then save signed file as 'MyProjectSigned.exe'. Existing 'MyProjectSigned.exe' file will be overwritten. Logging to file 'log.txt' is enabled."
                });

            #region RequiredOptions
            cmd.Add(GetOptionUrl());
            cmd.Add(GetOptionSourceFile());
            cmd.Add(GetOptionSignArguments());


            #endregion RequiredOptions
            #region OptionalOptions
            cmd.Add(GetOptionDestinationFile());
            cmd.Add(GetOptionOverwriteDestinationFile());

            cmd.Add(GetOptionTimeOut());
            cmd.Add(GetOptionMaxThread());
            cmd.Add(GetOptionBlockSize());

            cmd.Add(GetOptionConsoleLogLevel());
            cmd.Add(GetOptionFileLog());
            cmd.Add(GetOptionFileLogLevel());

            cmd.Add(GetOptionHelp());

            #endregion OptionalOptions
            return cmd;
        }


        public static DDNode GetCommandHelp()
        {

            var cmd = new DDNode(CmdCommandNameHelp, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, CmdCommandNameHelp);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "Provides full information about program and its options.");

            return cmd;
        }

        #endregion commands

        #region options

        public static DDNode GetOptionHelp()
        {
            var name = "?";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "h", "help" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "show help information for this command.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Forbidden.ToString() }); // without value
            return opt;
        }

        public static DDNode GetOptionUrl()
        {
            var name = "u";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "url" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "server for sign");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "https://srv1/page.asm");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }



        public static DDNode GetOptionSignArguments()
        {
            var name = "a";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "source" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "arguments for signtool. based on a signing certificate. This option allow you to specify signing parameters and to select the signing certificate you wish to use.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.List.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "sign options");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");

            return opt;
        }

        public static DDNode GetOptionSourceFile()
        {
            var name = "sf";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "source" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "file for sign. The source file will be overwritten after the file will be signed.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "test.exe");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");

            return opt;
        }

        public static DDNode GetOptionDestinationFile()
        {
            var name = "df";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "save a signed file as different file. This option is used to keep the source file is not signed.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "signed.exe");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionOverwriteDestinationFile()
        {
            var name = "odf";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "allow overwrite existing destination file. By default, the existing file will not be overwritten and will be skipped. This option is used in conjunction with option '-{10}' only.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Forbidden.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.TermsOfDependency, "df");

            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionUrls()
        {
            var name = "u";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "url" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "server list for sign separated by spaces");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.List.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "https://srv1/page.asm https://srv2/page.asm");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionTimeOut()
        {
            var name = "t";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "timeout" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify the timeout value in seconds for a data connection associated with sign server. The default value is '{4}' infinitely.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, 0);
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "timeout");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionMaxThread()
        {
            var name = "mt";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "The maximum number of threads. The default value is '{4}'.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, 10);
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "max thread");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionBlockSize()
        {
            var name = "bs";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "timeout" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "Size of transmission block in kilobytes. By default, '{4}' kB.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() }); // required and list
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, 1024);
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "block size");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionConsoleLogLevel()
        {
            var name = "cll";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "level of console logging, the default value is: '{4}'. The follow values or its numeric equivalent is allowed: '{7}'.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString(), DrCmdValueType.ListOfRestriction.ToString(), DrCmdValueType.AllowNumeric.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, new[] { LogLevel.INFO.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(LogLevel)));
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(LogLevel)));
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "console log level");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionFileLog()
        {
            var name = "lf";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify path and name for log file.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString(), DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "log file");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionFileLogLevel()
        {
            var name = "fll";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Description, "level of logging to a log file, the default value is: '{4}'. The follow values or its numeric equivalent is allowed: '{7}'. This option is used in conjunction with option '-{10}' only");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueType, new[] { DrCmdValueType.Required.ToString(), DrCmdValueType.Single.ToString(), DrCmdValueType.ListOfRestriction.ToString(), DrCmdValueType.AllowNumeric.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, new[] { LogLevel.ALL.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(LogLevel)));
            opt.Attributes.Add(DrCmdOptionSettings.TermsOfDependency, "lf");
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(LogLevel)));
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "file log level");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        #endregion options

    }
}
