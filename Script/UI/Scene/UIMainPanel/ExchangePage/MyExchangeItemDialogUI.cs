//******************************************************************
// File Name:					MyExchangeItemDialogUI
// Description:					MyExchangeItemDialogUI class 
// Author:						lanjian
// Date:						3/3/2017 3:23:18 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Exchange;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 我的兑换dialog
    /// </summary>
    class MyExchangeItemDialogUI:DialogBaseUI
    {
        protected MyExchangeItemDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/MyExchangeInfoDialog";
            this.m_DType = DialogType.MyExchange;
        }

        public static MyExchangeItemDialogUI Create()
        {
            return new MyExchangeItemDialogUI();
        }

        private string m_resPath = "UIRootPrefabs/ExchangePanel_PageItem/exchangeObjectItem/myexchangeItem";
        private List<GameObject> m_myChangeList = new List<GameObject>();

        private Transform m_centerTran;

        private List<ExchangeItemOrder> m_orderList;

        private ExchangeItemOrder m_currentSelectOrder;

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

        private GameObject ReloadItem()
        {
            GameObject prefabs;
            prefabs = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(m_resPath) as GameObject);
            prefabs.transform.parent = this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0);
            prefabs.transform.localScale = Vector3.one;
            prefabs.transform.localPosition = Vector3.zero;
            prefabs.name = "myexchange";
            return prefabs;
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            for (int i = 0; i < m_myChangeList.Count; i++)
            {
                Utility.Utility.GetUIEventListener(m_myChangeList[i].transform.Find("desc").GetChild(1))
                    .onClick = OnViewMyExchageInfo;
            }
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            m_centerTran = this.m_DialogUIGo.transform.GetChild(1);
            //修正
            Utility.Utility.ModifyItemT0p(m_centerTran.GetChild(0).gameObject, new Vector3(0, 440, 0));
            //test
            for (int i = 0; i < m_orderList.Count; i++)
            {
                m_myChangeList.Add(ReloadItem());
            }
            //重排
            m_centerTran.GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();
            for (int i = 0; i < m_myChangeList.Count; i++)
            {
                GameObject item = m_myChangeList[i];
                //物品图片
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.ExchangePath + "/" + m_orderList[i].Item.Icon);
                if (texture != null)
                   item.transform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                Transform descTra = item.transform.GetChild(1);
                descTra.Find("name").GetComponent<UILabel>().text = m_orderList[i].Item.ExchangePrizeName;
                descTra.Find("price").GetComponent<UILabel>().text = m_orderList[i].TotalPay.ToString();
                descTra.Find("num").GetComponent<UILabel>().text = m_orderList[i].Count.ToString();
                descTra.Find("detailDesc").GetComponent<UILabel>().text = m_orderList[i].Item.ShortDesc;
                descTra.Find("orderNum").GetComponent<UILabel>().text = m_orderList[i].OrderId;
                NGUITools.SetActive(descTra.Find("haveRecstate").gameObject, true);
                NGUITools.SetActive(descTra.Find("onroadstate").gameObject, false);
                if (m_orderList[i].State == OrderState.ApplyFor)  
                {
                    descTra.Find("haveRecstate").GetComponent<UILabel>().text = "申请中";
                }
                if (m_orderList[i].State == OrderState.DenyDeliverd)
                {
                    descTra.Find("haveRecstate").GetComponent<UILabel>().text = "拒绝发货";
                }
                if (m_orderList[i].State == OrderState.Finished)
                {
                    descTra.Find("haveRecstate").GetComponent<UILabel>().text = "完成";
                }
                if (m_orderList[i].State == OrderState.Delivered)  
                {
                    NGUITools.SetActive(descTra.Find("haveRecstate").gameObject, false);
                    NGUITools.SetActive(descTra.Find("onroadstate").gameObject, true);
                }
            }
        }

        private void OnViewMyExchageInfo(GameObject go)
        {
            this.FindCurrentSelectEXOrder(go.transform.parent.Find("orderNum").GetComponent<UILabel>().text);
            DialogMgr.Load(DialogType.ExchangeLogistics);
            //将订单传给详情
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(m_currentSelectOrder));
        }

        private void FindCurrentSelectEXOrder(string id)
        {
            for (int i = 0; i < m_orderList.Count; i++)
            {
                if (id.Equals(m_orderList[i].OrderId))
                {
                    m_currentSelectOrder = m_orderList[i];
                    return;
                }
            }
        }

        private void RegistEvent()
        {
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.ExchnagePrizeOrder_change,OnGetOrdersBack);
        }

        //请求订单列表返回
        private void OnGetOrdersBack()
        {
            m_orderList = ExchangePrizeMgr.GetGetExchangeOrders();
            Debug.Log("我的订单记录"+m_orderList.Count);
            this.FillDataUI(null);
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("center/btnGroup"));
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.RegistEvent();
            this.GetDialogAbout();
            this.OpenDialog();
            //请求订单
            ExchangePrizeMgr.RequestGetExchangeOrders();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.ExchnagePrizeOrder_change, OnGetOrdersBack);
            base.DisPose();
        }
    }
}
