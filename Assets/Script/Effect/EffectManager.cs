using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class EffectManager : Singleton<EffectManager> {

    const string path = "Effect/Prefabs/";
    //特效的缓存
    Dictionary<string, List<EffectEntity>> m_EffectCaches = new Dictionary<string, List<EffectEntity>>();

  

    GameObject root = null;
    public Transform EffectRoot
    {
        get 
        {
            if (root == null)
                root = new GameObject("EffectRoot");
            return root.transform; 
        }
    }


    public EffectEntity GetEffect(string effectName)
    {
        EffectEntity effect = null;
        List<EffectEntity> list = null;

        bool find = false;
        if (m_EffectCaches.TryGetValue(effectName, out list))
        {
            //查找对象池中是否有没用到的特效对象
            for (var i = 0; i < list.Count;++i )
            {
                EffectEntity ef = list[i];
                if (!ef.IsActive)
                {
                    ef.IsActive = true;
                    effect = ef;
                    find = true;
                }
            }

            //没有找到可用的
            if (!find)
            {
                effect = new EffectEntity(path + effectName);
                list.Add(effect);
            }
        }
        else
        {
            list = new List<EffectEntity>();
            effect = new EffectEntity(path + effectName);

            list.Add(effect);
            m_EffectCaches.Add(effectName, list);
        }
        return effect;
    }

   
    public void Clear()
    {
        var e = m_EffectCaches.GetEnumerator();
        while(e.MoveNext())
        {
            List<EffectEntity> list = e.Current.Value;
            var e1 = list.GetEnumerator();
            while(e1.MoveNext())
            {
                e1.Current.Dispose();
            }
        }
        m_EffectCaches.Clear();
    }

    public void Update()
    {
        var e = m_EffectCaches.GetEnumerator();
        while(e.MoveNext())
        {
            List<EffectEntity> list = e.Current.Value;
            var e1 = list.GetEnumerator();
            while (e1.MoveNext())
            {
                if (e1.Current.IsActive)
                    e1.Current.Update();
            }
        }
    }

}
