//******************************************************************
// File Name:					LuckyJackPot
// Description:					LuckyJackPot class 
// Author:						lanjian
// Date:						5/22/2017 9:56:41 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.LuckyJoy
{ 
    //单个奖池物品
    class LuckyJackPot
    {
        private string m_id;
        private string m_name;
        private string m_iconPath;
        private int m_chance;                           //标准出现概率
        private int m_chance1;                          //J1出现概率
        private int m_chance2;                          //J2出现概率
        private int m_chance3;                          //J3出现概率

        public string ID { get { return this.m_id; } }
        public string Name { get { return this.m_name; } }
        public string Icon { get { return this.m_iconPath; } }
        public float Chance { get { return this.m_chance; } }
        public float Chance1 { get { return this.m_chance1; } }
        public float Chance2 { get { return this.m_chance2; } }
        public float Chance3 { get { return this.m_chance3; } }
        public int Index { get { return int.Parse(this.m_id); } }

        public LuckyJackPot(string id, JsonItem jsonItem)
        {
            this.m_id = id;
            this.Init(jsonItem);
        }

        private void Init(JsonItem jsonItem)
        {
            if (jsonItem == null) return;
            this.m_name = jsonItem.Get("name").AsString();
            this.m_iconPath = DatasMgr.GetRes(jsonItem.Get("icon").AsInt());
            this.m_chance = jsonItem.Get("chance").AsInt();
            this.m_chance1 = jsonItem.Get("chance1").AsInt();
            this.m_chance2 = jsonItem.Get("chance2").AsInt();
            this.m_chance3 = jsonItem.Get("chance3").AsInt();
        }
    }
}
