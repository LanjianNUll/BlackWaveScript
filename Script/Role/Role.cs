//******************************************************************
// File Name:					Role.cs
// Description:					Role class 
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

using FW.Player;
using FW.Item;
using Network;
using Network.Serializer;
using FW.Exchange;

namespace FW.Role
{
    delegate void SetRolePointFunc(Int32 value);
    class Role
    {
        private KitBagProctor m_kitbagProctor;              //背包系统
        private MailProctor m_mailProctor;                  //邮件系统
        private GamePlayerProctor m_gamePlayerProctor;      //好友系统
        private StatisticProctor m_statisticProctor;        //统计数据
        private PersonImageProctor m_personImageProctor;    //角色形像
        private MedelProctor m_medelProctor;                //勋章信息
        private ProficencyProctor m_proficencyProctor;      //熟练度信息
        private ExchangePrizeProctor m_exchangePrizeProctor;//兑换公告信息

        private Dictionary<WeaponType, string> m_equippedPCItems;
        private Dictionary<CurrencyType, int> m_currencyCounts;//货币数量

        private Dictionary<RolePointType, SetRolePointFunc> m_SetPointFuncs;

        private bool m_isInited;                            //是否初始化
        private RoleOnlineState m_onlineState;              //在线状态
        private int m_InputErrorCount;                      //兑换界面输入错误次数
        private Gameplayer m_ToGamePrivateChat;             //跳到私聊去的玩家
        private bool m_isOpenAnnoucement;                     //是否打开过兑换公告

        private int m_id;                                   //id
        private string m_name;                              //名称
        private sbyte m_grade;                              //性别
        private int m_level;                                //等级
        private int m_modelId;                              //模型id
        private int m_isNewLevelOver;                       //是否完成新手关卡
        private int m_exp;                                  //经验
        private int m_expMax;                               //最大经验值
        private int m_point;                                //天赋点
        private bool m_hasChangeName;                       //是否改过名字

        private static Role sm_instance = null;
        public static Role Instance()
        {
            if(sm_instance == null)
                sm_instance = new Role();
            return sm_instance;
        }

