//******************************************************************
// File Name:					Mail.cs
// Description:					Mail class 
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
using FW.ResMgr;

namespace FW.Role
{
    class Mail
    {
        private string m_id;
        private int m_type;            //类型
        private string m_title;             //标题
        private string m_content;           //正文
        private int m_time;                 //时间
        private int m_lastTime;             //到期时间

        private bool m_isreaded;            //是否已读
        private bool m_hasAttachment;       //是否有附件
        private bool m_isExtract;           //是否已提取

        private List<MailAttachment> m_attachments;   //附件列表

        public Mail(DataObj data)
        {
            this.m_isreaded = false;
            this.m_hasAttachment = false;
            this.m_isExtract = false;
            this.m_attachments = new List<MailAttachment>();

            this.Init(data);

            NetDispatcherMgr.Inst.Regist(Commond.Request_Read_mail_back, OnRead);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Extract_mail_back, OnExtract);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public string ID { get { return this.m_id; } }
        //类型
        public int Type { get { return this.m_type; } }
        //是否已读
        public bool IsReaded { get { return this.m_isreaded; } }
        //时间
        public int Time { get { return this.m_time; } }
        //到期时间，如果为0，则没有到时间
        public int LastTime { get { return this.m_lastTime; } }
        //标题
        public string Title { get { return this.m_title; } }
        //正文
        public string Content { get { return this.m_content; } }
        //是否有附件
        public bool HasAttachment { get { return this.m_hasAttachment; } }
        //是否已提取过附件
        public bool IsExtract { get { return this.m_isExtract; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init(DataObj data)
        {
            if (data == null) return;
            this.m_id = data.GetString("mailId");
            this.m_type = data.GetInt8("mailType");
            this.m_time = data.GetInt32("createTime");
            this.m_lastTime = data.GetInt32("deleteTime");
            this.m_title = data.GetString("topic");
            this.m_content = data.GetString("content");
            this.m_isreaded = data.GetInt8("hasRead") == 1;
            List<DataObj> affixDatas = data.GetDataObjList("affixs");
            this.m_hasAttachment = affixDatas != null && affixDatas.Count > 0;
            this.LoadAttachments(affixDatas);
            //从配置表值读取邮件内容
            this.LoadMailTemplate();
            //处理下文本（参数替换到--s--）
            List<string> paramsStrList = data.GetStringList("params");
            this.DoWithMainContent(paramsStrList);
        }

        private void DoWithMainContent(List<string> paramsStrList)
        {
            if (paramsStrList != null)
            {
                string[] str = this.m_content.Split("--s--".ToCharArray());
                List<string> strList = new List<string>();
                for (int i = 0; i < str.Length; i++)
                {
                    if (!string.IsNullOrEmpty(str[i]))
                        strList.Add(str[i]);
                }
                this.m_content = "";
                for (int i = 0; i < strList.Count-1; i++)
                {
                    this.m_content += strList[i] + paramsStrList[i];
                }
                this.m_content += strList[strList.Count - 1];
            }
        }

        private void LoadMailTemplate()
        {
            JsonConfig jsonConfig = DatasMgr.MailTemplateCfg;
            int id = (int)this.m_type;
            this.m_title = jsonConfig.GetJsonItem(id.ToString()).Get("topic").AsString();
            this.m_content = jsonConfig.GetJsonItem(id.ToString()).Get("content").AsString();
        }

        //load attachment
        private void LoadAttachments(List<DataObj> datas)
        {
            this.RemoveAttachments();
            if (datas == null || datas.Count == 0) return;
            foreach(DataObj data in datas)
            {
                MailAttachment ment = new MailAttachment(data);
                this.m_attachments.Add(ment);
            }
        }

        //删除附件
        private void RemoveAttachments()
        {
            if (this.m_attachments == null || this.m_attachments.Count == 0) return;
            foreach(MailAttachment ment in this.m_attachments)
            {
                ment.Dispose();
            }
            this.m_attachments.Clear();
        }

        //响应读取邮件
        private void OnRead(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            string id = data.GetString("mailId");
            if (id != this.ID) return;
            if (ret == 0)
                this.m_isreaded = true;
            Event.FWEvent.Instance.Call(Event.EventID.Mail_readed, new Event.EventArg(this, ret == 0));
        }

        //响应提取邮件附件
        private void OnExtract(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            string id = data.GetString("mailId");
            if (id != this.ID) return;
            if (ret == 0)
                this.m_isExtract = true;
            Event.FWEvent.Instance.Call(Event.EventID.Mail_extracted, new Event.EventArg(this, ret));
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Read_mail_back, OnRead);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Extract_mail_back, OnExtract);

            this.RemoveAttachments();
        }

        //获取附件列表
        public List<MailAttachment> GetAttachments()
        {
            return this.m_attachments;
        }

        //阅读邮件
        public void Read()
        {
            if (this.m_isreaded) return;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["mailId"] = this.ID;
            NetMgr.Instance.Request(Commond.Request_Read_mail, data);
        }

        //提取邮件
        public void Extract()
        {
            if (this.HasAttachment == false || this.m_isExtract == true) return;
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["mailId"] = this.ID;
            NetMgr.Instance.Request(Commond.Request_Extract_mail, data);
        }
    }
}