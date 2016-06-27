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
    public System.Action<uint,List<Entity>> func = null;
    public bool isActive = false;
    public uint skillID = 0;
    public List<Entity> targets = null;
}

public class SkillModule : ModuleBase {
    public int normalAttackStep = 0;
 
    private List<SkillBase> m_normalAttackList = new List<SkillBase>();
    private List<SkillBase> m_skillList = new List<SkillBase>();


    SkillBase m_curSkill = null;
    public bool m_bIsCasting = false;
    List<SKillEvent> m_skillEventList = new List<SKillEvent>();

    public float m_LastNormalAttackTime = 0f;
    SnapShot m_snapShot = null;
    public uint ComboCount = 0;
	public SkillModule(Entity entity) : base(entity)
    {

        SetSpecialSkills(entity.EntityCfg.Skill);
       
    }

    public void SetNormalSkills(string str)
    {
        string[] s = str.Split(new char[]{'|'});
        m_normalAttackList.Clear();
        for(int i = 0; i < s.Length;i++)
        {
            uint SkillID =  System.Convert.ToUInt32(s[i]);
            CSVSkill sk = CSVManager.GetSkillCfg(SkillID);
            if(sk == null)
            {
                Log.Error("不存在技能id " + SkillID);
                continue;
            }
            m_normalAttackList.Add(new SkillBase(sk, m_entity));
        }
    }
    public void SetSpecialSkills(string str)
    {
        string[] s = str.Split(new char[] { '|' });
        m_skillList.Clear();
        for (int i = 0; i < s.Length; i++)
        {
            uint SkillID = System.Convert.ToUInt32(s[i]);
            CSVSkill sk = CSVManager.GetSkillCfg(SkillID);
            if (sk == null)
            {
                Log.Error("不存在技能id " + SkillID);
                continue;
            }
            m_skillList.Add(new SkillBase(sk, m_entity));
        }
    }

