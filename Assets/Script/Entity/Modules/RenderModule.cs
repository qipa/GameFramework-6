using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RenderModule : ModuleBase {

    SkinnedMeshRenderer[] m_renders = null;
    GameObject m_weapon = null;
    string weapon_path = "Model/Weapon/Prefab/";

    public static Dictionary<string, string> m_sDicWeaponSkill = new Dictionary<string, string>()
    {
        {"am_Sword","6|7|8"},{"am_Spear","2|3|4"},{"am_Bow","10|11|12"},{"am_Broadsword","14|15|16"}
    };

	public RenderModule(Entity entity) : base(entity)
    {
        m_renders = m_object.GetComponentsInChildren<SkinnedMeshRenderer>();
        ChangeWeapon("am_Sword");
    }

    public void ChangeShader(string shaderName)
    {
        Shader s = Shader.Find(shaderName);
        if(s == null)
        {
            Log.Error(string.Format("不存在名字为{0}的Shader", shaderName));
            return;
        }
        for(var i = 0; i < m_renders.Length;i++)
        {
            m_renders[i].sharedMaterial.shader = s;
        }
    }
    public override void Update()
    {
        
    }

    public void ChangeWeapon(string weapon)
    {
        Object obj = ResManager.Instance.Load(weapon_path + weapon);
        if(obj == null)
        {
            Log.Error("不存在武器 " + weapon);
            return;
        }
        if (m_weapon != null)
            GameObject.Destroy(m_weapon);

        m_weapon = GameObject.Instantiate(obj) as GameObject;
        m_weapon.transform.SetParent(m_entity.GetBone("Bip001_Weapon Point R"));
        m_weapon.transform.localPosition = Vector3.zero;
        m_weapon.transform.localScale = Vector3.one;
        m_weapon.transform.localRotation = Quaternion.identity;
        Log.Info("切换武器成功 : " + weapon);
        if (m_sDicWeaponSkill[weapon] != null)
            m_entity.Skill.SetNormalSkills(m_sDicWeaponSkill[weapon]);
    }

    public void RandomChangeWeapon()
    {
        int index = Random.Range(0, m_sDicWeaponSkill.Count + 1);
        int i = 0;
        var e = m_sDicWeaponSkill.GetEnumerator();
        while(i < index && e.MoveNext())
        {
            i++;
        }
        ChangeWeapon(e.Current.Key);
    }
}
