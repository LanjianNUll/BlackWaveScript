//******************************************************************
// File Name:					MailProctor.cs
// Description:					MailProctor class 
// Author:						wuwei
// Date:						2017.02.17
// Reference:
// Using:
// Revision History:
//******************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Network;
using Network.Serializer;
using FW.Item;

namespace FW.Role
{
    class MailProctor
    {
        private Role m_owner;
        private Dictionary<string, Mail> m_mails;

        public MailProctor(Role role)
        {
            this.m_owner = role;
            this.m_mails = new Dictionary<string, Mail>();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public Role Owner { get { return this.m_owner; } }
        //是否有新（未读邮件）
        public bool HasNewMail { get { return this.HasNewMails(); } }

        //--------------------------------------
        //private 
        //--------------------------------------
        //查询是否有未读邮件
        private bool HasNewMails()
        {
            foreach(Mail mail in this.m_mails.Values)
            {
                if (mail.IsReaded == false)
                    return true;
            }
            return false;
        }

        //创建一封邮件
        private void CreateMail(List<DataObj> datas)
        {
            if (datas == null || datas.Count == 0) return;
            foreach (DataObj mailData in datas)
            {
                Mail mail = new Mail(mailData);
                if (mail == null) continue;
                this.m_mails.Add(mail.ID, mail);
            }
        }

        //删除所有邮件
        private void DeleteAllMail()
        {
            foreach (Mail mail in this.m_mails.Values)
            {
                mail.Dispose();
            }
            this.m_mails.Clear();
        }

        //请求邮件列表返回
        private void OnRequestMail(DataObj data)
        {
            if (data == null) return;
            this.DeleteAllMail();
            List<DataObj> datas = data.GetDataObjList("infos");
            this.CreateMail(datas);
            //发送初始化消息
            Event.FWEvent.Instance.Call(Event.EventID.Mail_inited, new Event.EventArg());
        }

        //响应删除邮件
        private void OnDeleteMail(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0) return;
            string id = data.GetString("mailId");
            Mail mail = this.GetMail(id);
            if (mail == null) return;
            mail.Dispose();
            this.m_mails.Remove(id);
            Event.FWEvent.Instance.Call(Event.EventID.Mail_delete, new Event.EventArg(mail, ret == 0));
        }

        //接受新邮件
        private void OnRecevieMail(DataObj data)
        {
            if (data == null) return;
            List<DataObj> datas = data.GetDataObjList("infos");
            this.CreateMail(datas);
            //发送初始化消息
            Event.FWEvent.Instance.Call(Event.EventID.Mail_Receive_New, new Event.EventArg());
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            NetDispatcherMgr.Inst.Regist(Commond.Request_Init_mail_back, OnRequestMail);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Delete_mail_back, OnDeleteMail);
            NetDispatcherMgr.Inst.Regist(Commond.Mail_Receive_Add, OnRecevieMail);

            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Init_mail, data);
        }

        //销毁
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Init_mail_back, OnRequestMail);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Delete_mail_back, OnDeleteMail);
            NetDispatcherMgr.Inst.UnRegist(Commond.Mail_Receive_Add, OnRecevieMail);

            this.DeleteAllMail();
        }

        //获取邮件
        public Mail GetMail(string id)
        {
            Mail mail = null;
            this.m_mails.TryGetValue(id, out mail);
            return mail;
        }

        //获取邮件列表
        public List<Mail> GetMails()
        {
            Mail[] mails = new Mail[this.m_mails.Count];
            this.m_mails.Values.CopyTo(mails, 0);
            List<Mail> ms = new List<Mail>(mails);
            ms.Sort((x,y)=> -x.Time.CompareTo(y.Time));
            return ms;
        }
        
        //读取邮件
        public void ReadMail(Mail mail)
        {
            if (mail == null) return;
            mail.Read();
        }

        //提取邮件
        public void ExtractMail(Mail mail)
        {
            if (mail == null) return;
            mail.Extract();
        }

        //删除邮件
        public void DeleteMail(Mail mail)
        {
            if (mail == null) return;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["mailId"] = mail.ID;
            NetMgr.Instance.Request(Commond.Request_Delete_mail, data);
        }

    }
}