    public override void Update()
    {
        CheckSkillEvent();
        for (int i = 0; i < m_skillList.Count; i++)
        {
            if (m_skillList[i] != null)
            {
                m_skillList[i].Update();
            }
        }
        if (m_entity.IsMainPlayer)
        {
            if (m_snapShot == null)
            {
                m_snapShot = m_object.AddComponent<SnapShot>();
            }
            m_snapShot.m_CanBirth = (Time.time - m_LastNormalAttackTime < 1f && normalAttackStep > 0);
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
                evt.func(evt.skillID,evt.targets);
                evt.isActive = false;
            }
        }
    }


    public void ReqCastSkill()
    {

    }

    public  bool CastSkill(int index)
    {
        SkillBase skill = null;
        if (index == 0)  //普攻
        {           
            float delta = Time.time - m_LastNormalAttackTime;

            if (delta <= 0.5f)   //普攻释放频率太高也不行
            {
                return false;
            }
            else if (delta <= 1f)
            {
                normalAttackStep++;              
            }
            else
            {
                normalAttackStep = 0;
            }
            skill = m_normalAttackList[normalAttackStep % m_normalAttackList.Count];
            m_LastNormalAttackTime = Time.time;       //上一次普通攻击的时间
           
        }
        else     //技能
        {
            skill = m_skillList[index - 1];
        }
        return CastSkill(skill);       
    }

    public bool CastSkill(SkillBase skill)
    {
        if (skill == null || m_bIsCasting || skill.IsInCD)
            return false;
        m_entity.Move.StopMove(false);
        m_entity.Anim.Stop();
      

        m_curSkill = skill;
        if (m_curSkill.IsNormalAttackSkill)     //普攻
        {
            m_bIsCasting = false;       //普攻不算技能释放
        }
        else
        {
            m_curSkill.BeginCD();
            m_bIsCasting = true;
        }
        RegSkillEvent(m_curSkill);
        return true;
    }

    void RegSkillEvent(SkillBase skill)
    {

        List<Entity> targets =  skill.GetSkillTargets();

        SKillEvent evt = null;
        //播放特效
        if (!string.IsNullOrEmpty(skill.m_skillInfo.castEffect))  
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.m_skillInfo.castEffectBeginTime / m_entity.Property.AttackSpeed + Time.time;
            evt.func = PlayEffect;
            evt.isActive = true;
            evt.skillID = skill.m_skillInfo.ID;
            AddSkillEvent(evt);
        }

        //播放动作
        if (!string.IsNullOrEmpty(skill.m_skillInfo.castAction))
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.m_skillInfo.castActionBeginTime / m_entity.Property.AttackSpeed + Time.time;
            evt.func = PlayAction;
            evt.isActive = true;
            evt.skillID = skill.m_skillInfo.ID;
            AddSkillEvent(evt);
        }

        //释放子弹
        if (!string.IsNullOrEmpty(skill.m_skillInfo.BulletEffect))
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = skill.m_skillInfo.BulletBeginTime + Time.time;
            evt.skillID = skill.m_skillInfo.ID;
            evt.func = CastBullet;
            evt.isActive = true;
            AddSkillEvent(evt);
        }

        /*技能击中一般分为2种：
        * 1、可配置型：    由时间控制、即hitTime，在技能编辑器中可以调好适当的值， 非子弹型技能一般都可以这样
        * 2、不可配置型：  如子弹型技能，因为与怪距离有远有近，无法配置hitTime，  这种由代码控制
       */

        //击中处理
        if (skill.m_skillInfo.hitTime.Equals(0f) == false)
        {
            evt = GetSkillEvent();
            evt.fTriggerTime = (skill.m_skillInfo.castActionBeginTime + skill.m_skillInfo.hitTime) / m_entity.Property.AttackSpeed + Time.time;
            evt.func = SkillHit;
            evt.isActive = true;
            evt.skillID = skill.m_skillInfo.ID;
            evt.targets = targets;   //只有击中才需要得到技能的作用目标
            AddSkillEvent(evt);
        }

        evt = GetSkillEvent();
        evt.fTriggerTime = (skill.m_skillInfo.castActionBeginTime + skill.m_skillInfo.castActionDuration) / m_entity.Property.AttackSpeed + Time.time;
        evt.func = SkillEnd;
        evt.isActive = true;
        evt.skillID = skill.m_skillInfo.ID;
        AddSkillEvent(evt);
    }


    //由时间控制的击中事件，一般是近战类攻击击中目标
    public void SkillHit(uint skillID, List<Entity> targets = null)
    {
        if(targets == null)
        {
            Log.Info("技能id = " + skillID + " 没有目标");
            return;
        }

        CSVSkill skillInfo = CSVManager.GetSkillCfg(skillID);

        //技能类型(1.普攻 2.物理 3.法术 4.保护 5.治疗 6.辅助 7.召唤 8.被动)
        if (skillInfo.type <= 3)
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
        else if(skillInfo.type <= 6)
        {
            //添加增益buff
            for (int i = 0; i < targets.Count; i++)
            {
            }
        }
    }

    void PlayEffect(uint skillID,List<Entity> targets = null)
    {
        CSVSkill skillInfo = CSVManager.GetSkillCfg(skillID);
        if (string.IsNullOrEmpty(skillInfo.castEffect))
            return;

        EffectEntity effect = EffectManager.Instance.GetEffect(skillInfo.castEffect);
        effect.Init(eDestroyType.Time, skillInfo.castEffectDuration);
        //特效绑定的骨骼

        if (!string.IsNullOrEmpty(skillInfo.castEffectBindBone))
        {
            Transform bone = null;

            if (skillInfo.castEffectBindBone.CompareTo("self") == 0)
                bone = m_object.transform;
            else
                bone = m_entity.GetBone(skillInfo.castEffectBindBone);

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

    void CastBullet(uint skillID, List<Entity> targets = null)
    {
        CSVSkill skillInfo = CSVManager.GetSkillCfg(skillID);
        Bullet bu = BulletManager.Instance.Get();
        if (bu != null)
        {
            bu.Init(m_entity, skillInfo, SkillProcesser.BulletHit, m_entity.SelectTarget);
        }
    }

    void PlayAction(uint skillID, List<Entity> targets = null)
    {
        CSVSkill skillInfo = CSVManager.GetSkillCfg(skillID);
        if(!string.IsNullOrEmpty(skillInfo.castAction))
        {
            m_entity.Anim.SyncAction(skillInfo.castAction);
        }
       
    }

    void SkillEnd(uint skillID, List<Entity> targets = null)
    {
        m_bIsCasting = false;
        //m_entity.Anim.SyncAction("Idle_Sword");
    }

    public override void OnEvent(eEntityEvent eventID, object args = null)
    {
        if (eventID == eEntityEvent.OnSkillResult)
        {

        }
    }
    
}



