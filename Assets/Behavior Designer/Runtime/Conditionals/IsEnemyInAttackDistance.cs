using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

//敌人是否在攻击范围内
class IsEnemyInAttackDistance : Conditional
{
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        if (owner.SelectTarget != null && owner.SelectTarget.IsDead == false &&
            (owner.SelectTarget.Pos - owner.Pos).sqrMagnitude <= owner.Skill.AttackDistance * owner.Skill.AttackDistance)
        {
            return TaskStatus.Success;
        }
        else
        {
            owner.AICmd = eAICmd.Null;
        }
        return TaskStatus.Failure;
    }
}