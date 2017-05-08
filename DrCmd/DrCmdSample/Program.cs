using System;
using System.IO;
using System.Xml.Serialization;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrExt;
using DrOpen.DrCommon.DrDataSx;

namespace DrOpen.DrCommon.DrCmd
{
    class Program
    {

        [Flags]
        public enum LogLevel
        {
            /// <summary>
            /// ничего не логировать
            /// </summary>
            NONE = 0,
            ERR = 1,
            WAR = 2,
            INF = 4,
            TRC = 8,
            TRC2 = 16,
            /// <summary>
            /// только error, warning and information, исключая stack
            /// </summary>
            ALL_INFO = ERR | WAR | INF,
            ALL_TRACE = ALL_INFO | TRC,
            /// <summary>
            /// все сообщения
            /// </summary>
            ALL = ALL_TRACE | TRC2
        }

        static void Main(string[] args)
        {

            var ddNode = new DDNode(DrCmdConst.TypeSettings, new DDType (DrCmdConst.TypeSettings));
            ddNode.Attributes.Add(DrCmdSettings.ApplicationDescription, "It's test application.");
            ddNode.Add(GetCommandHelp());
            ddNode.Add(GetCommandRUN());
            

            var cmd = new DrCmdParser(ddNode);

            Console.Write( cmd.GetHelp(true));
            Console.WriteLine();
            do
            {
                Console.WriteLine("Specify arguments: ");
                var line = Console.ReadLine();
                ddNode.Attributes.Add(DrCmdSettings.Arguments, line.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries), ResolveConflict.OVERWRITE);
                SerialyzeToXml(ddNode);

            } while (!ParseSample(cmd));
            
        }

        private static void SerialyzeToXml(DDNode node)
        {
            using (FileStream file = new FileStream("Sample.xml", FileMode.Create, FileAccess.Write))
            {

                var serializer = new XmlSerializer(typeof(DDNodeSx));
                serializer.Serialize(file, ((DDNodeSx)node));
                file.Flush();
            }


        }

        private static bool ParseSample(DrCmdParser cmd)
        {
            try
            {
                var res =cmd.Parse();
                Console.WriteLine();
                Console.Write(res.Name );
                if (res.HasAttributes) Console.Write(": ");
                foreach (var a in res.Attributes)
                {

                    Console.Write(a.Key + "='" + a.Value + "', ");
                }
                Console.WriteLine();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine( GetExceptionAsString(e));
                return false;
            }   
        }

        private static string GetExceptionAsString(Exception e)
        {
            var text = e.Message;
            if (e.InnerException != null) text += " --> (" + GetExceptionAsString(e.InnerException) + ")";
            return text;
        }

        public static DDNode GetCommandHelp()
        {
            var name = "HELP";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "Provides help information");
            
            return cmd;
        }

        public static DDNode GetCommandRUN()
        {
            var name = "RUN";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "This is the long description for the test team showing how to construct a description of the command to output to the console. Its length allows you to look at the new line, as it is formed.");
            cmd.Attributes.Add(DrCmdCommandSettings.Example, new[] { "{0} {1} -xf xml file\r\nExample for this command.", "{0} {1} -xf xml file -lf log file spcified -mode select mode for this application -ll log level\r\nIt is very very long descriptions for this example. Multilines! Here very long description for the given example. Should be a new line, a new line.", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "e10" });

            cmd.Add(GetOptionLogFile());
            cmd.Add(GetOptionLogLevelFile());
            
            cmd.Add(GetOptionXmlFile());
            cmd.Add(GetOptionMode());
            
            return cmd;
        }

        public static DDNode GetOptionLogFile()
        {
            var name = "lf";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] {"log"});
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify path and name for log file.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString(), DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "log file");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionLogLevelFile()
        {
            var name = "ll";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            //opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] {"log"});
            opt.Attributes.Add(DrCmdOptionSettings.Description, "level of logging to a log file, the default value is: '{4}'. The follow values or its numeric equivalent is allowed: '{7}'. This option is used in conjunction with option '-{10}' only");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString()});
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Optional.ToString(), DrCmdValueFlags.Single.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.DefaultValueIfNoneSpecified, new[] { LogLevel.ALL_INFO.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames( typeof(LogLevel) ));
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(LogLevel)));
            opt.Attributes.Add(DrCmdOptionSettings.TermsOfDependency, "lf");
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "log file level");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionXmlFile()
        {
            var name = "xf";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "x", "xml" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify path and name for xml file. This file should be contains the all settings for application. It's very very long description for value .");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString()});
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "xml file");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionMode()
        {
            var name = "m";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "mode" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify mode. This is working mode for application. It's very very long description for value . {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionList, new[] { "ALL","ENABLED", "DISABLED" });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "application mode");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

    }


}
