//******************************************************************
// File Name:					GameUI
// Description:					GameUI class 
// Author:						lanjian
// Date:						1/13/2017 9:56:31 AM
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
    class GameUI: UIEventBase
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
        public void BackButtonClick()
        {
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.GAME_UI_BACK_TO_MAIN);
        }
    }
}
