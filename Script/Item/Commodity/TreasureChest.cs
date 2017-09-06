//******************************************************************
// File Name:					TreasureChest.cs
// Description:					TreasureChest class 
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
    /// 宝箱
    /// </summary>
    class TreasureChest : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new TreasureChest(id, item);
        }

        public TreasureChest(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.TreasureChest;
        }
    }
}