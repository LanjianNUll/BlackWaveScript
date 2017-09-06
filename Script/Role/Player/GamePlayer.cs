//******************************************************************
// File Name:					Gameplayer
// Description:					Gameplayer class 
// Author:						lanjian
// Date:						12/29/2016 5:47:37 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using Network.Serializer;
using System;
using System.Collections.Generic;


namespace FW.Player
{
    /// <summary>
    /// 玩家类
    /// </summary>
    class Gameplayer
    {
        private int m_pid;              
        private string m_PlayerName;                        //用户名
        private int m_level;                                //等级
        private bool m_isOnline;
        private string m_unionName ="射击俱乐部会长";         //公会名称

        public Gameplayer(DataObj data)
        {
            this.m_pid = data.GetInt32("pid");
            this.m_PlayerName = data.GetString("name");
            this.m_level = data.GetInt32("level");
            this.m_isOnline = data.GetInt8("isOnline") == 1;
        }

        public Gameplayer(int pid, string name,int level)
        {
            this.m_pid = pid;
            this.m_PlayerName = name;
            this.m_level = level;
            this.m_isOnline = false;
        }        
        //--------------------------------------
        //properties 
        //--------------------------------------
        public int PID { get { return this.m_pid; } }
        public string PlayerName { get { return this.m_PlayerName; } }
        public int Level { get { return this.m_level; } }
        public bool IsOnline { get { return this.m_isOnline; } }
        public string UnionName { get { return this.m_unionName; } }
    }
}
