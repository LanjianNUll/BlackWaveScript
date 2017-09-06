//******************************************************************
// File Name:					PanelConverOffical
// Description:					PanelConverOffical class 
// Author:						lanjian
// Date:						1/5/2017 4:22:37 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FW.UI
{
    class PanelConverOffical:PanelBase
    {
        private UIInput m_OfficalAccountInput;           //正式账号
        private UIInput m_PassWordInput;                 //密码
        private UIInput m_PassWordAgainInput;            //再次密码
        private UIInput m_PhoneNumberInput;              //手机号   
        private GameObject m_NotifacionBar;              //通知栏   
        //private GameObject m_InputBgLight;               //输入时的背景光晕

        private bool m_IsAutoLogin;
        private string m_OfficalAccout;
        private string m_PassWord;
        private string m_PassWordAgain;
        private string m_PhoneNumber;

        private bool m_IsFrontVerificteOk;                 //前端验证是否通过

        protected PanelConverOffical()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/ConverOfficalPanel";
            this.panelType = PanelType.ConverOffical;
        }

        public static PanelConverOffical Create()
        {
            return new PanelConverOffical();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            //Debug.Log("当前面板的"+PanelMgr.CurrPanel.RootObj.name);
            Transform currenTransform = PanelMgr.CurrPanel.RootObj.transform;
            Transform centerTransform = currenTransform.Find("center");
            m_OfficalAccountInput = centerTransform.Find("officalAccountInput").GetComponent<UIInput>();
            m_PassWordInput = centerTransform.Find("passWordInput").GetComponent<UIInput>();
            m_PassWordAgainInput = centerTransform.Find("passWordInputAgain").GetComponent<UIInput>();
            m_PhoneNumberInput = centerTransform.Find("phoneNumberInput").GetComponent<UIInput>();
            //m_InputBgLight = centerTransform.Find("bgLight").gameObject;
            m_NotifacionBar = currenTransform.Find("bottom").Find("notifaction").gameObject;
            //m_NotifacionBar.SetActive(false);
            NGUITools.SetActive(m_NotifacionBar,false);
        }

        private void ShowNotifactionBar(string msg)
        {
            //m_NotifacionBar.SetActive(true);
            NGUITools.SetActive(m_NotifacionBar, true);
            m_NotifacionBar.GetComponentInChildren<UILabel>().text = msg;
        }
        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_CONVEROFFICAL_CONFIMR_BTN,OnConfirmBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_CONVEROFFICAL_CANEL_BTN, OnCancelBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_CONVEROFFICAL_AUTOLOGIN_BTN, OnAutoLogin);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Bind_Account, OnConverToOfficalCommit);
        }

        //转为正式账号的回调
        private void OnConverToOfficalCommit(FW.Event.EventArg args)
        {
            //int 状态(0：成功 1：失败2：账号名错误 
            //    3：旧密码错误4：新账号名格式不对5：新账号名 已被使用6：新密码格式不对7：手机号码格式不对))
            int returnNum = (int)args[0];
            switch (returnNum)
            {
                case 0: PanelMgr.BackToMainPanel(); SaveLoginInfoToConfig(); break;    
                case 1: ShowNotifactionBar("失败"); break;
                case 2: ShowNotifactionBar("账号名错误 "); break;
                case 3: ShowNotifactionBar("旧密码错误"); break;
                case 4: ShowNotifactionBar("新账号名格式不对"); break;
                case 5: ShowNotifactionBar("新账号名 已被使用"); break;
                case 6: ShowNotifactionBar("新密码格式不对"); break;
                case 7: ShowNotifactionBar("手机号码格式不对"); break;
                default:
                    break;
            }
        }

        //保存登录信息文件
        private void SaveLoginInfoToConfig()
        {
            FW.Login.LoginConfig.SetUserNameAndPwd(m_OfficalAccout, Utility.Utility.MD5(this.m_PassWord));
            FW.Login.LoginConfig.SetUserAutoLogin(m_IsAutoLogin);
            FW.Login.LoginConfig.SaveFile();
        }

        private void OnConfirmBtn()
        {
            if (m_IsFrontVerificteOk)
            {
                //确定转换为正式账号
                m_OfficalAccout = m_OfficalAccountInput.value;
                m_PassWord = m_PassWordInput.value;
                m_PassWordAgain = m_PassWordAgainInput.value;
                m_PhoneNumber = m_PhoneNumberInput.value;
                if (!m_PassWord.Equals(m_PassWordAgain))
                {
                    //ShowNotifactionBar("前后密码不一致！！");
                    ShowNotifactionBar(Localization.Get("PasswordNotConsistent"));
                    return;
                }
                //ShowNotifactionBar("信息提交中...请稍候");
                ShowNotifactionBar(Localization.Get("Committing"));
                FW.Login.LoginHandler.BindAccount(this.m_OfficalAccout,
                    Utility.Utility.MD5(this.m_PassWord),m_PhoneNumber, this.m_IsAutoLogin);

            }
            else
            {
                //ShowNotifactionBar("填写信息有误！！！");
                ShowNotifactionBar(Localization.Get("InfoError"));
            }
        }

        private void OnCancelBtn()
        {
            PanelMgr.BackToMainPanel();
        }

        private void OnAutoLogin()
        {
            m_IsAutoLogin = !m_IsAutoLogin;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            ResgistEvents();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_CONVEROFFICAL_CONFIMR_BTN, OnConfirmBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_CONVEROFFICAL_CANEL_BTN, OnCancelBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_CONVEROFFICAL_AUTOLOGIN_BTN, OnAutoLogin);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Bind_Account, OnConverToOfficalCommit);

            base.DisPose();
        }

        /// <summary>
        /// 检查所有填入是否正确，正确就填充图标
        /// </summary>
        public override void UpdateInput()
        {
            bool isAccountOk;
            bool isPassWordOk;
            bool isPassWordAaginOk;
            bool isPhoneNumberOk;
            ///满足用户名条件
            if (Utility.Utility.IsMathRex(@"^[0-9A-Za-z\u4e00-\u9fa5]+$", m_OfficalAccountInput.value))
            {
                m_OfficalAccountInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_full";
                isAccountOk = true;
            }
            else
            {
                m_OfficalAccountInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_empty";
                isAccountOk = false;
            }
            ///满足密码条件
            if (Utility.Utility.IsMathRex(@"^[0-9A-Za-z]+$", m_PassWordInput.value))
            {
                m_PassWordInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_01_full";
                isPassWordOk = true;
            }
            else
            {
                m_PassWordInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_01_empty";
                isPassWordOk = false;
            }
            ///满足再次输入条件
            if (Utility.Utility.IsMathRex(@"^[0-9A-Za-z]+$", m_PassWordAgainInput.value))
            {
                m_PassWordAgainInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_02_full";
                isPassWordAaginOk = true;
            }
            else
            {
                m_PassWordAgainInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_02_empty";
                isPassWordAaginOk = false;
            }
            ///满足手机条件条件
            if (Utility.Utility.IsMathRex(@"^((\+)?86|((\+)?86)?)0?1[3578]\d{9}$", m_PhoneNumberInput.value))
            {
                m_PhoneNumberInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "phone_full";
                isPhoneNumberOk = true;
            }
            else
            {
                m_PhoneNumberInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "phone_empty";
                isPhoneNumberOk = false;
            }
            if (isAccountOk && isPassWordOk && isPassWordAaginOk && isPhoneNumberOk)
            {
                m_IsFrontVerificteOk = true;
            }
        }
    }
}
