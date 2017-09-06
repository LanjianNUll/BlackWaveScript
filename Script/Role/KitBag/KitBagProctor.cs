//******************************************************************
// File Name:					KitBagProctor.cs
// Description:					KitBagProctor class 
// Author:						wuwei
// Date:						2016.12.27
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

namespace FW.Role
{
    class KitBagProctor
    {
        private Role m_role;
        private Dictionary<ItemType, List<string>> m_items;

        public KitBagProctor(Role role)
        {
            this.m_role = role;
            this.m_items = new Dictionary<ItemType, List<string>>();
            this.m_items.Add(ItemType.Weapon, new List<string>());
            this.m_items.Add(ItemType.Accessory, new List<string>());
            this.m_items.Add(ItemType.Commodity, new List<string>());
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public Role Owner { get { return this.m_role; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        //请求背包
        private void RequestKitBag()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_KitBag, data);
        }

        //响应请求背包
        private void OnRequestKitBag(DataObj data)
        {
            if (data == null) return;
            //读出武器
            List<DataObj> weaponDatas = data.GetDataObjList("weapondatas");
            this.ChangeWeaponItem(weaponDatas);
            //读出配件
            List<DataObj> partDatas = data.GetDataObjList("partdatas");
            this.ChangePartItem(partDatas);
            //读出道具
            List<DataObj> commdityDatas = data.GetDataObjList("itemdatas");
            this.ChangeCommdityItem(commdityDatas);
        }

        //响应已装备武器
        private void OnEquipedItem(DataObj data)
        {
            if (data == null) return;
            List<DataObj> datas = data.GetDataObjList("items");
            Role.Instance().EquipPcWeapon(datas);
        }

        //组装武器
        private void AssembleWeapon(WeaponBase weapon, List<DataObj> datas)
        {
            if (weapon == null) return;
            //装配到武器上去
            weapon.InitAssemble(datas);
        }

        //添加一件物品
        private ItemBase CreateItem(ItemType type, string id, int resID,int count =1)
        {
            ItemBase  itembase = ItemMgr.Create(type, id, resID);
            //添加物品数量
            itembase.Count = count;
            return itembase;
        }

        //删除一件
        private bool DestroyItem(ItemType type, string id, int resID)
        {
            if(ItemMgr.Remove(id))
            {
                this.RemoveItem(type, id);
                return true;
            }
            return false;
        }

        //item改变
        private ItemBase ChangeItem(DataObj data)
        {
            if (data == null) return null;
            ItemType type = (ItemType)data.GetInt8("type");
            string id = data.GetString("id");
            int resId = data.GetInt32("stdid");
            int count = data.GetInt32("count");
            bool isBind = data.GetInt8("bind") == 1; //1绑定 0不绑定
            if (resId < 0)
            {
                this.DestroyItem(type, id, resId);
                return null;
            }
            else
            {
                ItemBase item = this.CreateItem(type, id, resId,count);
                item.SetIndex(data.GetInt32("index"));
                item.IsBind = isBind;
                return item;
            }
        }

        //武器列表修改
        private void ChangeWeaponItem(List<DataObj> datas)
        {
            if (datas == null) return;
            foreach(DataObj data in datas)
            {
                DataObj itemData = data.GetDataObj("info");
                //配件
                WeaponBase weapon = this.ChangeItem(itemData) as WeaponBase;
                if (weapon == null) continue;
                List<DataObj> parts = data.GetDataObjList("parts");
                this.AssembleWeapon(weapon, parts);
                //添加武器到列表中
                this.AddItem(ItemType.Weapon,weapon.ID);
            }
            if (datas.Count > 0)
                Event.FWEvent.Instance.Call(Event.EventID.WeaponList_changed,new Event.EventArg());
        }

        //添加物品
        private void AddItem(ItemType type, string id)
        {
            List<string> ids;
            if(this.m_items.TryGetValue(type, out ids))
            {
                if (ids.Contains(id) == false)
                {
                    ids.Add(id);
                }
            }
        }

        //删除物品
        private void RemoveItem(ItemType type, string id)
        {
            List<string> ids;
            if (this.m_items.TryGetValue(type, out ids))
            {
                if (ids.Contains(id))
                {
                    ids.Remove(id);
                }
            }
        }

        //配件列表修改
        private void ChangePartItem(List<DataObj> datas)
        {
            if (datas == null) return;
            foreach (DataObj data in datas)
            {
                AccessoryBase accessory = this.ChangeItem(data) as AccessoryBase;
                if (accessory == null) continue;
                //添加配件到列表中
                this.AddItem(ItemType.Accessory, accessory.ID);
            }
            if (datas.Count > 0)
                Event.FWEvent.Instance.Call(Event.EventID.AccessoryList_changed, new Event.EventArg());
        }

        //道具列表修改
        private void ChangeCommdityItem(List<DataObj> datas)
        {
            if (datas == null) return;
            foreach (DataObj data in datas)
            {
                CommodityBase commdity = this.ChangeItem(data) as CommodityBase;
                if (commdity == null) continue;
                //添加道具到列表中
                this.AddItem(ItemType.Commodity, commdity.ID);
            }
            if (datas.Count > 0)
                Event.FWEvent.Instance.Call(Event.EventID.CommdityList_changed, new Event.EventArg());
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            NetDispatcherMgr.Inst.Regist(Commond.Request_KitBag_back, OnRequestKitBag);
            NetDispatcherMgr.Inst.Regist(Commond.Request_equiped_back, OnEquipedItem);

            this.RequestKitBag();
        }

        //销毁
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_KitBag_back, OnRequestKitBag);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_equiped_back, OnEquipedItem);

