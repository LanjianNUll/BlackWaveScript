// ------------------------------------------------------------------
// Description : 网络调度管理器
// Author      : wangpu
// Date        : 2014.08.05
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Network.Serializer;
using Network;

namespace FW
{
	//网络通知执行者
	delegate void NotifyExecuter(DataObj obj);

	//请求返回执行者  reqID: 请求id
	delegate void RequestExecuter(int reqID, DataObj obj);

	class NetDispatcherMgr
	{
		public static readonly NetDispatcherMgr Inst = new NetDispatcherMgr();

		//通过注册(Regist)的执行器
		private Dictionary<int, NotifyExecuter> m_dlgtDict;  //cmd -> delegate		
		private Queue<KeyValuePair<NotifyExecuter, DataObj>> m_notifyDispatchQueue;

        //通过发送请求时指定的执行器
        //private Dictionary<int, RequestExecuter> m_reqDict; //dataID -> delegate
        //private Queue<DataObj> m_reqDispatchQueue;
        private Queue<NetMgr.CONN_MSG> m_netMsgs;

		private object m_mutexObj = new object();

		private NetDispatcherMgr()
		{
			m_dlgtDict = new Dictionary<int, NotifyExecuter>();
			m_notifyDispatchQueue = new Queue<KeyValuePair<NotifyExecuter, DataObj>>();

            m_netMsgs = new Queue<NetMgr.CONN_MSG>();
        }

		//--------------------------
		// 注册消网络息执行者
		//--------------------------
		public void Regist(int cmd, NotifyExecuter executer)
		{
			lock (m_mutexObj)
			{
				if (m_dlgtDict.ContainsKey(cmd))
				{
					NotifyExecuter delg = m_dlgtDict[cmd];
					delg += executer;
					m_dlgtDict[cmd] = delg;
				}
				else
					m_dlgtDict[cmd] = executer;
			}
		}

		//--------------------------
		// 注销网络消息执行者
		//--------------------------
		public void UnRegist(int cmd, NotifyExecuter executer)
		{
			lock (m_mutexObj)
			{
				if (m_dlgtDict.ContainsKey(cmd))
				{
					NotifyExecuter delg = m_dlgtDict[cmd];
					delg -= executer;
					if (delg != null)
						m_dlgtDict[cmd] = delg;
					else
						m_dlgtDict.Remove(cmd);
				}
			}
		}

		//--------------------------
		// 请求消息  返回: reqID
		//--------------------------
		public int Request(int cmd, DataObj obj, RequestExecuter executer = null)
		{
			if (executer != null)
			{
				lock (m_mutexObj)
				{
					//m_reqDict[obj.DataIdx] = executer;
				}
			}

			NetMgr.Instance.Request(cmd, obj);

			return 0;
		}

		public int Request(int cmd, RequestExecuter executer = null)
		{
			return Request(cmd, new DataObj(), executer);
		}

		//--------------------------
		// 调度消息
		//--------------------------
		public void Dispatch(int cmd, DataObj obj)
		{
			lock (m_mutexObj)
			{
				if (m_dlgtDict.ContainsKey(cmd))
				{
					m_notifyDispatchQueue.Enqueue(new KeyValuePair<NotifyExecuter, DataObj>(m_dlgtDict[cmd], obj));
				}

				//if (m_reqDict.ContainsKey(obj.DataIdx))
				//{
				//	m_reqDispatchQueue.Enqueue(new KeyValuePair<RequestExecuter, DataObj>(m_reqDict[obj.DataIdx], obj));
				//}
			}
		}

		//------------------------
		// 网络状态改变
		//------------------------
		public void ChangeNetState(NetMgr.CONN_MSG conn)
		{
            lock (m_mutexObj)
            {
                m_netMsgs.Enqueue(conn);
            }
		}

		//--------------------------
		// 主循环调用
		//--------------------------
		public void Updata()
		{
			lock (m_mutexObj)
			{
				if (m_notifyDispatchQueue.Count > 0)
				{
					KeyValuePair<NotifyExecuter, DataObj> notifyPair = m_notifyDispatchQueue.Dequeue();
					if (notifyPair.Key != null)
					{
						notifyPair.Key(notifyPair.Value);
					}
				}

               
                while(this.m_netMsgs.Count > 0)
                {
                    NetMgr.CONN_MSG conn = this.m_netMsgs.Dequeue();
                    FW.Event.FWEvent.Instance.Call(FW.Event.EventID.NET_CONN_STATE, new FW.Event.EventArg(conn));
                }
            }
		}

	}
}
