using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillBase 
{

    public CSVSkill skillInfo;
    public float fCDTotalTime;
    public float fCDLeftTime;
    public bool isCasting = false;

    public bool IsInCD
    {
        get { return fCDLeftTime > 0f; }
    }
    public SkillBase(CSVSkill si)
    {
        this.skillInfo = si;
        fCDTotalTime = fCDLeftTime = si.coolTime;
    }

    public void BeginCD()
    {
        fCDLeftTime = fCDTotalTime;
    }

    public virtual void Reset(CSVSkill si)
    {
        this.skillInfo = si;
        fCDTotalTime = fCDLeftTime = si.coolTime;
    }

    public virtual void Update()
    {
        if (fCDLeftTime > 0)
        {
            fCDLeftTime -= Time.deltaTime;
            fCDLeftTime = Mathf.Max(0, fCDLeftTime);
        }
    }

    
}