            foreach(List<string> items in this.m_items.Values)
            {
                items.Clear();
            }
        }

        //获取武器列表
        public List<WeaponBase> GetWeapons(WeaponType type)
        {
            List<WeaponBase> ids = new List<WeaponBase>();
            List<string> total;
            if (this.m_items.TryGetValue(ItemType.Weapon, out total) == false)
                return ids;
            foreach(string id in total)
            {
                WeaponBase item = ItemMgr.GetItem(id) as WeaponBase;
                if (item != null && item.WeaponType == type)
                    ids.Add(item);
            }
            return ids;
        }

        //获取配件列表
        public List<AccessoryBase> GetAccessory(AccessoryType type)
        {
            List<AccessoryBase> ids = new List<AccessoryBase>();
            List<string> total;
            if (this.m_items.TryGetValue(ItemType.Accessory, out total) == false)
                return ids;
            foreach (string id in total)
            {
                AccessoryBase item = ItemMgr.GetItem(id) as AccessoryBase;
                if (item != null && item.Type == type)
                    ids.Add(item);
            }
            return ids;
        }

        //获取武器适合的配件
        public List<AccessoryBase> GetWeaponAceesory(WeaponBase weapon, AccessoryType type)
        {
            List<AccessoryBase> ids = new List<AccessoryBase>();
            if (weapon == null) return ids;
            List<string> total;
            if (this.m_items.TryGetValue(ItemType.Accessory, out total) == false)
                return ids;
            foreach (string id in total)
            {
                AccessoryBase item = ItemMgr.GetItem(id) as AccessoryBase;
                if (item != null && item.Type == type && weapon.CheckAccessoryPort(item))
                    ids.Add(item);
            }
            return ids;
        }

        //根据道具类型获取道具列表 
        public List<CommodityBase> GetCommditys(CommodityType type)
        {
            List<CommodityBase> all = new List<CommodityBase>();
            List<CommodityBase> ids = GetAllCommditys();
            foreach (CommodityBase item in ids)
            {
                if (item != null && item.Type == type)
                    all.Add(item);
            }
            return all;
        }

        //获取所有的道具列表
        public List<CommodityBase> GetAllCommditys()
        {
            List<CommodityBase> ids = new List<CommodityBase>();
            List<string> total;
            if (this.m_items.TryGetValue(ItemType.Commodity, out total) == false)
                return ids;
            foreach (string id in total)
            {
                CommodityBase item = ItemMgr.GetItem(id) as CommodityBase;
                if (item != null)
                    ids.Add(item);
            }
            return ids;
        }

        //根据主类型获取消耗道具
        public List<CommodityBase> GetAllConsumeCommditys()
        {
            List<CommodityBase> all = new List<CommodityBase>();
            List<CommodityBase> ids = GetAllCommditys();
            foreach (CommodityBase item in ids)
            {
                if (item != null && item.MainUseType == CommodityUseType.Consumption)
                    all.Add(item);
            }
            return all;
        }

        //根据主类型获取非消耗道具
        public List<CommodityBase> GetAllNotConsumeCommditys()
        {
            List<CommodityBase> all = new List<CommodityBase>();
            List<CommodityBase> ids = GetAllCommditys();
            foreach (CommodityBase item in ids)
            {
                if (item != null && item.MainUseType != CommodityUseType.Consumption && item.MainUseType != CommodityUseType.Unknow)
                    all.Add(item);
            }
            return all;
        }

        public List<CommodityBase> GetTradeCommditys(CommodityTradeType type)
        {
            List<CommodityBase> all = new List<CommodityBase>();
            List<CommodityBase> ids = GetAllCommditys();
            foreach (CommodityBase item in ids)
            {
                if (item != null && item.MainTradeType == type)
                    all.Add(item);
            }
            return all;
        }

    }
}