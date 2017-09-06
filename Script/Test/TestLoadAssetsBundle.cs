//******************************************************************
// File Name:					TestLoadAssetsBundle
// Description:					TestLoadAssetsBundle class 
// Author:						lanjian
// Date:						2/21/2017 9:58:57 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Test
{
    class TestLoadAssetsBundle:UIEventBase
    {
        public Texture[] textureList;
        private float m_progress;
        IEnumerator Start()
        {
            string path = string.Empty;
            if (Application.platform.Equals(RuntimePlatform.WindowsEditor))
               path = "file://" + Application.dataPath + "/StreamingAssets/";
            if (Application.platform.Equals(RuntimePlatform.Android))
                path = "jar:file://" + Application.dataPath + "!/assets/";
            if (Application.platform.Equals(RuntimePlatform.IPhonePlayer))
                path = Application.dataPath + "/Raw/";
            using (WWW www = new WWW(path + "Android/texture"))
            {
                m_progress = www.progress;
                yield return www;
                textureList = www.assetBundle.LoadAllAssets<Texture>();
                www.assetBundle.Unload(false);
            }
            foreach (var item in textureList)
            {
                Debug.Log("资源的名称----："+ item.name);
            }
        }

        void Update()
        {

        }
    }
}
