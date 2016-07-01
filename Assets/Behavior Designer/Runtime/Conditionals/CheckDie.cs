using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class CheckDie : Conditional {

    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        if (owner.IsDead)
            return TaskStatus.Success;
        return TaskStatus.Failure;
    }
}
