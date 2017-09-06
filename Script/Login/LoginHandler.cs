//******************************************************************
// File Name:					LoginHandler.cs
// Description:					LoginHandler class 
// Author:						wuwei
// Date:						2016.12.27
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Network;
using Network.Serializer;
using FW.Event;


namespace FW.Login
{
    enum ELoginState
    {
        Route,
        Login,
        Gate,
        Game,       //成功
    }

    static class LoginHandler
    {
        private static int sm_reconnectTimes = 0; //重连计数
        private static int sm_maxTimes = 0; //本次最高重连尝试次数

        private static int MAX_CONNNECT_TIMES = 3; //启动游戏时尝试连接次数
        private static int MAX_RECONNNECT_TIMES = 10; //游戏中断线最多自动尝试重连次数
        private static int RECONNECT_INTERVAL = 3; //自动重连间隔
        private static Int64 sm_reConnectCBID;

        private static ELoginState sm_LoginState;   //登陆状态
        private static Int64 sm_intervalID;         //心跳计时id
        private static float sm_interval;           //心跳间隔

        private static string sm_loginIP;           //当前登陆ip
        private static int sm_loginPort;            //当前登陆port

        private static string sm_name;              //帐号
        private static string sm_pwd;               //密码
        private static int sm_account;              //帐号ID
        private static string sm_token;             //token
        private static int sm_serverID;             //serverID

        private static string sm_bindName;          //绑定帐号
        private static string sm_bindPwd;           //绑定帐号密码
        private static string sm_bindPhone;         //绑定帐号手机号
        private static bool sm_isAutoLogin;         //是否自动登陆
        private static bool sm_isTourist;           //是否游客帐号登陆
        private static bool sm_isReconnect;         //是否重连成功

