//******************************************************************
// File Name:					PutUpSaleAndReadyControlScrollBaseDialogUI
// Description:					PutUpSaleAndReadyControlScrollBaseDialogUI class 
// Author:						lanjian
// Date:						3/16/2017 11:24:14 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 控制在售和寄售的左右翻页
    /// </summary>
    class PutUpSaleAndReadyControlScrollBaseDialogUI: PutUpSaleAndReadySaleBaseDialogUI
    {
        
        protected bool m_GoNextOrPrePage = false;   //是否翻页
        protected int m_offset;                     //偏移量
        protected int m_MoveCount = 0;              //偏移计数
        protected float distance = 0;               //移动的距离
        protected Vector3 m_StartPosition;           //开始触摸的点
        protected Vector3 m_OverPosition;            //结束触摸的点
        protected int m_currentPageNum = 1;
        protected Transform m_pageMgr;
        protected int m_sensitivity = 100;

        //ScroView的偏移
        protected void ScrollViewMove(float scrollView_Offset)
        {
            scrollView.GetComponent<SpringPanel>().target += new Vector3(scrollView_Offset, 0, 0);
            scrollView.GetComponent<UIPanel>().clipOffset += new Vector2(-scrollView_Offset, 0);
            scrollView.transform.localPosition += new Vector3(scrollView_Offset, 0, 0);
        }

        protected void ScrollViewMoveHor(float offset)
        {
            if (offset < 0)
                m_offset = -30;
            else
                m_offset = 30;
            m_GoNextOrPrePage = true;
        }

        protected void BaseAnimtionControl()
        {
            if (m_GoNextOrPrePage)
            {
                ScrollViewMove(m_offset);
                m_MoveCount += 30;
                if (m_MoveCount == 930)
                {
                    m_GoNextOrPrePage = false;
                    m_MoveCount = 0;
                    if (m_offset < 0)
                    {
                        //修正
                        ScrollViewMove(-4);
                    }
                    else
                    {
                        ScrollViewMove(4);
                    }
                }
            }
        }

        protected void BaseTouchControl()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_StartPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                m_OverPosition = Input.mousePosition;
                distance = m_OverPosition.x - m_StartPosition.x;
            }
            if (Input.GetMouseButtonUp(0))
            {

            }

            if (distance < -m_sensitivity && m_currentPageNum == 1)
            {
                m_currentPageNum++;
                Debug.Log("-100");
                ScrollViewMoveHor(-934);
            }

            if (distance > m_sensitivity && m_currentPageNum == 2)
            {
                Debug.Log("100");
                m_currentPageNum--;
                ScrollViewMoveHor(934);
            }
            distance = 0;
        }

        protected void ChangePageNum()
        {
            if (m_currentPageNum == 1)
            {
                m_pageMgr.GetChild(0).GetComponent<UISprite>().spriteName = "player_hint_down";
                m_pageMgr.GetChild(1).GetComponent<UISprite>().spriteName = "player_hint_up";
            }
            if (m_currentPageNum == 2)
            {
                m_pageMgr.GetChild(1).GetComponent<UISprite>().spriteName = "player_hint_down";
                m_pageMgr.GetChild(0).GetComponent<UISprite>().spriteName = "player_hint_up";
            }
        }
    }
}
