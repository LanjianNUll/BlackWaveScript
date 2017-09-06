//******************************************************************
// File Name:					PutupSaleAndReadySaleDialogUI
// Description:					PutupSaleAndReadySaleDialogUI class 
// Author:						lanjian
// Date:						3/6/2017 4:21:34 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Deal;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    //出售列表和 预售列表
    class PutupSaleAndReadySaleDialogUI: PutUpSaleAndReadyControlScrollBaseDialogUI
    {
        protected PutupSaleAndReadySaleDialogUI()
        {
            this.m_resName = "UIRootPrefabs/DealPanel_PageItem/DialogUI/putUpSaleAndreadySaleDialog";
            this.m_DType = DialogType.PutUpSaleAndReadySale;
            //注册事件
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Deal_itemChanged,OnGetTradeList);
        }

        public static PutupSaleAndReadySaleDialogUI Create()
        {
            return new PutupSaleAndReadySaleDialogUI();
        }

        private GameObject m_PutUpTopage1;
        private GameObject m_ReadySoldpage2;

        private List<DealItemInfo> m_CurrentForTradeList;
        private List<DealItemInfo> m_CurrentWaitForTradeList;

        private List<GameObject> m_forTradeList  = new List<GameObject>();
        private List<GameObject> m_waitTradeList = new List<GameObject>();

        //记录当前的list状态
        private int m_fPreClickNum = -1;                 //记录上一个点击的index
        private bool m_fIsExistUnFold;                  // 是否当前还存在没有折叠的

        private int m_wPreClickNum = -1;                 //记录上一个点击的index
        private bool m_wIsExistUnFold;                  // 是否当前还存在没有折叠的

        private List<int> m_ForSaleTime = new List<int>();      //保存倒计时的
        private List<int> m_WaitSaleTime = new List<int>();

        private string m_resPath = "UIRootPrefabs/DealPanel_PageItem/OnSoldItem/MainWeaponSoldItem";

        private DealFitterItem m_dealFitterItem;
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
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.GetChild(1)
                .GetChild(1).GetChild(0)).onClick = OnBack;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            m_centerTran = this.m_DialogUIGo.transform.GetChild(1).GetChild(0);
            m_PutUpTopage1 = m_centerTran.GetChild(0).GetChild(0).GetChild(0).gameObject;
            m_ReadySoldpage2 = m_centerTran.GetChild(0).GetChild(0).GetChild(1).gameObject;
            //处理下顶格
            Utility.Utility.ModifyItemT0p(m_PutUpTopage1.transform.GetChild(1).GetChild(0).gameObject,new Vector3(0,607,0));
            Utility.Utility.ModifyItemT0p(m_ReadySoldpage2.transform.GetChild(1).GetChild(0).gameObject, new Vector3(0, 607, 0));
            
            scrollView = this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<UIScrollView>();
            m_pageMgr  = m_DialogUIGo.transform.GetChild(1).GetChild(1).GetChild(1);
        }

        //请求交易列表
        private void RequestTradeList()
        {
            m_dealFitterItem = (DealFitterItem)this.m_currentArgs[0];
            DealItemMgr.RequestTradeList(m_dealFitterItem);
        }

        //交易列表返回
        private void OnGetTradeList()
        {
            this.RefreshList();

            m_CurrentForTradeList = DealItemMgr.GetTradeList(ItemState.ForSale);
            m_CurrentWaitForTradeList = DealItemMgr.GetTradeList(ItemState.WaitForSale);
            //保存剩余时间的数组
            m_ForSaleTime.Clear();
            m_WaitSaleTime.Clear();
            foreach (DealItemInfo item in m_CurrentForTradeList)
            {
                m_ForSaleTime.Add(item.EndTime);
            }
            foreach (DealItemInfo item in m_CurrentWaitForTradeList)
            {
                m_WaitSaleTime.Add(item.EndTime);
            }

            if (m_CurrentForTradeList.Count == 0)
                Debug.Log("当前在售列表为空");
            if (m_CurrentWaitForTradeList == null)
                Debug.Log("当前预售列表为空");
            FillPutUpTopageUI();
            FillReadySoldpageUI();
            
        }

        //填充第一页的列表
        private void FillPutUpTopageUI()
        {
            //禁止滑动
            if (m_CurrentForTradeList.Count < 4)
            {
                m_PutUpTopage1.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().enabled = false;
            }
            this.DoWithPage(m_CurrentForTradeList, m_forTradeList, this.m_PutUpTopage1);
        }

        //填充第二页的列表
        private void FillReadySoldpageUI()
        {
            //禁止滑动
            if (m_CurrentWaitForTradeList.Count < 4)
            {
                m_ReadySoldpage2.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().enabled = false;
            }
            this.DoWithPage(m_CurrentWaitForTradeList, m_waitTradeList,this.m_ReadySoldpage2);
        }

        private void DoWithPage(List<DealItemInfo> dList,List<GameObject> gList,GameObject parent)
        {
            for (int i = 0; i < dList.Count; i++)
            {
                gList.Add(this.ReloadItem(parent));
                this.BindItemListener(gList[i]);
                this.FindItemData(gList[i], dList[i]);
            }
        }

        private GameObject ReloadItem(GameObject parent)
        {
            GameObject prefabs;
            prefabs = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(m_resPath) as GameObject);
            prefabs.transform.parent = parent.transform.GetChild(1).GetChild(0).GetChild(0);
            prefabs.transform.localScale = Vector3.one;
            prefabs.transform.localPosition = Vector3.zero;
            int num = prefabs.transform.parent.childCount-1;
            prefabs.name = "itemList" + num;
            prefabs.transform.localPosition = new Vector3(0, -m_ItemHeight * num, 0);
            return prefabs;
        }

        //找到当前点击的按个item
        private void FindCurrentDealItemInfo(string key)
        {
            for (int i = 0; i < m_CurrentForTradeList.Count; i++)
            {
                if (key.Equals(m_CurrentForTradeList[i].DealKey))
                {
                    m_currentDealItemInfo = m_CurrentForTradeList[i];
                    return;
                }
            }
            for (int i = 0; i < m_CurrentWaitForTradeList.Count; i++)
            {
                if (key.Equals(m_CurrentWaitForTradeList[i].DealKey))
                {
                    m_currentDealItemInfo = m_CurrentWaitForTradeList[i];
                    return;
                }
            }
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
            //销毁自物体
            DestroyGameObject(m_forTradeList);
            DestroyGameObject(m_waitTradeList);

            m_forTradeList.Clear();
            m_waitTradeList.Clear();

            m_fPreClickNum = -1;                 //记录上一个点击的index
            m_fIsExistUnFold = false;                  // 是否当前还存在没有折叠的
            m_wPreClickNum = -1;                 //记录上一个点击的index
            m_wIsExistUnFold = false;
        }

        private void ResetPosition(int i)
        {
           Utility.Utility.MoveScrollViewTOTarget(this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0), new Vector3(i * -934, 0, 0));
        }
        //--------------------------------------
        //public
        //--------------------------------------

        public override void OnBuyItemClick(GameObject go)
        {
            string key = go.transform.parent.parent.GetChild(0).GetComponent<UILabel>().text;
            FindCurrentDealItemInfo(key);
            if (m_currentDealItemInfo == null) return;
            //判断是否自己的物品
            if (!m_currentDealItemInfo.BelongSelf)
            {
                DialogMgr.Load(DialogType.OffAndBuy);
                DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(m_currentDealItemInfo, this.m_DType, this.m_dealFitterItem));
            }
            else
            {
                Utility.Utility.NotifyStr("你不能操作你自己交易的物品！！");
            }
        }

        public override void OnItemClick(GameObject itemGo)
        {
            //判断当前是那个GameObjectList
            if (this.m_currentPageNum == 1)
            {
                this.m_currentGoList = this.m_forTradeList;
                this.PreClickNum = this.m_fPreClickNum;
                this.IsExistUnFold = this.m_fIsExistUnFold;
            }

            if (this.m_currentPageNum == 2)
            {
                this.m_currentGoList = this.m_waitTradeList;
                this.PreClickNum = this.m_wPreClickNum;
                this.IsExistUnFold = this.m_wIsExistUnFold;
            }
            base.OnItemClick(itemGo);

            //记录下点击的参数
            if (this.m_currentPageNum == 1)
            {
                this.m_fPreClickNum = this.PreClickNum;
                this.m_fIsExistUnFold = this.IsExistUnFold;
            }

            if (this.m_currentPageNum == 2)
            {
                this.m_wPreClickNum = this.PreClickNum;
                this.m_wIsExistUnFold = this.IsExistUnFold;  
             }
        }

        //倒计时函数
        public override void SecondUpdateDialog()
        {
            //m_forTradeList;
            //m_waitTradeList);
            for (int i = 0; i < m_forTradeList.Count; i++)
            {
                if(m_ForSaleTime[i] > 0)
                    m_forTradeList[i].transform.Find("content/value").GetComponent<UILabel>().text =
                        Utility.Utility.GetTimeString(m_ForSaleTime[i]--);
            }
            for (int i = 0; i < m_waitTradeList.Count; i++)
            {
                if (m_WaitSaleTime[i] > 0)
                    m_waitTradeList[i].transform.Find("content/value").GetComponent<UILabel>().text =
                    Utility.Utility.GetTimeString(m_WaitSaleTime[i]--);
            }
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.RequestTradeList();
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }

        public override void UpadateDialog()
        {
            BaseTouchControl();
            BaseAnimtionControl();
            ChangePageNum();
        }

        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("center/btnGroup"));
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
        }

        public override void DisPose()
        {
            DealItemMgr.ExitForSold();
            //注销事件
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Deal_itemChanged, OnGetTradeList);
            base.DisPose();
        }
    }
}
