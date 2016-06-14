using System;
using System.Collections.Generic;



public class SkillInfo
{
    public uint uSkillID = 0;   //技能id
    public uint uSkillType = 0;     // 1:普通攻击  2：直线   3：扇形   4：范围
    public string strSkillName = "";        //技能名称
    public string strSkillIcon = "";        //技能图标
    public string strSkillIntro = "";       //技能简介
    public uint uSkillLevel = 0;      //技能等级  
    public float fAttackRange = 0;                                                      // 攻击范围
    public float fHitTime = 0;       //击中时间
    public float fCoolTime = 0;     //冷却时间
    public float fCostMP = 0;       //消耗MP
    public string strCastEffect = "";       //释放特效
    public string strHitEffect = "";        //击中特效
    public float fCastEffectBeginTime = 0;      //释放特效开始时间
    public string strCastEffectBindBone = "";       //释放特效绑定的骨骼
    public float fCastEffectDuration = 0;       //释放特效持续时间
    public float fFlySpeed = 0;     //飞行轨迹速度
    public string strGuideAction = "";      //指引动作
    public float fGuideActionBeginTime = 0;     //指引动作开始时间
    public float fGuideActionDuration = 0;      //指引动作持续时间
    public string strCastAction = "";       //攻击动作
    public float fCastActionBeginTime = 0;  //攻击动作开始时间
    public float fCastActionDuration = 0;       //攻击动作持续时间
    public string strSound = "";        //技能音效
    public float fComboHitBeginTime = 0;                                                // 连击接受指令开始时间
    public float fComboHitDuration = 0;                                                 // 连击接受指令持续时间
}

