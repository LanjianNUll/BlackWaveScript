//******************************************************************
// File Name:					RealExchageItemPage
// Description:					RealExchageItemPage class 
// Author:						lanjian
// Date:						3/2/2017 11:36:26 AM
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
    class RealExchageItemPage: ExchangeBasePage
    {
        protected RealExchageItemPage()
        {
            this.m_sortName = "实物商品";
            this.m_PageName = "UIRootPrefabs/ExchangePanel_PageItem/Realexchange";
            // this.gunItemList = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/MainWeaponSoldItem";
            this.m_PageIndex = 2;
            this.Init();
            FW.Event.FWEvent.Instance.Regist(Event.EventID.ExchnagePrizeItem_change, OnGetExchangePSBack);

        }

        public static RealExchageItemPage Create()
        {
            return new RealExchageItemPage();
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
            
            m_ExchangePrizieList = ExchangePrizeMgr.GetExchangeItems(ExchangePrizeType.Real);

            Debug.Log("count"+m_ExchangePrizieList.Count);
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
