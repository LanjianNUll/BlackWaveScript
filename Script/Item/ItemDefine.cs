//******************************************************************
// File Name:					ItemDefine.cs
// Description:
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

namespace FW.Item
{

    //物品分类
    enum ItemType
    {
        Unknow,
        Weapon,             //武器
        Accessory,          //配件
        Commodity,          //道具类
    }

    //武器类型
    enum WeaponType
    {
        Unknow,
        Main,               //主武器
        Second,             //副武器
        Melee,              //近战
        Grenade,            //手雷
    }

    //武器配件类型
    enum AccessoryType
    {
        Unknow,
        Muzzle = 1,         //枪口
        Barrel,             //枪管
        Sight,              //瞄具
        Maganize,           //弹夹
        MuzzleSuit,         //枪管套件
        Trigger,            //扳机套件
    }

    //道具类型
    enum CommodityType
    {
        Unknow,
        Revival,                //1.复活币
        GrenadeItem,            //2.手雷
        HPAgent,                //3.治疗补给
        DefenceAgent,           //4.护盾补给
        AmmoAgent,              //5.弹药补给
        ExpAgent,               //6.经验道具
        TreasureChest,          //7.关卡宝箱
        HPItem,                 //8.准备界面道具血量
        AttackItem,             //9.准备界面道具攻击
        SpeedItem,              //10.准备界面道具速度
        AmmoItem,               //11.准备界面道具备弹数量
        ProfitItem,             //12.准备界面道具收益
        GoldCurrency,           //13.黄金
        DiamondCurrency,        //14.钻石
        CashCurrency,           //15.现金
        WashPointCard,          //16.洗点卡
    }

    //道具用途类型
    enum CommodityUseType
    {
        Unknow,
        Consumption,            //1.消耗品
        TreasureChest,          //2.宝箱
        AddPlayerPro,           //3.增加玩家点数    
        Supply = 9,             //9.补给品
    }

    //道具交易类型
    enum CommodityTradeType
    {
        Unknow,
        Revival,                //1.复活币
        TreasureChest,          //2.宝箱
        ExpAgent,               //3.经验道具
        Supply = 9,             //9.补给品
        GrenadeItem = 10,       //10.手雷
        ProfitItem = 12,        //12.准备界面道具收益
        Money = 12,             //12.货币
    }
}