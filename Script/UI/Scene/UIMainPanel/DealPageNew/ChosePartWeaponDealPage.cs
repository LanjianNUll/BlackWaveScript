//******************************************************************
// File Name:					ChosePartWeaponDealPage
// Description:					ChosePartWeaponDealPage class 
// Author:						lanjian
// Date:						3/6/2017 10:09:09 AM
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
    class ChosePartWeaponDealPage : ChoseItemBasePage
    {
        protected ChosePartWeaponDealPage()
        {
            this.m_PageName = "UIRootPrefabs/DealPanel_PageItem/PartsChoseWeaponDeal";
            this.gunItemList = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/DealItem";
            this.m_PageIndex = 2;
            this.iconPath = Utility.ConstantValue.PartIcon;
            this.Init();
            //test  这里要用事件
            FillItem(null);
        }

        public static ChosePartWeaponDealPage Create()
        {
            return new ChosePartWeaponDealPage();
        }
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetFiltWeapons()
        {
            //获取主武器数据
            m_mFirstList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.Muzzle);
            m_mSecondList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.Barrel);
            m_mThridList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.Sight);
            m_mforthtList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.Maganize);
            m_mfifthList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.MuzzleSuit);
            m_msixthList = DeaLItemProtocol.GetFitterPartItemList((int)AccessoryType.Trigger);
        }

        //--------------------------------------
        //pulbic
        //--------------------------------------
        public override void FillItem(EventArg eventArg)
        {
            GetFiltWeapons();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            m_centerTra = this.CurrentItem.transform.GetChild(1);
            for (int i = 1; i <= 6; i++)
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
