//******************************************************************
// File Name:					MyForSoldDialogUI
// Description:					MyForSoldDialogUI class 
// Author:						lanjian
// Date:						3/6/2017 11:57:27 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Deal;
using FW.Item;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    //我的寄售 列表
    class MyForSoldDialogUI:PutUpSaleAndReadySaleBaseDialogUI
    {
        protected MyForSoldDialogUI()
        {
            this.m_resName = "UIRootPrefabs/DealPanel_PageItem/DialogUI/mySoldDialog";
            this.m_DType = DialogType.MyForSold;
            //注销事件
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Deal_mySelfItemChanged, OnBackMyRequestMySaleInfo);
        }

        public static MyForSoldDialogUI Create()
        {
            return new MyForSoldDialogUI();
        }
        
        private string path = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/MainWeaponSoldItem";

        private List<GameObject> m_MySoldItemList = new List<GameObject>();

        private List<DealItemInfo> m_mySelfDealItemList;    //我的售卖消息
        private List<int> m_mySelfTime = new List<int>();
        
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        private GameObject ReloadItem(bool first = false)
        {
            GameObject prefabs;
            if (first)
            {
                prefabs = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(path) as GameObject);
                prefabs.transform.parent = this.m_centerTran.GetChild(0).GetChild(0);
                prefabs.transform.localScale = Vector3.one;
                prefabs.transform.localPosition = Vector3.zero;
                prefabs.name = "itemList0";
                return prefabs;
            }
            prefabs = this.m_centerTran.GetChild(0).GetChild(0).GetChild(0).gameObject;
            int num = this.m_centerTran.GetChild(0).GetChild(0).childCount;
            GameObject item = GameObject.Instantiate(prefabs);
            item.transform.parent = this.m_centerTran.GetChild(0).GetChild(0);
            item.transform.localScale = Vector3.one;
            item.name = "itemList" + num;
            item.transform.localPosition = new Vector3(0, -m_ItemHeight * num, 0);
            //重置下位置
            this.m_centerTran.GetChild(0).GetComponent<UIScrollView>().ResetPosition();
            this.m_centerTran.GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();
            return item;
        }

        //找到当前dealInfo
        private void FindCurrentDealItemInfo(string key)
        {
            for (int i = 0; i < m_mySelfDealItemList.Count; i++)
            {
                if (key.Equals(m_mySelfDealItemList[i].DealKey))
                {
                    m_currentDealItemInfo = m_mySelfDealItemList[i];
                    break;
                }
            }
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            m_centerTran = this.m_DialogUIGo.transform.GetChild(1).Find("center");
            m_MySoldItemList.Add(this.ReloadItem(true));

            for (int i = 0; i < m_mySelfDealItemList.Count-1; i++)
            {
                m_MySoldItemList.Add(this.ReloadItem());
            }
            //修正
            Utility.Utility.ModifyItemT0p(m_centerTran.GetChild(0).gameObject,new Vector3(0,666,0));
            //填充数据
            //绑定点击事件
            for (int i = 0; i < m_MySoldItemList.Count; i++)
            {
                this.BindItemListener(m_MySoldItemList[i]);
                this.FindItemData(m_MySoldItemList[i],m_mySelfDealItemList[i]);
            }
        }

        //请求我的售卖信息
        private void RequestMyForSaleInfo()
        {
            DealItemMgr.RequestMySelfTradeList();
        }

        //我的售卖信息返回
        private void OnBackMyRequestMySaleInfo()
        {
            RefreshList();
            m_mySelfDealItemList = DealItemMgr.GetMyTradeList();
            //保存剩余时间的数组
            m_mySelfTime.Clear();
            foreach (DealItemInfo item in m_mySelfDealItemList)
            {
                m_mySelfTime.Add(item.EndTime);
            }
            if (m_mySelfDealItemList.Count == 0) return;
            this.FillDataUI(null);
        }

        private void DestroyGameObject(List<GameObject> list)
        {
            foreach (GameObject item in list)
            {
                UnityEngine.GameObject.Destroy(item);
            }
        }

        private void RefreshList()
        {
            DestroyGameObject(m_MySoldItemList);
            m_MySoldItemList.Clear();
        }

        //倒计时函数
        private void UpdataUIOneSecond()
        {
            if (m_MySoldItemList == null  || m_mySelfDealItemList ==null) return;
            for (int i = 0; i < m_MySoldItemList.Count; i++)
            {
                if(m_mySelfTime[i] > 0)
                    m_MySoldItemList[i].transform.Find("content/value").GetComponent<UILabel>().text =
                        Utility.Utility.GetTimeString(m_mySelfTime[i]--);
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------

        public override void OnBuyItemClick(GameObject go)
        {
            string key = go.transform.parent.parent.GetChild(0).GetComponent<UILabel>().text;
            FindCurrentDealItemInfo(key);
            if (m_currentDealItemInfo == null) return;
            DialogMgr.Load(DialogType.OffAndBuy);
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(m_currentDealItemInfo, this.m_DType,null));
        }

        public override void OnItemClick(GameObject itemGo)
        {
            //将当前的listgo给他
            this.m_currentGoList = this.m_MySoldItemList;
            base.OnItemClick(itemGo);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.RequestMyForSaleInfo();
            this.OpenDialog();
            //test
            //this.FillDataUI(null);
        }

        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("center/btnGroup"));
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
        }

        public override void SecondUpdateDialog()
        {
            UpdataUIOneSecond();
        }

        public override void DisPose()
        {
            DealItemMgr.ExitMySold();
            //注销事件
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Deal_mySelfItemChanged, OnBackMyRequestMySaleInfo);
            base.DisPose();
        }
    }
}