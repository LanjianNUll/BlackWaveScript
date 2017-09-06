//******************************************************************
// File Name:					WeaponBase.cs
// Description:					WeaponBase class 
// Author:						wuwei
// Date:						2016.12.27
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
using FW.ResMgr;
using System.Text;

namespace FW.Item
{
    /// <summary>
    /// 武器基类
    /// </summary>
    class WeaponBase : EquipmentBase
    {
        delegate WeaponBase WeaponCreator(string id, JsonItem item);
        private static Dictionary<WeaponType, WeaponCreator> sm_creators; 
        //武器共有的属性
        private string m_name;                          //名称
        private float m_injure;                         //伤害值
        private int m_sunderArmor;                      //破甲
        private float m_fireRange;                      //射程
        private int m_boxAmmoCount;                     //弹夹子弹数量上限

        private float[] m_injureScale;                  //部位影响系数
        private float m_poundPower;                     //击中冲力
        private float m_critRatio;                      //暴击率
        private float m_critFilter;                     //暴击系数
        private float m_shootTime;                      //射击 (攻击) 间隔
        private float m_throughForce;                   //穿透值
        private float m_slowTime;                       //停滞时间
        private float m_slowRatio;                      //停滞比例
        private float m_accuracyMax;                    //射击精准度Max
        private string m_bagIcon;                       //背包物品图标
        private int m_tradeType;                        //武器交易类型
        private int m_quality;                          //品质
        private int m_levleLimit;                       //等级限制
        private int m_sellValue;                        //出售价格
        private int m_count;                            //物品数量

        private bool m_isPCEquiped;                     //是否pc端装备
        private string m_FunctionType;                  //武器功能类型

        private Dictionary<AccessoryType, AccessoryBase> m_accessories;                 //配件表
        private Dictionary<AccessoryType, AccessoryBase> m_modifyAccessories;           //改装配件表
        private Dictionary<AccessoryType, bool> m_firstUnistall;                        //是否第一次卸载
        private Dictionary<AccessoryType, int> m_accessoryPorts;

        protected WeaponType m_WeaponType;              //武器类型

        public static ItemBase Create(ItemType type, string id, int resID)
        {
            JsonItem item = DatasMgr.GetJsonItem(resID);
            if(item == null)
            {
                Debug.LogFormat("weapon didn't exist!!!"+ resID);
                return null;
            }
            WeaponType weapontype = (WeaponType)item.Get("maintype").AsInt();
            WeaponCreator creator;
            if (sm_creators.TryGetValue(weapontype, out creator))
            {
                return creator(id, item);
            }
            Debug.LogFormat("weapon  type didn't exist!!!" + weapontype);
            return null;
        }
      
        static WeaponBase()
        {
            sm_creators = new Dictionary<WeaponType, WeaponCreator>();
            sm_creators.Add(WeaponType.Main, MainWeapon.Create);
            sm_creators.Add(WeaponType.Second, SecondaryWeapon.Create);
            sm_creators.Add(WeaponType.Melee, WrestleWeapon.Create);
            sm_creators.Add(WeaponType.Grenade, HandGrenade.Create);
        }

