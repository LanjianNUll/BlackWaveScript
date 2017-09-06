//******************************************************************
// File Name:					DealItemInfo
// Description:					DealItemInfo class 
// Author:						lanjian
// Date:						3/10/2017 10:56:24 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Item;
using FW.Store;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Deal
{
    //物品的售卖状态
    enum ItemState
    {
        ForSale = 0,                                //出售
        WaitForSale,                                //待售 
    }

    class DealItemInfo
    {
        private string m_dealKey;                      //交易的key值
        private string m_itemID;                       //物品的唯一id
        private int m_endTime;                         //物品售卖到期的时间
        private ItemType m_itemType;                   //物品类型
        private int m_tradeType;                       //交易商品类型
        private int m_resid;                           //物品资源id
        private int m_itemCount;                       //物品数量
        private int m_price;                           //物品价格
        private ItemState m_state;                     //物品状态 0  待售   1 出售
        private bool m_belongSelf;                     //是否自己的
        
        private ItemBase m_item;                        //物品类

        private int m_gold_cost;                        //黄金价格
        private int m_diamond_cost;                     //砖石价格

        //--------------------------------------
        //properties 
        //--------------------------------------
        public string DealKey { get { return this.m_dealKey; } }
        public string ItemId { get { return this.m_itemID; } }
        public int EndTime { get { return this.m_endTime; } }
        public ItemType Type {get{ return this.m_itemType; } }
        public int TradeType { get { return this.m_tradeType; }}
        public int ResId { get { return this.m_resid; } }
        public int ItemCount { get { return this.m_itemCount; } }
        public int Price { get { return this.m_price; } }
        public ItemState State { get { return this.m_state; } }
        public ItemBase Item { get { return this.m_item; } }
        public bool BelongSelf { get { return this.m_belongSelf; } }

       
        public DealItemInfo(DataObj data)
        {
            this.Init(data);
            NetDispatcherMgr.Inst.Regist(Commond.Trade_Buy_Good_back, OnBuyTradeItem);
            NetDispatcherMgr.Inst.Regist(Commond.Off_Shelve_Good_back, OnOffShelveGood);
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(DataObj data)
        {
            this.m_itemType = (ItemType)data.GetInt8("type");
            this.m_tradeType = data.GetInt8("trade_type");
            this.m_dealKey = data.GetString("key");
            this.m_itemID = data.GetString("uid");
            this.m_resid = data.GetInt32("stdid");
            this.m_itemCount = data.GetInt32("count");
            this.m_price = data.GetInt32("price");
            this.m_endTime = data.GetInt32("end_time");
            this.m_state = (ItemState)data.GetInt8("status");
            //判读这个是否是自己的物品
            this.m_belongSelf = data.GetInt32("pid") == Role.Role.Instance().ID;
            //根据属性创建一个物品
            this.CreateItem();
        }

        //创建物品
        private void CreateItem()
        {
            this.m_item = StoreItemProctor.Create(this.Type, this.ItemId.ToString(), this.ResId);
        }

        //购买返回
        private void OnBuyTradeItem(DataObj data)
        {
            //0  成功   1 找不到玩家  2 类型错误  3 找不到物品  4 已售卖  
            //5 交易不匹配   6发送邮件   7 创建邮件错误 8 钱不够
            int ret = data.GetUInt16("ret");
            string id = data.GetString("uni_id");
            int status = data.GetInt8("status");
            if (id != this.ItemId) return;
            Event.FWEvent.Instance.Call(Event.EventID.Deal_itemBought, new Event.EventArg(this, ret));
        }

        //下架返回
        private void OnOffShelveGood(DataObj data)
        {
            int ret = data.GetUInt16("ret");
            string id = data.GetString("uni_id");
            if (id != this.ItemId) return;
            Event.FWEvent.Instance.Call(Event.EventID.Deal_offShelveItem, new Event.EventArg(this, ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------

        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Trade_Buy_Good_back, OnBuyTradeItem);
            if (this.Item != null)
            {
                StoreItemProctor.Remove(this.Item);
            }
        }

        //设置黄金和砖石的价格
        public void SetGoldAndDiamondPrice(int goldCost, int diamondCost)
        {
            this.m_gold_cost = goldCost;
            this.m_diamond_cost = diamondCost;
        }

        //请求购买  要先设值砖石价格（转换把）
        public void Buy()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)this.Type;
            data["trade_type"] = (sbyte)this.m_tradeType;
            data["key"] = this.m_dealKey;
            data["uni_id"] = this.m_itemID;
            data["gold_cost"] = this.m_gold_cost;
            data["diamond_cost"] = this.m_diamond_cost;
            NetMgr.Instance.Request(Commond.Trade_Buy_Good,data);
        }

        //下架请求
        public void OffShelveGood()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)this.Type;
            data["trade_type"] = (sbyte)this.m_tradeType;
            data["key"] = this.m_dealKey;
            data["uni_id"] = this.m_itemID;
            NetMgr.Instance.Request(Commond.Off_Shelve_Good, data);

        }
    }
}
