//******************************************************************
// File Name:					BagItemBasePage
// Description:					BagItemBasePage class 
// Author:						lanjian
// Date:						3/8/2017 10:31:11 AM
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
    class BagItemBasePage : ScrollViewItemBase
    {
        public override void FillItem(EventArg eventArg){}
        protected string iconPath = "";
        protected int m_currentTab = 0;  //加载那个Tab的内容 0常规  特殊
        protected GameObject center;
        protected int m_fisrtPageCount;
        protected int m_secondPageCount;
        protected int m_thirdPageCount;
        protected int m_tabTotalCount;
        protected Transform ScrollViewTF;
        protected List<ItemBase> m_firstItemList = new List<ItemBase>();
        protected List<ItemBase> m_secondItemList = new List<ItemBase>();
        protected List<ItemBase> m_thirdItemList = new List<ItemBase>();
        protected ItemBase m_currentClickItem;
        //记录当前scrollow的页数
        protected int m_firstidd = 1;
        protected int m_secondidd = 1;
        protected int m_thirdidd = 1;
        //--------------------------------------
        //public
        //--------------------------------------
        bool sFirst = true;
        bool sSecond = true;
        //bool sThird = true;
        public void OnShowWhichWeapon(GameObject go)
        {
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            if (m_currentTab == 1 && sFirst)
            {
                PreLoadTwoPage(1, m_secondPageCount, m_secondItemList);
                sFirst = false;
            }
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            if (m_currentTab == 2 && sSecond)
            {
                PreLoadTwoPage(2, m_thirdPageCount, m_thirdItemList);
                sSecond = false;
            }
        }

        public int CalculatePageCount(List<ItemBase> list, int i)
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

        //预先加载两页
        public void PreLoadTwoPage(int tabIndex, int pageCount, List<ItemBase> list)
        {
            FillDataToItem(0, this.ReloadItem(tabIndex, true), list);
            if (pageCount >= 2)
            {
                FillDataToItem(1, this.ReloadItem(tabIndex), list);
            }
        }

        public void ControlLoadPage(int tabindex, ref int idd, int count, List<ItemBase> wecList)
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

        //计算页数
        public void CaulateWeaponPageCount()
        {
            m_fisrtPageCount = CalculatePageCount(m_firstItemList, 0);
            m_secondPageCount = CalculatePageCount(m_secondItemList, 1);
            m_thirdPageCount = CalculatePageCount(m_secondItemList, 1);
        }

        public void GetTabClickLister()
        {
            Transform topTransform = this.CurrentItem.transform.GetChild(0);
            for (int i = 1; i <= m_tabTotalCount; i++)
            {
                Utility.Utility.GetUIEventListener(topTransform.GetChild(i).gameObject).onClick = OnShowWhichWeapon;
                topTransform.GetChild(i).gameObject.name += (i - 1) + "";
            }
        }

        public void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab);
            if (m_currentTab == 0)
            {
                ControlLoadPage(m_currentTab, ref m_firstidd, m_fisrtPageCount, m_firstItemList);
            }

            if (m_currentTab == 1)
            {
                ControlLoadPage(m_currentTab, ref m_secondidd, m_secondPageCount, m_secondItemList);
            }

            if (m_currentTab == 2)
            {
                ControlLoadPage(m_currentTab, ref m_thirdidd, m_thirdPageCount, m_thirdItemList);
            }
        }

        //返回当前点击的那个ItemBase
        public void FindCurrentClick(string id)
        {
            if (m_currentTab == 0)
                for (int i = 0; i < m_firstItemList.Count; i++)
                {
                    if (m_firstItemList[i].ID.Equals(id))
                        m_currentClickItem = m_firstItemList[i];
                }
            if (m_currentTab == 1)
                for (int i = 0; i < m_secondItemList.Count; i++)
                {
                    if (m_secondItemList[i].ID.Equals(id))
                        m_currentClickItem = m_secondItemList[i];
                }

            if (m_currentTab == 2)
                for (int i = 0; i < m_thirdItemList.Count; i++)
                {
                    if (m_thirdItemList[i].ID.Equals(id))
                        m_currentClickItem = m_thirdItemList[i];
                }
        }

        //刷新下这个页
        protected void ReLoadBagBasePage()
        {
            this.ClearScrollView();
            //获取新数据
            this.GetWeapons();
            this.CaulateWeaponPageCount();
        }

        public override void RefreshUI()
        {
            if (m_currentTab == 0)
            {
                m_firstidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_fisrtPageCount, m_firstItemList);
            }

            if (m_currentTab == 1)
            {
                m_secondidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_secondPageCount, m_secondItemList);
            }
            if (m_currentTab == 2)
            {
                m_thirdidd = 1;
                this.PreLoadTwoPage(m_currentTab, m_thirdPageCount, m_thirdItemList);
            }
        }

        public override void ReLoadpage()
        {
            this.ReLoadBagBasePage();
            base.ReLoadpage();
        }

        //清空当前的tab下的scrollView
        private void ClearScrollView()
        {
            Transform parent = center.transform.GetChild(m_currentTab).GetChild(0);
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        //展示详情窗口
        public void ShowDialogBox(GameObject go)
        {
            FindCurrentClick(go.transform.parent.Find("gunid").GetComponent<UILabel>().text);
            DialogMgr.Load(DialogType.HandBombAndCommdityDetail);
            //将点击的Itembase  和当前第几页
            DialogMgr.CurrentDialog.ShowCommonDialog(new EventArg(m_currentClickItem,this.m_PageIndex));
        }
        
        public virtual void FillDataToItem(int pageIndex, GameObject itemGo, List<ItemBase> weList) { }

        public virtual void GetWeapons() { }
    }
}
