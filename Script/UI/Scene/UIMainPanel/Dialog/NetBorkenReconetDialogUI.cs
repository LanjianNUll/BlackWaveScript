//******************************************************************
// File Name:					NetBorkenReconetDialogUI
// Description:					NetBorkenReconetDialogUI class 
// Author:						lanjian
// Date:						4/18/2017 10:59:54 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class NetBorkenReconetDialogUI:DialogBaseUI
    {
        protected NetBorkenReconetDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/NetBReconnectDialog";
            this.m_DType = DialogType.NetBrokenReconnect;
            //断线失败
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_FAIL, OnReconnetFail);
            //断线重连中
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
        }

        public static NetBorkenReconetDialogUI Create()
        {
            return new NetBorkenReconetDialogUI();
        }

        private long m_timer;
        private int m_invteral = 10;            //10秒后可点击
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
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnReLogin;
        }

        private void FillDataUI()
        {
            this.m_DialogUIGo.transform.GetChild(1).GetComponent<UILabel>().text = "你的网络不稳定，正在尝试重新连接，请稍候！";
            //隐藏按钮
            NGUITools.SetActive(m_DialogUIGo.transform.Find("confirm").gameObject, false);
        }

        private void ShowReLoginUI()
        {
            this.m_DialogUIGo.transform.GetChild(1).GetComponent<UILabel>().text = "无法连接服务器，请重新登录！！";
            NGUITools.SetActive(m_DialogUIGo.transform.Find("confirm").gameObject, true);
            NGUITools.SetActive(m_DialogUIGo.transform.GetChild(2).gameObject, false);
        }

        private void ZeroCount()
        {
            if (m_invteral <= 0)
                ShowReLoginUI();
            m_DialogUIGo.transform.GetChild(2).GetComponent<UILabel>().text = " " + m_invteral--;

        }

        private void OnReconnetFail()
        {
            
        }

        private void OnReconnetSuccess()
        {
            this.CloseDialog();
        }

        private void OnReLogin(GameObject go)
        {
            //将自动登录去掉
            FW.Login.LoginConfig.SetUserAutoLogin(false);
            //重新进入登录界面
            Scene.SceneMgr.Enter(Scene.SceneType.Login);
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void DisPose()
        {
            Timer.Cancel(this.m_timer);
            //断线失败
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_FAIL, OnReconnetFail);
            //断线重连中
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
            base.DisPose();
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI();
            this.OpenDialog();
            //开启一个计时
            this.m_timer = Timer.Regist(0, 1, m_invteral+1, ZeroCount);
        }
    }
}
