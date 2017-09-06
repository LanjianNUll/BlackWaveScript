//******************************************************************
// File Name:					ChoseThrowWeaponDealPage
// Description:					ChoseThrowWeaponDealPage class 
// Author:						lanjian
// Date:						3/6/2017 10:07:42 AM
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
    class ChoseThrowWeaponDealPage : ChoseItemBasePage
    {
        protected ChoseThrowWeaponDealPage()
        {
            this.m_PageName = "UIRootPrefabs/DealPanel_PageItem/ThrowChoseWeaponeDeal";
            this.gunItemList = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/DealItem";
            this.iconPath = Utility.ConstantValue.HandBombIcon;
            this.m_PageIndex = 3;
            this.Init();
            //test  这里要用事件
            FillItem(null);
        }

        public static ChoseThrowWeaponDealPage Create()
        {
            return new ChoseThrowWeaponDealPage();
        }

        //--------------------------------------
        //private
        //--------------------------------------

        private void GetFiltWeapons()
        {
            //获取手雷数据  1  是手雷  2  是其他  （类型表里的数据是这样的）
            m_mFirstList = DeaLItemProtocol.GetFitterCommdityItemList((int)CommodityType.GrenadeItem);
        }

        //--------------------------------------
        //pulbic
        //--------------------------------------
        public override void FillItem(EventArg eventArg)
        {
            GetFiltWeapons();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            m_centerTra = this.CurrentItem.transform.GetChild(1);
            for (int i = 1; i <= 1; i++)
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
