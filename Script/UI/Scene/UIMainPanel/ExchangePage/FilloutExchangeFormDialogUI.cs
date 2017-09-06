//******************************************************************
// File Name:					FilloutExchangeFormDialogUI
// Description:					FilloutExchangeFormDialogUI class 
// Author:						lanjian
// Date:						3/2/2017 7:02:09 PM
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
    /// 填写兑换收件信息
    /// </summary>
    class FilloutExchangeFormDialogUI:DialogBaseUI
    {
        protected FilloutExchangeFormDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/ExchangeInfoDialog";
            this.m_DType = DialogType.FillOutExchangeForm;

        }

        public static FilloutExchangeFormDialogUI Create()
        {
            return new FilloutExchangeFormDialogUI();
        }

        private int m_ExchangeNum = -1;
        private bool m_isInitFinshed = false;      //是否初始化完成 可以updatel 
        private bool m_allVerfication;             //是否全部验证通过 

        private UIInput m_receiverInput;
        private UIInput m_phoneInput;
        private UIInput m_emailInput;
        private UIInput m_localcityInput;
        private UIInput m_detailInput;
        private UILabel m_waringTipLabel;


        private ExchangePrizeItem m_exchangPrize;
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
            Transform descItem = this.m_DialogUIGo.transform.Find("desc");
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            Utility.Utility.GetUIEventListener(descItem.Find("sub")).onClick = OnSubNum;
            Utility.Utility.GetUIEventListener(descItem.Find("add")).onClick = OnAddNum;
            Utility.Utility.GetUIEventListener(this.m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
            //检测输入框的变化
            Transform inputArea = this.m_DialogUIGo.transform.Find("inputArea");
            for (int i = 0; i < inputArea.childCount; i++)
            {
                Utility.Utility.GetUIEventListener(inputArea.GetChild(i)).onSelect = OnInputSelect;
            }
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            ExchangePrizeItem item = (ExchangePrizeItem)args[0];
            Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.ExchangePath + "/" + item.Icon);
            if (texture != null)
                this.m_DialogUIGo.transform.GetChild(2).GetComponent<UITexture>().mainTexture = texture;
            Transform descTra = this.m_DialogUIGo.transform.Find("desc");
            descTra.Find("name").GetComponent<UILabel>().text = item.ExchangePrizeName;
            descTra.Find("price").GetComponent<UILabel>().text = item.Price.ToString();
            descTra.Find("num").GetComponent<UILabel>().text = "1";
            descTra.Find("detailDesc").GetComponent<UILabel>().text = item.ShortDesc;
            //计算当前的数量  这个放这里不好
            TextConvertoInt();
            FindAllInput();
        }

        private void FindAllInput()
        {
            m_receiverInput = this.m_DialogUIGo.transform.Find("inputArea/receiver").GetComponent<UIInput>();
            m_phoneInput = this.m_DialogUIGo.transform.Find("inputArea/phone").GetComponent<UIInput>();
            m_emailInput = this.m_DialogUIGo.transform.Find("inputArea/email").GetComponent<UIInput>();
            m_localcityInput = this.m_DialogUIGo.transform.Find("inputArea/localtionCity").GetComponent<UIInput>();
            m_detailInput = this.m_DialogUIGo.transform.Find("inputArea/detailAdress").GetComponent<UIInput>();
            m_waringTipLabel = this.m_DialogUIGo.transform.Find("waringtip").GetComponent<UILabel>();
            NGUITools.SetActive(m_waringTipLabel.gameObject, false);
        }

        private void TextConvertoInt()
        {
            string text = this.m_DialogUIGo.transform.Find("desc/num").GetComponent<UILabel>().text;
            m_ExchangeNum = int.Parse(text.Trim());
        }

        //数量减
        private void OnSubNum(GameObject go)
        {
            if (m_ExchangeNum > 1)
            {
                m_ExchangeNum--;
                this.m_DialogUIGo.transform.Find("desc/num").GetComponent<UILabel>().text = m_ExchangeNum.ToString();
                SetButtonLight();
            }
            else
            {
                go.GetComponent<UISprite>().color = Color.grey;
            }
            //价格跟着数量变化
            this.m_DialogUIGo.transform.Find("desc/price").GetComponent<UILabel>().text 
                = m_ExchangeNum * m_exchangPrize.Price + "";
        }

        //数量加
        private void OnAddNum(GameObject go)
        {
            if (m_ExchangeNum < 99)
            {
                m_ExchangeNum++;
                this.m_DialogUIGo.transform.Find("desc/num").GetComponent<UILabel>().text = m_ExchangeNum.ToString();
                SetButtonLight();
            }
            else
            {
                go.GetComponent<UISprite>().color = Color.grey;
            }
            //价格跟着数量变化
            this.m_DialogUIGo.transform.Find("desc/price").GetComponent<UILabel>().text 
                = m_ExchangeNum * m_exchangPrize.Price + "";
        }

        private void SetButtonLight()
        {
            this.m_DialogUIGo.transform.Find("desc/sub").GetComponent<UISprite>().color = Color.white;
            this.m_DialogUIGo.transform.Find("desc/add").GetComponent<UISprite>().color = Color.white;
        }

        //选中那个输入框
        private void OnInputSelect(GameObject go,bool isSelect)
        {
           NGUITools.SetActive(go.transform.Find("tipsLabel").gameObject, isSelect);
        }

        private void OnConfirm(GameObject go)
        {
            NGUITools.SetActive(m_waringTipLabel.gameObject, true);
            if (m_allVerfication)
            {
                m_waringTipLabel.text = "            信息提交中。。。请稍候。。。";
                //前端验证通过
                string name = m_receiverInput.value;
                string phone = m_phoneInput.value;
                string email = m_emailInput.value;
                string city = m_localcityInput.value;
                string detailAdr = m_detailInput.value;
                Address adr = new Address(name, phone, email, city, detailAdr);
                //服务器检测玩家货币账户；



                DialogMgr.Load(DialogType.VerifyInfo);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(this.m_exchangPrize,this.m_ExchangeNum,adr));
            }
            else
            {
                m_waringTipLabel.text = "            请填写正确的信息。。。";
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
            m_exchangPrize = (ExchangePrizeItem)args[0];
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
            m_isInitFinshed = true;
        }
        
        public override void UpadateDialog()
        {      
            if(m_isInitFinshed)
            {
                bool isReceiverOk;
                bool isPhoneNumOk;
                bool isEmailOk;
                bool isCityOk;
                bool isDatilCityOK;
                ///满足用户名条件
                if (Utility.Utility.IsMathRex(@"^([\u4e00-\u9fa5]{1,20}|[a-zA-Z\.\s]{1,20})$", m_receiverInput.value))
                {
                    m_receiverInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_full";
                    isReceiverOk = true;
                }
                else
                {
                    m_receiverInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_empty";
                    isReceiverOk = false;
                }

                ///满足手机号条件
                if (Utility.Utility.IsMathRex(@"^((\+)?86|((\+)?86)?)0?1[3578]\d{9}$", m_phoneInput.value))
                {
                    m_phoneInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "phone_full";
                    isPhoneNumOk = true;
                }
                else
                {
                    m_phoneInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "phone_empty";
                    isPhoneNumOk = false;
                }

                ///满足邮箱条件
                if (Utility.Utility.IsMathRex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", m_emailInput.value))
                {
                    m_emailInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "mail_full";
                    isEmailOk = true;
                }
                else
                {
                    m_emailInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "mail_empty";
                    isEmailOk = false;
                }
                ///满足城市名条件
                if (m_localcityInput.value.Length>2)
                {
                    m_localcityInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "city_full";
                    isCityOk = true;
                }
                else
                {
                    m_localcityInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "city_empty";
                    isCityOk = false;
                }
                /////满足详细地址
                if (m_detailInput.value.Length>5)
                {
                    m_detailInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "address_full";
                    isDatilCityOK = true;
                }
                else
                {
                    m_detailInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "address_empty";
                    isDatilCityOK = false;
                }

                if (isReceiverOk && isPhoneNumOk && isEmailOk && isCityOk && isDatilCityOK)
                {
                    m_allVerfication = true;
                    NGUITools.SetActive(m_waringTipLabel.gameObject,true);
                    m_waringTipLabel.text = " 警告：请认真核对您填写的资料，因资料错误造成的一切后果本公司概不负责";
                }
            }
        }

        
    }
}