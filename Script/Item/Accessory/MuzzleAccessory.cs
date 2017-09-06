//******************************************************************
// File Name:					MuzzleAccessory
// Description:					MuzzleAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:25:26 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;

using Network.Serializer;
using FW.ResMgr;

namespace FW.Item
{
    class MuzzleAccessory : AccessoryBase
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new MuzzleAccessory(id, item);
        }

        public MuzzleAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.Muzzle;
            this.Init();
        }

        private float m_damage;                     //单发伤害
        private int m_accuracyrenew;              //精准自动恢复值
        private float m_backpower;                     //后坐力

        //--------------------------------------
        //properties 
        //--------------------------------------
        public float Damage { get { return m_damage; } }
        public int Accuracyrenew { get { return m_accuracyrenew; } }
        public float Backpower { get { return m_backpower; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            if (this.JsonItem == null) return;
            this.m_damage = this.JsonItem.Get("damage").AsFloat();
            this.m_accuracyrenew = this.JsonItem.Get("accuracyrenew").AsInt();
            this.m_backpower = this.JsonItem.Get("backpower").AsFloat();
        }
        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(MuzzleAccessory muzzle)
        {
            string[] addOrSub = new string[4];
            if (this.Damage - muzzle.Damage > 0)
            {
                addOrSub[0] = "+" + (this.Damage - muzzle.Damage);
            }
            else
            {
                addOrSub[0] = (this.Damage - muzzle.Damage).ToString();
            }
            if (this.Accuracyrenew - muzzle.Accuracyrenew > 0)
            {
                addOrSub[1] = "+" + (this.Accuracyrenew - muzzle.Accuracyrenew);
            }
            else
            {
                addOrSub[1] = (this.Accuracyrenew - muzzle.Accuracyrenew).ToString();
            }
            if (this.Backpower - muzzle.Backpower > 0)
            {
                addOrSub[2] = "+" + (this.Backpower - muzzle.Backpower);
            }
            else
            {
                addOrSub[2] = (this.Backpower - muzzle.Backpower).ToString();
            }
            if (this.Gravity - muzzle.Gravity > 0)
            {
                addOrSub[3] = "+" + (this.Gravity - muzzle.Gravity);
            }
            else
            {
                addOrSub[3] = (this.Gravity - muzzle.Gravity).ToString();
            }
            if (String.IsNullOrEmpty(addOrSub[0]))
                return null;
            return addOrSub;
        }
    }
}
