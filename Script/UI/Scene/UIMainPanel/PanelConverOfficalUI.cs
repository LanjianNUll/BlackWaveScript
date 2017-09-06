//******************************************************************
// File Name:					PanelConverOfficalUI
// Description:					PanelConverOfficalUI class 
// Author:						lanjian
// Date:						1/5/2017 5:19:04 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelConverOfficalUI: UIEventBase
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
            if (PanelMgr.CurrPanel != null)
            {
                PanelMgr.CurrPanel.UpdateInput();
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public void ConfirmButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_CONVEROFFICAL_CONFIMR_BTN);
        }

        public void CancelButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_CONVEROFFICAL_CANEL_BTN);
        }

        public void AutoLoginButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_CONVEROFFICAL_AUTOLOGIN_BTN);
        }
    }
}
