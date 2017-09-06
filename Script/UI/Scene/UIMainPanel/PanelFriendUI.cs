//******************************************************************
// File Name:					PanelFriendUI
// Description:					PanelFriendUI class 
// Author:						lanjian
// Date:						1/5/2017 6:46:03 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelFriendUI:UIEventBase
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
        //主面板
        public void BackMainPaneButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN);
        }

        public void AddFriendBUttonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_FRIEND_ADDFRIEND_BTN);
        }

        //弹出确定或取消面板
        //确定加好友的按钮
        public void ConfirmAddFriendButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_FRIEND_CONFIRM_ADDFRIEND_BTN);
        }

        //取消加好友的按钮
        public void CancelAddFriendButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_FRIEND_CANCEL_ADDFRIEND_BTN);
        }

        //弹出确定或取消面板
        //删除确定
        public void ConfirmDeleteFriendButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_FRIEND_CONFIRM_DELETEFRIEND_BTN);
        }

        //删除取消
        public void CancelDeleteFriendButtonClick()
        {
            Event.FWEvent.Instance.Call(Event.EventID.PANEL_FRIEND_CANCEL_DELETEFRIEND_BTN);
        }
    }
}
