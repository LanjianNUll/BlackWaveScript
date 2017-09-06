//******************************************************************
// File Name:					CurrencyBase.cs
// Description:					CurrencyBase class 
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
    class CurrencyBase : CommodityBase
    {
        public CurrencyBase(string id, JsonItem item) : base(id, item)
        {
        }
    }
}