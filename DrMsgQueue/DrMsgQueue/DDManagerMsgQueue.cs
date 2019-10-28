/*
  DDMsgFlushConditions.cs -- Manager message queue 1.0.0, August 30, 2015
 
  Copyright (c) 2013-2015 Kudryashov Andrey aka Dr
 
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
using System;
using System.Collections.Generic;
using System.Timers;

namespace  DrOpen.DrCommon.DrMsgQueue
{
    /// </summary>
    public class DDManagerMsgQueue: IDisposable
    {
        /// <summary>
        /// Initialize Manager Message Queue with default proccessing conditions
        /// </summary>
        public DDManagerMsgQueue()
            : this(DDMsgFlushConditions.GetDefaultConditions())
        { }
        /// <summary>
        /// Initialize Manager Message Queue with specified proccessing conditions
        /// </summary>
        /// <param name="conditions">proccessing conditions</param>
        public DDManagerMsgQueue(DDMsgFlushConditions conditions)
        {
            lock (lockActiveMsgQueue)
            {
                activeMsgQueue = new DDMsgQueue();
            }
            lock (lockLostMsgQueue)
            {
                lostMsgQueue = new DDMsgQueue();
            }
            lock (lockQueue4Processing)
            {
                queue4Processing = new Queue<DDMsgQueue>();
            }
            this.conditions = conditions;
            SetTimerCondition();
            SetTimerProccessMsg();
        }

        private object lockActiveMsgQueue = new object();
        private object lockLostMsgQueue = new object();
        private object lockQueue4Processing = new object();

        private Timer timerCondition;
        private Timer timerProccessMsg;
        /// <summary>
        /// timeout occurrence processing queue
        /// </summary>
        private const double timerProccessMsgInterval = 200;

        private DDMsgFlushConditions conditions;
        private DDMsgQueue activeMsgQueue;
        private DDMsgQueue lostMsgQueue;
        private Queue<DDMsgQueue> queue4Processing;

        #region events
        #region BeforePutMsgToQueue
        /// <summary>
        /// delegate for raise event before put message to queue
        /// </summary>
        /// <param name="msg">message which will be placed in the queue</param>
        /// <param name="cancel">to prevent this action set value to true, the default value is false</param>
        public delegate void BeforePutMsgToQueue(DDNode msg, ref bool cancel);
        /// <summary>
        /// Event raised before put message to queue
        /// </summary>
        public event BeforePutMsgToQueue DoBeforePutMsgToQueue;
        /// <summary>
        /// before put message to queue
        /// </summary>
        /// <param name="msg">message which will be placed in the queue</param>
        /// <param name="cancel">to prevent this action set value to true, the default value is false</param>
        private void OnDoBeforePutMsgToQueue(DDNode msg, ref bool cancel)
        {
            if (DoBeforePutMsgToQueue != null) DoBeforePutMsgToQueue(msg, ref cancel);
        }
        #endregion BeforePutMsgToQueue
        #region BeforeProccessMsgQueue
        /// <summary>
        /// delegate for raise event before processing the messages from queue
        /// </summary>
        /// <param name="queue">queue, its messages will be processed</param>
        /// <param name="cancel">to prevent this action set value to true, the default value is false</param>
        public delegate void BeforeProccessMsgQueue(DDMsgQueue queue, ref bool cancel);
        /// <summary>
        /// Event raised before processing the messages from queue
        /// </summary>
        public event BeforeProccessMsgQueue DoBeforeProccessMsgQueue;
        /// <summary>
        /// before processing the messages from queue
        /// </summary>
        /// <param name="queue">queue, its messages will be processed</param>
        /// <param name="cancel">to prevent this action set value to true, the default value is false</param>
        private void OnDoBeforeProccessMsgQueue(DDMsgQueue queue, ref bool cancel)
        {
            if (DoBeforeProccessMsgQueue != null) DoBeforeProccessMsgQueue(queue, ref cancel);
        }
        #endregion BeforeProccessMsgQueue
        #region ProccessMsg
        /// <summary>
        /// delegate for raise event process the message
        /// </summary>
        /// <param name="msg">message to process</param>
        public delegate void ProccessMsg(DDNode msg);
        /// <summary>
        /// Event raised then message is proccessed
        /// </summary>
        public event ProccessMsg DoProccessMsg;
        /// <summary>
        ///  processing the message
        /// </summary>
        /// <param name="msg">message to process</param>
        private void OnDoProccessMsg(DDNode msg)
        {
            if (DoProccessMsg != null) DoProccessMsg(msg);
        }
        #endregion BeforeProccessMsgQueue
        #endregion events


        private void SetTimerProccessMsg()
        {
            if (this.conditions.FlushImmediately) // need to disable timer
            {
                if (timerProccessMsg != null)
                {
                    timerProccessMsg.Stop();
                    timerProccessMsg.Elapsed -= TimerProccessMsgElapsed;
                    timerProccessMsg.Dispose();
                }
            }
            else
            {
                timerProccessMsg = new Timer(timerProccessMsgInterval);
                timerProccessMsg.AutoReset = false;
                timerProccessMsg.Elapsed += TimerProccessMsgElapsed;
                timerProccessMsg.Start();
            }
        }

        private void SetTimerCondition()
        {
            if (this.conditions.Timer == 0) // need to disable timer
            {
                if (timerCondition != null)
                {
                    timerCondition.Stop();
                    timerCondition.Elapsed -= TimerConditionElapsed;
                    timerCondition.Dispose();
                }
            }
            else
            {
                timerCondition = new Timer(this.conditions.Timer);
                timerCondition.AutoReset = false;
                timerCondition.Elapsed += TimerConditionElapsed;
                timerCondition.Start();
            }
        }

        /// <summary>
        /// put message to queue or proccess msg immediately without queue by condition
        /// </summary>
        /// <param name="msg">message</param>
        public void Put(DDNode msg)
        {
            var cancel = false;
            OnDoBeforePutMsgToQueue(msg, ref cancel); // raise event
            if (cancel) return;
            if (conditions.FlushImmediately) // proccess msg immediately without queue
            {
                ProcessMsg(msg);
            }
            else
            {
                if (CheckConditions4ActiveQueue()) ShiftActiveQueue();
                lock (lockActiveMsgQueue)
                {
                    activeMsgQueue.Enqueue(msg);
                }
            }
        }
        /// <summary>
        /// Check conditions and return true if the active msg queue exceeded the limit, otherwise return false
        /// </summary>
        /// <returns>return true if the active msg queue exceeded the limit of conditions, otherwise return false</returns>
        private bool CheckConditions4ActiveQueue()
        {
            if ((conditions.MaxMsgSize != 0) && (conditions.MaxMsgSize <= activeMsgQueue.Size)) return true;
            if ((conditions.MaxMsgCount != 0) && (conditions.MaxMsgCount <= activeMsgQueue.Count)) return true;
            return false;
        }

        /// <summary>
        /// by timer, shift active msg queue to queue for processing and create new active queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerConditionElapsed(object sender, EventArgs e)
        {
            timerCondition.Stop();
            ShiftActiveQueue();
            timerCondition.Start();
        }

        /// <summary>
        /// function places an active non empty queue to the queue for processing
        /// </summary>
        private void ShiftActiveQueue()
        {
            if (activeMsgQueue.Count == 0) return; // skip empty queue
            lock (lockActiveMsgQueue)
            {
                lock (lockQueue4Processing)
                {
                    queue4Processing.Enqueue(activeMsgQueue); // enqueue active queue
                }
                activeMsgQueue = new DDMsgQueue(); // init new active queue
            }
        }
        /// <summary>
        /// by timer, start processing queue for processing and proccess all messages from these queues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerProccessMsgElapsed(object sender, EventArgs e)
        {
            timerProccessMsg.Stop();
            ProccessQueue();
            timerProccessMsg.Start();
        }
        /// <summary>
        /// start processing queue for processing and proccess all messages from these queues
        /// </summary>
        private void ProccessQueue()
        {
            DDMsgQueue q; //queue, its messages will be processed
            var cancel=false;
            int iCount;
            lock (lockQueue4Processing)
            {
                iCount = queue4Processing.Count;
            }
            while (iCount > 0)
            {
                lock (lockQueue4Processing)
                {
                    q = queue4Processing.Dequeue();
                }
                OnDoBeforeProccessMsgQueue(q, ref cancel); // raise event
                if (cancel) return;
                while (q.Count > 0)
                {
                    ProcessMsg(q.Dequeue());
                }
                lock (lockQueue4Processing)
                {
                    iCount = queue4Processing.Count;
                }
            }
        }

        private void ProcessMsg(DDNode msg)
        {
            OnDoProccessMsg(msg);
        }

        public virtual void Dispose()
        {
            lock (lockActiveMsgQueue)
            {
                lock (lockQueue4Processing)
                {
                    // stop all timers
                    timerProccessMsg.Stop();
                    timerCondition.Stop();
                    if (activeMsgQueue.Count != 0) 
                    {
                        queue4Processing.Enqueue(activeMsgQueue); // enqueue active queue
                    }
                     activeMsgQueue = new DDMsgQueue(); // init new active queue
                }
                ProccessQueue();
            }
        }
    }
}
