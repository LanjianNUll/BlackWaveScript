//******************************************************************
// File Name:					LoginUI.cs
// Description:					LoginUI class 
// Author:						lanjian
// Date:						2016.12.30
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;

namespace FW.UI
{
    class LoginUI : UIEventBase
    {
        void Start()
        {
            if(UISceneMgr.CurrScene != null)
            {
                UISceneMgr.CurrScene.BindScript(this);
            } 
        }

        // Update is called once per frame
        void Update()
        {
            if (UISceneMgr.CurrScene != null)
            {
                UISceneMgr.CurrScene.UpdateInput();
            }
        }

        //销毁
        void OnDestroy()
        {
        }

        //--------------------------------------
        //private
        //--------------------------------------

        //--------------------------------------
        //public
        //--------------------------------------
        /// <summary>
        /// 自动登陆勾选
        /// </summary> 
        public void AutoLoginClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.UI_LOGIN_AUTOLOGIN_BUTTON);
        }

        /// <summary>
        /// 登陆按钮
        /// </summary>
        public void LoginButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.UI_LOGIN_LOGIN_BUTTON);
        }
        
        /// <summary>
        /// 退出按钮
        /// </summary>
        public void ExitButtonClick()
        {
            
        }

        /// <summary>
        /// 监听是否改动自动填写的账户
        /// </summary>
        public void InputUserChange()
        {
            Event.FWEvent.Instance.Call(Event.EventID.UI_LOGIN_CHANGE_CONTENT,new Event.EventArg(true,0));
        }

        public void InputPassWordChange()
        {
            Event.FWEvent.Instance.Call(Event.EventID.UI_LOGIN_CHANGE_CONTENT, new Event.EventArg(true,1));
        }

        /// <summary>
        /// 快速开始按钮
        /// </summary>
        public void QuickStartButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.UI_LOGIN_QUICKLOGIN_BUTTON);
        }

        /// <summary>
        /// 切换账户按钮
        /// </summary>
        public void ChangeAccountButtonClick()
        {
           
        }

        ///// <summary>
        ///// 监听到一旦用户名有改动，密码域清空
        ///// </summary>
        //public void ClearPassWord()
        //{
        //    m_PassWordInput.value = "";
        //}
    }
}
