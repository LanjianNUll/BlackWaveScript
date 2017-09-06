//******************************************************************
// File Name:					UISceneLogin.cs
// Description:					UISceneLogin class 
// Author:						wuwei
// Date:						2017.01.04
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FW.UI
{
    class UISceneLogin : UISceneBase
    {
        private UIInput m_UserNameInput;                //输入框
        private UIInput m_PassWordInput;
        private UIToggle m_AutoLoginCheckBox;
        private GameObject m_NotifactionBar;

        private string m_UserAccount;                     //帐号
        private string m_UserPassWord;                    //用户密码
        private bool m_IsAutoLogin;                       //是否自动登陆
        private bool m_isModifyUserOrPwd;                 //是否修改过密码
        private int m_whichInputChange = -1;                   //改变了 0用户  1密码  
        private bool m_IsFrontVerificteOk;                //前端验证是否通过

        private string m_currentIpAr = "14.17.65.164";     //测试  ip

        protected UISceneLogin()
        {
            this.m_resName = "UIRootPrefabs/LoginUIRoot";
            //注册
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.LOGIN_INFO, this.OnLoginInfo);
        }

        public static UISceneBase Create()
        {
            return new UISceneLogin();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public string UserAccount { get { return m_UserAccount; } }                     //帐号
        public string UserPassWord { get { return m_UserPassWord; } }                   //用户密码
        public bool IsAutoLogin { get { return m_IsAutoLogin; } }                       //是否自动登陆

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI() {
            string rootPath = "UIRootContainer/LoginUIRoot(Clone)/LoginPanel/";
            m_UserNameInput = GameObject.Find(rootPath + "center/UserNameInput").GetComponent<UIInput>();
            m_PassWordInput = GameObject.Find(rootPath + "center/PassWordInput").GetComponent<UIInput>();
            m_AutoLoginCheckBox = GameObject.Find(rootPath + "center/AutoLogin").GetComponent<UIToggle>();
            m_NotifactionBar = GameObject.Find(rootPath + "bottom/notifaction");
            //ip测试
            IPTest();
        }

        private void LoadUIComponnet()
        {
            FindAllUI();
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.UI_LOGIN_LOGIN_BUTTON, OnLoginBtn);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.UI_LOGIN_AUTOLOGIN_BUTTON, OnAutoLoginBtn);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.UI_LOGIN_QUICKLOGIN_BUTTON, OnQuickLogin);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.UI_LOGIN_CHANGE_CONTENT, OnNameOrPwdChange);
            //隐藏通知栏
            this.m_NotifactionBar.SetActive(false);

            this.SetUserNameAndPwd(Login.LoginConfig.UserName, Login.LoginConfig.UserPwd);
            this.m_IsAutoLogin = Login.LoginConfig.IsAutoLogin;
            this.m_AutoLoginCheckBox.value = this.m_IsAutoLogin;

            this.m_isModifyUserOrPwd = false;
            this.m_whichInputChange = -1;
        }

        private void SetUserNameAndPwd(string name, string pwd)
        {
            if (string.IsNullOrEmpty(name)) return;
            this.m_UserAccount = name;
            this.m_UserPassWord = pwd;

            this.m_UserNameInput.value = name;
            this.m_PassWordInput.value = "12345678";    //改了下 回填的时候不要符号 
        }

        //检查自动登陆
        private void CheckAutoLogin()
        {
            if (m_IsAutoLogin)
            {
                m_UserAccount = Login.LoginConfig.UserName;
                m_UserPassWord = Login.LoginConfig.UserPwd;
                m_currentIpAr = Login.LoginConfig.IpAdr;
                FW.Login.LoginHandler.Login(m_UserAccount, m_UserPassWord, m_currentIpAr);
            }
        }

        //登录消息的回调
        private void OnLoginInfo(FW.Event.EventArg args)
        {
            int ret = (int)args[0];
            if(ret == 0)
            {
                Debug.Log("UI---Login Success");
                SaveLoginInfoToConfig();
                //进入main场景
                Scene.SceneMgr.Enter(Scene.SceneType.Main);
            }
            string msg = "";
            switch (ret)
            {
                case 0: msg = Localization.Get("SuccessLogin"); break;
                case 1: msg = Localization.Get("AccountError"); break;
                case 2: msg = Localization.Get("PassWordError"); break;
                case 3:msg = Localization.Get("GatewayValidationError"); break;
                default:
                    break;
            }
            DisPlayNotifactionBar(msg);
        }

        //保存登录信息文件
        private void SaveLoginInfoToConfig()
        {
            FW.Login.LoginConfig.SetUserAutoLogin(m_IsAutoLogin);
            FW.Login.LoginConfig.SetLoginIPAdr(m_currentIpAr);
            FW.Login.LoginConfig.SaveFile();
            
        }

        private void DisPlayNotifactionBar(string msg, bool isStay = false)
        {
            this.m_NotifactionBar.SetActive(true);
            this.m_NotifactionBar.transform.GetChild(0).gameObject.GetComponent<UILabel>().text = msg;
            //是否停留下消失
            if (isStay)
            {
                TweenAlpha ta = this.m_NotifactionBar.GetComponent<TweenAlpha>();
                ta.ResetToBeginning();
                ta.enabled = true;
                ta.duration = 2;
                ta.PlayForward();
                EventDelegate.Set(ta.onFinished, HideNotifacionBar);
            }
        }

        private void HideNotifacionBar()
        {
            this.m_NotifactionBar.SetActive(false);
        }

        private bool IsMathRex(string patternStr,string inputStr)
        {
            Regex regex = new Regex(patternStr);
            return regex.IsMatch(inputStr);
        }

        private void IPTest()
        {
            Utility.Utility.GetUIEventListener(this.RootObj.transform.Find("top/test/226")).onClick = TestBtn;
            Utility.Utility.GetUIEventListener(this.RootObj.transform.Find("top/test/185")).onClick = TestBtn;
            Utility.Utility.GetUIEventListener(this.RootObj.transform.Find("top/test/231")).onClick = TestBtn;
            //回填
            m_currentIpAr = Login.LoginConfig.IpAdr;
            this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value = m_currentIpAr;
        }

        private void TestBtn(GameObject go)
        {
            //将登录状态回到路由
            Login.LoginHandler.Recover();
            if (go.name.Equals("226"))
            {
                this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value = "14.17.65." + go.name;
                Login.LoginConfig.SetLoginIPAdr("14.17.65." + go.name);
                //this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value = "192.168.8.134";
                //Login.LoginConfig.SetLoginIPAdr("192.168.8.134");
            }
            else
            { 
                this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value = "192.168.8." + go.name;
                Login.LoginConfig.SetLoginIPAdr("192.168.8." + go.name);
            }
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public override void Init()
        {
            base.Init();
            this.m_IsAutoLogin = Login.LoginConfig.IsAutoLogin;
            //检查自动登陆
            CheckAutoLogin();
       }

        //销毁
        public override void DisPose()
        {
            //反注册
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.LOGIN_INFO, this.OnLoginInfo);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.UI_LOGIN_LOGIN_BUTTON, OnLoginBtn);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.UI_LOGIN_AUTOLOGIN_BUTTON, OnAutoLoginBtn);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.UI_LOGIN_QUICKLOGIN_BUTTON, OnQuickLogin);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.UI_LOGIN_CHANGE_CONTENT, OnNameOrPwdChange);

            base.DisPose();
        }

        //绑定消息
        public override void BindScript(UIEventBase eventBase)
        {
            LoadUIComponnet();
        }

        //响应自动登陆按钮
        public void OnAutoLoginBtn(Event.EventArg args)
        {
            this.m_IsAutoLogin = this.m_AutoLoginCheckBox.value;
            Login.LoginConfig.SetUserAutoLogin(this.m_IsAutoLogin);
            Debug.Log("UI-----" + m_IsAutoLogin);
        }

        //响应登陆按钮
        public void OnLoginBtn(Event.EventArg args)
        {
            if (m_IsFrontVerificteOk)            //前端验证通过
            {
                //DisPlayNotifactionBar("登录中...请稍候！");  国际化
                DisPlayNotifactionBar(Localization.Get("Logging"));
                if (this.m_isModifyUserOrPwd)
                {
                    this.m_UserAccount = this.m_UserNameInput.value.Trim();
                    Debug.Log(this.m_UserAccount.Length);
                }
                if (this.m_whichInputChange == 1)
                {
                    this.m_UserPassWord = Utility.Utility.MD5(this.m_PassWordInput.value);
                }
                //test
                m_currentIpAr = this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value;
                Debug.Log("当前登录的ip为"+m_currentIpAr);
                FW.Login.LoginHandler.Login(this.m_UserAccount, this.m_UserPassWord,m_currentIpAr);
            }
            else
            {
                DisPlayNotifactionBar(Localization.Get("AccountOrPasswordFormatError"));
            }
        }

        //响应快速登陆按钮
        public void OnQuickLogin(Event.EventArg args)
        {
            DisPlayNotifactionBar(Localization.Get("Logging"));
            //test
            m_currentIpAr = this.RootObj.transform.Find("top/test/ipInput").GetComponent<UIInput>().value;
            Debug.Log("当前登录的ip为" + m_currentIpAr);
            FW.Login.LoginHandler.Login("", "", m_currentIpAr);
        }

        //响应id or pwd修改
        public void OnNameOrPwdChange(Event.EventArg args)
        {
            this.m_isModifyUserOrPwd = (bool)args[0];
            this.m_whichInputChange = (int)args[1];
        }

        /// <summary>
        /// 前端校验，人性化
        /// </summary>
        public override void UpdateInput()
        {
            bool isNameOk;
            bool isPassWordOk;
            ///满足用户名条件  最多16个字符 (NGUI控件控制)   可以包含中英文与数字   [\u4e00-\u9fa5]
            if (IsMathRex(@"^[0-9A-Za-z\u4e00-\u9fa5]+$", m_UserNameInput.value))
            {
                m_UserNameInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_full";
                isNameOk = true;
            }
            else
            {
                m_UserNameInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_empty";
                isNameOk = false;
            }
            ///满足密码条件    最多16个字符 (NGUI控件控制)   只能包含英文大小写和数字；
            if (IsMathRex(@"^[0-9A-Za-z]+$", m_PassWordInput.value))
            {
                m_PassWordInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_01_full";
                isPassWordOk = true;
            }
            else
            {
                m_PassWordInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "password_01_empty";
                isPassWordOk = false;
            }
            if (isNameOk && isPassWordOk)
            {
                m_IsFrontVerificteOk = true;
            }
            else
            {
                m_IsFrontVerificteOk = false;
            }
        }
    }
}