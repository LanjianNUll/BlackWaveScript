//******************************************************************
// File Name:					SceneMain.cs
// Description:					SceneMain class 
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

using FW.Role;

namespace FW.Scene
{
    class SceneMain : SceneBase
    {

        protected SceneMain()
        {
            this.m_type = SceneType.Main;
        }

        public static SceneBase Create()
        {
            return new SceneMain();
        }
        //--------------------------------------
        //public 
        //--------------------------------------
        //初始化
        public override void Init()
        {
            base.Init();
            Role.Role.Instance().Init();
        }

        //update
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        //销毁
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}