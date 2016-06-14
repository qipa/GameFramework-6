using UnityEngine;
using System.Collections;

public class BuffBase
{
    public CSVBuff buffInfo;    //buff的配置信息
    public Entity buffOwner;    //buff的拥有者
    public float leftTime;
    EffectEntity effect = null;
    public BuffBase(int buffID,Entity buffOwner)
    {
        buffInfo = CSVManager.GetBuffCfg(buffID);
        this.buffOwner = buffOwner;
    }
	
    //buff 开始执行
    public virtual void BuffStart()
    {
        if(!string.IsNullOrEmpty( buffInfo.EffectName) )
        {
            effect = EffectManager.Instance.GetEffect(buffInfo.EffectName);
            effect.duration = buffInfo.KeepTime;
            if(effect != null)
            {
                Transform bone = buffOwner.GetBone(buffInfo.BindBone);
                if(bone != null)
                {
                    effect.Bind(bone);
                }
            }
        }
    }

    //buff 更新
    public virtual void BuffUpdate()
    {

    }

    //buff 结束
    public virtual void BuffEnd()
    {
        //隐藏特效
        if(effect != null)
        {
            effect.SetActive(false);
        }
    }
}
