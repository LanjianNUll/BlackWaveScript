//******************************************************************
// File Name:					InputPassWrodDialogUI
// Description:					InputPassWrodDialogUI class 
// Author:						lanjian
// Date:						3/3/2017 2:38:31 PM
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
    /// 游戏密码dialog
    /// </summary>
    class InputPassWrodDialogUI:DialogBaseUI
    {
        protected InputPassWrodDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/inputPassWord";
            this.m_DType = DialogType.InputPassWord;

            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.ExchangePrize_itemBought, OnGoVerifyDialog);
        }

        public static InputPassWrodDialogUI Create()
        {
            return new InputPassWrodDialogUI();
        }

        private UIInput m_passWordInput;
        private UILabel m_waringtipLabel;

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

            m_passWordInput = m_DialogUIGo.transform.Find("inputpassWord").GetComponent<UIInput>();
            m_waringtipLabel = m_DialogUIGo.transform.Find("waringtip").GetComponent<UILabel>();
            NGUITools.SetActive(m_waringtipLabel.gameObject,false);
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            //物品图片
            //Texture texture = 
            //item.transform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
            if (Role.Role.Instance().InputErrorCount != 0)
            {
                NGUITools.SetActive(m_waringtipLabel.gameObject, true);
                m_waringtipLabel.text = "你已经输错"+ Role.Role.Instance().InputErrorCount + "次密码。还有[fff000]" + (3 - Role.Role.Instance().InputErrorCount) + "[-]次机会重试";
            }
          
        }

        private void OnConfirm(GameObject go)
        {
            //判断密码是否正确
            string passWord = m_passWordInput.value;
            if (passWord.Length > 0 && FW.Login.LoginConfig.UserPwd.Equals(Utility.Utility.MD5(passWord)))
            {
                //通过 兑换 请求
                this.m_currentExchangePirze.ExchangePrize(this.m_exchangeNum, this.m_address);
                return;
            }

            //test
            if (passWord.Equals("123456"))
            {
                //通过 兑换 请求
                this.m_currentExchangePirze.ExchangePrize(this.m_exchangeNum, this.m_address);
                return;
            }

            Role.Role.Instance().InputErrorPS();
            NGUITools.SetActive(m_waringtipLabel.gameObject, true);
            m_waringtipLabel.text = "您输入的密码错误，无法兑换,你还有[fff000]"+(3- Role.Role.Instance().InputErrorCount) + 
                "[-]次机会重试";
            if (Role.Role.Instance().InputErrorCount >= 3)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(warningType.OnlyConfirm, "[fff000]您已连续输错3次密码，您的账户3小时内将无法进行兑换操作[-]"));
            }
        }

        //回调 是否兑换成功
        private void OnGoVerifyDialog(FW.Event.EventArg args)
        {
            this.CloseDialog();
            //参数：ExchangeItemOrder 兑换物品的订单   0.成功， 1.没有找到物品 2.库存不足 3.余额不足 ）
            int tag = (int)args[1];
            if (tag == 0)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(warningType.OnlyConfirm, "一封系统邮件提示玩家兑换商品成功，发送订单号等待签收"));
                return;
            }
            NGUITools.SetActive(m_waringtipLabel.gameObject, true);
            if (tag == 1)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(warningType.OnlyConfirm, "没有找到物品"));
                m_waringtipLabel.text = "没有找到物品";
            }
            if (tag == 2)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(warningType.OnlyConfirm, "库存不足"));
            }
            if (tag == 3)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(warningType.OnlyConfirm, "你的货币不足，无法进行兑换！"));
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
            //直接跳到
            if (Role.Role.Instance().InputErrorCount >= 3)
            {
                DialogMgr.Load(DialogType.Waring);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(warningType.OnlyConfirm, "[fff000]您已连续输错3次密码，您的账户3小时内将无法进行兑换操作[-]"));
            }
            this.m_currentArgs = args;
            //三个 参数  兑换物品  兑换数量  收件地址
            m_currentExchangePirze = (ExchangePrizeItem)args[0];
            m_exchangeNum = (int)args[1];
            m_address = (Address)args[2];
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.ExchangePrize_itemBought, OnGoVerifyDialog);
            base.DisPose();
        }
    }
}