        static LoginHandler()
        {
            sm_intervalID = -1;                     //心跳计时id
            sm_interval = 60.0f;                    //心跳间隔
            sm_reConnectCBID = -1;
            FWEvent.Instance.Regist(EventID.NET_CONN_STATE, OnNetConnState);

            NetDispatcherMgr.Inst.Regist(Commond.Get_Info_Error, OnGetInfoError);			// 登陆成功回调
            NetDispatcherMgr.Inst.Regist(Commond.Login_Rounte_back, OnLoginRounte);			// 登陆成功回调
            NetDispatcherMgr.Inst.Regist(Commond.Login_Account_back, OnLoginServer);        // 登陆帐号回调
            NetDispatcherMgr.Inst.Regist(Commond.Login_Gate_back, OnLoginGate);             // 登陆gate回调
            NetDispatcherMgr.Inst.Regist(Commond.Login_reConnect_back, OnLoginReConnect);   // 重新登陆gate回调

            NetDispatcherMgr.Inst.Regist(Commond.Create_Guest_back, OnCreateGuest);         // 创建游客帐号回调

            NetDispatcherMgr.Inst.Regist(Commond.Bind_Account_back, OnBindAccount);         // 绑定帐号回调


            sm_LoginState = ELoginState.Route;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static bool IsTourist { get { return sm_isTourist; } }                       //是否游客帐号登陆
        /// <summary>
        /// 登陆状态
        /// </summary>
        public static ELoginState LoginState {get{return sm_LoginState ;} }
        //--------------------------------------
        //private 
        //--------------------------------------

        private static void OnGetInfoError(DataObj data)
        {
            if (data == null) return;
            int type = data.GetInt32("type");
            if(type > 0)
            {
                Event.FWEvent.Instance.Call(EventID.Get_Info_Error, new EventArg(type));
            }
        }
         
        //网络状态变化
        private static void OnNetConnState(EventArg arg)
        {
            NetMgr.CONN_MSG connMsg = (NetMgr.CONN_MSG)arg[0];

            switch (connMsg)
            {
                case NetMgr.CONN_MSG.ClientBreak:
                case NetMgr.CONN_MSG.ServerBreak:
                    NetMgr.Instance.LockQueue = true; //锁定网络请求队列
                    sm_maxTimes = MAX_RECONNNECT_TIMES;
                    sm_reconnectTimes = 0;
                    Debug.LogFormat("network is broken......");
                    ReConnect();
                    break;
                case NetMgr.CONN_MSG.Fail:
                    //ReConnect();
                    break;

                case NetMgr.CONN_MSG.TimeOut:
                    //ReConnect();
                    break;

                case NetMgr.CONN_MSG.Success:
                    if(sm_intervalID < 0)
                    {
                        sm_intervalID = Timer.Regist(sm_interval, sm_interval, OnIntervalCB);
                    }
                    NetMgr.Instance.LockQueue = false;
                    OnConnectCB();
                    break;
            }
        }

        //发送心跳包
        private static void OnIntervalCB()
        {
            Debug.LogFormat("send interval......");
            NetMgr.Instance.SuperRequest(0, null);
        }

        private static void OnConnectCB()
        {
            Debug.LogFormat("connect successed!!!!"+ sm_LoginState);
            if(sm_LoginState == ELoginState.Route)
            {
                LoginRounte();//登路由
            }
            else if (sm_LoginState == ELoginState.Login)
            {
                LoginAccount(sm_name, sm_pwd);
            }
            else if(sm_LoginState == ELoginState.Gate)
            {
                LoginGate();//登陆gate
            }
            else if(sm_LoginState == ELoginState.Game)
            {
                LoginReConnect();//重连连接成功
            }
        }

        //重新连接
        private static void ReConnect()
        {
            if (sm_reConnectCBID != -1 || sm_LoginState != ELoginState.Game) return;
            if (sm_reconnectTimes >= sm_maxTimes)
            {
                if (sm_LoginState == ELoginState.Game)
                    Event.FWEvent.Instance.Call(EventID.NET_RECONN_FAIL, new EventArg());
                return;
            }
            Debug.LogFormat("ReConnect======");
        
            Event.FWEvent.Instance.Call(EventID.NET_RECONN_START, new EventArg());
            
            sm_reConnectCBID = Timer.Regist(RECONNECT_INTERVAL, RECONNECT_INTERVAL, 1, ()=> 
            {
                sm_reConnectCBID = -1;
                sm_reconnectTimes++;
                Debug.LogFormat("ReConnect connect");
                //连接
                NetDispatcherMgr.Inst.ChangeNetState(NetMgr.CONN_MSG.ClientBreak);
                NetMgr.Instance.ReConnect();
                //if (sm_LoginState == ELoginState.Game && sm_reconnectTimes == 1)
                //    Event.FWEvent.Instance.Call(EventID.NET_RECONN_START, new EventArg());
            });
        }

        // 请求链接服务器
        private static void Connect(string ip, int port)
        {
            CloseConnect();
            sm_maxTimes = MAX_CONNNECT_TIMES;
            sm_reconnectTimes = 0;
            sm_loginIP = ip;                            //当前登陆ip
            sm_loginPort = port;                        //当前登陆port
            Debug.LogFormat("Connect ......", sm_loginIP, sm_loginPort);
            NetMgr.Instance.Connect(sm_loginIP, sm_loginPort);
        }

        private static void CloseConnect()
        {
            NetMgr.Instance.Close();
        }

        //登陆路由
        private static void LoginRounte()
        {
            //暂将断线重连的放在这里
            sm_isReconnect = false;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;   
            NetMgr.Instance.SuperRequest(Commond.Login_Rounte, data);
            Debug.LogFormat("login rounte start.......");
        }

        //登陆路由返回
        private static void OnLoginRounte(DataObj data)
        {
            Debug.LogFormat("login rounte return......");
            if (data == null || data.GetUInt16("ret") != 0) return;

            string ip = data.GetString("ip");
            short port = data.GetInt16("port");
            Debug.LogFormat("OnLoginRounte successed!!!!"+ip +" "+port);
            //连接login
            Connect(ip, port);
            sm_LoginState++;
        }

        //用户登陆
        private static void LoginAccount(string name ,string pwd)
        {
            if(string.IsNullOrEmpty(name))
            {
                RequestGuest();//申请游客帐号
            }
            else
            {
                LoginServer(name, pwd); ;
            }
        }

        //帐号登陆
        private static void LoginServer(string name, string pwd)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["name"] = name;
            data["pwd"] = pwd;
            data["eTypeTerminal"] = 2;
            NetMgr.Instance.SuperRequest(Commond.Login_Account, data);
            Debug.LogFormat("login server start.......");
        }

        //帐号登陆返回
        private static void OnLoginServer(DataObj data)
        {
            Debug.LogFormat("login server return......");
            ushort ret = data.GetUInt16("ret");
            sm_account = data.GetInt32("account");
            sm_token = data.GetString("token");
            sm_serverID = data.GetInt32("serverID");
            string ip = data.GetString("ip");
            int port = data.GetInt32("port");
            sm_isTourist = data.GetInt8("isTourist") == 1;
            Debug.LogFormat("OnLoginServer successed!!!!" + ret + " " + sm_account + " " + sm_token + " " + sm_serverID + " " + ip + " " + port);
            if (ret == 0)
            {
                //连接login
                Connect(ip, port);
                sm_LoginState++;
            }
            else
            {
                Event.FWEvent.Instance.Call(EventID.LOGIN_INFO, new EventArg((int)ret));
            }
        }

