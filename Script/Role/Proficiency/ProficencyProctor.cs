//******************************************************************
// File Name:					ProficencyProctor
// Description:					ProficencyProctor class 
// Author:						lanjian
// Date:						4/14/2017 5:47:00 PM
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
    class ProficencyProctor
    {
        private Dictionary<int,Proficiency> m_ProficDic;
        private Dictionary<int, Proficiency> m_TotolProficDic;

        public ProficencyProctor()
        {
            m_ProficDic = new Dictionary<int, Proficiency>();
            m_TotolProficDic = new Dictionary<int, Proficiency>();
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void OnRequestProfBack(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0) return;
            this.m_ProficDic.Clear();
            foreach (DataObj dataObj in data.GetDataObjList("proficiency"))
            {
                CreateProficency(dataObj);
            }
        }

        private void CreateProficency(DataObj data)
        {
            int id = data.GetInt32("x");
            int value = data.GetInt32("y");
            JsonItem jsonItem = DatasMgr.ProrficiencyCfg.GetJsonItem(id.ToString());
            m_ProficDic.Add(id,new Proficiency(jsonItem,id,value));
        }

        private void ReadFromJson()
        {
            this.m_TotolProficDic.Clear();
            for (int i = 0; i < DatasMgr.ProrficiencyCfg.Data.Count-1; i++)
            {
                int id = 1 + i;
                JsonItem jsonItem = DatasMgr.ProrficiencyCfg.GetJsonItem(id.ToString());
                this.m_TotolProficDic.Add(id, new Proficiency(jsonItem, id, 0));
            }
            //枪械类型id不统一 ，读表不顺序
            JsonItem jsonItem1 = DatasMgr.ProrficiencyCfg.GetJsonItem(9.ToString());
            this.m_TotolProficDic.Add(9, new Proficiency(jsonItem1, 9, 0));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            //从配置表中读取
            ReadFromJson();
            NetDispatcherMgr.Inst.Regist(Commond.Request_Proficency_Data_back, OnRequestProfBack);
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Proficency_Data, data);
        }

        //获取配置武器熟练度
        public List<Proficiency> GetWProficency()
        {
            List<Proficiency> list = new List<Proficiency>();
            foreach (int item in this.m_ProficDic.Keys)
            {
                m_TotolProficDic[item] = m_ProficDic[item];
            }
            foreach (Proficiency item in this.m_TotolProficDic.Values)
            {
                list.Add(item);
            }
            return list;
        }

        public void Dispose()
        {
            this.m_ProficDic.Clear();
            this.m_TotolProficDic.Clear();
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Proficency_Data_back, OnRequestProfBack);
        }
    }
}