        public WeaponBase(string id, JsonItem item) : base(ItemType.Weapon, id, item)
        {
            this.m_WeaponType = WeaponType.Unknow;
            this.m_accessories = new Dictionary<AccessoryType, AccessoryBase>();
            this.m_modifyAccessories = new Dictionary<AccessoryType, AccessoryBase>();
            this.m_firstUnistall = new Dictionary<AccessoryType, bool>();
            this.Init();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public WeaponType WeaponType { get { return this.m_WeaponType; } }
        public override string Name { get { return Utility.Utility.GetColorStr(this.m_name,this.Quality);}}                                  //名称
        public float Injure { get { return this.m_injure; } }                               //伤害值
        public int  SunderArmor { get { return this.m_sunderArmor; } }                      //破甲
        public float FireRange { get { return this.m_fireRange; } }                         //射程
        public float[] InjureScale { get { return this.m_injureScale; } }                   //部位影响系数
        public float PoundPower { get { return this.m_poundPower; } }                       //击中冲力
        public float CritRatio { get { return this.m_critRatio; } }                         //暴击率
        public float CritFilter { get { return this.m_critFilter; } }                       //暴击系数
        public float ShootTime { get { return this.m_shootTime; } }                         //射击 (攻击) 间隔
        public float ThroughForce { get { return this.m_throughForce; } }                   //穿透值
        public float SlowTime { get { return this.m_slowTime;  } }                          //停滞时间
        public float SlowRatio { get { return this.m_slowRatio;   } }                       //停滞比例
        public int BoxAmmoCount { get { return this.m_boxAmmoCount; } }     
        public float AccuracyMax { get { return this.m_accuracyMax; } }
        public string BagIcon { get { return this.m_bagIcon; } }
        public override string Icon { get { return this.BagIcon; } }
        public override int TradeType { get { return this.m_tradeType; } }
        public override int Quality { get { return this.m_quality; } }
        public override int Levellimit { get { return this.m_levleLimit; } }
        public override int SubType{get{return (int)this.WeaponType; }}
        public override int SellValue { get { return this.m_sellValue; } }
        public override int Count { get { return this.m_count; }  set { this.m_count = value; } }

        public bool IsPCEquiped { get { return this.m_isPCEquiped; } }                      //是否pc端装备
        public int Price { get { return this.JsonItem.Get("price").AsInt(); }}              //商城显示的价格    
        public string Desc { get { return this.JsonItem.Get("desc").AsString(); } }         //武器描述
        public int Currency { get { return this.JsonItem.Get("currency").AsInt(); } }       //购买货币种类

        public int BackAmmoCount { get { return this.JsonItem.Get("backAmmoCount").AsInt(); } }
        public float ReloadTime { get { return this.JsonItem.Get("reloadTime").AsFloat(); } }
        public string FunctionType { get { return this.m_FunctionType; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            if (this.JsonItem == null) return;
            this.m_name = this.JsonItem.Get("name").AsString();                                 //名称
            this.m_injure = this.JsonItem.Get("injure").AsFloat();                              //伤害值
            this.m_sunderArmor = this.JsonItem.Get("sunderArmor").AsInt();                      //破甲
            this.m_fireRange = this.JsonItem.Get("fireRange").AsFloat();                        //射程
            this.m_injureScale = this.JsonItem.Get("injureScale").AsFloats();                   //部位影响系数
            this.m_poundPower = this.JsonItem.Get("poundPower").AsFloat();                      //击中冲力
            this.m_critRatio = this.JsonItem.Get("critRatio").AsFloat();                        //暴击率
            this.m_critFilter = this.JsonItem.Get("critFilter").AsFloat();                      //暴击系数
            this.m_shootTime = this.JsonItem.Get("shootTime").AsFloat();                        //射击 (攻击) 间隔
            this.m_throughForce = this.JsonItem.Get("throughForce").AsFloat();                  //穿透值
            this.m_slowTime = this.JsonItem.Get("slowTime").AsFloat();                          //停滞时间
            this.m_slowRatio = this.JsonItem.Get("slowRatio").AsFloat();                        //停滞比例
            this.m_boxAmmoCount = this.JsonItem.Get("boxAmmoCount").AsInt();
            this.m_accuracyMax = this.JsonItem.Get("accuracyMax").AsFloat();
            this.m_bagIcon = DatasMgr.GetRes(this.JsonItem.Get("bagIcon").AsInt());
            this.m_tradeType = this.JsonItem.Get("tradetype").AsInt();
            this.m_quality = this.JsonItem.Get("quality").AsInt();
            this.m_levleLimit = this.JsonItem.Get("levellimit").AsInt();
            this.m_sellValue = this.JsonItem.Get("sellValue").AsInt();
            GetFunctionType();

            this.m_accessoryPorts = new Dictionary<AccessoryType, int>();
            this.m_accessoryPorts.Add(AccessoryType.Muzzle, this.JsonItem.Get("partMuzzleType").AsInt());
            this.m_accessoryPorts.Add(AccessoryType.Barrel, this.JsonItem.Get("partBarrelType").AsInt());
            this.m_accessoryPorts.Add(AccessoryType.Sight, this.JsonItem.Get("partSightType").AsInt());
            this.m_accessoryPorts.Add(AccessoryType.Maganize, this.JsonItem.Get("partMaganizeType").AsInt());
            this.m_accessoryPorts.Add(AccessoryType.MuzzleSuit, this.JsonItem.Get("partMuzzleSuitType").AsInt());
            this.m_accessoryPorts.Add(AccessoryType.Trigger, this.JsonItem.Get("partTriggerType").AsInt());

            NetDispatcherMgr.Inst.Regist(Commond.Request_putonAccessory_back, OnAssemble);
        }

        //--------------------------------------
        //private 
        //--------------------------------------

        private void GetFunctionType()
        {
            int type = this.JsonItem.Get("type").AsInt();
            if (type == 1)
                this.m_FunctionType = "步枪";
            if (type == 2)
                this.m_FunctionType = "散弹枪";
            if (type == 3)
                this.m_FunctionType = "机枪";
            if (type == 4)
                this.m_FunctionType = "狙击";
            if (type == 5)
                this.m_FunctionType = "手枪";
            if (type == 6)
                this.m_FunctionType = "格斗武器";
            if (type == 7)
                this.m_FunctionType = "手雷";
            if (type == 8)
                this.m_FunctionType = "加特林";
            if (type == 9)
                this.m_FunctionType = "冲锋枪";
        }

        //添加一件物品
        private AccessoryBase AddItem(string id, int resID)
        {
            AccessoryBase accessory = AccessoryBase.Create(ItemType.Accessory, id, resID) as AccessoryBase;
            accessory.IsBind = true;
            if (accessory != null)
            {
                this.m_accessories.Add(accessory.Type, accessory);
            }
            return accessory;
        }

        //移除一个配件
        private AccessoryBase RemoveItem(string id)
        {
            AccessoryBase accessory = GetItem(id);
            if (accessory != null && this.m_accessories.ContainsKey(accessory.Type))
            {
                this.m_accessories.Remove(accessory.Type);
            }
            return null;
        }
        
        //查找一个配件
        private AccessoryBase GetItem(string id)
        {
            foreach (AccessoryBase accessory in this.m_accessories.Values)
            {
                if (accessory.ID == id)
                {
                    return accessory;
                }
            }
            return null;
        }

        //装配返回
        private void OnAssemble(DataObj data)
        {
            if (data == null || data.GetString("weapon_id") != this.ID) return;
            List<DataObj> offDataList = data.GetDataObjList("take_off");
            for (int i = 0; i < offDataList.Count; i++)
            {
                this.RemoveItem(offDataList[i].GetString("id"));
            }
            List<DataObj> onDataList = data.GetDataObjList("put_on");
            for (int i = 0; i < onDataList.Count; i++)
            {
                int resId = onDataList[i].GetInt32("resourceID");
                string id = onDataList[i].GetString("id");
                // AccessoryBase onAccessroy = ItemMgr.GetItem(id) as AccessoryBase;;
                AccessoryBase onAccessroy = this.AddItem(id,resId);
            }
            //清除改装的
            this.clearModify();
            Event.FWEvent.Instance.Call(Event.EventID.Weapon_AccessoryChanged, new Event.EventArg());
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public override void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_putonAccessory_back, OnAssemble);

            base.Dispose();
        }

        //调置pc端装备状态
        public void SetPcEquipState(bool equiped)
        {
            this.m_isPCEquiped = equiped;
            Event.FWEvent.Instance.Call(Event.EventID.Weapon_PCEquipedChanged, new Event.EventArg(this, this.m_isPCEquiped));
        }

        //检查配件接口
        public bool CheckAccessoryPort(AccessoryBase accessory)
        {
            if (accessory == null) return false;
            int port = 0;
            if (this.m_accessoryPorts.TryGetValue(accessory.Type, out port) == false || port <= 0)
                return false;
            int[] ports = accessory.Ports;
            if (ports == null || ports.Length == 0) return false;
            return Utility.Utility.HasItem<int>(ports, port);
        }

        //初始化配件
        public void InitAssemble(List<DataObj> datas)
        {
            this.m_accessories.Clear();
            if (datas == null || datas.Count == 0) return;
            foreach (DataObj data in datas)
            {
                int resId = data.GetInt32("resourceID");
                string id = data.GetString("id");
                this.AddItem(id, resId);
            }
        }

        //获取配件
        public AccessoryBase GetAccessory(AccessoryType type)
        {
            AccessoryBase accessory;
            this.m_modifyAccessories.TryGetValue(type, out accessory);
            if (accessory == null)
            {
                if (this.m_firstUnistall.ContainsKey(type))
                {
                    this.m_firstUnistall.Remove(type);
                    return null;
                }
                this.m_accessories.TryGetValue(type, out accessory);
                return accessory;
            }
            return accessory;
        }
        
        //装配武器配件
        public virtual void Assemble(List<AccessoryBase> accessoryList, List<AccessoryType> aTypeList)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["type"] = (sbyte)this.ItemType;
            data["subtype"] = (sbyte)this.WeaponType;
            data["index"] = this.Index;
            List<object> dataList = new List<object>();
            for (int i = 0; i < accessoryList.Count; i++)
            {
                DataObj data1 = new DataObj();
                data1["x"] = (Int32)aTypeList[i];
                if (accessoryList[i] == null)  //卸载的
                    data1["y"] = -1;
                else
                    data1["y"] = (Int32)accessoryList[i].Index;
                dataList.Add(data1);
            }
            data["parts"] = dataList;
            NetMgr.Instance.Request(Commond.Request_putonAccessory, data);
        }

