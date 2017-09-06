//******************************************************************
// File Name:					FWEvent.cs
// Description:					FWEvent class 
// Author:						wuwei
// Date:						2016.12.29
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FW.Event
{
    public delegate void FWEventCB1();
    public delegate void FWEventCB2(EventArg arg);
    public delegate void FWEventCB3(EventID eid, EventArg arg);

    class FWEventCBs
    {
        private EventID m_id;
        private List<Delegate> m_cbs;
        private List<Delegate> m_current;

        public FWEventCBs(EventID id)
        {
            this.m_id = id;
            this.m_cbs = new List<Delegate>();
            this.m_current = new List<Delegate>();
        }

        public void Add(Delegate cb)
        {
            this.m_cbs.Add(cb);
        }

        public bool Remove(Delegate cb)
        {
            if (this.m_cbs.Contains(cb))
            {
                this.m_cbs.Remove(cb);
            }
            return this.m_cbs.Count == 0;
        }

        public void Call(EventArg arg)
        {
            m_current.Clear();
            m_current.AddRange(m_cbs);

            foreach (Delegate dl in this.m_current)
            {
                if (dl.GetType() == typeof(FWEventCB1))
                    ((FWEventCB1)dl)();
                else if (dl.GetType() == typeof(FWEventCB2))
                    ((FWEventCB2)dl)(arg);
                else
                    ((FWEventCB3)dl)(this.m_id, arg);
            }
        }
    }


    class FWEvent
    {
        private static FWEvent sm_inst;
        private Dictionary<EventID, FWEventCBs> m_events;

        private FWEvent()
        {
            m_events = new Dictionary<EventID, FWEventCBs>();
        }

        public static FWEvent Instance
        {
            get
            {
                if (sm_inst == null) sm_inst = new FWEvent();
                return sm_inst;
            }
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private FWEventCBs GetEventCBS(EventID id, bool force = false)
        {
            FWEventCBs eventcbs = null;
            if(!this.m_events.TryGetValue(id, out eventcbs))
            {
                if (force == false) return null;
                eventcbs = new FWEventCBs(id);
                this.m_events.Add(id, eventcbs);
            }
            return eventcbs;
        }
        

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Regist(EventID id, FWEventCB1 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id, true);
            eventcbs.Add(cb);
        }

        public void UnRegist(EventID id, FWEventCB1 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id);
            if(eventcbs != null)
               eventcbs.Remove(cb);
        }

        public void Regist(EventID id, FWEventCB2 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id, true);
            eventcbs.Add(cb);
        }

        public void UnRegist(EventID id, FWEventCB2 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id);
            if (eventcbs != null)
                eventcbs.Remove(cb);
        }

        public void Regist(EventID id, FWEventCB3 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id, true);
            eventcbs.Add(cb);
        }

        public void UnRegist(EventID id, FWEventCB3 cb)
        {
            FWEventCBs eventcbs = GetEventCBS(id);
            if (eventcbs != null)
                eventcbs.Remove(cb);
        }

        //调用
        public void Call(EventID id, EventArg arg)
        {
            FWEventCBs eventcbs = GetEventCBS(id);
            if (eventcbs != null)
                eventcbs.Call(arg);
        }

        public void Call(EventID id)
        {
            this.Call(id, new EventArg());
        }
    }
}