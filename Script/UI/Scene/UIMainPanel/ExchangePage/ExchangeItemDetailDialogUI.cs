//******************************************************************
// File Name:					ExchangeItemDetailDialogUI
// Description:					ExchangeItemDetailDialogUI class 
// Author:						lanjian
// Date:						3/2/2017 4:58:58 PM
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
    /// 查看物品详情dialog
    /// </summary>
    class ExchangeItemDetailDialogUI: DialogBaseUI
    {
        protected ExchangeItemDetailDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/ExchangeIItemDetailDialog";
            this.m_DType = DialogType.ExchangeItemDetail;
        }

        public static ExchangeItemDetailDialogUI Create()
        {
            return new ExchangeItemDetailDialogUI();
        }

        private ExchangePrizeItem m_currentExchangePrize;
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
            Transform descItem = this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0).Find("desc");
            Utility.Utility.GetUIEventListener(descItem.GetChild(0)).onClick = OnExchange;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            ExchangePrizeItem item = (ExchangePrizeItem)args[0];
            //物品图片
            Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.ExchangePath + "/" + item.Icon);
            if (texture != null)
                this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0)
                    .GetChild(1).GetComponent<UITexture>().mainTexture = texture;
            Transform descTra = this.m_DialogUIGo.transform.GetChild(1).GetChild(0).GetChild(0).Find("desc");
            descTra.Find("name").GetComponent<UILabel>().text = item.ExchangePrizeName;
            descTra.Find("price").GetComponent<UILabel>().text = item.Price.ToString();
            descTra.Find("num").GetComponent<UILabel>().text = item.RemainingCount.ToString();
            descTra.Find("detailDesc").GetComponent<UILabel>().text = item.ShortDesc;

            //Debug.Log(item.DetailDesc);
            string[] strArray = item.DetailDesc.Replace("\\\\RETURN","$").Split('$');
            string desc = "";
            for (int i = 0; i < strArray.Length; i++)
            {
                desc += strArray[i]+"\n";
            }
            descTra.Find("content").GetComponent<UILabel>().text = desc;
        }

        //兑换
        private void OnExchange(GameObject go)
        {
            if (m_currentExchangePrize != null)
            {
                //先判断是否金钱足够
                if (m_currentExchangePrize.Price > Role.Role.Instance().Gold)
                {
                    Utility.Utility.NotifyStr("您账号金钱不足！！");
                    return;
                }
                this.CloseDialog();
                DialogMgr.Load(DialogType.FillOutExchangeForm);
                DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
            }
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
            this.m_currentArgs = args;
            m_currentExchangePrize = (ExchangePrizeItem)this.m_currentArgs[0];
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
