//******************************************************************
// File Name:					PanelExchange
// Description:					PanelExchange class 
// Author:						lanjian
// Date:						1/5/2017 4:19:05 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    class PanelExchange:PanelBase
    {
        protected PanelExchange()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/ExchangePanel";
            this.panelType = PanelType.Exchange;
        }

        public static PanelExchange Create()
        {
            return new PanelExchange();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).gameObject;
            FWPageMgr.Instance.LoadExchangePage();
            pageTotalCount = FWPageMgr.Instance.CurrentPageCount;
            LoadPageNum(pageTotalCount);
            //我的兑换
            Utility.Utility.GetUIEventListener(this.RootObj.transform.GetChild(0).Find("btnGroup/myExchange"))
                .onClick = OnMyExchange;
        }

        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Enter_ExchangePanel, OnLoadExchangePanel);
        }

        private void OnMyExchange(GameObject go)
        {
            DialogMgr.Load(DialogType.MyExchange);
            DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg());
        }

        private void OnBackMainBtn()
        {
            PanelMgr.BackToMainPanel();
        }

        private void ShowAnnounceMentDialogUI()
        {
            string noticeContent = Role.Role.Instance().ExchangePrizeProctor.GetNotice();
            DialogMgr.Load(DialogType.Announcement);
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(noticeContent));
        }

        private void OnLoadExchangePanel()
        {
            this.IsAllowHorMove(true);
            this.FindAllUI();
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void UpdateInput()
        {
            //页item的update
            //FWPageMgr.Instance.ScrollViewItemBaseUpdate();
            //触摸加载控制
            if (this.m_IsAllowHorzMove)
            {
                BaseTouchControl();
                BaseAnimtionControl();
            }
            //对话框的Update
            if (DialogMgr.CurrentDialog != null)
                DialogMgr.CurrentDialog.UpadateDialog();
        }

        public override void BindScript(UIEventBase eventBase)
        {
            if (Role.Role.Instance().IsOpenAnn)
            {
                this.IsAllowHorMove(true);
                this.FindAllUI();
            }
            else
            {
                //先显示公告，禁止滑动
                this.IsAllowHorMove(false);
                this.ShowAnnounceMentDialogUI();
                Role.Role.Instance().IsOpenAnn = true;
            }
            ResgistEvents();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Enter_ExchangePanel, OnLoadExchangePanel);
            //销毁
            FWPageMgr.Instance.ExitPage();
            base.DisPose();
        }
    }
}
