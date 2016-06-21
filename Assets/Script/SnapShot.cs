using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SnapShotRecord
{
    public List<GameObject> objs;   // maybe have body and weapon  ,maybe only have body,it depends on  how many SkinnedMeshRenderers
    public float disTime;     // when the objects should be destroyed
}
public class SnapShot : MonoBehaviour {

    public float m_LiveTime = 0.3f;
    public float m_BirthInterval = 0.3f;

    private List<SnapShotRecord> m_SnapShotList = new List<SnapShotRecord>();
    private SkinnedMeshRenderer [] m_SkinnedRenders;
    private float m_LastBirth = 0.0f;
    public bool m_CanBirth = true;

 // Use this for initialization
 void Start () {
        m_SkinnedRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (m_SkinnedRenders.Length == 0)
            DestroyImmediate(this);
 }
 
 // Update is called once per frame
 void Update () {
     if(Input.GetKey(KeyCode.V))
        {
            m_CanBirth = !m_CanBirth;
        }
        if(m_CanBirth)
        {
            if(Time.time > (m_LastBirth + m_BirthInterval))
            {
                m_LastBirth = Time.time;
                DoSnapShot();
            }
        }
        DestroyUnLivedSnapShot();
 }

    void DoSnapShot()
    {
        SnapShotRecord record = new SnapShotRecord();
        record.objs = new List<GameObject>();
        for (int i = 0; i < m_SkinnedRenders.Length; i++)
        {
            //copy transtrom
            GameObject obj = new GameObject();

            obj.transform.localPosition = m_SkinnedRenders[i].transform.localPosition;
            obj.transform.localRotation = m_SkinnedRenders[i].transform.localRotation;
            obj.transform.localScale = m_SkinnedRenders[i].transform.localScale;
            obj.transform.position = m_SkinnedRenders[i].transform.position;
            obj.transform.rotation = m_SkinnedRenders[i].transform.rotation;

            //copy mesh and material
            MeshFilter mesh = obj.AddComponent<MeshFilter>();
            MeshRenderer ren = obj.AddComponent<MeshRenderer>();

            m_SkinnedRenders[i].BakeMesh(mesh.mesh);
            ren.sharedMaterial = m_SkinnedRenders[i].sharedMaterial;
            ren.sharedMaterial.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            ren.sharedMaterial.SetColor("_MainColor", new Color(255, 255, 255, 0.5f));
            record.objs.Add(obj);
        }
        record.disTime = Time.time + m_LiveTime;
        m_SnapShotList.Add(record);
    }

    void DestroyUnLivedSnapShot()
    {
        if(m_SnapShotList.Count > 0)
        {
            if(m_SnapShotList[0].disTime < Time.time)
            {
                for (int i = 0; i < m_SnapShotList[0].objs.Count; i++)
                {
                    MeshFilter mesh = m_SnapShotList[0].objs[i].GetComponent<MeshFilter>();
                    if (mesh != null)
                    {
                        Destroy(mesh.mesh);
                    }
                    Destroy(m_SnapShotList[0].objs[i]);
                }
                m_SnapShotList.RemoveAt(0);
            }
        }
    }
}
