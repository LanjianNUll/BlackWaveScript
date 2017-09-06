//******************************************************************
// File Name:					PanelMgr
// Description:					PanelMgr class 
// Author:						lanjian
// Date:						1/5/2017 3:35:00 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    /// <summary>
    /// 分页的类型
    /// </summary>
    enum PanelType
    {
        Player,                         //角色面板
        Bag,                            //背包    
        Shop,                           //商城
        Exchange,                       //兑换
        Rank,                           //排行榜
        ConverOffical,                  //转换为正式账号
        Firends,
        Email,
        Deal,
        LuckJoy,                        //摇奖
    }
    delegate PanelBase PanelCreator();
    static class PanelMgr
    {
        private static Dictionary<PanelType, PanelCreator> sm_creators;
        private static PanelBase sm_CurrPanel;

        static PanelMgr()
        {
            sm_creators = new Dictionary<PanelType, PanelCreator>();
            sm_creators.Add(PanelType.Player, PanelPlayer.Create);
            sm_creators.Add(PanelType.Bag, PanelBag.Create);
            sm_creators.Add(PanelType.Shop, PanelShop.Create);
            sm_creators.Add(PanelType.Exchange, PanelExchange.Create);
            sm_creators.Add(PanelType.Rank, PanelRank.Create);
            sm_creators.Add(PanelType.ConverOffical, PanelConverOffical.Create);
            sm_creators.Add(PanelType.Firends, PanelFriend.Create);
            sm_creators.Add(PanelType.Email, PanelEmail.Create);
            sm_creators.Add(PanelType.Deal, PanelDeal.Create);
            sm_creators.Add(PanelType.LuckJoy,PanelLuckJoy.Create);

            sm_CurrPanel = null;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static PanelBase CurrPanel { get { return sm_CurrPanel; } }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static void Load(PanelType type,bool isFullScreen = true)//加入的子界面是否全屏
        {
            DisPlayTopProp(isFullScreen);
            PanelCreator creator;
            if (sm_creators.TryGetValue(type, out creator))
            {
                sm_CurrPanel = creator();
            }
            sm_CurrPanel.Init();
        }

        //销毁
        public static void Dispose()
        {
            if (sm_CurrPanel != null)
            {
                sm_CurrPanel.DisPose();
                sm_CurrPanel = null;
            }
        }

        //是否显示头部状态
        public static void DisPlayTopProp(bool isFullScreen)
        {
            //隐藏主界面   加入的子界面是否全屏(不隐藏顶部)
            if (isFullScreen)
            {
                //FW.UI.UISceneMain.sm_currentPanel.SetActive(false);
                NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel, false);
            }
            else
            {
                //FW.UI.UISceneMain.sm_currentPanel.transform.Find("center").gameObject.SetActive(false);
                //FW.UI.UISceneMain.sm_currentPanel.transform.Find("bottom").gameObject.SetActive(false);
                NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel, true);
                NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel.transform.Find("top").gameObject, true);
                NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel.transform.Find("center").gameObject, false);
                NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel.transform.Find("bottom").gameObject, false);
            }
        }

        public static void BackToMainPanel()
        {
            Dispose();
            NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel,true);
            NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel.transform.Find("center").gameObject, true);
            NGUITools.SetActive(FW.UI.UISceneMain.sm_currentPanel.transform.Find("bottom").gameObject, true);
            //回调 刷新主界面UI
            Event.FWEvent.Instance.Call(FW.Event.EventID.MAIN_UI_REFRESHMAINUI);
        }
    }
}
