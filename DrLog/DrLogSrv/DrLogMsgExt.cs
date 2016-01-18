/*
  DrLogMsgExt.cs -- extension for log messages 1.0.0, 4 January 30, 2016
 
  Copyright (c) 2013-2016 Kudryashov Andrey aka Dr
 
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

using DrOpen.DrCommon.DrData;
using System;
using System.Text;

namespace DrOpen.DrCommon.DrLog.DrLogSrv
{
    public static class DrLogMsgExt
    {

        #region GetData

        /// <summary>
        ///  Determines whether the Recipients is specified
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static public bool ContainsRecipients(this DDNode msg)
        {
            return msg.Attributes.Contains(DrLogMsgConst.AttRecipients);
        }
        /// <summary>
        /// Returns recipients of message. If attribute doesn't exist returns String.Empty
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public string[] GetRecipients(this DDNode msg)
        {
            return msg.Attributes.GetValue(DrLogMsgConst.AttRecipients, String.Empty);
        }
        /// <summary>
        ///  Determines whether the Providers is specified
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static public bool ContainsProviders(this DDNode msg)
        {
            return msg.Attributes.Contains(DrLogMsgConst.AttProviders);
        }
        /// <summary>
        /// Returns providers of message. If attribute doesn't exist returns String.Empty
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public string[] GetProviders(this DDNode msg)
        {
            return msg.Attributes.GetValue(DrLogMsgConst.AttProviders, String.Empty);
        }

        /// <summary>
        /// Returns body of message. If attribute doesn't exist returns String.Empty
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public string GetBody(this DDNode msg)
        {
            return msg.Attributes.GetValue(DrLogMsgConst.AttBody, String.Empty);
        }
        /// <summary>
        /// Returns source of message. If attribute doesn't exist returns String.Empty
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public string GetSource(this DDNode msg)
        {
            return msg.Attributes.GetValue(DrLogMsgConst.AttSource, String.Empty);
        }
        /// <summary>
        /// Log level. If attribute doesn't exist returns DrLogSrv.LogLevel.NONE
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public DrLogSrv.LogLevel GetLogLevel(this DDNode msg)
        {
            return (DrLogSrv.LogLevel)Enum.Parse(typeof(DrLogSrv.LogLevel), msg.Attributes.GetValue(DrLogMsgConst.AttLevel, DrLogSrv.LogLevel.NONE), true);
        }

        static public string GetLogException(this DDNode msg, DrLogSrv.LogExceptionLevel eLevel)
        {
            var result = new StringBuilder();
            if (eLevel == LogExceptionLevel.NONE) return String.Empty; // supress log Exception

            foreach (var n in msg)
            {
                if (n.Value.Type.Name == DDNode.TpException) result.Append("'" + BuildExceptionAsString(eLevel, n.Value) + "'");
            }

            return result.ToString();
        }


        static public string BuildExceptionAsString(DrLogSrv.LogExceptionLevel eLevel, DDNode eNode)
        {
            var result = new StringBuilder();

            if ((eLevel & LogExceptionLevel.MESSAGE) == LogExceptionLevel.MESSAGE) 
                result.Append("'" + eNode.Attributes.GetValue(DDNode.AttMessage, string.Empty).GetValueAsString() + "'.");
            if (((eLevel & LogExceptionLevel.SOURCE) == LogExceptionLevel.SOURCE) && (eNode.Attributes.Contains(DDNode.AttSource))) 
                result.Append(" Source: '" + eNode.Attributes.GetValue(DDNode.AttSource, string.Empty).GetValueAsString() + "'.");
            if (((eLevel & LogExceptionLevel.TYPE) == LogExceptionLevel.TYPE) && (eNode.Attributes.Contains(DDNode.AttType)))
                result.Append(" Type: '" +  eNode.Attributes.GetValue(DDNode.AttType, string.Empty).GetValueAsString() + "'.");
            if (((eLevel & LogExceptionLevel.STACK_TRACE) == LogExceptionLevel.STACK_TRACE) && (eNode.Attributes.Contains(DDNode.AttStackTrace)))
                result.Append(" StackTrace: '" + eNode.Attributes.GetValue(DDNode.AttStackTrace, string.Empty).GetValueAsString() + "'.");
            if (((eLevel & LogExceptionLevel.HELP_LINK) == LogExceptionLevel.HELP_LINK) && (eNode.Attributes.Contains(DDNode.AttHelpLink)))
                result.Append(" HelpLink: '" + eNode.Attributes.GetValue(DDNode.AttHelpLink, string.Empty).GetValueAsString() + "'.");

            if (((eLevel & LogExceptionLevel.DATA) == LogExceptionLevel.DATA) && (eNode.Contains(DDNode.NdData))) 
                result.Append(BuildExceptionDataAsString(eNode.GetNode(DDNode.NdData)));

            if (((eLevel & LogExceptionLevel.INNERT_EXCEPTION) == LogExceptionLevel.INNERT_EXCEPTION) && (eNode.Contains(DDNode.NdInnerException)))
            {
                result.Append(" --> {");
                result.Append(BuildExceptionAsString(eLevel, eNode.GetNode(DDNode.NdInnerException)));
                result.Append("}.");
            }

            return result.ToString();
        }

        private static string BuildExceptionDataAsString(DDNode nData)
        {
            var result = new StringBuilder();
            if ((nData != null) && (nData.HasAttributes))
            {
                result.Append(" Data: ");
                var bFirstAttr = true;
                foreach (var a in nData.Attributes)
                {
                    if (bFirstAttr == false) result.Append(", "); // separate key value pair
                    result.Append("[Key: '");
                    result.Append(a.Key);
                    result.Append("', Value: '");
                    result.Append(a.Value.GetValueAsString());
                    result.Append("']");
                    bFirstAttr = false;
                }
                result.Append(").");
            }
            return result.ToString();
        }


        /// <summary>
        /// Returns message creation date and time. If attribute doesn't exist returns DateTime.MinValue
        /// </summary>
        /// <param name="n">message</param>
        /// <returns></returns>
        static public DateTime GetDateTime(this DDNode msg)
        {
            return msg.Attributes.GetValue(DrLogMsgConst.AttDateTime, DateTime.MinValue);
        }



        #endregion GetData
    }
}
