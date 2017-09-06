//******************************************************************
// File Name:					ExchangePrizeItem
// Description:					ExchangePrizeItem class 
// Author:						lanjian
// Date:						3/7/2017 9:50:06 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Exchange
{
    class ExchangePrizeItem
    {
        private int m_id;                               //兑换物品ID
        private int m_RemainingCount;                   //库存
        private CurrencyType m_currencyType;            //货币类型
        private int m_price;                            //价格

        private string m_ExchangePrizeName;             //兑换物品的名称
        private ExchangePrizeType m_ExchangeType;       //兑换物品类型  1 实物 2 虚拟道具
        private string m_shortDesc;                     //兑换商品简介
        private string m_Icon;                          //图片
        private string m_DetailDesc;                    //兑换商品详细介绍

        public ExchangePrizeItem(int id)
        {
            GetDataFromJsonItem(id);
        }

        public ExchangePrizeItem(DataObj data)
        {
            this.Init(data);
            NetDispatcherMgr.Inst.Regist(Commond.Buy_RealPrize_back, OnBuy);
        }
        
        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return this.m_id; } }
        public int RemainingCount { get { return this.m_RemainingCount; } }
        public CurrencyType CurrentcyTp { get { return this.m_currencyType; } }
        public int Price { get { return this.m_price; } }
        public string ExchangePrizeName { get { return this.m_ExchangePrizeName;} }
        public ExchangePrizeType  ExPrType {get{ return this.m_ExchangeType; } }
        public string ShortDesc { get { return this.m_shortDesc; } }
        public string Icon { get { return this.m_Icon; } }
        public string DetailDesc { get { return this.m_DetailDesc; } }
        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(DataObj data)
        {
            if (data == null) return;
            this.m_id = data.GetInt32("id");                                            //id
            this.m_RemainingCount = data.GetInt32("remaining");                         //库存
            this.m_currencyType = (CurrencyType)data.GetInt32("currency_type");         //货币类型 
            this.m_price = data.GetInt32("price");                                      //价格
            GetDataFromJsonItem(this.m_id);
        }

        //其他属性从配置表中读出来
        private void GetDataFromJsonItem(int id)
        {
            //获取对应的配置表
            JsonItem item = DatasMgr.GetJsonItem(id);
            if (item == null)
                return;
            this.m_ExchangeType = (ExchangePrizeType)item.Get("shoptype").AsInt();
            this.m_ExchangePrizeName = item.Get("shopname").AsString();
            this.m_shortDesc = item.Get("desc").AsString();
            this.m_DetailDesc = item.Get("desc1").AsString();
            this.m_Icon = DatasMgr.GetRes(item.Get("shopicon").AsInt());
        }
        
        //兑换返回
        private void OnBuy(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            ExchangeItemOrder order = new ExchangeItemOrder(data.GetDataObj("record"));
            Event.FWEvent.Instance.Call(Event.EventID.ExchangePrize_itemBought, new Event.EventArg(order, (int)ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Buy_RealPrize_back, OnBuy);
        }

        public void ExchangePrize(int exchangeNum,Address addr)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["id"] = this.ID;
            data["count"] = exchangeNum;
            data["addr"] = addr.GetDataObj();
            NetMgr.Instance.Request(Commond.Buy_RealPrize, data);
        }
    }
}
