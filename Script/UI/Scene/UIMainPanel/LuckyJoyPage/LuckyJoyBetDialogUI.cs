//******************************************************************
// File Name:					LuckyJoyBetDialogUI
// Description:					LuckyJoyBetDialogUI class 
// Author:						lanjian
// Date:						5/18/2017 11:41:15 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.LuckyJoy;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class LuckyJoyBetDialogUI : DialogBaseUI
    {
        protected LuckyJoyBetDialogUI()
        {
            this.m_resName = "UIRootPrefabs/LuckJoyPanel/LuckJoyDialog";
            this.m_DType = DialogType.LuckyJoy;
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.LuckyJoy_Begin,StartLuckyJoy);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.LuckyJoy_Fail, StartLuckyJoyFail);
        }

        public static LuckyJoyBetDialogUI Create()
        {
            return new LuckyJoyBetDialogUI();
        }

        private Transform m_topTra;
        private Transform m_bottomTra;
        private List<Transform> m_betLineArray;                 //下注的线数
        private List<Transform> m_luckyLineArray;               //中奖的线数
        private UILabel m_betCountLabel;                        
        private UILabel m_luckyCountLabel;
        private LuckyBetItem m_luckyBetItem;
        private bool m_isStartJoy= false;                              //是否正在开奖中
        private Transform m_LuckyJoyPoolTran;

        private long m_timer1;
        private long m_timer2;
        private long m_timer3;
        private long m_timer4;
        private long m_timer11;
        private long m_timer22;
        private long m_timer33;
        private long m_timer5;
        private List<LuckyJackPot> m_LJPItems;                  //奖池物品
        private string m_iconPaht = Utility.ConstantValue.LuckyIconPath;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetData(Event.EventArg args)
        {
            m_luckyBetItem = (LuckyBetItem)args[0];
            ShowLuckyUI();
        }

        //显示押注时的线数 和押注 金额
        private void ShowLuckyUI()
        {
            for (int i = 0; i < m_luckyBetItem.BetLine.Length; i++)
            {
                if (m_luckyBetItem.BetLine[i])
                {
                   // m_betLineArray[i].GetComponent<UISprite>().spriteName = "line_01_down";
                    NGUITools.SetActive(m_betLineArray[i].GetChild(0).gameObject, false);
                    NGUITools.SetActive(m_betLineArray[i].GetChild(1).gameObject, true);
                }
            }
            m_betCountLabel.text = m_luckyBetItem.BetMoney+"";
        }

        //显示押注结果和 中奖线 和中奖金额
        private void ShowLuckyResult(int [] reward,int totalmoney)
        {
            for (int i = 0; i < reward.Length; i++)
            {
                if (reward[i] != 0)
                {
                    m_betLineArray[i].GetComponent<UISprite>().spriteName = "line_01_down";
                    NGUITools.SetActive(m_betLineArray[i].GetChild(0).gameObject, false);
                    NGUITools.SetActive(m_betLineArray[i].GetChild(1).gameObject, true);
                    m_luckyLineArray[i].GetComponent<UISprite>().spriteName = "circle_big_down";
                }
            }
            m_luckyCountLabel.text = totalmoney+"";
        }

        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            m_topTra = this.m_DialogUIGo.transform.GetChild(0);
            m_bottomTra = this.m_DialogUIGo.transform.GetChild(1);
            m_LuckyJoyPoolTran = this.m_topTra.GetChild(4);
            //所有的线数
            m_betLineArray = new List<Transform>();
            for (int i = 0; i < this.m_topTra.GetChild(3).childCount; i++)
            {
                m_betLineArray.Add(this.m_topTra.GetChild(3).GetChild(i));
            }
            //中奖的线数
            m_luckyLineArray = new List<Transform>();
            for (int i = 0; i < this.m_bottomTra.GetChild(8).childCount; i++)
            {
                m_luckyLineArray.Add(this.m_bottomTra.GetChild(8).GetChild(i));
            }
            m_betCountLabel = this.m_bottomTra.GetChild(5).GetComponent<UILabel>();
            m_luckyCountLabel = this.m_bottomTra.GetChild(10).GetComponent<UILabel>();
            //获取奖池所有物品
            this.m_LJPItems = LuckyJoy.LuckyJoyMgr.GetLuckyItemList();
            this.GetNeedHideGo();
            this.BindEventLister();
            this.ReSetPosition();
        }

        //设置奖池初始
        private void ReSetPosition()
        {
            Transform rowOne = null;
            for (int i = 0; i < this.m_LuckyJoyPoolTran.childCount; i++)
            {
                rowOne = this.m_LuckyJoyPoolTran.GetChild(i);
                for (int j = 0; j < rowOne.GetChild(0).childCount; j++)
                {
                    rowOne.GetChild(0).GetChild(j).localPosition = new Vector3(0,800-j*600,0);
                }
                // 800  200  -400
                // 0   1    
            }
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            Utility.Utility.GetUIEventListener(m_topTra.GetChild(2)).onClick = OnWinRules;
            Utility.Utility.GetUIEventListener(m_bottomTra.GetChild(1)).onClick = OnBet;
            Utility.Utility.GetUIEventListener(m_bottomTra.GetChild(2)).onClick = OnViewRecord;
            Utility.Utility.GetUIEventListener(m_bottomTra.GetChild(3)).onClick = OnLuckyJoyBegin;
        }

        //中奖规则按钮
        private void OnWinRules(GameObject go)
        {
            this.CloseDialog();
            Utility.Utility.MoveScrollViewTOTarget(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0), new Vector3(0, 0, 0));
            PanelMgr.CurrPanel.SetPageNum(1);
        }

        //下注按钮
        private void OnBet(GameObject go)
        {
            DialogMgr.Load(DialogType.LuckyJoyBetInput);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }
        
        //查看中奖记录
        private void OnViewRecord(GameObject gp)
        {
            this.CloseDialog();
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.Refresh_histroy);
            //快速滑动到指定界面
            Utility.Utility.MoveScrollViewTOTarget(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0), new Vector3(-934,0,0));
            PanelMgr.CurrPanel.SetPageNum(2);
        }

        //开动摇奖
        private void OnLuckyJoyBegin(GameObject go)
        {
            if (m_luckyBetItem == null)
            {
                Utility.Utility.NotifyStr("请重新下注！！");
                return;
            }
            if (m_isStartJoy)
            {
                Utility.Utility.NotifyStr("当前开奖中，请耐心等待结果！！");
                return;
            }
            LuckyJoyMgr.StartLuckyJoy(m_luckyBetItem);
        }
        
        //开始服务器摇奖
        private void StartLuckyJoy(Event.EventArg args)
        {
            //不显示头部的信息
            PanelMgr.DisPlayTopProp(true);
            List<sbyte> m_result = (List<sbyte>)args[0];                //中奖的九宫格
            int[] m_line = (int[])args[1];                              //中奖线数数组
            int money = (int)args[2];                                   //中奖总金额
            bool isContinous = (bool)args[3];                           //是否连续中奖
            float animationTime = 0.5f;
            m_timer1 = Timer.Regist(0, 0.5f, 4, StartMove, animationTime,  0,false,m_result);
            m_timer11 = Timer.Regist(2.0f, 0, 1, StartMove, animationTime, 0, true,m_result);
            m_timer2 = Timer.Regist(0, 0.5f, 9, StartMove, animationTime, 1, false, m_result);
            m_timer22 = Timer.Regist(4.5f, 0, 1, StartMove, animationTime, 1, true, m_result);
            m_timer3 = Timer.Regist(0, 0.5f, 14, StartMove, animationTime, 2, false, m_result);
            m_timer33 = Timer.Regist(7.0f, 0, 1, StartMove, animationTime, 2, true, m_result);

            m_timer4 = Timer.Regist(15 * animationTime, 0, GetResult, m_line , money);
            m_timer5 = Timer.Regist(15 * animationTime, 0.3f, 9, Flash,m_line);

            m_isStartJoy = true;
        }

        bool isfalsh;                       //闪烁每次
        private void Flash(object[] o)
        {
            int[] m_line = (int[])o[0];
            isfalsh = !isfalsh;
            string iconStr = "line_01_up";
            if (isfalsh)
            {
                 iconStr = "line_01_down";
            }
            for (int i = 0; i < m_line.Length; i++)
            {
                if (m_line[i] == 1)
                    m_betLineArray[i].GetComponent<UISprite>().spriteName = iconStr;
            }
        }

        private void StartLuckyJoyFail()
        {
            Utility.Utility.NotifyStr("摇奖失败，你的金钱不足！！");
        }

        bool isagain = false;
        private void StartMove(object[] o)
        {
            float time = (float)o[0];
            int row = (int)o[1];
            bool isStop = (bool)o[2];
            List<sbyte> result = (List<sbyte>)o[3];

            Transform rowOne = this.m_LuckyJoyPoolTran.GetChild(row);
            List<Transform> items = new List<Transform>();
            for (int i = 0; i < rowOne.GetChild(0).childCount; i++)
            {
                items.Add(rowOne.GetChild(0).GetChild(i));
            }
            if (isagain)
            {
                items.Reverse();
                isagain = !isagain;
            }
            else
            {
                isagain = !isagain;
            }
            SetItemRandomPic(items[0],row, isStop,result);
            Vector3[] positon = { new Vector4(0,800,0),new Vector3(0,200,0),new Vector3(0, -400, 0) };
            UITweener.Method TWMethod =  UITweener.Method.Linear;
            Utility.Utility.MoveAnimation(items[0], positon[0], positon[1], time, TWMethod);
            Utility.Utility.MoveAnimation(items[1], positon[1], positon[2], time, TWMethod);
            items[items.Count - 1].localPosition = positon[0];
        }

        private void SetItemRandomPic(Transform item,int row,bool isStop,List<sbyte> result)
        {
            if (isStop)
            {
                for (int i = 0; i < 3; i++)
                {
                    Texture texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + this.m_LJPItems[result[row * 3 + i] - 1].Icon
                    + Utility.ConstantValue.EndIconPath);
                    item.GetChild(i).GetComponent<UITexture>().mainTexture = texture1;
                }
                return;
            }
            //产生随机数
            System.Random random = new System.Random();
            for (int i = 0; i < 3; i++)
            {
                int ran = random.Next(0, 6);
                Texture texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + this.m_LJPItems[ran].Icon
                    + Utility.ConstantValue.EndIconPath);
                item.GetChild(i).GetComponent<UITexture>().mainTexture = texture1;
            }
        }
        
        private void GetResult(object[] o)
        {
            //得到结果判断
            int [] reward = (int[])o[0];
            int luckyMoney = (int)o[1];
            //显示
            ShowLuckyResult(reward,luckyMoney);
            string betLuckyLine = "";
            for (int i = 0; i < m_luckyBetItem.BetLine.Length; i++)
            {
                if(reward[i]!=0 && m_luckyBetItem.BetLine[i])
                    betLuckyLine += (i+1) + " ";
            }
            if (string.IsNullOrEmpty(betLuckyLine))
            {
                Utility.Utility.NotifyStr("很遗憾，本次摇奖你没有中奖！！");
            }
            else
            {
                Utility.Utility.NotifyStr("恭喜你，你押中了 "+ betLuckyLine);
            }
            //清理上次摇奖的记录
            m_isStartJoy = false;
            m_luckyBetItem = null;
            //显示头部的信息
            PanelMgr.DisPlayTopProp(false);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0));
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.GetChild(1));
        }

        public override void OnBack(GameObject go)
        {
            this.CloseDialog();
            //返回主界面
            PanelMgr.BackToMainPanel();
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.GetDialogAbout();
            this.OpenDialog();
            if (args != null)
                GetData(args);
        }

        public override void DisPose()
        {
            Timer.Cancel(this.m_timer1);
            Timer.Cancel(this.m_timer2);
            Timer.Cancel(this.m_timer3);
            Timer.Cancel(this.m_timer11);
            Timer.Cancel(this.m_timer22);
            Timer.Cancel(this.m_timer33);
            Timer.Cancel(this.m_timer4);
            Timer.Cancel(this.m_timer5);
            base.DisPose();
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.LuckyJoy_Begin, StartLuckyJoy);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.LuckyJoy_Fail, StartLuckyJoyFail);
        }
    }
}
