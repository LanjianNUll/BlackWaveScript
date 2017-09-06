//******************************************************************
// File Name:					NetMgr.cs
// Description:					NetMgr class 
// Author:						wangpu
// Date:						2014.03.27
// Reference:
// Using:
// Revision History:
//******************************************************************

using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;

namespace Network
{
	/// <summary>
	/// 连接状态委托
	/// </summary>
	public delegate void ConnectCallBack(int state);

	/// <summary>
	/// 读取数据委托
	/// </summary>
	public delegate void GetDataBack(Stream stream);

	/// <summary>
	/// 网络管理器
	/// </summary>
	public class NetMgr
	{
		//连接请求状态 
		public enum CONN_MSG
		{
			Success = 0,
			Fail,
			TimeOut,
			ServerBreak,
			ClientBreak,
			Last
		}

		private const int PACKAGE_LENGTH_SIZE = 2; //包长度数据用2个字节表示

		//默认连接超时时间 
		private const int TIMEOUT_CONN_DEF = 10000; //默认连接超时时间 

		//发包最小间隔时间(毫秒) 
		private const int SEND_INTERVAL = 10;

		private TcpClient m_tcpClient;

		private String m_addr = ""; //地址 
		private int m_port;    //端口 

		private bool m_connecting; //是否在连接中, 防止重复请求连接 
		private int m_connTimeout;

		//请求队列
		private Queue m_reqQueue = new Queue();
		private Queue m_reqSuperQueue = new Queue(); //高级请求队列, 不会被锁定
		private object m_mutexObj = new object();

		private bool m_lockQueue; //锁定请求队列, (可以往队列放请求,但不会向服务器发送)

		private MemoryStream m_readBuf = new MemoryStream(); //读取数据缓冲

		////获得服务器数据事件
		//private GetDataBack m_getDataBack;

		//包协议
		private IPackProtocol m_customProtocol;

		/// <summary>
		/// 网络实例
		/// </summary>
		public static NetMgr Instance = new NetMgr();

		private NetMgr()
		{
		}

		//-------------------------------------------------
		// properties
		//-------------------------------------------------

		///// <summary>
		///// 读取到服务器数据事件委托
		///// </summary>
		//public GetDataBack GetDataBack
		//{
		//	get { return m_getDataBack; }
		//	set { m_getDataBack = value;}
		//}

