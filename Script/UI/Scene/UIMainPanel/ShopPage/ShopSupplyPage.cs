//******************************************************************
// File Name:					ShopSupplyPage
// Description:					ShopSupplyPage class 
// Author:						lanjian
// Date:						2/10/2017 5:49:08 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Store;
using FW.Item;

namespace FW.UI
{
    class ShopSupplyPage: ShopPageBase
    {
        protected ShopSupplyPage()
        {
            this.m_PageName = "UIRootPrefabs/ShopPanel_PageItem/supply";
            this.gunItemList = "UIRootPrefabs/ShopPanel_PageItem/supplyPageItem/supplyItem";
            this.m_PageIndex = 2;
            this.m_ItemHeight = 1500;
            this.m_selfPageCapatiy = 6;
            this.Init();
            this.FillItem(null);
            //注册事件
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Shop_changed,OnRefrshSupplyList);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Shop_itemBought, OnBuySupplyItemInfo);
        }

        public static ShopSupplyPage Create()
        {
            return new ShopSupplyPage();
        }

        Transform ScrollViewTF;
        //补给品列表
        private List<StoreItem> m_StoreItemList;
        private int m_storePageCount;
        //--------------------------------------
        //private
        //--------------------------------------
        private int idd = 1;
        private void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(0);
            if (ScrollViewTF.localPosition.y > (1500 * idd - 100) && idd < m_storePageCount-1)
            {
                idd++;
                Vector3 currentScorllPosition = this.CurrentItem.transform.GetChild(1).GetChild(0).transform.localPosition;
                GameObject item = this.ReloadItem(0);
                FillDateInPageItem(idd, item, m_StoreItemList);
                //恢复在当前位置
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(0), currentScorllPosition);
            }
        }
        
        //获取补给品列表
        private void GetSupplyList()
        {
            m_StoreItemList = StoreMgr.GetItem(StoreItemType.Supply);
            if (m_StoreItemList.Count == 0)
            {
                Utility.Utility.NotifyStr("当前商城无补给品！！");
            }
        }

        private void OnRefrshSupplyList(EventArg args)
        {
            //判断是否刷新成功了
            if ((int)args[0] != 0) return;
            this.DestroyChilds(0);
            this.FillItem(null);
        }
        //购买后的反馈
        private void OnBuySupplyItemInfo(EventArg args)
        {
            //商品购买反馈(参数：EventArg(StoreItem 商品对象, bool 0成功1未成功))
            StoreItem backBuyStoreItem = (StoreItem)args[0];
            //如果是供给品
            if (backBuyStoreItem.Type == StoreItemType.Supply)
                this.OnBuyItemInfo(args);
        }
        //--------------------------------------
        //public
        //--------------------------------------

        //显示是否确定购买按钮
        public override void OnShowConfirmBuy(GameObject go)
        {
            this.ConfirmButtonDealWith(go, m_StoreItemList);
        }

        public override void FillItem(EventArg eventArg)
        {
            GetSupplyList();
            Debug.Log("补给品列表的count"+m_StoreItemList.Count);
            m_storePageCount = this.CalculatePageCount(m_StoreItemList);
            //预先加载两页
            FillDateInPageItem(0,this.ReloadItem(0, true), m_StoreItemList);
            if(m_storePageCount>=2)
                FillDateInPageItem(1,this.ReloadItem(0), m_StoreItemList);
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }
        
        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Shop_changed, OnRefrshSupplyList);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Shop_itemBought, OnBuyItemInfo);
            base.DisPose();
        }
    }
}
