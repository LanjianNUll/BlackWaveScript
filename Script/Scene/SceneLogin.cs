//******************************************************************
// File Name:					SceneLogin.cs
// Description:					SceneLogin class 
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

namespace FW.Scene
{
    class SceneLogin : SceneBase
    {

        protected SceneLogin()
        {
            this.m_type = SceneType.Login;
        }

        public static SceneBase Create()
        {
            return new SceneLogin();
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //初始化
        public override void Init()
        {
            base.Init();
            //清理玩家
            Role.Role.Instance().Reset();
            //物品清理
            Item.ItemMgr.Dispose();
            //恢复登录状态 
            FW.Login.LoginHandler.Recover();
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