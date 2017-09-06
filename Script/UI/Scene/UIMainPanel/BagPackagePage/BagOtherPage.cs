//******************************************************************
// File Name:					BagOtherPage
// Description:					BagOtherPage class 
// Author:						lanjian
// Date:						1/17/2017 2:31:08 PM
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
    class BagOtherPage:BagItemBasePage
    {
        protected BagOtherPage()
        {
            this.m_PageName = "UIRootPrefabs/BagPackagePanel_PageItem/Other";
            this.gunItemList = "UIRootPrefabs/BagPackagePanel_PageItem/handBombItem";
            this.iconPath = Utility.ConstantValue.CommodityIcon;
            this.m_tabTotalCount = 3;
            this.m_PageIndex = 4;
            this.Init();
            this.FillItem(null);
        }

        public static BagOtherPage Create()
        {
            return new BagOtherPage();
        }

        //--------------------------------------
        //private
        //--------------------------------------
        

        //--------------------------------------
        //public
        //--------------------------------------
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
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + weList[i + WBeginIndex].Icon + Utility.ConstantValue.UpEndPath);
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                //itemGo.transform.GetChild(i).GetChild(0).GetComponent<UISprite>()
                //    .spriteName = weList[i + WBeginIndex].BagIcon + "_down";
                //CommodityBase dd = (CommodityBase)weList[i + WBeginIndex];
                //Debug.Log(dd.BagIcon + "图标");
                string name = weList[i + WBeginIndex].Name;
                itemGo.transform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = name;
                //隐藏的id
                itemGo.transform.GetChild(i).GetChild(3).GetComponent<UILabel>().text = weList[i + WBeginIndex].ID;
                itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(2).gameObject.SetActive(false);
                //显示数量的label
                NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(5).gameObject, true);
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
            }
            for (int i = 0; i < 10 - displayNum; i++)
            {
                NGUITools.SetActive(itemGo.transform.GetChild(9 - i).gameObject, false);
            }
        }

        public override void GetWeapons()
        {
            m_firstItemList.Clear();
            m_secondItemList.Clear();
            m_thirdItemList.Clear();
            List<CommodityBase> list1 = Role.Role.Instance().KitBag.GetTradeCommditys(CommodityTradeType.ExpAgent);
            foreach (var item in list1)
            {
                m_firstItemList.Add(item);
            }
            List<CommodityBase> list2 = Role.Role.Instance().KitBag.GetTradeCommditys(CommodityTradeType.ProfitItem);
            foreach (var item in list2)
            {
                m_secondItemList.Add(item);
            }
            List<CommodityBase> list3 = Role.Role.Instance().KitBag.GetTradeCommditys(CommodityTradeType.Revival);
            foreach (var item in list3)
            {
                m_thirdItemList.Add(item);
            }
            //Debug.Log("经验卡" + m_firstItemList.Count);
            //Debug.Log("增益道具" + m_secondItemList.Count);
            //Debug.Log("复活币" + m_thirdItemList.Count);
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

        public override void DisPose()
        {
            if (this.m_CurrentItem != null)
                UnityEngine.Object.Destroy(this.m_CurrentItem);
        }
    }
}
