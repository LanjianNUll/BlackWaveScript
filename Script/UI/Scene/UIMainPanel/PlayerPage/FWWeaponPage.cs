//******************************************************************
// File Name:					FWWeaponPage
// Description:					FWWeaponPage class 
// Author:						lanjian
// Date:						1/13/2017 2:58:53 PM
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
    class FWWeaponPage:ScrollViewItemBase
    {
        protected FWWeaponPage()
        {
            this.m_PageName = "UIRootPrefabs/PlayerPanel_PageItem/fwWeaponPage";
            this.m_PageIndex = 4;
            this.Init();
            this.FillItem(null);
        }

        public static FWWeaponPage Create()
        {
            return new FWWeaponPage();
        }

        private List<GameObject> m_itemList = new List<GameObject>();
        private List<Proficiency> m_proficiency;
        private string m_prefabPath = "UIRootPrefabs/PlayerPanel_PageItem/Itemprefabs/WProficencyitem";
        //--------------------------------------
        //private
        //--------------------------------------

        private void GetProficencyList()
        {
            m_proficiency = Role.Role.Instance().ProficencyProctor.GetWProficency();
        }

        private void LoadProficency()
        {
            m_itemList = this.LoadItemPath(m_proficiency.Count, m_prefabPath,
                this.CurrentItem.transform.GetChild(1).GetChild(0));
            this.CurrentItem.transform.GetChild(1).GetComponent<UIScrollView>().ResetPosition();
            if (m_proficiency.Count <= 3)
                this.CurrentItem.transform.GetChild(1).GetComponent<UIScrollView>().enabled = false;
            FillDataToUI();
        }

        private void FillDataToUI()
        {
            float distance = 318 - (-136);
            for (int i = 0; i < m_itemList.Count; i++)
            {
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.RoleIcon + "/" + this.m_proficiency[i].Icon);
                Transform item = m_itemList[i].transform.GetChild(0);
                item.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                item.GetChild(1).GetComponent<UILabel>().text = this.m_proficiency[i].Name;
                item.GetChild(4).GetComponent<UISprite>().fillAmount = this.m_proficiency[i].Radio;
                if (this.m_proficiency[i].Radio == 0)
                    NGUITools.SetActive(item.GetChild(5).gameObject,false);
                else
                    item.GetChild(5).localPosition = new Vector3(-136 + distance * this.m_proficiency[i].Radio, -18, 0);
                Transform proAndValue = item.Find("ProandValue");
                for (int j = 0; j < proAndValue.childCount; j++)
                {
                    NGUITools.SetActive(proAndValue.GetChild(j).gameObject,false);
                }
                int indexPro = 0;
                int indexVal = 0;
                for (int j = 0; j < this.m_proficiency[i].Propery.Count*2; j++)
                {
                    NGUITools.SetActive(proAndValue.GetChild(j).gameObject, true);
                    if (j % 2 == 0)
                    {
                        proAndValue.GetChild(j).GetComponent<UILabel>().text = this.m_proficiency[i].Propery[indexPro];
                        indexPro++;
                    }
                    else
                    {
                        proAndValue.GetChild(j).GetComponent<UILabel>().text = this.m_proficiency[i].Value[indexVal];
                        indexVal++;
                    }
                }
            }
        }
        //--------------------------------------
        //public
        //--------------------------------------

        public override void FillItem(FW.Event.EventArg eventArg)
        {
            GetProficencyList();
            LoadProficency();
        }

        public override void DisPose()
        {
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.PLAYER_WEAPON_INFO, FillItem);
            base.DisPose();
        }
    }
}
