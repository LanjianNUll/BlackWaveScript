//******************************************************************
// File Name:					Statistic
// Description:					Statistic class 
// Author:						lanjian
// Date:						3/17/2017 11:35:40 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Role
{
   
    /// <summary>
    /// 战绩数据
    /// </summary>
    class Statistic
    {
        private int m_key;
        private int m_dataId;
        private int m_value;
        private string m_showName;
        private CarrerType m_CarrerType;
        private ShowType m_ShowType;
        private String m_ShowIcon;

        public Statistic(JsonItem jsonItem,int id)
        {
            this.m_key = id;
            this.Init(jsonItem);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int Key { get { return this.m_key; }  }
        public int Value { get { return this.m_value; } set { this.m_value = value; } }
        public int DataId { get { return this.m_dataId; } }
        public string ShowName { get { return this.m_showName; } }
        public CarrerType CType { get { return this.m_CarrerType; } }
        public ShowType SType { get { return this.m_ShowType; } }
        public String SIcon { get { return this.m_ShowIcon; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(JsonItem jsonItem)
        {
            this.m_dataId = jsonItem.Get("dataId").AsInt();
            this.m_showName = jsonItem.Get("showName").AsString();
            this.m_CarrerType = (CarrerType)jsonItem.Get("careerType").AsInt();
            this.m_ShowType = (ShowType)jsonItem.Get("showType").AsInt();
            this.m_ShowIcon = DatasMgr.GetRes(jsonItem.Get("honorRes").AsInt());
        }
    }
}
