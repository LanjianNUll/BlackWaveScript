//******************************************************************
// File Name:					EquipmentBase.cs
// Description:					EquipmentBase class 
// Author:						wuwei
// Date:						2016.12.27
// Reference:
// Using:
// Revision History:
//******************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Network.Serializer;
using FW.ResMgr;

namespace FW.Item
{
    //装备基类
    class EquipmentBase : ItemBase
    {
        public EquipmentBase(ItemType type, string id, JsonItem item) : base(type, id, item)
        {
        }
    }
}