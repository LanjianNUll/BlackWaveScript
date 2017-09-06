//******************************************************************
// File Name:					FWMedalPage
// Description:					FWMedalPage class 
// Author:						lanjian
// Date:						1/13/2017 1:58:00 PM
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
    class FWMedalPage:ScrollViewItemBase
    {
        protected FWMedalPage()
        {
            this.m_PageName = "UIRootPrefabs/PlayerPanel_PageItem/fwMedalPage";
            this.m_PageIndex = 3;
            this.Init();
            FillItem(null);
        }

        public static FWMedalPage Create()
        {
            return new FWMedalPage();
        }

        private List<GameObject> m_ItemList;
        private List<Medel> m_medelList;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetMedelList()
        {
            m_medelList = Role.Role.Instance().MedelProctor.GetMedelList();
        }

        private void LoadPicList()
        {
            string path = "UIRootPrefabs/PlayerPanel_PageItem/Itemprefabs/medelPictureList";
            m_ItemList = this.LoadItemPath((m_medelList.Count / 9)+1, path,
                this.CurrentItem.transform.GetChild(1).GetChild(0));
            //顶格
            Utility.Utility.ModifyItemT0p(this.CurrentItem.transform.GetChild(1).gameObject, new Vector3(0, -70, 0));
            //不满一页是  禁止滑动
            if (m_medelList.Count <= 9)
                this.CurrentItem.transform.GetChild(1).GetComponent<UIScrollView>().enabled = false;
            FillDataToUI();
        }

        private void FillDataToUI()
        {
            for (int i = 0; i < m_ItemList.Count; i++)
            {
                FillDataPage(m_ItemList[i],i);
            }
        }

        //填充一页
        private void FillDataPage(GameObject go,int pageNum)
        {
            List<Transform> picList = new List<Transform>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
                picList.Add(go.transform.GetChild(i));
                Utility.Utility.GetUIEventListener(go.transform.GetChild(i)).onClick = OnHideFlow;
            }
            int displayNum = (pageNum + 1) * 9 < m_medelList.Count ? 9 : m_medelList.Count - pageNum * 9;
            int WBeginIndex = pageNum * 9;
            //隐藏
            for (int i = 0; i < 9; i++)
            {
                NGUITools.SetActive(picList[i].gameObject, false);
            }
            for (int i = 0; i < displayNum; i++)
            {
                Transform content = picList[i].GetChild(2).GetChild(0);
                content.GetChild(0).GetComponent<UILabel>().text = m_medelList[i+ WBeginIndex].Name;
                content.GetChild(3).GetComponent<UILabel>().text = m_medelList[i + WBeginIndex].GetWayDesc;
                string iconPath = "";
                if (m_medelList[i + WBeginIndex].GetTime == -1)
                {
                    iconPath = Utility.ConstantValue.RoleIcon + "/" + m_medelList[i + WBeginIndex].ICon+"_gray";
                    content.GetChild(1).GetComponent<UILabel>().text = "尚未获得";
                    picList[i].GetComponent<UITexture>().color = Color.gray;
                    picList[i].GetChild(4).GetComponent<UILabel>().text = m_medelList[i + WBeginIndex].Tip1;
                    if (string.IsNullOrEmpty(m_medelList[i + WBeginIndex].Tip1))
                        picList[i].GetChild(4).GetComponent<UILabel>().text = m_medelList[i + WBeginIndex].Tip2;
                    else
                    {
                        picList[i].GetChild(5).GetComponent<UILabel>().text = m_medelList[i + WBeginIndex].Tip2;
                    }
                }
                else
                {
                    picList[i].GetChild(4).GetComponent<UILabel>().text = "[66A1BCFF]" + m_medelList[i + WBeginIndex].Tip1 + "[-]";
                    if (string.IsNullOrEmpty(m_medelList[i + WBeginIndex].Tip1))
                        picList[i].GetChild(4).GetComponent<UILabel>().text = "[66A1BCFF]" + m_medelList[i + WBeginIndex].Tip2 + "[-]";
                    {
                        picList[i].GetChild(5).GetComponent<UILabel>().text = "[66A1BCFF]" + m_medelList[i + WBeginIndex].Tip2 + "[-]";
                    }
                    iconPath = Utility.ConstantValue.RoleIcon + "/" + m_medelList[i + WBeginIndex].ICon;
                    content.GetChild(1).GetComponent<UILabel>().text = Utility.Utility.Int2DateAndSecondNOtime(m_medelList[i + WBeginIndex].GetTime)+" 获得";
                }
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath);
                picList[i].GetChild(1).GetComponent<UITexture>().mainTexture = texture;
                picList[i].GetComponent<UITexture>().mainTexture = texture;
                NGUITools.SetActive(picList[i].gameObject, true);
            }
        }

        //再次点击隐藏Tip
        string lastid = "";
        private void OnHideFlow(GameObject go)
        {
            GameObject cont = go.transform.GetComponent<UIToggledObjects>().activate[0];
            string id = go.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<UILabel>().text;
            if (!string.IsNullOrEmpty(lastid))
            {
                if (id.Equals(lastid))
                {
                    NGUITools.SetActive(cont, false);
                    lastid = "";
                }
                else
                {
                    NGUITools.SetActive(cont, true);
                    lastid = id;
                }
            }
            else
            {
                NGUITools.SetActive(cont, true);
                lastid = id;
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override List<GameObject> LoadItem(int itemCount, GameObject prefabs, Transform parent)
        {
            return base.LoadItem(itemCount,  prefabs, parent);
        }

        public override void FillItem(FW.Event.EventArg eventArg)
        {
            GetMedelList();
            LoadPicList();
        }

        public override void DisPose()
        {
            base.DisPose();
        }
    }
}
