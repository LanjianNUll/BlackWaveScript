//******************************************************************
// File Name:					PanelBase
// Description:					PanelBase class 
// Author:						lanjian
// Date:						1/5/2017 3:20:23 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    abstract class PanelBase
    {
        protected string m_panelName;
        protected GameObject m_panelObj;
        protected bool m_IsAllowHorzMove = true;
        private List<GameObject> pageNumList = new List<GameObject>();
        protected PanelType panelType;

        protected PanelBase()
        {
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject RootObj { get { return m_panelObj; } }
        public PanelType PanelTypeTY { get { return panelType; } }

        protected int pageTotalCount;                                   //总页数
        protected GameObject scrollView;                                //当前滚动的scorllView
        private int m_CurrentPageNum = 1;                               //当前的页码
        private bool m_GoNextOrPrePage = false;                         //是否翻页
        private int m_offset;                                           //偏移量
        private int m_MoveCount = 0;                                    //偏移计数
        private float m_distance_X = 0;                                 //x移动的距离
        private float m_distance_Y = 0;                                 //y移动的距离
        private Vector3 m_StartPosition;                                //开始触摸的点
        private Vector3 m_OverPosition;                                 //结束触摸的点
        private bool isCanDrag = true;                                  //是否触摸
        private bool m_Pressed = true;                                  //一直按住屏幕
        private bool m_LastAnimationFinish = true;                      //上一个动画是否结束

        protected int m_Sensitivity = 150;                              //触摸灵敏度调节（值越小 灵敏度越高）
        protected int m_SlideSpeed = 30;                                //当前ScrollView滑动的速度
        protected float m_XYSesitivity = (Mathf.PI / 180) * 30;         //当滑动角度小于多少度是 判断是左右滑动 这里取的是30度

        //--------------------------------------
        //public 
        //--------------------------------------

        public int CurrentPageNum { get { return this.m_CurrentPageNum; } }

        public virtual void Init()
        {
            this.m_panelObj = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(this.m_panelName) as GameObject);
            if (this.m_panelObj != null)
            {
                this.m_panelObj.transform.parent = GameObject.Find("MainUIRoot(Clone)").transform;
                this.m_panelObj.transform.localScale = Vector3.one;   //这里和UIRoot的大小有点冲突，先这样解决
            }
        }

        public virtual void DisPose()
        {
            if (this.m_panelObj != null)
                UnityEngine.Object.Destroy(this.m_panelObj);
        }

        public abstract void BindScript(UIEventBase eventBase);
       
        public virtual void UpdateInput() { }

        public virtual void IsAllowHorMove(bool isAllow) { m_IsAllowHorzMove = isAllow; }
        
        //--------------------------------------
        //private
        //--------------------------------------
        //对控制是左右 移动标志
        private void ScrollViewMoveHor(float offset)
        {
            m_LastAnimationFinish = false;
            m_Pressed = false;
            if (offset < 0)
                m_offset = -m_SlideSpeed;
            else
                m_offset = m_SlideSpeed;
            m_GoNextOrPrePage = true;
        }

        //ScroView的偏移
        private void ScrollViewMove(float scrollView_Offset)
        {
            scrollView.GetComponent<SpringPanel>().target += new Vector3(scrollView_Offset, 0, 0);
            scrollView.GetComponent<UIPanel>().clipOffset += new Vector2(-scrollView_Offset, 0);
            scrollView.transform.localPosition += new Vector3(scrollView_Offset, 0, 0);
        }

        /// <summary>
        /// 加载页码   默认第一页
        /// </summary>
        public void LoadPageNum(int pageTotalCount,int defaultPageNum = 1)
        {

            //每一个页项的都要有bottom/pageMgr这个结构
            Transform pageMgr = PanelMgr.CurrPanel.RootObj.transform.Find("bottom").Find("pageMgr");
            //每次都要先清空下
            if (pageMgr.childCount!=0)
            {
                for (int i = 0; i < pageMgr.childCount; i++)
                {
                    UnityEngine.Object.Destroy(pageMgr.GetChild(i).gameObject);
                }
            }
            if (pageTotalCount == 0)
            {
                NGUITools.SetActive(pageMgr.gameObject, false);
                return;
            }
            NGUITools.SetActive(pageMgr.gameObject, true);
            string pageNumPrefabPath = "UIRootPrefabs/PlayerPanel_PageItem/PageNumPrefab";
            pageNumList.Clear();
            //创建
            for (int i = 0; i < pageTotalCount; i++)
            {
                GameObject pageNum = UnityEngine.Object
                    .Instantiate(ResMgr.ResLoad.Load(pageNumPrefabPath) as GameObject);
                pageNum.transform.parent = pageMgr;
                //初始化的位置和大小
                pageNum.transform.localPosition = new Vector3(0, 80, 0);
                pageNum.transform.localScale = Vector3.one;
                pageNumList.Add(pageNum);
            }
            //均匀的排好 (上限是9页 要扩展还要优化)
            float beginPostion = 0;
            float gap = -100;
            //偶数
            if (pageNumList.Count % 2 == 0)
            {
                beginPostion = (pageNumList.Count / 2) * (gap) + 50;
            }
            else
            {
                beginPostion = (pageNumList.Count / 2) * (gap);
            }
            for (int i = 0; i < pageNumList.Count; i++)
            {
                pageNumList[i].transform.localPosition = new Vector3(beginPostion, 80, 0);
                beginPostion -= gap;
            }
            SetPageNum(defaultPageNum);
        }

        /// <summary>
        /// 设置页数
        /// </summary>
        /// <param name="index"></param>
        public void SetPageNum(int index)
        {
            m_CurrentPageNum = index;
            if (index - 1 >= 0)
            {
                foreach (var item in pageNumList)
                {
                    item.GetComponent<UISprite>().spriteName = "player_hint_up";
                }
                pageNumList[index - 1].GetComponent<UISprite>().spriteName = "player_hint_down";
            }
        }

       
        public void BaseTouchControl()
        {
            if (Input.GetMouseButtonDown(0) && isCanDrag)
            {
                m_StartPosition = Input.mousePosition;
                isCanDrag = false;
                //Debug.Log("Mouse down   M_StartPosition" + m_StartPosition.ToString());
            }
            if (Input.GetMouseButton(0) && m_Pressed)
            {
                m_OverPosition = Input.mousePosition;
                m_distance_X = m_OverPosition.x - m_StartPosition.x;
                m_distance_Y = m_OverPosition.y - m_StartPosition.y;
                isCanDrag = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_Pressed = true;
            }

            if (Mathf.Abs(m_distance_Y) < Mathf.Sin(m_XYSesitivity) *Mathf.Abs(m_distance_X))//当上下滑动幅度在什么范围内才能左右滑动
            {
                if (m_distance_X < -m_Sensitivity && m_LastAnimationFinish)
                {
                    if (m_CurrentPageNum != pageTotalCount)
                    {
                        ScrollViewMoveHor(-934);
                        m_CurrentPageNum++;
                        //设置页码
                        SetPageNum(m_CurrentPageNum);
                        Debug.Log(".当前页是：： " + m_CurrentPageNum);
                    }
                }

                if (m_distance_X > m_Sensitivity && m_LastAnimationFinish)
                {
                    if (m_CurrentPageNum != 1)
                    {
                        ScrollViewMoveHor(934);
                        m_CurrentPageNum--;
                        //设置页码
                        SetPageNum(m_CurrentPageNum);
                        Debug.Log("当前页是：： " + m_CurrentPageNum);
                    }
                }
                m_distance_X = 0;
                m_distance_Y = 0;
            }
        }

        public void BaseAnimtionControl()
        {
            if (m_GoNextOrPrePage)
            {
                ScrollViewMove(m_offset);
                m_MoveCount += m_SlideSpeed;
                if (m_MoveCount >= 934)
                {
                    m_GoNextOrPrePage = false;
                    m_MoveCount = 0;
                    //修正
                    Vector3 target = new Vector3((this.m_CurrentPageNum-1) * -934,0,0);
                    Utility.Utility.MoveScrollViewTOTarget(this.scrollView.gameObject, target,this.m_SlideSpeed/2);
                    m_LastAnimationFinish = true;
                }
            }
        }
    }
}
