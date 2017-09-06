//******************************************************************
// File Name:					PayMgr
// Description:					PayMgr class 
// Author:						lanjian
// Date:						5/2/2017 4:16:28 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Store;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Pay
{
    //支付管理
    class PayMgr
    {
        private static int sm_rechargeId;  
        private static int sm_channel; ///* 充值渠道, 11.PC端微信， 12:移动端微信  21.PC端支付宝,  22.移动端支付宝*/
        static PayMgr()
        {
            NetDispatcherMgr.Inst.Regist(Commond.MCC_Request_pay_back, OnRequestPayBack);
            NetDispatcherMgr.Inst.Regist(Commond.Pay_WX_Mobile_back, OnPayWXBack);
            NetDispatcherMgr.Inst.Regist(Commond.Pay_ALi_Mobile_back, OnPayAliPayBack);
            NetDispatcherMgr.Inst.Regist(Commond.Pay_WX_Mobile_Arrived_back, OnArraiveeBack);
        }
        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------
        //请求充值返回
        private static void OnRequestPayBack(DataObj data)
        {
            if (data == null) return;
            int ret = data.GetUInt16("ret");
            /* 0 请求成功, 1. 充值id错误, 2.渠道id错误*/
            Debug.Log("请求充值返回码" + ret);
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.MCC_Pay_Back_Info);
        }
        
        //微信充值返回
        private static void OnPayWXBack(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            int rechargeId = data.GetInt32("rechargeId");
            string appId = data.GetString("appid");
            string partnerId = data.GetString("partnerid");
            string prepayId = data.GetString("prepayid");
            string nonceStr = data.GetString("noncestr");
            string timeStamp = data.GetString("timestamp");
            string sign = data.GetString("sign");
            Debug.Log("微信充值参数："+ rechargeId+"-"+ appId+"-"+ partnerId+"-"
                + prepayId+"-"+ nonceStr+"-"+ timeStamp+"-"+ sign);
            GoWXCharge(rechargeId,appId,partnerId,prepayId,nonceStr,timeStamp, sign);
        }

        //支付宝支付返回
        private static void OnPayAliPayBack(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            int rechargeId = data.GetInt32("rechargeId");
            string info = data.GetString("info");
            GoAliPayCharge(info);
            Debug.Log("支付宝orderinfo"+info);
        }

        //到账返回
        private static void OnArraiveeBack(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            int rechargeId = data.GetInt32("rechargeId");
            int channel = data.GetInt8("channel");
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.MCC_Pay_Arraival);
        }

        //请求充值
        private static void PayRequst()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["rechargeId"] = sm_rechargeId;
            data["channel"] = (sbyte)sm_channel;
            NetMgr.Instance.Request(Commond.MCC_Request_pay, data);
        }

        private static void GoWXCharge(int rechargeId,string appId,string partnerId,
            string prepayId,string nonceStr,string timeStamp, string sign)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                //调用安卓的支付
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("wchatPay", appId, partnerId, prepayId, nonceStr, sign, timeStamp);
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

            }
        }

        private static void GoAliPayCharge(string orderinfo)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                //调用安卓的Toast
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("aliPay", orderinfo);
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

            }
        }
        
        //--------------------------------------
        //public 
        //--------------------------------------
        //销毁
        public static void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.MCC_Request_pay_back, OnRequestPayBack);
            NetDispatcherMgr.Inst.UnRegist(Commond.Pay_WX_Mobile_back, OnPayWXBack);
            NetDispatcherMgr.Inst.UnRegist(Commond.Pay_ALi_Mobile_back, OnPayAliPayBack);
            NetDispatcherMgr.Inst.UnRegist(Commond.Pay_WX_Mobile_Arrived_back, OnArraiveeBack);
        }

        //微信支付
        public static void WChatPay(PayItem payItem)
        {
            sm_rechargeId = int.Parse(payItem.ID);
            sm_channel = 1; /* 充值渠道, 1微信，2.支付宝*/
            PayRequst();
        }

        //支付宝支付
        public static void AliPay(PayItem payItem)
        {
            sm_rechargeId = int.Parse(payItem.ID);
            sm_channel = 2; /* 充值渠道, 1微信，2.支付宝*/
            PayRequst();
        }
        
//        // test ios invoke
//        private static extern int IOSPay(string message, string title);

//        public static void TestPay()
//        {
//#if UNITY_IPHONE
//		Debug.Log(IOSPay("Hello", "World"));
//#endif
//        }
    }
}
