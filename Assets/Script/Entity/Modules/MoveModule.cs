using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveModule : ModuleBase {

    bool m_isMoving = false;
     NavMeshAgent m_navAgent = null;
     NavMeshPath m_navPath = null;
//     NavMeshObstacle m_obstacle = null;
    List<Vector3> m_pathList = new List<Vector3>();
    int m_curStep = 0;
    Vector3 m_MoveDir = Vector3.zero;

    Vector3 _faceDir = Vector3.zero;

    

	public MoveModule(Entity entity) : base(entity)
    {
        
    }

    bool _isInit = false;
    void InitNavMeshComponent()
    {
        if(!_isInit)
        {
            if (!GameDefine.UseAstar)
            {
                //             if (m_obstacle == null)
                //             {
                //                 m_obstacle = m_object.AddComponent<NavMeshObstacle>();
                //                 m_obstacle.shape = NavMeshObstacleShape.Capsule;
                //                 m_obstacle.center = new Vector3(0f, 1f, 0f);
                //                 m_obstacle.radius = 1f;
                //             }
                if (m_navAgent == null)
                {
                    m_navAgent = m_object.AddComponent<NavMeshAgent>();
                }
            }
            _isInit = true;
        }
    }

    public void MoveTo(Vector3 des)
    {

        if ((m_object.transform.position - des).sqrMagnitude < 0.1f)
            return;

        //正在释放技能
        if (m_entity.Skill.m_bIsCasting)
            return;

        if (GameDefine.UseAstar)
        {

            m_pathList = AStar.FindPath(m_entity.Pos, des);
            if (m_pathList != null && m_pathList.Count > 0)
            {
                m_entity.Pos = m_pathList[0];
                m_curStep = 0;
                m_isMoving = true;
                OnMoveToNextPos();
                m_entity.Anim.SyncAction("Run");
            }
            else
            {
                m_isMoving = false;
            }
        }
        else
        {
            InitNavMeshComponent();
            //计算路径
            if (m_navPath == null)
                m_navPath = new NavMeshPath();

            m_navAgent.CalculatePath(des, m_navPath);
            if (m_navPath.corners.Length > 0)
            {
                m_pathList.Clear();
                m_pathList.AddRange(m_navPath.corners);
                m_curStep = 0;
                m_isMoving = true;
                OnMoveToNextPos();
                m_entity.Anim.SyncAction("Run");
            }
        }
    }

    bool OnMoveToNextPos()
    {
        m_curStep++;
        if (m_curStep >= m_pathList.Count)
        {
            StopMove();
            return true;    //寻路结束
        }
        m_MoveDir = m_pathList[m_curStep] - m_pathList[m_curStep - 1];
        _faceDir = m_MoveDir.normalized;
        return false;
    }

    void UpdateMoving()
    {
       
        float delta = m_entity.Property.MoveSpeed * Time.deltaTime;
        if (m_curStep >= m_pathList.Count)
            StopMove();
        Vector3 dist = m_entity.Pos - m_pathList[m_curStep];
        //m_entity.Anim.SyncAction("Run");
        //如果移动到了当前的目标点，进入下个目标点
        if(dist.sqrMagnitude <= delta * delta)
        {
            m_entity.Pos = m_pathList[m_curStep];
            OnMoveToNextPos();                
        }
        else
        {
            m_entity.Pos += delta * m_MoveDir.normalized;
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
            Util.DrawPathLine(m_pathList);
            UpdateMoving();
            UpdateRotation();
        }
        
    }

    public void StopMove(bool PlayIdle = true)
    {
        m_curStep = 0;
        m_isMoving = false;
        if (PlayIdle)
            m_entity.Anim.SyncAction("Idle_Sword");
    }

    public override void OnEvent(eEntityEvent eventID, object args = null)
    {
        if (eventID == eEntityEvent.OnAlive)
        {

        }
        else if (eventID == eEntityEvent.OnDead)
        {
            StopMove();
        }
    }
    
}
