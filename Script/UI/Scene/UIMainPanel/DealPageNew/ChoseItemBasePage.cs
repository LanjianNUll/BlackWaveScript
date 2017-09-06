//******************************************************************
// File Name:					ChoseItemBasePage
// Description:					ChoseItemBasePage class 
// Author:						lanjian
// Date:						3/13/2017 5:17:44 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Deal;

namespace FW.UI
{
    class ChoseItemBasePage : ScrollViewItemBase
    {
        public override void FillItem(EventArg eventArg) {}

        protected string iconPath = "";
        protected List<DealFitterItem> m_mFirstList;
        protected List<DealFitterItem> m_mSecondList;
        protected List<DealFitterItem> m_mThridList;
        protected List<DealFitterItem> m_mforthtList;
        protected List<DealFitterItem> m_mfifthList;
        protected List<DealFitterItem> m_msixthList;

        protected int m_firstPageCount;
        protected int m_secondPageCount;
        protected int m_thridPageCount;
        protected int m_forthPageCount;
        protected int m_fifthPageCount;
        protected int m_sixthPageCount;

        //记录当前scrollow的页数
        protected int mainidd = 1;
        protected int secondidd = 1;
        protected int closeidd = 1;
        protected int forthidd = 1;
        protected int fifthidd = 1;
        protected int sixthidd = 1;

        protected int m_currentTab = 0;  //加载那个Tab的内容

        protected Transform ScrollViewTF;
        protected Transform m_centerTra;

        //当前点击的分类
        protected DealFitterItem m_currentClickFilter;

        //填充这一页
        protected void FillDataToItem(int pageIndex, GameObject itemGo, List<DealFitterItem> weList)
        {
            //这页要显示的个数
            int displayNum = (pageIndex + 1) * 10 < weList.Count ? 10 : weList.Count - pageIndex * 10;
            int WBeginIndex = pageIndex * 10;
            //Debug.Log("加载di" + (pageIndex) + "页  显示个数 " + displayNum + "开始显示的位置" + WBeginIndex);
            for (int i = 0; i < displayNum; i++)
            {
                Utility.Utility.GetUIEventListener(itemGo.transform.GetChild(i).GetChild(0).gameObject)
                       .onClick = OnShowReadySold;

                //处理图标问题
                Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + weList[i + WBeginIndex].Icon + Utility.ConstantValue.UpEndPath);
                //Debug.Log("手雷手雷"+iconPath + "/" + weList[i + WBeginIndex].Icon + Utility.ConstantValue.UpEndPath);
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().mainTexture = texture;

                itemGo.transform.GetChild(i).GetChild(0).GetComponent<UITexture>().SetRect(0,0,texture.width,texture.height);
                itemGo.transform.GetChild(i).GetChild(0).GetComponent<Transform>().localPosition = Vector3.zero;
                itemGo.transform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = weList[i + WBeginIndex].Name;
            }

            for (int i = 0; i < 10 - displayNum; i++)
            {
                NGUITools.SetActive(itemGo.transform.GetChild(9 - i).gameObject, false);
            }
        }

