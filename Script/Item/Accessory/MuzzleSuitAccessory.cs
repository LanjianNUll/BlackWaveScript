//******************************************************************
// File Name:					MuzzleSuitAccessory
// Description:					MuzzleSuitAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:29:54 PM
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
    /// <summary>
    /// 枪管套件
    /// </summary>
    class MuzzleSuitAccessory : AccessoryBase 
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new MuzzleSuitAccessory(id, item);
        }
        public MuzzleSuitAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.MuzzleSuit;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        //精准值 伤害值  破甲值  射程  
        public float Accuracy { get { return this.JsonItem.Get("accuracy").AsFloat(); } }
        public float Damage { get { return this.JsonItem.Get("damage").AsFloat(); } }
        public int Sunder { get { return this.JsonItem.Get("sunder").AsInt(); } }
        public float Range { get { return this.JsonItem.Get("range").AsFloat(); } }
        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(MuzzleSuitAccessory muzzle)
        {
            string[] addOrSub =  new string[5];
            if (this.Accuracy - muzzle.Accuracy > 0)
            {
                addOrSub[0] = "+" + (this.Accuracy - muzzle.Accuracy);
            }
            else
            {
                addOrSub[0] = (this.Accuracy - muzzle.Accuracy).ToString();
            }
            if (this.Damage - muzzle.Damage > 0)
            {
                addOrSub[1] = "+" + (this.Damage - muzzle.Damage);
            }
            else
            {
                addOrSub[1] = (this.Damage - muzzle.Damage).ToString();
            }
            if (this.Sunder - muzzle.Sunder > 0)
            {
                addOrSub[2] = "+" + (this.Sunder - muzzle.Sunder);
            }
            else
            {
                addOrSub[2] = (this.Sunder - muzzle.Sunder).ToString();
            }
            if (this.Range - muzzle.Range > 0)
            {
                addOrSub[3] = "+" + (this.Range - muzzle.Range);
            }
            else
            {
                addOrSub[3] = (this.Range - muzzle.Range).ToString();
            }
            if (this.Gravity - muzzle.Gravity > 0)
            {
                addOrSub[4] = "+" + (this.Gravity - muzzle.Gravity);
            }
            else
            {
                addOrSub[4] = (this.Gravity - muzzle.Gravity).ToString();
            }
            if (String.IsNullOrEmpty(addOrSub[0]))
                return null;
            return addOrSub;
        }
    }
}
