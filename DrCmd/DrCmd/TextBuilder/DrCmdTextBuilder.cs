/*
  DrCmdTextBuilder.cs -- Text builder of library to parse arguments. 1.0.0, May 2, 2014
 
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
using System.Text;

namespace DrOpen.DrCommon.DrCmd.TextBuilder
{
    public class DrCmdTextBuilder
    {

        #region const
        public static readonly char[] TextSplitter = new char[] { ' ', '\r', '\n', '\t', '\0' };

        #endregion comnt


        #region DrCmdTextBuilder
        /// <summary>
        /// Initialyze text builder by words to the lines depending by margins from left to right.
        /// </summary>
        /// <param name="format">Text formatting options</param>
        public DrCmdTextBuilder(DrCmdTextBuilderFormat format)
        {
            this.Format = format;
        }

        #endregion DrCmdTextBuilder
        #region Properties
        /// <summary>
        /// Text formatting options
        /// </summary>
        public DrCmdTextBuilderFormat Format { get; set; }
        #endregion Properties

        public string BuildText(string text)
        {
            return BuildWorlds(text.Split(TextSplitter, StringSplitOptions.RemoveEmptyEntries));
        }

        public string BuildWorlds(params string[] words)
        {
            if (words.Length == 0) return string.Empty;
            var result = new StringBuilder();
            words =NormalyzeTextArray(words);

            for (int i = 0; i < words.Length; i++)
            {
                result.Append(GetNextLine(ref i, Format, words)); 
                result.Append("\r\n"); // cannot use all console width, because the symbole \n at the end-(

/*              var newLine = GetNextLine(ref i, Format, words); // can use all console width bacause for maximum line the new line \n symbol is not specified
                result.Append(newLine);
                if (newLine.Length < Format.Width) result.Append("\r\n");
                
 */
            }

            if (Format.RemovePrefixForFistLine) return result.ToString().Substring(Format.MarginLeftChars);
            return result.ToString();
        }



        
        public static string GetNextLine(ref int currentIndex, DrCmdTextBuilderFormat format, string[] words)
        {
            int count = GetElementCountForNewLine(currentIndex, format, words);
            string result  = format.SpaceLeft + String.Join(" ", words, currentIndex, count);
            if ((format.Justify) && (result.Length != format.Width) && (currentIndex + count < words.Length)) // need to Align Left and Right
            {
                var deficit = (format.Width - result.Length);
                var resBuilder =new StringBuilder();
                int step = (deficit / count) + 1;
                int extraDeficit = deficit - (step -1)* (count - 1);
                var stepString = new string(' ', step);
                for (int i = currentIndex; i < currentIndex+count; i++)
                {
                    if (resBuilder.Length != 0)
                    {
                        resBuilder.Append(stepString);
                        if (extraDeficit > 0)
                        {
                            extraDeficit--;
                            resBuilder.Append(" ");
                        }
                    }
                    else
                    {
                        resBuilder.Append(format.SpaceLeft); // add left margin
                    }
                    resBuilder.Append(words[i]);
                }
                result = resBuilder.ToString();
            }

            currentIndex += count-1;
            return result;
            
        }

        /// <summary>
        ///  Calculates element of word from words array for end of line
        /// </summary>
        /// <param name="startIndex">Index for start calculate</param>
        /// <param name="format">Formats of text</param>
        /// <param name="words">Words array</param>
        /// <returns></returns>
        /// <remarks>Return the number of elements for new line.</remarks>>
        static public int GetElementCountForNewLine(int startIndex, DrCmdTextBuilderFormat format, string[] words)
        {

            var maxLength = format.Length;
            var lenght = 0;
            int nextIndex = startIndex;

            while (nextIndex < words.Length)
            {
                lenght += words[nextIndex].Length +1;
                nextIndex++;
                if ((nextIndex < words.Length) && ((lenght + words[nextIndex].Length) > maxLength)) break; // it's maximum
            }

            return nextIndex - startIndex;
        }

        #region Formating
        /// <summary>
        /// Split the text into words and  format them
        /// </summary>
        /// <param name="text">text for format</param>
        /// <returns>Returns formatted string array</returns>
        static public string[] SplitAndNormalyzeText(string text)
        {
            return NormalyzeTextArray(SplitTextIntoWords(text));
        }

        /// <summary>
        /// Split the text into words. Using a space and '\r', '\n', '\t', '\0' as a separator
        /// </summary>
        /// <param name="text">text to split</param>
        /// <returns></returns>
        static public string[] SplitTextIntoWords(string text)
        {
            return text.Split(TextSplitter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Trim all words, replace \r\n\t\0 to space character
        /// </summary>
        /// <param name="text">array of words to normalyze</param>
        static public string[] NormalyzeTextArray(params string[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = NormalyzeText(text[i]);
            }
            return text;
        }

        /// <summary>
        /// Trim and replace \r, \n, \t, \0 to space character. Prevent repetition of spaces in the text.
        /// </summary>
        /// <param name="text">world to normalyze</param>
        static public string NormalyzeText(string text)
        {
            text = text.Trim();
            var result = text.ToCharArray();
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i].Equals('\r')) || (text[i].Equals('\n')) || (text[i].Equals('\t')) ||(text[i].Equals('\0'))) result[i] = ' ';
            }
            return RemoveDoubleSpace(new string(result)).Trim();
        }


        /// <summary>
        /// Remove double space in the text Prevent repetition of spaces in the text.
        /// </summary>
        /// <param name="text">text to normalyze</param>
        static public string RemoveDoubleSpace(string text)
        {
            int lastLength = 0;
            do
            {
                lastLength = text.Length;
                text= text.Replace("  ", " ");
            } while (lastLength != text.Length); //do until the string length changes
            return text;
        }
        #endregion Formating
    }
}
