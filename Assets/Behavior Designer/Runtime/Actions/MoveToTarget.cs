using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToTarget : Action
{
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        if (owner.AICmd != eAICmd.Move)
            return TaskStatus.Failure;

        if (owner.SelectTarget != null)
        {
            if ((owner.Pos - owner.SelectTarget.Pos).sqrMagnitude <= owner.Skill.AttackDistance * owner.Skill.AttackDistance)
            {
                return TaskStatus.Success;
            }
            owner.Move.MoveTo(owner.SelectTarget.Pos);
        }
        else
            return TaskStatus.Failure;

        return TaskStatus.Running;
    }
}
