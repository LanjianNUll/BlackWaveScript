//******************************************************************
// File Name:					DiamondCurrency.cs
// Description:					DiamondCurrency class 
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
    class DiamondCurrency : CurrencyBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new DiamondCurrency(id, item);
        }

        public DiamondCurrency(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.DiamondCurrency;
        }
    }
}