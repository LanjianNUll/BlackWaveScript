//******************************************************************
// File Name:					AmmoItem.cs
// Description:					AmmoItem class 
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
    //弹药
    class AmmoItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new AmmoItem(id, item);
        }

        public AmmoItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.AmmoItem;
        }
    }
}