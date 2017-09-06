//******************************************************************
// File Name:					ScrollViewItemBase
// Description:					ScrollViewItemBase class 
// Author:						lanjian
// Date:						1/13/2017 11:28:36 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    abstract class ScrollViewItemBase
    {
        protected string m_PageName;
        protected int m_PageIndex;
        protected GameObject m_CurrentItem;
        protected string gunItemList = "";
        protected float m_ItemHeight = 1400;
        private float m_ItemWidth = 934;
        protected ScrollViewItemBase()
        {
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject CurrentItem{ get{ return m_CurrentItem; } }
     
        public virtual void Init()
        {
            this.m_CurrentItem = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(this.m_PageName) as GameObject);
            if (this.m_CurrentItem != null)
            {
                //找到每一个panel下的UIGrid为父物体  将页放进来
                this.m_CurrentItem.transform.parent = PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0).GetChild(0);
                this.m_CurrentItem.transform.localScale = Vector3.one;
                this.m_CurrentItem.transform.localPosition = new Vector3((m_PageIndex-1)* m_ItemWidth, 0,0);
            }
        }

        public virtual void SetPosition(int index)
        {
            this.m_CurrentItem.transform.localPosition = new Vector3(index * m_ItemWidth, 0, 0);
        }

        public virtual void FindItem() { }

        public virtual void Update() {  }

        public virtual void SecondInvoke() { }

        //填充数据
        public abstract void FillItem(FW.Event.EventArg eventArg);

        public virtual List<GameObject> LoadItemPath(int itemCount, string path, Transform parent)
        {
            GameObject prefab = GameObject.Instantiate(ResMgr.ResLoad.Load(path) as GameObject);
            prefab.transform.parent = parent;
            prefab.transform.localScale = Vector3.one;
            if (prefab == null) return null;
            return this.LoadItem(itemCount,prefab,parent);
        }


        public virtual List<GameObject> LoadItem(int itemCount, GameObject prefabs, Transform parent)
        {
            List<GameObject> itemList = new List<GameObject>();
            //拿原有的来复制
            //GameObject prefabs = this.CurrentItem.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            //把原件加进来
            itemList.Add(prefabs);
            prefabs.transform.parent = parent;
            prefabs.name = "itemList0";
            for (int i = 0; i < itemCount - 1; i++)
            {
                GameObject item = GameObject.Instantiate(prefabs);
                //item.transform.parent = this.CurrentItem.transform.GetChild(1).GetChild(0).transform;
                item.transform.parent = parent;
                item.transform.localScale = Vector3.one;
                item.name = "itemList" + (i + 1);
                itemList.Add(item);
            }
            return itemList;
        }

        //加载下一个Item
        public GameObject ReloadItem(int tabindex, bool first = false)
        {
            GameObject prefabs;
            if (first)
            {
                prefabs = UnityEngine.Object.Instantiate(ResMgr.ResLoad.Load(gunItemList) as GameObject);
                prefabs.transform.parent = this.CurrentItem.transform.GetChild(1).GetChild(tabindex).GetChild(0);
                prefabs.transform.localScale = Vector3.one;
                prefabs.transform.localPosition = Vector3.zero;
                prefabs.name = "itemList0";
                return prefabs;
            }
            prefabs = this.CurrentItem.transform.GetChild(1).GetChild(tabindex).GetChild(0).
                GetChild(0).gameObject;
            int num = this.CurrentItem.transform.GetChild(1).GetChild(tabindex).GetChild(0).childCount;
            GameObject item = GameObject.Instantiate(prefabs);
            item.transform.parent = this.CurrentItem.transform.GetChild(1).GetChild(tabindex).GetChild(0);
            item.transform.localScale = Vector3.one;
            item.name = "itemList" + num;
            item.transform.localPosition = new Vector3(0, -m_ItemHeight * num, 0);
            //重置下位置
            this.CurrentItem.transform.GetChild(1).GetChild(tabindex).GetComponent<UIScrollView>().ResetPosition();
            return item;
        }

        //销毁那个一个第所有子物体
        public void DestroyChilds(int index)
        {
            Transform parentTr = this.CurrentItem.transform.GetChild(1).GetChild(index).GetChild(0);
            for (int i = 0; i < parentTr.childCount; i++)
            {
                UnityEngine.Object.Destroy(parentTr.GetChild(i).gameObject);
            }
        }

        //刷新页面
        private long id;
        public virtual void ReLoadpage()
        {
            //设置个时间段后刷新，保证Destroy完了 
            id = Timer.Regist(0.2f, 2, 1, RefreshUI);
        }

        public virtual void RefreshUI() { }

        public virtual void DisPose()
        {
            if (this.m_CurrentItem != null)
                UnityEngine.Object.Destroy(this.m_CurrentItem);
            Timer.Cancel(id);
        }
    }
}
