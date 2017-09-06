//******************************************************************
// File Name:					DealItemMgr
// Description:					DealItemMgr class 
// Author:						lanjian
// Date:						3/10/2017 3:31:24 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Item;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Deal
{
    class DealItemMgr
    {
        private static List<DealItemInfo> sm_items;
        private static List<DealItemInfo> sm_mySelfItems;
        private static DealFitterItem sm_currentDealFitter;

        private static Int64 sm_timerId = -1;
        private static Int64 sm_timerId2 = -1;

        static DealItemMgr()
        {
            sm_items = new List<DealItemInfo>();
            sm_mySelfItems = new List<DealItemInfo>();
            NetDispatcherMgr.Inst.Regist(Commond.Request_Trade_Info_back, OnRequestDealItem);
            NetDispatcherMgr.Inst.Regist(Commond.Request_My_Trade_Info_back, OnRequestMySelfDealItem);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------
        //获取最短的那个倒计时
        private static void GetMinEndTime()
        {
            if (sm_items.Count > 0)
            {
                Int64 min = sm_items[0].EndTime;
                foreach (DealItemInfo item in sm_items)
                {
                    if (item.EndTime < min)
                        min = item.EndTime;
                }
                if (min < 0)
                {
                    RomoveItems(sm_items);
                    return;//防止出错
                }
                sm_timerId = Timer.Regist(min, 0, 1, () =>
                {
                    //请求一次交易列表
                    RequestTradeList(sm_currentDealFitter);
                });
            }
        }

        //请求交易物品列表返回
        private static void OnRequestDealItem(DataObj data)
        {
            if (data == null) return;
            if (data.GetUInt16("ret") != 0) return;
            RomoveItems(sm_items);
            foreach (DataObj itemdata in data.GetDataObjList("trade_info"))
            {
                CreateItems(itemdata, sm_items);
            }
            foreach (DataObj itemdata in data.GetDataObjList("pre_trade_info"))
            {
                CreateItems(itemdata, sm_items);
            }

            if (sm_timerId != -1)
                Timer.Cancel(sm_timerId);
            GetMinEndTime();
            Event.FWEvent.Instance.Call(Event.EventID.Deal_itemChanged, new Event.EventArg());
        }

        //获取最短的那个倒计时
        private static void GetMinEndTime2()
        {
            if (sm_mySelfItems.Count > 0)
            {
                Int64 min = sm_mySelfItems[0].EndTime;
                foreach (DealItemInfo item in sm_mySelfItems)
                {
                    if (item.EndTime < min)
                        min = item.EndTime;
                }
                if (min < 0)
                {
                    RomoveItems(sm_mySelfItems);
                    return;//防止出错
                }
                sm_timerId2 = Timer.Regist(min, 0, 1, () =>
                {
                    //请求一次自己的列表
                    RequestMySelfTradeList();
                });
            }
        }

        //请求我的交易列表返回
        private static void OnRequestMySelfDealItem(DataObj data)
        {
            if (data == null)
                return;
            if (data.GetUInt16("ret") != 0) return;
            RomoveItems(sm_mySelfItems);
            foreach (DataObj itemdata in data.GetDataObjList("trade_info"))
            {
                CreateItems(itemdata, sm_mySelfItems);
            }
            if (sm_timerId2 != -1)
                Timer.Cancel(sm_timerId2);
            GetMinEndTime2();
            Event.FWEvent.Instance.Call(Event.EventID.Deal_mySelfItemChanged, new Event.EventArg());
        }

        //创建一个DealItemInfo
        private static void CreateItems(DataObj data, List<DealItemInfo> items)
        {
            if (data == null || data.Count == 0) return;
            DealItemInfo item = new DealItemInfo(data);
            items.Add(item);
        }

        //清空列表
        private static void RomoveItems(List<DealItemInfo> items)
        {
            foreach (DealItemInfo item in items)
            {
                item.Dispose();
            }
            items.Clear();
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //销毁
        public static void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Store_back, OnRequestDealItem);

            RomoveItems(sm_items);
            RomoveItems(sm_mySelfItems);
        }

        //请求交易列表 是否刷新1刷新 0请求下一页*/
        public static void RequestTradeList(DealFitterItem item, int is_refresh = 1)
        {
            sm_currentDealFitter = item;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)item.IType;
            data["trade_type"] = (sbyte)item.DealTradeType;
            data["is_refresh"] = (sbyte)is_refresh;
            NetMgr.Instance.Request(Commond.Request_Trade_Info, data);
        }

        //请求我的交易列表
        public static void RequestMySelfTradeList()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_My_Trade_Info, data);
        }

        //获取交易列表(还需要分类获取)
        public static List<DealItemInfo> GetTradeList(ItemState itemState)
        {
            List<DealItemInfo> list = new List<DealItemInfo>();
            foreach (DealItemInfo item in sm_items)
            {
                if (item.State == itemState)
                    list.Add(item);
            }
            return list;
        }

        //获取我的交易列表
        public static List<DealItemInfo> GetMyTradeList()
        {
            List<DealItemInfo> list = new List<DealItemInfo>();
            foreach (DealItemInfo item in sm_mySelfItems)
            {
                list.Add(item);
            }
            return list;
        }

        //退出我的寄售是将时间重置
        public static void ExitMySold()
        {
            sm_timerId2 = -1;
        }

        public static void ExitForSold()
        {
            sm_timerId = -1;
        }
    }
}
