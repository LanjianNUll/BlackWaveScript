//******************************************************************
// File Name:					PanelDeal
// Description:					PanelDeal class 
// Author:						lanjian
// Date:						2/5/2017 3:55:12 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelDeal:PanelBase
    {
        protected PanelDeal()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/DealPanel";
            this.panelType = PanelType.Deal;
        }

        public static PanelDeal Create()
        {
            return new PanelDeal();
        }

        private long m_time;

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).gameObject;
            FWPageMgr.Instance.LoadDealPage();
            pageTotalCount = FWPageMgr.Instance.CurrentPageCount;
            LoadPageNum(pageTotalCount);

            Utility.Utility.GetUIEventListener(this.RootObj.transform.GetChild(0).GetChild(2).GetChild(0).gameObject)
                .onClick = OnShowMySoldList;
        }

        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
        }

        //我的寄售
        private void OnShowMySoldList(GameObject go)
        {
            DialogMgr.Load(DialogType.MyForSold);
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg());
        }

        private void OnBackMainBtn()
        {
            PanelMgr.BackToMainPanel();
        }

        private void SecondInvoke()
        {
            //对话框的Update
            if (DialogMgr.CurrentDialog != null)
                DialogMgr.CurrentDialog.SecondUpdateDialog();
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
