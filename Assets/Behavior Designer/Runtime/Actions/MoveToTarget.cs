using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
public class MoveToTarget : Action{

    public Transform target = null;
    public float speed = 0;

    public override TaskStatus OnUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - target.position) < 0.1f)
            return TaskStatus.Success;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        return TaskStatus.Running;
    }

    public override void OnReset()
    {
        target = null;
        speed = 0;
    }
}
