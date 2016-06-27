using UnityEngine;
using System.Collections;


public abstract class EntityBase  {

    public GameObject m_object = null;     //U3D对象
    private bool _isActive = false;
    public bool IsActive    //是否正在使用
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if(m_object != null)
                m_object.SetActive(value); 
        }
    }
    
    public EntityBase()
    {

    }

    public Vector3 Pos
    {
        set { m_object.transform.position = value; }
        get { return m_object.transform.position; }
    }

    public Vector3 Scale
    {
        set { m_object.transform.localScale = value; }
        get { return m_object.transform.localScale; }
    }

   

    public Vector3 Forward
    {
        set 
        { 
            m_object.transform.rotation = Quaternion.LookRotation(value); 
        }
        get { return m_object.transform.forward.normalized; }
    }

    public void SetRot(float angle)
    {
        m_object.transform.rotation = Quaternion.EulerAngles(0f, angle, 0f);
    }

    public GameObject GetGameObject() { return m_object; }
    void EntityLoadEnd(Object obj)
    {
       
    }

    protected bool IsLoaded() { return m_object != null; }


    public virtual void Update() { }
    
    public virtual void Dispose()
    {
        Log.Info(m_object.name + " 被销毁");
        GameObject.Destroy(m_object);
    }

}
