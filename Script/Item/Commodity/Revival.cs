//******************************************************************
// File Name:					Revival.cs
// Description:					Revival class 
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
    //复活币
    class Revival : CommodityBase
    {
        public static CommodityBase Create(string id, JsonItem item)
        {
            return new Revival(id, item);
        }

        public Revival(string id, JsonItem item) : base(id, item)
        {
            this.m_type = CommodityType.Revival;
        }


    }
}