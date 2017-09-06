//******************************************************************
// File Name:					TriggerAccessory
// Description:					TriggerAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:31:02 PM
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
    /// 扳机套件配件
    /// </summary>
    class TriggerAccessory : AccessoryBase
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new TriggerAccessory(id, item);
        }

        public TriggerAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.Trigger;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        //射速（攻击间隔）   装填时间  后坐力
        public float ShootTime1 { get { return this.JsonItem.Get("shootTime").AsFloat(); } }
        public float Reloadtime { get { return this.JsonItem.Get("reloadtime").AsFloat(); } }
        public float Backpower { get { return this.JsonItem.Get("backpower").AsFloat(); } }
        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(TriggerAccessory muzzle)
        {
            string[] addOrSub = new string[4];
            if (this.ShootTime1 - muzzle.ShootTime1 > 0)
            {
                addOrSub[0] = "+" + (this.ShootTime1 - muzzle.ShootTime1);
            }
            else
            {
                addOrSub[0] = (this.ShootTime1 - muzzle.ShootTime1).ToString();
            }
            if (this.Reloadtime - muzzle.Reloadtime > 0)
            {
                addOrSub[1] = "+" + (this.Reloadtime - muzzle.Reloadtime);
            }
            else
            {
                addOrSub[1] = (this.Reloadtime - muzzle.Reloadtime).ToString();
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
