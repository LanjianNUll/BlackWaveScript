//******************************************************************
// File Name:					ChoseWeaponDealPage
// Description:					ChoseWeaponDealPage class 
// Author:						lanjian
// Date:						3/6/2017 9:55:58 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Item;
using FW.Deal;

namespace FW.UI
{
    class ChoseWeaponDealPage:ChoseItemBasePage
    {
        protected ChoseWeaponDealPage()
        {
            this.m_PageName = "UIRootPrefabs/DealPanel_PageItem/MainChoseWeaponeDeal";
            this.gunItemList = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/DealItem";
            this.m_PageIndex = 1;
            this.iconPath = Utility.ConstantValue.WeaponIcon;
            this.Init();
            //test  这里要用事件
            FillItem(null);
        }

        public static ChoseWeaponDealPage Create()
        {
            return new ChoseWeaponDealPage();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void GetFiltWeapons()
        {
            //获取主武器数据
            m_mFirstList = DeaLItemProtocol.GetFitterWeaponItemList((int)WeaponType.Main);
            m_mSecondList = DeaLItemProtocol.GetFitterWeaponItemList((int)WeaponType.Second);
            m_mThridList = DeaLItemProtocol.GetFitterWeaponItemList((int)WeaponType.Melee);
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
