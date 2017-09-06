//******************************************************************
// File Name:					WashPointCardItem
// Description:					WashPointCardItem class 
// Author:						lanjian
// Date:						4/25/2017 4:33:39 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.ResMgr;
using UnityEngine;
namespace FW.Item
{
    class WashPointCardItem: CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new WashPointCardItem(id, item);
        }

        public WashPointCardItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.WashPointCard;
        }
    }
}
