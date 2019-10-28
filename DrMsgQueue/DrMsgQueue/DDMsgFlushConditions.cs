/*
  DDMsgFlushConditions.cs -- conditions for the message queue 1.0.1, February 7, 2016
 
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
using DrOpen.DrData.DrDataObject;
using DrOpen.DrData.DrDataObject.Exceptions;
using System;

namespace DrOpen.DrCommon.DrMsgQueue
{
    /// <summary>
    /// conditions for the message queue
    /// </summary>
    public class DDMsgFlushConditions
    {
        #region DDMsgFlushConditions
        /// <summary>
        /// initialize Flush Conditions with default conditions, see DDMsgFlushConditions.GetDefaultConditions()
        /// </summary>
        public DDMsgFlushConditions(): this(GetDefaultConditionsNode())
        { }
        /// <summary>
        /// initialize Flush Conditions with specified conditions
        /// </summary>
        /// <param name="conditions">conditions</param>
        public DDMsgFlushConditions(DDNode conditions)
        {
            if (conditions.Type != GetConditionsType()) throw new DDTypeExpectedException(conditions.Type, GetConditionsType());
            this.conditions = conditions;
            MaxMsgSize = conditions.Attributes.GetValue(FLUSH_CONDITION.MAX_MSG_SIZE, DEFAULT_MAX_MSG_SIZE);
            MaxMsgCount = conditions.Attributes.GetValue(FLUSH_CONDITION.MAX_MSG_COUNT, DEFAULT_MAX_MSG_COUNT);
            Timer = conditions.Attributes.GetValue(FLUSH_CONDITION.TIMER, DEFAULT_TIMER);
            MaxLostMsgSize = conditions.Attributes.GetValue(FLUSH_CONDITION.MAX_LOST_MSG_SIZE, DEFAULT_MAX_LOST_MSG_SIZE);
            FlushImmediately = conditions.Attributes.GetValue(FLUSH_CONDITION.FLUSH_IMMEDIATELY, DEFAULT_FLUSH_IMMEDIATELY);
        }
        #endregion DDMsgFlushConditions
        /// <summary>
        /// list of supported conditions
        /// </summary>
        public enum FLUSH_CONDITION
        {
            /// <summary>
            /// maximum buffer size beyond which the message begins to flush, by default 1024 kbytes. If this parameter is 0 then this condition is disabled.
            /// </summary>
            MAX_MSG_SIZE,
            /// <summary>
            /// maximum message count beyond which the message begins to flush, by default 100 messages. If this parameter is 0 then this condition is disabled.
            /// </summary>
            MAX_MSG_COUNT,
            /// <summary>
            /// after a period of time to flush, by default 1000 milliseconds. If this parameter is 0 then this condition is disabled.
            /// </summary>
            TIMER,
            /// <summary>
            /// dumping lost messages as raw to the file system after exceeding this size, by default 102400 kbytes. If this parameter is 0 then this condition is disabled.
            /// </summary>
            MAX_LOST_MSG_SIZE,
            /// <summary>
            /// If this parameter is true, all messages are processed immediately without delay. All other conditions will be ignored, by default is false
            /// </summary>
            FLUSH_IMMEDIATELY
        }

        private DDNode conditions;
        /// <summary>
        /// maximum buffer size beyond which the message begins to flush, by default 1024 kbytes
        /// </summary>
        const long DEFAULT_MAX_MSG_SIZE = 1024;
        /// <summary>
        /// maximum message count beyond which the message begins to flush, by default 100 messages
        /// </summary>
        const long DEFAULT_MAX_MSG_COUNT = 100;
        /// <summary>
        /// after a period of time to flush, by default 1000 milliseconds 
        /// </summary>
        const long DEFAULT_TIMER = 1000;
        /// <summary>
        /// dumping lost messages as raw to the file system after exceeding this size, by default 102400 kbytes
        /// </summary>
        const long DEFAULT_MAX_LOST_MSG_SIZE = 102400;
        /// <summary>
        /// If this parameter is true, all messages are processed immediately without delay. All other conditions will be ignored, by default is false
        /// </summary>
        const bool DEFAULT_FLUSH_IMMEDIATELY = false;
        /// <summary>
        /// maximum buffer size beyond which the message begins to flush, by default 1024 kbytes
        /// </summary>
        public long MaxMsgSize
        { get; private set; }
        /// <summary>
        /// maximum message count beyond which the message begins to flush, by default 100 messages. If this parameter is 0 then this condition is disabled.
        /// </summary>
        public long MaxMsgCount
        { get; private set; }
        /// <summary>
        /// after a period of time to flush, by default 1000 milliseconds. If this parameter is 0 then this condition is disabled.
        /// </summary>
        public long Timer
        { get; private set; }
        /// <summary>
        /// dumping lost messages as raw to the file system after exceeding this size, by default 102400 kbytes. If this parameter is 0 then this condition is disabled.
        /// </summary>
        public long MaxLostMsgSize
        { get; private set; }
        /// <summary>
        ///  If this parameter is true, all messages are processed immediately without delay. All other conditions will be ignored, by default is false
        /// </summary>
        public bool FlushImmediately
        { get; private set; }

        #region static
        /// <summary>
        /// return DDNode type for this conditions
        /// </summary>
        /// <returns></returns>
        public static DDType GetConditionsType()
        {
            return new DDType(typeof(DDMsgFlushConditions));
        }
        /// <summary>
        /// return default conditions node
        /// </summary>
        /// <returns>return default conditions node</returns>
        public static DDNode GetDefaultConditionsNode()
        {
            var n = new DDNode(GetConditionsType());
            n.Attributes.Add(FLUSH_CONDITION.MAX_MSG_SIZE, DEFAULT_MAX_MSG_SIZE);
            n.Attributes.Add(FLUSH_CONDITION.MAX_MSG_COUNT, DEFAULT_MAX_MSG_COUNT);
            n.Attributes.Add(FLUSH_CONDITION.TIMER, DEFAULT_TIMER);
            n.Attributes.Add(FLUSH_CONDITION.MAX_LOST_MSG_SIZE, DEFAULT_MAX_LOST_MSG_SIZE);
            n.Attributes.Add(FLUSH_CONDITION.FLUSH_IMMEDIATELY, DEFAULT_FLUSH_IMMEDIATELY);
            return n;
        }
        /// <summary>
        /// return default conditions
        /// </summary>
        /// <returns>return default conditions</returns>
        public static DDMsgFlushConditions GetDefaultConditions()
        {
            return new DDMsgFlushConditions();
        }
        #endregion static
    }
}