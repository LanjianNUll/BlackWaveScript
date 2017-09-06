//******************************************************************
// File Name:					CommodityBase.cs
// Description:					CommodityBase class 
// Author:						wuwei
// Date:						2017.02.22
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
    //道具基类
    class CommodityBase : ItemBase
    {
        delegate CommodityBase CommodityCreator(string id, JsonItem item);
        private static Dictionary<CommodityType, CommodityCreator> sm_creators;

        protected CommodityType m_type;                         //类型
        private int m_count;

        public static ItemBase Create(ItemType type, string id, int resID)
        {
            JsonItem item = DatasMgr.GetJsonItem(resID);
            if (item == null)
            {
                Debug.LogFormat("commodity didn't exist!!!" + resID);
                return null;
            }
            CommodityType commodityType = (CommodityType)item.Get("type").AsInt();
            CommodityCreator creator;
            if (sm_creators.TryGetValue(commodityType, out creator))
            {
                return creator(id, item);
            }
            Debug.LogFormat("commodity  type didn't exist!!!" + commodityType);
            return null;
        }

        static CommodityBase()
        {
            sm_creators = new Dictionary<CommodityType, CommodityCreator>();
            sm_creators.Add(CommodityType.Revival, Revival.Create);                         //1.复活币
            sm_creators.Add(CommodityType.GrenadeItem, GrenadeItem.Create);                 //2.手雷
            sm_creators.Add(CommodityType.HPAgent, HPAgent.Create);                         //3.治疗补给
            sm_creators.Add(CommodityType.DefenceAgent, DefenceAgent.Create);               //4.护盾补给
            sm_creators.Add(CommodityType.AmmoAgent, AmmoAgent.Create);                     //5.弹药补给
            sm_creators.Add(CommodityType.ExpAgent, ExpAgent.Create);                       //6.经验道具
            sm_creators.Add(CommodityType.TreasureChest, TreasureChest.Create);             //7.关卡宝箱
            sm_creators.Add(CommodityType.HPItem, HPItem.Create);                           //8.准备界面道具血量
            sm_creators.Add(CommodityType.AttackItem, AttackItem.Create);                   //9.准备界面道具攻击
            sm_creators.Add(CommodityType.SpeedItem, SpeedItem.Create);                     //10.准备界面道具速度
            sm_creators.Add(CommodityType.AmmoItem, AmmoItem.Create);                       //11.准备界面道具备弹数量
            sm_creators.Add(CommodityType.ProfitItem, ProfitItem.Create);                   //12.准备界面道具收益
            sm_creators.Add(CommodityType.GoldCurrency, GoldCurrency.Create);               //13.黄金
            sm_creators.Add(CommodityType.DiamondCurrency, DiamondCurrency.Create);         //14.钻石
            sm_creators.Add(CommodityType.CashCurrency, CashCurrency.Create);               //15.现金
            sm_creators.Add(CommodityType.WashPointCard, WashPointCardItem.Create);         //15.洗点卡
        }

        public CommodityBase(string id, JsonItem item) : base(ItemType.Commodity, id, item)
        {
        }

        //类型
        public CommodityType Type { get { return m_type; } }
        //道具名称
        public override string Name { get { return Utility.Utility.GetColorStr(this.JsonItem.Get("name").AsString(), this.Quality); } }
        public CommodityUseType MainUseType { get { return (CommodityUseType)this.JsonItem.Get("maintype").AsInt(); } }
        public CommodityTradeType MainTradeType { get { return (CommodityTradeType)this.JsonItem.Get("tradetype").AsInt(); } }
        public override int Quality { get { return this.JsonItem.Get("quality").AsInt(); } }
        public override int Levellimit { get { return this.JsonItem.Get("levellimit").AsInt(); }}
        public string Desc { get { return this.JsonItem.Get("desc").AsString(); } }
        public int IsSell { get { return this.JsonItem.Get("issell").AsInt(); } }
        public int Value { get { return this.JsonItem.Get("sellValue").AsInt(); } }
        //背包图标
        public string BagIcon { get { return DatasMgr.GetRes(this.JsonItem.Get("bagIcon").AsInt()); } }
        public override string Icon { get { return this.BagIcon; } }
        public int ItemModel { get { return this.JsonItem.Get("itemModel").AsInt(); } }
        public int LevelLimit { get { return this.JsonItem.Get("levellimit").AsInt(); } }
        public string Param { get { return this.JsonItem.Get("param").AsString(); } }
        //道具交易类型
        public override int TradeType { get { return this.JsonItem.Get("tradetype").AsInt(); } }
        public override int SubType { get {  return (int)this.Type; } }
        public override int SellValue { get { return this.Value; } }
        public override int Count { get { return this.m_count; } set { this.m_count = value; } }

        //--------------------------------------
        //private 
        //--------------------------------------



        //--------------------------------------
        //public s
        //--------------------------------------
    }
}
