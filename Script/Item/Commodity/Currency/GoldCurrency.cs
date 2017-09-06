//******************************************************************
// File Name:					GoldCurrency.cs
// Description:					GoldCurrency class 
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
    class GoldCurrency : CurrencyBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new GoldCurrency(id, item);
        }

        public GoldCurrency(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.GoldCurrency;
        }
    }
}