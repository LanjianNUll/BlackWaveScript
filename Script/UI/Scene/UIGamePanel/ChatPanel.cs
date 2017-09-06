//******************************************************************
// File Name:					ChatPanel
// Description:					ChatPanel class 
// Author:						lanjian
// Date:						3/21/2017 11:51:36 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    //聊天模块
    class ChatPanel
    {
        //--------------------------------------
        //private 
        //--------------------------------------

        private GameObject m_chatPanel;
        private Transform m_topTra;
        private Transform m_centerTra;
        private UIInput m_wordInput;
        private UIInput m_channelInput;
        private UIInput m_firendIdInput;

        private UITextList m_currentTextList;               //当前的输入框
        private int m_currentTab = 0;                       //0 频道， 1 私聊  2 公告
        //private GameObject m_newAnno;                       //新公告

        private UITextList[] m_textList = new UITextList[3];
        private GameObject m_flowPanel;                     //悬浮的框

        private string m_currendPlayerId;                   //当前点击玩家的ID
        private int m_pindaoID;                             //当前平道

        private void FindUI()
        {
            m_chatPanel = UISceneMgr.CurrScene.RootObj.transform.Find("centerandBottom").gameObject;
            m_topTra = m_chatPanel.transform.Find("top");
            m_centerTra = m_chatPanel.transform.Find("center");
            m_wordInput = m_centerTra.GetChild(0).Find("Inputword").GetChild(0).GetComponent<UIInput>();
            m_textList[0] = m_centerTra.GetChild(0).Find("channelChatWindow").GetChild(0).GetComponent<UITextList>();
            m_textList[1] = m_centerTra.GetChild(0).Find("PersonnalChatWindow").GetChild(0).GetComponent<UITextList>();
            m_textList[2] = m_centerTra.GetChild(0).Find("AnnochmentWindow").GetChild(0).GetComponent<UITextList>();

            m_channelInput = m_centerTra.GetChild(0).Find("InputPindao").GetChild(0).GetComponent<UIInput>();
            m_firendIdInput = m_centerTra.GetChild(0).Find("InputfirendName").GetChild(0).GetComponent<UIInput>();
            m_flowPanel = m_centerTra.GetChild(1).gameObject;

            Utility.Utility.GetUIEventListener(m_flowPanel.transform.Find("close")).onClick = OnCloseFlowPanel;
            Utility.Utility.GetUIEventListener(m_flowPanel.transform.Find("sendPersonal")).onClick = OnSendPrivateChat;
            Utility.Utility.GetUIEventListener(m_flowPanel.transform.Find("addFriend")).onClick = OnAddFriend;

            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("send").gameObject).onClick = OnSendBtnClick;
            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("turn").gameObject).onClick = OnChangeChannelClick;
            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("firend").gameObject).onClick = OnGetPersonIdClick;

            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("channelChatWindow")
                .GetChild(0).GetChild(0).gameObject).onClick = OnClickLabel;
            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("PersonnalChatWindow")
                .GetChild(0).GetChild(0).gameObject).onClick = OnClickLabel;
            Utility.Utility.GetUIEventListener(m_centerTra.GetChild(0).Find("AnnochmentWindow")
                .GetChild(0).GetChild(0).gameObject).onClick = OnClickLabel;
            Transform topTran = m_chatPanel.transform.Find("top");
            for (int i = 0; i < topTran.childCount; i++)
            {
                topTran.GetChild(i).name = i + "";
                Utility.Utility.GetUIEventListener(topTran.GetChild(i)).onClick = whichTab;
                NGUITools.SetActive(topTran.GetChild(i).GetChild(1).gameObject, false);
            }
            NGUITools.SetActive(topTran.GetChild(0).GetChild(1).gameObject,true);
        }

        private void whichTab(GameObject go)
        {
            whichTab(go.name);
        }

        private void whichTab(string str)
        {
            m_currentTab = int.Parse(str);
            m_currentTextList = m_textList[m_currentTab];
            for (int i = 0; i < m_topTra.childCount; i++)
            {
                NGUITools.SetActive(m_topTra.GetChild(i).GetChild(1).gameObject, false);
            }
            NGUITools.SetActive(m_topTra.GetChild(m_currentTab).GetChild(1).gameObject, true);
            //将红点隐藏（说明已读）
            ShowNotReadMsg(m_currentTab, false);
            NGUITools.SetActive(m_flowPanel, false);
            NGUITools.SetActive(m_centerTra.Find("chat/channelChatWindow").gameObject, false);
            NGUITools.SetActive(m_centerTra.Find("chat/PersonnalChatWindow").gameObject, false);
            NGUITools.SetActive(m_centerTra.Find("chat/AnnochmentWindow").gameObject, false);
            NGUITools.SetActive(m_centerTra.Find("chat/turn").gameObject, false);
            NGUITools.SetActive(m_centerTra.Find("chat/InputPindao").gameObject, false);
            NGUITools.SetActive(m_centerTra.Find("chat/InputfirendName").gameObject, false);
            if (m_currentTab == 0)
            {
                NGUITools.SetActive(m_centerTra.Find("chat/channelChatWindow").gameObject, true);
                NGUITools.SetActive(m_centerTra.Find("chat/turn").gameObject, true);
                NGUITools.SetActive(m_centerTra.Find("chat/InputPindao").gameObject, true);
                m_centerTra.Find("chat/currentpingdao")
                    .GetChild(0).GetComponent<UILabel>().text = "当前频道：" + m_pindaoID;
            }
            if (m_currentTab == 1)
            {
                NGUITools.SetActive(m_centerTra.Find("chat/InputfirendName").gameObject, true);
                NGUITools.SetActive(m_centerTra.Find("chat/PersonnalChatWindow").gameObject, true);
                m_centerTra.Find("chat/currentpingdao")
                    .GetChild(0).GetComponent<UILabel>().text = "当前私聊频道";
                if (!string.IsNullOrEmpty(m_currendPlayerId))
                    m_firendIdInput.value = m_currendPlayerId;
            }
            if (m_currentTab == 2)
            {
                NGUITools.SetActive(m_centerTra.Find("chat/AnnochmentWindow").gameObject, true);
                m_centerTra.Find("chat/currentpingdao")
                    .GetChild(0).GetComponent<UILabel>().text = "当前公告栏";
            }
        }

        private void OnClickLabel(GameObject go)
        {
            UILabel lbl = go.GetComponent<UILabel>();
            m_currendPlayerId = lbl.GetUrlAtPosition(UICamera.lastHit.point);
            //点击的名字为空或者点击的是自己的名字
            if (string.IsNullOrEmpty(m_currendPlayerId)
                || Role.Role.Instance().Name.Equals(m_currendPlayerId))
            {
                NGUITools.SetActive(m_flowPanel,false);
                return;
            }
            NGUITools.SetActive(m_flowPanel, true);
            m_flowPanel.transform.GetChild(1).GetComponent<UILabel>().text = m_currendPlayerId;
            
            float offset = Input.mousePosition.y - 550;
            m_flowPanel.transform.localPosition = new Vector3(m_flowPanel.transform.localPosition.x,
                -230+offset, m_flowPanel.transform.localPosition.z);
            Debug.Log("点击的文字"+ m_currendPlayerId + Input.mousePosition);
        }

        private void OnCloseFlowPanel(GameObject go)
        {
            NGUITools.SetActive(m_flowPanel, false);
        }

        //发送私聊
        private void OnSendPrivateChat(GameObject go)
        {
            NGUITools.SetActive(m_flowPanel, false);
            //切换到私聊
            whichTab("1");
            m_currentTab = 1;
            //显示Tab显示的
            List<GameObject> hideList = m_topTra.GetChild(0).GetComponent<UIToggledObjects>().activate;
            List<GameObject> showList = m_topTra.GetChild(1).GetComponent<UIToggledObjects>().activate;
            for (int i = 0; i < hideList.Count; i++)
            {
                NGUITools.SetActive(hideList[i],false);
            }
            for (int i = 0; i < showList.Count; i++)
            {
                NGUITools.SetActive(showList[i], true);
            }
            m_centerTra.Find("chat/currentpingdao").GetChild(0).GetComponent<UILabel>().text = "当前好友" + m_currendPlayerId;
        }

        //加好友回调
        private void OnFriendAddBack(Event.EventArg args)
        {
            int ret = (int)args[1];
            if (ret == 0)
                Utility.Utility.NotifyStr(((Player.Gameplayer)args[0]).PlayerName+"已经是你的好友了！！");
            if (ret == 1)
                Utility.Utility.NotifyStr("没有该玩家，请重新输入!!!");
            if (ret == 2)
                Utility.Utility.NotifyStr("该玩家已经是好友了");
            if (ret == 3)
                Utility.Utility.NotifyStr("好友数量满了");
        }

        //加为好友
        private void OnAddFriend(GameObject go)
        {
            NGUITools.SetActive(m_flowPanel, false);
            Utility.Utility.NotifyStr(m_currendPlayerId +"好友请求");
            if (string.IsNullOrEmpty(m_currendPlayerId)) return;
            //请求加好友
            Role.Role.Instance().GamePlayerProctor.AddFriend(m_currendPlayerId);
        }

        //发送聊天
        private void OnSendBtnClick(GameObject go)
        {
            //隐藏悬浮面板
            NGUITools.SetActive(m_flowPanel, false);
            if (!string.IsNullOrEmpty(m_wordInput.value))
            {
                ChatMode mode = ChatMode.ChannelChat;
                if (m_currentTab == 0)
                {
                    mode = ChatMode.ChannelChat;
                    ChatMgr.Instance.RequestSendChat(mode, m_wordInput.value);
                }
                if (m_currentTab == 1)
                {
                    if (!string.IsNullOrEmpty(m_firendIdInput.value))
                    {
                        m_currendPlayerId = m_firendIdInput.value;
                    }
                    if (string.IsNullOrEmpty(m_currendPlayerId))
                    {
                        Utility.Utility.NotifyStr("未选择好友！！！");
                        return;
                    }
                    mode = ChatMode.PrivateChat;
                    ChatMgr.Instance.RequestSendChat(ChatMode.PrivateChat, m_wordInput.value, m_currendPlayerId);
                }

                if (m_currentTab == 2)
                {
                    mode = ChatMode.SystemNotice;
                    Utility.Utility.NotifyStr("公告不支持聊天！！");
                }
            }
        }

        //查找好友Id
        private void OnGetPersonIdClick(GameObject go)
        {
            if (!string.IsNullOrEmpty(m_firendIdInput.value))
            {
                m_currendPlayerId = m_firendIdInput.value;
                Utility.Utility.NotifyStr("正在查找+" + m_firendIdInput.value);
            }
            else
            {
                Utility.Utility.NotifyStr("请输入你要私聊的好友ID!!");
            }
        }

        private void OnChangeChannelClick(GameObject go)
        {
            int currentChannetId = -1;
            if (!int.TryParse(m_channelInput.value, out currentChannetId))
            {
                Utility.Utility.NotifyStr("输入的频道ID有错误");
                return;
            }
            Chat.ChatMgr.Instance.RequestJoinChannel(currentChannetId);
        }

        private void ShowInChatWindow(int whichMode,string name, string content)
        {
            m_currentTextList = m_textList[whichMode];
            string str = this.DoWithCanClickStr(name) + content;
            m_currentTextList.Add(str);
        }

        private void OnReciveMsg(Event.EventArg args)
        {
            ChatItem item = (ChatItem)args[0];
            int whickMode = 0;
            if (item.Mode == ChatMode.ChannelChat)
            {
                whickMode = 0;
                if (m_currentTab != whickMode)//显示未读
                    ShowNotReadMsg(whickMode);
            }

            if (item.Mode == ChatMode.PrivateChat)
            {
                whickMode = 1;
                if (m_currentTab != whickMode)//显示未读
                    ShowNotReadMsg(whickMode);
            }

            if (item.Mode == ChatMode.SystemNotice)
            {
                whickMode = 2;
                if (m_currentTab != whickMode)//显示未读
                    ShowNotReadMsg(whickMode);
            }
            ShowInChatWindow(whickMode, item.FromPlayer.Name, item.Content);
            //判断是否是自己的消息 ，飘字
            if (item.FromPlayer.Name.Equals(Role.Role.Instance().Name))
            {
                //Utility.Utility.NotifyStr("发送成功");
                m_wordInput.value = "";
                m_wordInput.isSelected = false;
            }
        }

        //显示未读的消息红点
        private void ShowNotReadMsg(int which,bool isShow = true)
        {
            NGUITools.SetActive(m_topTra.GetChild(which).Find("dot").gameObject, isShow);
        }

        private void OnSentError(Event.EventArg args)
        {
            //1.没有找到玩家, 2.发送内容过长
            if((int)args[0]==1)
                Utility.Utility.NotifyStr("没有该玩家，请重新输入!!");
            if((int)args[0]==2)
                Utility.Utility.NotifyStr("发送内容过长!!");
        }

        private void OnJoinchannelBack(Event.EventArg args)
        {
            /* 0成功加入频道,  1:频道人数已满  2. 没有找到频道 3.你已经在该频道中*/
            int ret = (int)args[0];
            if (ret == 0)
            {
                m_pindaoID = Chat.ChatMgr.Instance.CurrentChannel;
                m_centerTra.Find("chat/currentpingdao").GetChild(0).GetComponent<UILabel>().text = "当前频道："+
                   m_pindaoID.ToString();
                Utility.Utility.NotifyStr("切换频道成功");
            }
            if (ret ==1)
                Utility.Utility.NotifyStr("频道人数已满,请选择其他频道！！");
            if (ret == 2)
                Utility.Utility.NotifyStr("没有找到频道");
            if (ret == 3)
                Utility.Utility.NotifyStr("你已经在该频道中");
        }

        //返回url可点击的字段
        private string DoWithCanClickStr(string canClickStr)
        {
            return DoWithNameStr("[url=" + canClickStr + "]" + canClickStr + "[/url]");
        }

        //给lable设置颜色
        private string DoWithColorStr(string Str,string color = "")
        {
            return "[" + color + "]" + Str + "[-]";
        }

        //处理用户名
        private string DoWithNameStr(string name)
        {
            return "【" + name + "】";
        }

        private void ChatRecordShow()
        {
            List<ChatItem> channelList;
            channelList = ChatMgr.Instance.GetChatList(ChatMode.ChannelChat);
            foreach (var item in channelList)
            {
                ShowInChatWindow(0, item.FromPlayer.Name, item.Content);
            }
            List<ChatItem> personList;
            personList = ChatMgr.Instance.GetChatList(ChatMode.PrivateChat);
            foreach (var item in personList)
            {
                ShowInChatWindow(1, item.FromPlayer.Name, item.Content);
            }
            List<ChatItem> SystemList;
            SystemList = ChatMgr.Instance.GetChatList(ChatMode.SystemNotice);
            foreach (var item in SystemList)
            {
                ShowInChatWindow(2, item.FromPlayer.Name, item.Content);
            }
        }

        private void AdjustWidthUI()
        {
            float screenWidth = Screen.width;
            float ratio = 800f / 1080f;
            m_centerTra.GetChild(0).Find("InputPindao").GetComponent<UISprite>().width = (int)(screenWidth * ratio);
            float ratio2 = 950f / 1080f;
            m_centerTra.GetChild(0).Find("Inputword").GetComponent<UISprite>().width = (int)(screenWidth * ratio2);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Chat_ReceiveInfoNotify, OnReciveMsg);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Chat_SendChatError, OnSentError);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Chat_JoinChannelBack, OnJoinchannelBack);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Friend_add, OnFriendAddBack);
            Debug.Log("聊天初始化");
            FindUI();
            //调整宽屏UI显示
            //AdjustWidthUI();
            //请求默认的频道
            ChatMgr.Instance.RequestJoinRandomChannel();
            //拉取聊天记录
            ChatRecordShow();
            //第一次显示的网是
            whichTab("0");
            //是否是好友列表过来的私聊
            if (Role.Role.Instance().ToGamePrivateChat != null)
            {
                m_currendPlayerId = Role.Role.Instance().ToGamePrivateChat.PlayerName;
                OnSendPrivateChat(null);
            }
        }

        public void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Chat_ReceiveInfoNotify, OnReciveMsg);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Chat_SendChatError, OnSentError);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Chat_JoinChannelBack, OnJoinchannelBack);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Friend_add, OnFriendAddBack);
            //情空聊天
            if (m_currentTextList!=null)
                m_currentTextList.Clear();
            for (int i = 0; i < m_textList.Length; i++)
            {
                m_textList[i].Clear();
            }
            //将私聊玩家设置为空
            Role.Role.Instance().ToGamePrivateChat = null;
        }
    }
}
