//******************************************************************
// File Name:					StoreItem.cs
// Description:					StoreItem class 
// Author:						wuwei
// Date:						2017.02.17
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
using FW.Item;
using FW.ResMgr;

namespace FW.Store
{
    class StoreItem
    {
        private int m_id;                               //id
        private int m_resID;                            //资原id
        private StoreItemType m_type;                   //商品类型
        private ItemType m_itemType;                    //物品类型
        private bool m_isBought;                        //是否已购
        private CurrencyType m_currencyType;            //货币类型
        private int m_price;                            //价格
        private int m_count;                            //数量
        private int m_level;                            //限购等级
        private int m_sellStartTime;                    //限购开始时间
        private int m_sellEndTime;                      //限购终止时间

        private ItemBase m_item;                        //物品类

        public StoreItem(StoreItemType type, DataObj data)
        {
            this.m_type = type;
            this.Init(data);

            NetDispatcherMgr.Inst.Regist(Commond.Request_Store_buy_back, OnBuy);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return this.m_id; } }                                     //id
        public StoreItemType Type { get { return this.m_type; } }                       //商品类型
        public ItemType ItemType { get { return this.m_itemType; } }                    //物品类型
        public bool IsBought { get { return this.m_isBought; } }                        //是否已购
        public CurrencyType CurrencyType { get { return this.m_currencyType; } }        //货币类型
        public int Price { get { return this.m_price; } }                               //价格
        public int Count { get { return this.m_count; } }                               //数量
        public int Level { get { return this.m_level; } }                               //限购等级
        public int SellStartTime { get { return this.m_sellStartTime; } }               //限购开始时间
        public int SellEndTime { get { return this.m_sellEndTime; } }                   //限购终止时间

        public ItemBase Item { get { return m_item; } }                                 //物品类
        
        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(DataObj data)
        {
            if (data == null) return;
            this.m_id = data.GetInt32("id");                                            //id
            this.m_resID = data.GetInt32("item_id");                                    //资原id
            this.m_itemType = (ItemType)data.GetInt8("type");                           //物品类型
            this.m_isBought = data.GetInt8("buy_tag") == 1;                             //是否已购
            int currencyID = data.GetInt32("currency_type");
            int currencyType = ResMgr.DatasMgr.GetJsonItem(currencyID).Get("type").AsInt();
            this.m_currencyType = StoreItemProctor.GetCurrencyType((CommodityType)currencyType);          //货币类型
            this.m_price = data.GetInt32("price");                                      //价格
            this.m_count = data.GetInt32("count");                                      //数量
            this.m_level = data.GetInt32("level");                                      //限购等级
            this.m_sellStartTime = data.GetInt32("time_res_begin");                     //限购开始时间
            this.m_sellEndTime = data.GetInt32("time_res_end");                         //限购终止时间
            //根据商品属性创建一个物品
            this.CreateItem();
        }

        //创建物品
        private void CreateItem()
        {
            this.m_item = StoreItemProctor.Create(this.ItemType, this.ID.ToString(), this.m_resID);
        }

        //购买返回
        private void OnBuy(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            int id = data.GetInt32("shopid");
            if (id != this.ID) return;
            this.m_isBought = ret == 0 ? true : false;
            Event.FWEvent.Instance.Call(Event.EventID.Shop_itemBought, new Event.EventArg(this, ret == 0));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Store_buy_back, OnBuy);
            if(this.Item != null)
            {
                StoreItemProctor.Remove(this.Item);
            }
        }

        //购买
        public void Buy()
        {
            if (this.IsBought) return;

            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)this.Type;
            data["shopid"] = this.ID;
            NetMgr.Instance.Request(Commond.Request_Store_buy, data);
        }
    }
}