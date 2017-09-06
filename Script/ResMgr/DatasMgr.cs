//******************************************************************
// File Name:					DatasMgr.cs
// Description:					DatasMgr class 
// Author:						wuwei
// Date:						2016.12.30
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using GJson;

namespace FW.ResMgr
{
    class JsonValue
    {
        private GJsonData m_data;
        public JsonValue(GJsonData data)
        {
            this.m_data = data;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public bool AsBool()
        {
            return this.m_data.AsBool();
        }

        public bool[] AsBools()
        {
            return this.m_data.AsBools();
        }

        public int AsInt()
        {
            return this.m_data.AsInt();
        }

        public int[] AsInts()
        {
            return this.m_data.AsInts();
        }

        public Int64 AsInt64()
        {
            return this.m_data.AsInt64();
        }

        public Int64[] AsInt64s()
        {
            return this.m_data.AsInt64s();
        }

        public double AsDouble()
        {
            return this.m_data.AsDouble();
        }

        public double[] AsDoubles()
        {
            return this.m_data.AsDoubles();
        }

        public float AsFloat()
        {
            return this.m_data.AsFloat();
        }

        public float[] AsFloats()
        {
            return this.m_data.AsFloats();
        }

        public string AsString()
        {
            return this.m_data.AsString();
        }

        public string[] AsStrings()
        {
            return this.m_data.AsStrings();
        }

        public Vector2 AsVector2()
        {
            float[] value = this.AsFloats();
            if (value.Length != 2)
                return Vector2.zero;
            return new Vector2(value[0], value[1]);
        }

        public Vector3 AsVector3()
        {
            float[] value = this.AsFloats();
            if (value.Length != 3)
                return Vector3.zero;
            return new Vector3(value[0], value[1], value[2]);
        }

        public Vector4 AsVector4()
        {
            float[] value = this.AsFloats();
            if (value.Length != 4)
                return Vector4.zero;
            return new Vector4(value[0], value[1], value[2], value[3]);
        }
    }

    class JsonItem
    {
        private GJsonDict m_data;
        public JsonItem(GJsonDict data)
        {
            this.m_data = data;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public JsonValue Get(string key)
        {
            string[] keys = key.Split('/');

            GJsonDict data = this.m_data;

            foreach (string value in keys)
            {
                GJsonData subData;
                if (data.TryGetValue(value, out subData))
                {
                    if (subData is GJsonDict)
                        data = subData as GJsonDict;
                    else
                        return new JsonValue(subData);
                }
                else
                    return null;
            }
            return new JsonValue(data);
        }
    }

    class JsonConfig
    {
        private GJsonDict m_data;
        public JsonConfig(string file)
        {
            TextAsset text = ResLoad.Load<TextAsset>(file);
            if(text != null)
                this.m_data = GJson.GJson.Parse(text.text);
        }