		/// <summary>
		/// 当前是否连接状态
		/// </summary>
		public bool Connected
		{
			get
			{
				try
				{
					if (m_tcpClient != null)
					{
						return m_tcpClient.Connected;
					}
					else
					{
						return false;
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning(e.Message);
					return false;
				}
			}
		}

		/// <summary>
		/// 自定义通讯协议
		/// </summary>
		public IPackProtocol CustomProtocol
		{
			get { return m_customProtocol; }
		}

		public bool Connecting
		{
			get { return m_connecting; }
			set { m_connecting = value; }
		}

		public int ConnTimeout
		{
			get { return m_connTimeout; }
			set { m_connTimeout = value; }
		}

		public String Addr
		{
			get { return m_addr; }
		}

		public int Port
		{
			get	{ return m_port; }
		}

		public bool LockQueue
		{
			get { return m_lockQueue; }
			set { m_lockQueue = value; }
		}

		//------------------------------------------------
		// public
		//------------------------------------------------

		/// <summary>
		/// 初始化服务器地址
		/// </summary>
		/// <param name="addr">
		/// 服务器地址
		/// </param>  
		/// <param name="port">
		/// 端口
		/// </param>
		public void Init(IPackProtocol proctocol)
		{
			this.m_customProtocol = proctocol; 
			this.m_lockQueue = false;
		}

		/// <summary>
		/// 请求连接
		/// </summary>
        public void ReConnect()
		{
            Debug.LogWarning("--start ReConnect");
            this.Connect(m_addr,m_port,TIMEOUT_CONN_DEF);
            Debug.LogWarning("--end ReConnect");
		}

		/// <summary>
		/// 请求连接
		/// </summary>
		/// <param name="connTimeout">
		/// 超时时间(毫秒)
		/// </param>
        public void Connect(String addr, int port, int connTimeout = TIMEOUT_CONN_DEF)
		{
            if (!string.IsNullOrEmpty(m_addr) && addr.Equals(m_addr) && port == m_port && this.Connected)
            {
                CallConnBack(CONN_MSG.Success);
                return;
            }

            this.m_addr = addr;
            this.m_port = port;

			if (!m_connecting)
			{
				Thread checkThread = null;
				if (m_tcpClient != null && m_tcpClient.Connected)
				{
					m_tcpClient.Close();
				}

				this.m_tcpClient = new TcpClient();
				this.m_tcpClient.SendTimeout = 10000;
				try
				{
					m_connecting = true;

                    this.m_connTimeout = connTimeout;
					//开始请求连接
					this.m_tcpClient.BeginConnect(m_addr, m_port, new AsyncCallback(ConnectCallback), null);

					//创建一个检测连接超时的线程
					//CheckConnect checkConn = new CheckConnect(this.m_tcpClient, this);
					//checkThread = new Thread(checkConn.CheckConnTimeout); //超时检测
					//checkThread.Start();
				}
				catch (Exception)
				{
					try
					{
						this.m_tcpClient.Close();
					}
					catch (Exception e1)
					{
						Debug.LogWarning("tcpClient close exception: "+e1.Message); 
					}

					try
					{
						if (checkThread != null)
						{
							checkThread.Abort();
						}
					}
					catch (Exception e2)
					{
                        Debug.LogWarning("checkThread abort exception: " + e2.Message); 
					}

					CallConnBack(CONN_MSG.Fail);
				}
			}
		}

		/// <summary>
		/// 发送请求
		/// </summary>
		/// <param name="stream">
		/// 发送的数据
		/// </param>
		public void Request(Stream stream)
		{
			if (m_tcpClient != null /*&& m_tcpClient.Connected*/)
			{
				//添加请求到队列
				lock (m_mutexObj)
				{
					m_reqQueue.Enqueue(stream);
				}
			}
		}

		/// <summary>
		/// 发送请求
		/// </summary>
		/// <param name="cmd">
		/// 消息ID
		/// </param>
		/// <param name="obj">
		/// 发送的对象
		/// </param>
		public void Request(int cmd, object obj)
		{
			if (m_tcpClient != null)
			{
				lock (m_mutexObj)
				{
					m_reqQueue.Enqueue(m_customProtocol.CreatePacket(cmd, obj));
				}
			}
		}

		//放入高级请求队列, 在锁定情况下也能发送数据
		public void SuperRequest(int cmd, object obj)
		{
			if (m_tcpClient != null)
			{
				lock (m_mutexObj)
				{
					m_reqSuperQueue.Enqueue(m_customProtocol.CreatePacket(cmd, obj));
				}
			}
		}

		//清空请求队列
		public void ClearRequest()
		{
			lock (m_mutexObj)
			{
				m_reqQueue.Clear();
				m_reqSuperQueue.Clear();
			}
		}

		/// <summary>
		/// 断开连接并关闭socket
		/// </summary>
		public void Close()
		{
			if (m_tcpClient != null)
			{
				m_tcpClient.Close();
                m_tcpClient = null;
                CallConnBack(CONN_MSG.ClientBreak);
			}
            this.m_connecting = false;
        }

		/// <summary>
		/// 异步请求连接回调
		/// </summary>
		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				if (this.m_tcpClient.Connected)
				{
					this.m_tcpClient.EndConnect(ar);
					CallConnBack(CONN_MSG.Success);

					//创建发送请求线程
					new Thread(SendRequestThread).Start();

					//开始异步读取服务数据
					StateObject state = new StateObject();
					state.client = this.m_tcpClient;
					NetworkStream stream = this.m_tcpClient.GetStream();
					if (stream.CanRead)
					{
						try
						{
							stream.BeginRead(state.buffer, 0, StateObject.BufferSize,
									   new AsyncCallback(ReadSvrMsg), state);
						}
						catch (Exception e)
						{
							Debug.LogWarning("Network ConnectCallback exception: "+e.Message);
						}
					}
				}
				else
				{
					CallConnBack(CONN_MSG.Fail);
				}
			}
			catch (Exception e)
			{
				CallConnBack(CONN_MSG.Fail);
                Debug.LogWarning("ConnectCallback exception: "+e.Message); 
			}
			finally
			{
				m_connecting = false;
			}
		}

		public void CallConnBack(CONN_MSG state)
		{
			if (m_customProtocol != null)
			{
				m_customProtocol.ConnectState(state);
			}
		}

		//-----------------------------------------------------
		// private
		//-----------------------------------------------------

