//******************************************************************
// File Name:					ConstantValue
// Description:					ConstantValue class 
// Author:						lanjian
// Date:						3/16/2017 2:03:36 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Utility
{
    //静态的常量
    static class ConstantValue
    {
        private static string UITexturePath = "res/UITexture/";       
        //武器
        public static string WeaponIcon = UITexturePath + "weaponIcon";
        //配件
        public static string PartIcon = UITexturePath + "partsIcon";
        //道具
        public static string CommodityIcon = UITexturePath + "commodity";
        //手雷
        public static string HandBombIcon = UITexturePath + "handBomb";
        //商城
        public static string ShopIcon = UITexturePath + "ShopIcon";
        //角色
        public static string RoleIcon = UITexturePath + "role";
        //down or uo
        public static string EndIconPath = "_down";
        public static string UpEndPath = "_up";
        //兑换
        public static string ExchangePath = UITexturePath + "exchange";
        //摇奖
        public static string LuckyIconPath = UITexturePath + "luckyIcon";
        //手续费
        public static int HandCharge
        {
            get
            {
                return (int)DatasMgr.FWMDefaultCfg.Data["trade_shelve_cost"];
            }
        }
    }
}
