using UnityEngine;
using System.Collections;
using System;


public class AnimationModule : ModuleBase
{
    Animator m_animator = null;
	public AnimationModule(Entity entity) : base(entity)
    {
        m_animator = m_object.GetComponent<Animator>();
        SyncAction("Idle_Sword");
    }

     
    public AnimatorStateInfo GetCurAnimatorState()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0);
    }
    public void SyncAction(string animName,bool bFade = true)
    {
        if(m_animator == null)
        {
            Log.Error("不存在Animator组件!");
            return;
        }
        
        m_animator.speed = 1f;
        if (bFade)
        {
            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(animName))   //动作不一样，触发之
            {
                m_animator.SetTrigger(animName);
                //Debug.Log(animName);
            }
            else if (!stateInfo.loop)   //动作一样，但此动作不是循环动作，再次触发之
            {
                m_animator.Play(animName, 0, 0f);
                //Debug.Log(animName);
            }
        }
        else
        {
            //m_animator.Play(animName);
            m_animator.CrossFade(animName, 0.2f);
        }

       
    }

    public override void Update()
    {
        
    }

    public void Stop()
    {
        if (m_animator != null)
            m_animator.speed = 0f;
    }
}
