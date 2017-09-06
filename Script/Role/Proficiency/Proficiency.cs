//******************************************************************
// File Name:					Proficiency
// Description:					Proficiency class 
// Author:						lanjian
// Date:						4/14/2017 4:54:50 PM
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
    /// 武器熟练度
    /// </summary>
    class Proficiency
    {
        private int m_weaponSortId;                             //枪械类型
        private string m_weaponSortName;                        //枪械类型名称
        private float m_proficiency;                            //熟练度总值
        private float m_sunderArmor;                            //破甲
        private float m_injure;                                 //伤害
        private float m_shoootTime;                             //射击间隔
        private float m_reloadTime;                             //装填时间
        private float m_accuracy;                               //初始精度
        private float m_critRatio;                              //暴击率
        private float m_throughForce;                           //穿透值
        private float m_fireRange;                              //射程
        private float m_boxAmmoCount;                           //弹夹上限
        private float m_ciritFilter;                            //暴击系数
        private float m_slowTime;                               //停滞时间
        private float m_slowRatio;                              //停滞比例
        private float m_changerTime;                            //取枪时间
        private float m_gravity;                                //枪重
        private string m_icon;                                  //图标
        private float m_radio;                                  //比例

        private List<string> m_propery;                         //属性名称
        private List<string> m_value;                           //属性值

        public Proficiency(JsonItem jsonItem, int id, int proficiency)
        {
            this.m_weaponSortId = id;
            this.m_proficiency = proficiency;
            this.Init(jsonItem);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int WeaponId { get { return this.m_weaponSortId; } }
        public float Proficie { get { return this.m_proficiency; } }
        public string Icon { get { return this.m_icon; } }
        public string Name { get { return this.m_weaponSortName; } }
        public float Radio { get { return this.m_radio; } }
        public List<string> Propery { get { return this.m_propery; } }
        public List<string> Value { get { return this.m_value; } }
        //--------------------------------------
        //private 
        //--------------------------------------

        private void Init(JsonItem jsonItem)
        {
            m_propery = new List<string>();
            m_value = new List<string>();
            this.m_icon = DatasMgr.GetRes(jsonItem.Get("icon").AsInt());
            this.m_weaponSortName = jsonItem.Get("name").AsString();
          
            this.m_sunderArmor = jsonItem.Get("sunderArmor").AsFloat();
            this.m_injure = jsonItem.Get("injure").AsFloat();
            this.m_shoootTime = jsonItem.Get("shootTime").AsFloat();
            this.m_reloadTime = jsonItem.Get("reloadTime").AsFloat();
            this.m_accuracy = jsonItem.Get("accuracy0").AsFloat();
            this.m_critRatio = jsonItem.Get("critRatio").AsFloat();
            this.m_throughForce = jsonItem.Get("throughForce").AsFloat();
            this.m_fireRange = jsonItem.Get("fireRange").AsFloat();
            this.m_boxAmmoCount = jsonItem.Get("boxAmmoCount").AsFloat();
            this.m_ciritFilter = jsonItem.Get("critFilter").AsFloat();
            this.m_slowTime = jsonItem.Get("slowTime").AsFloat();
            this.m_slowRatio = jsonItem.Get("slowRatio").AsFloat();
            this.m_changerTime = jsonItem.Get("changeTime").AsFloat();
            this.m_gravity = jsonItem.Get("gravity").AsFloat();
            this.m_radio = (float)m_proficiency > (float)jsonItem.Get("proficiency").AsInt()?1: (float)m_proficiency / (float)jsonItem.Get("proficiency").AsInt();
            GetProPery();
        }

        private void GetProPery()
        {
            if (!DoWithStr(this.m_sunderArmor).Equals(""))
            {
                m_propery.Add("破甲");
                m_value.Add(DoWithStr(this.m_sunderArmor));
            }
            if (!DoWithStr(this.m_injure).Equals(""))
            {
                m_propery.Add("伤害");
                m_value.Add(DoWithStr(this.m_injure));
            }
            if (!DoWithStr(this.m_shoootTime).Equals(""))
            {
                m_propery.Add("射速");
                m_value.Add(DoWithStr(this.m_injure));
            }
            if (!DoWithStr(this.m_reloadTime).Equals(""))
            {
                m_propery.Add("装填时间");
                m_value.Add(DoWithStr(this.m_reloadTime));
            }
            if (!DoWithStr(this.m_accuracy).Equals(""))
            {
                m_propery.Add("初始精度");
                m_value.Add(DoWithStr(this.m_accuracy));
            }
            if (!DoWithStr(this.m_critRatio).Equals(""))
            {
                m_propery.Add("暴击率");
                m_value.Add(DoWithStr(this.m_critRatio));
            }
            if (!DoWithStr(this.m_throughForce).Equals(""))
            {
                m_propery.Add("穿透");
                m_value.Add( DoWithStr(this.m_throughForce));
            }
            if (!DoWithStr(this.m_fireRange).Equals(""))
            {
                m_propery.Add("射程");
                m_value.Add(DoWithStr(this.m_fireRange));
            }
            if (!DoWithStr(this.m_boxAmmoCount).Equals(""))
            {
                m_propery.Add("弹夹上限");
                m_value.Add( DoWithStr(this.m_boxAmmoCount));
            }
            if (!DoWithStr(this.m_ciritFilter).Equals(""))
            {
                m_propery.Add("暴击系数");
                m_value.Add(DoWithStr(this.m_ciritFilter));
            }
            if (!DoWithStr(this.m_slowTime).Equals(""))
            {
                m_propery.Add("停滞时间");
                m_value.Add(DoWithStr(this.m_slowTime));
            }
            if (!DoWithStr(this.m_slowRatio).Equals(""))
            {
                m_propery.Add("停滞比例");
                m_value.Add(DoWithStr(this.m_slowRatio));
            }
            if (!DoWithStr(this.m_changerTime).Equals(""))
            {
                m_propery.Add("取枪时间");
                m_value.Add(DoWithStr(this.m_changerTime));
            }
            if (!DoWithStr(this.m_gravity).Equals(""))
            {
                m_propery.Add("枪重");
                m_value.Add(DoWithStr(this.m_gravity));
            }
        }

        private string DoWithStr(float data)
        {
            if (data > 0)
                return "+" + (data * 100).ToString() + "%";
            if (data < 0)
                return (data * 100).ToString() + "%";
            else
                return "";
        }

        //--------------------------------------
        //public 
        //--------------------------------------
    }
}
