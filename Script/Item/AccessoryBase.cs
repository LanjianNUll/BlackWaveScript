//******************************************************************
// File Name:					AccessoryBase.cs
// Description:					AccessoryBase class 
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
    class AccessoryBase : EquipmentBase
    {
        delegate AccessoryBase AccessoryCreator(string id, JsonItem item);
        private static Dictionary<AccessoryType, AccessoryCreator> sm_creators;

        //配件共有的属性
        private  string m_name;                             //名称
        private string m_desc;                              //物品描述
        private int m_quality;                              //道具品质
        private int m_levellimit;                           //等级限制
        private float m_gravity;                            //重量
        private string m_partsIcon;                         //配件图标
        private int m_sellValue;                            //出售价格
        private int m_equipValue;                           //装配价格
        private int m_count;                                //数量

        private int[] m_ports;                              //适用接口类型

        protected AccessoryType m_type;                      //道具类型

        public static ItemBase Create(ItemType type, string id, int resID)
        {
            JsonItem item = DatasMgr.GetJsonItem(resID);
            if (item == null)
            {
                Debug.LogFormat("accessory didn't exist!!!" + resID);
                return null;
            }
            AccessoryType accessoryType = (AccessoryType)item.Get("type").AsInt();
            AccessoryCreator creator;
            if (sm_creators.TryGetValue(accessoryType, out creator))
            {
                return creator(id, item);
            }
            Debug.LogFormat("accessory  type didn't exist!!!" + accessoryType);
            return null;
        }

        static AccessoryBase()
        {
            sm_creators = new Dictionary<AccessoryType, AccessoryCreator>();
            sm_creators.Add(AccessoryType.Muzzle, MuzzleAccessory.Create);//枪口
            sm_creators.Add(AccessoryType.Barrel, BarrelAccessory.Create);//枪管
            sm_creators.Add(AccessoryType.Sight, SighyAccessory.Create);//瞄具
            sm_creators.Add(AccessoryType.Maganize, MaganizeAccessory.Create); //弹夹
            sm_creators.Add(AccessoryType.MuzzleSuit, MuzzleSuitAccessory.Create);//枪管套件
            sm_creators.Add(AccessoryType.Trigger, TriggerAccessory.Create);//扳机套件
        }

        public AccessoryBase(string id, JsonItem item) : base(ItemType.Accessory, id, item)
        {
            this.Init();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public AccessoryType Type { get { return this.m_type; } }

        public override string Name { get { return Utility.Utility.GetColorStr(this.m_name, this.Quality); } }
        public string Desc { get { return m_desc; } }
        public override int Quality { get { return m_quality; } }
        public override int Levellimit { get { return this.m_levellimit; } }
        public float Gravity { get { return m_gravity; } }
        public string PartsIcon { get { return m_partsIcon; } }
        public int[] Ports { get { return this.m_ports; } }             //适用接口类型
        public override string Icon{ get{ return this.PartsIcon; } }    //统一图标
        public int Value { get { return this.JsonItem.Get("value").AsInt(); } }             //购买价格
        public override int SellValue { get { return this.m_sellValue; } }     //出售价格    
        public int CurrencyType { get { return this.JsonItem.Get("currencyType").AsInt(); }}//购买货币种类
        //配件交易类型
        public override int TradeType { get { return this.JsonItem.Get("tradetype").AsInt(); } }
        public override int SubType { get { return (int)this.Type; } }
        public override int Count { get { return this.m_count; } set { this.m_count = value; } }

        public int EquipValue { get { return this.m_equipValue; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            if (this.JsonItem == null) return;
            this.m_name = this.JsonItem.Get("name").AsString();                                 //名称
            this.m_desc = this.JsonItem.Get("desc").AsString();                              //伤害值
            this.m_quality = this.JsonItem.Get("quality").AsInt();
            this.m_levellimit = this.JsonItem.Get("levellimit").AsInt();
            this.m_gravity = this.JsonItem.Get("gravity").AsFloat();
            this.m_partsIcon = DatasMgr.GetRes(this.JsonItem.Get("partsIcon").AsInt());
            this.m_ports = this.JsonItem.Get("partstype").AsInts();
            this.m_sellValue = this.JsonItem.Get("sellValue").AsInt();
            this.m_equipValue = this.JsonItem.Get("equipvalue").AsInt();
        }

        //--------------------------------------
        //public 
        //--------------------------------------


    }
}