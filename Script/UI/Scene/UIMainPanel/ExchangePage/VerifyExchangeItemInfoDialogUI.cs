//******************************************************************
// File Name:					VerifyExchangeItemInfoDialogUI
// Description:					VerifyExchangeItemInfoDialogUI class 
// Author:						lanjian
// Date:						3/3/2017 1:58:56 PM
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
    /// 核实兑换界面
    /// </summary>
    class VerifyExchangeItemInfoDialogUI:DialogBaseUI
    {
        protected VerifyExchangeItemInfoDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/VerifyExchangeInfoDialog";
            this.m_DType = DialogType.VerifyInfo;

        }

        public static VerifyExchangeItemInfoDialogUI Create()
        {
            return new VerifyExchangeItemInfoDialogUI();
        }

        private ExchangePrizeItem m_currentExchangePirze;
        private int m_exchangeNum;
        private Address m_address;
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
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            //物品图片
            Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.ExchangePath + "/" + m_currentExchangePirze.Icon);
            if (texture != null)
                this.m_DialogUIGo.transform.GetChild(2).GetComponent<UITexture>().mainTexture = texture;
            Transform descTra = this.m_DialogUIGo.transform.Find("desc");
            descTra.Find("name").GetComponent<UILabel>().text = m_currentExchangePirze.ExchangePrizeName;
            descTra.Find("price").GetComponent<UILabel>().text = m_currentExchangePirze.Price.ToString();
            descTra.Find("price1").GetComponent<UILabel>().text = m_currentExchangePirze.Price.ToString();
            descTra.Find("num").GetComponent<UILabel>().text = this.m_exchangeNum.ToString();
            descTra.Find("detailDesc").GetComponent<UILabel>().text = m_currentExchangePirze.ShortDesc;
            Transform inputArea = this.m_DialogUIGo.transform.Find("inputArea");
            inputArea.GetChild(0).GetChild(1).GetComponent<UILabel>().text = this.m_address.Name;
            inputArea.GetChild(1).GetChild(1).GetComponent<UILabel>().text = this.m_address.Phone;
            inputArea.GetChild(2).GetChild(1).GetComponent<UILabel>().text = this.m_address.Email;
            inputArea.GetChild(3).GetChild(1).GetComponent<UILabel>().text = this.m_address.City;
            inputArea.GetChild(4).GetChild(1).GetComponent<UILabel>().text = this.m_address.DetailAddr;
            //核对下钱够不够
            CheckUserMoney();
        }

        private void CheckUserMoney()
        {
            Transform descTra = this.m_DialogUIGo.transform.Find("desc");
            int totolGold = m_exchangeNum * m_currentExchangePirze.Price;

            if (Role.Role.Instance().Gold >= totolGold)
            {
                descTra.Find("price").GetComponent<UILabel>().text = totolGold.ToString();
                NGUITools.SetActive(descTra.GetChild(3).gameObject, false);
                NGUITools.SetActive(descTra.GetChild(4).gameObject, false);
            }

            int[] arr = Utility.Utility.Calculate(totolGold,Role.Role.Instance().Gold);
            if (arr[1] > Role.Role.Instance().Diamond)
            {
                Debug.Log("信息核对，你的钱不够");
                descTra.Find("price").GetComponent<UILabel>().text = "老板，钱不够";
                NGUITools.SetActive(descTra.GetChild(1).gameObject, false);
                NGUITools.SetActive(descTra.GetChild(2).gameObject, true);
                NGUITools.SetActive(descTra.GetChild(3).gameObject, false);
                NGUITools.SetActive(descTra.GetChild(4).gameObject, false);
                //将确定按钮隐藏
                NGUITools.SetActive(this.m_DialogUIGo.transform.Find("confirm").gameObject, false);
                return;
            }
            else
            {
                Debug.Log("信息核对，差价处理了");
                descTra.Find("price").GetComponent<UILabel>().text = arr[0].ToString();
                descTra.Find("price1").GetComponent<UILabel>().text = arr[1].ToString();
            }
        }

        private void OnConfirm(GameObject go)
        {
            //确定 输入密码
            DialogMgr.Load(DialogType.InputPassWord);
            DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
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
            //三个 参数  兑换物品  兑换数量  收件地址
            m_currentExchangePirze = (ExchangePrizeItem)args[0];
            m_exchangeNum = (int)args[1];
            m_address = (Address)args[2];
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
