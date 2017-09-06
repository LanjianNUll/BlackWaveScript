//******************************************************************
// File Name:					SettingDialogUI
// Description:					SettingDialogUI class 
// Author:						lanjian
// Date:						3/8/2017 11:35:53 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class SettingDialogUI:DialogBaseUI
    {
        protected SettingDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/SettingDialog";
            this.m_DType = DialogType.Setting;
        }

        public static SettingDialogUI Create()
        {
            return new SettingDialogUI();
        }
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(UISceneMgr.CurrScene.RootObj.transform.Find("MainPanel/DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnLoginBackIn;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("cancel")).onClick = OnLogOutAndExit;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {

        }

        //重新登录
        private void OnLoginBackIn(GameObject go)
        {
            //将自动登录去掉
            FW.Login.LoginConfig.SetUserAutoLogin(false);
            //重新进入登录界面
            Scene.SceneMgr.Enter(Scene.SceneType.Login);
        }

        //退出游戏
        private void OnLogOutAndExit(GameObject go)
        {
            Application.Quit();
        }

        //--------------------------------------
        //public
        //--------------------------------------

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
