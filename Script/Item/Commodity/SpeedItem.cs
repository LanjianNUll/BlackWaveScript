//******************************************************************
// File Name:					SpeedItem.cs
// Description:					SpeedItem class 
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
    /// <summary>
    /// 道具速度
    /// </summary>
    class SpeedItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new SpeedItem(id, item);
        }

        public SpeedItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.SpeedItem;
        }
    }
}