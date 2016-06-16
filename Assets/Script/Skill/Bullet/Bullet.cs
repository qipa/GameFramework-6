using UnityEngine;
using System.Collections;

public enum eBulletType
{
    Dir,        //方向型，无目标
    Target,     //目标型
}
//子弹类型 

public delegate void BulletHitCallback(Entity caster,Entity target,CSVSkill skillInfo);
public class Bullet  
{
    public BulletHitCallback m_hitNotify;
    private EffectEntity m_effect;
    private float m_fSpeed;
    private CSVSkill m_skillInfo;
    private Entity m_caster;
    private Entity m_target;
    private Vector3 m_dir;
    public bool IsUsing = false;
    private float duration = 0f;
    private eBulletType bulletType = eBulletType.Dir;
    public Vector3 Pos
    {
        set { m_effect.Pos = value; }
        get { return m_effect.Pos; }
    }

    public void Init(Entity caster, CSVSkill skillInfo, BulletHitCallback notify = null, Entity target = null)
    {
        m_effect = EffectManager.Instance.GetEffect(skillInfo.BulletEffect);
        if (m_effect == null)
        {
            Log.Error("不存在特效 " + skillInfo.BulletEffect);
            return;
        }
        m_caster = caster;
        m_target = target;
        if (target == null)
        {
            bulletType = eBulletType.Dir;
            m_dir = m_caster.Forward;
        }
        else
        {
            bulletType = eBulletType.Target;
            m_dir = (m_target.Pos - m_caster.Pos).normalized;
        }

        m_skillInfo = skillInfo;
        m_hitNotify = notify;
        //子弹存在的时间 = 子弹的攻击距离 / 子弹的飞行速度  小学物理知识  呵呵哒
        duration = m_skillInfo.attackDistance / m_skillInfo.flySpeed;
        m_effect.Init(eDestroyType.Time, duration);
        m_fSpeed = m_skillInfo.flySpeed;

        if (!string.IsNullOrEmpty(m_skillInfo.BulletBindBone))
            m_effect.Pos = GetBonePos(m_caster, m_skillInfo.BulletBindBone);
        else
            m_effect.Pos = new Vector3(m_caster.Pos.x,m_caster.Pos.y+0.5f,m_caster.Pos.z);
        
        IsUsing = true;
        
    }

    Vector3 GetBonePos(Entity owner,string bone)
    {
        Transform tf = owner.GetBone(bone);
        if (tf != null)
            return tf.position;
        return owner.Pos;
    }

    public void Update()
    {
        if (!IsUsing)
            return;
        if (bulletType == eBulletType.Target)
        {
            m_dir = (m_target.Pos - m_caster.Pos).normalized;
        }
        

        m_effect.Pos += m_dir * m_fSpeed * Time.deltaTime;
        m_effect.Forward = m_dir;

        //击中处理
        if(IsHit() && null != m_hitNotify)
        {
            m_effect.SetActive(false);
            IsUsing = false;
            m_hitNotify(m_caster, m_target, m_skillInfo);
        }

        duration -= Time.deltaTime;
        //子弹攻击距离已过
        if (duration <= 0)
        {
            m_effect.SetActive(false);
            IsUsing = false;
        }
    }

    bool IsHit()
    {
        bool result = false;
        if (bulletType == eBulletType.Target)
        {
            float dis = (m_effect.Pos - m_target.Pos).sqrMagnitude;
            if ( dis <= 10f)
                result = true;
        }
        else
        {
            var e = EntityManager.Instance.m_dicObject.GetEnumerator();
            while (e.MoveNext())
            {
                Entity ent = e.Current.Value;
                if (ent.Camp == eCamp.Hero || ent.Camp == eCamp.Friend)
                    continue;

                if ((m_effect.Pos - ent.Pos).sqrMagnitude < 0.5f)
                {
                    m_target = ent;
                    result = true;
                    break;
                }

            }
        }
        return result;
    }
}
