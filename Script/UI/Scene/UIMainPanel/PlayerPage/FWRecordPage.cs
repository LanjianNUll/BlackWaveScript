//******************************************************************
// File Name:					FWRecordPage
// Description:					FWRecordPage class 
// Author:						lanjian
// Date:						1/13/2017 2:51:31 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Role;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class FWRecordPage : ScrollViewItemBase
    {
        protected FWRecordPage()
        {
            this.m_PageName = "UIRootPrefabs/PlayerPanel_PageItem/fwRecordPage";
            this.m_PageIndex = 1;
            this.Init();
            //注册
           this.FillItem(null);
        }

        public static FWRecordPage Create()
        {
            return new FWRecordPage();
        }

        private List<GameObject> m_ItemList;
        private List<Statistic> m_HonorDataList;
        private List<Statistic> m_StatisticDataList;
        private UIToggledObjects m_PVPUIToggledObjects;
        private UIToggledObjects m_PVEUIToggledObjects;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDataList()
        {
            m_HonorDataList = Role.Role.Instance().StatisitcProctor.GetDataList(ShowType.Honor);
            m_StatisticDataList = Role.Role.Instance().StatisitcProctor.GetDataList(ShowType.Statistic);
        }

        private void LoadItem()
        {
            LoadGameObject();
            FillDataToGo();
        }
        
        private void LoadGameObject()
        {
            string path = "UIRootPrefabs/PlayerPanel_PageItem/Itemprefabs/PageRecord";
            m_ItemList =  this.LoadItemPath(1,path, 
                this.CurrentItem.transform.GetChild(2).GetChild(0));
            ////顶格
            //Utility.Utility.ModifyItemT0p(this.CurrentItem.transform.GetChild(2).gameObject, new Vector3(0, 510, 0));
            //不满一页是  禁止滑动
            this.CurrentItem.transform.GetChild(2).GetComponent<UIScrollView>().enabled = false;
        }

        private void FillDataToGo()
        {
            List<Transform> pvpHDataList = new List<Transform>();
            List<Transform> PveHDataList = new List<Transform>();
            List<Transform> pvpSDataList = new List<Transform>();
            List<Transform> PveSDataList = new List<Transform>();
            //清理加入activate的go
            m_PVPUIToggledObjects.activate.Clear();
            m_PVEUIToggledObjects.activate.Clear();
            

            Transform tabHonor = this.CurrentItem.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            Transform tabStatistic = this.CurrentItem.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1);

            for (int i = 1; i < tabHonor.childCount; i++)
            {
                m_PVPUIToggledObjects.activate.Add(tabHonor.GetChild(i).GetChild(0).gameObject);
                pvpHDataList.Add(tabHonor.GetChild(i).GetChild(0));
                m_PVEUIToggledObjects.activate.Add(tabHonor.GetChild(i).GetChild(1).gameObject);
                PveHDataList.Add(tabHonor.GetChild(i).GetChild(1));
            }

            for (int i = 1; i < tabStatistic.childCount; i++)
            {
                m_PVPUIToggledObjects.activate.Add(tabStatistic.GetChild(i).GetChild(0).gameObject);
                pvpSDataList.Add(tabStatistic.GetChild(i).GetChild(0));
                m_PVEUIToggledObjects.activate.Add(tabStatistic.GetChild(i).GetChild(1).gameObject);
                PveSDataList.Add(tabStatistic.GetChild(i).GetChild(1));
            }

            FillDataHonor(pvpHDataList, CarrerType.PVP);
            FillDataHonor(PveHDataList, CarrerType.PVE);
            FillDataStatistic(pvpSDataList, CarrerType.PVP);
            FillDataStatistic(PveSDataList, CarrerType.PVE);
        }

        //填充荣誉统计数据
        private void FillDataHonor(List<Transform> list, CarrerType type)
        {
            List<Statistic> dataList = new List<Statistic>();
            foreach (Statistic item in m_HonorDataList)
            {
                if (item.CType == type)
                    dataList.Add(item);
            }
            List<Transform> dataTranList = new List<Transform>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].childCount; j++)
                {
                    dataTranList.Add(list[i].GetChild(j));
                }
            }

            //隐藏
            for (int i = 0; i < dataTranList.Count; i++)
            {
                NGUITools.SetActive(dataTranList[i].gameObject, false);
            }
            
            for (int i = 0; i < dataList.Count; i++)
            {
                NGUITools.SetActive(dataTranList[i].gameObject, true);
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.RoleIcon + "/" + dataList[i].SIcon);
                dataTranList[i].GetChild(1).GetComponent<UITexture>().mainTexture = texture;
                dataTranList[i].GetChild(2).GetComponent<UILabel>().text = dataList[i].ShowName;
                dataTranList[i].GetChild(3).GetComponent<UILabel>().text = dataList[i].Value.ToString();
            }
        }

        //填充数据统计数据
        private void FillDataStatistic(List<Transform> list, CarrerType type)
        {
            List<Statistic> dataList = new List<Statistic>();
            foreach (Statistic item in m_StatisticDataList)
            {
                if (item.CType == type)
                    dataList.Add(item);
            }
            List<Transform> dataTranList = new List<Transform>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].childCount; j++)
                {
                    dataTranList.Add(list[i].GetChild(j));
                }
            }

            for (int i = 0; i < dataList.Count; i++)
            {
                NGUITools.SetActive(dataTranList[i].gameObject, true);
                dataTranList[i].GetChild(0).GetComponent<UILabel>().text = dataList[i].ShowName;
                dataTranList[i].GetChild(1).GetComponent<UILabel>().text = dataList[i].Value.ToString();
            }

            for (int i = dataTranList.Count-1; i >= dataList.Count; i--)
            {
                UnityEngine.GameObject.Destroy(dataTranList[i].gameObject);
            }
        }
       
        //--------------------------------------
        //public
        //--------------------------------------

        public override void FillItem(FW.Event.EventArg eventArg)
        {
            GetDataList();
            this.CurrentItem.transform.Find("titleBg/title").GetComponent<UILabel>().text = "荣誉统计";
            m_PVPUIToggledObjects = this.CurrentItem.transform.Find("titleBg/PVPBtn").GetComponent<UIToggledObjects>();
            m_PVEUIToggledObjects = this.CurrentItem.transform.Find("titleBg/PVEBtn").GetComponent<UIToggledObjects>();
            LoadItem();
        }

        public override void DisPose()
        {
            //注册
            base.DisPose();
        }
    }
}
