//******************************************************************
// File Name:					LuckyJoyHistoryPage
// Description:					LuckyJoyHistoryPage class 
// Author:						lanjian
// Date:						5/10/2017 4:57:04 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.LuckyJoy;

namespace FW.UI
{
    class LuckyJoyHistoryPage: ScrollViewItemBase
    {
        protected LuckyJoyHistoryPage()
        {
            this.m_PageName = "UIRootPrefabs/LuckJoyPanel/LuckJoyHistory";
            this.m_PageIndex = 2;
            this.Init();
            FillItem(null);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Refresh_histroy, RefreshHistory);
        }

        public static LuckyJoyHistoryPage Create()
        {
            return new LuckyJoyHistoryPage();
        }

        private List<LuckyJoyReward> m_RewardHis;
        private List<GameObject> m_RewardGOList = new List<GameObject>();
        private string m_iconPaht = Utility.ConstantValue.LuckyIconPath;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetData()
        {
            m_RewardHis = LuckyJoyMgr.GetHistoryReward();
        }

        private void FillData()
        {
            if (m_RewardHis.Count != 0)
            {
                string luckyRecord = "UIRootPrefabs/LuckJoyPanel/RecordItem/luckyRecord";
                GameObject prefab = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(luckyRecord) as GameObject);
                m_RewardGOList = this.LoadItem(m_RewardHis.Count, prefab, this.CurrentItem.transform.GetChild(1).GetChild(0).GetChild(0));
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(0), new Vector3(0, 483, 0));
                if (m_RewardGOList.Count < 10)
                {
                    this.CurrentItem.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().enabled = false;
                }

                for (int i = 0; i < m_RewardGOList.Count; i++)
                {
                    Texture texture1 = null;
                    if (m_RewardHis[i].JcakPotArray[0].Icon == null)
                        texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + "dice");
                    else
                        texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + m_RewardHis[i].JcakPotArray[0].Icon
                         + Utility.ConstantValue.EndIconPath);

                    Texture texture2 = null;
                    if (m_RewardHis[i].JcakPotArray[1].Icon == null)
                        texture2 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + "dice");
                    else
                        texture2 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + m_RewardHis[i].JcakPotArray[1].Icon
                         + Utility.ConstantValue.EndIconPath);

                    Texture texture3 = null;
                    if (m_RewardHis[i].JcakPotArray[2].Icon == null)
                        texture3 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + "dice");
                    else
                        texture3 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + m_RewardHis[i].JcakPotArray[2].Icon
                         + Utility.ConstantValue.EndIconPath);
                    m_RewardGOList[i].transform.GetChild(1).GetComponent<UILabel>().text = m_RewardHis[i].BetMoney+"";
                    m_RewardGOList[i].transform.GetChild(2).GetComponent<UITexture>().mainTexture = texture1;
                    m_RewardGOList[i].transform.GetChild(3).GetComponent<UITexture>().mainTexture = texture2;
                    m_RewardGOList[i].transform.GetChild(4).GetComponent<UITexture>().mainTexture = texture3;
                    //调整位置
                    m_RewardGOList[i].transform.localPosition = new Vector3(0,-135*i,0);
                }
            }
        }

        private void RefreshHistory()
        {
            foreach (var item in m_RewardGOList)
            {
                //这里立即销毁否则会影响再次生成的位置，这里先这样处理
                UnityEngine.GameObject.DestroyImmediate(item);
            }
            m_RewardGOList.Clear();
            this.FillItem(null);
        }
        //--------------------------------------
        //public
        //--------------------------------------

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Refresh_histroy, RefreshHistory);
            base.DisPose();
        }

        public override void FillItem(EventArg eventArg)
        {
            GetData();
            FillData();
        }
    }
}

