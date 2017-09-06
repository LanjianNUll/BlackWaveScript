//******************************************************************
// File Name:					EventArg.cs
// Description:					EventArg class 
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
    public class EventArg
    {
        private object[] m_args;
        public EventArg(params object[] args)
        {
            this.m_args = args;
        }

        public object this[int index]
        {
            get { return this.m_args[index]; }
        }

        public object[] Args
        {
            get { return m_args; }
        }
    }
}