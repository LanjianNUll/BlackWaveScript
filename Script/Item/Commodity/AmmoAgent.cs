//******************************************************************
// File Name:					AmmoAgent.cs
// Description:					AmmoAgent class 
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
    //弹药补给
    class AmmoAgent : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new AmmoAgent(id, item);
        }

        public AmmoAgent(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.AmmoAgent;
        }
    }
}