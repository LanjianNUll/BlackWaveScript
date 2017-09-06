//******************************************************************
// File Name:					MailAttachment.cs
// Description:					MailAttachment class 
// Author:						wuwei
// Date:						2017.02.18
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
    class MailAttachment
    {
        private string m_id;                                //id
        private int m_resID;                                //res id
        private int m_count;                                //数量
        private ItemBase m_itemBase;                        //物品
        private CashCurrency m_cashCurrency;                //货币


        public MailAttachment(DataObj data)
        {
            this.Init(data);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public string ID { get { return this.m_id; } }             //id
        public int ResID { get { return m_resID; } }               //res id
        public int Count { get { return m_count; } }               //数量
        public ItemBase ItemBase { get{ return m_itemBase; } }
       

        //--------------------------------------
        //public 
        //--------------------------------------    
        private void Init(DataObj data)
        {
            if (data == null) return;
            this.m_id = data.GetString("itemId");
            this.m_resID = data.GetInt32("stdid");
            this.m_count = data.GetInt32("num");
            //210  武器   220 配件   204 道具  206 货币
            if (m_resID / 1000000 == 210)
                this.m_itemBase = ItemMgr.Create(ItemType.Weapon,this.m_id,this.m_resID);
            if (m_resID / 1000000 == 220)
                this.m_itemBase = ItemMgr.Create(ItemType.Accessory, this.m_id, this.m_resID);
            if (m_resID / 1000000 == 204)
                this.m_itemBase = ItemMgr.Create(ItemType.Commodity, this.m_id, this.m_resID);
            if (m_resID / 1000000 == 206)
                this.m_itemBase = ItemMgr.Create(ItemType.Commodity, this.m_id, this.m_resID);
        }

        //--------------------------------------
        //public 
        //--------------------------------------                    
        public void Dispose()
        {
        }
    }
}