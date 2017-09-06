//******************************************************************
// File Name:					UISceneBase.cs
// Description:					UISceneBase class 
// Author:						wuwei
// Date:						2017.01.04
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FW.UI
{
    delegate void OnUIEventCB(Event.EventArg args);
    abstract class UISceneBase
    {
        protected string m_resName;
        protected GameObject m_rootObj;

        protected UISceneBase()
        {
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject RootObj { get { return m_rootObj; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        public virtual void Init()
        {
            this.m_rootObj = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(this.m_resName) as GameObject);
            if(this. m_rootObj != null)
                this.m_rootObj.transform.parent = GameObject.Find("UIRootContainer").transform;
        }

        public virtual void DisPose()
        {
            if(this.m_rootObj != null)
                UnityEngine.Object.Destroy(this.m_rootObj);
        }

        public abstract void BindScript(UIEventBase eventBase);
        public virtual void UpdateInput() { }
    }
}