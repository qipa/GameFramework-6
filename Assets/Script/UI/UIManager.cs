using UnityEngine;
using System.Collections;
using LuaInterface;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get { return GameManager.Instance.uiMgr; }
    }
    private Transform _uiRoot;
    Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                GameObject go = GameObject.FindWithTag("UIRoot");
                if (go != null)
                    _uiRoot = go.transform;
            }
            return _uiRoot;
        }
    }


    //创建面板的接口，  name为预设的名称  func 为lua中的回调 
    //UI界面预制体 通通放在 UIPrefab目录
    public void CreatePanel(string name,LuaFunction func = null)
    {       
        if(UIRoot.FindChild(name) != null)
        {
            Log.Error("UIRoot下已经存在面板 : " + name);
            return;
        }
        Object prefab = ResManager.Load("UIPrefab/" + name);
        if(prefab == null)
        {
            Log.Error("加载预设失败 : " + name);
            return;
        }
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.name = name;
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(UIRoot);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<LuaBehaviour>();
        if (func != null)
            func.Call(go);
        Log.Info("CreatePanel === > " + name + "  " + prefab);
    }
	
}
