/*
  DrCmdTextBuilderFormat.cs -- Parameters for text builder of library to parse arguments. 1.0.0, May 2, 2014
 
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
using DrCmd.Res;

namespace DrOpen.DrCommon.DrCmd.TextBuilder
{
    /// <summary>
    /// Text formatting options
    /// </summary>
    public class DrCmdTextBuilderFormat
    {

        #region DrCmdTextBuilderFormat
        /// <summary>
        /// Initialyze text builder by words to the lines depending by margins from left to right. Don't distribute your text evenly between the margins
        /// </summary>
        /// <param name="marginLeftChars">left margin in characters</param>
        /// <param name="width">width of text</param>
        public DrCmdTextBuilderFormat(int marginLeftChars, int width) : this(marginLeftChars, width, false, false) { }

        /// <summary>
        /// Initialyze text builder by words to the lines depending by margins from left to right.
        /// </summary>
        /// <param name="marginLeftChars">left margin in characters</param>
        /// <param name="width">width of text</param>
        /// <param name="justify">distribute your text evenly between the margins</param>
        /// <param name="removePrefixForFistLine">Settings for first line of text formatting. If it is true the first line indent formatted text from left to be removed.</param>
        public DrCmdTextBuilderFormat(int marginLeftChars, int width, bool justify, bool removePrefixForFistLine)
        {
            SetFormat(marginLeftChars, width, false);
            this.Justify = justify;
            RemovePrefixForFistLine = removePrefixForFistLine;
        }
        #endregion DrCmdTextBuilderFormat
        #region Properties
        /// <summary>
        /// Settings for first line of text formatting. If it is true the first line indent formatted text from left to be removed.
        /// </summary>
        public bool RemovePrefixForFistLine { get; private set; }

        /// <summary>
        /// left margin in characters
        /// </summary>
        public int MarginLeftChars { get; private set; }
        /// <summary>
        /// width of text
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The maximum length of the text in characters excluding margins
        /// </summary>
        public int Length
        {
            get { return (this.Width - this.MarginLeftChars ); }
        }
        /// <summary>
        /// distribute your text evenly between the margins
        /// </summary>
        public bool Justify { get; set; }
        /// <summary>
        /// left margin
        /// </summary>
        public string SpaceLeft { get; private set; }
        #endregion Properties
        /// <summary>
        /// Change text width and margins size
        /// </summary>
        /// <param name="marginLeftChars">left margin in characters</param>
        /// <param name="width">width of text</param>
        public void SetFormat(int marginLeftChars, int width)
        {
            SetFormat(marginLeftChars, width, false);
        }
        /// <summary>
        /// Change text width and margins size
        /// </summary>
        /// <param name="marginLeftChars">left margin in characters</param>
        /// <param name="width">width of text</param>
        /// <param name="removePrefixForFistLine">settings for first line of text formatting. If it is true the first line indent formatted text from left to be removed.</param>
        public void SetFormat(int marginLeftChars, int width, bool removePrefixForFistLine)
        {
            ValidateFormat(marginLeftChars, width);
            RemovePrefixForFistLine = removePrefixForFistLine;
            this.MarginLeftChars = marginLeftChars;
            this.Width = width;
            SpaceLeft = new string(' ', marginLeftChars);
        }
        /// <summary>
        /// checks the values of the specified parameters and, if the error throws an <exception cref="ArgumentException">ArgumentException</exception>.
        /// </summary>
        /// <param name="marginLeftChars">left margin in characters</param>
        /// <param name="width">width of text</param>
        private void ValidateFormat(int marginLeftChars, int width)
        {
            if ((width - marginLeftChars ) < 0) throw new ArgumentException(Msg.TEXT_BUILDER_LEFT_MARGIN_GREATER_THAN_TEXT_WIDTH);
            if (width <= 0) throw new ArgumentException(Msg.TEXT_BUILDER_TEXT_WIDTH_CANNOT_BE_LESS_THAN_ZERO);
            if ((marginLeftChars < 0)) throw new ArgumentException(Msg.TEXT_BUILDER_MARGIN_CANNOT_BE_LESS_THAN_ZERO);
        }

    }
}
