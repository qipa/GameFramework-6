using System;
using System.Collections.Generic;
public class PriorityQueue<TKey,TValue>
{
    private SortedDictionary<TKey, Queue<TValue>> sd = new SortedDictionary<TKey, Queue<TValue>>();

    public void Push(TKey k, TValue v)
    {
        Queue<TValue> q;
        if(!sd.TryGetValue(k,out q))
        {
            q = new Queue<TValue>();
            sd.Add(k,q);
        }
        q.Enqueue(v);
    }

    public TValue Pop()
    {
        TValue ret = default(TValue);
        var e = sd.GetEnumerator();
        if(e.MoveNext())
        {
            ret = e.Current.Value.Dequeue();
            if (e.Current.Value.Count <= 0)
                sd.Remove(e.Current.Key);
        }

        return ret;
    }

    public void Clear()
    {
        sd.Clear();
    }
}
