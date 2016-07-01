using UnityEngine;
using System.Collections;

public class PropertyModule : ModuleBase {
    public float MoveSpeed = 5;
    public float AttackSpeed = 5;
    public float PhysicDamage = 100;
    public float PhysicDefence = 50;
    public float MagicDamage = 100;
    public float MagicDefence = 50;
    public float HP = 1000;
    public float CritRate = 0.8f;       //暴击率
    public float AvoidCritRate = 0.2f;   //免暴击率
	public PropertyModule(Entity entity) : base(entity)
    {
        Init();
    }

    public override void Update()
    {
        
    }

    public override void Init()
    {
         MoveSpeed = 5;
         AttackSpeed = 5;
         PhysicDamage = 100;
         PhysicDefence = 50;
         MagicDamage = 100;
         MagicDefence = 50;
         HP = 1000;
         CritRate = 0.8f;       //暴击率
         AvoidCritRate = 0.2f;   //免暴击率
    }

    public override void OnEvent(eEntityEvent eventID, object args = null)
    {
        if(eventID == eEntityEvent.OnAlive)
        {
            Init();
        }
        else if(eventID == eEntityEvent.OnDead)
        {

        }
    }
}
