//******************************************************************
// File Name:					ShopPayShop
// Description:					ShopPayShop class 
// Author:						lanjian
// Date:						4/26/2017 2:14:57 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Event;
using FW.Pay;
using FW.Store;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class ShopPayPage : ShopPageBase
    {
        protected ShopPayPage()
        {
            this.m_PageName = "UIRootPrefabs/ShopPanel_PageItem/pay";
            this.gunItemList = "UIRootPrefabs/ShopPanel_PageItem/supplyPageItem/payItem";
            this.m_PageIndex = 1;
            this.m_ItemHeight = 1500;
            this.m_selfPageCapatiy = 6;
            this.Init();
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.WChat_back_Info,OnWChatBack);

            this.FillItem(null);
        }

        public static ShopPayPage Create()
        {
            return new ShopPayPage();
        }

        Transform ScrollViewTF;
        //充值列表
        private List<PayItem> m_StoreItemList;
        private int m_storePageCount;
        private GameObject m_payPanel;                                          //支付方式选择面板
        private bool m_isOpenPayPanel;                                          //是否已经打开了支付面板
        private PayItem m_cPayItem;                                              //当前选择的支付项
        //--------------------------------------
        //private
        //--------------------------------------
        private int idd = 1;
        private void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(0);
            if (ScrollViewTF.localPosition.y > (1500 * idd - 100) && idd < m_storePageCount - 1)
            {
                idd++;
                Vector3 currentScorllPosition = this.CurrentItem.transform.GetChild(1).GetChild(0).transform.localPosition;
                GameObject item = this.ReloadItem(0);
                FillDataPayItemint(idd, item, m_StoreItemList);
                //恢复在当前位置
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(0), currentScorllPosition);
            }
        }

        private void FillDataPayItemint(int pageIndex, GameObject pageGo, List<PayItem> storeList)
        {
            int displayNum = (pageIndex + 1) * m_selfPageCapatiy < storeList.Count ? m_selfPageCapatiy : storeList.Count - pageIndex * m_selfPageCapatiy;
            int ABeginIndex = pageIndex * m_selfPageCapatiy;
            for (int i = 0; i < displayNum; i++)
            {
                //点击显示确定对话框
                Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("buyButton"))
                       .onClick = OnShowConfirmBuy;
                //点击显示tip
                Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("Objectpicbtn"))
                      .onClick = OnShowOrHideTips;
                pageGo.transform.GetChild(i).Find("gunid").GetComponent<UILabel>().text = storeList[ABeginIndex + i].ID.ToString();
                List<GameObject> activateList = new List<GameObject>();
                activateList.Add(pageGo.transform.GetChild(i).Find("Content").gameObject);
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UIToggledObjects>().activate = activateList;
                string iconpath = "res/UITexture/ShopIcon/"+ storeList[ABeginIndex + i].Icon;
                Texture texture1 = ResMgr.ResLoad.Load<Texture>(iconpath);
                if (texture1 == null)
                    texture1 = ResMgr.ResLoad.Load<Texture>("res/UITexture/ShopIcon/esp");
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UITexture>().SetRect(0, 0, texture1.width, texture1.height);
                int offset = -157;
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<Transform>().localPosition = new Vector3(0, offset, 0);
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UITexture>().mainTexture = texture1;
                pageGo.transform.GetChild(i).Find("ObjectName").GetComponent<UILabel>().text = storeList[ABeginIndex + i].Name;
                pageGo.transform.GetChild(i).Find("Content/ObjectName (1)").GetComponent<UILabel>().text = storeList[ABeginIndex + i].Name;
                pageGo.transform.GetChild(i).Find("ObjectNum").GetComponent<UILabel>().text = "￥" + storeList[ABeginIndex + i].Price.ToString();
                pageGo.transform.GetChild(i).Find("Content/content").GetComponent<UILabel>().text = storeList[ABeginIndex + i].Desc;
                if (storeList[ABeginIndex + i].Discount == 0)
                    NGUITools.SetActive(pageGo.transform.GetChild(i).Find("dis").gameObject,false);
                pageGo.transform.GetChild(i).Find("dis/discountLabel").GetComponent<UILabel>().text = storeList[ABeginIndex + i].Discount+"折";
            }
            //隐藏这页多余的多余的
            for (int i = 0; i < m_selfPageCapatiy - displayNum; i++)
            {
                NGUITools.SetActive(pageGo.transform.GetChild(m_selfPageCapatiy - 1 - i).gameObject, false);
            }
        }

        private void GetPayItemList()
        {
            m_StoreItemList = StoreMgr.GetPayItemList();
        }

        private void InitPayPanel()
        {
            m_payPanel = this.CurrentItem.transform.parent.parent.parent.GetChild(1).gameObject;
            Utility.Utility.GetUIEventListener(m_payPanel.transform.GetChild(3)).onClick = OnwChatPay;
            Utility.Utility.GetUIEventListener(m_payPanel.transform.GetChild(4)).onClick = Onalipay;
            Utility.Utility.GetUIEventListener(m_payPanel.transform.GetChild(5)).onClick = OnCancel;
            NGUITools.SetActive(m_payPanel.transform.GetChild(5).gameObject, false);
        }

        private void ShowOrHidePayPanel(bool isShow)
        {
            Vector3 start = new Vector3(0, 0, 0); 
            Vector3 end = new Vector3(0, -579, 0);
            if (isShow)
            {
                start = new Vector3(0, -579, 0);
                end = new Vector3(0, 0, 0);
            }
            m_isOpenPayPanel = isShow;
            //是否可以左右滑动
            PanelMgr.CurrPanel.IsAllowHorMove(!isShow);
            NGUITools.SetActive(m_payPanel.transform.GetChild(5).gameObject,isShow);
            //弹出支付选择框
            TweenPosition ta = m_payPanel.GetComponent<TweenPosition>();
            if (ta == null)
            {
                ta = m_payPanel.AddComponent<TweenPosition>();
            }
            ta.ResetToBeginning();
            ta.method = UITweener.Method.EaseIn;
            ta.from = start;
            ta.to = end;
            ta.enabled = true;
            ta.duration = 0.2f;
            ta.PlayForward();
        }

        //微信支付
        private void OnwChatPay(GameObject go)
        {
            if(m_cPayItem!=null)
                PayMgr.WChatPay(m_cPayItem);
        }
        //支付宝支付
        private void Onalipay(GameObject go)
        {
            if (m_cPayItem != null)
                PayMgr.AliPay(m_cPayItem);
        }

        //消失支付选项
        private void OnCancel(GameObject go)
        {
            ShowOrHidePayPanel(false);
        }

        private void OnWChatBack(FW.Event.EventArg args)
        {
            this.OnCancel(null);
            Utility.Utility.NotifyStr((string)args[0] + "zhaxinl");
        }

        private PayItem FindClickPayItem(string id)
        {
            for (int i = 0; i < m_StoreItemList.Count; i++)
            {
                if (id.Equals(m_StoreItemList[i].ID))
                    return m_StoreItemList[i];
            }
            return null;
        }
        //--------------------------------------
        //public
        //--------------------------------------

        //显示是否确定购买按钮
        public override void OnShowConfirmBuy(GameObject go)
        {
            if (!m_isOpenPayPanel)
            {
                ShowOrHidePayPanel(true);
                m_cPayItem = FindClickPayItem(go.transform.parent.Find("gunid").GetComponent<UILabel>().text);
            }
        }

        public override void FillItem(EventArg eventArg)
        {
            GetPayItemList();
            m_storePageCount = this.CalculatePageCount(m_StoreItemList);
            //预先加载两页
            FillDataPayItemint(0, this.ReloadItem(0, true), m_StoreItemList);
            if (m_storePageCount >= 2)
                FillDataPayItemint(1, this.ReloadItem(0), m_StoreItemList);
            //初始化支付页面
            InitPayPanel();
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.WChat_back_Info, OnWChatBack);
            base.DisPose();
        }
    }
}
