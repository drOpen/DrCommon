/*
  UTestDrCmdTextBuilder.cs -- Unit tests for text builder of library to parse arguments. 1.0.0, May 2, 2014
 
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrOpen.DrCommon.DrCmd.TextBuilder;

namespace UTestDrCmd
{

    [TestClass]
    public class UTestDrCmdTextBuilder
    {
        private const string textSample1 =
            "This software is provided \"as-is\", without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.  Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:";

        public static readonly string[] wordsSample10x10 = new string[]
                {
                    "0123456789", "0123456789", "0123456789", "0123456789",
                    "0123456789", "0123456789", "0123456789", "0123456789", "0123456789", "0123456789"
                };

        public static readonly string[] wordsSample10xFrom10To1 = new string[]
                {
                    "0123456789", "012345678", "01234567", "0123456",
                    "012345", "01234", "0123", "012", "01", "0", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L"
                };

        #region RemoveDoubleSpace

        [TestMethod]
        public void TestRemoveDoubleSpaceStringEmpty()
        {
            TestRemoveDoubleSpace("", "");
        }

        [TestMethod]
        public void TestRemoveDoubleSpaceSingleChar()
        {
            TestRemoveDoubleSpace("A", "A");
        }

        [TestMethod]
        public void TestRemoveDoubleSpaceSingleSpace()
        {
            TestRemoveDoubleSpace(" ", " ");
        }

        [TestMethod]
        public void TestRemoveDoubleSpaceDoubleSpace()
        {
            TestRemoveDoubleSpace("  ", " ");
        }

        [TestMethod]
        public void TestRemoveDoubleSpaceTripleSpace()
        {
            TestRemoveDoubleSpace("   ", " ");
        }

        [TestMethod]
        public void TestRemoveDoubleSpaceString1()
        {
            TestRemoveDoubleSpace(" Test1 Test2  Test3         ", " Test1 Test2 Test3 ");
        }

        public static void TestRemoveDoubleSpace(string original, string expected)
        {
            var actual = DrCmdTextBuilder.RemoveDoubleSpace(original);
            Assert.AreEqual(expected, actual,
                            string.Format("Actual text '{0}' not equals expected '{1}'.", actual, expected));
        }


        #endregion RemoveDoubleSpace

        #region NormalyzeText

        [TestMethod]
        public void TestTestNormalyzeTextEmpty()
        {
            TestNormalyzeText("", "");
        }

        [TestMethod]
        public void TestTestNormalyzeTextSpaces()
        {
            TestNormalyzeText("       ", "");
        }

        [TestMethod]
        public void TestTestNormalyzeTextNewLine()
        {
            TestNormalyzeText("  \r\n\t\0     ", "");
        }

        [TestMethod]
        public void TestTestNormalyzeTextString1()
        {
            TestNormalyzeText(" Test\t\t1 Test\r\r\n\n\t\r\02 \0 Test    3         ", "Test 1 Test 2 Test 3");
        }


        public static void TestNormalyzeText(string original, string expected)
        {
            var actual = DrCmdTextBuilder.NormalyzeText(original);
            Assert.AreEqual(expected, actual,
                            string.Format("Actual text '{0}' not equals expected '{1}'.", actual, expected));
        }

        #endregion NormalyzeText

        #region NormalyzeTextArray

        [TestMethod]
        public void TestTestNormalyzeTextArrayEmpty()
        {
            TestNormalyzeTextArray(new[] { "" }, new[] { "" });
        }

        [TestMethod]
        public void TestTestNormalyzeTextArraySpaces()
        {
            TestNormalyzeTextArray(new[] { "       " }, new[] { "" });
        }

        [TestMethod]
        public void TestTestNormalyzeTextArrayNewLine()
        {
            TestNormalyzeTextArray(new[] { "  \r\n\t\0     " }, new[] { "" });
        }

        [TestMethod]
        public void TestTestNormalyzeTextArrayString1()
        {
            TestNormalyzeTextArray(new[] { " Test\t\t1 Test\r\r\n\n\t\r\02 \0 Test    3         " },
                                   new[] { "Test 1 Test 2 Test 3" });
        }

        [TestMethod]
        public void TestTestNormalyzeTextArrayString2()
        {
            TestNormalyzeTextArray(
                new[] { " Test\t\t1 Test\r\r\n\n\t\r\02 \0 Test    3         ", "  \r\n\t\0     ", "" },
                new[] { "Test 1 Test 2 Test 3", "", "" });
        }

        public static void TestNormalyzeTextArray(string[] original, string[] expected)
        {
            var actual = DrCmdTextBuilder.NormalyzeTextArray(original);
            Assert.IsTrue(CompareStringArray(expected, actual),
                          string.Format("Actual text '{0}' not equals expected '{1}'.", actual, expected));
        }

        #endregion NormalyzeTextArray

        #region GetEndIndexForNewLine


        public static DrCmdTextBuilderFormat GetFormat80WithoutLeftMarginJustifyIsFalse()
        {
            return new DrCmdTextBuilderFormat(0, 80);
        }

        public static DrCmdTextBuilderFormat GetFormat80With10LeftMarginJustifyIsFalse()
        {
            return new DrCmdTextBuilderFormat(10, 80);
        }

        public static DrCmdTextBuilderFormat GetFormat80With10LeftMarginJustifyIsTrue()
        {
            return new DrCmdTextBuilderFormat(10, 80, true, false);
        }

        [TestMethod]
        public void TestGetElementCountForNewLineEmptyString()
        {
            TestGetElementCountForNewLine(new string[] { }, GetFormat80WithoutLeftMarginJustifyIsFalse(), 0);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10FromStartWithoutLeftMargin()
        {
            TestGetElementCountForNewLine(wordsSample10x10, GetFormat80WithoutLeftMarginJustifyIsFalse(), 0);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10xFrom10To1FromStartWithoutLeftMargin()
        {
            TestGetElementCountForNewLine(wordsSample10xFrom10To1, GetFormat80WithoutLeftMarginJustifyIsFalse(), 0);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10FromMiddle()
        {
            TestGetElementCountForNewLine(wordsSample10x10, GetFormat80WithoutLeftMarginJustifyIsFalse(), 2);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10FromMiddleWith10LeftMargin()
        {
            TestGetElementCountForNewLine(wordsSample10x10, GetFormat80With10LeftMarginJustifyIsFalse(), 2);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10FromBeginWith10LeftMargin()
        {
            TestGetElementCountForNewLine(wordsSample10x10, GetFormat80With10LeftMarginJustifyIsFalse(), 0);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10EndArray()
        {
            TestGetElementCountForNewLine(wordsSample10x10, GetFormat80With10LeftMarginJustifyIsFalse(), 9);
        }

        [TestMethod]
        public void TestGetElementCountForNewLine10x10OneWord()
        {
            TestGetElementCountForNewLine(new string[] { "A" }, GetFormat80With10LeftMarginJustifyIsFalse(), 0);
        }

        public static void TestGetElementCountForNewLine(string[] words, DrCmdTextBuilderFormat format,
                                                         int startIndex)
        {
            var actual = DrCmdTextBuilder.GetElementCountForNewLine(startIndex, format, words);
            var length = 0;
            int expected = startIndex;
            while (expected < words.Length)
            {
                length += words[expected].Length + 1;
                expected++;
                if ((expected < words.Length) && (length + words[expected].Length > format.Length)) break;
            }
            expected = expected - startIndex;
            Assert.AreEqual(expected, actual,
                            string.Format("Actual element count'{0}' not equals expected '{1}'.", actual, expected));
            var lineLenght = String.Join(" ", words, startIndex, actual);
            Assert.IsTrue(lineLenght.Length <= format.Length,
                          string.Format("Actual text '{0}' not equals format.lenght '{1}'.", lineLenght,
                                        format.Length));
            // The next element can be joined to line.
            if (expected + startIndex + 1 < words.Length)
            {
                Assert.IsFalse(
                    ((lineLenght.Length + 1 + words[startIndex + expected + 1].Length) <= format.Length),
                    "The next element can be joined to line.");
            }

        }

        #endregion GetEndIndexForNewLine

        [TestMethod]
        public void TestBuildTextJustifyFalse()
        {
            var marginLeft = 10;
            var textWidth = 80;
            var justify = false;

            var drTextFormat = new DrCmdTextBuilderFormat(marginLeft, textWidth, justify, false);
            var drCmdTextBuilder = new DrCmdTextBuilder(drTextFormat);
            var result = drCmdTextBuilder.BuildText(textSample1);
            ValidateFormatedText(drTextFormat, textSample1, result);
        }

        [TestMethod]
        public void TestBuildTextJustifyTrue()
        {
            var marginLeft = 10;
            var textWidth = 80;
            var justify = true;
            
            var drTextFormat = new DrCmdTextBuilderFormat(marginLeft, textWidth, justify, false);
            var drCmdTextBuilder = new DrCmdTextBuilder(drTextFormat);
            var result = drCmdTextBuilder.BuildText(textSample1);
            ValidateFormatedText(drTextFormat, textSample1, result);
        }

        public void ValidateFormatedText(DrCmdTextBuilderFormat format, string original, string formated)
        {
            var lines = formated.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            CheckStringTextByFormat(format, lines);
            IntegrityTextCheck(original, formated);
        }


        /// <summary>
        /// Integrity check the of words and all text. 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="formated"></param>
        public void IntegrityTextCheck(string original, string formated)
        {
            var originalWords = DrCmdTextBuilder.SplitAndNormalyzeText(original);
            var formatedWords = DrCmdTextBuilder.SplitAndNormalyzeText(formated);

            Assert.IsTrue(CompareStringArray(originalWords, formatedWords),
                          "There was a loss of data when formatting.");

        }



        /// <summary>
        /// Check a string text specified format
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="lines">array of strings</param>
        public void CheckStringTextByFormat(DrCmdTextBuilderFormat format, string[] lines)
        {
            int lineCount = 0;
            foreach (var line in lines)
            {
                lineCount++;
                if ((format.Justify) && (lineCount != lines.Length)) // exclude the last line
                {
                    Assert.IsTrue(line.Length == format.Width,
                                  "Enabled aligned left and right, the string width must be equals format width.");
                }
                else
                {
                    Assert.IsTrue(line.Length <= format.Width,
                                  "The string width must be less or equals format width.");
                }
                if (format.MarginLeftChars != 0)
                {
                    Assert.IsTrue(line.StartsWith(format.SpaceLeft),
                                  String.Format("Each line must begin with '{0}' gaps.",
                                                format.MarginLeftChars.ToString()));
                }
                Assert.IsFalse(line[format.MarginLeftChars] == ' ',
                               "Each line should not start with a space include left margin.");

            }

        }

        public static bool CompareStringArray(string[] a, string[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}