        public GJsonDict Data { get { return this.m_data; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private bool HasItem(string key)
        {
            GJsonData data;
            return m_data.TryGetValue(key, out data);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public JsonItem GetJsonItem(string key)
        {
            if (this.m_data == null || HasItem(key) == false) return null;
            return new JsonItem(m_data[key] as GJsonDict);
        }
    }

    static class DatasMgr
    {
        private static Dictionary<string, JsonConfig> sm_cfgs;
        private static Dictionary<int, string> sm_cfgIndexs;

        static DatasMgr()
        {
            sm_cfgs = new Dictionary<string, JsonConfig>();
            sm_cfgIndexs = new Dictionary<int, string>();
            sm_cfgIndexs.Add(102, "RoleCfg");               //角色
            sm_cfgIndexs.Add(210, "WeaponCfg");             //武器
            sm_cfgIndexs.Add(220, "WeaponPartCfg");         //配件
            sm_cfgIndexs.Add(230, "WeaponSuitCfg");         //套装
            sm_cfgIndexs.Add(104, "MedalCfg");              //勋章
            sm_cfgIndexs.Add(204, "ItemCfg");               //道具
            sm_cfgIndexs.Add(206, "ItemCfg");               //货币
            sm_cfgIndexs.Add(207, "ItemCfg");               //宝箱
            sm_cfgIndexs.Add(208, "ExChangeItemCfg");       //兑换实物
            sm_cfgIndexs.Add(110, "FWMTaskCfg");            //任务数据
            sm_cfgIndexs.Add(120, "FWMLevelCfg");           //关卡数据
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        //角色配置
        public static JsonConfig RoleCfg { get { return sm_cfgs["RoleCfg"]; } }
        //武器配置
        public static JsonConfig WeaponCfg { get { return sm_cfgs["WeaponCfg"]; } }
        //武器配件配置
        public static JsonConfig WeaponPartCfg { get { return sm_cfgs["WeaponPartCfg"]; } }
        //武器套装配置
        public static JsonConfig WeaponSuitCfg { get { return sm_cfgs["WeaponSuitCfg"]; } }
        //勋章配置
        public static JsonConfig MedalCfg { get { return sm_cfgs["MedalCfg"]; } }
        //资源配置
        public static JsonConfig ResCfg { get { return sm_cfgs["ResCfg"]; } }
        //物品配置
        public static JsonConfig ItemCfg { get { return sm_cfgs["ItemCfg"]; } }
        //兑换物品配置
        public static JsonConfig ExChangeItemCfg { get { return sm_cfgs["ExChangeItemCfg"]; } }
        //交易类型配置
        public static JsonConfig WTradeTypeCfg { get { return sm_cfgs["WeaponTradetypeCfg"]; } }
        public static JsonConfig PTradeTypeCfg { get { return sm_cfgs["PartTradetypeCfg"]; } }
        public static JsonConfig ITradeTypeCfg { get { return sm_cfgs["ItemTradetypeCfg"]; } }
		//统计数据
        public static JsonConfig CareerCfg { get { return sm_cfgs["CareerCfg"]; } }
		//任务数据配置
        public static JsonConfig FWMTaskCfg { get { return sm_cfgs["FWMTaskCfg"]; } }
        //关卡数据配置
        public static JsonConfig FWMLevelCfg { get { return sm_cfgs["FWMLevelCfg"]; } }
        //邮件模版
        public static JsonConfig MailTemplateCfg { get { return sm_cfgs["FWMailTemplateCfg"]; } }
        //武器熟练度
        public static JsonConfig ProrficiencyCfg { get { return sm_cfgs["FWProficiencyCfg"]; } }
        //充值档位表
        public static JsonConfig FWRecharge { get { return sm_cfgs["FWRecharge"]; } }
        //摇奖奖次图标
        public static JsonConfig FWLckyGroup { get { return sm_cfgs["FWLckyGroup"]; } }
        //摇奖组合物品
        public static JsonConfig FWMSlotCfg { get { return sm_cfgs["FWMSlotCfg"]; } }
        //杂项配置表
        public static JsonConfig FWMDefaultCfg { get { return sm_cfgs["FWMDefaultCfg"]; } }
        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Init()
        {
            sm_cfgs.Add("RoleCfg", new JsonConfig("config/FWRoleCfg.json"));
            sm_cfgs.Add("WeaponCfg", new JsonConfig("config/FWweaponCfg.json"));
            sm_cfgs.Add("WeaponPartCfg", new JsonConfig("config/FWweaponPartCfg.json"));
            sm_cfgs.Add("WeaponSuitCfg", new JsonConfig("config/FWweaponSuitCfg.json"));
            sm_cfgs.Add("MedalCfg", new JsonConfig("config/FWMedalCfg.json"));
            sm_cfgs.Add("ResCfg", new JsonConfig("resConfig/FWResourceCfg.json"));
            sm_cfgs.Add("ItemCfg", new JsonConfig("config/FWItemCfg.json"));
            sm_cfgs.Add("ExChangeItemCfg", new JsonConfig("config/share_cfg/FWExChangeItems.json"));
            sm_cfgs.Add("WeaponTradetypeCfg", new JsonConfig("config/FWMweponsCfg.json"));
            sm_cfgs.Add("PartTradetypeCfg", new JsonConfig("config/FWMpartsCfg.json"));
            sm_cfgs.Add("ItemTradetypeCfg", new JsonConfig("config/FWMitemsCfg.json"));
			sm_cfgs.Add("CareerCfg", new JsonConfig ("config/FWCareerCfg"));
			sm_cfgs.Add("FWMTaskCfg", new JsonConfig("config/FWMTaskCfg.json"));
            sm_cfgs.Add("FWMLevelCfg", new JsonConfig("config/FWMLevelCfg.json"));
            sm_cfgs.Add("FWMailTemplateCfg", new JsonConfig("config/FWMailTemplateCfg.json"));
            sm_cfgs.Add("FWProficiencyCfg", new JsonConfig("config/FWProficiencyCfg.json"));
            sm_cfgs.Add("FWRecharge", new JsonConfig("config/FWRecharge.json"));
            sm_cfgs.Add("FWLckyGroup", new JsonConfig("config/FWMSlotRewardCfg"));
            sm_cfgs.Add("FWMSlotCfg", new JsonConfig("config/FWMSlotCfg"));
            sm_cfgs.Add("FWMDefaultCfg", new JsonConfig("config/FWMDefaultCfg"));
        }

        //获取配置文件
        public static JsonConfig GetConfig(int id)
        {
            int index = id / 1000000;
            if(sm_cfgIndexs.ContainsKey(index) == false)
            {
                Debug.LogFormat("DatasMgr::GetConfig id error:"+id);
                return null;
            }
            JsonConfig jcfg;
            sm_cfgs.TryGetValue(sm_cfgIndexs[index], out jcfg);
            return jcfg;
        }

        //获取配置item
        public static JsonItem GetJsonItem(int id)
        {
            JsonConfig cfg = GetConfig(id);
            if (cfg == null) return null;
            return cfg.GetJsonItem(id.ToString());
        }

        //从资源表中获取资源
        public static string[] GetResList(int id)
        {
            if (id <= 0) return null;
            return ResCfg.GetJsonItem(id.ToString()).Get("resource").AsStrings();
        }

        //从资源表中获取资源
        public static string GetRes(int id)
        {
            string[] strs = GetResList(id);
            return (strs == null || strs.Length == 0) ? string.Empty : strs[0];
        }
    }
}