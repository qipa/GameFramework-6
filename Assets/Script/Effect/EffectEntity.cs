using UnityEngine;
using System.Collections;
using System;
//特效消失的类型
public enum eDestroyType
{
    Time,       //时间型特效，赋值一个时间，时间到后消失
    Custom,     //自定义消失
}
public class EffectEntity : EntityBase {

    eDestroyType type = eDestroyType.Time;
    public float duration = 0;  //特效持续时间
    ParticleEmitter[] m_particleEmits = null;
    ParticleSystem[] m_particleSystems = null;
    public EffectEntity(string path)
    {
        UnityEngine.Object o = ResManager.Load(path);
        if( o == null)
        {
            throw new Exception ("不存在的特效 ： " + path);
            
        }

        
        m_object = GameObject.Instantiate<GameObject>(o as GameObject);
        SetParent(EffectManager.Instance.EffectRoot);
        if (m_object == null)
        {
            throw new Exception ("加载U3D对象失败  " + path);
        }

        m_particleSystems = m_object.GetComponentsInChildren<ParticleSystem>(true);
        m_particleEmits = m_object.GetComponentsInChildren<ParticleEmitter>(true);
    }

    public void Init(eDestroyType type, float duration = 0f)
    {
        this.type = type;
        SetActive(true);
        this.duration = duration;
        if (m_particleSystems != null)
        {
            foreach (ParticleSystem par in m_particleSystems)
            {
                par.Clear(true);
                par.playbackSpeed = 1f;
            }
        }

        if (m_particleEmits != null)
        {
            foreach (ParticleEmitter par in m_particleEmits)
                par.ClearParticles();
        }
    }

    //将m_object绑定到bone
    public void Bind(Transform bone)
    {
        if (IsLoaded())
        {
            SetParent(bone);
        }
    }

    public override void Update()
    {
        if (!IsLoaded() || !IsActive)
            return;

        if (type == eDestroyType.Time)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                SetActive(false);

            }
        }
    }

    public void SetActive(bool bActive)
    {
        duration = 0;
        IsActive = bActive;
//         if(!bActive)
//         {
//             SetParent(EffectManager.Instance.EffectRoot);
//         }
    }

    public void SetParent(Transform tf)
    {
        m_object.transform.SetParent(tf);
        m_object.transform.localPosition = Vector3.zero;
        m_object.transform.localScale = Vector3.one;
        m_object.transform.localRotation = Quaternion.identity;
    }
}
