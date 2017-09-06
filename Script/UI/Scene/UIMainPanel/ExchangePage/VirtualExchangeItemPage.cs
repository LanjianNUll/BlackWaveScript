//******************************************************************
// File Name:					VirtualExchangeItemPage
// Description:					VirtualExchangeItemPage class 
// Author:						lanjian
// Date:						3/2/2017 11:39:34 AM
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
    class VirtualExchangeItemPage:ExchangeBasePage
    {
        protected VirtualExchangeItemPage()
        {
            this.m_sortName = "虚拟道具";
            this.m_PageName = "UIRootPrefabs/ExchangePanel_PageItem/virtualexchange";
            this.m_PageIndex = 3;
            this.Init();
            FW.Event.FWEvent.Instance.Regist(Event.EventID.ExchnagePrizeItem_change, OnGetExchangePSBack);
        }

        public static VirtualExchangeItemPage Create()
        {
            return new VirtualExchangeItemPage();
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
            //test
            m_ExchangePrizieList = ExchangePrizeMgr.GetExchangeItems(ExchangePrizeType.Virtual);
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
