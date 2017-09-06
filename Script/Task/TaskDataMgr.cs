//******************************************************************
// File Name:					TaskDataMgr.cs
// Description:					TaskDataMgr class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       任务配置管理类
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
    /// 任务配置数据的实体类
    /// </summary>
    public class TaskData
    {
        private int m_id;
        //任务名称
        private string m_name;
        //关卡id
        private int m_levelID;
        //进入条件id
        private int m_enterCondition;
        //关卡结束条件类型
        private TaskCompleteType m_endConditionType;
        //关卡结束条件参数
        private int m_endConditionParam;
        //奖励物品类型
        private int[] m_itemIDs;
        //奖励物品数量
        private int[] m_itemAmounts;

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return m_id; } }
        public string Name { get { return m_name; } }
        public int LevelID { get { return m_levelID; } }
        public int EnterCondition { get { return m_enterCondition; } }
        public TaskCompleteType EndConditionType { get { return m_endConditionType; } }
        public int EndConditionParam { get { return m_endConditionParam; } }
        public int[] ItemIDs { get { return m_itemIDs; } }
        public int[] ItemAmounts { get { return m_itemAmounts; } }

        internal void Init(int id, JsonItem item)
        {
            this.m_id = id;
            this.m_name = item.Get("name").AsString();
            this.m_levelID = item.Get("levelID").AsInt();
            this.m_enterCondition = item.Get("enterCondition").AsInt();
            this.m_endConditionType = (TaskCompleteType)item.Get("endConditionType").AsInt();
            this.m_endConditionParam = item.Get("endConditionParam").AsInt();
            this.m_itemIDs = item.Get("itemID").AsInts();
            this.m_itemAmounts = item.Get("itemAmount").AsInts();
        }
    }
    /// <summary>
    /// 加载任务配置文件
    /// </summary>
    public class TaskDataMgr
    {
        public static readonly TaskDataMgr Instance = new TaskDataMgr();
        //关卡数据
        private static Dictionary<int, TaskData> m_taskDic;

        //--------------------------------------
        //private
        //--------------------------------------
        static TaskDataMgr()
        {
            m_taskDic = new Dictionary<int, TaskData>();
            Init();
        }

        private static void Remove()
        {
            m_taskDic.Clear();
        }

        private static void Init()
        {
            JsonConfig jsonconfig = DatasMgr.FWMTaskCfg;
            for (int i = 0; i < jsonconfig.Data.Count; i++)
            {
                TaskData task = new TaskData();
                int id = 110000000 + i + 1;
                task.Init(id, jsonconfig.GetJsonItem(id.ToString()));
                m_taskDic.Add(id, task);
            }
        }
        //--------------------------------------
        //public
        //--------------------------------------
        //根据任务id去配置表去具体数据
        public TaskData GetTaskItem(int id)
        {
            TaskData taskData = null;
            if (m_taskDic.TryGetValue(id, out taskData))
            {
                return taskData;
            }
            return null;
        }

        public TaskData GetDefaultTaskItem()
        {
            return GetTaskItem(110000001);
        }

        public static void Dispose()
        {
            Remove();
        }
    }
}
