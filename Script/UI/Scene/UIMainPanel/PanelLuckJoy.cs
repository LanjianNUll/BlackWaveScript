//******************************************************************
// File Name:					PanelLuckJoy
// Description:					PanelLuckJoy class 
// Author:						lanjian
// Date:						5/9/2017 10:05:11 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelLuckJoy : PanelBase
    {
        protected PanelLuckJoy()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/LuckJoyPanel";
            this.panelType = PanelType.LuckJoy;
        }

        public static PanelLuckJoy Create()
        {
            return new PanelLuckJoy();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            //进入摇奖
            DialogMgr.Load(DialogType.LuckyJoy);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);

            scrollView = PanelMgr.CurrPanel.RootObj.transform.Find("center")
               .GetChild(0).gameObject;
            FWPageMgr.Instance.LoadLuckJoyPage();
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
            //这里这个返回为中奖Rule和record子界面的返回  
            DialogMgr.Load(DialogType.LuckyJoy);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            ResgistEvents();
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

        public override void DisPose()
        {
            //中奖记录写入文件
            LuckyJoy.LuckyJoyMgr.ExitLuckyBet();

            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            //退出就销毁
            FWPageMgr.Instance.ExitPage();
            base.DisPose();
        }
    }
}
