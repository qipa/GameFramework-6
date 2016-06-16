using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//请求释放技能
public class ReqCastSkill
{
    public uint skillID;    //技能id
    public ulong caster;    //释放者id
    public ulong target;    //目标id(如果有的话)    
    public Vector3 dir;     //技能的方向 如直线攻击
    public Vector3 pos;     //技能的释放点  如范围攻击
}

public class SkillResult
{
    public ulong target;        //技能作用的目标
    public float damage;        //技能造成的伤害，如果是负数，说明是加血等技能
}
//技能释放结果
public class OnCastSkill
{
    public List<SkillResult> result = null;
}

class SKillEvent
{
    public float fTriggerTime = 0f;    //事件触发时间
    public System.Action<CSVSkill> func = null;
    public bool isActive = false;
    public CSVSkill skillInfo = null;
}

public class SkillModule : ModuleBase {
    public int normalAttackStep = 0;
 
    private List<SkillBase> normalAttacks = new List<SkillBase>();
    private List<SkillBase> Skills = new List<SkillBase>();
    List<Entity> targets = new List<Entity>();

    SkillBase CurSkill = null;
    bool m_bIsCasting = false;
    List<SKillEvent> m_skillEventList = new List<SKillEvent>();

    public float LastNormalAttackTime = 0f;

	public SkillModule(Entity entity) : base(entity)
    {
        
        
    }

    public void SetNormalSkills(string str)
    {
        string[] s = str.Split(new char[]{'|'});
        normalAttacks.Clear();
        for(int i = 0; i < s.Length;i++)
        {
            uint SkillID =  System.Convert.ToUInt32(s[i]);
            CSVSkill sk = CSVManager.GetSkillCfg(SkillID);
            if(sk == null)
            {
                Log.Error("不存在技能id " + SkillID);
                continue;
            }
            normalAttacks.Add(new SkillBase(sk));
        }
    }

    public override void Update()
    {
        CheckSkillEvent();
        for(int i = 0; i < Skills.Count;i++)
        {
            if(Skills[i] != null)
            {
                Skills[i].Update();
            }
        }
       
    }

    void AddSkillEvent(SKillEvent evt)
    {
        m_skillEventList.Add(evt);
    }

    SKillEvent GetSkillEvent()
    {
        SKillEvent evt = null;
        for (var i = 0; i < m_skillEventList.Count;++i )
        {
            if (!m_skillEventList[i].isActive)
                return m_skillEventList[i];
        }
        evt = new SKillEvent();
        m_skillEventList.Add(evt);
        return evt;
    }
    
    void CheckSkillEvent()
    {
        for (var i = 0; i < m_skillEventList.Count; ++i)
        {
            SKillEvent evt = m_skillEventList[i];
            if (Time.time >= evt.fTriggerTime && evt.isActive)   //触发时间到了
            {
                evt.func(evt.skillInfo);
                evt.isActive = false;
            }
        }
    }

    public virtual void GetTargets()   //得到技能的释放对象
    {
        // 1:普通攻击  2：直线   3：扇形   4：范围
        targets.Clear();
//         if (CurSkill.skillInfo.uSkillType == 1)
//         {
//             if (m_entity.SelectTarget != null)
//             {
//                 targets.Add(m_entity.SelectTarget);
//             }
//         }
//         else if (CurSkill.skillInfo.uSkillType == 2)
//         {
// 
//         }
//         else if (CurSkill.skillInfo.uSkillType == 3)
//         {
// 
//         }
//         else if (CurSkill.skillInfo.uSkillType == 4)
//         {
// 
//         }
    }

    public void ReqCastSkill()
    {

    }

    public  bool CastSkill(int index)
    {
        SkillBase skill = null;
        if (index == 0)  //普攻
        {           
            if (Time.time - LastNormalAttackTime > 1f)
            {
                normalAttackStep = 0;
            }
            else
            {
                normalAttackStep++;
            }
            skill = normalAttacks[normalAttackStep % normalAttacks.Count];
            LastNormalAttackTime = Time.time;       //上一次普通攻击的时间
           
        }
        else     //技能
        {
            skill = Skills[index];
        }
        return CastSkill(skill);       
    }

    public bool CastSkill(SkillBase skill)
    {
        if (skill == null || m_bIsCasting)
            return false;
        m_entity.Move.StopMove(false);
        m_entity.Anim.Stop();
        GetTargets();

        CurSkill = skill;
        if (CurSkill.skillInfo.type == 1)        //普攻
        {
            if (m_entity.SelectTarget == null)
                m_entity.SelectTarget = FindTarget(CurSkill.skillInfo);
            if (m_entity.SelectTarget != null)
            {
                targets.Add(m_entity.SelectTarget);
                m_entity.Forward = (m_entity.SelectTarget.Pos - m_entity.Pos).normalized;
            }
        }
        else
        {
            CurSkill.BeginCD();
        }
        RegSkillEvent(CurSkill);
        return true;
    }

    void RegSkillEvent(SkillBase skill)
    {

        SKillEvent evt = null;
        //播放特效
        if (!string.IsNullOrEmpty(skill.skillInfo.castEffect))  
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.skillInfo.castEffectBeginTime / m_entity.Property.AttackSpeed + Time.time;
            evt.func = PlayEffect;
            evt.isActive = true;
            evt.skillInfo = skill.skillInfo;
            AddSkillEvent(evt);
        }

