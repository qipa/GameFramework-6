using UnityEngine;
using System.Collections;

public class PropertyModule : ModuleBase {
    public float MoveSpeed = 5;
    public float AttackSpeed = 5;
    public float PhysicDamage = 100;
    public float PhysicDefence = 50;
    public float MagicDamage = 100;
    public float MagicDefence = 50;
	public PropertyModule(Entity entity) : base(entity)
    {

    }

    public override void Update()
    {
        
    }
}
