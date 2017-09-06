//******************************************************************
// File Name:					ExchangeBasePage
// Description:					ExchangeBasePage class 
// Author:						lanjian
// Date:						3/2/2017 3:58:37 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Exchange;

namespace FW.UI
{
    class ExchangeBasePage : ScrollViewItemBase
    {
        protected string m_sortName;
        protected ExchangeBasePage()
        {
            this.gunItemList = "UIRootPrefabs/ExchangePanel_PageItem/exchangeObjectItem/exchangeItem";
            this.m_ItemHeight = 700;
        }

        protected Transform m_centerTran;
        protected List<ExchangePrizeItem> m_ExchangePrizieList;
        protected List<GameObject> m_exchangePrizeGoList = new List<GameObject>();

        protected ExchangePrizeItem m_currentExPrize;

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindCurrentClick(string id)
        {
            int ClickId = -1;
            if (int.TryParse(id, out ClickId))
            {
                for (int i = 0; i < m_ExchangePrizieList.Count; i++)
                {
                    if (ClickId == m_ExchangePrizieList[i].ID)
                        m_currentExPrize = m_ExchangePrizieList[i];
                }
            }
        }

        public void FillDataTOItem()
        {
            for (int i = 0; i < m_exchangePrizeGoList.Count; i++)
            {
                GameObject item = m_exchangePrizeGoList[i];
                //设置头部名称
                this.CurrentItem.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = this.m_sortName;
                Utility.Utility.GetUIEventListener(item.transform.GetChild(0)).onClick = OnViewDetail;
                Utility.Utility.GetUIEventListener(item.transform.GetChild(1).Find("exchange")).onClick = OnExchange;
                //物品图片
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.ExchangePath+"/"+ m_ExchangePrizieList[i].Icon);
                if(texture!=null)
                    item.transform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                Transform descTra = item.transform.GetChild(1);
                descTra.Find("name").GetComponent<UILabel>().text = m_ExchangePrizieList[i].ExchangePrizeName;
                descTra.Find("price").GetComponent<UILabel>().text = m_ExchangePrizieList[i].Price.ToString() ;
                descTra.Find("num").GetComponent<UILabel>().text = m_ExchangePrizieList[i].RemainingCount.ToString();
                descTra.Find("detailDesc").GetComponent<UILabel>().text = m_ExchangePrizieList[i].ShortDesc;
                descTra.Find("itemID").GetComponent<UILabel>().text = m_ExchangePrizieList[i].ID.ToString();
            }
        }

        //请求返回兑换返回
        public void OnGetExchangePSBack()
        {
            this.FillItem(null);
        }

        //显示兑换物品详情详情
        public void OnViewDetail(GameObject go)
        {
            FindCurrentClick(go.transform.parent.Find("desc/itemID").GetComponent<UILabel>().text);
            if (m_currentExPrize != null)
            {
                DialogMgr.Load(DialogType.ExchangeItemDetail);
                DialogMgr.CurrentDialog.ShowCommonDialog(new EventArg(m_currentExPrize));
            }
        }

        //兑换按钮
        public void OnExchange(GameObject go)
        {
            FindCurrentClick(go.transform.parent.Find("itemID").GetComponent<UILabel>().text);
            if (m_currentExPrize != null)
            {
                //先判断是否金钱足够
                if (m_currentExPrize.Price > Role.Role.Instance().Gold)
                {
                    Utility.Utility.NotifyStr("您账号金钱不足！！");
                    return;
                }
                    
                DialogMgr.Load(DialogType.FillOutExchangeForm);
                DialogMgr.CurrentDialog.ShowCommonDialog(new EventArg(m_currentExPrize));
            }
           
        }
        public override void FillItem(EventArg eventArg)
        {
            m_centerTran = this.CurrentItem.transform.GetChild(1);
            //修正
            Utility.Utility.ModifyItemT0p(m_centerTran.GetChild(0).gameObject, new Vector3(0, 440, 0));
        }
    }
}
