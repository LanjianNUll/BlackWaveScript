//******************************************************************
// File Name:					BagHandBombPage
// Description:					BagHandBombPage class 
// Author:						lanjian
// Date:						1/17/2017 2:29:41 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Item;

namespace FW.UI
{
    class BagHandBombPage: BagItemBasePage
    {
        protected BagHandBombPage()
        {
            this.m_PageName = "UIRootPrefabs/BagPackagePanel_PageItem/HandBomb";
            this.gunItemList = "UIRootPrefabs/BagPackagePanel_PageItem/handBombItem";
            this.m_PageIndex = 3;
            this.m_tabTotalCount = 1;
            this.iconPath = Utility.ConstantValue.HandBombIcon;
            this.Init();
            this.FillItem(null);
        }

        public static BagHandBombPage Create()
        {
            return new BagHandBombPage();
        }

        //--------------------------------------
        //private
        //--------------------------------------
       

        //--------------------------------------
        //public
        //--------------------------------------
        //填充这一页
        public override void FillDataToItem(int pageIndex, GameObject itemGo, List<ItemBase> weList)
        {
            //这页要显示的个数
            int displayNum = (pageIndex + 1) * 10 < weList.Count ? 10 : weList.Count - pageIndex * 10;
            int WBeginIndex = pageIndex * 10;
            //Debug.Log("加载di" + (pageIndex) + "页  显示个数 " + displayNum + "开始显示的位置" + WBeginIndex);
            for (int i = 0; i < displayNum; i++)
            {
                Utility.Utility.GetUIEventListener(itemGo.transform.GetChild(i).GetChild(0).gameObject)
                        .onClick = ShowDialogBox;
                //处理图标问题
                CommodityBase aa =  (CommodityBase)weList[i + WBeginIndex];
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + aa.BagIcon + Utility.ConstantValue.UpEndPath);
                //Debug.Log("手雷问题："+ aa.BagIcon);
                //Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + "grenade1_down");
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                string name = weList[i + WBeginIndex].Name;
                itemGo.transform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = name;
                //隐藏的id
                itemGo.transform.GetChild(i).GetChild(3).GetComponent<UILabel>().text = weList[i + WBeginIndex].ID;
                //状态  pc装备   手机装备   寄售
                itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(2).gameObject.SetActive(false);
                //显示数量的label
                itemGo.transform.GetChild(i).GetChild(5).GetComponent<UILabel>().text
                    = weList[i + WBeginIndex].Count.ToString();
                //绑定状态
                if (weList[i + WBeginIndex].ItemState != -1)
                {
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject, true);
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(2).GetChild(0).GetChild(0).gameObject, false);
                    itemGo.transform.GetChild(i).GetChild(2).GetChild(0)
                        .GetChild(1).GetComponent<UILabel>().text = "已绑定";
                }
                if (!weList[i + WBeginIndex].IsBind)
                {
                    itemGo.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(true);
                }
                //if (m_currentTab == 0)
                //{
                //    WeaponBase a = (WeaponBase)weList[i + WBeginIndex];
                //    if (a.IsPCEquiped)
                //        itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject.SetActive(true);
                //}

            }
            for (int i = 0; i < 10 - displayNum; i++)
            {
                NGUITools.SetActive(itemGo.transform.GetChild(9 - i).gameObject, false);
            }
        }

        public override void GetWeapons()
        {
            m_firstItemList.Clear();
            List<CommodityBase> list = Role.Role.Instance().KitBag.GetCommditys(CommodityType.GrenadeItem);
            foreach (var item in list)
            {
                m_firstItemList.Add(item);
            }

            Debug.Log("全部" + m_firstItemList.Count);
            //Debug.Log("特殊手雷(道具)"+ m_secondItemList.Count);
        }

        public override void FillItem(EventArg eventArg)
        {
            this.GetWeapons();
            center = this.CurrentItem.transform.GetChild(1).gameObject;
            this.GetTabClickLister();
            CaulateWeaponPageCount();
            //预先加载两页
            PreLoadTwoPage(0, m_fisrtPageCount, m_firstItemList);
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }
    }
}