        //播放动作
        if (!string.IsNullOrEmpty(skill.skillInfo.castAction))
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.skillInfo.castActionBeginTime / m_entity.Property.AttackSpeed + Time.time;
            evt.func = PlayAction;
            evt.isActive = true;
            evt.skillInfo = skill.skillInfo;
            AddSkillEvent(evt);
        }

        if (!string.IsNullOrEmpty(skill.skillInfo.BulletEffect))
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.skillInfo.BulletBeginTime + Time.time;
            evt.skillInfo = skill.skillInfo;
            evt.func = CastBullet;
            evt.isActive = true;
            AddSkillEvent(evt);
        }
        //击中处理
        if (skill.skillInfo.hitTime.Equals(0f) == false)
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = (skill.skillInfo.castActionBeginTime + skill.skillInfo.hitTime) / m_entity.Property.AttackSpeed + Time.time;
            evt.func = SkillHit;
            evt.isActive = true;
            evt.skillInfo = skill.skillInfo;
            AddSkillEvent(evt);
        }

//         //combo时机开始
//         if (CurSkill.skillInfo.comboBeginTime != 0)                      
//         {
//             evt = GetSkillEvent();
//             evt.func = ComboBegin;
//             evt.fTriggerTime = CurSkill.skillInfo.comboBeginTime / m_entity.Property.AttackSpeed + Time.time;
//             evt.isActive = true;
//             AddSkillEvent(evt);
//             
//         }
// 
//         //combo时机结束
//         if (CurSkill.skillInfo.comboEndTime != 0)                       
//         {
//             evt = GetSkillEvent();
//             evt.func = ComboEnd;
//             evt.fTriggerTime = (CurSkill.skillInfo.comboBeginTime + CurSkill.skillInfo.comboEndTime) / m_entity.Property.AttackSpeed + Time.time;
//             evt.isActive = true;
//             AddSkillEvent(evt);
//             
//         }

        evt = GetSkillEvent();
        evt.fTriggerTime = (skill.skillInfo.castActionBeginTime + skill.skillInfo.castActionDuration) / m_entity.Property.AttackSpeed + Time.time;
        evt.func = SkillEnd;
        evt.isActive = true;
        evt.skillInfo = skill.skillInfo;
        AddSkillEvent(evt);
    }


    //由时间控制的击中事件，一般是近战类攻击击中目标
    public void SkillHit(CSVSkill skillInfo)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Entity target = targets[i];
            //受击动作
            if (!string.IsNullOrEmpty(skillInfo.beattackAction))
            {
                target.Anim.SyncAction(skillInfo.beattackAction);
            }
            //受击特效

            if (!string.IsNullOrEmpty(skillInfo.beattackEffect))
            {
                EffectEntity effect = EffectManager.Instance.GetEffect(skillInfo.beattackEffect);
                effect.Init(eDestroyType.Time, skillInfo.beattackActionDuration);

                effect.Pos = target.Pos;
                effect.Forward = target.Forward;
            }
        }
    }

    void PlayEffect(CSVSkill skillInfo)
    {
        
        if (string.IsNullOrEmpty(skillInfo.castEffect))
            return;

        EffectEntity effect = EffectManager.Instance.GetEffect(skillInfo.castEffect);
        effect.Init(eDestroyType.Time, skillInfo.castEffectDuration);
        //特效绑定的骨骼

        if (!string.IsNullOrEmpty(skillInfo.castEffectBindBone))
        {
            Transform bone = m_entity.GetBone(skillInfo.castEffectBindBone);
            if (bone != null)
            {
                effect.Bind(bone);
            }
        }
        //如果特效没有绑定位置，则默认位置为释放者的位置
        else
        {
            effect.Pos = m_entity.Pos;
            effect.Forward = m_entity.Forward;
        }
    }

    void CastBullet(CSVSkill skillInfo)
    {
        //子弹型普攻  即远程普攻
        if (skillInfo.attackType == 3 || skillInfo.attackType == 4)
        {
            Bullet bu = BulletManager.Instance.Get();
            if (bu != null)
            {
                if (m_entity.SelectTarget == null)
                    m_entity.SelectTarget = FindTarget(skillInfo);
                bu.Init(m_entity, skillInfo, BulletManager.BulletHit, m_entity.SelectTarget);
            }
        }
    }

    void PlayAction(CSVSkill skillInfo)
    {
        if(!string.IsNullOrEmpty(skillInfo.castAction))
        {
            m_entity.Anim.SyncAction(skillInfo.castAction);
        }
       
    }

    void SkillEnd(CSVSkill skillInfo)
    {
        
        //m_entity.Anim.SyncAction("Idle_Sword");
    }

    public override void OnEvent(eEntityEvent eventID, object args = null)
    {
        if (eventID == eEntityEvent.OnSkillResult)
        {

        }
    }
    

    public Entity FindTarget(CSVSkill skillInfo)
    {
        Entity ent = null;
        var e = EntityManager.Instance.m_dicObject.GetEnumerator();
        while(e.MoveNext())
        {
            ent = e.Current.Value;
            if (ent.Camp == eCamp.Hero || ent.Camp == eCamp.Friend)
                continue;
            if ((ent.Pos - m_entity.Pos).sqrMagnitude <= skillInfo.attackDistance * skillInfo.attackDistance)
            {
                return ent;
            }
        }
        return null;
    }
}



