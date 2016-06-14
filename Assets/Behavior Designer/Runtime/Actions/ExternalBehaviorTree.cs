using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    // Wrapper class for the External Behavior task. The external behavior tree task allows you to run another behavior tree within the current behavior tree. 
    // One use for this is that if you have a unit that plays a series of tasks to attack. You may want the unit to attack at different points within the 
    // behavior tree and you want that attack to always be the same. Instead of copying and pasting the same tasks over and over you can just use an 
    // external behavior and then the tasks are always guaranteed to be the same. This example is demonstrated in the RTS sample project located 
    // at http://www.opsive.com/assets/BehaviorDesigner/samples.php.
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=15")]
    [TaskIcon("ExternalBehaviorTreeIcon.png")]
    public class ExternalBehaviorTree : ExternalBehavior
    {
        // intentionally left blank - subclass of ExternalBehavior
        //
        // From ExternalBehavior:
        // 
        // External task that this task should reference. External task must be relative to the Resources folder otherwise it won't load.
        // public GameObject externalTask;
    }
}