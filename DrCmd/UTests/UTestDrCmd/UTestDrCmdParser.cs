/*
  DrCmdParser.cs -- Unit tests for library to parse arguments. 1.0.0, May 2, 2014
 
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
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrCmd;
using DrOpen.DrCommon.DrExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTestDrCmd
{
    [TestClass]
    public class UTestDrCmdParser
    {
        #region Common
        public enum TestValue
        {
            val1,
            val2,
            val3
        }

        public static DDNode GetInitialParametrs(params string[] args)
        {
            return GetInitialParametrs(false, false, args);
        }

        public static DDNode GetInitialParametrs(bool caseSensetive, bool ignoreUnknowArguments, params string[] args)
        {
            var ddNode = new DDNode(DrCmdConst.TypeSettings, DrCmdConst.TypeSettings);
            ddNode.Attributes.Add(DrCmdSettings.Arguments, args);
            ddNode.Attributes.Add(DrCmdSettings.CaseSensitive, caseSensetive);
            ddNode.Attributes.Add(DrCmdSettings.IgnoreUnknowArguments, ignoreUnknowArguments);
            ddNode.Add("HELP").Type=DrCmdConst.TypeCommand;
            ddNode.Add(GetCommandCOMMAND());
            ddNode.Add(GetCommandRUN());

            return ddNode;
        }

        public static DDNode GetCommandCOMMAND()
        {
            var name = "COMMAND";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "The test command.");
            cmd.Attributes.Add(DrCmdCommandSettings.Example, "Example for this command.");
            cmd.Add(CreateOptionWithName("t1", "test1", "FirstTest"));
            cmd.Add(CreateOptionWithName("test2"));
            cmd.Add(CreateOptionWithName("test3"));
            cmd.Add(CreateOptionWithName("test4"));
            return cmd;
        }

        public static DDNode GetResultCOMMANDWithoutValue()
        {
            var expected = new DDNode("COMMAND", DrCmdConst.TypeCommand);
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            return expected;
        }

        public static DDNode CreateOptionWithName(string name, params string[] aliases)
        {
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            if (aliases.Length > 0) opt.Attributes.Add(DrCmdOptionSettings.Aliases, aliases);
            return opt;
        }

        public static DDNode GetCommandRUN()
        {
            var name = "RUN";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "The test command.");
            cmd.Attributes.Add(DrCmdCommandSettings.Example, "{0} {1} -xf xml file Example for this command.");
            cmd.Add(GetOptionLogFile());
            cmd.Add(GetOptionMode());
            
            return cmd;
        }

        public static DDNode GetOptionLogFile()
        {
            var name = "LogFile";
            var opt = new DDNode(name,DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, "lf");
            opt.Attributes.Add(DrCmdOptionSettings.Description, "log file");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString(), DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.Single.ToString() });
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
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify mode. This is working mode for application. It's very very long description for value. {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.RestrictionList, new[] { "ALL", "ENABLED", "DISABLED" });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "application mode");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static void ValidateOptionParser(DDNode result, DDNode expected)
        {
            Assert.IsTrue(result.Name == expected.Name, "The result command '{0}' is not expected command name '{1}'.", result.Name, expected.Name);
            foreach (var eItem in expected.Attributes)
            {
                Assert.IsTrue(result.Attributes.Contains(eItem.Key), string.Format("Cannot found expected option with name '{0}'.", eItem.Key));
                var rValue = result.Attributes.GetValue(eItem.Key, string.Empty);
                Assert.IsTrue(eItem.Value.CompareTo(rValue) == 0, "The result is '{0}' is not equals expected '{1}' in option '{2}'.", rValue.ToString(), eItem.Value.ToString(), eItem.Key);
            }
            Assert.IsTrue(expected.Attributes.Count == result.Attributes.Count, "The result options count '{0}' is not equals expected '{1}' options count.", result.Attributes.Count, expected.Attributes.Count);
        }
        #endregion #region Common
        #region TestEmptyCommandLine
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestEmptyCommandLine")]
        public void TestEmptyCommandLine()
        {
            var root = GetInitialParametrs("");
            var cmdParser = new DrCmdParser(root);
            try
            {
                cmdParser.Parse();
            }
            catch (ArgumentException e)
            {
                /* it's ok*/
                Assert.IsNull(cmdParser.ActiveCommnand, "The empty command line must have 'null' active command.");
            }
        }
        #endregion TestEmptyCommandLine
        #region CheckDisableCommand
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("CheckDisableCommand")]
        public void TestCheckDisableCommand_CommandIsDisabled()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3", "-t1" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND").Attributes.Replace(DrCmdCommandSettings.Enabled, false);
            try
            {
                var cmdParser = new DrCmdParser(root);
                cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'command is вшыфидув'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion
        #region OptionIsSpecifiedMoreThanOnce
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("OptionIsSpecifiedMoreThanOnce")]
        public void TestOptionIsSpecifiedMoreThanOnce_SpecifyOptionTwise()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" , "-t1"};

            var root = GetInitialParametrs(args);
            try
            {
                var cmdParser = new DrCmdParser(root);
                cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'option is specified twise'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("OptionIsSpecifiedMoreThanOnce")]
        public void TestOptionIsSpecifiedMoreThanOnce_SpecifyOptionTwiseByAlias()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3", "-Test1Alias" };
            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "Test1Alias" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'option is specified twise'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion OptionIsSpecifiedMoreThanOnce
        #region DisabledOption
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestDisabledOption")]
        public void TestDisabledOption_SpecifyDisabledOption()
        {
            var args = new[] { "COMMAND", "-t1","-test2", "val1", "val2", "val3"};

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Enabled, false, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'disabled option is specified'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion DisabledOption
        #region UnknowArguments
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestCheckSettingsUnknowArguments")]
        public void TestCheckSettingsUnknowArguments_UnknowArgument()
        {
            var args = new[] { "COMMAND", "-test2", "val1", "val2", "val3", "-UnknowArgument" };

            var root = GetInitialParametrs(args);
            try
            {
                var cmdParser = new DrCmdParser(root);
                cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'unknow agument is specified'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestCheckSettingsUnknowArguments")]
        public void TestCheckSettingsUnknowArguments_AllowUnknowArguments()
        {
            var args = new[] { "COMMAND", "-UnknowArgument", "-test2", "val1", "val2", "val3"};

            var root = GetInitialParametrs(false, true, args);
            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        #endregion UnknowArguments
        #region CheckIncongruousTypeOfValue
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("CheckIncongruousTypeOfValue")]
        public void TestCheckIncongruousTypeOfValue_OptionalVsRequired()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Optional.ToString(), DrCmdValueFlags.Required.ToString() }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched - 'Incongruous types of value for option are specified: Optional and Required'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("CheckIncongruousTypeOfValue")]
        public void TestCheckIncongruousTypeOfValue_SingleVsList()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Single.ToString(), DrCmdValueFlags.List.ToString() }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched - 'Incongruous types of value for option are specified: Single and List'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        #endregion CheckIncongruousTypeOfValue
        #region ValidateOptionsIncongruous
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsIncongruous_IncongruousValueIsSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfIncongruous, new[] { "test3", "test2" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Incongruous option is specified'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsIncongruous_IncongruousValueIfIsNotSpecified()
        {
            var args = new[] { "COMMAND", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfIncongruous, new[] { "test2", "t1" }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsIncongruous_IncongruousValueIsSpecifiedByAlias()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfIncongruous, new[] { "test2", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);

            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Incongruous option is specified by alias'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsIncongruous_IncongruousLinkToNotExistOption()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfIncongruous, new[] { "null", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);

            try
            {
                var cmdParser = new DrCmdParser(root);

                Assert.Fail("The incorrect arguments is not catched - 'Incongruous option is linked to not exist option'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsIncongruous_IncongruousLinkToDisabledOption()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfIncongruous, new[] { "test3", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test3").Attributes.Add(DrCmdOptionSettings.Enabled, false, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);

                Assert.Fail("The incorrect arguments is not catched - 'Incongruous option is linked to disabled option'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        #endregion ValidateOptionsIncongruous
        #region ValidateOptionsDependency
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestValidateOptionsDependency_DependentValueIsNotSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfDependency, new[] { "test2", "test3" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Dependent option is not specified'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestValidateOptionsDependency_DependencyValueIfIsNotSpecified()
        {
            var args = new[] { "COMMAND", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfDependency, new[] { "test2", "t1" }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestValidateOptionsDependency_DependentValueIsSpecifiedByAlias()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfDependency, new[] { "test2", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestValidateOptionsDependency_DependencyLinkToNotExistOption()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfDependency, new[] { "null", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);

            try
            {
                var cmdParser = new DrCmdParser(root);

                Assert.Fail("The incorrect arguments is not catched - 'Dependency option is linked to not exist option'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateOptionsIncongruous")]
        public void TestValidateOptionsDependency_DependencyLinkToDisabledOption()
        {
            var args = new[] { "COMMAND", "-t1", "-t2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.TermsOfDependency, new[] { "test3", "t1" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T2" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test3").Attributes.Add(DrCmdOptionSettings.Enabled, false, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);

                Assert.Fail("The incorrect arguments is not catched - 'Dependency option is linked to disabled option'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        #endregion ValidateOptionsDependency
        #region VerifyValueRestrictionsType
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestVerifyValueRestrictionsType_ForbiddenValueTypeWithAnotherType()
        {
            var args = new[] { "COMMAND", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Forbidden.ToString(), DrCmdValueFlags.Optional.ToString() }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched - 'Forbidden value flag is specified with another flag'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestVerifyValueRestrictionsType_ForbiddenValueType()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Forbidden.ToString() }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("t1", true);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestVerifyValueRestrictionsType_AllowNumericWithOutRestrictionList()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched - 'AllowNumeric value flag is specified without RestrictionList flag'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException e)
            {/* it's ok*/}
        }
        #endregion VerifyValueRestrictionsType
        #region RequiredOption
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRequiredOption_IsNotSpecified()
        {
            var args = new[] { "COMMAND", "-test2", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Required option is not specified'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRequiredOption_SpecifiedByAlias()
        {
            var args = new[] { "COMMAND", "-alias", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        #endregion RequiredOption
        #region SingleValue
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestSingleValue_IsNotSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Single.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test3").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Single.ToString(), ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", new[] { "value for test 3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);


        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestSingleValue_SpecifiedMoreThanOne()
        {
            var args = new[] { "COMMAND", "-alias", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Single.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Sigle value has more than one values'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion SingleValue
        #region ForbiddenValue
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestForbiddenValue_IsNotSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "-test2", "val1", "val2", "val3", "-test3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Forbidden.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test3").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Forbidden.ToString(), ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", true);
            expected.Attributes.Add("test2", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test3", true);
            expected.Attributes.Add("test4", String.Empty);
            ValidateOptionParser(result, expected);
        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestForbiddenValue_IsSpecifiedOnce()
        {
            var args = new[] { "COMMAND", "-alias", "val1", "-test2", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Forbidden.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Forbidden value cannot have value'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestForbiddenValue_SpecifiedMoreThanOne()
        {
            var args = new[] { "COMMAND", "-alias", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Forbidden.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Forbidden value has value'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion ForbiddenValue
        #region RequiredValue
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRequiredValue_IsSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test3").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "value for test 3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRequiredValue_IsSpecifiedByAlias()
        {
            var args = new[] { "COMMAND", "-alias", "val1", "val2", "val3", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "value for test 3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRequiredValue_IsNotSpecified()
        {
            var args = new[] { "COMMAND", "-alias" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, DrCmdOptionType.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "alias", ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched - 'Required value without value'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException e)
            {/* it's ok*/}
        }
        #endregion RequiredValue
        #region RestrictionList
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_AsNumericIncorrectType()
        {
            var args = new[] { "COMMAND", "-t1", "Val1", "vAl2", "val3", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, new byte[] { 0x1, 0x2, 0x3 }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect argument flag is not catched - 'RestrictionListAsNumeric flag is byte[]'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException)
            {/* it's ok*/}


        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueIsSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "Val1", "vAl2", "val3", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "Val1", "vAl2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "value for test 3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueIsSpecifiedCaseSensitive()
        {
            var args = new[] { "COMMAND", "-t1", "Val1", "vAl2", "val3", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(true, false, args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);

            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect argument flag is not catched - 'RestrictionList, specified value is incorrected because case sensitive is enabled'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueNumericIsSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "123232", "777", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "123232", "777" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "value for test 3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueNegativeNumericIsSpecified()
        {
            var args = new[] { "COMMAND", "-t1", "--", "-123232", "-+", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString(), DrCmdValueFlags.AllowNumeric.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect argument flag is not catched - 'RestrictionList, specified value is negative number'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueNumericIsNotAllow()
        {
            var args = new[] { "COMMAND", "-t1", "1", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.ListOfRestriction.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect argument flag is not catched - 'RestrictionList, specified value is number but number is not allowed in the ValueFlags for this option'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (ArgumentException)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_ValueNumericAndAllowNumericHaveDifferentElementCount()
        {
            var args = new[] { "COMMAND", "-t1", "1", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.AllowNumeric.ToString(), DrCmdValueFlags.ListOfRestriction.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, new int[] { 1, 2 }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect argument flag is not catched - 'Number of items in lists RestrictionList and RestrictionListAsNumeric is different.'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException)
            {/* it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("ValidateCommandParameters")]
        public void TestRestrictionList_RestrictionDescriptionListWithDifferentElementCount()
        {
            var args = new[] { "COMMAND", "-t1", "1", "-test2", "-test3", "value for test 3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, DrCmdValueFlags.Required.ToString(), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.AllowNumeric.ToString(), DrCmdValueFlags.ListOfRestriction.ToString() }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionList, Enum.GetNames(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListAsNumeric, DrExtEnum.GetFlags(typeof(TestValue)), ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.RestrictionListDescription, new []{ "A", "B" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect argument flag is not catched - 'Number of items in lists RestrictionList and RestrictionListDescription is different.'.");
            }
            catch (AssertFailedException e)
            { throw; }
            catch (FormatException)
            {/* it's ok*/}
        }
        #endregion RestrictionList
        #region OptionType

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetOptionType")]
        public void TestGetOptionType()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString(), "Incorrect" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect option flag is not catched.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {
                // it's ok
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetOptionType")]
        public void TestGetOptionTypeCaseInsensitive()
        {
            var args = "RuN -LogFile val1".Split(new[] { ' ' });

            var root = GetInitialParametrs(args);
            root.GetNode("RUN/LogFile").Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString().ToUpper(), DrCmdOptionType.Required.ToString().ToLower() }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("RUN");
            expected.Attributes.Add("LogFile", new[] { "val1" });
            expected.Attributes.Add("m", String.Empty );
            ValidateOptionParser(result, expected);
        }

        #endregion OptionType
        #region ValueFlags

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetValueFlags")]
        public void TestGetValueType()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), "Incorrect" }, ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect option flag is not catched.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (Exception)
            {
                // it's ok
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetValueFlags")]
        public void TestGetValueTypeCaseInsensitive()
        {
            var args = "RuN -LogFile val1".Split(new[] { ' ' });

            var root = GetInitialParametrs(args);
            root.GetNode("RUN/LogFile").Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString().ToUpper(), DrCmdValueFlags.Required.ToString().ToLower() }, ResolveConflict.OVERWRITE);

            var cmdParser = new DrCmdParser(root);
            var result = cmdParser.Parse();
            var expected = new DDNode("RUN");
            expected.Attributes.Add("LogFile", new[] { "val1" });
            expected.Attributes.Add("m", String.Empty);
            ValidateOptionParser(result, expected);
        }

        #endregion ValueFlags
        #region VerifyComandSettings
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsDublicateOptionNameCaseInsensitive()
        {
            var args = new[] { "CoMMAND", "-t1", "val1", "val2", "val3" };
            var additionalName = "Test2";

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND").Add(additionalName, DrCmdConst.TypeOption).Attributes.Add(DrCmdOptionSettings.Name, additionalName);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched 'Command name is not specified.'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (FormatException e)
            {/*it's ok*/}

        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsDublicateOptionNameCaseSensitive()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };
            var additionalName = "Test2";

            var root = GetInitialParametrs(true, false, args);
            root.GetNode("COMMAND").Add(additionalName, DrCmdConst.TypeOption).Attributes.Add(DrCmdOptionSettings.Name, additionalName);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add(additionalName, string.Empty);
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsDublicateOptionAliasCaseSensitive()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(true, false, args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "Test2", "Dublicate" }, ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "T1", "DUBLICATE" }, ResolveConflict.OVERWRITE);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsDublicateOptionAliasCaseInsensitiveA()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "Test2", ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched 'Dublicate alias name .'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (FormatException e)
            {
                //it's ok
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsDublicateOptionAliasCaseSensitiveB()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(true, false, args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Aliases, "dublicate", ResolveConflict.OVERWRITE);
            root.GetNode("COMMAND/test2").Attributes.Add(DrCmdOptionSettings.Aliases, "dublicate", ResolveConflict.OVERWRITE);
            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect arguments is not catched 'Dublicate alias name .'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (FormatException e)
            {/*it's ok*/}
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("VerifyComandSettings")]
        public void TestVerifyComandSettingsIncorrectOptionType()
        {
            var args = new[] { "COMMAND", "-t1", "val1", "val2", "val3" };

            var root = GetInitialParametrs(true, false, args);
            root.GetNode("COMMAND/t1").Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Optional.ToString(), DrCmdOptionType.Required.ToString() }, ResolveConflict.OVERWRITE);

            try
            {
                var cmdParser = new DrCmdParser(root);
                Assert.Fail("The incorrect format is not catched 'Option has both flag Optional and Required'.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (FormatException e)
            {/*it's ok*/}
        }


        #endregion VerifyComandSettings
        #region GetSelectedCommandNameFromArguments
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetSettingsArguments")]
        public void TestGetSettingsArgumentsSupportSpaceInValue()
        {
            var args = new[] { "\"CoMMAND\"", "-t1", "-test2", "-test3", "\"val1 val2 val3\"", "-test4" };
            var root = GetInitialParametrs(args);
            root.Attributes.Add(DrCmdSettings.RemoveStartEndQuotas, true, ResolveConflict.OVERWRITE);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "val1 val2 val3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetSettingsArguments")]
        public void TestGetSettingsArgumentsSupportOnlySpaceOrEmptyStringInValue()
        {
            var args = new[] { "\"CoMMAND\"", "-t1", "-test2", "-test3", "\" \"", "-test4", "\"\"" };
            var root = GetInitialParametrs(args);
            root.Attributes.Add(DrCmdSettings.RemoveStartEndQuotas, true, ResolveConflict.OVERWRITE);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { " " });
            expected.Attributes.Add("test4", new string[] { });
            ValidateOptionParser(result, expected);

        }
        #endregion GetSelectedCommandNameFromArguments
        #region GetSelectedCommandNameFromArguments
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetSelectedCommandNameFromArguments")]
        public void TestGetSelectedCommandNameFromArgumentsCommnadNameEmptyException()
        {
            var root = GetInitialParametrs("\"\"");
            var cmdParser = new DrCmdParser(root);
            root.Attributes.Add(DrCmdSettings.RemoveStartEndQuotas, true, ResolveConflict.OVERWRITE);
            try
            {

                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched 'Command name is double quotas.'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException nullExc)
            {
                // it's ok
            }

        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("GetSelectedCommandNameFromArguments")]
        public void TestGetSelectedCommandNameFromArgumentsCommnadNameIsNotSpecifiedException()
        {
            var root = GetInitialParametrs(string.Empty);
            var cmdParser = new DrCmdParser(root);
            try
            {

                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched 'Command name is not specified.'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException nullExc)
            {
                // it's ok
            }

        }
        #endregion GetSelectedCommandNameFromArguments
        #region TestSplitOptionsAndTheirValuesByArguments
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsCommnadNameOnlyValidIgnoreCase()
        {
            var args = "command";
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();
            ValidateOptionParser(result, GetResultCOMMANDWithoutValue());

        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsCommnadNameOnlyIncorrectIgnoreCase()
        {
            var args = "c0mmand";
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("The incorrect arguments is not catched 'Command name is incorrect.'");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException argExc)
            {
                // it's true
            }
        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsCommnadNameOnlyValidCaseSensetive()
        {
            var args = "COMMAND";
            var root = GetInitialParametrs(true, false, args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            ValidateOptionParser(result, GetResultCOMMANDWithoutValue());


        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsCommnadNameOnlyIncorrectCaseSensetive()
        {
            var args = "CoMMAND";
            var root = GetInitialParametrs(true, false, args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("Case sensetive parameter doesnt' work correctly.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException argExc)
            {
                // it's true
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsCheckThrowValueWithoutOption()
        {
            var args = "CoMMAND test".Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("Contains the value of the option without declaring itself options.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException argExc)
            {
                // it's true
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsOptionSpecifyTheOptionTwice()
        {
            var args = "CoMMAND -t1 -test2 -test3 val1 val2 val3 -test2 -t1".Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("Missed the opportunity to specify the option twice.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException argExc)
            {
                // it's true
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsOptionSymbolName()
        {
            var args = string.Format("CoMMAND -t1 -test2 {0} test3 val1 val2 val3 -test4", DrCmdConst.OptionStartSymbolName).Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail(string.Format("Option symbol name '{0}' is not catched.", DrCmdConst.OptionStartSymbolName));
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException argExc)
            {
                // it's true
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsParseMultipleOptions()
        {
            var args = ("CoMMAND -t1 -test2 -test3 val1 val2 val3 -test4").Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", string.Empty);
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsParseStopScanWithoutStartOption()
        {
            var args = ("CoMMAND -- -t1 -test2 -test3 val1 val2 val3 -test4").Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);
            try
            {
                var result = cmdParser.Parse();
                Assert.Fail("Contains the value of the option without declaring itself options.");
            }
            catch (AssertFailedException e)
            {
                throw;
            }
            catch (ArgumentException exc)
            {
                // it's ok
            }
        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsParseStopScan()
        {
            var args = ("CoMMAND -t1 -- -test2 -test3 val1 val2 val3 -test4").Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "-test2", "-test3", "val1", "val2", "val3", "-test4" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsParseStopStartScan()
        {
            var args = ("CoMMAND -t1 -- -test2 -test3 -+ val1 val2 val3 -test4").Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "-test2", "-test3", "val1", "val2", "val3" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", string.Empty);
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }

        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsOptionParseStartSymbolNameAsValue()
        {
            var args = string.Format("CoMMAND -t1 -- {0} -+ -test3 val1 val2 val3 -test4", DrCmdConst.OptionStartSymbolName).Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "-" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsOptionParseStopScanAsValue()
        {
            var args = string.Format("CoMMAND -t1 {0} {0} -+ -test3 val1 val2 val3 -test4", DrCmdConst.EndOptionScaning).Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "--" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);

        }
        [TestMethod, TestCategory("DrCmdParser"), TestCategory("TestSplitOptionsAndTheirValuesByArguments")]
        public void TestSplitOptionsAndTheirValuesByArgumentsOptionParseStartScanAsValue()
        {
            var args = string.Format("CoMMAND -t1 {0} -test3 val1 val2 val3 -test4", DrCmdConst.StartOptionScaning).Split(new[] { ' ' });
            var root = GetInitialParametrs(args);
            var cmdParser = new DrCmdParser(root);

            var result = cmdParser.Parse();

            var expected = new DDNode("COMMAND");
            expected.Attributes.Add("t1", new[] { "-+" });
            expected.Attributes.Add("test2", string.Empty);
            expected.Attributes.Add("test3", new[] { "val1", "val2", "val3" });
            expected.Attributes.Add("test4", string.Empty);
            ValidateOptionParser(result, expected);
        }

        #endregion TestSplitOptionsAndTheirValuesByArguments
    }
}
