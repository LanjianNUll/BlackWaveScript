//******************************************************************
// File Name:					UISceneMain.cs
// Description:					UISceneMain class 
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
using FW.Player;

namespace FW.UI
{
    class UISceneMain : UISceneBase
    {
        //头部信息显示
        private UILabel m_PlayerNameLabel;
        private UILabel m_PCLevelLabel;
        private UISprite m_ExpFillSprite;
        private UISprite m_ExpThumbSprite;
        private UILabel m_GoldCountLabel;
        private UILabel m_DiamondCountLabel;
        private UILabel m_BagCountLabel;
        private GameObject m_EmailNotictSprite;    //是否有邮件

        private Transform m_ZeroTranformThumb;   //计算expthumb的位置
        private Transform m_OneTranformThumb;
        private bool isPCOnline;
        private bool isLoadPlayerInfo;             //是否加载过来头部
        private bool isReconnecting;                //是否正在重连中

        public static GameObject sm_currentPanel;    //主界面的panel

        protected UISceneMain()
        {
            this.m_resName = "UIRootPrefabs/MainUIRoot";
        }

        public static UISceneBase Create()
        {
            return new UISceneMain();
        }
        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            sm_currentPanel = GameObject.Find("UIRootContainer/MainUIRoot(Clone)/MainPanel");
            string playerInforootPath = "UIRootContainer/MainUIRoot(Clone)/MainPanel/top/";
            m_PlayerNameLabel = GameObject.Find(playerInforootPath + "playerInfo/name").GetComponent<UILabel>();
            m_PCLevelLabel = GameObject.Find(playerInforootPath + "playerInfo/mLevel").GetComponent<UILabel>();
            //m_PhoneLevelLabel = GameObject.Find(playerInforootPath + "playerInfo/mLevel").GetComponent<UILabel>();
            m_ExpFillSprite = GameObject.Find(playerInforootPath+"playerInfo/expFill").GetComponent<UISprite>();
            m_ExpThumbSprite = GameObject.Find(playerInforootPath + "playerInfo/expthumb").GetComponent<UISprite>();
            m_ZeroTranformThumb = GameObject.Find(playerInforootPath + "playerInfo/oneTranformThumb").GetComponent<Transform>();
            m_OneTranformThumb = GameObject.Find(playerInforootPath + "playerInfo/zeroTanformThumb").GetComponent<Transform>();
            m_GoldCountLabel = GameObject.Find(playerInforootPath + "money/goldCount").GetComponent<UILabel>();
            m_DiamondCountLabel = GameObject.Find(playerInforootPath + "money/diamondCount").GetComponent<UILabel>();
            m_BagCountLabel = GameObject.Find(playerInforootPath + "money/bagCount").GetComponent<UILabel>();
            m_EmailNotictSprite = GameObject.Find("UIRootContainer/MainUIRoot(Clone)/MainPanel/bottom/Email/countRed").gameObject;
            this.ResgitEvent();
            this.FillingDataToUI();
        }

