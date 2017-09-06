//******************************************************************
// File Name:					StatisticMgr
// Description:					StatisticMgr class 
// Author:						lanjian
// Date:						3/17/2017 5:11:41 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Role
{
    //生涯类型
    enum CarrerType
    {
        Other,
        PVE,
        PVP,
    }
    //UI显示类型
    enum ShowType
    {
        Unkonw,
        Honor,
        Statistic,
    }

    class StatisticProctor
    {
        private List<Statistic> m_careertics;
        private Dictionary<int, int> m_IdValueDic;
        public  StatisticProctor()
        {
            m_IdValueDic = new Dictionary<int, int>();
            m_careertics = new List<Statistic>();
        }
        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------v
        private void OnRequestStatistic(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0) return;
            m_careertics.Clear();
            foreach (DataObj d in data.GetDataObjList("statisticinfo"))
            {
                int key = d.GetInt32("id");
                int value = d.GetInt32("value");
                if (this.m_IdValueDic.ContainsKey(key))
                    this.m_IdValueDic[key] = value;
                else
                    m_IdValueDic.Add(key, value);
            }
            CreateStatistic();
        }

        //创建数据item
        private void CreateStatistic()
        {
            for (int i = 0; i < DatasMgr.CareerCfg.Data.Count; i++)
            {
                JsonItem jsonItem = DatasMgr.CareerCfg.GetJsonItem((i+1).ToString());
                this.m_careertics.Add(new Statistic(jsonItem,i));
            }
            foreach (Statistic item in this.m_careertics)
            {
                if (this.m_IdValueDic.ContainsKey(item.DataId))
                    item.Value = m_IdValueDic[item.DataId];
            }
        }

        //--------------------------------------
        //private 
        //--------------------------------------v

        public void Init()
        {
            NetDispatcherMgr.Inst.Regist(Commond.Request_Statistic_Data_back, OnRequestStatistic);

            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Statistic_Data, data);
        }

        public List<Statistic> GetDataList(ShowType type)
        {
            List<Statistic> list = new List<Statistic>();
            foreach (Statistic item in m_careertics)
            {
                if (item.SType == type)
                    list.Add(item);
            }
            return list;
        }

        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Statistic_Data_back, OnRequestStatistic);
            m_IdValueDic.Clear();
            m_careertics.Clear();
        }
    }
}
