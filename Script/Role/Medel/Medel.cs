//******************************************************************
// File Name:					Medel
// Description:					Medel class 
// Author:						lanjian
// Date:						3/29/2017 1:52:12 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Role
{
    /// <summary>
    /// 勋章
    /// </summary>
    class Medel
    {
        private int m_Id;
        private int m_GetTime;
        private string m_Name;
        private string m_Icon;
        private string m_GetWayDesc;
        private float m_liftAdd;                        //生命值
        private float m_shieldAdd;                      //护甲
        private float m_defendseAdd;                    //防御值
        private float m_damageAdd;                      //伤害值
        private float m_sunderAdd;                      //破甲
        private float m_fireRangeAdd;                   //射程
        private float m_cirticalRateAdd;                //暴击率
        private float m_ciritcalDemageAdd;              //暴击伤害
        private float m_initAccuracyAdd;                //精确值
        private float m_recoilAdd;                      //后坐力
        private float m_throughForceAdd;                //穿透率
        private float m_moveSpeedAdd;                   //移动速度
        private float m_changeWeaponTimeAdd;            //换枪时间
        private float m_reloadTimeAdd;                  //装填时间
        private float m_shottTimeAdd;                   //射击间隔
        private float m_slowTimeAdd;                    //停滞力
        private float m_boxmoCountAdd;                  //弹夹量上限
        private float m_derateAdd;                      // 伤害减免

        private string m_tip1;
        private string m_tip2;
        
        public Medel(JsonItem jsonItem, int id,int getTime)
        {
            this.m_Id = id;
            this.m_GetTime = getTime;
            this.Init(jsonItem);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------

        public int Id { get { return this.m_Id; } }
        public int GetTime { get { return this.m_GetTime; } }
        public string Name { get { return this.m_Name; } }
        public string ICon { get { return this.m_Icon; } }
        public string GetWayDesc { get { return this.m_GetWayDesc; } }
        public string Tip1 { get { return this.m_tip1; } }
        public string Tip2 { get { return this.m_tip2; } }
        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(JsonItem json)
        {
            this.m_Name = json.Get("name").AsString();
            this.m_Icon = DatasMgr.GetRes(json.Get("icon").AsInt());
            this.m_GetWayDesc = json.Get("achieve").AsString();

            this.m_liftAdd = json.Get("Health").AsFloat();
            this.m_shieldAdd = json.Get("Shield").AsFloat();
            this.m_defendseAdd = json.Get("Defense").AsFloat();
            this.m_damageAdd = json.Get("Damage").AsFloat();
            this.m_sunderAdd = json.Get("Sunder").AsFloat();
            this.m_fireRangeAdd = json.Get("FireRange").AsFloat();
            this.m_cirticalRateAdd = json.Get("CriticalRate").AsFloat();
            this.m_ciritcalDemageAdd = json.Get("CritFilter").AsFloat();
            this.m_initAccuracyAdd = json.Get("InitAccuracy").AsFloat();
            this.m_recoilAdd = json.Get("recoil").AsFloat();
            this.m_throughForceAdd = json.Get("ThroughForce").AsFloat();
            this.m_moveSpeedAdd = json.Get("MoveSpeed").AsFloat();
            this.m_changeWeaponTimeAdd = json.Get("ChangeWeapTime").AsFloat();
            this.m_reloadTimeAdd = json.Get("ReloadTime").AsFloat();
            this.m_shottTimeAdd = json.Get("ShootTime").AsFloat();
            this.m_slowTimeAdd = json.Get("SlowTime").AsFloat();
            this.m_boxmoCountAdd = json.Get("backAmmoCount").AsFloat();
            this.m_derateAdd = json.Get("Derate").AsFloat();

            GetTips();
        }

        private void GetTips()
        {
            string str = "";
            if (!DoWithStr(m_liftAdd).Equals(""))
                str += "生命值" + DoWithStr(m_liftAdd) + "|";
            if (!DoWithStr(m_shieldAdd).Equals(""))
                str += "护甲" + DoWithStr(m_shieldAdd) + "|";
            if (!DoWithStr(m_defendseAdd).Equals(""))
                str += "防御值" + DoWithStr(m_defendseAdd) + "|";
            if (!DoWithStr(m_damageAdd).Equals(""))
                str += "伤害值" + DoWithStr(m_damageAdd) + "|";
            if (!DoWithStr(m_sunderAdd).Equals(""))
                str += "破甲" + DoWithStr(m_sunderAdd) + "|";
            if (!DoWithStr(m_fireRangeAdd).Equals(""))
                str += "射程" + DoWithStr(m_fireRangeAdd) + "|";
            if (!DoWithStr(m_cirticalRateAdd).Equals(""))
                str += "暴击率" + DoWithStr(m_cirticalRateAdd) + "|";
            if (!DoWithStr(m_ciritcalDemageAdd).Equals(""))
                str += "暴击伤害" + DoWithStr(m_ciritcalDemageAdd) + "|";
            if (!DoWithStr(m_initAccuracyAdd).Equals(""))
                str += "精确值" + DoWithStr(m_initAccuracyAdd) + "|";
            if (!DoWithStr(m_recoilAdd).Equals(""))
                str += "后坐力" + DoWithStr(m_recoilAdd) + "|";
            if (!DoWithStr(m_throughForceAdd).Equals(""))
                str += "穿透率" + DoWithStr(m_throughForceAdd) + "|";
            if (!DoWithStr(m_moveSpeedAdd).Equals(""))
                str += "移动速度" + DoWithStr(m_moveSpeedAdd) + "|";
            if (!DoWithStr(m_changeWeaponTimeAdd).Equals(""))
                str += "换枪时间" + DoWithStr(m_changeWeaponTimeAdd) + "|";
            if (!DoWithStr(m_reloadTimeAdd).Equals(""))
                str += "装填时间" + DoWithStr(m_reloadTimeAdd) + "|";
            if (!DoWithStr(m_moveSpeedAdd).Equals(""))
                str += "射击间隔" + DoWithStr(m_moveSpeedAdd) + "|";
            if (!DoWithStr(m_shottTimeAdd).Equals(""))
                str += "射击间隔" + DoWithStr(m_shottTimeAdd) + "|";
            if (!DoWithStr(m_slowTimeAdd).Equals(""))
                str += "停滞力" + DoWithStr(m_slowTimeAdd) + "|";
            if (!DoWithStr(m_boxmoCountAdd).Equals(""))
                str += "弹夹上限" + DoWithStr(m_boxmoCountAdd) + "|";
            if (!DoWithStr(m_derateAdd).Equals(""))
                str += "伤害减免" + DoWithStr(m_derateAdd) + " | ";
            if (str.Split('|').Length >= 2)
            {
                m_tip1 = str.Split('|')[0];
                m_tip2 = str.Split('|')[1];
            }
        }

        private string DoWithStr(float data)
        {
           if (data > 0)
                return "+" + (data * 100).ToString() + "%";
           else if (data < 0)
                return "-" + (data * 100).ToString() + "%";
           else
                return "";
        }

        //--------------------------------------
        //public 
        //--------------------------------------

    }
}
