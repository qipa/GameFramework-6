using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillBase 
{
    public Entity m_caster;
    public CSVSkill m_skillInfo;
    public float m_fCDTotalTime;
    public float m_fCDLeftTime;
    public bool m_bIsCasting = false;

    List<Entity> m_targets = null;
    public bool IsInCD
    {
        get { return m_fCDLeftTime > 0f; }
    }

    //是否为普攻
    public bool IsNormalAttackSkill
    {
        get { return m_skillInfo.type == 1; }
    }

    //是否为攻击性技能
    public bool IsAttackSkill
    {
        get { return m_skillInfo.type == 2 || m_skillInfo.type == 3; }
    }

    //是否为辅助性技能
    public bool IsAssistSkill
    {
        get { return m_skillInfo.type >= 4 && m_skillInfo.type <= 6; }
    }

    //是否为召唤技能
    public bool IsCallSkill
    {
        get { return m_skillInfo.type == 7; }
    }

    //是否为被动技能
    public bool IsPassiveSkill
    {
        get { return m_skillInfo.type == 8; }
    }

    public SkillBase(CSVSkill si,Entity caster)
    {
        m_caster = caster;
        m_skillInfo = si;
        m_fCDTotalTime = si.coolTime;
        m_fCDLeftTime = 0;
        m_targets = new List<Entity>();
    }

    public void BeginCD()
    {
        m_fCDLeftTime = m_fCDTotalTime;
    }


    public virtual void Update()
    {
        if (m_fCDLeftTime > 0)
        {
            m_fCDLeftTime -= Time.deltaTime;
            m_fCDLeftTime = Mathf.Max(0, m_fCDLeftTime);
        }
    }

    public List<Entity> GetSkillTargets()
    {
        m_targets.Clear();
        if(IsNormalAttackSkill)      //普攻
        {          
            if (m_caster.SelectTarget != null && !m_caster.SelectTarget.IsDead)
            {
                m_targets.Add(m_caster.SelectTarget);
                m_caster.Forward = (m_caster.SelectTarget.Pos - m_caster.Pos).normalized;
            }
            else
            {
                m_caster.SelectTarget = SkillProcesser.GetTargetInSkillDistance(m_caster, m_skillInfo);
                if (m_caster.SelectTarget != null)
                {
                    m_targets.Add(m_caster.SelectTarget);
                    m_caster.Forward = (m_caster.SelectTarget.Pos - m_caster.Pos).normalized;
                }
            }
        }
        else    //技能
        {
            if (m_skillInfo.attackType == 1)        //单个目标
            {
                if (m_caster.SelectTarget != null && !m_caster.SelectTarget.IsDead)
                {
                    m_targets.Add(m_caster.SelectTarget);
                    m_caster.Forward = (m_caster.SelectTarget.Pos - m_caster.Pos).normalized;
                }
                else
                {
                    m_caster.SelectTarget = SkillProcesser.GetTargetInSkillDistance(m_caster, m_skillInfo);
                    m_targets.Add(m_caster.SelectTarget);
                    m_caster.Forward = (m_caster.SelectTarget.Pos - m_caster.Pos).normalized;
                }
            }
            else if(m_skillInfo.attackType == 2)        //自己
            {
                m_targets.Add(m_caster);
            }
            else if (m_skillInfo.attackType == 3)      //线性
            {
                var e = EntityManager.Instance.m_dicObject.GetEnumerator();
                while(e.MoveNext())
                {
                    Entity ent = e.Current.Value;

                    if (m_caster.IsEnemy(ent) == false)
                        continue;

                    //自己在纸上画一下就知道啦
                    /*
                       v3  v1
                        \  |
                         \ |
                          \|
                    */

                    Vector3 v1 = m_caster.Forward;
                    Vector3 v2 = ent.Pos - m_caster.Pos;
                    Vector3 v3 = v2.normalized;

                    float cos_value = Vector3.Dot(v1, v3);
                    float dv = Mathf.Abs(cos_value * cos_value * v2.sqrMagnitude);     //forward方向的距离的平方

                    float sin_value = Mathf.Sin(Mathf.Acos(cos_value));
                    float dh = Mathf.Abs(sin_value * sin_value * v2.sqrMagnitude);      //left、right方向的距离的平方

                    if(dh <= 1f && dv <= m_skillInfo.attackDistance * m_skillInfo.attackDistance)
                    {
                        m_targets.Add(ent);
                    }
                }
            }
            else if (m_skillInfo.attackType == 4)     //扇形，目前都是判断 60度，后续有需求可配表
            {
                 var e = EntityManager.Instance.m_dicObject.GetEnumerator();
                 while (e.MoveNext())
                 {
                     Entity ent = e.Current.Value;
                     if (m_caster.IsEnemy(ent) == false)
                         continue;
                     Vector3 v1 = m_caster.Forward;
                     Vector3 v2 = ent.Pos - m_caster.Pos;

                     if (v2.sqrMagnitude <= m_skillInfo.attackDistance * m_skillInfo.attackDistance)
                     {
                         float cos_value = Vector3.Dot(v1.normalized, v2.normalized);
                         if (cos_value >= Mathf.Cos(Mathf.PI / 3))    // 60度
                         {
                             m_targets.Add(ent);
                         }
                     }
                 }
            }
            else if (m_skillInfo.attackType == 5)        //圆形
            {
                //圆形范围技能，一般还要判断作用对象是敌人还是自己人

                var e = EntityManager.Instance.m_dicObject.GetEnumerator();
                while (e.MoveNext())
                {
                    Entity ent = e.Current.Value;

                    if (IsAttackSkill)   //如果是攻击性技能，作用对象是敌人
                    {
                        if (m_caster.IsEnemy(ent) == false)
                            continue;
                    }
                    else if(IsAssistSkill)  //如果是辅助性技能
                    { //作用对象是自己人
                        if (m_caster.IsEnemy(ent) == true)
                            continue;
                    }

                    if( (ent.Pos - m_caster.Pos).sqrMagnitude <= m_skillInfo.attackDistance * m_skillInfo.attackDistance )
                    {
                        m_targets.Add(ent);
                    }
                }
            }
           
        }

        return m_targets;
    }
    
}

