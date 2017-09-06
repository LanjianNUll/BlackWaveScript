//******************************************************************
// File Name:					PanelShop
// Description:					PanelShop class 
// Author:						lanjian
// Date:						1/5/2017 4:17:15 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    class PanelShop:PanelBase
    {
        protected PanelShop()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/ShopPackagePanel";
            this.panelType = PanelType.Shop;
        }

        public static PanelShop Create()
        {
            return new PanelShop();
        }

        private long m_time;

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).gameObject;

            FWPageMgr.Instance.LoadShopPage();
            
            pageTotalCount = FWPageMgr.Instance.CurrentPageCount;
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

        private void SecondInvoke()
        {
            //控制是否一秒调用一次
            FWPageMgr.Instance.ScrollViewItemBaseSecondInvoke();
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            ResgistEvents();
            //一秒调用一次
            m_time = Timer.Regist(0, 1, SecondInvoke);
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

        public override void DisPose()
        {
            Timer.Cancel(m_time);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            //销毁
            FWPageMgr.Instance.ExitPage();
            base.DisPose();
        }
    }
}
