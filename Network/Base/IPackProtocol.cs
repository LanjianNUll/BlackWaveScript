//******************************************************************
// File Name:					IPackProtocol.cs
// Description:					IPackProtocol interface 
// Author:						wangpu
// Date:						2014.04.03
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.IO;

namespace Network
{
	//协议层抽象为接口,减少实际协议与网络层耦合
	public interface IPackProtocol
	{
		/// <summary>
		/// 创建一个数据包
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		Stream CreatePacket(int cmd, Object obj);

		/// <summary>
		/// 解析数据包
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		void ParsePacket(Stream stream);

		/// <summary>
		/// 连接状态
		/// </summary>
		/// <param name="state"></param>
		void ConnectState(NetMgr.CONN_MSG state);
	}
}