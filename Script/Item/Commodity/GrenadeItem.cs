//******************************************************************
// File Name:					GrenadeItem.cs
// Description:					GrenadeItem class 
// Author:						wuwei
// Date:						2017.02.22
// Reference:
// Using:
// Revision History:
//******************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Network;
using Network.Serializer;
using FW.ResMgr;


namespace FW.Item
{
    //手雷
    class GrenadeItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new GrenadeItem(id, item);
        }

        public GrenadeItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.GrenadeItem;
        }
    }
}