//******************************************************************
// File Name:					ViewWeaponModelDialogUI
// Description:					ViewWeaponModelDialogUI class 
// Author:						lanjian
// Date:						2/6/2017 2:33:45 PM
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
    /// 查看3D模型详细
    /// </summary>
    class ViewWeaponModelDialogUI
    {
        private static ViewWeaponModelDialogUI sm_WModelDialogUI;
        public static ViewWeaponModelDialogUI Instance
        {
            get
            {
                if (sm_WModelDialogUI == null) sm_WModelDialogUI = new ViewWeaponModelDialogUI();
                return sm_WModelDialogUI;
            }
        }

        private GameObject m_CurrentModel;
    }
}
