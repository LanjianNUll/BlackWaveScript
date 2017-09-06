//******************************************************************
// File Name:					StoreItemProctor.cs
// Description:					StoreItemProctor class 
// Author:						wuwei
// Date:						2017.02.23
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
using FW.ResMgr;
using FW.Item;


namespace FW.Store
{
    static class StoreItemProctor 
    {
        private static Dictionary<ItemType, ItemCreator> sm_Creators;
        private static Dictionary<string, ItemBase> sm_items;
        private static Dictionary<CommodityType, CurrencyType> sm_currencyTypes;

        static StoreItemProctor()
        {
            sm_Creators = new Dictionary<ItemType, ItemCreator>();
            sm_Creators.Add(ItemType.Weapon, WeaponBase.Create);
            sm_Creators.Add(ItemType.Accessory, AccessoryBase.Create);
            sm_Creators.Add(ItemType.Commodity, CommodityBase.Create);

            sm_currencyTypes = new Dictionary<CommodityType, CurrencyType>();
            sm_currencyTypes.Add(CommodityType.GoldCurrency, CurrencyType.Gold);
            sm_currencyTypes.Add(CommodityType.DiamondCurrency, CurrencyType.Diamond);
            sm_currencyTypes.Add(CommodityType.CashCurrency, CurrencyType.Cash);

            sm_items = new Dictionary<string, ItemBase>();
        }

        public static ItemBase Create(ItemType type, string id, int resID)
        {
            ItemBase item = null;
            if (sm_items.TryGetValue(id, out item))
            {
                return item;
            }
            ItemCreator creator;
            if (sm_Creators.TryGetValue(type, out creator) == false)
                return null;
            item = creator(type, id, resID);
            if (item == null) return item;
            sm_items.Add(id, item);
            return item;
        }

        //查询货币类型
        public static CurrencyType GetCurrencyType(CommodityType type)
        {
            CurrencyType currencyType = CurrencyType.Unknow;
            sm_currencyTypes.TryGetValue(type, out currencyType);
            return currencyType;
        }

        //移除
        public static bool Remove(ItemBase item)
        {
            if (item == null) return false;
            return Remove(item.ID);
        }

        //移除
        public static bool Remove(string id)
        {
            if (sm_items.ContainsKey(id))
            {
                sm_items.Remove(id);
                return true;
            }
            return false;
        }

        public static void Dispose()
        {

            sm_items.Clear();
        }
    }
}