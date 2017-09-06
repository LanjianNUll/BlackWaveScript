//******************************************************************
// File Name:					BarrelAccessory
// Description:					BarrelAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:24:08 PM
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
    /// 枪管
    /// </summary>
    class BarrelAccessory : AccessoryBase
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new BarrelAccessory(id, item);
        }

        public BarrelAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.Barrel;
        }

        private int m_sunder;                     //破甲值
        private float  m_pierce;                  //穿透值
        private float m_range;                    //射程
        private float m_slowTime1;                //停滞时间
        //--------------------------------------
        //properties 
        //--------------------------------------
        public int Sunder { get { return this.JsonItem.Get("sunder").AsInt(); } }
        public float Pirerce { get { return this.JsonItem.Get("pierce").AsFloat(); } }
        public float Range { get { return this.JsonItem.Get("range").AsFloat(); } }
        public float SlowTime1 { get { return this.JsonItem.Get("slowTime1").AsFloat(); } }
       
        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(BarrelAccessory muzzle)
        {
            string[] addOrSub = new string[5];
            if (this.Sunder - muzzle.Sunder > 0)
            {
                addOrSub[0] = "+" + (this.Sunder - muzzle.Sunder);
            }
            else
            {
                addOrSub[0] = (this.Sunder - muzzle.Sunder).ToString();
            }
            if (this.Pirerce - muzzle.Pirerce > 0)
            {
                addOrSub[1] = "+" + (this.Pirerce - muzzle.Pirerce);
            }
            else
            {
                addOrSub[1] = (this.Pirerce - muzzle.Pirerce).ToString();
            }
            if (this.Range - muzzle.Range > 0)
            {
                addOrSub[2] = "+" + (this.Range - muzzle.Range);
            }
            else
            {
                addOrSub[2] = (this.Range - muzzle.Range).ToString();
            }
            if (this.SlowTime1 - muzzle.SlowTime1 > 0)
            {
                addOrSub[3] = "+" + (this.SlowTime1 - muzzle.SlowTime1);
            }
            else
            {
                addOrSub[3] = (this.SlowTime1 - muzzle.SlowTime1).ToString();
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
