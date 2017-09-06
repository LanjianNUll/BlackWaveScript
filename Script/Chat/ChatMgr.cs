//******************************************************************
// File Name:					ChatMgr.cs
// Description:					ChatMgr Class
// Author:						yangyongfang
// Date:						2017.03.22
// Reference:
// Using:                       聊天管理类,服务器端通信,聊天数据临时存储,UI层通信
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FW.ResMgr;
using Network.Serializer;
using Network;

namespace FW.Chat
{
    /// <summary>
    /// 聊天模式
    /// </summary>
    enum ChatMode
    {
        SystemNotice=1,//系统公告
        ChannelChat=2,//频道聊天
        TeamChat=3,//队伍聊天
        PrivateChat=4,//私聊 
        DungeonTeamChat =5,//副本队伍聊天
        DungeonAllChat = 6//副本全体聊天
    }

    /// <summary>
    /// 聊天角色信息实体类
    /// </summary>
    class ChatPlayer
    {
        private int m_id;/*玩家id*/
        
        private string m_name;/*玩家名字*/

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return m_id; } }
        public string Name { get { return m_name; } set { m_name = value; } }

        internal void Init(DataObj item)
        {
            this.m_name = item.GetString("name");
            this.m_id = item.GetInt32("id");
        }
    }

    class ChatItem
    {
        private ChatMode m_mode;
        private ChatPlayer m_from;
        private ChatPlayer m_to;
        private string m_content;
        public ChatItem(ChatMode m_mode, ChatPlayer m_from, ChatPlayer m_to, string content)
        {
            this.m_mode = m_mode;
            this.m_from = m_from;
            this.m_to = m_to;
            this.m_content = content;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public ChatMode Mode { get { return m_mode; } }
        public ChatPlayer FromPlayer { get { return m_from; } }
        public ChatPlayer ToPlayer { get { return m_to; } }
        public string Content { get { return m_content; } }
    }

    /// <summary>
    /// 频道信息实体类
    /// </summary>
    class ChannelInfo
    {
        private int m_id;/*频道id*/
        private string m_name;/*频道名字字*/
        private int m_playerCount;/*当前人数*/
        private int m_maxCount;/*最高人数*/

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return m_id; } }
        public string Name { get { return m_name; } }
        public int PlayerCount { get { return m_playerCount; } }
        public int MaxCount { get { return m_maxCount; } }

        internal void Init(DataObj item)
        {
            this.m_name = item.GetString("name");
            this.m_id = item.GetInt32("id");
            this.m_playerCount = item.GetInt32("playerCount");
            this.m_maxCount = item.GetInt32("maxCount");
        }
    }
    class ChatMgr
    {
        public const int MAX_CHAT_COUNT = 50;
        public static ChatMgr Instance = new ChatMgr();
        //自动增长的消息id
        private int m_index=0;
        //存放所有消息
        private Dictionary<int, ChatItem> m_ChatInfoDic = new Dictionary<int, ChatItem>();
        //保存消息顺序
        private List<int> m_IndexList = new List<int>();
        //当前频道id
        private int m_currentChannel=0;
        private ChatMgr()
        {
            
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public int CurrentChannel { get { return m_currentChannel; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            //监听服务器端消息
            NetDispatcherMgr.Inst.Regist(Commond.Request_Send_Chat_back, ReceiveChatInfo);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Join_Channel_back, ResponseJoinChannel);

            RequestJoinRandomChannel();
        }

        #region 服务器通信
        /// <summary>
        /// 向服务器发送聊天消息
        /// </summary>
        /// <param name="mode">聊天模式</param>
        /// <param name="content">聊天内容</param>
        /// <param name="playerID">接收者id, 仅私聊有用</param>
        public void RequestSendChat(ChatMode mode,string content,string playerID="")
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["mode"] = (sbyte)mode;
            data["toPlayerName"] = playerID;
            data["content"] = content;
            NetMgr.Instance.Request(Commond.Request_Send_Chat, data);
        }

        //发送聊天消息的反馈,或者其他人发送消息
        public void ReceiveChatInfo(DataObj data)
        {// 0发送成功, 1.没有找到玩家, 2.发送内容过长  其中0还表示其他人发消息过来的通知
            if (data == null) return;
            int ret = data.GetUInt16("ret");
            if (ret == 0)
            {
                ChatMode mode = (ChatMode)data.GetInt8("mode");
                ChatPlayer fromPlayer = new ChatPlayer();
                fromPlayer.Init(data.GetDataObj("fromPlayer"));
                string content = data.GetString("content");
                ++m_index;
                ChatItem item = new ChatItem(mode, fromPlayer, null, content);
                m_ChatInfoDic.Add(m_index, item);
                m_IndexList.Add(m_index);
                FW.Event.FWEvent.Instance.Call(Event.EventID.Chat_ReceiveInfoNotify, new Event.EventArg(item));
            }
            else if (ret == 1|| ret == 2)
            {
                
                FW.Event.FWEvent.Instance.Call(Event.EventID.Chat_SendChatError, new Event.EventArg(ret));
            }
            else
            {
                Debug.LogWarning("ReceiveChatInfo,未识别的返回状态,ret:"+ret);
            }


        }

        /// <summary>
        /// 请求一个默认的频道id
        /// </summary>
        public void RequestJoinRandomChannel()
        {
            RequestJoinChannel(-1);
        }
        //向服务器发送 任务开始请求
        public void RequestJoinChannel(int channelID)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["channelID"] = channelID;
            NetMgr.Instance.Request(Commond.Request_Join_Channel, data);
        }

        //服务器反馈 任务开始请求
        public void ResponseJoinChannel(DataObj data)
        {
            if (data == null) return;
            int ret = data.GetUInt16("ret");
            if (ret == 0)/* 0成功加入频道,  1:频道人数已满  2. 没有找到频道 3.你已经在该频道中*/
            {
                int playerCount = data.GetInt32("playerCount");
                int id = data.GetInt32("channelID");
                if (id!= m_currentChannel)
                {
                    for (int i = 0; i < m_IndexList.Count; i++)
                    {
                        int index = m_IndexList[i];
                        ChatItem item = m_ChatInfoDic[index];
                        if (item.Mode == ChatMode.ChannelChat)
                        {
                            m_ChatInfoDic.Remove(index);
                        }
                    }
                }
                //Debug.LogError("成功加入频道,channel:" + ChatMgr.Instance.CurrentChannel);
                m_currentChannel = id;
                FW.Event.FWEvent.Instance.Call(Event.EventID.Chat_JoinChannelBack, new Event.EventArg(ret));
            }
            else if (ret == 1|| ret == 2|| ret == 3)
            {
                FW.Event.FWEvent.Instance.Call(Event.EventID.Chat_JoinChannelBack, new Event.EventArg(ret));
            }
            else
            {
                Debug.LogWarning("未识别的错误码，ret:"+ret);
            }
        }
        #endregion
        /// <summary>
        /// 根据聊天模式获取消息集合
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public List<ChatItem> GetChatList(ChatMode mode)
        {
            List<ChatItem> list = new List<ChatItem>();
            for (int i = 0; i < m_IndexList.Count; i++)
            {
                int index = m_IndexList[i];
                if (m_ChatInfoDic.ContainsKey(index))
                {
                    ChatItem item = m_ChatInfoDic[index];
                    if (item.Mode == mode || item.Mode == ChatMode.SystemNotice)
                    {
                        list.Add(item);
                    }
                }
                
            }
            //删除多余50行的更早数据
            if(list.Count> MAX_CHAT_COUNT)
            {
                list.RemoveRange(0, list.Count - MAX_CHAT_COUNT);
            }
            return list;
        }

        public void Dispose()
        {
            m_index = 0;
            m_ChatInfoDic.Clear();
            m_IndexList.Clear();
            m_currentChannel = 0;

            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Send_Chat_back, ReceiveChatInfo);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Join_Channel_back, ResponseJoinChannel);
        }
    }
}
