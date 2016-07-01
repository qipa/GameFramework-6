using System;
using System.Collections.Generic;
using UnityEngine;
//用来模拟服务器，接收处理技能的请求，并返回结果
public class SkillProcesser
{
    public static void ProcessSkill(ReqCastSkill msg)
    {
        Entity caster = EntityManager.Instance.Find(msg.caster);
        if (caster == null)
        {
            Log.Error("找不到技能释放者!  id = " + msg.caster);
            return;
        }
        CSVSkill skillInfo = CSVManager.GetSkillCfg(msg.skillID);
        if (skillInfo == null)
        {
            Log.Error("找不到技能配置 skillid = " + msg.skillID);
            return;
        }

        List<Entity> targets = GetTargets(msg, skillInfo);
        if (targets == null)
            return;

        List<SkillResult> result = null;
        for (int i = 0; i < targets.Count; i++)
        {
            SkillResult sr = new SkillResult();
            sr.target = msg.target;
            if (skillInfo.type == 1) //普攻
            {
                sr.damage = caster.Property.PhysicDamage - targets[i].Property.PhysicDefence;
            }
            else if (skillInfo.type == 2)        //物理技能攻击
            {

            }
            else if (skillInfo.type == 3)    //法术技能攻击
            {

            }
            result.Add(sr);
        }
        OnCastSkill rsp = new OnCastSkill();
        rsp.result = result;
        caster.OnEvent(eEntityEvent.OnSkillResult, rsp);
    }

    //得到这个技能作用的目标
    private static List<Entity> GetTargets(ReqCastSkill msg, CSVSkill skillInfo)
    {
        List<Entity> targets = null;
        if (skillInfo.type == 1)     //普攻
        {
            Entity target = EntityManager.Instance.Find(msg.target);
            if (targets == null)
            {
                Log.Error("找不到普通攻击的目标  target = " + msg.target);
                return null;
            }
            targets = new List<Entity>();
            targets.Add(target);
        }
        else       //技能
        {

        }
        return targets;
    }
    public static Entity GetTargetInSkillDistance(Entity caster, CSVSkill skillInfo)
    {
        Entity ent = null;
        var e = EntityManager.Instance.m_dicObject.GetEnumerator();
        while (e.MoveNext())
        {
            ent = e.Current.Value;
            if (caster.IsEnemy(ent) == false || ent.IsDead)
                continue;
                
            if ((ent.Pos - caster.Pos).sqrMagnitude <= skillInfo.attackDistance * skillInfo.attackDistance)
            {
                return ent;
            }
        }
        return null;
    }

    //得到以ent为中心，在distance范围内的敌人
    public static Entity GetTargetInDistance(Entity ent,float distance)
    {
        Entity ret = null;
        var e = EntityManager.Instance.m_dicObject.GetEnumerator();
        while (e.MoveNext())
        {
            ret = e.Current.Value;
            if (ret.IsDead )
                continue;
            if (ent.IsEnemy(ret) == false)
                continue;
            if ((ret.Pos - ent.Pos).sqrMagnitude <= distance * distance)
            {
                return ret;
            }
        }
        return null;

    }

    public static void BulletHit(Entity caster, Entity target, CSVSkill skillInfo)
    {
        if (target.IsDead || target.invincible)
            return;

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

        CalcBlood(caster, target, skillInfo);
    }

    //计算伤害
    public static void CalcBlood(Entity caster,Entity target,CSVSkill skillInfo)
    {
        if (target == null) return;
        //target.Buff.AddBuff((int)skillInfo.attackBuff);
        bool isCrit = false;
        int damage = GetDamage(caster, target, skillInfo, out isCrit);

        if(damage != 0)
        {
            target.Property.HP -= damage;
            if (target.IsDead)
                target.Die();
        }
    }

    static int GetDamage(Entity caster,Entity target,CSVSkill skillInfo,out bool isCrit)
    {
        float damage = 0;
        int randomValue = UnityEngine.Random.Range(20, 40);

        float realCritRate = Mathf.Min(caster.Property.CritRate - target.Property.AvoidCritRate, 0.8f);
        isCrit = GetProResult(realCritRate);
        switch(skillInfo.type)
        {
            case 1:      //普攻
                damage = Mathf.RoundToInt((Mathf.Max(caster.Property.PhysicDamage + 
                    caster.Property.MagicDamage - target.Property.PhysicDefence - target.Property.MagicDefence, 0)) + randomValue );
                break;
            case 2:     //物攻
                damage = Mathf.RoundToInt(Mathf.Max(caster.Property.PhysicDamage - target.Property.PhysicDefence, 0) + randomValue );
                break;
            case 3:     //法攻
                damage = Mathf.RoundToInt(Mathf.Max(caster.Property.MagicDamage - target.Property.MagicDefence, 0) + randomValue);
                break;
            default: break;
        }
        if(isCrit)
        {
            float f = UnityEngine.Random.Range(1.5f, 2);
            damage *= 2;
            if(caster.IsMainPlayer)
            {
                CameraController.Instance.Shake();
            }
        }
        return Mathf.RoundToInt(damage);
    }
    public static bool GetProResult(float pro)
    {
        if (pro >= 1)
            return true;
        else if (pro <= 0)
            return false;
        //取小数后四位
        int probability = Mathf.RoundToInt((float)Math.Round(pro, 4) * 10000);
        int value = UnityEngine.Random.Range(0, 10000);
        return value > probability ? false : true;
    }
}