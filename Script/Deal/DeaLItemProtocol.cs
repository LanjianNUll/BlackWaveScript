//******************************************************************
// File Name:					DeaLItemProtocol
// Description:					DeaLItemProtocol class 
// Author:						lanjian
// Date:						3/13/2017 11:44:27 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Deal
{
    static class DeaLItemProtocol
    {
        public static List<DealFitterItem> sm_weaponSort = new List<DealFitterItem>();
        public static List<DealFitterItem> sm_partSort = new List<DealFitterItem>();
        public static List<DealFitterItem> sm_ItemSort = new List<DealFitterItem>();

        static DeaLItemProtocol()
        {
            ShowAllFitter();
        }

        //重配置表中拿到所有的
        private static void ShowAllFitter()
        {
            JsonConfig jsonConfig =  DatasMgr.WTradeTypeCfg;
            for (int i = 0; i < jsonConfig.Data.Count; i++)
            {
                JsonItem jsonItem = jsonConfig.GetJsonItem(i.ToString());
                sm_weaponSort.Add(new DealFitterItem(i, jsonItem,Item.ItemType.Weapon));
            }

            JsonConfig jsonConfig1 = DatasMgr.PTradeTypeCfg;
            for (int i = 0; i < jsonConfig1.Data.Count; i++)
            {
                JsonItem jsonItem = jsonConfig1.GetJsonItem(i.ToString());
                sm_partSort.Add(new DealFitterItem(i, jsonItem, Item.ItemType.Accessory));
            }

            //道具这张表有点奇怪
            JsonConfig jsonConfig2 = DatasMgr.ITradeTypeCfg;
            sm_ItemSort.Add(new DealFitterItem(1, jsonConfig2.GetJsonItem("1"), Item.ItemType.Commodity));
            sm_ItemSort.Add(new DealFitterItem(3, jsonConfig2.GetJsonItem("3"), Item.ItemType.Commodity));
            sm_ItemSort.Add(new DealFitterItem(10, jsonConfig2.GetJsonItem("10"), Item.ItemType.Commodity));
            sm_ItemSort.Add(new DealFitterItem(11, jsonConfig2.GetJsonItem("11"), Item.ItemType.Commodity));
        }

        //根据类型获取武器分类
        public static List<DealFitterItem> GetFitterWeaponItemList(int type)
        {
            List<DealFitterItem> list = new List<DealFitterItem>();
            for (int i = 0; i < sm_weaponSort.Count; i++)
            {
                if (sm_weaponSort[i].SubType == type)
                    list.Add(sm_weaponSort[i]);
            }
            return list;
        }

        //根据类型获取配件分类
        public static List<DealFitterItem> GetFitterPartItemList(int type)
        {
            List<DealFitterItem> list = new List<DealFitterItem>();
            for (int i = 0; i < sm_partSort.Count; i++)
            {
                if (sm_partSort[i].SubType == type)
                    list.Add(sm_partSort[i]);
            }
            return list;
        }

        //根据类型获取道具分类
        public static List<DealFitterItem> GetFitterCommdityItemList(int type)
        {
            List<DealFitterItem> list = new List<DealFitterItem>();
            for (int i = 0; i < sm_ItemSort.Count; i++)
            {
                if (sm_ItemSort[i].SubType == type)
                    list.Add(sm_ItemSort[i]);
            }
            return list;
        }


        public static void Dispose()
        {
            sm_weaponSort.Clear();
            sm_partSort.Clear();
            sm_ItemSort.Clear();
        }
    }
}
