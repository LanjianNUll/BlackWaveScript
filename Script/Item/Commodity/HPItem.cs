//******************************************************************
// File Name:					HPItem.cs
// Description:					HPItem class 
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
    //血量
    class HPItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new HPItem(id, item);
        }

        public HPItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.HPItem;
        }
    }
}