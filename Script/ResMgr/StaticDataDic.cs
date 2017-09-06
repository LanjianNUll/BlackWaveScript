//******************************************************************
// File Name:					StaticDataDic.cs
// Description:					StaticDataDic class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       存储一些少量配置数据,以后考虑把这些数据放到配置文件里
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace FW.ResMgr
{
    public class StaticDataDic
    {
        public static Dictionary<int, string> sm_ISDic = new Dictionary<int, string>();
        static StaticDataDic()
        {
            sm_ISDic.Add(1, "res/model/characters/Hero/hero_leonus/hero_leonus");//英雄模型
            sm_ISDic.Add(2, "res/model/characters/Monster/automaticrifleman001/enemy");//敌人模型
            sm_ISDic.Add(3, "res/effects/particlas/VFX_Fire/VFX_Fire01");//开枪特效
            sm_ISDic.Add(4, "res/effects/particlas/VFX_Hit/VFX_Hit01");//被击特效
        }
        public static string GetISValue(int key)
        {
            if (!sm_ISDic.ContainsKey(key))
            {
                return string.Empty;
            }
            return sm_ISDic[key];
        }
    }
}
