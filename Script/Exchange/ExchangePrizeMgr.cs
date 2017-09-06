//******************************************************************
// File Name:					ExchangePrizeMgr
// Description:					ExchangePrizeMgr class 
// Author:						lanjian
// Date:						3/7/2017 10:01:54 AM
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
    //兑换分类
    enum ExchangePrizeType
    {
        Unknow,                          //未知
        Real,                            //实物
        Virtual,                         //虚拟道具
    }

    static class ExchangePrizeMgr
    {
        //兑换商品列表
        private static Dictionary<int, ExchangePrizeItem> sm_exchangeItems;
        //兑换记录（订单）
        private static Dictionary<string, ExchangeItemOrder> sm_exchangeOrders;

        static ExchangePrizeMgr()
        {
            sm_exchangeItems = new Dictionary<int, ExchangePrizeItem>();
            sm_exchangeOrders = new Dictionary<string, ExchangeItemOrder>();

            NetDispatcherMgr.Inst.Regist(Commond.Load_Exchange_RealPrize_back, OnRequestExchangePrizeItems);
            NetDispatcherMgr.Inst.Regist(Commond.Load_RealPrize_Record_back, OnRequestExchangePrizeOrders);
        }

        //--------------------------------------
        //private 
        //--------------------------------------

        //删除兑换列表
        private static void RomoveExchangeItems()
        {
            foreach (ExchangePrizeItem item in sm_exchangeItems.Values)
            {
                item.Dispose();
            }
            sm_exchangeItems.Clear();
        }

        //删除订单列表
        private static void RemoveExchangeOrders()
        {
            foreach (ExchangeItemOrder item in sm_exchangeOrders.Values)
            {
                item.Dispose();
            }
            sm_exchangeOrders.Clear();
        }

        //请求兑换物品list返回
        private static void OnRequestExchangePrizeItems(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            RomoveExchangeItems();
            foreach (DataObj itemdata in data.GetDataObjList("prizes"))
            {
                CreateExchangeItems(itemdata);
            }
            Event.FWEvent.Instance.Call(Event.EventID.ExchnagePrizeItem_change, new Event.EventArg());
        }

        //创建兑换物品
        private static void CreateExchangeItems(DataObj data)
        {
            ExchangePrizeItem item = new ExchangePrizeItem(data);
            sm_exchangeItems.Add(item.ID, item);
        }
        //请求兑换订单list返回
        private static void OnRequestExchangePrizeOrders(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            RemoveExchangeOrders();
            foreach (DataObj itemdata in data.GetDataObjList("records"))
            {
                CreateExchangeOrders(itemdata);
            }
            Event.FWEvent.Instance.Call(Event.EventID.ExchnagePrizeOrder_change, new Event.EventArg());
        }

        //创建订单
        private static void CreateExchangeOrders(DataObj data)
        {
            ExchangeItemOrder item = new ExchangeItemOrder(data);
            sm_exchangeOrders.Add(item.OrderId, item);
        }

        //--------------------------------------
        //public 
        //--------------------------------------

        public static void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Load_Exchange_RealPrize_back, OnRequestExchangePrizeItems);
            NetDispatcherMgr.Inst.UnRegist(Commond.Load_RealPrize_Record_back, OnRequestExchangePrizeOrders);
            RomoveExchangeItems();
            RemoveExchangeOrders();
        }

        //获取兑换物品List 根据类型
        public static List<ExchangePrizeItem> GetExchangeItems(ExchangePrizeType type)
        {
            List<ExchangePrizeItem> items = new List<ExchangePrizeItem>();
            foreach (ExchangePrizeItem item in sm_exchangeItems.Values)
            {
                if (type == item.ExPrType)
                {
                    items.Add(item);
                    continue;
                }
            }
            items.Sort((x, y) => -x.ID.CompareTo(y.ID));
            return items;
        }

        //获取所有的兑换物品List
        public static List<ExchangePrizeItem> GetExchangeItems()
        {
            List<ExchangePrizeItem> items = new List<ExchangePrizeItem>();
            foreach (ExchangePrizeItem item in sm_exchangeItems.Values)
            {
                    items.Add(item);
            }
            items.Sort((x, y) => -x.ID.CompareTo(y.ID));
            return items;
        }

        //获取兑换物品
        public static ExchangePrizeItem GetExchangeItem(int id)
        {
            ExchangePrizeItem item = null;
            sm_exchangeItems.TryGetValue(id, out item);
            return item;
        }

        ///请求兑换物品
        public static void RequestGetExchangeItems()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Load_Exchange_RealPrize, data);
        }

        //获取兑换订单物品List
        public static List<ExchangeItemOrder> GetGetExchangeOrders()
        {
            List<ExchangeItemOrder> items = new List<ExchangeItemOrder>();
            foreach (ExchangeItemOrder item in sm_exchangeOrders.Values)
            {
                items.Add(item);
            }
            return items;
        }

        //获取订单
        public static ExchangeItemOrder GetGetExchangeOrder(string OrderId)
        {
            ExchangeItemOrder item = null;
            sm_exchangeOrders.TryGetValue(OrderId, out item);
            return item;
        }

        ///请求兑换物品
        public static void RequestGetExchangeOrders()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Load_RealPrize_Record, data);
        }
    }
}
