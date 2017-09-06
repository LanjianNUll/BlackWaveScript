//******************************************************************
// File Name:					LevelDataMgr.cs
// Description:					LevelDataMgr class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       关卡配置文件管理类
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GJson;
using FW.ResMgr;

namespace FW.Task
{
    /// <summary>
    /// 关卡配置数据的实体类
    /// </summary>
    public class LevelItem
    {
        private int m_id;
        //关卡名称
        private string m_name;
        //地图(部分路径)
        private string m_terrain;
        //刷怪的分布方式
        private EnemyDistributeType m_enemyDistribute;
        //刷怪的方式
        private EnemyAppearType m_enemyAppear;
        //参数1
        private int[] m_param1;
        //参数2
        private int[] m_param2;

        public int ID { get { return m_id; } }
        public string Name { get { return m_name; } }
        public string Terrain { get { return m_terrain; } }
        public EnemyDistributeType EnemyDistribute { get { return m_enemyDistribute; } }
        public EnemyAppearType EnemyAppear { get { return m_enemyAppear; } }
        public int[] Param1 { get { return m_param1; } }
        public int[] Param2 { get { return m_param2; } }

        internal void Init(int id, JsonItem item)
        {
            this.m_id = id;
            this.m_name = item.Get("name").AsString();
            this.m_terrain = item.Get("terrain").AsString();
            this.m_enemyDistribute = (EnemyDistributeType)item.Get("enemyDistributeType").AsInt();
            this.m_enemyAppear = (EnemyAppearType)item.Get("enemyAppearType").AsInt();

            this.m_param1 = item.Get("param1").AsInts();
            this.m_param2 = item.Get("param2").AsInts();
        }
    }

    /// <summary>
    /// 关卡配置文件管理类
    /// </summary>
    public class LevelDataMgr
    {

        public static readonly LevelDataMgr Instance = new LevelDataMgr();
        //关卡数据
        private Dictionary<int, LevelItem> m_levelDic = new Dictionary<int, LevelItem>();

        //--------------------------------------
        //public 拿到其中一个关卡数据
        //--------------------------------------
        public LevelItem GetLevelItem(int id)
        {
            if (m_levelDic.ContainsKey(id))
            {
                return m_levelDic[id];
            }
            JsonItem item = DatasMgr.FWMLevelCfg.GetJsonItem(id.ToString());
            if (item == null) return null;
            LevelItem level = new LevelItem();
            level.Init(id, item);
            m_levelDic.Add(id,level);
            return level;
        }
    }
}
