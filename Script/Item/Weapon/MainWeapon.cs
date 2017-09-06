//******************************************************************
// File Name:					MainWeapon
// Description:					MainWeapon class 
// Author:						wuwei
// Date:						2017.01.18
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections.Generic;

using Network.Serializer;
using FW.ResMgr;

namespace FW.Item
{
    class MainWeapon : WeaponBase
    {
        public static WeaponBase Create(string id, JsonItem item)
        {
            return new MainWeapon(id, item);
        }

        public MainWeapon(string id, JsonItem item) : base(id, item)
        {
            this.m_WeaponType = WeaponType.Main;
        }

    }
}