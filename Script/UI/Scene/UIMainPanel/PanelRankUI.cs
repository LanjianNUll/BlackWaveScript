//******************************************************************
// File Name:					PanelRankUI
// Description:					PanelRankUI class 
// Author:						lanjian
// Date:						1/5/2017 6:47:29 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelRankUI:UIEventBase
    {
        void Start()
        {
            if (PanelMgr.CurrPanel != null)
            {
                PanelMgr.CurrPanel.BindScript(this);
            }
        }

        void Update()
        {

        }

        //--------------------------------------
        //public
        //--------------------------------------
        public void BackMainPaneButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN);
        }
    }
}
