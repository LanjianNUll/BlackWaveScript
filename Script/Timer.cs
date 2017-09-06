//******************************************************************
// File Name:					Timer.cs
// Description:					Timer class 
// Author:						wuwei
// Date:						2016.12.30
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;

namespace FW
{
    delegate void TimerCB1();
    delegate void TimerCB2(params object[] args);
    static class Timer
    {
        private abstract class TimeCallback
        {
            private bool m_alive;
            private float m_start;
            private float m_interval;
            private int m_count;
            private float m_nextTime;
            private int m_currCount;

            public TimeCallback(float start, float interval, int count)
            {
                this.m_alive = true;
                this.m_currCount = 0;
                this.m_start = start;
                this.m_interval = interval;
                this.m_count = count;
                this.m_nextTime = Time.time + this.m_start;
            }

            public bool Alive { get { return this.m_alive; } }
            public float NextTime { get { return this.m_nextTime; } }

            public void Dispose()
            {
                this.m_alive = false;
            }

            public abstract void Call();

            public bool TryCall(float time)
            {
                if (this.Alive == false)
                    return true;
                if (time < this.m_nextTime)
                    return false;
                if(this.m_currCount < this.m_count || this.m_count == 0 || this.m_interval == 0.0f)
                {
                    this.m_currCount++;
                    this.Call();
                }
                //检查是否存活
                if((this.m_currCount >= this.m_count && this.m_count != 0) || this.m_interval == 0.0f)
                {
                    this.m_alive = false;
                    return true;
                }
                this.m_nextTime += this.m_interval;
                return false;
            }
        }

        private class TimeCallbck1 : TimeCallback
        {
            private TimerCB1 m_cb;
            public TimeCallbck1(float start, float interval, int count, TimerCB1 cb)
                : base(start, interval, count)
            {
                this.m_cb = cb;
            }

            public override void Call()
            {
                if (this.m_cb != null)
                    this.m_cb();
            }
        }

        private class TimeCallbck2 : TimeCallback
        {
            private TimerCB2 m_cb;
            private object[] m_args;
            public TimeCallbck2(float start, float interval, int count, TimerCB2 cb, params object[] args)
                : base(start, interval, count)
            {
                this.m_cb = cb;
                this.m_args = args;
            }

            public override void Call()
            {
                if (this.m_cb != null)
                    this.m_cb(this.m_args);
            }
        }

        private static Dictionary<Int64, TimeCallback> sm_cbs;
        private static Int64 sm_index;
        private static float sm_nextTime;					// 下次回调时间
        private static Queue<Int64> sm_invalids;

        static Timer()
        {
            sm_index = 1;
            sm_nextTime = 0.0f;
            sm_cbs = new Dictionary<long, TimeCallback>();
            sm_invalids = new Queue<long>();
        }

        private static Int64 GetID()
        {
            //if (sm_invalids.Count > 0)
            //    return sm_invalids.Dequeue();
            if (sm_index < Int64.MaxValue)
                return ++sm_index;
            return -1;
        }

        internal static void Update()
        {
            float time = Time.time;
            if (time < sm_nextTime) return;
            sm_nextTime = time + 100.0f;
            List<Int64> invalids = new List<Int64>();
            foreach (KeyValuePair<Int64, TimeCallback> pair in sm_cbs)
            {
                Int64 id = pair.Key;
                TimeCallback cb = pair.Value; 
                if (cb.TryCall(time))
                    invalids.Add(id);
                //找出下次要执行的
                if (cb.Alive && sm_nextTime > cb.NextTime)
                    sm_nextTime = cb.NextTime;
            }
            //清除
            foreach (Int64 id in invalids)
            {
                sm_cbs.Remove(id);
                sm_invalids.Enqueue(id);
            }
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static Int64 Regist(float start, float interval, TimerCB1 cb)
        {
            return Regist(start, interval, 0, cb);
        }

        public static Int64 Regist(float start, float interval, int count, TimerCB1 cb)
        {
            Int64 id = Timer.GetID();
            sm_nextTime = Math.Min(sm_nextTime, Time.time + start);
            sm_cbs.Add(id, new TimeCallbck1(start, interval, count, cb));
            return id;
        }

        public static Int64 Regist(float start, float interval, TimerCB2 cb, params object[] args)
        {
            return Regist(start, interval, 0, cb, args);
        }

        public static Int64 Regist(float start, float interval, int count, TimerCB2 cb, params object[] args)
        {
            Int64 id = Timer.GetID();
            sm_nextTime = Math.Min(sm_nextTime, Time.time + start);
            sm_cbs.Add(id, new TimeCallbck2(start, interval, count, cb,args));
            return id;
        }

        public static bool Cancel(Int64 id)
        {
            TimeCallback cb;
            if (sm_cbs.TryGetValue(id, out cb))
            {
                cb.Dispose();
                return sm_cbs.Remove(id);
            }
            return false;
        }
    }
}