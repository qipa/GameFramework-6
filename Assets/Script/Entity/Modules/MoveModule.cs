using UnityEngine;
using System.Collections;

public class MoveModule : ModuleBase {

    bool m_isMoving = false;
    NavMeshAgent m_navAgent = null;
    NavMeshPath m_navPath = null;
    Vector3[] m_path = null;
    int m_curStep = 0;
    Vector3 m_MoveDir = Vector3.zero;
    Quaternion m_targetRot = Quaternion.identity;

    Vector3 _faceDir = Vector3.zero;


	public MoveModule(Entity entity) : base(entity)
    {
        m_navAgent = m_object.AddComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 des)
    {
        if ((m_object.transform.position - des).sqrMagnitude < 0.1f)
            return;
        

        //计算路径
        if (m_navPath == null)
            m_navPath = new NavMeshPath();

        m_navAgent.CalculatePath(des, m_navPath);
        if (m_navPath.corners.Length > 0)
        {
            m_path = m_navPath.corners;
            m_curStep = 0;  
            m_isMoving = true;
            OnMoveToNextPos();
            m_entity.Anim.SyncAction("Run");
        }           
    }

    bool OnMoveToNextPos()
    {
        m_curStep++;
        if(m_curStep >= m_path.Length)
        {
            StopMove();
            return true;    //寻路结束
        }
        m_MoveDir = m_path[m_curStep] - m_path[m_curStep - 1];
        _faceDir = m_MoveDir.normalized;
        return false;
    }

    void UpdateMoving()
    {
       
        float delta = m_entity.Property.MoveSpeed * Time.deltaTime;
        Vector3 dist = m_object.transform.position - m_path[m_curStep];

        //如果移动到了当前的目标点，进入下个目标点
        if(dist.sqrMagnitude <= delta * delta)
        {
            m_entity.Pos = m_path[m_curStep];
            OnMoveToNextPos();                
        }
        else
        {          
            m_navAgent.Move(delta * m_MoveDir.normalized);
        }
    }

    void UpdateRotation()
    {
        if (m_entity.Forward.Equals(_faceDir))
            return;

        m_entity.Forward = Vector3.Slerp(m_entity.Forward, _faceDir, 0.5f);
    }

    public override void Update()
    {
        if(m_isMoving)
        {
            UpdateMoving();
        }
        UpdateRotation();
    }

    public void StopMove(bool isIdle = true)
    {
        m_curStep = 0;
        m_isMoving = false;
        if (isIdle)
            m_entity.Anim.SyncAction("Idle_Sword");
    }
    
}
