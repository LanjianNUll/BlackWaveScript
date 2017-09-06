//******************************************************************
// File Name:					PersonImage
// Description:					PersonImage class 
// Author:						lanjian
// Date:						3/20/2017 4:59:01 PM
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
    //人物形象
    class PersonImage
    {
        private int m_key;
        private string m_name;
        private string m_getWay;                    //获取途径
        private string m_desc;                      //描述
        private bool m_Unlock;                      //是否解锁形象
        private string m_icon;                      //图片路径

        public PersonImage(JsonItem jsonItem, int id)
        {
            this.m_key = id;
            this.Init(jsonItem);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int Key { get { return this.m_key; } }
        public string Name { get { return this.m_name; }  }
        public string GetWay { get { return this.m_getWay; } }
        public string Desc { get { return this.m_desc; } }
        public bool IsUnLock { get { return this.m_Unlock; } }
        public string Icon { get { return this.m_icon; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(JsonItem jsonItem)
        {
            //临时处理
            string[] str = jsonItem.Get("desc").AsString().Split('：'); 
            
            this.m_name = jsonItem.Get("name").AsString();
            this.m_getWay = str[str.Length-1];
            this.m_desc = str[0].Substring(0, str[0].Length-8);
            this.m_Unlock = true;
            this.m_icon = DatasMgr.GetRes(jsonItem.Get("image").AsInt());
        }
    }
}
