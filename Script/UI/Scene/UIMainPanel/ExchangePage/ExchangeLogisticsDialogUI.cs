//******************************************************************
// File Name:					ExchangeLogisticsDialogUI
// Description:					ExchangeLogisticsDialogUI class 
// Author:						lanjian
// Date:						3/3/2017 4:12:37 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Exchange;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 兑换追踪  物流
    /// </summary>
    class ExchangeLogisticsDialogUI:DialogBaseUI
    {
        protected ExchangeLogisticsDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/ExchangeItemStateInfoDialog";
            this.m_DType = DialogType.ExchangeLogistics;
        }

        public static ExchangeLogisticsDialogUI Create()
        {
            return new ExchangeLogisticsDialogUI();
        }

        private ExchangeItemOrder m_currentOrder;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            //物品图片
            //Texture texture = 
            //item.transform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
            Transform descTra = this.m_DialogUIGo.transform.Find("desc");
            descTra.Find("name").GetComponent<UILabel>().text = m_currentOrder.Item.ExchangePrizeName;
            descTra.Find("price").GetComponent<UILabel>().text = m_currentOrder.TotalPay.ToString();
            descTra.Find("num").GetComponent<UILabel>().text = m_currentOrder.Count.ToString();
            descTra.Find("detailDesc").GetComponent<UILabel>().text = m_currentOrder.Item.ShortDesc;
            descTra.Find("orderNum").GetComponent<UILabel>().text = m_currentOrder.OrderId;
            //填充物流信息
            FillScrollData();
        }

        private void FillScrollData()
        {

        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void OnBack(GameObject go)
        {
            //返回 我的兑换信息界面
            DialogMgr.Load(DialogType.MyExchange);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.m_currentOrder = (ExchangeItemOrder)args[0];
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}