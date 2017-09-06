//******************************************************************
// File Name:					PanelPlayer
// Description:					PanelPlayer class 
// Author:						lanjian
// Date:						1/5/2017 3:16:19 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    class PanelPlayer : PanelBase
    {
        protected PanelPlayer()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/PlayerPanel";
            this.panelType = PanelType.Player;
        }

        public static PanelPlayer Create()
        {
            return new PanelPlayer();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).gameObject;
            //加载角色页面
            FWPageMgr.Instance.LoadPlayerPage();
            pageTotalCount = FWPageMgr.Instance.CurrentPageCount;
            //加载页码
            LoadPageNum(pageTotalCount);
        }

        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
        }

        private void OnBackMainBtn()
        {
            PanelMgr.BackToMainPanel();
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            ResgistEvents();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            //退出就销毁
            FWPageMgr.Instance.ExitPage();
            base.DisPose();
        }

        public override void UpdateInput()
        {
            //触摸加载控制
            if (this.m_IsAllowHorzMove)
            {
                BaseTouchControl();
                BaseAnimtionControl();
            }
        }
    }
}
