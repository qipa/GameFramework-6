using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BuffModule : ModuleBase
{
    Dictionary<int, BuffBase> m_dicBuff = null;
    List<int> m_removeBuffList = null;
    public BuffModule(Entity entity)
        : base(entity)
    {
        m_dicBuff = new Dictionary<int, BuffBase>();
        m_removeBuffList = new List<int>();
    }


    public override void Update()
    {
        var e = m_dicBuff.GetEnumerator();
        while(e.MoveNext())
        {
            KeyValuePair<int, BuffBase> pair = e.Current;
            pair.Value.BuffUpdate();
            if(pair.Value.leftTime > 0)
            {
                pair.Value.leftTime -= Time.deltaTime;
                pair.Value.leftTime = Mathf.Max(0, pair.Value.leftTime);
            }
            else
            {
                pair.Value.BuffEnd();
                m_removeBuffList.Add(pair.Key);
            }
        }

        for(var i = 0 ; i < m_removeBuffList.Count;++i)
        {
            m_dicBuff.Remove(m_removeBuffList[i]);
        }
        m_removeBuffList.Clear();
    }

    public void AddBuff(int buffID)
    {
        BuffBase buff = BuffManager.CreateBuff(buffID,m_entity);
        if(buff == null)
        {
            Log.Error("找不到buffID = " + buffID);
            return;
        }

        if(m_dicBuff.ContainsKey(buffID))
        {
            m_dicBuff[buffID].leftTime = buff.buffInfo.KeepTime;
        }
        else
        {
            buff.leftTime = buff.buffInfo.KeepTime;
            buff.BuffStart();
            m_dicBuff.Add(buffID, buff);
        }
        
    }

    public void RemoveBuff(int buffID)
    {
        BuffBase buff = null;
        if (!m_dicBuff.TryGetValue(buffID,out buff))
            return;
        if (buff == null)
            return;
        buff.BuffEnd();
        m_dicBuff.Remove(buffID);
    }

    public void RemoveAllBuff()
    {
        m_removeBuffList.Clear();
        var e = m_dicBuff.GetEnumerator();
        while(e.MoveNext())
        {
            m_removeBuffList.Add(e.Current.Key);
        }

        var re = m_removeBuffList.GetEnumerator();
        while(re.MoveNext())
        {
            RemoveBuff(re.Current);
        }
    }


    public BuffBase GetBuff(int buffID)
    {
        BuffBase ret = null;
        m_dicBuff.TryGetValue(buffID, out ret);
        return ret;
    }
}
