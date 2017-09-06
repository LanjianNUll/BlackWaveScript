//******************************************************************
// File Name:					AllExchangeItemPage
// Description:					AllExchangeItemPage class 
// Author:						lanjian
// Date:						3/2/2017 11:31:08 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Event;
using FW.Exchange;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class AllExchangeItemPage: ExchangeBasePage
    {
        protected AllExchangeItemPage()
        {
            this.m_sortName = "全部";
            this.m_PageName = "UIRootPrefabs/ExchangePanel_PageItem/Allexchange";
            this.gunItemList = "UIRootPrefabs/ExchangePanel_PageItem/exchangeObjectItem/exchangeItem";
            this.m_PageIndex = 1;
            this.Init();
            FW.Event.FWEvent.Instance.Regist(Event.EventID.ExchnagePrizeItem_change,OnGetExchangePSBack);
            //请求返回兑换物品列表
            ExchangePrizeMgr.RequestGetExchangeItems();
        }

        public static AllExchangeItemPage Create()
        {
            return new AllExchangeItemPage();
        }


        //--------------------------------------
        //private
        //--------------------------------------
       
        //--------------------------------------
        //public
        //--------------------------------------
        public override void FillItem(EventArg eventArg)
        {
            base.FillItem(null);
            m_ExchangePrizieList = ExchangePrizeMgr.GetExchangeItems();
            if (m_ExchangePrizieList.Count == 0)
                return;
            m_exchangePrizeGoList.Add(this.ReloadItem(0, true));
            for (int i = 1; i < m_ExchangePrizieList.Count; i++)
            {
                m_exchangePrizeGoList.Add(this.ReloadItem(0));
            }
            FillDataTOItem();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.ExchnagePrizeItem_change, OnGetExchangePSBack);
            base.DisPose();
        }

    }
}
