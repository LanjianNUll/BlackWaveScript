//******************************************************************
// File Name:					FWRolePage
// Description:					FWRolePage class 
// Author:						lanjian
// Date:						1/13/2017 2:57:05 PM
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
    class FWRolePage:ScrollViewItemBase
    {
        protected FWRolePage()
        {
            this.m_PageName = "UIRootPrefabs/PlayerPanel_PageItem/fwRolePage";
            this.m_PageIndex = 2;
            this.Init();
            //test
            FillItem(new Event.EventArg());
        }

        public static FWRolePage Create()
        {
            return new FWRolePage();
        }

        private List<GameObject> m_ItemList;
        private List<PersonImage> m_personImageList;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetPersonImageList()
        {
            m_personImageList = Role.Role.Instance().PersonImageProctor.GetPeronsImages();
        }
        
        private void LoadPicList()
        {
            string path = "UIRootPrefabs/PlayerPanel_PageItem/Itemprefabs/personPictureList";
            m_ItemList = this.LoadItemPath((m_personImageList.Count /9) +1, path,
                this.CurrentItem.transform.GetChild(1).GetChild(0));
            //顶格
            Utility.Utility.ModifyItemT0p(this.CurrentItem.transform.GetChild(1).gameObject, new Vector3(0, -70, 0));
            //不满一页是  禁止滑动
            if (m_personImageList.Count <= 9)
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
            int displayNum = (pageNum + 1) * 9 < m_personImageList.Count ? 9 : m_personImageList.Count - pageNum * 9;
            int WBeginIndex = pageNum * 9;

            //隐藏
            for (int i = 0; i < 9 ; i++)
            {
                NGUITools.SetActive(picList[i].gameObject, false);
            }

            for (int i = 0; i < displayNum; i++)
            {
                Transform content = picList[i].GetChild(2).GetChild(0);
                content.GetChild(0).GetComponent<UILabel>().text = m_personImageList[i].Name;
                content.GetChild(1).GetComponent<UILabel>().text = m_personImageList[i].GetWay;
                content.GetChild(3).GetComponent<UILabel>().text = m_personImageList[i].Desc;
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.RoleIcon + "/" + m_personImageList[i].Icon);
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
        public override void FillItem(FW.Event.EventArg eventArg)
        {
            GetPersonImageList();
            LoadPicList();
        }

        public override void DisPose()
        {
            base.DisPose();
        }
    }
}
