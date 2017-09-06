//******************************************************************
// File Name:					SceneControl.cs
// Description:					SceneControl class 
// Author:						wuwei
// Date:						2016.12.27
// Reference:
// Using:
// Revision History:
//******************************************************************

using UnityEngine;
using System.Collections;

namespace FW
{
    class SceneControl : MonoBehaviour
    {
        void Awake()
        {
            Utility.DebugEx.DebugLog.Open();
            GameControl.Init();
        }

        void Start()
        {
            GameControl.Start();
        }

        void Update()
        {
            GameControl.Update(Time.deltaTime);
        }

        //先把微信回调的方法放这里
        public void WChatBack(string args)
        {
            Debug.Log("回调了");
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.WChat_back_Info, new Event.EventArg(args));
        }
    }
}