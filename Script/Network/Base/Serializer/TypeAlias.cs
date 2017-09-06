using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using KeyTypePair = System.Collections.Generic.KeyValuePair<string, Network.Serializer.NetStreamType>;

namespace Network.Serializer
{
    #region BaseClass NetStreamType
    abstract class NetStreamType
    {
        protected long BuffLen(BinaryReader stream)
        {
            if (stream == null) return 0;
            return stream.BaseStream.Length - stream.BaseStream.Position;
        }
        abstract public void WriteToStream(string tname, BinaryWriter stream, object value);
        abstract public Int64 ReadFromStream(BinaryReader stream, out object data);
    }
    #endregion

    // --------------------------------------------------------------
    #region Int8NSType
    class Int8NSType: NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is sbyte))
                throw new ErrorTypeException(tname, value);
            stream.Write((sbyte)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if(BuffLen(stream) < sizeof(sbyte))
                throw new ErrorStreamException();
            data = stream.ReadSByte();
            return sizeof(sbyte);
        }
    }
    #endregion

    #region Int16NSType
    class Int16NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if(!(value is Int16))
                throw new ErrorTypeException(tname, value);
            stream.Write((Int16)value);
        }
        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(Int16))
                throw new ErrorStreamException();
            data = stream.ReadInt16();
            return sizeof(Int16);
        }
    }
    #endregion

    #region Int32NSType
    class Int32NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is Int32))
                throw new ErrorTypeException(tname, value);
            stream.Write((Int32)value);
        }
        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(Int32))
                throw new ErrorStreamException();
            data = stream.ReadInt32();
            return sizeof(Int32);
        }
    }
    #endregion 

    #region Int64NSType
    class Int64NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is Int64))
                throw new ErrorTypeException(tname, value);
            stream.Write((Int64)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(Int64))
                throw new ErrorStreamException();
            data = stream.ReadInt64();
            return sizeof(Int64);
        }
    }
    #endregion

    #region UInt8NSType 
    class UInt8NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is byte))
                throw new ErrorTypeException(tname, value);
            stream.Write((byte)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(byte))
                throw new ErrorStreamException();
            data = stream.ReadByte();
            return sizeof(byte);
        }
    }
    #endregion

    #region UInt16NSType
    class UInt16NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is UInt16))
                throw new ErrorTypeException(tname, value);
            stream.Write((UInt16)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(UInt16))
                throw new ErrorStreamException();
            data = stream.ReadUInt16();
            return sizeof(UInt16);
        }
    }
    #endregion

    #region UInt32NSType
    class UInt32NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is UInt32))
                throw new ErrorTypeException(tname, value);
            stream.Write((UInt32)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(UInt32))
                throw new ErrorStreamException();
            data = stream.ReadUInt32();
            return sizeof(UInt32);
        }
    }
    #endregion

    #region UInt64NSType
    class UInt64NSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is UInt64))
                throw new ErrorTypeException(tname, value);
            stream.Write((UInt64)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(UInt64))
                throw new ErrorStreamException();
            data = stream.ReadUInt64();
            return sizeof(UInt64);
        }
    }
    #endregion

    #region FloatNSType
    class FloatNSType: NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is Single))
                throw new ErrorTypeException(tname, value);
            stream.Write((Single)value);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(float))
                throw new ErrorStreamException();
            data = stream.ReadSingle();
            return sizeof(float);
        }
    }
    #endregion

    #region BoolNSType
    class BoolNSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is bool))
                throw new ErrorTypeException(tname, value);
            byte bv = ((bool)value) ? (byte)1 : (byte)0; 
            stream.Write(bv);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(byte))
                throw new ErrorStreamException();
            data = stream.ReadByte() > 0 ? true : false;
            return sizeof(byte);
        }
    }
    #endregion 

    #region StringNSType
    class StringNSType : NetStreamType
    {
        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (!(value is string))
                throw new ErrorTypeException(tname, value);
            string str = (string)value;
            byte[] bstrs = System.Text.Encoding.UTF8.GetBytes(str);

            int len = bstrs.Length;
            if (len > 0xffff)
                throw new Exception("string stream must less than 65536 bytes.");
            stream.Write((UInt16)len);
            stream.Write(bstrs);
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(UInt16))
                throw new ErrorStreamException();
            UInt16 count = stream.ReadUInt16();
            if (BuffLen(stream) < count)
                throw new ErrorStreamException();
            byte[] bstrs = stream.ReadBytes(count);
            data = System.Text.Encoding.UTF8.GetString(bstrs);
            return count + sizeof(UInt16);
        }
    }
    #endregion

    #region ArrayNSType
    class ArrayNSType : NetStreamType 
    {
        private readonly NetStreamType m_nstype;
        public ArrayNSType(NetStreamType nstype)
        {
            this.m_nstype = nstype;            
        }

        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            if (value == null)
            {
                stream.Write((UInt16)0);
                return;
            }

            List<object> vs;
            try
            {
                vs = (List<object>)value;
            }
            catch (InvalidCastException)
            {
                throw new ErrorTypeException(tname, value);
            }
            int count = vs.Count;
            if (count > 0xffff)
                throw new ErrorTypeException(tname, value);           
            stream.Write((UInt16)count);
            foreach (object v in vs)
                this.m_nstype.WriteToStream(tname, stream, v);
        }
        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            if (BuffLen(stream) < sizeof(UInt16))
                throw new ErrorStreamException();
            UInt16 count = stream.ReadUInt16();
            Int64 len = sizeof(UInt16);
            List<object> vs = new List<object>(count);
            for(UInt16 i = 0; i < count; ++i)
            {
                object subData = null;
                len += this.m_nstype.ReadFromStream(stream, out subData);
                vs.Add(subData);
            }
            data = vs;
            return len;
        }
    }
    #endregion

    #region UnionNSType
    class UnionNSType : NetStreamType
    {
        private List<KeyTypePair> m_nstypes;

        public UnionNSType()
        {
        }

        public UnionNSType(List<KeyTypePair> nstypes)
        {
            this.SetTypes(nstypes);
        }

        public void SetTypes(List<KeyTypePair> nstypes)
        {
            m_nstypes = nstypes;
        }

        public override void WriteToStream(string tname, BinaryWriter stream, object value)
        {
            Dictionary<string, object> members;
            try
            {
                members = (Dictionary<string, object>)value;
            }
            catch (InvalidCastException)
            {
                throw new ErrorTypeException(tname, value);
            }
            foreach (KeyTypePair keyType in m_nstypes)
            {
                if (members.ContainsKey(keyType.Key))
                    keyType.Value.WriteToStream(tname, stream, members[keyType.Key]);
                else
                    throw new ErrorTypeException(tname, value);
            }
        }

        public override Int64 ReadFromStream(BinaryReader stream, out object data)
        {
            Int64 len = 0;
            DataObj dataObj = new DataObj();
            foreach (KeyTypePair keyType in m_nstypes)
            {
                object subData = null;
                len += keyType.Value.ReadFromStream(stream, out subData);
                dataObj[keyType.Key] = subData;
            }
            data = dataObj;
            return len;
        }
    }
    #endregion

}