        private Role()
        {
            this.m_kitbagProctor = new KitBagProctor(this);
            this.m_mailProctor = new MailProctor(this);
            this.m_gamePlayerProctor = new GamePlayerProctor();
            this.m_statisticProctor = new StatisticProctor();
            this.m_personImageProctor = new PersonImageProctor();
            this.m_medelProctor = new MedelProctor();
            this.m_proficencyProctor = new ProficencyProctor();
            this.m_exchangePrizeProctor = new ExchangePrizeProctor();

            this.m_equippedPCItems = new Dictionary<WeaponType, string>();
            this.m_currencyCounts = new Dictionary<CurrencyType, int>();
            this.m_currencyCounts.Add(CurrencyType.Cash, 0);
            this.m_currencyCounts.Add(CurrencyType.Diamond, 0);
            this.m_currencyCounts.Add(CurrencyType.Gold, 0);
            this.m_isInited = false;

            this.m_SetPointFuncs = new Dictionary<RolePointType, SetRolePointFunc>();
            this.m_SetPointFuncs.Add(RolePointType.Cash, this.ChangeCash);
            this.m_SetPointFuncs.Add(RolePointType.Diamond, this.ChangeDiamond);
            this.m_SetPointFuncs.Add(RolePointType.Gold, this.ChangeGold);

            NetDispatcherMgr.Inst.Regist(Commond.Request_Role_back, OnRoleInfo);
            NetDispatcherMgr.Inst.Regist(Commond.Create_Role_back, OnCreateRole);
            NetDispatcherMgr.Inst.Regist(Commond.Role_Change_Point, OnChangePoint);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Online_back, OnRoleOnline);
            
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return m_id; } }                                  //id
        public string Name { get { return m_name; } }                           //名称
        public sbyte Grade { get { return m_grade; } }                          //性别
        public int Level { get { return m_level; } }                            //等级
        public int ModelID{ get { return m_modelId; } }                         //模型id
        public int IsNewLevelOver { get { return m_isNewLevelOver;  } }         //是否完成新手关卡
        public int Cash { get { return this.m_currencyCounts[CurrencyType.Cash]; } }//现金
        public int Diamond { get { return this.m_currencyCounts[CurrencyType.Diamond]; } }//钻石
        public int Gold { get { return this.m_currencyCounts[CurrencyType.Gold]; } }//金币
        public int Exp{ get { return m_exp;  } }                                //经验
        public int ExpMax{ get { return m_expMax;  } }                          //最大经验值
        public int Point{ get { return m_point;  } }                            //天赋点
        public bool HasChangeName { get { return m_hasChangeName; } }           //是否改过名字


        public RoleOnlineState OnlineState { get { return this.m_onlineState; } }//在线状态
        public int InputErrorCount { get { return this.m_InputErrorCount; } }    //兑换界面输入密码错误次数
        public Gameplayer ToGamePrivateChat { get { return this.m_ToGamePrivateChat; } set { this.m_ToGamePrivateChat = value; } }
        public bool IsOpenAnn { get { return this.m_isOpenAnnoucement; } set { this.m_isOpenAnnoucement = value; } }

        public KitBagProctor KitBag { get { return m_kitbagProctor; } }
        public MailProctor MailProctor { get { return m_mailProctor; } }
        public GamePlayerProctor GamePlayerProctor{get{return m_gamePlayerProctor;}}
        public StatisticProctor StatisitcProctor { get { return m_statisticProctor; } }
        public PersonImageProctor PersonImageProctor { get { return m_personImageProctor; } }
        public MedelProctor MedelProctor { get { return m_medelProctor; } }
        public ProficencyProctor ProficencyProctor { get { return m_proficencyProctor; } }
        public ExchangePrizeProctor ExchangePrizeProctor { get { return m_exchangePrizeProctor; } }

        //--------------------------------------
        //private 
        //--------------------------------------

        //创建角色
        private void RequestCreateRole()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["name"] = "";
            data["gender"] = (sbyte)0;
            NetMgr.Instance.Request(Commond.Create_Role, data);
        }

        //创建角色返回
        private void OnCreateRole(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if (ret != 0)
            {
                Debug.LogFormat("create role failed!!!错误码"+ret);
                Debug.LogFormat("0:成功, 1:失败,2:名字重复,3:名字太长,4:该帐号已有角色，不能再创建  5名字包含敏感词");
                ///*0:成功, 1:失败,2:名字重复,3:名字太长,4:该帐号已有角色，不能再创建  5名字包含敏感词*/
            }
        }

        //请求角色信息
        private void RequestRoleInfo()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Role, data);
        }

        //请求角色信息返返回
        private void OnRoleInfo(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            if(ret != 0)
            {
                RequestCreateRole();
                return;
            }

            this.m_id = data.GetInt32("id");                                    //id
            this.m_name = data.GetString("name");                               //名称
            this.m_grade = data.GetInt8("gender");                              //性别
            int level = data.GetInt32("level");                                 //等级
            this.m_modelId = data.GetInt32("modelID");                          //模型id
            this.m_isNewLevelOver = data.GetInt8("isFinishNewLevel");           //是否完成新手关卡
            int exp = data.GetInt32("exp");                                     //经验
            int expMax = data.GetInt32("max_exp");                              //最大经验值
            int point = data.GetInt32("talent");                                //天赋点
            this.m_hasChangeName = data.GetInt8("hasChangeName") == 1;
            this.m_InputErrorCount = 0;
            //货币修改
            this.ChangeCurrency(data);
            //修改等级
            this.ChangeLevel(level);
            //修改经验值
            this.ChangeExp(exp, expMax);
            //修改天赋点数
            this.ChangePoint(point);

            //初始化背包
            if (this.KitBag != null)
            {
                this.KitBag.Init();
            }

            //初始化邮件
            if(this.MailProctor != null)
            {
                this.MailProctor.Init();
            }

            //初始化聊天模块
            Chat.ChatMgr.Instance.Init();

            //初始化统计数据
            if (this.StatisitcProctor != null)
                this.StatisitcProctor.Init();

            //初始化角色形象数据
            if (this.PersonImageProctor != null)
                this.PersonImageProctor.Init();

            //初始化角色勋章信息
            if (this.MedelProctor != null)
                this.MedelProctor.Init();

            //初始化熟练度信息
            if (this.ProficencyProctor != null)
                this.ProficencyProctor.Init();

            //初始化好友信息
            if (this.GamePlayerProctor != null)
                this.GamePlayerProctor.Init();

            //初始化公告
            if (this.ExchangePrizeProctor != null)
                this.ExchangePrizeProctor.Init();

            //请求玩家在线消息
            RequestRoleOnline();

            Event.FWEvent.Instance.Call(Event.EventID.Role_inited);
        }

        //请求玩家在线消息
        private void RequestRoleOnline()
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Online, data);
        }

        //请求玩家在线消息
        private void OnRoleOnline(DataObj data)
        {
            if (data == null) return;
            UInt16 ret = data.GetUInt16("ret");
            this.m_onlineState = (RoleOnlineState)ret;
            Event.FWEvent.Instance.Call(Event.EventID.Role_online_Change, new Event.EventArg(this, this.m_onlineState));
        }

        //响应消耗点数改变
        private void OnChangePoint(DataObj data)
        {
            if (data == null) return;
            int level = data.GetInt32("level");                                 //等级
            int exp = data.GetInt32("exp");                                     //经验
            int expMax = data.GetInt32("max_exp");                              //最大经验值
            int point = data.GetInt32("talent");                                 //天赋点
            //货币修改
            foreach(DataObj curdata in data.GetDataObjList("point"))
            {
                RolePointType type = (RolePointType)curdata.GetInt8("type");
                int value = curdata.GetInt32("value");
                this.ChangePoints(type, value);
            }
            //修改等级
            this.ChangeLevel(level);
            //修改经验值
            this.ChangeExp(exp, expMax);
            //修改天赋点数
            this.ChangePoint(point);
        }
        
        //改变点数
        private void ChangePoints(RolePointType type, int value)
        {
            SetRolePointFunc funcs;
            if(this.m_SetPointFuncs.TryGetValue(type, out funcs))
            {
                funcs(value);
            }
        }

        //改变货币数量
        private void ChangeCurrency(DataObj data)
        {
            if (data == null) return;
            int cash = data.GetInt32("cash");                                   //现金
            int diamond = data.GetInt32("diamond");                             //钻石
            int gold = data.GetInt32("gold");                                   //金币

            this.ChangeCash(cash);
            this.ChangeDiamond(diamond);
            this.ChangeGold(gold);
        }

        //改变现金
        private void ChangeCash(int cash)
        {
            this.m_currencyCounts[CurrencyType.Cash] = cash < 0 ? 0 : cash;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Cash, new Event.EventArg(this, cash));
        }

        //改变钻石
        private void ChangeDiamond(int diamond)
        {
            this.m_currencyCounts[CurrencyType.Diamond] = diamond < 0 ? 0 : diamond;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Diamond, new Event.EventArg(this, diamond));
        }

        //改变黄金
        private void ChangeGold(int gold)
        {
            this.m_currencyCounts[CurrencyType.Gold] = gold < 0 ? 0 : gold;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Gold, new Event.EventArg(this, gold));
        }

        //修改等级
        private void ChangeLevel(int level)
        {
            this.m_level = level;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Level, new Event.EventArg(this, this.Level));
        }

        //修改经验值
        private void ChangeExp(int exp, int mxpMax)
        {
            this.m_exp = exp;
            this.m_expMax = ExpMax;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Exp, new Event.EventArg(this, this.Exp, this.ExpMax));
        }

        //修改天赋点数
        private void ChangePoint(int point)
        {
            this.m_point = point;
            Event.FWEvent.Instance.Call(Event.EventID.Role_Change_Point, new Event.EventArg(this, this.Point));
        }

        //--------------------------------------
        //public 
        //--------------------------------------

        //初始化
        public void Init()
        {
            if (this.m_isInited) return;
            this.m_isInited = true;
            //先请求玩家信息
            RequestRoleInfo();
        }

        //销毁
        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Role_back, OnRoleInfo);
            NetDispatcherMgr.Inst.UnRegist(Commond.Create_Role_back, OnCreateRole);
            NetDispatcherMgr.Inst.UnRegist(Commond.Role_Change_Point, OnChangePoint);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Online_back, OnRoleOnline);
            this.Reset();
        }

        //重置状态
        public void Reset()
        {
            if (this.KitBag != null)
            {
                this.KitBag.Dispose();
            }
            //清理邮件系统
            if (this.MailProctor != null)
            {
                this.MailProctor.Dispose();
            }
            //清理数据
            if (this.StatisitcProctor != null)
            {
                this.StatisitcProctor.Dispose();
            }
            //清理形象
            if (this.PersonImageProctor != null)
            {
                this.PersonImageProctor.Dispose();
            }
            //清理勋章信息
            if (this.MedelProctor != null)
            {
                this.MedelProctor.Dispose();
            }
            //清理熟练度信息
            if (this.ProficencyProctor != null)
            {
                this.ProficencyProctor.Dispose();
            }
            //清理好友系统
            if (this.GamePlayerProctor != null)
            {
                this.GamePlayerProctor.Dispose();
            }
            //清理公告
            if (this.ExchangePrizeProctor != null)
            {
                this.ExchangePrizeProctor.Dispose();
            }
            //清理聊天系统
            Chat.ChatMgr.Instance.Dispose();
            this.m_isInited = false;
        }

        //卸载pc端武器
        public void DispersPcWeapon(WeaponType type)
        {
            string id;
            if(this.m_equippedPCItems.TryGetValue(type, out id))
            {
                WeaponBase weapon = ItemMgr.GetItem(id) as WeaponBase;
                if(weapon != null)
                {
                    weapon.SetPcEquipState(false);
                }
            }
        }

        //装配pc端武器
        public void EquipPcWeapon(WeaponType type, string id)
        {
            WeaponBase weapon = ItemMgr.GetItem(id) as WeaponBase;
            if (weapon == null) return;
            weapon.SetPcEquipState(true);
            this.m_equippedPCItems.Add(type, id);
        }

        //装配pc端武器
        public void EquipPcWeapon(List<DataObj> datas)
        {
            this.m_equippedPCItems.Clear();
            if (datas == null || datas.Count == 0) return;
            foreach (DataObj equipData in datas)
            {
                WeaponType type = (WeaponType)equipData.GetInt8("type");
                if(this.m_equippedPCItems.ContainsKey(type))
                {
                    this.DispersPcWeapon(type);
                }
                string id = equipData.GetString("unique_id");
                this.EquipPcWeapon(type, id);
            }
        }

        //获取已装备武器
        public WeaponBase GetEquipedWeapon(WeaponType type)
        {
            WeaponBase weapon = null;
            string id;
            if (this.m_equippedPCItems.TryGetValue(type, out id))
            {
                weapon = ItemMgr.GetItem(id) as WeaponBase;
            }
            return weapon;
        }

        //输入密码错误
        public void InputErrorPS()
        {
            this.m_InputErrorCount++;
        }
    }
}