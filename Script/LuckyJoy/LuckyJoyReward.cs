//******************************************************************
// File Name:					LuckyJoyItem
// Description:					LuckyJoyItem class 
// Author:						lanjian
// Date:						5/19/2017 11:57:39 AM
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
    //中奖奖励
    class LuckyJoyReward
    {
        private string m_id;                    //组合id
        private int[] m_groups;                 //中奖组合
        private int m_returnRadio;              //返奖倍率
        private float m_expect;                 //期望   概率
        private LuckyJackPot[] m_groupsItem;    //中奖组合数组
        private int m_betMoney;                 //押注金额

        public string Id { get { return this.m_id; } }
        public int ReturnRadio { get { return this.m_returnRadio; } }
        public float Expect { get { return this.m_expect; } }
        public int[] Groups { get { return this.m_groups; } }
        public LuckyJackPot[] JcakPotArray { get { return this.m_groupsItem; } }
        public int BetMoney { get { return this.m_betMoney; } set { this.m_betMoney = value; } }

        public LuckyJoyReward(string id, JsonItem jsonItem)
        {
            this.m_id = id;
            this.Init(jsonItem);
        }

        private void Init(JsonItem jsonItem)
        {
            if (jsonItem == null) return;
            this.m_groups = jsonItem.Get("combination").AsInts();
            this.m_returnRadio = jsonItem.Get("reward").AsInt();
            this.m_expect = jsonItem.Get("expecet").AsFloat();
            InitData(this.m_groups);
        }

        private void InitData(int[] groups)
        {
            this.m_groupsItem = new LuckyJackPot[groups.Length];
            JsonConfig jsonConfig = DatasMgr.FWMSlotCfg; 
            for (int i = 0; i < groups.Length; i++)
            {
                JsonItem jsonItem = jsonConfig.GetJsonItem(groups[i].ToString());
                this.m_groupsItem[i] = new LuckyJackPot(groups[i].ToString(),jsonItem);
            }
        }

        public void ReSetData(int[] groups)
        {
            this.m_groups = groups;
            this.InitData(groups);
        }
    }
}
