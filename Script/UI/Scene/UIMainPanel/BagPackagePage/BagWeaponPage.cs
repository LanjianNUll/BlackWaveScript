//******************************************************************
// File Name:					FWWeaponPage
// Description:					FWWeaponPage class 
// Author:						lanjian
// Date:						1/17/2017 11:06:56 AM
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
    class BagWeaponPage : ScrollViewItemBase
    {
        protected BagWeaponPage()
        {
            this.m_PageName = "UIRootPrefabs/BagPackagePanel_PageItem/Weapone";
            this.gunItemList = "UIRootPrefabs/BagPackagePanel_PageItem/gunItem";
            this.m_PageIndex = 1;
            this.Init();
            //注册刷新哪个武器装备到Role
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Weapon_PCEquipedChanged,RefreshList);

            //test  这里要用事件
            FillItem(null);
        }

        public static BagWeaponPage Create()
        {
            return new BagWeaponPage();
        }
        private string iconPath = Utility.ConstantValue.WeaponIcon;
        private int m_currentTab = 0;  //加载那个Tab的内容  主武器0   副武器1  近战武器2 

        private List<WeaponBase> m_MainWeaponList;
        private List<WeaponBase> m_SecondlyWeaponList;
        private List<WeaponBase> m_MeleeWeaponList;

        private GameObject center;
        //上次装备到role的哪个装备的ID
        private string lastMainEquPcId = "";
        private string lastSecondEquPcId = "";
        private string lastCloseEquPcId = "";

        int mainWeaponPageCount;
        int secondlyWeaponPageCount;
        int MeleeWeaponPageCount;
        Transform ScrollViewTF;
        //记录当前scrollow的页数
        int mainidd = 1;
        int secondidd = 1;
        int closeidd = 1;
        //--------------------------------------
        //private
        //--------------------------------------

        //装备暂时不做 
        private void RefreshList(Event.EventArg args)
        {
            //GetWeapons();//重新获取下背包的数据
            //WeaponBase pcEqulWB = (WeaponBase)args[0];
            //bool isPcEqul = (bool)args[1];
            //if (isPcEqul)
            //{
            //    string lastEquPcId = "";
            //    if (m_currentTab == 0)
            //        lastEquPcId = lastMainEquPcId;
            //    if (m_currentTab == 1)
            //        lastEquPcId = lastSecondEquPcId;
            //    if (m_currentTab == 2)
            //        lastEquPcId = lastCloseEquPcId;  
            //    Transform UIGrid = center.transform.GetChild(m_currentTab).GetChild(0);
            //    for (int i = 0; i < UIGrid.childCount; i++)
            //    {
            //        for (int j = 0; j < UIGrid.GetChild(i).childCount; j++)
            //        {
            //            if (UIGrid.GetChild(i).GetChild(j).gameObject.activeSelf == true)
            //            {
            //                if (UIGrid.GetChild(i).GetChild(j).GetChild(3)
            //                    .GetComponent<UILabel>().text.Equals(lastEquPcId))
            //                {
            //                    NGUITools.SetActive(UIGrid.GetChild(i).GetChild(j)
            //                        .GetChild(2).GetChild(0).gameObject, false);
            //                }

            //                if (UIGrid.GetChild(i).GetChild(j).GetChild(3)
            //                    .GetComponent<UILabel>().text.Equals(pcEqulWB.ID))
            //                {
            //                    NGUITools.SetActive(UIGrid.GetChild(i).GetChild(j)
            //                        .GetChild(2).GetChild(0).gameObject, true);
            //                }
            //            }
            //        }
            //    }
            //    if (m_currentTab == 0)
            //        lastMainEquPcId = pcEqulWB.ID;   //记录装备到role的装备ID
            //    if (m_currentTab == 1)
            //        lastSecondEquPcId = pcEqulWB.ID;   //记录装备到role的装备ID
            //    if (m_currentTab == 2)
            //        lastCloseEquPcId = pcEqulWB.ID;   //记录装备到role的装备ID
            //}
        }

        private void FillDigLogBox(string gunID)
        {   
            //调用对话框
            WeaponDetailsDialogUI.Instance.CurrentItem = this.CurrentItem;
            WeaponDetailsDialogUI.Instance.FillDigLogBox(gunID);
        }

        //展示武器详情
        private void ShowDialogBox(GameObject go)
        {
            //注册一个是否装配成功           
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Weapon_AccessoryChanged,
                WeaponDetailsDialogUI.Instance.OnReciveUpdateUI);
            //Debug.Log("被点击的内容 是   " + go.transform.parent.GetChild(1).GetComponent<UILabel>().text);

            string ID = go.transform.parent.GetChild(3).GetComponent<UILabel>().text;
            FillDigLogBox(ID);
            //Debug.Log("被点击的内容 是   " + m_DiglogBox.name);
            //NGUITools.SetActive(m_DiglogBox, true);
            NGUITools.SetActive(this.CurrentItem.transform.GetChild(0).gameObject, false);
            NGUITools.SetActive(this.CurrentItem.transform.GetChild(1).gameObject, false);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("bottom").gameObject, false);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
               .gameObject, true);
            //禁止左右滑动
            PanelMgr.CurrPanel.IsAllowHorMove(false);
        }

        private int CalculatePageCount(List<WeaponBase> list,int i)
        {
            if (list.Count == 0)
                return 0;
            int PageCount = (list.Count / 10) + 1;
            if (PageCount == 1)
            {
                //禁止上下页滑动
                this.CurrentItem.transform.GetChild(1).GetChild(i).GetComponent<UIScrollView>().enabled = false;
            }
            return PageCount;
        }

        bool sFirst = true;
        bool cFirst = true;

        //点击top的tab加载这个页面
        private void ShowWhichWeapon(GameObject go)
        {
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            if (m_currentTab == 1 && sFirst)
            {
                PreLoadTwoPage(1, secondlyWeaponPageCount, m_SecondlyWeaponList);
                sFirst = false;
            }
            if (m_currentTab == 2 && cFirst)
            {
                PreLoadTwoPage(2, MeleeWeaponPageCount, m_MeleeWeaponList);
                cFirst = false;
            }
        }       

        //填充这一页
        private void FillDataToItem(int pageIndex, GameObject itemGo, List<WeaponBase> weList)
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
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath +"/"+weList[i + WBeginIndex].BagIcon + Utility.ConstantValue.UpEndPath);
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                //itemGo.transform.GetChild(i).GetChild(0).GetComponent<UISprite>()
                //    .spriteName = weList[i + WBeginIndex].BagIcon + "_down";
                //Debug.Log(weList[i].BagIcon + "图标");
                
                string name = weList[i + WBeginIndex].Name;
                itemGo.transform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = name;
                //隐藏的id
                itemGo.transform.GetChild(i).GetChild(3).GetComponent<UILabel>().text = weList[i + WBeginIndex].ID;
                //状态  pc装备   手机装备   寄售
                if (weList[i + WBeginIndex].IsPCEquiped)
                {
                    itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject.SetActive(true);
                    if (m_currentTab == 0)
                        lastMainEquPcId = weList[i + WBeginIndex].ID;   //记录装备到role的装备ID
                    if (m_currentTab == 1)
                        lastSecondEquPcId = weList[i + WBeginIndex].ID;   //记录装备到role的装备ID
                    if (m_currentTab == 2)
                        lastCloseEquPcId = weList[i + WBeginIndex].ID;   //记录装备到role的装备ID
                }
                else if (weList[i + WBeginIndex].ItemState != -1)
                {
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject,true);
                    NGUITools.SetActive(itemGo.transform.GetChild(i).GetChild(2).GetChild(0).GetChild(0).gameObject,false);
                }
                else
                    itemGo.transform.GetChild(i).GetChild(2).GetChild(0).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(false);
                itemGo.transform.GetChild(i).GetChild(2).GetChild(2).gameObject.SetActive(false);
                if(!weList[i + WBeginIndex].IsBind)
                    itemGo.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(true );
            }
            for (int i = 0; i < 10 - displayNum; i++)
            {
                NGUITools.SetActive(itemGo.transform.GetChild(9 - i).gameObject, false);
            }
        }

        private void ControlLoadPage(int tabindex, ref int idd, int count, List<WeaponBase> wecList)
        {
            if (ScrollViewTF.localPosition.y > (1400 * idd - 100) && idd < count - 1)
            {
                idd++;
                Vector3 currentScorllPosition = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab).transform.localPosition;
                GameObject item = this.ReloadItem(tabindex);
                FillDataToItem(idd, item, wecList);
                //恢复在当前位置
                Utility.Utility.MoveScrollViewTOTarget(this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab), currentScorllPosition);
            }
        }

        private void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab);
            if (m_currentTab == 0)
            {
                ControlLoadPage(m_currentTab, ref mainidd, mainWeaponPageCount, m_MainWeaponList);
            }

            if (m_currentTab == 1)
            {
                ControlLoadPage(m_currentTab, ref secondidd, secondlyWeaponPageCount, m_SecondlyWeaponList);
            }

            if (m_currentTab == 2)
            {
                ControlLoadPage(m_currentTab, ref closeidd, MeleeWeaponPageCount, m_MeleeWeaponList);
            }
        }

        private void GetWeapons()
        {
            //获取主武器数据
            m_MainWeaponList =  Role.Role.Instance().KitBag.GetWeapons(WeaponType.Main);
            m_SecondlyWeaponList = Role.Role.Instance().KitBag.GetWeapons(WeaponType.Second);
            m_MeleeWeaponList = Role.Role.Instance().KitBag.GetWeapons(WeaponType.Melee);
            if (m_MainWeaponList.Count == 0)
                Utility.Utility.NotifyStr("当前武器无数据");
        }

        //计算页数
        private void CaulateWeaponPageCount()
        {
            mainWeaponPageCount = CalculatePageCount(m_MainWeaponList, 0);
            secondlyWeaponPageCount = CalculatePageCount(m_SecondlyWeaponList, 1);
            MeleeWeaponPageCount = CalculatePageCount(m_MeleeWeaponList, 2);
        }

        //预先加载两页
        private void PreLoadTwoPage(int tabIndex,int PageCount,List<WeaponBase> wList)
        {
            FillDataToItem(0, this.ReloadItem(tabIndex, true), wList);
            if (PageCount >= 2)
            {
                FillDataToItem(1, this.ReloadItem(tabIndex), wList);
            }
        }

        //重新加载页
        private void ReLoadBagWeaponPage()
        {
            this.DestroyChilds(m_currentTab);
            this.GetWeapons();
            this.CaulateWeaponPageCount();
        }
        
        //--------------------------------------
        //public
        //--------------------------------------
        public override void RefreshUI()
        {
            //当前要刷新的是那一页
            if (m_currentTab == 0)
            {
                mainidd = 1;
                PreLoadTwoPage(0, mainWeaponPageCount, m_MainWeaponList);
            }
            if (m_currentTab == 1)
            {
                secondidd = 1;
                PreLoadTwoPage(1, secondlyWeaponPageCount, m_SecondlyWeaponList);
            }
            if (m_currentTab == 2)
            {
                closeidd = 1;
                PreLoadTwoPage(2, MeleeWeaponPageCount, m_MeleeWeaponList);
            }
        }

        public override void ReLoadpage()
        {
            ReLoadBagWeaponPage();
            base.ReLoadpage();
        }

        public override void FillItem(EventArg eventArg)
        {
            GetWeapons();
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            center = this.CurrentItem.transform.GetChild(1).gameObject;
            for (int i = 1; i <= 3; i++)
            {
                Utility.Utility.GetUIEventListener(topTransform.GetChild(i).gameObject).onClick = ShowWhichWeapon;
                topTransform.GetChild(i).gameObject.name += (i-1)+"";
            }
            Debug.Log("主武器" + m_MainWeaponList.Count);
            CaulateWeaponPageCount();
            //预先加载两页
            PreLoadTwoPage(0, mainWeaponPageCount, m_MainWeaponList);
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }

        public override void DisPose()
        {
            //注册刷新哪个武器装备到Role
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Weapon_PCEquipedChanged, RefreshList);
            base.DisPose();
        }
    }
}
