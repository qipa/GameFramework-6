using System;
using System.Collections.Generic;
using UnityEngine;

public enum eEntityEvent
{
    OnAlive,
    OnDead,
    OnSkillResult,      //模拟服务器技能结果，便于单机和pvp的切换
    OnChangeWeapon,     //换武器
}

public enum eCamp
{
    Hero,
    Friend,
    Enemy,
    Boss,
}

public class Entity : EntityBase
{
    Dictionary<string, Transform> m_mapBones = new Dictionary<string, Transform>();
    public AnimationModule Anim{ get;private set;}
    public MoveModule Move  { get;private set;}
    public SkillModule Skill  { get;private set;}
    public HUDModule Hud  { get;private set;}
    public PropertyModule Property  { get;private set;}

    public BuffModule Buff { get; private set; }

    public RenderModule Render { get; private set; }

    List<ModuleBase> _moduleList = null;

    public Entity SelectTarget = null;

    public ulong UID { get; private set; }

    public CSVEntity EntityCfg { get; private set; }

    public eCamp Camp = eCamp.Enemy;

    public bool IsMainPlayer
    {
        get { return Camp == eCamp.Hero; }
    }

    //此构造函数主要用于加载角色
    //param : 角色配置表的key值
    public Entity(uint configID,ulong uID) 
    {
        //注意顺序，首先应该加载 gameobject，然后实例化其他模块，因为其他模块的构造会依赖于这个gameobject
        EntityCfg = CSVManager.GetEntityCfg(configID);
        if(EntityCfg == null)
        {
            throw new Exception ("找不到 configID = " + configID + " 的配置");
        }

        GameObject o = ResManager.Load<GameObject>(EntityCfg.ResPath);
        if(o == null)
        {
            throw new Exception("不存在的entity ： " + EntityCfg.ResPath);
        }
        m_object = GameObject.Instantiate(o);
        m_object.transform.SetParent(EntityManager.Instance.EntityRoot);
        UID = uID;
        FillMapBones(); //映射骨骼信息
        InitModules();
        
    }
    void InitModules()
    {      
        Anim = new AnimationModule(this);   //动作控制模块        
        Move = new MoveModule(this);        //移动模块     
        Skill = new SkillModule(this);      //技能模块    

        Hud = new HUDModule(this);          //角色头顶信息模块      
        Property = new PropertyModule(this);    //属性模块
        Buff = new BuffModule(this);            //Buff 模块       
        Render = new RenderModule(this);      //渲染模块    
   
        _moduleList = new List<ModuleBase>()
        { 
            Anim, 
            Move,
            Skill,
            Hud,
            Property,
            Buff,
            Render
        };
    }

    void FillMapBones()
    {
        Transform[] bones = m_object.GetComponentsInChildren<Transform>(true);
        for (var i = 0; i < bones.Length; ++i)
        {
            Transform bone = bones[i];
            if (m_mapBones.ContainsKey(bone.gameObject.name))
            {
                Log.Warning(bone.gameObject.name + "骨骼名字重复");
                continue;
            }
            m_mapBones.Add(bone.gameObject.name, bone);
        }
    }
    
    

    void UpdateModules()
    {
        for (var i = 0; i < _moduleList.Count; ++i)
            _moduleList[i].Update();
    }

    public override void Update()
    {
        UpdateModules();
    }

    public Transform GetBone(string boneName)
    {
        Transform tf = null;
        m_mapBones.TryGetValue(boneName, out tf);
        return tf;        
    }

    public void OnEvent(eEntityEvent eventID, object args = null)
    {
        for (var i = 0; i < _moduleList.Count; ++i)
            _moduleList[i].OnEvent(eventID, args);
    }
    

}

