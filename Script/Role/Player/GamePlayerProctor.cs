//******************************************************************
// File Name:					GamePlayerProctor
// Description:					GamePlayerProctor class 
// Author:						lanjian
// Date:						12/29/2016 5:57:44 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using Network.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FW.Player
{
    /// <summary>
    /// 好友管理类
    /// </summary>
    class GamePlayerProctor
    {
        private Dictionary<int, Gameplayer> m_players;

        public GamePlayerProctor()
        {
            this.m_players = new Dictionary<int, Gameplayer>();
        }
        //--------------------------------------
        //private 
        //--------------------------------------
        private void DeleteAllFriends()
        {
            this.m_players.Clear();
        }

        private void CreateFriends(List<DataObj> datas)
        {
            if (datas == null || datas.Count == 0) return;
            foreach (DataObj friendData in datas)
            {
                Gameplayer friend = new Gameplayer(friendData);
                if (friend == null) continue;
                this.m_players.Add(friend.PID, friend);
            }
        }

        //请求好友列表的返回
        private void OnRequestFriend(DataObj data)
        {
            if (data == null) return;
            this.DeleteAllFriends();
            List<DataObj> datas = data.GetDataObjList("infos");
            this.CreateFriends(datas);
            //发送初始化好友列表消息
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.Friend_inited, new Event.EventArg());
        }

        //添加好友的返回
        private void OnAddFriend(DataObj data)
        {
            if (data == null) return;
            int ret = data.GetUInt16("ret");// 0 成功   1 找不到该玩家 2该玩家已经是好友了  3好友数量满了 */
            Gameplayer player = null;
            if (ret == 0)
            {
                int pid = data.GetInt32("pid");
                string name = data.GetString("name");
                int level = data.GetInt32("level");
                player = new Gameplayer(pid, name, level);
                this.m_players.Add(player.PID, player);
            }
            Event.FWEvent.Instance.Call(Event.EventID.Friend_add, new Event.EventArg(player, ret));
        }

        //删除好友的返回
        private void OnRemoveFriend(DataObj data)
        {
            if (data == null) return;
            int ret = data.GetUInt16("ret");  /* 0 成功   1 失败*/
            Gameplayer player = null;
            if (ret == 0)
            {
                int pid = data.GetInt32("pid");
                if (this.m_players.TryGetValue(pid, out player))
                {
                    this.m_players.Remove(pid);
                }
            }
            Event.FWEvent.Instance.Call(Event.EventID.Friend_delete, new Event.EventArg(player, ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            NetDispatcherMgr.Inst.Regist(Commond.Request_Friend_list_back, OnRequestFriend);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Add_Friend_back, OnAddFriend);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Remove_Friend_back, OnRemoveFriend);

            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            Network.NetMgr.Instance.Request(Commond.Request_Friend_list, data);
        }

        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Friend_list_back, OnRequestFriend);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Add_Friend_back, OnAddFriend);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Remove_Friend_back, OnRemoveFriend);
            this.DeleteAllFriends();
        }

        //获取好友列表
        public List<Gameplayer> GetFriendList()
        {
            Gameplayer[] GParr = new Gameplayer[this.m_players.Count];
            this.m_players.Values.CopyTo(GParr, 0);
            List<Gameplayer> list = new List<Gameplayer>(GParr);
            return list;
        }

        //添加好友
        public void AddFriend(string playName)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["name"] = playName;
            Network.NetMgr.Instance.Request(Commond.Request_Add_Friend, data);
        }

        //删除好友
        public void RemoveFriend(int pid)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["pid"] = pid;
            Network.NetMgr.Instance.Request(Commond.Request_Remove_Friend,data);
        }
    }
}
