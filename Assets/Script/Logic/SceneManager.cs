using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

    public static SceneManager Instance
    {
        get { return GameManager.Instance.sceneMgr; }
    }
    public string CurSceneName
    {
        private set;
        get;
    }

    public CSVMap CurMapInfo
    {
        private set;
        get;
    }

    public void LoadScene(string name,System.Action callback = null)
    {
        if (CurSceneName == "101")
            CurSceneName = "Main";
        else if (CurSceneName == "Main")
            CurSceneName = "101";
        else
            CurSceneName = name;
        StartCoroutine(LoadSync(callback));
    }

    IEnumerator LoadSync(System.Action callback)
    {
        GC();
        if(CameraController.Instance != null)
            CameraController.Instance.LookTarget = null;
        yield return Application.LoadLevelAsync(CurSceneName);

        CurMapInfo = CSVManager.GetMapCfg(CurSceneName);
        if(CurMapInfo == null)
        {
            Log.Error("找不到地图配置 : " + CurSceneName);
            yield break;
        }
        AStar.LoadPathInfo(CurMapInfo.pathInfo);
        char[] sp = { '*' };
        string[] str = CurMapInfo.bornPos.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);

        if (str.Length < 4)
        {
            Log.Error("检查Map.csv   " + name);
            yield break;
        }
        float x = System.Convert.ToSingle(str[0]);
        float y = System.Convert.ToSingle(str[1]);
        float z = System.Convert.ToSingle(str[2]);
        float angle = System.Convert.ToSingle(str[3]);

        GameManager.MainPlayer = EntityManager.Instance.Get(10, 1);
        GameManager.MainPlayer.Camp = eCamp.Hero;
        GameManager.MainPlayer.Pos = new Vector3(x, y, z);
        GameManager.MainPlayer.SetRot(angle);

        CameraController.Instance.LookTarget = GameManager.MainPlayer;

        if (callback != null)
            callback();
    }

    void GC()
    {
        EntityManager.Instance.Clear();
        BulletManager.Instance.Clear();
        EffectManager.Instance.Clear();
        System.GC.Collect();
    }
}