		/// <summary>
		/// 异步读取服务器数据回调
		/// </summary>
		/// <param name="ar"></param>
		private void ReadSvrMsg(IAsyncResult ar)
		{
			StateObject state = (StateObject)ar.AsyncState;

			if ((state.client == null) || (!state.client.Connected))
			{
				return;
			}

			try
			{
				NetworkStream netStream = state.client.GetStream();
				int count = netStream.EndRead(ar);

				if (count == 0)
				{//服务器主动断开
					CallConnBack(CONN_MSG.ServerBreak);
					state.client.Close();
					return;
				}

                m_readBuf.Write(state.buffer, 0, count);
                m_readBuf.Position = 0;
                ParseReadBuf();

                netStream.BeginRead(state.buffer, 0, StateObject.BufferSize,
							new AsyncCallback(ReadSvrMsg), state);
			}
			catch (IOException e)
			{//服务器异常中断
				Debug.LogWarning("readSvrMsg exception: "+e.Message);
				//Debug.LogException(e);

				if (!state.client.Connected)
				{
					CallConnBack(CONN_MSG.ServerBreak);
					state.client.Close();
				}
			}
		}

		/// <summary>
		/// 发送请求线程
		/// </summary>
		private void SendRequestThread()
		{
			while (m_tcpClient != null && (m_tcpClient.Connected))
			{
				if (m_reqSuperQueue.Count > 0)
					SendQueueData(m_reqSuperQueue);

				if (m_reqQueue.Count > 0 && !m_lockQueue)
					SendQueueData(m_reqQueue);

				Thread.Sleep(SEND_INTERVAL);
			}
		}

        //解析读取缓冲区
		private void ParseReadBuf()
		{
            BinaryReader br = null;
			while (m_readBuf.Length - m_readBuf.Position >= PACKAGE_LENGTH_SIZE)
			{
                if (br == null) br = new BinaryReader(m_readBuf);

				short packLen = br.ReadInt16();
				if (m_readBuf.Length < packLen)
				{//流不够长, 等待下次发包
                    m_readBuf.Position -= PACKAGE_LENGTH_SIZE;  //回退两个字节(上面读出的包长)
					break;
				}
				else
				{
                    m_readBuf.Position -= PACKAGE_LENGTH_SIZE;
                    MemoryStream packStream = new MemoryStream();
					NetUitl.CopyStream(m_readBuf, packStream, packLen);
					//if (m_getDataBack != null)
					//{  //回调读取事件
					//	m_getDataBack(packStream);
					//	packStream.Position = 0;
					//}
					packStream.Position = 0;
					m_customProtocol.ParsePacket(packStream);
					packStream.Close();
				}
			}

			//如果包被截断, 此处没有用完， 把剩余部分复制下来, 释放用过的数据
            if (m_readBuf.Position < m_readBuf.Length)
            {
                MemoryStream tmpStream = new MemoryStream();
                int copyLen = (int)(m_readBuf.Length - m_readBuf.Position);
                NetUitl.CopyStream(m_readBuf, tmpStream, copyLen);
                m_readBuf = tmpStream;
            }
            else
                m_readBuf = new MemoryStream(); //释放之前已经处理过的数据
		}

		private void SendQueueData(Queue queue)
		{
			Stream stream = null;
			lock (m_mutexObj)
			{
				stream = (Stream)queue.Dequeue();
			}

			if (stream != null)
			{
				stream.Position = 0;
				NetUitl.CopyStream(stream, this.m_tcpClient.GetStream());
				this.m_tcpClient.GetStream().Flush();
			}
		}

		internal class StateObject
		{
			public TcpClient client = null;
			public const int BufferSize = 1024 * 20;
			public byte[] buffer = new byte[BufferSize];
		}
	}

	//检测连接超时线程 
	class CheckConnect
	{
		private TcpClient checkThread;
		private NetMgr mgr;

		public CheckConnect(TcpClient checkTcpClient, NetMgr mgr)
		{
			this.checkThread = checkTcpClient;
			this.mgr = mgr;
		}

		public void CheckConnTimeout()
		{
			Thread.Sleep(mgr.ConnTimeout);

			if (!checkThread.Connected && mgr.Connecting)
			{
				try
				{
					mgr.Connecting = false;
					checkThread.Close();// 停止连接 
				}
				finally
				{
					mgr.CallConnBack(NetMgr.CONN_MSG.TimeOut);
				}
			}
		}
	}
}