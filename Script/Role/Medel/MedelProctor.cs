//******************************************************************
// File Name:					MedelProctor
// Description:					MedelProctor class 
// Author:						lanjian
// Date:						3/29/2017 2:18:32 PM
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
    class MedelProctor
    {
        private Dictionary <int,Medel> m_Medels;
        private Dictionary<int, Medel> m_totalMedles;                  //读取配置表中的勋章

        public MedelProctor()
        {
            m_Medels = new Dictionary<int, Medel>();
            m_totalMedles = new Dictionary<int, Medel>();
        }
        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------

        private void ReadFromJson()
        {
            m_totalMedles.Clear();
            for (int i = 0; i < DatasMgr.MedalCfg.Data.Count; i++)
            {
                int id = 104001001 + i;
                JsonItem jsonItem = DatasMgr.MedalCfg.GetJsonItem(id.ToString());
                //将获得时间设为-1 标识
                this.m_totalMedles.Add(id, new Medel(jsonItem, 10400100+i,-1));
            }
        }

        private void OnRequestMedesBack(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0) return;
            this.m_Medels.Clear();
            foreach (DataObj d in data.GetDataObjList("emblem_list"))
            {
                CreateMedel(d);
            }
        }

        private void CreateMedel(DataObj data)
        {
            int id = data.GetInt32("id");
            int time = data.GetInt32("time");
            JsonItem jsonItem = DatasMgr.MedalCfg.GetJsonItem(id.ToString());
            this.m_Medels.Add(id,new Medel(jsonItem,id,time));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            //从配置表中读取
            ReadFromJson();
            NetDispatcherMgr.Inst.Regist(Commond.Request_Medel_Data_back, OnRequestMedesBack);
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Medel_Data, data);
        }
        
        public void Dispose()
        {
            this.m_Medels.Clear();
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Medel_Data_back, OnRequestMedesBack);
        }

        //获取勋章信息
        public List<Medel> GetMedelList()
        {
            List<Medel> list = new List<Medel>();
            foreach (int item in m_Medels.Keys)
            {
                m_totalMedles[item] = m_Medels[item];
            }
            foreach (Medel item in m_totalMedles.Values)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
