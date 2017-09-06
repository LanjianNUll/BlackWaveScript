//******************************************************************
// File Name:					SecondaryWeapon
// Description:					SecondaryWeapon class 
// Author:						wuwei
// Date:						2017.01.18
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
    class SecondaryWeapon : WeaponBase
    {
        public static WeaponBase Create(string id, JsonItem item)
        {
            return new SecondaryWeapon(id, item);
        }

        public SecondaryWeapon(string id, JsonItem item) : base(id, item)
        {
            this.m_WeaponType = WeaponType.Second;
        }
    }
}