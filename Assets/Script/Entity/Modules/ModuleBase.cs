using UnityEngine;
using System.Collections;

public class ModuleBase  {
    protected Entity m_entity = null;
    protected GameObject m_object = null;
    public ModuleBase(Entity entity)
    {
        m_entity = entity;
        m_object = m_entity.GetGameObject();
    }

    public virtual void Update() { }

    //entity的事件，如果模块需要关注某个事件，需要重载此虚函数
    public virtual void OnEvent(eEntityEvent eventID, object args = null) { }
}
