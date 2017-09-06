//******************************************************************
// File Name:					DialogMgr
// Description:					DialogMgr class 
// Author:						lanjian
// Date:						2/27/2017 3:17:09 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    enum DialogType
    {
        UnKown,
        //兑换
        ConmonSimple,                     //商城确定购买
        Announcement,                     //兑换的公告栏  
        Waring,                           //兑换里面的各种提示
        ExchangeItemDetail,               //兑换物品的详解
        FillOutExchangeForm,              //填写兑换物品信息表格
        VerifyInfo,                       //信息核实
        InputPassWord,                    //输入密码
        MyExchange,                       //我的兑换
        ExchangeLogistics,                //兑换追踪物流  
        
        //交易    
        MyForSold,                        //我的寄售    
        PutUpSaleAndReadySale,            //寄售列表和待售列表
        OffAndBuy,

        //设置
        Setting,                           //游戏设置
        GetInfoError,                      //加载信息错误
        NetBrokenReconnect,                //掉线和重连

        //背包
        HandBombAndCommdityDetail,         //手雷和道具的详情界面
        CondsignForSaleDialog,             //交易
        ConfirmToSold,                     //确定出售
        ConfirmPutUp,                      //确定上架

        //摇奖
        LuckyJoy,                          //摇奖主界面
        LuckyJoyBetInput,                  //押注输入界面

    }



    delegate DialogBaseUI DialogCreator();
    //对话框管理
    class DialogMgr
    {
        private static Dictionary<DialogType, DialogCreator> sm_creators;
        private static DialogBaseUI sm_currentDialog;

        //--------------------------------------
        //public 
        //--------------------------------------
        public static DialogBaseUI CurrentDialog { get { return sm_currentDialog; } }
        //--------------------------------------
        //private
        //--------------------------------------
        static DialogMgr()
        {
            sm_creators = new Dictionary<DialogType, DialogCreator>();
            sm_creators.Add(DialogType.ConmonSimple, ConmonSimpleDialogUI.Create);
            sm_creators.Add(DialogType.Announcement, AnnouncementDialog.Create);
            sm_creators.Add(DialogType.Waring, WarningDialogUI.Create);
            sm_creators.Add(DialogType.ExchangeItemDetail, ExchangeItemDetailDialogUI.Create);
            sm_creators.Add(DialogType.FillOutExchangeForm, FilloutExchangeFormDialogUI.Create);
            sm_creators.Add(DialogType.VerifyInfo, VerifyExchangeItemInfoDialogUI.Create);
            sm_creators.Add(DialogType.InputPassWord, InputPassWrodDialogUI.Create);
            sm_creators.Add(DialogType.MyExchange, MyExchangeItemDialogUI.Create);
            sm_creators.Add(DialogType.ExchangeLogistics, ExchangeLogisticsDialogUI.Create);
            sm_creators.Add(DialogType.MyForSold, MyForSoldDialogUI.Create);
            sm_creators.Add(DialogType.PutUpSaleAndReadySale, PutupSaleAndReadySaleDialogUI.Create);
            sm_creators.Add(DialogType.Setting, SettingDialogUI.Create);
            sm_creators.Add(DialogType.HandBombAndCommdityDetail, HandBombAndCommdityDetailDialogUI.Create);
            sm_creators.Add(DialogType.CondsignForSaleDialog, ConsignForSaleDialogUI.Create);
            sm_creators.Add(DialogType.ConfirmToSold, ConfirmToSoldItemDialogUI.Create);
            sm_creators.Add(DialogType.ConfirmPutUp, ConfirmPutOnShelfDialogUI.Create);
            sm_creators.Add(DialogType.OffAndBuy, OffAndBuyAndPreBuyDialogUI.Create);
            sm_creators.Add(DialogType.GetInfoError, GetInfoErrorDialogUI.Create);
            sm_creators.Add(DialogType.NetBrokenReconnect, NetBorkenReconetDialogUI.Create);
            sm_creators.Add(DialogType.LuckyJoy, LuckyJoyBetDialogUI.Create);
            sm_creators.Add(DialogType.LuckyJoyBetInput, LuckyJoyBetInputDialog.Create);

            sm_currentDialog = null;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public static void Load(DialogType type)
        {
            //每次加载一个对话的时候  都要清理 以免一个对话调用另一个 对话时 存在两个sm_currentDialog
            if (sm_currentDialog != null)
            {
                sm_currentDialog.DisPose();
                sm_currentDialog = null;
            }
            DialogCreator creator;
            if (sm_creators.TryGetValue(type, out creator))
            {
                sm_currentDialog = creator();
            }
            sm_currentDialog.Init();
            //禁止左右滑动
            if(PanelMgr.CurrPanel!=null)
                PanelMgr.CurrPanel.IsAllowHorMove(false);
        }

        //销毁
        public static void Dispose()
        {
            if (sm_currentDialog != null)
            {
                sm_currentDialog.DisPose();
                sm_currentDialog = null;
            }
            //解除禁止左右滑动
            if (PanelMgr.CurrPanel != null)
                PanelMgr.CurrPanel.IsAllowHorMove(true);
        }
    }
}
