//******************************************************************
// File Name:					PayItem
// Description:					PayItem class 
// Author:						lanjian
// Date:						4/28/2017 9:06:36 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Store
{
    //充值礼包
    class PayItem
    {
        private string m_id;                                           
        private string m_name;
        private int m_Diamondnum;
        private int m_price;
        private string m_icon;
        private float m_discount;
        private string m_desc;

        //--------------------------------------
        //properties 
        //--------------------------------------

        public string ID { get { return this.m_id; } }
        public string Name { get { return this.m_name; } }
        public int DiamondNum { get { return this.m_Diamondnum; } }
        public int Price { get { return this.m_price/100; } }
        public string Icon { get { return this.m_icon; } }
        public float Discount { get { return this.m_discount; } }
        public string Desc { get { return this.m_desc; } }

        public PayItem(string id,JsonItem jsonItem)
        {
            this.m_id = id;
            this.Init(jsonItem);
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(JsonItem jsonItem)
        {
            if (jsonItem == null) return;

            this.m_name = jsonItem.Get("name").AsString();
            this.m_Diamondnum = jsonItem.Get("num").AsInt();
            this.m_price = jsonItem.Get("price").AsInt();
            this.m_icon = DatasMgr.GetRes(jsonItem.Get("icon").AsInt());
            this.m_discount = jsonItem.Get("discount").AsFloat();
            this.m_desc = jsonItem.Get("desc").AsString();
        }
    }
}
