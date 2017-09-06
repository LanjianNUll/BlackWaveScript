//******************************************************************
// File Name:					PanelFriend
// Description:					PanelFriend class 
// Author:						lanjian
// Date:						1/5/2017 4:25:07 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Player;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace FW.UI
{
    class PanelFriend:PanelBase
    {
        private string m_firendItemPath = "UIRootPrefabs/ItemPrefabsForScrollView/firendItem";
        private GameObject m_FriendMain;
        private GameObject m_AddFriendArea;
        private GameObject m_DeleteFriendArea;
        private UILabel m_OnlineorTotalLabel;
        private UIInput m_AddFriendIdInput;

        private float m_ItemHeight;
        private Transform itemParent;                   //UIGrid
        private List<GameObject> m_FrendGoList = new List<GameObject>();
        private int m_TotalFriendsCount;
        private int m_OnlineFriendsCount;
        private string m_AddFriendId;

        private List<Gameplayer> m_friendList;
        private int m_CurrentClickNum = -1;             //当前点击了那个item
        private int m_PreClickNum = -1;
        private bool m_IsExistUnFold = false;           //当前是否存在展开的
       // private bool m_IsAnimFinish = true;           //动画是否完成，避免用户频繁点击bug

        protected PanelFriend()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/FirendPanel";
            this.panelType = PanelType.Firends;
        }

        public static PanelFriend Create()
        {
            return new PanelFriend();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        private void FindAllUI()
        {
            Transform currenTransform = PanelMgr.CurrPanel.RootObj.transform.Find("center");
            m_FriendMain = currenTransform.Find("friendMain").gameObject;
            m_AddFriendArea = currenTransform.Find("addFriendArea").gameObject;
            m_DeleteFriendArea = currenTransform.Find("deleteFriendArea").gameObject;

            m_OnlineorTotalLabel = m_FriendMain.transform.Find("friendCountAndOnline")
                .GetComponent<UILabel>();
            m_AddFriendIdInput = m_AddFriendArea.transform.Find("inputAddFriendsId").GetComponent<UIInput>();
            //获取好友列表
            GetFriendList();
            //注册一个方法 在线人数的
            GetAllItem();
            //填充数据
            FillDataToGO();
        }

        private void GetFriendList()
        {
            m_friendList = Role.Role.Instance().GamePlayerProctor.GetFriendList();
            m_OnlineFriendsCount = 0;
            m_TotalFriendsCount = 0;
            m_TotalFriendsCount = m_friendList.Count;
            for (int i = 0; i < m_friendList.Count; i++)
            {
                if (m_friendList[i].IsOnline)
                    m_OnlineFriendsCount++;
            }
        }

        private void GetAllItem()
        {
            itemParent = m_FriendMain.transform.Find("ScrollView_Frends").Find("UIGrid");
            m_ItemHeight = itemParent.GetComponent<UIGrid>().cellHeight;
            //先清空Item
            for (int i = 0; i < itemParent.childCount; i++)
            {
                GameObject.Destroy(itemParent.GetChild(0).gameObject);
            }
            m_FrendGoList.Clear();
            for (int i = 0; i < m_friendList.Count; i++)
            {
                GameObject currentItem = Utility.Utility.GetPrefabGameObject(m_firendItemPath, "item", itemParent);
                currentItem.transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                //改个名
                currentItem.name = "item" + i;
                BindItemListener(currentItem);
                m_FrendGoList.Add(currentItem);
            }
            //禁止滑动
            if (m_FrendGoList.Count<7)
            {
                m_FriendMain.transform.Find("ScrollView_Frends").GetComponent<UIScrollView>().enabled = false;
            }
        }

        private void FillDataToGO()
        {
            //显示在线人数比
            m_OnlineorTotalLabel.text = "[" + m_OnlineFriendsCount + "/" + m_TotalFriendsCount + "]";
            for (int i = 0; i < m_friendList.Count; i++)
            {
                FillOneFriendItem(m_FrendGoList[i]);
            }
        }

        private void FillOneFriendItem(GameObject go)
        {
            int index = int.Parse(go.name.Substring(4));
            //设置pid标志
            go.GetComponent<UILabel>().text = m_friendList[index].PID + "";
            go.transform.Find("firendName").GetComponent<UILabel>().text = m_friendList[index].PlayerName;
            if (m_friendList[index].IsOnline)
                go.transform.Find("Statetip").GetComponent<UISprite>().spriteName = "firends_account_full";
            else
                go.transform.Find("Statetip").GetComponent<UISprite>().spriteName = "firends_account_empty";
            go.transform.Find("HIdebg/level").GetComponent<UILabel>().text = m_friendList[index].Level + "";
            go.transform.Find("HIdebg/unionName").GetComponent<UILabel>().text = m_friendList[index].UnionName + "";
        }

        private void BindItemListener(GameObject go)
        {
            //绑定item点击 
            Utility.Utility.GetUIEventListener(go).onClick = ItemClick;
            Utility.Utility.GetUIEventListener(go.transform.Find("HIdebg").transform.Find("SendMsgBtn"))
                .onClick = ItemSendMsgBtnClick;
            Utility.Utility.GetUIEventListener(go.transform.Find("HIdebg").transform.Find("DeleteFriendBtn"))
                .onClick = ItemDeleteButtonBtnClick;
        }

        private void ItemClick(GameObject itemGo)
        {
            m_CurrentClickNum = int.Parse(itemGo.name.Substring(4));
            if (m_CurrentClickNum == m_PreClickNum)
            {
                this.ResetPosition();
                m_FrendGoList[m_PreClickNum].transform.Find("HIdebg").gameObject.SetActive(false);
                //同一个时在将上一个弄成-1，解决再次点击同一个不打开的bug
                m_PreClickNum = -1;
                return;
            }
            if (m_IsExistUnFold)
            {
               FoldItem();
            }
            UnFoldItem(itemGo);
        }

        private void ItemSendMsgBtnClick(GameObject item)
        {
            //跳转到Game视图的私聊
            PanelMgr.BackToMainPanel();
            FW.Event.FWEvent.Instance.Call(Event.EventID.MAIN_UI_GAME_BTN, new Event.EventArg(m_friendList[m_CurrentClickNum]));
        }

        private void ItemDeleteButtonBtnClick(GameObject item)
        {
            m_FriendMain.SetActive(false);
            m_DeleteFriendArea.SetActive(true);
        }

        //折叠
        private void FoldItem()
        {
            Transform itemParent = m_FriendMain.transform.Find("ScrollView_Frends").Find("UIGrid");
            itemParent.GetChild(m_PreClickNum).Find("HIdebg").gameObject.SetActive(false);
            float size = itemParent.GetComponent<UIGrid>().cellHeight;
            for (int i = m_PreClickNum + 1; i < m_FrendGoList.Count; i++)
            {
                GameObject currentItem = m_FrendGoList[i];
                currentItem.transform.localPosition += new Vector3(0, 2 * size, 0);
            }
            m_IsExistUnFold = false;
        }

        //展开
        private void UnFoldItem(GameObject go)
        {
            //go.transform.Find("HIdebg").gameObject.SetActive(true);
            NGUITools.SetActive(go.transform.Find("HIdebg").gameObject,true);
            Transform itemParent = m_FriendMain.transform.Find("ScrollView_Frends").Find("UIGrid");
            float size = itemParent.GetComponent<UIGrid>().cellHeight;
            for (int i = m_CurrentClickNum + 1; i < m_FrendGoList.Count; i++)
            {
                GameObject currentItem = m_FrendGoList[i];
                Vector3 regin = currentItem.transform.localPosition;
                Vector3 target =  currentItem.transform.localPosition + new Vector3(0, 0 - 2 * size, 0);
                GameObjectPositioAnim(currentItem, regin, target);
            }
            m_IsExistUnFold = true;
            m_PreClickNum = m_CurrentClickNum;
        }

        //重新刷新一下item的位置
        private void ResetPosition()
        {
            for (int i = 0; i < m_FrendGoList.Count; i++)
            {
                //改个名
                m_FrendGoList[i].name = "item" + i;
                //itemParent.GetChild(i).localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                Vector3 regin = m_FrendGoList[i].transform.localPosition;
                Vector3 target = m_FrendGoList[i].transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                GameObjectPositioAnim(m_FrendGoList[i], regin, target);
            }
            m_IsExistUnFold = false;
        }

        ///物体移动的动画
        private void GameObjectPositioAnim(GameObject go, Vector3 reginPosition, Vector3 targetPosition,
            float time = 0.5f, GameObject DestoryGo = null)
        {
            //m_IsAnimFinish = false;
            TweenPosition ta = go.GetComponent<TweenPosition>();
            if (ta == null)
            {
                ta = go.AddComponent<TweenPosition>();
            }
            ta.ResetToBeginning();
            ta.from = reginPosition;
            ta.to = targetPosition;
            ta.enabled = true;
            ta.duration = time;
            ta.PlayForward();
            //删除动画执行完 回调
            if (DestoryGo != null)
                EventDelegate.Set(ta.onFinished, delegate () {
                    m_FrendGoList.Remove(DestoryGo);
                    GameObject.DestroyImmediate(DestoryGo);
                    this.ResetPosition();
                });
        }

        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_FRIEND_ADDFRIEND_BTN, OnAddFriendBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_FRIEND_CONFIRM_ADDFRIEND_BTN, OnConfirmAddFriendBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_FRIEND_CANCEL_ADDFRIEND_BTN, OnCancelAddFriendBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_FRIEND_CONFIRM_DELETEFRIEND_BTN, OnConfirmDelteFriendBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_FRIEND_CANCEL_DELETEFRIEND_BTN, OnCancelDeleteFriendBtn);

            FW.Event.FWEvent.Instance.Regist(Event.EventID.Friend_add, OnFriendAddBack);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Friend_delete, OnFriendDeleteBack);
        }

        private void OnBackMainBtn()
        {
            PanelMgr.BackToMainPanel();
        }

        //响应添加好友的按钮
        private void OnAddFriendBtn()
        {
            NGUITools.SetActive(m_FriendMain, false);
            NGUITools.SetActive(m_AddFriendArea, true);
        }

        //响应确定添加好友的按钮
        private void OnConfirmAddFriendBtn()
        {
            string name = m_AddFriendIdInput.value;
            if (string.IsNullOrEmpty(name)) return;
            Role.Role.Instance().GamePlayerProctor.AddFriend(name);
        }

        //响应取消添加好友的按钮
        private void OnCancelAddFriendBtn()
        {
            NGUITools.SetActive(m_FriendMain, true);
            NGUITools.SetActive(m_AddFriendArea, false);
        }

        //响应确定删除好友的按钮
        private void OnConfirmDelteFriendBtn()
        {
            int pid = int.Parse(m_FrendGoList[m_CurrentClickNum].transform.GetComponent<UILabel>().text);
            Role.Role.Instance().GamePlayerProctor.RemoveFriend(pid);
            NGUITools.SetActive(m_FriendMain, true);
            NGUITools.SetActive(m_DeleteFriendArea, false);
        }

        //添加好友的回调
        private void OnFriendAddBack(FW.Event.EventArg args)
        {
            //0 成功   1 找不到该玩家 2该玩家已经是好友了  3好友数量满了))
            int ret = (int)args[1];
            if(ret == 0)
            {
                //弹框消失
                NGUITools.SetActive(m_FriendMain, true);
                NGUITools.SetActive(m_AddFriendArea, false);

                this.GetFriendList();
                //在ScrollView的顶部添加一个新的Item
                GameObject currentItem = Utility.Utility.GetPrefabGameObject(m_firendItemPath, "item", itemParent);
                currentItem.transform.localPosition = new Vector3(0, 0 , 0);
                //改个名
                currentItem.gameObject.name = "item0";
                //填充第一个新邮件的item
                this.BindItemListener(currentItem);
                //Debug.Log("接收到新邮件后的邮件数  "+m_MailList.Count);
                m_FrendGoList.Insert(0, currentItem);
                if (m_friendList.Count != 0)
                    this.FillOneFriendItem(currentItem);
                //每个都下移动多少
                for (int i = 1; i < m_FrendGoList.Count; i++)
                {
                    m_FrendGoList[i].transform.localPosition += new Vector3(0, -m_ItemHeight, 0);
                    m_FrendGoList[i].name = "item" + i;
                }
                //飘字通知
                Utility.Utility.NotifyStr("添加好友成功!!");
            }
            if(ret == 1)
                Utility.Utility.NotifyStr("找不到该玩家!!!");
            if (ret == 2)
                Utility.Utility.NotifyStr("该玩家已经是好友了");
            if (ret == 3)
                Utility.Utility.NotifyStr("好友数量满了");
        }

        //删除好友的回调
        private void OnFriendDeleteBack(FW.Event.EventArg args)
        {
            if ((int)args[1] == 0)
            {
                GameObjectPositioAnim(m_FrendGoList[m_CurrentClickNum],
                   m_FrendGoList[m_CurrentClickNum].transform.localPosition,
                    m_FrendGoList[m_CurrentClickNum].transform.localPosition + new Vector3(900, 0, 0), 0.2f,
                    m_FrendGoList[m_CurrentClickNum]);
                Utility.Utility.NotifyStr("删除好友成功！！");
                m_friendList.Remove((Gameplayer)args[0]);
            }
            else
            {
                Utility.Utility.NotifyStr("删除好友失败！！");
            }
        }

        //响应取消删除好友的按钮
        private void OnCancelDeleteFriendBtn()
        {
            NGUITools.SetActive(m_FriendMain, true);
            NGUITools.SetActive(m_DeleteFriendArea, false);
        }

        //正则验证
        private bool IsMathRex(string patternStr, string inputStr)
        {
            Regex regex = new Regex(patternStr);
            return regex.IsMatch(inputStr);
        }
        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            FindAllUI();
            ResgistEvents();
            if (m_friendList.Count == 0)
                Utility.Utility.NotifyStr("当前你没有任何好友！！！");
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_FRIEND_ADDFRIEND_BTN, OnAddFriendBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_FRIEND_CONFIRM_ADDFRIEND_BTN, OnConfirmAddFriendBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_FRIEND_CANCEL_ADDFRIEND_BTN, OnCancelAddFriendBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_FRIEND_CONFIRM_DELETEFRIEND_BTN, OnConfirmDelteFriendBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_FRIEND_CANCEL_DELETEFRIEND_BTN, OnCancelDeleteFriendBtn);

            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Friend_add, OnFriendAddBack);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Friend_delete, OnFriendDeleteBack);
            base.DisPose();
        }

        //前端检查Id是否正确
        public override void UpdateInput()
        {
            ///满足用户名条件
            if (IsMathRex(@"^[0-9A-Za-z\u4e00-\u9fa5]+$", m_AddFriendIdInput.value))
            {
                m_AddFriendIdInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_full";
            }
            else
            {
                m_AddFriendIdInput.transform.GetChild(0).GetComponent<UISprite>().spriteName = "account_empty";
            }
        }
    }
}
