//******************************************************************
// File Name:					OffAndBuyAndPreBuyDialogUI
// Description:					OffAndBuyAndPreBuyDialogUI class 
// Author:						lanjian
// Date:						3/15/2017 3:29:10 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Deal;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    //下架，预购， 购买的 确定dialogUI
    class OffAndBuyAndPreBuyDialogUI:DialogBaseUI
    {
        protected OffAndBuyAndPreBuyDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/WarningDialog";
            this.m_DType = DialogType.OffAndBuy;

            //注册事件
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Deal_offShelveItem, OnOffShelveGoodBack);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Deal_itemBought, OnBuyGoodBack);
        }

        public static OffAndBuyAndPreBuyDialogUI Create()
        {
            return new OffAndBuyAndPreBuyDialogUI();
        }

        private DealItemInfo m_currentDeal;
        private DealFitterItem m_currentFitter;
        private bool isCanbuy;

        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("cancel")).onClick = OnCancel;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            NGUITools.SetActive(m_DialogUIGo.transform.Find("BackBtn").gameObject,false);
            NGUITools.SetActive(m_DialogUIGo.transform.Find("cancel").gameObject, true);
        }

        private void OnConfirm(GameObject go)
        {
           
            if ((DialogType)this.m_currentArgs[1] == DialogType.MyForSold)
            {
                //下架 请求
                this.m_currentDeal.OffShelveGood();
            }
            if ((DialogType)this.m_currentArgs[1] == DialogType.PutUpSaleAndReadySale && isCanbuy)
            {
                //购买请求
                this.m_currentDeal.Buy();
            }
            //钱不足 购买不来
            if ((DialogType)this.m_currentArgs[1] == DialogType.PutUpSaleAndReadySale && !isCanbuy)
            {
                this.OnCancel(null);
            }
        }

        //下架返回
        private void OnOffShelveGoodBack(FW.Event.EventArg args)
        {
            int ret = (int)args[1];
            if (ret == 0)
            {
                Utility.Utility.NotifyStr("下架成功！！");

            }
            else if (ret == 12)
            {
                Utility.Utility.NotifyStr("预售物品不能下架！！");
            }
            else
            {
                Utility.Utility.NotifyStr("下架失败！！错误码"+ret);
            }
            DialogMgr.Load(DialogType.MyForSold);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }

        private void OnBuyGoodBack(FW.Event.EventArg args)
        {
            int pageNum = m_currentDeal.State == ItemState.ForSale ? 0 : 1;
            int ret = (int)args[1];
            if (ret == 0)
            {
                if(m_currentDeal.State == ItemState.ForSale)
                    Utility.Utility.NotifyStr("购买成功！！");
                if (m_currentDeal.State == ItemState.WaitForSale)
                    Utility.Utility.NotifyStr("预购成功！！");
            }
            else if (ret == 11)
            {
                Utility.Utility.NotifyStr("不能购买自己上架的物品！！！");
            }
            else if (ret == 15)
            {
                    Utility.Utility.NotifyStr("你已经预购了该物品！！！");
            }
            else if (ret == 3)
            {
                Utility.Utility.NotifyStr("购买失败！！！找不到该物品！！");
            }
            else if (ret == 9)
            {
                Utility.Utility.NotifyStr("购买失败！ 找不到玩家交易信息！");
            }
            DialogMgr.Load(DialogType.PutUpSaleAndReadySale);
            DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(m_currentFitter, pageNum));
        }

        private void FillDataUI()
        {
            string text = "";
            if ((DialogType)this.m_currentArgs[1] == DialogType.MyForSold)
            {
                m_currentDeal = (DealItemInfo)this.m_currentArgs[0];
                text = "你将下架" + m_currentDeal.Item.Name + "!!!";
                m_DialogUIGo.transform.Find("content").GetComponent<UILabel>().text = text;
            }
            if ((DialogType)this.m_currentArgs[1] == DialogType.PutUpSaleAndReadySale)
            {
                m_currentDeal = (DealItemInfo)this.m_currentArgs[0];
                if (Role.Role.Instance().Gold >= m_currentDeal.Price)
                {
                    m_currentDeal.SetGoldAndDiamondPrice(m_currentDeal.Price,0);
                    text = "你将花费 [a4edf4]" + m_currentDeal.Price+" [-]黄金购买" + m_currentDeal.Item.Name + "!!!";
                    isCanbuy = true;
                    //设置商品花费黄金和钻石（根据用户账号货币）
                    m_currentDeal.SetGoldAndDiamondPrice(m_currentDeal.Price,0);
                }

                if (Role.Role.Instance().Gold < m_currentDeal.Price)
                {
                    if (Role.Role.Instance().Diamond >= m_currentDeal.Price / 10)
                    {
                        int [] array = Utility.Utility.Calculate(m_currentDeal.Price, Role.Role.Instance().Gold);
                        text = "你将花费 [a4edf4]" + array[0] + " [-]黄金和[a4edf4]" + array[1] + "[-]钻石购买" + m_currentDeal.Item.Name + "!!!";
                        isCanbuy = true;
                        //设置商品花费黄金和钻石（根据用户账号货币）
                        m_currentDeal.SetGoldAndDiamondPrice(array[0], array[1]);
                    }
                    else
                    {
                        isCanbuy = false;
                        text = "你账户货币不足" + m_currentDeal.Item.Name + "!!!";
                    }
                }
            }
            m_DialogUIGo.transform.Find("content").GetComponent<UILabel>().text = text;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void OnCancel(GameObject go)
        {
            if ((DialogType)this.m_currentArgs[1] == DialogType.MyForSold)
            {
                DialogMgr.Load(DialogType.MyForSold);
                DialogMgr.CurrentDialog.ShowCommonDialog(null);
            }
            if ((DialogType)this.m_currentArgs[1] == DialogType.PutUpSaleAndReadySale)
            {
                DialogMgr.Load(DialogType.PutUpSaleAndReadySale);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(m_currentFitter));
            }
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            //处理来着售卖列表 返回时，保存请求分类信息
            m_currentFitter = (DealFitterItem)args[2];
            m_currentDeal = (DealItemInfo)args[0];
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI();
            this.OpenDialog();
        }

        public override void DisPose()
        {
            //注销事件
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Deal_offShelveItem, OnOffShelveGoodBack);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Deal_itemBought, OnBuyGoodBack);
            base.DisPose();
        }
    }
}
