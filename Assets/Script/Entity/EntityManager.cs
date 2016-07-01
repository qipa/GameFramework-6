using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    //正在使用的Entity池  key : uid  value : Entity
    public  Dictionary<ulong, Entity> m_dicObject = new Dictionary<ulong, Entity>();
    
    // key : configID   
    public Dictionary<uint, HashSet<Entity>> m_dicObjectPool = new Dictionary<uint, HashSet<Entity>>();

    public List<Entity> m_deavtiveList = new List<Entity>();

    GameObject root = null;
    public Transform EntityRoot
    {
        get
        {
            if (root == null)
                root = new GameObject("EntityRoot");
            return root.transform;
        }
    }
    public  Entity Get(uint configID,ulong uID)
    {
        if(!m_dicObject.ContainsKey(uID))
        {
            Entity ent = GetFromPool(configID, uID);
            ent.IsActive = true;
            m_dicObject.Add(uID, ent);
            return ent;
        }
        return m_dicObject[uID];
    }

    private Entity GetFromPool(uint configID,ulong uID)
    {
        HashSet<Entity> hash = null;
        if(!m_dicObjectPool.TryGetValue(configID,out hash))
        {
            hash = new HashSet<Entity>();
            m_dicObjectPool.Add(configID, hash);
        }
        Entity ent = null;
        var e = hash.GetEnumerator();
        if(e.MoveNext())
        {
            ent = e.Current;
            ent.UID = uID;
            ent.Alive();
            hash.Remove(ent);
        }
        else
        {
            ent = new Entity(configID, uID);
        }
        return ent;
    }
    public Entity Find(ulong uID)
    {
        Entity ent = null;
        m_dicObject.TryGetValue(uID, out ent);
        return ent;
    }


    private void PushToPool(Entity ent)
    {
        HashSet<Entity> hash = null;
        if(!m_dicObjectPool.TryGetValue(ent.EntityCfg.ID,out hash))
        {
            hash = new HashSet<Entity>();            
            m_dicObjectPool.Add(ent.EntityCfg.ID, hash);

        }
        
        if(!hash.Contains(ent))
            hash.Add(ent);
    }

    public void Update()
    {
        var e = m_dicObject.GetEnumerator();
        while(e.MoveNext())
        {
            Entity ent = e.Current.Value;
            if (ent.IsDead)
            {
                if (Time.time - ent.DeadTime > 2f)
                    m_deavtiveList.Add(ent);            
                continue;
            }

            ent.Update();
        }

        if (m_deavtiveList.Count > 0)
        {
            for (int i = 0; i < m_deavtiveList.Count; i++)
            {
                m_deavtiveList[i].IsActive = false;
                m_dicObject.Remove(m_deavtiveList[i].UID);
                PushToPool(m_deavtiveList[i]);
            }
            m_deavtiveList.Clear();
        }
    }

    public void Clear()
    {
        var e = m_dicObject.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.Value.Dispose();
        }

        var e1 = m_dicObjectPool.GetEnumerator();
        while(e1.MoveNext())
        {
            var e2 = e1.Current.Value.GetEnumerator();
            while(e2.MoveNext())
            {
                e2.Current.Dispose();
            }
        }

        m_dicObject.Clear();
        m_dicObjectPool.Clear();
    }
}