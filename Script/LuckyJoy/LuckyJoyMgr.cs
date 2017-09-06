//******************************************************************
// File Name:					LuckyJoyMgr
// Description:					LuckyJoyMgr class 
// Author:						lanjian
// Date:						5/19/2017 9:55:26 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using Network;
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.LuckyJoy
{
    class LuckyJoyMgr
    {
        private static List<LuckyJoyReward> m_LJRewardList;
        private static List<LuckyJoyReward> m_HistoryREList;

        private static List<sbyte> sm_results;                              //中奖的九宫格的luckyJackPot
        private static List<sbyte> sm_luckyLine;
        private static int sm_luckymoney;                                   //中奖的钱
        private static bool sm_isContinous;                                  //是否连续中奖啊

        static LuckyJoyMgr()
        {
            m_LJRewardList = new List<LuckyJoyReward>();
            m_HistoryREList = new List<LuckyJoyReward>();
            sm_luckyLine = new List<sbyte>();
            GetReWardFromJson();
            GetHistroyFromFile();
            NetDispatcherMgr.Inst.Regist(Commond.Request_Play_Slots_back, OnLuckyJoyBack);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------

        //--------------------------------------
        //private 
        //--------------------------------------
        private static void OnLuckyJoyBack(DataObj data)
        {
            if (data == null) return;
            int ret = data.GetUInt16("ret");
            if (ret == 0)
            {
                sm_results = data.GetInt8List("results");
                List<DataObj> d = data.GetDataObjList("reward");
                sm_luckyLine.Clear();
                int[] rewardId = new int[d.Count];
                int[] money = new int[d.Count];
                for (int i = 0; i < d.Count; i++)
                {
                    sm_luckyLine.Add(d[i].GetInt8("line"));
                    rewardId[i] = d[i].GetInt8("reward_id");
                    money[i] = d[i].GetInt32("reward_value");
                }
                //处理中奖纪录
                DoWithHistory(rewardId,money);
                sm_luckymoney = data.GetInt32("value");
                sm_isContinous = (data.GetInt8("is_continuity") == 0) ? false : true;
                FW.Event.FWEvent.Instance.Call(FW.Event.EventID.LuckyJoy_Begin, new Event.EventArg(sm_results, JudgeLuckResult(), sm_luckymoney, sm_isContinous));
            }
            else
            {
                FW.Event.FWEvent.Instance.Call(FW.Event.EventID.LuckyJoy_Fail);
            }
        }

        private static void DoWithHistory(int[] rewardId, int[] money)
        {
            //号线
            //九宫格
            //0   3   6
            //1   4   7
            //2   5   8
            int[][] line = new int[5][] { new int[3] { sm_results[1], sm_results[4], sm_results[7] },
                        new int[3] { sm_results[0], sm_results[3], sm_results[6] },
                        new int[3] { sm_results[2], sm_results[5], sm_results[8] },
                        new int[3] { sm_results[0], sm_results[4], sm_results[8] },
                        new int[3] { sm_results[2], sm_results[4], sm_results[6] },
                };
            for (int i = 0; i < sm_luckyLine.Count; i++)
            {
                int[] groups = line[sm_luckyLine[i]-1];
                //添加到中奖纪录记录
                LuckyJoyReward reward = m_LJRewardList[rewardId[i] - 1];
                reward.BetMoney = money[i];
                reward.ReSetData(groups);
                InsertHistroy(reward);
            }
        }
        
        private static void GetReWardFromJson()
        {
            m_LJRewardList.Clear();
            JsonConfig jsonConfig = DatasMgr.FWLckyGroup;
            for (int i = 0; i < jsonConfig.Data.Count; i++)
            {
                JsonItem jsonItem = jsonConfig.GetJsonItem((i+1).ToString());
                m_LJRewardList.Add(new LuckyJoyReward((i + 1) + "", jsonItem));
            }
        }
        
        /// <summary>
        /// 就保存中奖组合的id和 压中的金额
        /// </summary>
        private static void GetHistroyFromFile()
        {
            m_HistoryREList.Clear();
            JsonConfig jsonConfig = DatasMgr.FWLckyGroup;
            string luckyStr = Login.LoginConfig.LuckyStr;
            if (string.IsNullOrEmpty(luckyStr))
                return;
            string[] idOrBetMoney = luckyStr.Split('|');
            for (int i = 0; i < idOrBetMoney.Length-1; i++)
            {
                int id = int.Parse(idOrBetMoney[i].Split(',')[0]);
                int betMoney = int.Parse(idOrBetMoney[i].Split(',')[1]);
                int num = int.Parse(idOrBetMoney[i].Split(',')[2]);
                JsonItem jsonItem = jsonConfig.GetJsonItem(id.ToString());
                LuckyJoyReward luckyReward = new LuckyJoyReward(id + "", jsonItem);
                int[] group = new int[3];
                group[0] = num / 100;
                group[1] = num / 10 % 10;
                group[2] = num % 10;
                luckyReward.BetMoney = betMoney;
                luckyReward.ReSetData(group);
                InsertHistroy(luckyReward);
            }
        }

        //这里保存的格式和解析的要一致
        private static void WriteHistroyToFile(List<LuckyJoyReward> list)
        {
            string luckyStr = "";
            foreach (var item in list)
            {
                int[] iconGroup = item.Groups;
                string num = iconGroup[0] + "" + iconGroup[1] + "" + iconGroup[2];
                luckyStr += item.Id + ","+ item.BetMoney+","+num+"|";
            }
            Login.LoginConfig.SetLuckyStt(luckyStr);
            Login.LoginConfig.SaveFile();
        }

        private static void InsertHistroy(LuckyJoyReward luckyReward)
        {
            //这里实现队列效果
            List<LuckyJoyReward> list = new List<LuckyJoyReward>();
            list.Add(luckyReward);
            for (int i = 0; i < m_HistoryREList.Count; i++)
            {
                list.Add(m_HistoryREList[i]);
            }
            m_HistoryREList = list;
        }

        ////判断中奖的线
        private static int[] JudgeLuckResult()
        {
            int[] linelucky = new int[5];
            for (int i = 0; i < linelucky.Length; i++)
            {
                linelucky[i] = 0;
            }
            for (int i = 0; i < sm_luckyLine.Count; i++)
            {
                linelucky[sm_luckyLine[i] - 1] = 1;
            }
            return linelucky;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //销毁
        public static void Dispose()
        {
            m_LJRewardList.Clear();
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Play_Slots_back, OnLuckyJoyBack);
        }

        public static void ExitLuckyBet()
        {
            WriteHistroyToFile(m_HistoryREList);
        }

        //获取中奖组合
        public static List<LuckyJoyReward> GetLJReward()
        {
            return m_LJRewardList;
        }

        public static List<LuckyJackPot> GetLuckyItemList()
        {
            List<LuckyJackPot> list = new List<LuckyJackPot>();
            JsonConfig jsonConfig = DatasMgr.FWMSlotCfg;
            for (int i = 0; i < DatasMgr.FWMSlotCfg.Data.Count; i++)
            {
                JsonItem jsonItem = jsonConfig.GetJsonItem((i + 1).ToString());
                 list.Add(new LuckyJackPot((i+1).ToString(), jsonItem));
            }
            return list;
        }
        
        public static List<LuckyJoyReward> GetHistoryReward()
        {
            return m_HistoryREList;
        }

        //开始摇奖请求
        public static void StartLuckyJoy(LuckyBetItem betItem)
        {
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            data["value"] = betItem.BaseBetMoney;
            List<object> line = new List<object>();
            for (int i = 0; i < betItem.BetLine.Length; i++)
            {
                if (betItem.BetLine[i])
                {
                    line.Add((sbyte)(i + 1));    //这里加1是因为线数是从1开始的  到6
                }
            }
            data["lines"] = line;
            NetMgr.Instance.Request(Commond.Request_Play_Slots, data);
        }
    }
}
