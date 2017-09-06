//******************************************************************
// File Name:					AttackItem.cs
// Description:					AttackItem class 
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
    //道具攻击
    class AttackItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new AttackItem(id, item);
        }

        public AttackItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.AttackItem;
        }
    }
}