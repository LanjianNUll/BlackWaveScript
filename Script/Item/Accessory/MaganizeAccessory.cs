//******************************************************************
// File Name:					MaganizeAccessory
// Description:					MaganizeAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:27:42 PM
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
    /// 弹夹配件
    /// </summary>
    class MaganizeAccessory : AccessoryBase
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new MaganizeAccessory(id, item);
        }

        public MaganizeAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.Maganize;
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        //弹夹量  载弹量   射速 （攻击间隔）
        public int BoxAmmoCount1 { get { return this.JsonItem.Get("boxAmmoCount").AsInt(); } }
        public int BackAmmoCount1 { get { return this.JsonItem.Get("backAmmoCount").AsInt(); } }
        public float ShootTime1 { get { return this.JsonItem.Get("shootTime").AsFloat(); } }
        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(MaganizeAccessory muzzle)
        {
            string[] addOrSub = new string[4];
            if (this.BoxAmmoCount1 - muzzle.BoxAmmoCount1 > 0)
            {
                addOrSub[0] = "+" + (this.BoxAmmoCount1 - muzzle.BoxAmmoCount1);
            }
            else
            {
                addOrSub[0] = (this.BoxAmmoCount1 - muzzle.BoxAmmoCount1).ToString();
            }
            if (this.BackAmmoCount1 - muzzle.BackAmmoCount1 > 0)
            {
                addOrSub[1] = "+" + (this.BackAmmoCount1 - muzzle.BackAmmoCount1);
            }
            else
            {
                addOrSub[1] = (this.BackAmmoCount1 - muzzle.BackAmmoCount1).ToString();
            }
            if (this.ShootTime1 - muzzle.ShootTime1 > 0)
            {
                addOrSub[2] = "+" + (this.ShootTime1 - muzzle.ShootTime1);
            }
            else
            {
                addOrSub[2] = (this.ShootTime1 - muzzle.ShootTime1).ToString();
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
