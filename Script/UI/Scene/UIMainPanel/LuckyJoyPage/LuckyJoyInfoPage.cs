//******************************************************************
// File Name:					LuckyJoyPage
// Description:					LuckyJoyPage class 
// Author:						lanjian
// Date:						5/10/2017 3:54:25 PM
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
    class LuckyJoyInfoPage : ScrollViewItemBase
    {
        protected LuckyJoyInfoPage()
        {
            this.m_PageName = "UIRootPrefabs/LuckJoyPanel/LuckJoyInfo";
            this.m_PageIndex = 1;
            this.Init();
            FillItem(null);
        }

        public static LuckyJoyInfoPage Create()
        {
            return new LuckyJoyInfoPage();
        }

        private List<LuckyJoyReward> m_LJReList;
        private Transform m_groupsTran;
        private string m_iconPaht = Utility.ConstantValue.LuckyIconPath;

        //--------------------------------------
        //private
        //--------------------------------------
        private void LoadLuckyItem()
        {
            m_groupsTran = this.CurrentItem.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1);
            GameObject prefabs = this.m_groupsTran.GetChild(0).gameObject;
            List<GameObject> itemList = new List<GameObject>();
            itemList.Add(prefabs);
            for (int i = 0; i < m_LJReList.Count - 1; i++)
            {
                GameObject item = GameObject.Instantiate(prefabs);
                item.transform.parent = this.m_groupsTran;
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = new Vector3(0, -150 * (i + 1), 0);
                item.name = "LuckyItem" + (i + 1);
                itemList.Add(item);
            }
            FillData(itemList);
        }

        private void FillData(List<GameObject> itemList)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].transform.GetChild(1).GetComponent<UILabel>().text = m_LJReList[i].Id;
                itemList[i].transform.GetChild(5).GetComponent<UILabel>().text = m_LJReList[i].ReturnRadio+"X";
                Texture texture1 = null;
                if (m_LJReList[i].JcakPotArray[0].Icon == null)
                   texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" +"dice");
                else
                   texture1 = ResMgr.ResLoad.Load<Texture>(m_iconPaht +"/"+ m_LJReList[i].JcakPotArray[0].Icon 
                    + Utility.ConstantValue.EndIconPath);

                Texture texture2 = null;
                if (m_LJReList[i].JcakPotArray[1].Icon == null)
                    texture2 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + "dice");
                else
                    texture2 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + m_LJReList[i].JcakPotArray[1].Icon
                     + Utility.ConstantValue.EndIconPath);

                Texture texture3 = null;
                if (m_LJReList[i].JcakPotArray[2].Icon == null)
                    texture3 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + "dice");
                else
                    texture3 = ResMgr.ResLoad.Load<Texture>(m_iconPaht + "/" + m_LJReList[i].JcakPotArray[2].Icon
                     + Utility.ConstantValue.EndIconPath);
                itemList[i].transform.GetChild(2).GetComponent<UITexture>().mainTexture = texture1;
                itemList[i].transform.GetChild(3).GetComponent<UITexture>().mainTexture = texture2;
                itemList[i].transform.GetChild(4).GetComponent<UITexture>().mainTexture = texture3;
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override List<GameObject> LoadItem(int itemCount, GameObject prefabs, Transform parent)
        {
            return base.LoadItem(itemCount, prefabs, parent);
        }
        
        public override void FillItem(EventArg eventArg)
        {
            m_LJReList = LuckyJoyMgr.GetLJReward();
            LoadLuckyItem();
        }

        public override void DisPose()
        {
            base.DisPose();
        }
    }
}
