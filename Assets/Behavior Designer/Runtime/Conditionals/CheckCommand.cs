using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class CheckCommand : Conditional
{
    public eAICmd cmd;
    Entity owner;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }
    public override TaskStatus OnUpdate()
    {
        if (owner.AICmd == cmd)
            return TaskStatus.Success;
        return TaskStatus.Failure;
    }
}
