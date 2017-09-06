//******************************************************************
// File Name:					BagPartsPage
// Description:					BagPartsPage class 
// Author:						lanjian
// Date:						1/17/2017 2:24:57 PM
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
    class BagPartsPage:ScrollViewItemBase
    {
        protected BagPartsPage()
        {
            this.m_PageName = "UIRootPrefabs/BagPackagePanel_PageItem/Parts";
            this.gunItemList = "UIRootPrefabs/BagPackagePanel_PageItem/partItem";
            this.m_PageIndex = 2;
            this.Init();
            //test 
            this.FillItem(null);
        }

        public static BagPartsPage Create()
        {
            return new BagPartsPage();
        }

        private string iconPath = Utility.ConstantValue.PartIcon;
        private int m_currentTab = 0;  //配件的  枪口 0  枪身 1  瞄具 2   弹夹 3   枪管 4  扳机 5 

        private List<AccessoryBase> m_MuzzlePartList;
        private List<AccessoryBase> m_BarrelPartList;
        private List<AccessoryBase> m_SightingTeleScopePartList;
        private List<AccessoryBase> m_ClipPartList;
        private List<AccessoryBase> m_AssemblyPartList;
        private List<AccessoryBase> m_tirggerPartList;

        int mCount, bCount,sCount,cCount,aCount,tCount;
        Transform ScrollViewTF;
        //记录当前scrollow的页数
        int midd = 1;
        int bidd = 1;
        int sidd = 1;
        int cidd = 1;
        int aidd = 1;
        int tidd = 1;
        //记录是否第一次点击
        bool BFirst, SFirst, CFirst, AFirst, TFirst;
        //--------------------------------------
        //private
        //--------------------------------------
        private int CalculatePageCount(List<AccessoryBase> list, int i)
        {
            if (list.Count == 0)
                return 0;
            int PageCount = (list.Count / 15) + 1;
            if (PageCount == 1)
            {
                //禁止上下页滑动
                this.CurrentItem.transform.GetChild(1).GetChild(i).GetComponent<UIScrollView>().enabled = false;
            }
            return PageCount;
        }
        
        //展示配件详情
        private void ShowDialogBox(GameObject go)
        {
            //Debug.Log("被点击的内容 是   " + go.transform.parent.GetChild(1).GetComponent<UILabel>().text);
            string id = go.transform.parent.GetChild(4).GetComponent<UILabel>().text;
            PartsDetailsDialogUI.Instance.CurrentItem = this.CurrentItem;
            PartsDetailsDialogUI.Instance.FillDigLogBox(id);
            //Debug.Log("被点击的内容 是   " + m_DiglogBox.name);
            NGUITools.SetActive(this.CurrentItem.transform.GetChild(0).gameObject, false);
            NGUITools.SetActive(this.CurrentItem.transform.GetChild(1).gameObject, false);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("bottom").gameObject, false);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
               .gameObject, true);
            //禁止左右滑动
            PanelMgr.CurrPanel.IsAllowHorMove(false);
        }

        //填充这一页
        private void FillDataToItem(int pageIndex, GameObject itemGo, List<AccessoryBase> acList)
        {
            //这页要显示的个数
            int displayNum = (pageIndex + 1) * 15 < acList.Count ? 15 : acList.Count - pageIndex * 15;
            int ABeginIndex = pageIndex * 15;
            //Debug.Log("加载di" + (pageIndex) + "页  显示个数 " + displayNum + "开始显示的位置" + ABeginIndex);
            for (int i = 0; i < displayNum; i++)
            {
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + acList[i + ABeginIndex].PartsIcon + Utility.ConstantValue.UpEndPath);
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                //itemGo.transform.GetChild(i).GetChild(0).GetComponent<UISprite>().spriteName =
                //    acList[i+ ABeginIndex].PartsIcon + "_down";
                string name = acList[i+ ABeginIndex].Name;
                itemGo.transform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = name;

                //隐藏ID
                itemGo.transform.GetChild(i).GetChild(4).GetComponent<UILabel>().text = acList[i+ ABeginIndex].ID;
                //状态  pc装备   手机装备   寄售
                itemGo.transform.GetChild(i).GetChild(3).GetChild(0).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(3).GetChild(1).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(3).GetChild(2).gameObject.SetActive(false);
                //绑定状态
                if (acList[i + ABeginIndex].ItemState != -1)
                {
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(3).GetChild(0).gameObject, true);
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(3).GetChild(0).GetChild(0).gameObject, false);
                    itemGo.transform.GetChild(i).GetChild(3).GetChild(0)
                        .GetChild(1).GetComponent<UILabel>().text = "已绑定";
                }
                if (!acList[i + ABeginIndex].IsBind)
                    itemGo.transform.GetChild(i).GetChild(3).GetChild(1).gameObject.SetActive(true);
                Utility.Utility.GetUIEventListener(itemGo.transform.GetChild(i).GetChild(0).gameObject).
                    onClick = ShowDialogBox;
            }
            for (int i = 0; i < 15 - displayNum; i++)
            {
                NGUITools.SetActive(itemGo.transform.GetChild(14 - i).gameObject, false);
            }
        }

        private void ShowWhichPart(GameObject go)
        {
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            Debug.Log("当前点击的" + m_currentTab);
            if (m_currentTab == 1 && !BFirst)
            {
                PreLoadTwoPage(1, bCount, m_BarrelPartList);
                BFirst = true;
            }
            if (m_currentTab == 2 && !SFirst)
            {
                PreLoadTwoPage(2, sCount, m_SightingTeleScopePartList);
                SFirst = true;
            }
            if (m_currentTab == 3 && !CFirst)
            {
                PreLoadTwoPage(3, cCount, m_ClipPartList);
                CFirst = true;
            }
            if (m_currentTab == 4 && !AFirst)
            {
                PreLoadTwoPage(4, aCount, m_AssemblyPartList);
                AFirst = true;
            }
            if (m_currentTab == 5 && !TFirst)
            {
                PreLoadTwoPage(5, tCount, m_tirggerPartList);
                TFirst = true;
            }
        }

        private void ControlLoadPage(int tabindex, ref int idd, int count, List<AccessoryBase> aacList)
        {
            if (ScrollViewTF.localPosition.y > (1400 * idd - 100) && idd < count - 1)
            {
                idd++;
                Vector3 currentScorllPosition = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab).transform.localPosition;
                GameObject item = this.ReloadItem(tabindex);
                FillDataToItem(idd, item, aacList);
                //恢复在当前位置
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab), currentScorllPosition);
            }
        }

        private void updataOrLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab);
            if (m_currentTab == 0)
            {
                ControlLoadPage(m_currentTab, ref midd, mCount, m_MuzzlePartList);
            }
            if (m_currentTab == 1)
            {
                ControlLoadPage(m_currentTab, ref bidd, bCount, m_BarrelPartList);
            }
            if (m_currentTab == 2)
            {
                ControlLoadPage(m_currentTab, ref sidd, sCount, m_SightingTeleScopePartList);
            }
            if (m_currentTab == 3)
            {
                ControlLoadPage(m_currentTab, ref cidd, cCount, m_ClipPartList);
            }
            if (m_currentTab == 4)
            {
                ControlLoadPage(m_currentTab, ref aidd, aCount, m_AssemblyPartList);
            }
            if (m_currentTab == 5)
            {
                ControlLoadPage(m_currentTab, ref tidd, tCount, m_tirggerPartList);
            }
        }

        //获取所有的配件
        private void GetAccessory()
        {
            m_MuzzlePartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Muzzle);
            m_BarrelPartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Barrel);
            m_SightingTeleScopePartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Sight);
            m_ClipPartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Maganize);
            m_AssemblyPartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.MuzzleSuit);
            m_tirggerPartList = Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Trigger);
        }

        //计算页数
        private void CaulateAccessoryPageCount()
        {
            mCount = CalculatePageCount(m_MuzzlePartList, 0);
            bCount = CalculatePageCount(m_BarrelPartList, 1);
            sCount = CalculatePageCount(m_SightingTeleScopePartList, 2);
            cCount = CalculatePageCount(m_ClipPartList, 3);
            aCount = CalculatePageCount(m_AssemblyPartList, 4);
            tCount = CalculatePageCount(m_tirggerPartList, 5);
        }

        //预先加载两页
        private void PreLoadTwoPage(int tabIndex, int PageCount, List<AccessoryBase> aList)
        {
            FillDataToItem(0, this.ReloadItem(tabIndex, true), aList);
            if (PageCount >= 2)
            {
                FillDataToItem(1, this.ReloadItem(tabIndex), aList);
            }
        }

        //重新加载页
        private void ReLoadBagWeaponPage()
        {
            this.GetAccessory();
            this.CaulateAccessoryPageCount();
            this.DestroyChilds(m_currentTab);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void RefreshUI()
        {
            //当前要刷新的是那一页
            if (m_currentTab == 0)
            {
                midd = 1;
                PreLoadTwoPage(0, mCount, m_MuzzlePartList);
            }
            if (m_currentTab == 1)
            {
                bidd = 1;
                PreLoadTwoPage(1, bCount, m_BarrelPartList);
            }
            if (m_currentTab == 2)
            {
                sidd = 1;
                PreLoadTwoPage(2, sCount, m_SightingTeleScopePartList);
            }
            if (m_currentTab == 3)
            {
                cidd = 1;
                PreLoadTwoPage(3, cCount, m_ClipPartList);
            }
            if (m_currentTab == 4)
            {
                aidd = 1;
                PreLoadTwoPage(4, aCount, m_AssemblyPartList);
            }
            if (m_currentTab == 5)
            {
                tidd = 1;
                PreLoadTwoPage(5, tCount, m_tirggerPartList);
            }
        }

        public override void ReLoadpage()
        {
            this.ReLoadBagWeaponPage();
        }

        public override List<GameObject> LoadItem(int itemCount,GameObject prefab, Transform parent)
        {
            prefab = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab)
                    .GetChild(0).GetChild(0).gameObject;
            parent = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab)
                    .GetChild(0).transform;
            return base.LoadItem(itemCount, prefab, parent);
        }

        public override void FillItem(EventArg eventArg)
        {
            //获取所有的配件
            GetAccessory();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            for (int i = 1; i <= 6; i++)
            {
                Utility.Utility.GetUIEventListener(topTransform.GetChild(i).gameObject).onClick = ShowWhichPart;
                topTransform.GetChild(i).gameObject.name += (i-1)+"";
            }
            Debug.Log("Muzzle配件" + m_MuzzlePartList.Count);
            Debug.Log("Barrel配件" + m_BarrelPartList.Count);
            Debug.Log("SightingTeleScope配件" + m_SightingTeleScopePartList.Count);
            Debug.Log("ClipPart配件" + m_ClipPartList.Count);
            Debug.Log("Assembly配件" + m_AssemblyPartList.Count);
            Debug.Log("tirgger配件" + m_tirggerPartList.Count);
            //计算页数
            CaulateAccessoryPageCount();
            //预先加载第一个板块的两页
            PreLoadTwoPage(0, mCount, m_MuzzlePartList);
        }
         
        public override void Update()
        {
            updataOrLoadPage();
        }
    }
}
