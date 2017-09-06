//******************************************************************
// File Name:					ExchangePrizeProctor
// Description:					ExchangePrizeProctor class 
// Author:						lanjian
// Date:						3/7/2017 2:36:31 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Exchange
{
    class ExchangePrizeProctor
    {

        private string m_NoticeContent;
        public ExchangePrizeProctor()
        {
            m_NoticeContent = "公告";
        }
        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------
        private void OnRequestNotice(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0) return;
            this.m_NoticeContent = data.GetString("content");
        }
        
        //--------------------------------------
        //private 
        //--------------------------------------v
        public void Init()
        {
            NetDispatcherMgr.Inst.Regist(Commond.Request_RealPrize_Notice_back, OnRequestNotice);
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_RealPrize_Notice, data);
        }

        public string GetNotice()
        {
            return this.m_NoticeContent;
        }

        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_RealPrize_Notice_back, OnRequestNotice);
        }
    }
}
