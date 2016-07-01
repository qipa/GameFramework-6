using UnityEngine;
using System.Collections;

public class HUDModule : ModuleBase {

    public HUDModule(Entity entity)
        : base(entity)
    {

    }

    public override void Update()
    {
        
    }

    public override void OnEvent(eEntityEvent eventID, object args = null)
    {
        if (eventID == eEntityEvent.OnAlive)
        {

        }
        else if (eventID == eEntityEvent.OnDead)
        {

        }
    }
}
