//******************************************************************
// File Name:					ItemMgr.cs
// Description:					ItemMgr class 
// Author:						wuwei
// Date:						2017.01.18
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

namespace FW.Item
{
    delegate ItemBase ItemCreator(ItemType type, string id, int resID);
    static class ItemMgr
    {
        private static Dictionary<ItemType, ItemCreator> sm_Creators;
        private static Dictionary<string, ItemBase> sm_items;

        static ItemMgr()
        {
            sm_Creators = new Dictionary<ItemType, ItemCreator>();
            sm_Creators.Add(ItemType.Weapon, WeaponBase.Create);
            sm_Creators.Add(ItemType.Accessory, AccessoryBase.Create);
            sm_Creators.Add(ItemType.Commodity, CommodityBase.Create);

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

        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Dispose()
        {
            foreach(ItemBase item in sm_items.Values)
            {
                item.Dispose();
            }
            sm_items.Clear();
        }

        public static ItemBase GetItem(string id)
        {
            ItemBase item = null;
            sm_items.TryGetValue(id, out item);
            return item;
        }

        //移除
        public static bool Remove(string id)
        {
            if(sm_items.ContainsKey(id))
            {
                sm_items.Remove(id);
                return true;
            }
            return false;
        }
    }
}