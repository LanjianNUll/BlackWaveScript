//******************************************************************
// File Name:					WrestleWeapon
// Description:					WrestleWeapon class 
// Author:						lanjian
// Date:						12/29/2016 2:25:11 PM
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
    class WrestleWeapon : WeaponBase
    {
        public static WeaponBase Create(string id, JsonItem item)
        {
            return new WrestleWeapon(id, item);
        }


        public WrestleWeapon(string id, JsonItem item) : base(id, item)
        {
            this.m_WeaponType = WeaponType.Melee;
        }
    }
}
