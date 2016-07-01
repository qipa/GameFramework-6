using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

//敌人是否在警戒范围内
class IsEnemyInWarningRange : Conditional
{
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        Entity ent = SkillProcesser.GetTargetInDistance(owner, owner.EntityCfg.WarninngRange);
        if (ent != null)
        {
            if (owner.AICmd != eAICmd.Attack)
                owner.AICmd = eAICmd.Move;
            owner.SelectTarget = ent;
            return TaskStatus.Success;
        }
        else
        {
            owner.AICmd = eAICmd.Idle;
        }
        return TaskStatus.Failure;
    }
}