        //登陆gate
        private static void LoginGate()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["account"] = sm_account;
            data["token"] = sm_token;
            data["serverID"] = sm_serverID;
            data["eTypeTerminal"] = 2;
            NetMgr.Instance.SuperRequest(Commond.Login_Gate, data);
            Debug.LogFormat("login gate start.......");
        }

        //登陆gate返回
        private static void OnLoginGate(DataObj data)
        {
            ushort ret = data.GetUInt16("ret");
            Debug.LogFormat("login gate return......"+ret);
            sm_token = data.GetString("token");
            LoginConfig.SetUserNameAndPwd(sm_name,sm_pwd);
            LoginConfig.SaveFile();
            int result = (ret == 0) ? 0 : 3;
            if(result == 0)
            {
                sm_LoginState = ELoginState.Game;
            }
            Event.FWEvent.Instance.Call(EventID.LOGIN_INFO, new EventArg(result));
        }

        //重登gate
        private static void LoginReConnect()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["account"] = sm_account;
            data["token"] = sm_token;
            data["eTypeTerminal"] = 2;
            NetMgr.Instance.SuperRequest(Commond.Login_reConnect, data);
            Debug.LogFormat("login gate restart.......");
        }

        //重登gate返回
        private static void OnLoginReConnect(DataObj data)
        {
            ushort ret = data.GetUInt16("ret");
            sm_token = data.GetString("token");

            if (ret == 0)
            {
                Debug.LogFormat("reLogin gate sucess......错误码" + ret);
                Timer.Cancel(sm_reConnectCBID);
                sm_reConnectCBID = 0;
                sm_isReconnect = true;
                Event.FWEvent.Instance.Call(EventID.NET_RECONN_SUCCESS, new EventArg(ret));
            }
            else
            {
                Debug.LogFormat("reLogin gate fail......错误码" + ret);
                Event.FWEvent.Instance.Call(EventID.NET_RECONN_FAIL, new EventArg(ret));
            }
                
        }

        //请求临时帐号
        public static void RequestGuest()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.SuperRequest(Commond.Create_Guest, data);
        }

        //创建游客帐号返回
        private static void OnCreateGuest(DataObj data)
        {
            if (data == null || data.GetUInt16("ret") != 0) return;
            sm_name = data.GetString("name");
            sm_pwd = data.GetString("pwd");
            LoginServer(sm_name, sm_pwd);
            Debug.LogFormat("OnCreateGuest successed!!!!");
        }

        //绑定帐号返回
        private static void OnBindAccount(DataObj data)
        {
            if (data == null) return;
            ushort ret = data.GetUInt16("ret");
            if(ret == 0)
            {
                sm_name = sm_bindName;                                  //绑定帐号
                sm_pwd = sm_bindPwd;                                    //绑定帐号密码
                LoginConfig.SetUserNameAndPwd(sm_name, sm_pwd);
                LoginConfig.SetUserAutoLogin(sm_isAutoLogin);
                LoginConfig.SaveFile();
            }
            sm_isTourist = ret != 0;
            Event.FWEvent.Instance.Call(EventID.Bind_Account, new EventArg((int)ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Login(string name, string pwd,string ip)
        {
            sm_name = name;             //帐号
            sm_pwd = pwd;               //密码
            //如果已连接到登陆服务器，直接帐号登陆，否则从头开始
            if (sm_LoginState == ELoginState.Login)
            {
                LoginAccount(sm_name, sm_pwd);
            }
            else
            {
                sm_LoginState = ELoginState.Route;
                Connect(ip, 8056);
            }
        }

        // 请求登陆
        public static void Login(string name, string pwd)
        {
            Login(name,pwd, "192.168.8.231");
        }

        //绑定帐号
        public static void BindAccount(string name, string pwd, string phone, bool autoLogin = false)
        {
            if (IsTourist == false) return;
            sm_bindName = name;                                 //绑定帐号
            sm_bindPwd = pwd;                                   //绑定帐号密码
            sm_bindPhone = phone;                               //绑定帐号手机号
            sm_isAutoLogin = autoLogin;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["account"] = sm_name;
            data["oldPwd"] = sm_pwd;
            data["newUser"] = sm_bindName;
            data["newPwd"] = sm_bindPwd;
            data["phoneCode"] = sm_bindPhone;
            NetMgr.Instance.Request(Commond.Bind_Account, data);
        }

        //恢复到登录状态
        public static void Recover()
        {
            sm_reconnectTimes = 0;
            sm_intervalID = -1;                     //心跳计时id
            sm_interval = 60.0f;                    //心跳间隔
            sm_reConnectCBID = -1;
            sm_LoginState = ELoginState.Route;
        }

    }
}