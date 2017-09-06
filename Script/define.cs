//******************************************************************
// File Name:					define.cs
// Description:					
// Author:						wuwei
// Date:						2017.02.21
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;

namespace FW
{
    //货币类型
    enum CurrencyType
    {
        Unknow,
        Cash,               //现金
        Gold,               //金币
        Diamond,            //钻石
    }

    enum RoleOnlineState
    {
        Unknow,
        OnlyPc,             //仅pc
        OnlyMobile,         //仅移动
        PcAndMobile,        //pc和移动同时在线
    }

    enum RolePointType
    {
        Unknow,
        Cash,               //现金
        Diamond,            //钻石
        Gold,               //金币
    }
}
