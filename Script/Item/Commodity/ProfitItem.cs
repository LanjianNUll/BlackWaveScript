//******************************************************************
// File Name:					ProfitItem.cs
// Description:					ProfitItem class 
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
    //收益
    class ProfitItem : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new ProfitItem(id, item);
        }

        public ProfitItem(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.ProfitItem;
        }
    }
}