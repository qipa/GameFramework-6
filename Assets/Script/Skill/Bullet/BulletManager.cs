using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletManager : Singleton<BulletManager> 
{
    //子弹池
    List<Bullet> m_bulletPool = new List<Bullet>();

    public Bullet Get()
    {
        Bullet bullet = null;
        for(int i = 0; i < m_bulletPool.Count;i++)
        {
            if(m_bulletPool[i] != null && !m_bulletPool[i].IsUsing)
            {
                bullet = m_bulletPool[i];
                break;
            }
        }
        if(bullet == null)
        {
            bullet = new Bullet();
            m_bulletPool.Add(bullet);
        }
        return bullet;
    }

    public void Update()
    {
        for(int i = 0; i < m_bulletPool.Count;i++)
        {
            Bullet bullet = m_bulletPool[i];
            if (bullet != null && bullet.IsUsing)
                bullet.Update();
        }
    }

    public void Clear()
    {
        m_bulletPool.Clear();
    }
}
