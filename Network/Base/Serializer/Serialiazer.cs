using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

using KeyTNamePair = System.Collections.Generic.KeyValuePair<string, object>;
using KeyTypePair = System.Collections.Generic.KeyValuePair<string, Network.Serializer.NetStreamType>;

namespace Network.Serializer
{
    #region JsonWalker
    // 类型配置文件专用阅读器
    internal class JsonWalker
    {
        private JsonData m_table;
        private bool m_isCmd;

        public JsonWalker(string jsonStr, bool isCmd)
        {
            m_isCmd = isCmd;
            m_table = JsonMapper.ToObject(jsonStr);
            if (m_table == null)
                throw new ConfigErrorException(jsonStr);
        }

        private List<KeyTNamePair> HandleSubItems(JsonData items)
        {
            string aliaName;
            List<KeyTNamePair> subItems = new List<KeyTNamePair>();

            for (int i = 0; i < items.Count; ++i)
            {
                JsonData item = items[i];
                aliaName = ((List<string>)((IDictionary)item).Keys)[0];
                subItems.Add(new KeyTNamePair(aliaName, item[0].ToString()));
            }
            return subItems;
        }

        public IEnumerable<KeyTNamePair> Walk()
        {
            List<String> tableKeys = (List<String>)(((IDictionary)m_table).Keys);
            foreach (string key in tableKeys)
            {
                JsonData itemValue = m_table[key];

                if (!m_isCmd)
                {
                    if (itemValue.IsString)
                    {
                        yield return new KeyTNamePair(key, itemValue.ToString());
                    }
                    else if (itemValue.IsArray)
                    {
                        yield return new KeyTNamePair(key, this.HandleSubItems(itemValue));
                    }
                    else
                    {
                        throw new ConfigErrorException("");
                    }
                }
                else
                {
                    string cmdStr = itemValue["cmd"].ToString();// Convert.ToString((int));
                    yield return new KeyTNamePair(cmdStr, this.HandleSubItems(itemValue["struct"]));
                }
            }
        }
    }
    #endregion

    #region NetStreamSerializer
    class NetStreamSerializer
    {
        static private NetStreamSerializer sm_inst = null;
        //static private Dictionary<string, INetStreamPickler> sm_picklers;
        private Dictionary<string, NetStreamType> m_atypes;

        public NetStreamSerializer()
        {
            this.m_atypes = new Dictionary<string, NetStreamType>();
            m_atypes["int8"] = new Int8NSType();
            m_atypes["int16"] = new Int16NSType();
            m_atypes["int32"] = new Int32NSType();
            m_atypes["int64"] = new Int64NSType();
            m_atypes["uint8"] = new UInt8NSType();
            m_atypes["uint16"] = new UInt16NSType();
            m_atypes["uint32"] = new UInt32NSType();
            m_atypes["uint64"] = new UInt64NSType();
            m_atypes["float"] = new FloatNSType();
            m_atypes["bool"] = new BoolNSType();
            m_atypes["string"] = new StringNSType();
        }

        static NetStreamSerializer()
        {
            //sm_picklers = new Dictionary<string, INetStreamPickler>();
        }

        //static public void AddPickler(string name, INetStreamPickler pickler)
        //{
        //    //Debug.Assert(!sm_picklers.ContainsKey(name));
        //    sm_picklers[name] = pickler;
        //}

        static public NetStreamSerializer inst()
        {
            if (sm_inst == null)
                sm_inst = new NetStreamSerializer();
            return sm_inst;
        }

        // --------------------------------------------------------------------
        // private
        // --------------------------------------------------------------------

        private NetStreamType GetNetType(string key)
        {
            if (m_atypes.ContainsKey(key))
                return m_atypes[key];
            UnionNSType type = new UnionNSType();
            this.m_atypes.Add(key, type);
            return type;
        }

        #region
        private NetStreamType HandleTypeItem(string file,string key, object item)
        {
            if (item is string)
            {
                string typeName = (string)item;
                if (typeName.EndsWith("[]"))                                // 类型名称: 类型[]
                {
                    typeName = typeName.Remove(typeName.Length - 2, 2);
                    return new ArrayNSType(this.GetNetType(typeName));
                }
                else                                                        // 类型名称: 类型 
                {
                    return this.GetNetType(typeName);
                }
            }
            else if (item is List<KeyTNamePair>)                              // 类型: [{xx}, {xx}...]
            {
                UnionNSType types =  this.GetNetType(key) as UnionNSType;
                List<KeyTypePair> keyTypes = new List<KeyTypePair>();
                foreach (KeyTNamePair keyTName in (List<KeyTNamePair>)item)
                {
                    NetStreamType nstype = this.HandleTypeItem(file, keyTName.Key, keyTName.Value);
                    keyTypes.Add(new KeyTypePair(keyTName.Key, nstype));
                }
                types.SetTypes(keyTypes);
                return types;
            }
            throw new ConfigErrorException(file);
        }

        #endregion
        // --------------------------------------------------------------------
        // public
        // --------------------------------------------------------------------
        public void AddTypeConfig(string jsonStr, bool isCmd)
        {
            foreach (KeyTNamePair item in (new JsonWalker(jsonStr, isCmd)).Walk())
            {
                m_atypes[item.Key] = this.HandleTypeItem(jsonStr, item.Key, item.Value);
            }
        }

        public void WriteToStream(BinaryWriter stream, string tname, object value)
        {
            if (!this.m_atypes.ContainsKey(tname))
                throw new UnexistTypeException(tname);
            //if (sm_picklers.ContainsKey(tname))
            //{
            //    if (!typeof(INetStreamObject).IsInstanceOfType(value))
            //        throw new ErrorTypeException(tname, value);
            //    value = sm_picklers[tname].PickFromNetStreamObject((INetStreamObject)value);
            //}
            this.m_atypes[tname].WriteToStream(tname, stream, value);

        }

        public Int64 ReadFromStream(BinaryReader stream, string tname, out object data)
        {
            if (!this.m_atypes.ContainsKey(tname))
                throw new UnexistTypeException(tname);
            return this.m_atypes[tname].ReadFromStream(stream,out data);
            //if (sm_picklers.ContainsKey(tname))
            //    return sm_picklers[tname].UnpackToNetStreamObject(data);
        }
    }

    #endregion
}