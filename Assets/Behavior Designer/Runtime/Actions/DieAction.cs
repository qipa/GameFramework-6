using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class DieAction : Action
{
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        if (owner.Anim.GetCurAnimatorState().IsName("Die"))
            return TaskStatus.Success;

        owner.Anim.SyncAction("Die");
        return TaskStatus.Success;
    }
}
