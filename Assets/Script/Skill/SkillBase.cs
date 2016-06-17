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

    public bool IsNormalAttack
    {
        get { return m_skillInfo.type == 1; }
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
        if(IsNormalAttack)      //普攻
        {
            m_caster.SelectTarget = SkillProcesser.GetTargetInSkillDistance(m_caster, m_skillInfo);
            if (m_caster.SelectTarget != null)
            {
                m_targets.Add(m_caster.SelectTarget);
                m_caster.Forward = (m_caster.SelectTarget.Pos - m_caster.Pos).normalized;
            }
        }
        else    //技能
        {
            if (m_skillInfo.attackType == 1)        //单个目标
            {
                m_caster.SelectTarget = SkillProcesser.GetTargetInSkillDistance(m_caster, m_skillInfo);
                if (m_caster.SelectTarget != null)
                {
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
                    if (ent.Camp == eCamp.Hero || ent.Camp == eCamp.Friend)
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
                     if (ent.Camp == eCamp.Hero || ent.Camp == eCamp.Friend)
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
            else if (m_skillInfo.attackType == 5)     //圆形
            {
                
                //技能类型(1.普攻 2.物理 3.法术 4.保护 5.治疗 6.辅助 7.召唤 8.被动)

                bool toEnemy = m_skillInfo.type <= 3;

                var e = EntityManager.Instance.m_dicObject.GetEnumerator();
                while (e.MoveNext())
                {
                    Entity ent = e.Current.Value;

                    if (toEnemy)   //作用对象是敌人
                    {
                        if (ent.Camp == eCamp.Hero || ent.Camp == eCamp.Friend)
                            continue;
                    }
                    else
                    { //作用对象是自己人
                        if (ent.Camp == eCamp.Enemy || ent.Camp == eCamp.Boss)
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

