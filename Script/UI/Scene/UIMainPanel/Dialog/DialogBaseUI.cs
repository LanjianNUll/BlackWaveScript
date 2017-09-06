//******************************************************************
// File Name:					DialogBaseUI
// Description:					DialogBaseUI class 
// Author:						lanjian
// Date:						2/27/2017 2:54:53 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    abstract class DialogBaseUI
    {
        protected string m_resName = "";
        protected GameObject m_DialogUIGo;
        protected DialogType m_DType;
        protected FW.Event.EventArg m_currentArgs;//当前传入的参数 
        //需要隐藏的界面
        protected List<Transform> m_needHideGo = new List<Transform>();
        public DialogBaseUI() { }
        //--------------------------------------
        //public 
        //--------------------------------------
        public virtual void Init()
        {
            this.m_DialogUIGo = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(this.m_resName) as GameObject);
            this.m_DialogUIGo.transform.localScale = Vector3.one;
        }

        //设置父物体
        public void SetDialogParent(Transform parent)
        {
            if (parent != null)
            {
                m_DialogUIGo.transform.SetParent(parent);
                this.m_DialogUIGo.transform.localScale = Vector3.one;
            }
        }

        //显示还是隐藏
        public void ShowOrHideAll(bool isShowOrHide)
        {
            for (int i = 0; i < m_needHideGo.Count; i++)
            {
                NGUITools.SetActive(m_needHideGo[i].gameObject, isShowOrHide);
            }
        }

        public virtual void DisPose()
        {
            if (this.m_DialogUIGo != null)
                UnityEngine.Object.Destroy(this.m_DialogUIGo);
        }

        public virtual void OpenDialog()
        {
            NGUITools.SetActive(this.m_DialogUIGo, true);
            ShowOrHideAll(false);
        }

        public virtual void CloseDialog()
        {
            NGUITools.SetActive(this.m_DialogUIGo, false);
            ShowOrHideAll(true);
            DialogMgr.Dispose();
        }

        public virtual void ShowCommonDialog(FW.Event.EventArg args)
        {

        }

        public virtual void OnCancel(GameObject go)
        {
            this.CloseDialog();
        }

        public virtual void OnBack(GameObject go)
        {
            this.CloseDialog();
        }

        public virtual void GetNeedHideGo()
        {
            m_needHideGo.Clear();
            m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0));
        }

        //对话框的update
        public virtual void UpadateDialog(){}
        //对话框隔一秒调用
        public virtual void SecondUpdateDialog() { }

    }
}
