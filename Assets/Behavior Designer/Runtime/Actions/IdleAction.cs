using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class IdleAction : Action
{
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        owner.Anim.SyncAction("Idle_Sword");
        return TaskStatus.Success;
    }
}
