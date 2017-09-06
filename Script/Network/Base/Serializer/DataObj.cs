using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Serializer
{
    public class DataObj : Dictionary<string, object>, INetStreamObject
    {
		
		public DataObj()
		{
		}
        //sunlu:重写tostring,方便调试
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            foreach (var key in Keys)
            {
                sb.AppendFormat("{0}={1}, ", key, this[key]);
            }
            sb.Append("]");

            return sb.ToString();
        }

		public T Get<T>(string key)
		{
			object ret = this[key];
			if (ret == null)
			{
				return default(T);
			}

			if (!(ret is T))
				throw new ErrorTypeException(key, ret);
			return (T)ret;
		}

		public List<T> GetList<T>(string key)
		{
			object ret = this[key];
			if (!(ret is List<object>))
				throw new ErrorTypeException("List<" + key + ">", ret);
			return ChangeListType<T>(ret as List<object>);
		}

        public sbyte GetInt8(string key)
        {
            object ret = this[key];
            if (!(ret is sbyte))
                throw new ErrorTypeException("INT8", ret);
            return (sbyte)ret;
        }

        public List<sbyte> GetInt8List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<sbyte>(ret as List<object>);
        }

        public Int16 GetInt16(string key)
        {
            object ret = this[key];
            if (!(ret is Int16))
                throw new ErrorTypeException("INT16", ret);
            return (Int16)ret;
        }

        public List<Int16> GetInt16List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<Int16>(ret as List<object>);
        }

        public Int32 GetInt32(string key)
        {
            object ret = this[key];
            if (!(ret is Int32))
                throw new ErrorTypeException("INT32", ret);
            return (Int32)ret;
        }

        public List<Int32> GetInt32List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<Int32>(ret as List<object>);
        }

        public Int64 GetInt64(string key)
        {
            object ret = this[key];
            if (!(ret is Int64))
                throw new ErrorTypeException("Int64", ret);
            return (Int64)ret;
        }

        public List<Int64> GetInt64List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<Int64>(ret as List<object>);
        }

        public byte GetUInt8(string key)
        {
            object ret = this[key];
            if (!(ret is byte))
                throw new ErrorTypeException("UINT8", ret);
            return (byte)ret;
        }

        public List<byte> GetUInt8List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<byte>(ret as List<object>);
        }

        public UInt16 GetUInt16(string key)
        {
            object ret = this[key];
            if (!(ret is UInt16))
                throw new ErrorTypeException("UInt16", ret);
            return (UInt16)ret;
        }

        public List<UInt16> GetUInt16List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<UInt16>(ret as List<object>);
        }

        public UInt32 GetUInt32(string key)
        {
            object ret = this[key];
            if (!(ret is UInt32))
                throw new ErrorTypeException("UInt32", ret);
            return (UInt32)ret;
        }

        public List<UInt32> GetUInt32List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<UInt32>(ret as List<object>);
        }

        public UInt64 GetUInt64(string key)
        {
            object ret = this[key];
            if (!(ret is UInt64))
                throw new ErrorTypeException("UInt64", ret);
            return (UInt64)ret;
        }

        public List<UInt64> GetUInt64List(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<UInt64>(ret as List<object>);
        }

        public float GetFloat(string key)
        {
            object ret = this[key];
            if (!(ret is float))
                throw new ErrorTypeException("float", ret);
            return (float)ret;
        }

        public List<float> GetFloatList(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<float>(ret as List<object>);
        }
        
        public string GetString(string key)
        {
            object ret = this[key];
            if (!(ret is string))
                throw new ErrorTypeException("string", ret);
            return (string)ret;
        }

        public List<string> GetStringList(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<string>(ret as List<object>);
        }

        public DataObj GetDataObj(string key)
        {
            object ret = this[key];
            if (ret == null)
            {
                return null;
            }
            if (!(ret is DataObj))
                throw new ErrorTypeException("DataObj", ret);
            return (DataObj)ret;
        }

        public List<DataObj> GetDataObjList(string key)
        {
            object ret = this[key];
            if (!(ret is List<object>))
                throw new ErrorTypeException("List", ret);
            return ChangeListType<DataObj>(ret as List<object>);
        }

        private List<T> ChangeListType<T>(List<object> oList)
        {
            List<T> tList = new List<T>();
            for (int i = 0; i < oList.Count; ++i)
            {
                tList.Add((T)oList[i]);
            }
            return tList;
        }
    }

}