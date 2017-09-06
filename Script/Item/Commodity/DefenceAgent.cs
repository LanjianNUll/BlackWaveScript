//******************************************************************
// File Name:					DefenceAgent.cs
// Description:					DefenceAgent class 
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
    //护盾补给
    class DefenceAgent : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new DefenceAgent(id, item);
        }

        public DefenceAgent(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.DefenceAgent;
        }
    }
}