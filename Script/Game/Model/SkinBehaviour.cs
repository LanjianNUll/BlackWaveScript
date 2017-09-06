//******************************************************************
// File Name:					SkinBehaviour.cs
// Description:					SkinBehaviour class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       绑定到游戏对象上,角色的模型类
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkinBehaviour : MonoBehaviour {
    public const string ANIM_FIRE_NAME = "Fire";
    public const string ANIM_DEAD_NAME = "Dead";
    public const string ANIM_WALK_NAME = "RunF";
    public const string ANIM_IDLE_NAME = "Idle";
    public delegate void AnimFinishCallback();

    private Dictionary<string, AnimFinishCallback> m_callbackDic = new Dictionary<string, AnimFinishCallback>();
    private Animation m_animation;

    //--------------------------------------
    //private 
    //--------------------------------------
    private void Awake () {
        m_animation = GetComponent<Animation>();
        //注册监听事件
        RegistAnimEvent(ANIM_FIRE_NAME, "FireFinishCallback");
        RegistAnimEvent(ANIM_DEAD_NAME, "DieFinishCallback");
        RegistAnimEvent(ANIM_WALK_NAME, "DieFinishCallback");
        RegistAnimEvent(ANIM_IDLE_NAME, "DieFinishCallback");
    }

    //检测是否注册了动画
    private bool CheckAnimEvent(string name, string method, AnimationEvent e)
    {
        AnimationEvent[] events = m_animation[name].clip.events;
        if (events.Length == 0)
            return false;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].functionName == method && events[i].time == e.time)
                return true;
        }
        return false;
    }

    //--------------------------------------
    //public 播放动画 
    //--------------------------------------
    public void Play(string name, bool loop = false, SkinBehaviour.AnimFinishCallback callback = null)
    {
        if (this.m_animation == null) return;
        if (m_animation.GetClip(name) == null) {
            Debug.LogWarning("can not find anim,name:"+name);
            return;
        }
        
        AnimationState state = this.m_animation[name];
        state.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
        this.m_animation.Play(name);
        if (callback != null)
        {
            if (m_callbackDic.ContainsKey(name))
            {
                m_callbackDic[name] = callback;
            }
            else
            {
                m_callbackDic.Add(name, callback);
            }
        }
    }

    //注册动画播放完成后的事件
    public void RegistAnimEvent(string name, string method)
    {
        AnimationEvent e = new AnimationEvent();
        if (m_animation.GetClip(name) != null)
        {
            e.time = m_animation[name].clip.length;
            e.functionName = method;
            if (CheckAnimEvent(name, method, e) == false)
                m_animation[name].clip.AddEvent(e);
        }

    }

    public void UnRegistAnimEvent(string name)
    {
        if (m_animation.GetClip(name) != null)
        {
           m_animation[name].clip.events=null;
        }
    }
    

    //射击动作完成后的回调
    public void FireFinishCallback()
    {
        //Debug.LogError(name+ " FireFinishCallback");
        if (m_callbackDic.ContainsKey(ANIM_FIRE_NAME))
        {
            m_callbackDic[ANIM_FIRE_NAME]();
            //m_callbackDic.Remove(ANIM_FIRE_NAME);
        }
    }
    //死亡动作完成后的回调
    public void DieFinishCallback()
    {
        if (m_callbackDic.ContainsKey(ANIM_DEAD_NAME))
        {
            m_callbackDic[ANIM_DEAD_NAME]();
            m_callbackDic.Remove(ANIM_DEAD_NAME);
        }
    }

    public void Dispose()
    {
        //TODO 测试看删除监听是否成功
        //UnRegistAnimEvent(ANIM_FIRE_NAME);
        //UnRegistAnimEvent(ANIM_DEAD_NAME);
        //UnRegistAnimEvent(ANIM_WALK_NAME);
        //UnRegistAnimEvent(ANIM_IDLE_NAME);
    }
}
