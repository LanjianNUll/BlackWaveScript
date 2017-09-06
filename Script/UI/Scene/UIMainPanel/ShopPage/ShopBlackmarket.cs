//******************************************************************
// File Name:					ShopBlackmarket
// Description:					ShopBlackmarket class 
// Author:						lanjian
// Date:						2/10/2017 5:54:10 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Item;
using FW.Store;

namespace FW.UI
{
    class ShopBlackmarket : ShopPageBase
    {
        protected ShopBlackmarket()
        {
            this.m_PageName = "UIRootPrefabs/ShopPanel_PageItem/Blackmarket";
            this.gunItemList = "UIRootPrefabs/ShopPanel_PageItem/supplyPageItem/blackItem";
            this.m_PageIndex = 3;
            this.m_selfPageCapatiy = 8;
            this.Init();
            //注册事件
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Shop_changed, ReSetData);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Shop_itemBought, OnBuyBlackmarketItemInfo);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Shop_black_CountZero, OnRrefrshCountZero);
            FillItem(null);
        }

        public static ShopBlackmarket Create()
        {
            return new ShopBlackmarket();
        }

        private int m_currentTab = 0;  //加载那个Tab的内容  武器0   配件1  道具2 
        private List<StoreItem> m_AllStoreItemList;
        private List<StoreItem> m_weaponStoreItemList = new List<StoreItem>();
        private List<StoreItem> m_accesoryStoreItemList = new List<StoreItem>();
        private List<StoreItem> m_commodityStoreItemList = new List<StoreItem>();
        private Transform ScrollViewTF;
        //记录当前scrollow的页数
        int mainidd = 1;
        int secondidd = 1;
        int closeidd = 1;
        private int m_WeaponPageCount;
        private int m_AccesoryPageCount;
        private int m_commodityPageCount;

        private UIButton m_refreshButton;
        private UILabel m_timeCount;
        private CurrencyType m_currentType;                 //当前刷新金钱类型
        private int m_cost;                                                     //当前刷新所要的金钱 
        //--------------------------------------
        //private
        //--------------------------------------

        bool sFirst = true;
        bool cFirst = true;
        private void ShowWhichWeapon(GameObject go)
        {
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            if (m_currentTab == 1 && sFirst)
            {
                this.PreLoadTwoPage(m_currentTab, m_AccesoryPageCount, m_accesoryStoreItemList);
                sFirst = false;
            }
            if (m_currentTab == 2 && cFirst)
            {
                this.PreLoadTwoPage(m_currentTab, m_commodityPageCount, m_commodityStoreItemList);
                cFirst = false;
            }
        }

        private void ControlLoadPage(int tabindex, ref int idd, int count, List<StoreItem> wecList)
        {
            if (ScrollViewTF.localPosition.y > (1400 * idd - 100) && idd < count - 1)
            {
                idd++;
                Vector3 currentScorllPosition = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab).transform.localPosition;
                GameObject item = this.ReloadItem(tabindex);
                FillDateInPageItem(idd, item, wecList);
                //恢复在当前位置
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab), currentScorllPosition);
            }
        }

        private void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab);
            if (m_currentTab == 0)
            {
                ControlLoadPage(m_currentTab, ref mainidd, m_WeaponPageCount, m_weaponStoreItemList);
            }

            if (m_currentTab == 1)
            {
                ControlLoadPage(m_currentTab, ref secondidd, m_AccesoryPageCount, m_accesoryStoreItemList);
            }

            if (m_currentTab == 2)
            {
                ControlLoadPage(m_currentTab, ref closeidd, m_commodityPageCount, m_commodityStoreItemList);
            }
        }

        private void GetAllStoreItem()
        {
            m_AllStoreItemList = StoreMgr.GetItem(StoreItemType.Munition);
            m_cost = StoreMgr.CostFreshShop;
            m_currentType = StoreMgr.CurrentType;
            m_weaponStoreItemList.Clear();
            m_accesoryStoreItemList.Clear();
            m_commodityStoreItemList.Clear();
            //将所有的军火商品分类  武器  配件  道具
            for (int i = 0; i < m_AllStoreItemList.Count; i++)
            {
                if (m_AllStoreItemList[i].ItemType == ItemType.Weapon)
                    m_weaponStoreItemList.Add(m_AllStoreItemList[i]);
                if (m_AllStoreItemList[i].ItemType == ItemType.Accessory)
                    m_accesoryStoreItemList.Add(m_AllStoreItemList[i]);
                if (m_AllStoreItemList[i].ItemType == ItemType.Commodity)
                    m_commodityStoreItemList.Add(m_AllStoreItemList[i]);
            }
            Debug.Log("武器数量："+m_weaponStoreItemList.Count+"-配件-"+m_accesoryStoreItemList.Count+ "-道具-" + m_commodityStoreItemList.Count);
        }

        //填充刷新按钮的数据
        private void GetButtonData()
        {
            //设置刷新按钮
            m_refreshButton.transform.GetChild(0).GetComponent<UILabel>().text = "手动刷新" + m_cost.ToString();
            string uiSpriteName = "";
            if (m_currentType == CurrencyType.Cash)
                uiSpriteName = "gold";
            if (m_currentType == CurrencyType.Gold)
                uiSpriteName = "bag";
            if (m_currentType == CurrencyType.Diamond)
                uiSpriteName = "diamond";
            m_refreshButton.transform.GetChild(1).GetComponent<UISprite>().spriteName = uiSpriteName;
        }

        //计算页数
        private void CaulateWeaponPageCount()
        {
            m_WeaponPageCount = CalculatePageCount(m_weaponStoreItemList, 0);
            m_AccesoryPageCount = CalculatePageCount(m_accesoryStoreItemList, 1);
            m_commodityPageCount = CalculatePageCount(m_commodityStoreItemList, 2);
        }

        
        private void RefreshOrTimeControl()
        {
            m_refreshButton = this.CurrentItem.transform.GetChild(0).Find("Refresh").GetComponent<UIButton>();
            m_timeCount = this.CurrentItem.transform.GetChild(0).Find("Timer/timeCount").GetComponent<UILabel>();
            Utility.Utility.GetUIEventListener(m_refreshButton.gameObject).onClick = OnRreshShopList;
        }

        //请求刷新商城
        private void OnRreshShopList(GameObject go)
        {
            string str = "";
            if (m_currentType == CurrencyType.Cash)
            {
                str = "现金";
                if (m_cost > Role.Role.Instance().Cash)
                {
                    Utility.Utility.NotifyStr("你的" + str + "不足，无法刷新！！");
                    return;
                }
            }
            if (m_currentType == CurrencyType.Gold)
            {
                str = "金币";
                if (m_cost > Role.Role.Instance().Gold)
                {
                    Utility.Utility.NotifyStr("你的" + str + "不足，无法刷新！！");
                    return;
                }
            } 
            if (m_currentType == CurrencyType.Diamond)
            {
                str = "钻石";
                if (m_cost > Role.Role.Instance().Diamond)
                {
                    Utility.Utility.NotifyStr("你的" + str + "不足，无法刷新！！");
                    return;
                }
            }
            Store.StoreMgr.RequestItems();
        }

        //刷新商城刷新的金币
        private void ReSetRefreshButton(EventArg arg)
        {
            //手动刷新价格类型
            m_currentType = (CurrencyType)arg[1];
            //手动刷新价格
            m_cost = (int)arg[2];
            if (m_refreshButton != null)
            {
                GetButtonData();
            }
        }

        //重置数据 准备刷新商城返回
        private void ReSetData(EventArg args)
        {
            if ((int)args[0] != 0)
            {
                Utility.Utility.NotifyStr("刷新商城失败！");
                return;
            }
            ReSetRefreshButton(args);
            this.GetAllStoreItem();
            this.CaulateWeaponPageCount();
            this.DestroyChilds(m_currentTab);
            if (m_currentTab == 0)
            {
                mainidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_WeaponPageCount, m_weaponStoreItemList);
            }
            if (m_currentTab == 1)
            {
                secondidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_AccesoryPageCount, m_accesoryStoreItemList);
            }
            if (m_currentTab == 2)
            {
                closeidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_commodityPageCount, m_commodityStoreItemList);
            }
            Utility.Utility.NotifyStr("你已经刷新了商城！");
        }

        //购买后的反馈
        private void OnBuyBlackmarketItemInfo(EventArg args)
        {
            //商品购买反馈(参数：EventArg(StoreItem 商品对象, bool 0成功1未成功))
            StoreItem backBuyStoreItem = (StoreItem)args[0];
            //如果是供给品
            if (backBuyStoreItem.Type == StoreItemType.Munition)
                this.OnBuyItemInfo(args);
        }

        //预先加载两页
        private void PreLoadTwoPage(int tabIndex, int PageCount, List<StoreItem> storeList)
        {
            FillDateInPageItem(0, this.ReloadItem(tabIndex, true), storeList);
            if (PageCount >= 2)
            {
                FillDateInPageItem(1, this.ReloadItem(tabIndex), storeList);
            }
        }

        private void OnRrefrshCountZero(Event.EventArg arg)
        {
            int time = (int)arg[0];
            m_timeCount.text = Utility.Utility.GetTimeString(time);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        //显示是否确定购买按钮
        public override void OnShowConfirmBuy(GameObject go)
        {
            //根据当前是在那个tab下的道具
            if(m_currentTab == 0)
                this.ConfirmButtonDealWith(go, m_weaponStoreItemList);
            if (m_currentTab == 1)
                this.ConfirmButtonDealWith(go, m_accesoryStoreItemList);
            if (m_currentTab == 2)
                this.ConfirmButtonDealWith(go, m_commodityStoreItemList);
        }

        public override void FillItem(EventArg eventArg)
        {
            GetAllStoreItem();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            for (int i = 1; i <= 3; i++)
            {
                Utility.Utility.GetUIEventListener(topTransform.GetChild(i).gameObject).onClick = ShowWhichWeapon;
                topTransform.GetChild(i).gameObject.name += (i - 1) + "";
            }
            CaulateWeaponPageCount();
            //预先加载两页
            this.PreLoadTwoPage(m_currentTab, m_WeaponPageCount, m_weaponStoreItemList);
            RefreshOrTimeControl();
            //刷新按钮
            GetButtonData();
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Shop_changed, ReSetData); 
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Shop_itemBought, OnBuyItemInfo);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Shop_black_CountZero, OnRrefrshCountZero);
            base.DisPose();
        }
    }
}
