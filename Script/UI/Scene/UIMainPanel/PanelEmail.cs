//******************************************************************
// File Name:					PanelEmail
// Description:					PanelEmail class 
// Author:						lanjian
// Date:						1/5/2017 4:26:10 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Item;
using FW.Role;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PanelEmail:PanelBase
    {
        private string emailItemPath = "UIRootPrefabs/ItemPrefabsForScrollView/emailItem";
        protected PanelEmail()
        {
            this.m_panelName = "UIRootPrefabs/MainPanel/EmailPanel";
            this.panelType = PanelType.Email;
        }

        public static PanelEmail Create()
        {
            return new PanelEmail();
        }

        private int m_CurrentClickNum = -1;             //当前点击了那个item
        private int m_PreClickNum = -1;                 //记录上一个点击的index
        private bool m_IsExistUnFold = false;           //当前是否存在展开的
        private float m_ItemHeight;
        private Transform itemParent;                   //UIGrid
        private int m_EamilCount;                       //邮件的数量
        private float m_ExpandWidth_no = 400;           //无附件展开的大小
        private float m_ExpandWidth_have = 650;         //无附件展开的大小
        //private GameObject m_Hidebg;                  //没有附件
        //private GameObject m_Hidebg2;                 //有附件的
        //private bool m_HaveAttach = false;            //是否有附件  
        private Transform m_Dialog;                     //对话框

        private Transform m_center;
        private Transform m_bottom;
        private List<Mail> m_MailList = new List<Mail>();
        private List<GameObject> m_MailGoList = new List<GameObject>(0);

        //--------------------------------------
        //private
        //--------------------------------------
        //获取邮件list
        private void GetMailList()
        {
            m_MailList = Role.Role.Instance().MailProctor.GetMails();
            Debug.Log("邮件数目为————"+m_MailList.Count);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center")
                .GetChild(0).GetChild(2).gameObject, false);
            if (m_MailList.Count >= 40)
            {
                //飘字警告  邮件达到40封
                Utility.Utility.NotifyStr("您的邮件暂存数即将达到50封限制，请尽快清理无用邮件");
            }
        }

        private void FindAllUI()
        {
            m_center = PanelMgr.CurrPanel.RootObj.transform.Find("center");
            m_bottom = PanelMgr.CurrPanel.RootObj.transform.Find("bottom");
            m_EamilCount = m_MailList.Count;
            GetAllEmailItem();
            FillEmailItemData();
            DialogAbout();
        }

        private void DialogAbout()
        {
            m_Dialog = PanelMgr.CurrPanel.RootObj.transform.Find("dialog");
            Utility.Utility.GetUIEventListener(m_Dialog.Find("confirm").gameObject).onClick = OnConfirmDelete;
            Utility.Utility.GetUIEventListener(m_Dialog.Find("cancel").gameObject).onClick = OnCancel;
            NGUITools.SetActive(m_Dialog.gameObject, false);
        }

        //根据邮件数量 获取EmaliItem
        private void GetAllEmailItem()
        {
            itemParent = m_center.Find("ScrollView_Email").Find("UIGrid");
            m_ItemHeight = itemParent.GetComponent<UIGrid>().cellHeight;
            //先清空Item
            for (int i = 0; i < itemParent.childCount; i++)
            {
                GameObject.Destroy(itemParent.GetChild(0).gameObject);
            }
            for (int i = 0; i < m_EamilCount; i++)
            {
                GameObject currentItem = Utility.Utility.GetPrefabGameObject(emailItemPath, "item", itemParent);
                currentItem.transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                if (m_MailList[i].HasAttachment)
                    currentItem.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "1";
                //改个名
                currentItem.name = "item" + i;
                BindItemListener(currentItem);
                m_MailGoList.Add(currentItem);
            }
            //禁止滑动
            if (m_MailGoList.Count < 5)
            {
                m_center.Find("ScrollView_Email").GetComponent<UIScrollView>().enabled = false;
            }
        }

        private void BindItemListener(GameObject gg)
        {
            //绑定item点击 
            Utility.Utility.GetUIEventListener(gg).onClick = OnItemClick;
            //Utility.Utility.GetUIEventListener(currentItem.transform.Find("HIdebg").Find("GetEmailBtn").gameObject)
            //    .onClick = OnGetAttachMent;
            Utility.Utility.GetUIEventListener(gg.transform.Find("HIdebg").Find("DeleteEmailBtn").gameObject)
                .onClick = OnDeleteEmail;
            Utility.Utility.GetUIEventListener(gg.transform.Find("HIdebg1").Find("GetEmailBtn").gameObject)
                .onClick = OnGetAttachMent;
            Utility.Utility.GetUIEventListener(gg.transform.Find("HIdebg1").Find("DeleteEmailBtn").gameObject)
                .onClick = OnDeleteEmail;
            //默认设置为不显示
            //NGUITools.SetActive(gg.transform.Find("HIdebg").gameObject, false);
            //NGUITools.SetActive(gg.transform.Find("HIdebg1").gameObject, false);
        }

        //填充数据到邮件中
        private void FillEmailItemData()
        {
            for (int i = 0; i < m_MailGoList.Count; i++)
            {
                FillOneEmailItem(m_MailGoList[i]);
            }
        }

        private void FillOneEmailItem(GameObject go)
        {
            int index = int.Parse(go.name.Substring(4));
            go.transform.GetChild(0).GetComponent<UILabel>().text = m_MailList[index].ID;
            if(m_MailList[index].IsReaded)
                NGUITools.SetActive(go.transform.Find("eamiltip").GetChild(0).gameObject, false);
            else
                NGUITools.SetActive(m_MailGoList[index].transform.Find("eamiltip").GetChild(0).gameObject, true);
            string mailTypeStr = "未知";
            if (m_MailList[index].Type == 0)
                mailTypeStr = "系统";
            m_MailGoList[index].transform.GetChild(2).GetComponent<UILabel>().text = mailTypeStr;
            m_MailGoList[index].transform.GetChild(4).GetComponent<UILabel>().text = m_MailList[index].Title;
            //Debug.Log("邮件的内容：" + m_MailList[index].Content + m_MailList[index].HasAttachment);
            //这里判断是否有附jian的邮件
            if (m_MailList[index].HasAttachment)
            {
                go.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "1";
                Transform hidebg1 = m_MailGoList[index].transform.Find("HIdebg1");
                hidebg1.GetChild(1).GetComponent<UILabel>().text
                    = m_MailList[index].Content;
                hidebg1.GetChild(2).GetChild(1).GetComponent<UILabel>().text =
                    Utility.Utility.Int2DateAndSecond(m_MailList[index].Time);
                hidebg1.GetChild(3).GetChild(1).GetComponent<UILabel>().text = 
                    Utility.Utility.Int2DateAndSecond(m_MailList[index].LastTime);
                //附件相关
                if (m_MailList[index].IsExtract)
                    NGUITools.SetActive(hidebg1.Find("GetEmailBtn").gameObject,false);
                else
                    NGUITools.SetActive(hidebg1.Find("GetEmailBtn").gameObject, true);

                //获取附件
                List<MailAttachment> attachmentList = m_MailList[index].GetAttachments();
                if (attachmentList.Count > 0 && attachmentList[0].ItemBase!=null)
                {
                    hidebg1.Find("attachment").GetChild(2).GetComponent<UILabel>().text = attachmentList[0].ItemBase.Name+" X "+ attachmentList[0].Count;
                    //图片
                    string path = "";
                    if (attachmentList[0].ItemBase.ItemType == ItemType.Weapon)
                        path = Utility.ConstantValue.WeaponIcon;
                    if (attachmentList[0].ItemBase.ItemType == ItemType.Accessory)
                        path = Utility.ConstantValue.PartIcon;
                    if (attachmentList[0].ItemBase.ItemType == ItemType.Commodity)
                        path = Utility.ConstantValue.CommodityIcon;
                    Texture texture = ResMgr.ResLoad.Load<Texture>(path + "/" + attachmentList[0].ItemBase.Icon + "_down");
                    hidebg1.Find("attachment").GetChild(1).GetComponent<UITexture>().SetRect(0,0,texture.width,texture.height);
                    hidebg1.Find("attachment").GetChild(1).localPosition = new Vector3(-138,-27,0);
                    hidebg1.Find("attachment").GetChild(1).GetComponent<UITexture>().mainTexture = texture;
                }
            }
            else
            {
                //Debug.Log("邮件的内容------：" + m_MailList[index].Content);
                Transform hidebg = m_MailGoList[index].transform.Find("HIdebg");
                hidebg.GetChild(1).GetComponent<UILabel>().text
                    = m_MailList[index].Content;
                hidebg.GetChild(2).GetChild(1).GetComponent<UILabel>().text =
                    Utility.Utility.Int2DateAndSecond(m_MailList[index].Time);
            }
        }

        //设置邮件已读（隐藏右上角的邮件标志）
        private void SetEmailReaded(int index)
        {
            //邮件已读了
            NGUITools.SetActive(m_MailGoList[index].transform.Find("eamiltip").GetChild(0).gameObject, false);
        }

        //点击emailItem
        private void OnItemClick(GameObject itemGo)
        {
            
            m_CurrentClickNum = int.Parse(itemGo.name.Substring(4));
            //邮件是否读了
            string mailId = itemGo.transform.GetChild(0).GetComponent<UILabel>().text;
            if (GetMailInListById(mailId) != null && !GetMailInListById(mailId).IsReaded)
            {
                GetMailInListById(mailId).Read();
                SetEmailReaded(m_CurrentClickNum);
            }
            if (m_CurrentClickNum == m_PreClickNum)
            {
                this.ResetPosition();
                m_MailGoList[m_PreClickNum].transform.Find("HIdebg").gameObject.SetActive(false);
                m_MailGoList[m_PreClickNum].transform.Find("HIdebg1").gameObject.SetActive(false);
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

        //提取附件
        private void OnGetAttachMent(GameObject go)
        {
            GetMailInListById(go.transform.parent.parent.GetChild(0).GetComponent<UILabel>().text).Extract();
        }

        //提取成功的回调
        private void OnSuccessGetAttacement(Event.EventArg args)
        {
            ////隐藏提取附件的按钮
            //NGUITools.SetActive(this.GetGoByMail((Mail)args[0]), false);
            //参数：EventArg(Mail 邮件对象, UInt16 0成功1未成功2该邮件已过时)
            //提取了就删除
            if ((UInt16)args[1]==0)
            {
                OnConfirmDelete(null);
                //判断附近的类型 提示相关
                Utility.Utility.NotifyStr("领取附件成功，请去背包 / 账户查看");
            }
            if ((UInt16)args[1] == 1)
            {
                Utility.Utility.NotifyStr("领取附件失败！请重试！");
            }

            if ((UInt16)args[1] == 2)
            {
                OnConfirmDelete(null);
                Utility.Utility.NotifyStr("该邮件已经过期！已删除");
            }
        }

        //删除邮件
        private void OnDeleteEmail(GameObject go)
        {
            ShowDialog();
        }

        //对话框的确定
        private void OnConfirmDelete(GameObject go)
        {
            ExitDialog();
            string id = m_MailGoList[m_CurrentClickNum].transform.GetChild(0).GetComponent<UILabel>().text;
            Role.Role.Instance().MailProctor.DeleteMail(this.GetMailInListById(id));
            //先把数据删除
            m_MailList.Remove(this.GetMailInListById(id));
            GameObjectPositioAnim(m_MailGoList[m_CurrentClickNum],
               m_MailGoList[m_CurrentClickNum].transform.localPosition,
                m_MailGoList[m_CurrentClickNum].transform.localPosition + new Vector3(900,0,0),0.2f,
                m_MailGoList[m_CurrentClickNum]);
            //GameObject.DestroyImmediate(itemParent.GetChild(m_CurrentClickNum).gameObject);
            //ResetPosition();
        }

        //删除成功的回调
        private void OnSuccessDelete(Event.EventArg args)
        {
            //将邮件删除
            m_MailList.Remove((Mail)args[0]);
        }

        //对话框的取消
        private void OnCancel(GameObject go)
        {
            ExitDialog();
        }

        //接受到新邮件
        private void OnReciveNewMail()
        {
            GetMailList();
            //在ScrollView的顶部添加一个新的Item
            GameObject currentItem = Utility.Utility.GetPrefabGameObject(emailItemPath, "item", itemParent);
            currentItem.transform.localPosition = new Vector3(0, 0, 0);
            currentItem.gameObject.name = "item0";
            //填充第一个新邮件的item
            this.BindItemListener(currentItem);
            //Debug.Log("接收到新邮件后的邮件数  "+m_MailList.Count);
            m_MailGoList.Insert(0, currentItem);
            if (m_MailList.Count != 0)
                this.FillOneEmailItem(currentItem);
            //每个都下移动多少
            for (int i = 1; i < m_MailGoList.Count; i++)
            {
                m_MailGoList[i].transform.localPosition += new Vector3(0, -m_ItemHeight, 0);
                m_MailGoList[i].name = "item" +i;
            }
            //飘字通知
            Utility.Utility.NotifyStr("你收到一封新邮件!!");
        }

        //通过id在列表中找到该邮件
        private Mail GetMailInListById(string id)
        {
            foreach (var item in m_MailList)
            {
                if (item.ID.Equals(id))
                    return item;
            }
            return null;
        }

        //根据邮件找到ItemGO
        private GameObject GetGoByMail(Mail mail)
        {
            for (int i = 0; i < m_MailGoList.Count; i++)
            {
                if (mail.ID.Equals(m_MailGoList[i].transform.GetChild(0).GetComponent<UILabel>().text))
                    return m_MailGoList[i];
            }
            return null;
        }
        //重新刷新一下item的位置
        private void ResetPosition()
        {
            for (int i = 0; i < m_MailGoList.Count; i++)
            {
                //改个名
                m_MailGoList[i].name = "item" + i;
                //itemParent.GetChild(i).localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                Vector3 regin = m_MailGoList[i].transform.localPosition;
                Vector3 target = m_MailGoList[i].transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                GameObjectPositioAnim(m_MailGoList[i], regin, target);
                
            }
            m_IsExistUnFold = false;
        }

        //折叠
        private void FoldItem()
        {
            m_MailGoList[m_PreClickNum].transform.Find("HIdebg").gameObject.SetActive(false);
            m_MailGoList[m_PreClickNum].transform.Find("HIdebg1").gameObject.SetActive(false);
            for (int i = m_PreClickNum; i < m_MailGoList.Count; i++)
            {
                GameObject currentItem = m_MailGoList[i];
                currentItem.transform.localPosition =  new Vector3(0, 0 - i * m_ItemHeight, 0);
            }
            m_IsExistUnFold = false;
        }

        //展开
        private void UnFoldItem(GameObject go)
        {
            float currentExpandWidth = 0;
            //判断是那种邮件  是否有附件
            if (!go.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text.Equals("1"))
            {
                go.transform.Find("HIdebg").gameObject.SetActive(true);
                currentExpandWidth = m_ExpandWidth_no;
            }
            else
            {
                go.transform.Find("HIdebg1").gameObject.SetActive(true);
                currentExpandWidth = m_ExpandWidth_have;
            }

            for (int i = m_CurrentClickNum + 1; i < m_MailGoList.Count; i++)
            {
                GameObject currentItem = m_MailGoList[i];
                Vector3 regin = currentItem.transform.localPosition;
                Vector3 target = currentItem.transform.localPosition + new Vector3(0, 0 - currentExpandWidth, 0);
                GameObjectPositioAnim(currentItem, regin, target);
            }
            m_IsExistUnFold = true;
            m_PreClickNum = m_CurrentClickNum;
        }

        ///物体移动的动画
        private void GameObjectPositioAnim(GameObject go, Vector3 reginPosition, Vector3 targetPosition,
            float time =0.5f,GameObject DestoryGo =null)
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
                    m_MailGoList.Remove(DestoryGo);
                    GameObject.DestroyImmediate(DestoryGo);
                    this.ResetPosition();
                });
        }

        private void HideCenterAndbottom()
        {
            NGUITools.SetActive(m_center.gameObject,false);
            NGUITools.SetActive(m_bottom.gameObject, false);
        }

        private void ShowCenterAndbottom()
        {
            NGUITools.SetActive(m_center.gameObject, true);
            NGUITools.SetActive(m_bottom.gameObject, true);
        }

        private void ShowDialog()
        {
            HideCenterAndbottom();
            NGUITools.SetActive(m_Dialog.gameObject, true);
        }

        private void ExitDialog()
        {
            ShowCenterAndbottom();
            NGUITools.SetActive(m_Dialog.gameObject, false);
        }

        private void ResgistEvents()
        {
            FW.Event.FWEvent.Instance.Regist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Mail_extracted, OnSuccessGetAttacement);
            FW.Event.FWEvent.Instance.Regist(Event.EventID.Mail_delete, OnSuccessDelete);
            //是否有新邮件到来
            Event.FWEvent.Instance.Regist(FW.Event.EventID.Mail_Receive_New, OnReciveNewMail);
        }

        private void OnBackMainBtn()
        {
            PanelMgr.BackToMainPanel();
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void BindScript(UIEventBase eventBase)
        {
            GetMailList();
            FindAllUI();
            if (m_MailList.Count == 0)
               Utility.Utility.NotifyStr("当前你没有任何邮件！！！");
            ResgistEvents();
        }
        
        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.PANEL_BACK_TO_MAIN_PANEL_BTN, OnBackMainBtn);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Mail_extracted, OnSuccessGetAttacement);
            FW.Event.FWEvent.Instance.UnRegist(Event.EventID.Mail_delete, OnSuccessDelete);
            //是否有新邮件到来
            Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Mail_Receive_New, OnReciveNewMail);
            m_MailGoList.Clear();
            base.DisPose();
        }
    }
}