        //显示这个类别的武器列表
        public void OnShowReadySold(GameObject go)
        {
            FindCurrentCurrentItem(go);
            Debug.Log("当前点击的分类名是"+m_currentClickFilter.Name);
            DialogMgr.Load(DialogType.PutUpSaleAndReadySale);
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(m_currentClickFilter));
            //将当前的点击为空
            m_currentClickFilter = null;
        }

        private void FindCurrentCurrentItem(GameObject go)
        {
            string name = go.transform.parent.GetChild(1).GetComponent<UILabel>().text;
            if(m_mFirstList!=null&& m_currentClickFilter==null)
            {
                m_currentClickFilter = FindItemByName(name, m_mFirstList);
            }
            if (m_mSecondList != null && m_currentClickFilter == null)
            {
                m_currentClickFilter = FindItemByName(name, m_mSecondList);
            }
            if (m_mThridList != null && m_currentClickFilter == null)
            {
                m_currentClickFilter = FindItemByName(name, m_mThridList);
            }
            if (m_mforthtList != null && m_currentClickFilter == null)
            {
                m_currentClickFilter = FindItemByName(name, m_mforthtList);
            }
            if (m_mfifthList != null && m_currentClickFilter == null)
            {
                m_currentClickFilter = FindItemByName(name, m_mfifthList);
            }
            if (m_msixthList != null && m_currentClickFilter == null)
            {
                m_currentClickFilter = FindItemByName(name, m_msixthList);
            }
        }

        private DealFitterItem FindItemByName(string name,List<DealFitterItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name.Equals(name))
                    return list[i];
            }
            return null;
        } 

        protected int CalculatePageCount(List<DealFitterItem> list, int i)
        {
            if (list == null) return 0;
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

        //计算页数
        protected void CaulateWeaponPageCount()
        {
            m_firstPageCount = CalculatePageCount(m_mFirstList, 0);
            m_secondPageCount = CalculatePageCount(m_mSecondList, 1);
            m_thridPageCount = CalculatePageCount(m_mThridList, 2);
            m_forthPageCount = CalculatePageCount(m_mforthtList, 3);
            m_fifthPageCount = CalculatePageCount(m_mfifthList, 4);
            m_sixthPageCount = CalculatePageCount(m_msixthList, 5);
        }

        //预先加载两页
        protected void PreLoadTwoPage(int tabIndex, int PageCount, List<DealFitterItem> wList)
        {
            FillDataToItem(0, this.ReloadItem(tabIndex, true), wList);
            if (PageCount >= 2)
            {
                FillDataToItem(1, this.ReloadItem(tabIndex), wList);
            }
        }

        protected void ControlLoadPage(int tabindex, ref int idd, int count, List<DealFitterItem> wecList)
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

        protected void UpdateScrollChangeLoadPage()
        {
            ScrollViewTF = this.CurrentItem.transform.GetChild(1).GetChild(m_currentTab);
            if (m_currentTab == 0)
            {
                ControlLoadPage(m_currentTab, ref mainidd, m_firstPageCount, m_mFirstList);
            }

            if (m_currentTab == 1)
            {
                ControlLoadPage(m_currentTab, ref secondidd, m_secondPageCount, m_mSecondList);
            }

            if (m_currentTab == 2)
            {
                ControlLoadPage(m_currentTab, ref closeidd, m_thridPageCount, m_mThridList);
            }

            if (m_currentTab == 3)
            {
                ControlLoadPage(m_currentTab, ref forthidd, m_forthPageCount, m_mforthtList);
            }

            if (m_currentTab == 4)
            {
                ControlLoadPage(m_currentTab, ref fifthidd, m_fifthPageCount, m_mfifthList);
            }

            if (m_currentTab == 5)
            {
                ControlLoadPage(m_currentTab, ref sixthidd, m_sixthPageCount, m_msixthList);
            }
        }

        bool sFirst = true;
        bool cFirst = true;
        bool tFirst = true;
        bool fiFirst = true;
        bool siFirst = true;
        protected void OnShowWichWWeapon(GameObject go)
        {
            m_currentTab = Convert.ToInt32(go.name.Substring(go.name.Length - 1, 1));
            if (m_currentTab == 1 && sFirst)
            {
                PreLoadTwoPage(1, m_secondPageCount, m_mSecondList);
                sFirst = false;
            }
            if (m_currentTab == 2 && cFirst)
            {
                PreLoadTwoPage(2, m_thridPageCount, m_mThridList);
                cFirst = false;
            }
            if (m_currentTab == 3 && tFirst)
            {
                PreLoadTwoPage(3, m_forthPageCount, m_mforthtList);
                tFirst = false;
            }
            if (m_currentTab == 4 && fiFirst)
            {
                PreLoadTwoPage(4, m_fifthPageCount, m_mfifthList);
                fiFirst = false;
            }
            if (m_currentTab == 5 && siFirst)
            {
                PreLoadTwoPage(5, m_sixthPageCount, m_msixthList);
                siFirst = false;
            }
        }

        public override void Update()
        {
            //根据滑动Scrollview加载页
            UpdateScrollChangeLoadPage();
        }
    }
}
