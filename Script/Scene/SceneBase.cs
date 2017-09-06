//******************************************************************
// File Name:					SceneControl.cs
// Description:					SceneControl class 
// Author:						wuwei
// Date:						2016.12.27
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FW.Scene
{
    abstract class SceneBase
    {
        protected SceneType m_type;

        protected SceneBase()
        {
            this.m_type = SceneType.Unknow;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public SceneType Type { get { return this.m_type; } }

        //--------------------------------------
        //private 
        //--------------------------------------

        //--------------------------------------
        //public 
        //--------------------------------------
        //初始化
        public virtual void Init()
        {
            UI.UISceneMgr.Load(this.Type);
            Game.GameCamera.EnterGame(this.Type == SceneType.Game);
        }

        //update
        public virtual void Update(float deltaTime)
        {
        }

        //销毁
        public virtual void Dispose()
        {
            UI.UISceneMgr.Dispose();
        }
    }
}