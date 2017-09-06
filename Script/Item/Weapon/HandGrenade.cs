//******************************************************************
// File Name:					HandGrenade
// Description:					HandGrenade class 
// Author:						lanjian
// Date:						12/29/2016 2:26:06 PM
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
    /// 手雷投掷武器类
    /// </summary>
    class HandGrenade : WeaponBase
    {
        public static WeaponBase Create(string id, JsonItem item)
        {
            return new HandGrenade(id, item);
        }

        public HandGrenade(string id, JsonItem item) : base(id, item)
        {
            this.m_WeaponType = WeaponType.Grenade;
        }
    }
}
