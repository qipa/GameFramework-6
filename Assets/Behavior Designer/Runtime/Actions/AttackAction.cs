using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class AttackAction : Action
{
    Entity owner;
    float waitTime = 0.5f;
    public override void OnAwake()
    {
        owner = GetComponent<EntityAI>().entity;
    }

    int skillIndex = 0;
    public override TaskStatus OnUpdate()
    {
        if(owner.SelectTarget != null && !owner.SelectTarget.IsDead)
        {
            owner.AICmd = eAICmd.Attack;
            
            owner.Skill.CastSkill(skillIndex++);
            if (skillIndex > 3)
                skillIndex = 0;
        }
       
        return TaskStatus.Success;
    }
}
