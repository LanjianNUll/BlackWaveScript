//******************************************************************
// File Name:					TaskDefine.cs
// Description:					
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       一些任务模块的定义
// Revision History:
//******************************************************************
namespace FW.Task
{
    /// <summary>
    /// 任务关卡类型
    /// </summary>
    public enum TaskLevelType
    {
        /// <summary>
        /// 不需要进入关卡,就可完成
        /// </summary>
        None = 0,
        /// <summary>
        /// 普通关卡
        /// </summary>
        Normal = 1
    }
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskType
    {
        Normal = 1
    }
    /// <summary>
    ///任务完成类型
    /// </summary>
    public enum TaskCompleteType
    {
        /// <summary>
        /// 通过时间结束来控制
        /// </summary>
        TimeFinish = 0
    }

    /// <summary>
    /// 刷怪的分布方式
    /// </summary>
    public enum EnemyDistributeType
    {
        //最简单的
        Simple = 0,
        LeftRight = 1
    }

    /// <summary>
    /// 刷怪的方式
    /// </summary>
    public enum EnemyAppearType
    {
        //根据时间刷新
        TimeAppear = 0,
        //根据位置刷新
        PosAppear = 1
    }
}



