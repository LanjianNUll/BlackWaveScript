//******************************************************************
// File Name:					UISceneGame.cs
// Description:					UISceneGame class 
// Author:						wuwei
// Date:						2017.01.04
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FW.Game;
using FW.Event;
using Network.Serializer;
using System.Text;
using FW.Task;

namespace FW.UI
{
    class UISceneGame : UISceneBase
    {
        protected UISceneGame()
        {
            this.m_resName = "UIRootPrefabs/GameUIRoot";
            m_chantPanel = new ChatPanel();
            m_TaskGamePanel = new TaskGamePanel();
        }

        public static UISceneBase Create()
        {
            return new UISceneGame();
        }
        private Transform m_topTran;

        private ChatPanel m_chantPanel;                     //聊天面板
        private TaskGamePanel m_TaskGamePanel;              //任务相关面板
        //头部信息显示
        private UILabel m_PlayerNameLabel;
        private UILabel m_PCLevelLabel;
        //private UILabel m_PhoneLevelLabel;
        private UISprite m_ExpFillSprite;
        private UISprite m_ExpThumbSprite;
        private UILabel m_GoldCountLabel;
        private UILabel m_DiamondCountLabel;
        private UILabel m_BagCountLabel;

        private Transform m_ZeroTranformThumb;   //计算expthumb的位置
        private Transform m_OneTranformThumb;

        private bool isReconnecting;                //是否正在重连中
        //--------------------------------------
        //private 
        //--------------------------------------
        private void FindAllUI()
        {
            m_topTran = this.RootObj.transform.Find("top");
            FillDataRoleInfo();
        }

        //更新头部角色信息
        private void FillDataRoleInfo()
        {
            FW.Role.Role role = FW.Role.Role.Instance();
            m_PlayerNameLabel = m_topTran.Find("playerInfo").GetChild(0).GetComponent<UILabel>();
            m_PCLevelLabel = m_topTran.Find("playerInfo").GetChild(3).GetComponent<UILabel>();
            m_ExpFillSprite = m_topTran.Find("playerInfo").Find("expFill").GetComponent<UISprite>();
            m_ExpThumbSprite = m_topTran.Find("playerInfo").Find("expthumb").GetComponent<UISprite>();
            m_ZeroTranformThumb = m_topTran.Find("playerInfo").Find("oneTranformThumb").GetComponent<Transform>();
            m_OneTranformThumb = m_topTran.Find("playerInfo").Find("zeroTanformThumb").GetComponent<Transform>();
            m_GoldCountLabel = m_topTran.Find("money").Find("goldCount").GetComponent<UILabel>();
            m_DiamondCountLabel = m_topTran.Find("money/diamondCount").GetComponent<UILabel>();
            m_BagCountLabel = m_topTran.Find("money/bagCount").GetComponent<UILabel>();
           
            m_PlayerNameLabel.text = role.Name;
            m_PCLevelLabel.text = "LV" + role.Level;
            m_GoldCountLabel.text = role.Cash + "";
            m_DiamondCountLabel.text = role.Diamond + "";
            m_BagCountLabel.text = role.Gold + "";
            int extp = role.Exp;
            int extpMax = role.ExpMax;
            float percentage = (extpMax == 0 ? 0 : extp / extpMax);   //比例
            float distance = m_OneTranformThumb.transform.localPosition.x - m_ZeroTranformThumb.localPosition.x;
            m_ExpThumbSprite.transform.localPosition -= new Vector3(distance * percentage, 0, 0);
            m_ExpFillSprite.fillAmount = percentage;

        }

        private void RegistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_UI_BACK_TO_MAIN,OnBackMain);
            //玩家消耗点 (现金 砖石 金币  经验 等级 天赋)
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Cash, OnRoleCashChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Diamond, OnRoleDiamondChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Gold, OnRoleGoldChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Exp, OnRoleExpChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Level, OnRoleLevelChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Point, OnRolePointChange);
            //断线重连中
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_START, OnReconnetStart);
            //断线重新连接成功
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
        }

        private void OnBackMain()
        {
            Scene.SceneMgr.Enter(Scene.SceneType.Main);
            Event.FWEvent.Instance.Call(FW.Event.EventID.MAIN_UI_REFRESHMAINUI);
        }

        //角色消耗点的回调
        private void OnRoleCashChange(FW.Event.EventArg args)
        {
            m_GoldCountLabel.text = args[1].ToString();
        }

        private void OnRoleDiamondChange(FW.Event.EventArg args)
        {
            m_DiamondCountLabel.text = args[1].ToString();
        }

        private void OnRoleGoldChange(FW.Event.EventArg args)
        {
            m_BagCountLabel.text = args[1].ToString();
        }

        private void OnRoleExpChange(FW.Event.EventArg args)
        {
            //int 当前值，int 最大值
            int extp = (int)args[1];
            int extpMax = (int)args[2];
            float percentage = (extpMax == 0 ? 0 : extp / extpMax);   //比例
            float distance = m_OneTranformThumb.transform.localPosition.x - m_ZeroTranformThumb.localPosition.x;
            m_ExpThumbSprite.transform.localPosition -= new Vector3(distance * percentage, 0, 0);
            m_ExpFillSprite.fillAmount = percentage;
        }

        private void OnRoleLevelChange(FW.Event.EventArg args)
        {
            m_PCLevelLabel.text = "LV" + args[1].ToString();
        }

        private void OnRolePointChange(FW.Event.EventArg args)
        {

        }

        private void OnReconnetStart()
        {
            if (!isReconnecting)
            {
                isReconnecting = true;
                DialogMgr.Load(DialogType.NetBrokenReconnect);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(0));
            }
        }

        private void OnReconnetSuccess()
        {
            isReconnecting = false;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public override void Init()
        {
            base.Init();
        }
        
        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_UI_BACK_TO_MAIN, OnBackMain);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Cash, OnRoleCashChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Diamond, OnRoleDiamondChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Gold, OnRoleGoldChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Exp, OnRoleExpChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Level, OnRoleLevelChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Point, OnRolePointChange);
            //断线重连中
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_START, OnReconnetStart);
            //断线重新连接成功
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
            this.m_chantPanel.DisPose();
            this.m_TaskGamePanel.DisPose();
            base.DisPose();
        }

        //绑定消息
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            RegistEvents();
            //聊天面板的初始化
            m_chantPanel.Init();
            //任务面板的初始化
            m_TaskGamePanel.Init();
        }
    }
}