using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Network;
using Network.Serializer;

namespace FW
{
	class NSSerializerProtocol : IPackProtocol
	{
        struct PackHead
        {
            public short Len;
            public short Flag;
            public uint CRC;
            public ulong CRC64;
            public PackHead(short len, short flag, uint crc, ulong crc64)
            {
                Len = len;
                Flag = flag;
                CRC = crc;
                CRC64 = crc64;
            }
        }

		public NSSerializerProtocol()
		{
            TextAsset format = ResMgr.ResLoad.Load<TextAsset>("cmd/format.json");
            TextAsset cmd = ResMgr.ResLoad.Load<TextAsset>("cmd/cmd.json");
            NetStreamSerializer.inst().AddTypeConfig(format.text, false);
            NetStreamSerializer.inst().AddTypeConfig(cmd.text, true);
		}

        //--------------------------------------
        //private
        //--------------------------------------
        private uint GetStreamCRC(Stream stream)
        {
            long pos = stream.Position;
            byte[] bytes = new byte[stream.Length-pos];
            stream.Read(bytes, 0, bytes.Length);
            stream.Position = pos;
            return CRCITU.GetCrc32(bytes);
        }

        //读取包头
        private PackHead ReadHead(BinaryReader br)
        {
            PackHead head = new PackHead();
            head.Len = br.ReadInt16();
            head.Flag = br.ReadInt16();
            head.CRC = br.ReadUInt32();
            head.CRC64 = br.ReadUInt64();
            return head;
        }

        //创建包头
        private Stream CreateHead(short len, uint crc)
        {
            short packLen = (short)(len + 16);
            PackHead packhead = new PackHead(packLen, 0, crc, 0);
            Stream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(packhead.Len);//包长度
            bw.Write(packhead.Flag);//包长度
            bw.Write(packhead.CRC);//包长度
            bw.Write(packhead.CRC64);//包长度
            return stream;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public Stream CreatePacket(int cmd, object obj)
		{
            short cmdLen = 0;
            uint crc = 0;
            MemoryStream objStream = new MemoryStream();
            if(obj != null)
            {
                BinaryWriter objBw = new BinaryWriter(objStream);
                objBw.Write(cmdLen);
                objBw.Write((ushort)cmd);
                NetStreamSerializer.inst().WriteToStream(objBw, cmd.ToString(), obj);
                objStream.Position = 0;
                cmdLen = (short)(objStream.Length - 2);
                objBw.Write(cmdLen);
                objStream.Position = 0;
                crc = this.GetStreamCRC(objStream);
                cmdLen += 2;
            }
            Stream stream = this.CreateHead(cmdLen, crc);
            NetUitl.CopyStream(objStream, stream);			
			stream.Position = 0;
			return stream;
		}

		/// <summary>
		/// 连接状态消息处理
		/// </summary>
		/// <param name="state"></param>
		public void ConnectState(NetMgr.CONN_MSG state)
		{
			NetDispatcherMgr.Inst.ChangeNetState(state);
		}
            
		public void ParsePacket(Stream stream)
		{
            BinaryReader br = new BinaryReader(stream);
            //读出包头
            PackHead head = this.ReadHead(br);
            uint crc32 = this.GetStreamCRC(stream);             //crc332
            if (crc32 != head.CRC) return;                      //校验未过
            while(true)
            {
                short bufflen = (short)(stream.Length - stream.Position);
                if (bufflen <= 2) break;
                short cmdlen = br.ReadInt16();
                bufflen -= 2;
                if (bufflen < cmdlen) break;                    //包体长度不对
                int cmd = br.ReadUInt16();                      //cmd
                object dataObj = null;
                Int64 readLen =  0;
                try
                {
                    readLen = NetStreamSerializer.inst().ReadFromStream(br, cmd.ToString(), out dataObj);
                }
                catch(ErrorStreamException ex)
                {
                    Debug.LogFormat("cmd buffer error:" + cmd + ex);
                    return;
                }
                if (readLen != cmdlen-2)                        //减去cmd占用两个字节
                {
                    short count = (short)(cmdlen - readLen - 2);
                    stream.Position += count;
                    Debug.LogFormat("cmd buffer error!"+cmd);
                }
                else
			        NetDispatcherMgr.Inst.Dispatch(cmd, (DataObj)dataObj);
            }

		}
	}
}