        //装配到武器  尚未提交的
        public virtual void Assemble(AccessoryBase accessoryBase)
        {
            if (this.m_modifyAccessories.ContainsKey(accessoryBase.Type))
            {
                this.m_modifyAccessories[accessoryBase.Type] = accessoryBase;
            }
            else
            {
                this.m_modifyAccessories.Add(accessoryBase.Type, accessoryBase);
            }
        }

        //卸载配件  尚未提交的
        public virtual void Disperse(AccessoryBase accessoryBase)
        {
            if (this.m_modifyAccessories.ContainsKey(accessoryBase.Type))
            {
                this.m_modifyAccessories[accessoryBase.Type] = null;
            }
            else
            {
                //卸载了记录一个空
                this.m_modifyAccessories.Add(accessoryBase.Type, null);
            }
            this.m_firstUnistall.Add(accessoryBase.Type,true);
        }

        //是否修改了
        public virtual bool IsModify()
        {
            return this.m_modifyAccessories.Count > 0;
        }

        //提交修改
        public virtual void CommitModify()
        {
            //将修改的清理处理
            foreach (AccessoryBase item in m_accessories.Values)
            {
                if (this.m_modifyAccessories.ContainsKey(item.Type))
                {
                    if (item.Equals(this.m_modifyAccessories[item.Type]))
                        this.m_modifyAccessories.Remove(item.Type);
                }
            }
            List<AccessoryBase> valueList = new List<AccessoryBase>();
            List<AccessoryType> typeList = new List<AccessoryType>();
            foreach (AccessoryBase item in m_modifyAccessories.Values)
            {
                valueList.Add(item);
            }
            foreach (AccessoryType item in m_modifyAccessories.Keys)
            {
                typeList.Add(item);
            }
            if (valueList.Count > 0)
            {
                this.Assemble(valueList, typeList);
            }
        }

        //清理修改的
        public virtual void clearModify()
        {
            this.m_modifyAccessories.Clear();
            this.m_firstUnistall.Clear();
        }

        //计算本次修改总价格
        public virtual int TotalModifyPrice()
        {
            int totalPrice = 0;
            foreach (AccessoryBase item in m_modifyAccessories.Values)
            {
                if (item != null)
                    totalPrice += item.EquipValue;
            }
            return totalPrice;
        }
    }
}