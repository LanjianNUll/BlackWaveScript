//******************************************************************
// File Name:					PanelBag
// Description:					PanelBag class 
// Author:						lanjian
// Date:						1/5/2017 4:14:02 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    class PanelBag:PanelBase
    {
        protected PanelBag()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/BagPackagePanel";
            this.panelType = PanelType.Bag;
        }

        public static PanelBag Create()
        {
            return new PanelBag();
        }
        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).gameObject;
            //加载背包页面
            FWPageMgr.Instance.LoadBagPackPage();
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
            //销毁
            FWPageMgr.Instance.ExitPage();
            base.DisPose();
        }

        public override void UpdateInput()
        {
            //update
            FWPageMgr.Instance.ScrollViewItemBaseUpdate();
            //触摸加载控制
            if (this.m_IsAllowHorzMove)
            {
                BaseTouchControl();
                BaseAnimtionControl();
            }
        }
    }
}
