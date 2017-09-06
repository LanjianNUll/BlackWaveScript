//******************************************************************
// File Name:					StoreMgr.cs
// Description:					StoreMgr class 
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
    //商品分类
    enum StoreItemType
    {
        Unknow,                         //未知
        Munition,                       //军火
        Supply,                         //补给品
        PayItem,                        //充值项
    }

    static class StoreMgr
    {
        private static Dictionary<int, StoreItem> sm_items;
        private static int sm_freshDelayTime;                       //倒计时
        private static int sm_costfreshShop;
        private static CurrencyType sm_currentType = CurrencyType.Cash;
        private static List<PayItem> sm_payItemList;

        static StoreMgr()
        {
            sm_items = new Dictionary<int, StoreItem>();
            NetDispatcherMgr.Inst.Regist(Commond.Request_Store_back, OnRequestItems);
            sm_payItemList = new List<PayItem>();
            LoadPayItemData();
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        public static int FreshDelayTime { get { return sm_freshDelayTime; } }
        public static int CostFreshShop { get { return sm_costfreshShop; } }
        public static CurrencyType CurrentType { get { return sm_currentType; } }

        private static long m_timer = -1;
        //--------------------------------------
        //private 
        //--------------------------------------
        private static void LoadPayItemData()
        {
            sm_payItemList.Clear();
            JsonConfig jsonConfig = DatasMgr.FWRecharge;
            for (int i = 0; i < jsonConfig.Data.Count; i++)
            {
                JsonItem jsonItem = jsonConfig.GetJsonItem((i+1).ToString());
                sm_payItemList.Add(new PayItem((i + 1).ToString(), jsonItem));
            }
        }


        //物品刷新返回
        private static void OnRequestItems(DataObj data)
        {
            if (data == null) return;
            ChangeDelayTime(data.GetInt32("refresh_time"));
            int ret = data.GetUInt16("ret");
            if (ret == 0)
                Debug.Log("刷新了商城");
            else if (ret == 1)
            {
                Debug.Log("刷新时间未到，失败码" + ret);
            }
            else
            { 
                Debug.Log("刷新了商城失败，失败码"+ ret);
                Event.FWEvent.Instance.Call(Event.EventID.Shop_changed, new Event.EventArg(ret));
                return;
            }
            RomoveItems();
            foreach (DataObj itemdata in data.GetDataObjList("shopinfos"))
            {
                StoreItemType type = (StoreItemType)itemdata.GetInt8("shop_type");
                CreateItems(type, itemdata.GetDataObjList("items"));
            }
            //手动刷新价格类型
            int moneyType = data.GetInt32("refresh_cost_type");
            if (moneyType == 206000001)
                sm_currentType = CurrencyType.Gold;
            if (moneyType == 206000002)
                sm_currentType = CurrencyType.Diamond;
            if (moneyType == 206000003)
                sm_currentType = CurrencyType.Cash;
            //手动刷新价格
            sm_costfreshShop = data.GetInt32("refresh_cost");
            Event.FWEvent.Instance.Call(Event.EventID.Shop_changed, new Event.EventArg(ret, sm_currentType, sm_costfreshShop));
        }

        //修改刷新时间 且定时刷新商城
        private static void ChangeDelayTime(int time)
        {
            sm_freshDelayTime = time;
            if (m_timer != -1)
            {
                Timer.Cancel(m_timer);
            }
            m_timer = Timer.Regist(0,1, ChangerCountZero);
        }

        //一秒触发一次 倒计时
        private static void ChangerCountZero()
        {
            sm_freshDelayTime--;
            if (sm_freshDelayTime <= 0)
            {
                RequestItemsByAuto();
                return;
            }
            Event.FWEvent.Instance.Call(Event.EventID.Shop_black_CountZero, new Event.EventArg(sm_freshDelayTime));
        }

        //创建物品
        private static void CreateItems(StoreItemType type, List<DataObj> datas)
        {
            if (datas == null || datas.Count == 0) return;
            foreach(DataObj data in datas)
            {
                StoreItem item = new StoreItem(type, data);
                sm_items.Add(item.ID, item);
            }
        }

        //删除所有商品
        private static void RomoveItems()
        {
            foreach(StoreItem item in sm_items.Values)
            {
                item.Dispose();
            }
            sm_items.Clear();
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //初始化
        public static void Init()
        {
        }

        //销毁
        public static void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Store_back, OnRequestItems);
            Timer.Cancel(m_timer);
            RomoveItems();
        }

        //获取商品
        public static List<StoreItem> GetItem(StoreItemType type)
        {
            List<StoreItem> items = new List<StoreItem>();
            foreach (StoreItem item in sm_items.Values)
            {
                if (item.Type != type) continue;
                items.Add(item);
            }
            items.Sort((x, y) => -x.ID.CompareTo(y.ID));
            return items;
        }

        //获取商品
        public static StoreItem GetItem(int id)
        {
            StoreItem item = null;
            sm_items.TryGetValue(id, out item);
            return item;
        }

        //请求商品
        public static void RequestItems(int tag =2)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["tag"] = (sbyte)tag;
            NetMgr.Instance.Request(Commond.Request_Store, data);
        }

        //自动刷新商品列表
        public static void RequestItemsByAuto()
        {
            RequestItems(1);
        }

        //请求充值档位数据
        public static List<PayItem> GetPayItemList()
        {
            return sm_payItemList;
        }
    }
} 