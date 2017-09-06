//******************************************************************
// File Name:					ChoseOtherDealPage
// Description:					ChoseOtherDealPage class 
// Author:						lanjian
// Date:						3/6/2017 10:11:16 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Deal;
using FW.Item;

namespace FW.UI
{
    class ChoseOtherDealPage:ChoseItemBasePage
    {
        protected ChoseOtherDealPage()
        {
            this.m_PageName = "UIRootPrefabs/DealPanel_PageItem/OtherChoseWeaponeDeal";
            this.gunItemList = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/DealItem";
            this.iconPath = Utility.ConstantValue.CommodityIcon;
            this.m_PageIndex = 4;
            this.Init();
            //test  这里要用事件
            FillItem(null);
        }

        public static ChoseOtherDealPage Create()
        {
            return new ChoseOtherDealPage();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void GetFiltWeapons()
        {
            //获取其他的道具分类
            m_mFirstList = DeaLItemProtocol.GetFitterCommdityItemList((int)CommodityType.ExpAgent);
            m_mSecondList = DeaLItemProtocol.GetFitterCommdityItemList((int)CommodityTradeType.ProfitItem);
            m_mThridList = DeaLItemProtocol.GetFitterCommdityItemList((int)CommodityType.Revival);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void FillItem(EventArg eventArg)
        {
            GetFiltWeapons();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            m_centerTra = this.CurrentItem.transform.GetChild(1);
            for (int i = 1; i <= 3; i++)
            {
                Utility.Utility.GetUIEventListener(topTransform.GetChild(i).gameObject).onClick = OnShowWichWWeapon;
                topTransform.GetChild(i).gameObject.name += (i - 1) + "";
            }

            CaulateWeaponPageCount();
            //预先加载两页
            PreLoadTwoPage(0, m_firstPageCount, m_mFirstList);
        }

        public override void DisPose()
        {
            base.DisPose();
        }
    }
}
