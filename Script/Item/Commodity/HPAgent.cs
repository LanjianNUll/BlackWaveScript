//******************************************************************
// File Name:					HPAgentGrenadeItem.cs
// Description:					HPAgent class 
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
    //治疗补给
    class HPAgent : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new HPAgent(id, item);
        }

        public HPAgent(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.HPAgent;
        }
    }
}