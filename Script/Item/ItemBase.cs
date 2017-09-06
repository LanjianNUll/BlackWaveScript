//******************************************************************
// File Name:					ItemBase.cs
// Description:					ItemBase class 
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
using FW.ResMgr;

namespace FW.Item
{
    class ItemBase
    {
        private string m_id;
        private int m_index;
        private JsonItem m_item;
        private ItemType m_itemType;
        private int m_subType;
        private bool m_isBind;

        public ItemBase(ItemType type, string id, JsonItem item)
        {
            this.m_id = id;
            this.m_item = item;
            this.m_itemType = type;
            //上架
            NetDispatcherMgr.Inst.Regist(Commond.Shelve_Good_back, OnShelve);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public string ID { get { return this.m_id; } }
        //占据背包格式位置索引
        public int Index { get { return this.m_index; } }
        public virtual string Name { get { return null;} }
        public virtual int TradeType { get { return -1; } }
        public virtual int Quality { get { return -1; } }
        public virtual int Levellimit { get { return -1; } }
        public virtual string Icon { get { return null; } }
        public virtual int SellValue { get { return -1; } }
        public virtual int ItemState { get { return -1; } }                  //物品的状态绑定还是什么
        public virtual int Count { get { return 1; } set { } }               //该类物品的数量
        public bool IsBind { get { return this.m_isBind; } set { this.m_isBind = value; } }
        //子类型
        public virtual int SubType { get { return -1; } }

        public JsonItem JsonItem { get { return this.m_item; } }

        public ItemType ItemType { get { return m_itemType; } }
        

        //上架返回
        private void OnShelve(DataObj data)
        {
            int ret = data.GetUInt16("ret");
            string id = data.GetString("uni_id");
            if (id != this.ID) return;
            Event.FWEvent.Instance.Call(Event.EventID.Deal_putShelveItem, new Event.EventArg(this, ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------

        public virtual void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Shelve_Good_back, OnShelve);
        }

        //修改物品的布局数据
        public void SetIndex(int index)
        {
            this.m_index = index;
        }
        
        //上架
        public void PutShelveRe(int price, int count, int time)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)this.ItemType;
            //配件不需要发子类型 
            if (this.ItemType == ItemType.Accessory)
                data["sub_type"] = (sbyte)1;
            else
                data["sub_type"] = (sbyte)this.SubType;
            data["index"] = this.Index;
            data["uni_id"] = this.ID;
            data["price"] = price;
            data["count"] = count;
            data["time"] = (sbyte)time;
            NetMgr.Instance.Request(Commond.Shelve_Good, data);
        }
    }
}