        /// <summary>
        /// 注册主界面的事件
        /// </summary>
        private void ResgitEvent()
        {
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_PLAYER_BTN, OnPlayerBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_GAME_BTN, OnGameBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_BAG_BTN, OnBagBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_SHOP_BTN, OnShopBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_EXCHANGE_BTN, OnExchangeBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_RANK_BTN, OnRankBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_CONVERTOOFFICAL_BTN, OnConverToOfficalBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_FIREND_BTN, OnFirendBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_EMAIL_BTN, OnEmailBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_DEAL_BTN, OnDealBtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_CLOSEDIALOGONE_BTN, OnCloseDialogOne);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_Setting_BTN, OnSettingbtnClick);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_LuckJoy_BTN, OnLuckJoybtnClick);
            
            //刷新主界面的UI
            Event.FWEvent.Instance.Regist(FW.Event.EventID.MAIN_UI_REFRESHMAINUI, OnRefreshMainUI);
            //注册角色初始化
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_inited, OnRoleInit);
            //是否有新邮件到来
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Mail_Receive_New, OnReciveNewMail);
            //是否存在未读邮件
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Mail_inited, OnHaveNotReadEmail);
            //是否pc端在线  不能去背包（若在背包弹出来）
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_online_Change, OnPCOnlineStateChange);
            //玩家消耗点 (现金 砖石 金币  经验 等级 天赋)
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Cash, OnRoleCashChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Diamond, OnRoleDiamondChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Gold, OnRoleGoldChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Exp, OnRoleExpChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Level, OnRoleLevelChange);
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Role_Change_Point, OnRolePointChange);
            //加载信息错误返回登录
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Get_Info_Error, OnBackLoginUI);
            //断线重连中
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_START,OnReconnetStart);
            //断线重新连接成功
            Event.FWEvent.Instance.Regist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
        }

        private void FillingDataToUI()
        {
            //角色头部消息
            isLoadPlayerInfo = true;
            OnFillRoleInfo(null);
            //是否游客身份 是显示转换正式账号按钮
            ShowOrHideConvertoOff();
        }

        private void ShowOrHideConvertoOff()
        {
            NGUITools.SetActive(this.m_rootObj.transform.Find("MainPanel/center/ConverOffical").gameObject,
                FW.Login.LoginHandler.IsTourist);
            if (FW.Login.LoginHandler.IsTourist)
                this.m_rootObj.transform.Find("MainPanel/center").localPosition = Vector3.zero;
            else
                this.m_rootObj.transform.Find("MainPanel/center").localPosition = new Vector3(0,90,0);
        }

        private void OnRoleInit(FW.Event.EventArg args)
        {
            ///商城列表请求  姑且先放在这里请求
            OnFillRoleInfo(args);
            Store.StoreMgr.RequestItemsByAuto();
        }

        private void OnFillRoleInfo(FW.Event.EventArg args)
        {
            FW.Role.Role role = FW.Role.Role.Instance();
            m_PlayerNameLabel.text = role.Name;
            m_PCLevelLabel.text = "LV" + role.Level;

            m_GoldCountLabel.text = role.Cash.ToString();
            m_DiamondCountLabel.text = role.Diamond.ToString();
            m_BagCountLabel.text = role.Gold.ToString();
            int extp = role.Exp;
            int extpMax = role.ExpMax;
            float percentage = (extpMax == 0 ? 0 : extp / extpMax);   //比例
            float distance = m_OneTranformThumb.transform.localPosition.x - m_ZeroTranformThumb.localPosition.x;
            m_ExpThumbSprite.transform.localPosition -= new Vector3(distance*percentage, 0,0);
            m_ExpFillSprite.fillAmount = percentage;
            //Debug.Log("玩家ID :" + role.ID + "玩家姓名:" + role.Name + "玩家等级-" 
            //    + role.Level+"经验++"+role.Exp+ "Max经验"+role.ExpMax
            //    +"现金："+role.Cash+"砖石："+role.Diamond+"金币："+role.Gold);
            //是否有未读邮件
            if (Role.Role.Instance().MailProctor.HasNewMail)
                NGUITools.SetActive(m_EmailNotictSprite, true);
            else 
                NGUITools.SetActive(m_EmailNotictSprite, false);
            
        }

        //接受新邮件的回调
        private void OnReciveNewMail()
        {
            NGUITools.SetActive(m_EmailNotictSprite, true);
        }

        private void OnHaveNotReadEmail()
        {
            //是否有未读邮件
            if (Role.Role.Instance().MailProctor.HasNewMail)
                NGUITools.SetActive(m_EmailNotictSprite, true);
            else
                NGUITools.SetActive(m_EmailNotictSprite, false);
        }

        private void OnPlayerBtnClick()
        {
            //Debug.Log("进入角色界面");
            FW.UI.PanelMgr.Load(PanelType.Player,false);
        }

        private void OnGameBtnClick(Event.EventArg args)
        {
            if (args != null)
                Role.Role.Instance().ToGamePrivateChat = (Gameplayer)args[0];
            //进入Game场景
            FW.Scene.SceneMgr.Enter(Scene.SceneType.Game);
        }

        private void OnBagBtnClick()
        {
            //进入背包之前判断
            if (!isPCOnline)
            {
                NGUITools.SetActive(sm_currentPanel.transform.GetChild(3).gameObject, false);
                FW.UI.PanelMgr.Load(PanelType.Bag, false);
            }
            else
            {
                NGUITools.SetActive(sm_currentPanel.transform.GetChild(3).gameObject, true);
            }
        }

        private void OnShopBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Shop,false);

        }

        private void OnExchangeBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Exchange,false);
        }

        private void OnRankBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Rank);
        }

        private void OnConverToOfficalBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.ConverOffical);
        }

        private void OnFirendBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Firends,false);
        }

        private void OnEmailBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Email, false);
        }

        private void OnDealBtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.Deal,false);
        }

        private void OnCloseDialogOne()
        {
            NGUITools.SetActive(sm_currentPanel.transform.GetChild(3).gameObject, false);
        }

        private void OnSettingbtnClick()
        {
            DialogMgr.Load(DialogType.Setting);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }

        private void OnLuckJoybtnClick()
        {
            FW.UI.PanelMgr.Load(PanelType.LuckJoy, false);
        }

        private void OnRefreshMainUI()
        {
            OnFillRoleInfo(null);
            //检查是否是游客身份
            ShowOrHideConvertoOff();
            Debug.Log("返回刷新主界面,刷新角色信息");
        }

        //用户在线状态变化     1: 仅PC在线, 2: 仅手机在线, 3:PC和手机同时在线
        private void OnPCOnlineStateChange(FW.Event.EventArg args)
        {
            int state = (int)args[1];
            if (state == 1 || state == 3)
            {
                isPCOnline = true;
                BackMainScence();
            }
            else
                isPCOnline = false;
        }

        private void BackMainScence()
        {
            //如果是在背包页就跳粗来
            if(PanelMgr.CurrPanel != null&& PanelMgr.CurrPanel.PanelTypeTY == PanelType.Bag)
            {
                PanelMgr.BackToMainPanel();
                Utility.Utility.NotifyStr("PC端上线，手机端背包限制使用！！！");
            }
        }

        //private UILabel m_PCLevelLabel;
        //private UILabel m_PhoneLevelLabel;
        //private UISprite m_ExpFillSprite;
        //private UISprite m_ExpThumbSprite;
        //private UILabel m_GoldCountLabel;
        //private UILabel m_DiamondCountLabel;
        //private UILabel m_BagCountLabel;

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
                //回到主界面，避免一些操作
                PanelMgr.BackToMainPanel();
                DialogMgr.Load(DialogType.NetBrokenReconnect);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(0));
            }
        }

        private void OnReconnetSuccess()
        {
            isReconnecting = false;
        }

        //加载错误的返回  重新回到登录界面
        private void OnBackLoginUI(FW.Event.EventArg arg)
        {
            DialogMgr.Load(DialogType.GetInfoError);
            DialogMgr.CurrentDialog.ShowCommonDialog(arg);
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
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_PLAYER_BTN, OnPlayerBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_GAME_BTN, OnGameBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_BAG_BTN, OnBagBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_SHOP_BTN, OnShopBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_EXCHANGE_BTN, OnExchangeBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_RANK_BTN, OnRankBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_CONVERTOOFFICAL_BTN, OnConverToOfficalBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_FIREND_BTN, OnFirendBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_EMAIL_BTN, OnEmailBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_DEAL_BTN, OnDealBtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_CLOSEDIALOGONE_BTN, OnCloseDialogOne);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_Setting_BTN, OnSettingbtnClick);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_LuckJoy_BTN, OnLuckJoybtnClick);

            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.MAIN_UI_REFRESHMAINUI, OnRefreshMainUI);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_inited, OnRoleInit);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Mail_Receive_New, OnReciveNewMail);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Mail_inited, OnHaveNotReadEmail);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_online_Change, OnPCOnlineStateChange);

            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Cash, OnRoleCashChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Diamond, OnRoleDiamondChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Gold, OnRoleGoldChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Exp, OnRoleExpChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Level, OnRoleLevelChange);
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Role_Change_Point, OnRolePointChange);
            //断线重连中
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_START, OnReconnetStart);
            //断线重连成功
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.NET_RECONN_SUCCESS, OnReconnetSuccess);
            base.DisPose();
        }

        //绑定消息
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            //刷新角色信息
            //this.OnFillRoleInfo(null);
            //初始化一些请求
            //商城请求
           
        }
    }
}