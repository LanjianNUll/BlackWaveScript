//******************************************************************
// File Name:					MainUI
// Description:					MainUI class 
// Author:						lanjian
// Date:						1/4/2017 5:13:35 PM
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
    class MainUI : UIEventBase
    {
        
        void Start()
        {
            if (UISceneMgr.CurrScene != null)
            {
                UISceneMgr.CurrScene.BindScript(this);
            }
        }

        void Update()
        {
        }

        void OnDestroy()
        {

        }

        //--------------------------------------
        //private
        //--------------------------------------


        //--------------------------------------
        //public
        //--------------------------------------
        public void PlayerButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_PLAYER_BTN);
        }

        public void GameButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_GAME_BTN,null);
        }

        public void BagButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_BAG_BTN);
        }

        public void ShopButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_SHOP_BTN);
        }

        public void ExchangeButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_EXCHANGE_BTN);
        }

        public void RankButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_RANK_BTN);
        }

        public void ConverToOfficalButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_CONVERTOOFFICAL_BTN);
        }

        public void FirendsButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_FIREND_BTN);
        }

        public void Email()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_EMAIL_BTN);
        }

        public void DealButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_DEAL_BTN);
        }

        public void ConfirmAndCloseDialogOne()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_CLOSEDIALOGONE_BTN);
        }

        public void SettingBtnClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_Setting_BTN);
        }

        public void LuckJoyBtnClick()
        {
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_LuckJoy_BTN);
        }
    }
}
