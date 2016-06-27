using UnityEngine;
using System.IO;
using System.Collections;
using System;
//游戏的入口，不可销毁
public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    [HideInInspector]
    public LuaManager luaMgr = null;
    [HideInInspector]
    public NetworkManager netMgr = null;
    [HideInInspector]
    public ResManager resMgr = null;
    [HideInInspector]
    public UIManager uiMgr = null;
    [HideInInspector]
    public SceneManager sceneMgr = null;

    public static Entity MainPlayer = null;

    public uint HeroConfigID = 10;

    void Awake()
    {
        Application.targetFrameRate = 45;
        Instance = this;
        DontDestroyOnLoad(gameObject);   
        luaMgr = gameObject.AddComponent<LuaManager>();
        netMgr = gameObject.AddComponent<NetworkManager>();
        resMgr = gameObject.AddComponent<ResManager>();
        uiMgr = gameObject.AddComponent<UIManager>();
        sceneMgr = gameObject.AddComponent<SceneManager>();
    }
	// Use this for initialization
	void Start () 
    {
        if (GameDefine.UseBundle)
            Log.Info("<color=#00ff00ff>资源加载模式为 AssetBundle</color>");
        else
            Log.Info("<color=#00ff00ff>资源加载模式为 LoadMainAssetAtPath</color>");
        ExtractResources();

       
        /*AStar.LoadPathInfo("PathInfo/main");*/
        //luaMgr.InitStart();

        
//         MainPlayer = EntityManager.Instance.Get(HeroConfigID, 1);
//         MainPlayer.Camp = eCamp.Hero;
// 
//         Entity enemy = EntityManager.Instance.Get(10, 2);
//         enemy.Pos = new Vector3(25f, 4.6f, 42f);
//         enemy.Camp = eCamp.Enemy;
// 
//         MainPlayer.Pos = new Vector3(28.5f, 4.6f, 45f);
//         CameraController.Instance.LookTarget = MainPlayer;
        //MainPlayer.AttachCamera();

        //UIManager.Instance.CreatePanel("FightPanel");
	}
	
	// Update is called once per frame
	void Update ()
    {
        EntityManager.Instance.Update();
        EffectManager.Instance.Update();
        BulletManager.Instance.Update();

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            MainPlayer.Skill.CastSkill(0);
        }
	}

    void OnApplicationQuit()
    {
        CSVManager.ClearAllCsv();
        EntityManager.Instance.Clear();
        EffectManager.Instance.Clear();
        BulletManager.Instance.Clear();
        Log.Info("application quit.");
        Log.Stop();
    }


    //拷贝资源到沙盒目录
    void ExtractResources()
    {
        bool isExists = Directory.Exists(Util.DataPath) && 
            Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
        if(isExists)
        {
            //检查更新
            StartCoroutine(UpdateResource());
        }
        else
        {
            //释放资源到沙盒目录
            StartCoroutine(DoExtract());
        }
    }


    IEnumerator DoExtract()
    {
        string dataPath = Util.DataPath;
        string resPath = Util.AppContentPath();

        if (Directory.Exists(dataPath))
            Directory.Delete(dataPath,true);
        Directory.CreateDirectory(dataPath);

        string infile = resPath + "files.txt";
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile))
            File.Delete(outfile);

        string msg = "正在解包文件 : files.txt";
        Log.Info(msg);
        //拷贝files.txt到沙盒目录
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;
            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else
            File.Copy(infile, outfile, true);

        //释放资源到沙盒目录
        string[] files = File.ReadAllLines(outfile);
        for(var i = 0; i < files.Length;i++)
        {
            string[] fs = files[i].Split('|');
            infile = resPath + fs[0];
            outfile = dataPath + fs[0];
            msg = "正在解包文件: " + fs[0];
            Log.Info(msg);
            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if(Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;
                if(www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile)) File.Delete(outfile);
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        Log.Info("解包完成!");
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(UpdateResource());
     
    }

    IEnumerator UpdateResource()
    {
        if (!GameDefine.UpdateRes)
        {
            ResourceReady();
            yield break;
        }

        string dataPath = Util.DataPath;
        string url = GameDefine.WebUrl;
        string msg = string.Empty;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string listUrl = url + "files.txt?v=" + random;
        msg = "正在更新：" + listUrl;
        Log.Info(msg);
        WWW www = new WWW(listUrl);
        yield return www;
        if(www.error != null)
        {
            msg = "更新失败 : " + listUrl;
            Log.Error(msg);
            yield break;
        }
        if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
        File.WriteAllBytes(dataPath + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');

        for(int i = 0; i < files.Length;i++)
        {
            if (string.IsNullOrEmpty(files[i]))
                continue;
            string[] fs = files[i].Split('|');
            string localFile = (dataPath + fs[0]).Trim();
            string path = Path.GetDirectoryName(localFile);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string fileUrl = url + fs[0] + "?v=" + random;
            bool canUpdate = !File.Exists(localFile);
            if(!canUpdate)
            {
                string remoteMd5 = fs[1];
                string localMd5 = Util.md5file(localFile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate) File.Delete(localFile);
            }
            if(canUpdate)
            {
                msg = "正在下载 : " + fileUrl;
                Log.Info(msg);
                www = new WWW(fileUrl);
                yield return www;
                if(www.error != null)
                {
                    msg = "下载失败 ：" + fileUrl;
                    Log.Error(msg);
                    yield break;
                }
                File.WriteAllBytes(localFile, www.bytes);
                yield return new WaitForEndOfFrame();
            } 
        }
        yield return new WaitForEndOfFrame();
        msg = "更新完成";
        Log.Info(msg);
        ResourceReady();
        yield break;
    }


    void ResourceReady()
    {
        ResManager.Init();
        CSVManager.LoadAllCsv();
        sceneMgr.LoadScene("Main", () => { luaMgr.InitStart(); });
    }
}
