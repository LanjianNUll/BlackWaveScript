//******************************************************************
// File Name:					PanelRank
// Description:					PanelRank class 
// Author:						lanjian
// Date:						1/5/2017 4:21:03 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelRank:PanelBase
    {
        protected PanelRank()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/RankPanel";
            this.panelType = PanelType.Rank;
        }

        public static PanelRank Create()
        {
            return new PanelRank();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {

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

            base.DisPose();
        }
    }
}
