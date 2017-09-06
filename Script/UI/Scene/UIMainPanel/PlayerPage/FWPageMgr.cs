//******************************************************************
// File Name:					FWPageMgr
// Description:					FWPageMgr class 
// Author:						lanjian
// Date:						1/13/2017 2:14:56 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    delegate ScrollViewItemBase PageCreator();
    class FWPageMgr
    {
        private static FWPageMgr m_FWPageMgr;
        public static FWPageMgr Instance
        {
            get
            {
                if (m_FWPageMgr == null) m_FWPageMgr = new FWPageMgr();
                return m_FWPageMgr;
            }
        }

        private List<ScrollViewItemBase> m_ScrollViewItemList = new List<ScrollViewItemBase>();
        //private List<PageCreator> m_PageCreateList = new List<PageCreator>();

        public int CurrentPageCount;

        //获取当前页面
        public ScrollViewItemBase CurrentScorollViewItem {
            get {
                if(m_ScrollViewItemList.Count != 0)
                    return this.m_ScrollViewItemList[PanelMgr.CurrPanel.CurrentPageNum-1];
                return null;
            }
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private FWPageMgr()
        {
            
        }
        //
        //--------------------------------------
        //public 
        //--------------------------------------
        //update下放
        public void ScrollViewItemBaseUpdate()
        {
            foreach (var item in m_ScrollViewItemList)
            {
                item.Update();
            }
        }

        //隔一秒调用下放
        public void ScrollViewItemBaseSecondInvoke()
        {
            foreach (var item in m_ScrollViewItemList)
            {
                item.SecondInvoke();
            }
        }

        //角色的
        public void LoadPlayerPage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(FWRecordPage.Create());
            m_ScrollViewItemList.Add(FWRolePage.Create());
            m_ScrollViewItemList.Add(FWMedalPage.Create());
            m_ScrollViewItemList.Add(FWWeaponPage.Create());

            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //背包的(按顺序来add 不然 刷新时找不到正确的index)
        public void LoadBagPackPage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(BagWeaponPage.Create());
            m_ScrollViewItemList.Add(BagPartsPage.Create());
            m_ScrollViewItemList.Add(BagHandBombPage.Create());
            m_ScrollViewItemList.Add(BagOtherPage.Create());

            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //商城
        public void LoadShopPage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(ShopSupplyPage.Create());
            m_ScrollViewItemList.Add(ShopBlackmarket.Create());
            m_ScrollViewItemList.Add(ShopPayPage.Create());
            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //交易
        public void LoadDealPage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(ChoseWeaponDealPage.Create());
            m_ScrollViewItemList.Add(ChoseThrowWeaponDealPage.Create());
            m_ScrollViewItemList.Add(ChosePartWeaponDealPage.Create());
            m_ScrollViewItemList.Add(ChoseOtherDealPage.Create());

            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //兑换
        public void LoadExchangePage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(AllExchangeItemPage.Create());
            m_ScrollViewItemList.Add(RealExchageItemPage.Create());
            m_ScrollViewItemList.Add(VirtualExchangeItemPage.Create());

            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //摇奖
        public void LoadLuckJoyPage()
        {
            m_ScrollViewItemList.Clear();
            m_ScrollViewItemList.Add(LuckyJoyInfoPage.Create());
            m_ScrollViewItemList.Add(LuckyJoyHistoryPage.Create());

            CurrentPageCount = m_ScrollViewItemList.Count;
        }

        //退出
        public void ExitPage()
        {
            for (int i = 0; i < m_ScrollViewItemList.Count; i++)
            {
                m_ScrollViewItemList[i].DisPose();
            } 
            m_ScrollViewItemList.Clear();
        }
    }
}
