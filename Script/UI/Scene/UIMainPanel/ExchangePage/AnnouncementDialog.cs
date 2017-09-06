//******************************************************************
// File Name:					AnnouncementDialog
// Description:					AnnouncementDialog class 
// Author:						lanjian
// Date:						3/2/2017 2:05:25 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 公告对话
    /// </summary>
    class AnnouncementDialog:DialogBaseUI
    {
        protected AnnouncementDialog()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/AnnouncementDialog";
            this.m_DType = DialogType.Announcement;
        }

        public static AnnouncementDialog Create()
        {
            return new AnnouncementDialog();
        }

        private long m_timer;
        private int m_invteral= 10;            //10秒后可点击

        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = null;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            m_DialogUIGo.transform.Find("confirm").GetComponent<BoxCollider>().enabled = false;
            m_DialogUIGo.transform.Find("confirm").GetChild(0).GetComponent<UILabel>().color = Color.gray;
        }

        //显示公告
        private void ShowAnnounment(string announceStr)
        {
            m_DialogUIGo.transform.Find("content").GetComponent<UILabel>().text = announceStr;
        }

        private void OnConfirm(GameObject go)
        {
            this.CloseDialog();
            //打开兑换界面
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.Enter_ExchangePanel);
        }
        
        private void CountZero()
        {
            m_DialogUIGo.transform.Find("confirm").GetChild(1).GetComponent<UILabel>().text = " "+ m_invteral--;
            if (this.m_invteral < 0)
            {
                m_DialogUIGo.transform.Find("confirm").GetComponent<BoxCollider>().enabled = true;
                Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
                m_DialogUIGo.transform.Find("confirm").GetChild(0).GetComponent<UILabel>().color = new Color(0,198,255,255);
                NGUITools.SetActive(m_DialogUIGo.transform.Find("confirm").GetChild(1).gameObject,false);
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("center/btnGroup"));
        }

        public override void OnBack(GameObject go)
        {
            this.CloseDialog();
            //返回主界面
            PanelMgr.BackToMainPanel();
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.GetDialogAbout();
            this.ShowAnnounment((string)args[0]);
            this.OpenDialog();
            //开启一个计时
            this.m_timer = Timer.Regist(0, 1, m_invteral+1, CountZero);
        }

        public override void DisPose()
        {
            Timer.Cancel(this.m_timer);
            base.DisPose();
        }
    }
}
