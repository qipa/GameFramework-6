using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
public class BuildTools 
{
    static List<string> pathList = new List<string>();
    static List<string> fileList = new List<string>();
    static List<AssetBundleBuild> abBuildList = new List<AssetBundleBuild>();


    [MenuItem("Build/Build PC Resources")]
    public static void BuildPCResources()
    {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Build Android Resources")]
    public static void BuildAndroidResources()
    {
        BuildAssetResource(BuildTarget.Android);
    }

    [MenuItem("Build/Build iOS Resources")]
    public static void BuildIOSResources()
    {
        BuildAssetResource(BuildTarget.iOS);
    }


    public static void BuildAssetResource(BuildTarget target)
    {
        //沙盒目录
        if(Directory.Exists(Util.DataPath))
        {
            Directory.Delete(Util.DataPath,true);
        }

        string streamPath = Application.streamingAssetsPath;
        if (Directory.Exists(streamPath))
        {
            Directory.Delete(streamPath, true);
        }
        Directory.CreateDirectory(streamPath);
        AssetDatabase.Refresh();

        abBuildList.Clear();
        //打包
        BuildLua();
        BuildRes();
        BuildConfig();
        BuildPathInfo();
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.UncompressedAssetBundle;
        BuildPipeline.BuildAssetBundles(streamPath, abBuildList.ToArray(), options, target);
        GenMD5File();

        //删除lua临时文件
        if (File.Exists(Application.dataPath + "/TmpLua.meta"))
            File.Delete(Application.dataPath + "/TmpLua.meta");

        string tmpLuaPath = Application.dataPath + "/TmpLua/";
        if (Directory.Exists(tmpLuaPath))
            Directory.Delete(tmpLuaPath,true);
        AssetDatabase.Refresh();
        Debug.Log("打包完成");
    }

    static void BuildConfig()
    {
        string cfgPath = Application.dataPath + "/Res/Config/";
        string[] files = Directory.GetFiles(cfgPath, "*.csv");

        for(var i = 0; i < files.Length;i++)
        {
            files[i] = "Assets"+ files[i].Replace(Application.dataPath, string.Empty);
        }

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "Config/Config.unity3d";
        build.assetNames = files;
        abBuildList.Add(build);
        Debug.Log("abName : Config/Config.unity3d   资源路径 : Assets/Res/Config");
    }

    static void BuildPathInfo()
    {
        string cfgPath = Application.dataPath + "/Res/PathInfo/";
        string[] files = Directory.GetFiles(cfgPath, "*.bytes");

        for (var i = 0; i < files.Length; i++)
        {
            files[i] = "Assets" + files[i].Replace(Application.dataPath, string.Empty);
        }

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "PathInfo/PathInfo.unity3d";
        build.assetNames = files;
        abBuildList.Add(build);
        Debug.Log("abName : PathInfo/PathInfo.unity3d   资源路径 : Assets/Res/PathInfo");
    }


    static void BuildRes()
    {
        string resPath = Application.dataPath + "/Res/";
        string[] files = Directory.GetFiles(resPath, "*.prefab", SearchOption.AllDirectories);
        for(var i = 0; i < files.Length;i++)
        {
            if (files[i].Contains("SkillEditor"))
                continue;

            files[i] = files[i].Replace('\\', '/');

            //得到相对 Res/ 的目录  
            string relativePath = files[i].Replace(resPath, string.Empty);

            //prefab 相对Assets的完整路径，包括后缀名
            string path = "Assets/Res/" + relativePath;

            //去掉 .prefab后缀
            string bundleName = relativePath.Replace(".prefab", string.Empty) + ".unity3d";

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = new string[] { path };
            abBuildList.Add(build);
            Debug.Log("abName : " + bundleName + "   资源路径 : " + path);
        }
    }


    static void BuildLua()
    {
        string tmpLuaPath = Application.dataPath + "/TmpLua/";
        if (!Directory.Exists(tmpLuaPath))
            Directory.CreateDirectory(tmpLuaPath);

        //将需要打包的lua文件 拷贝到临时目录，并添加.bytes后缀
        string[] LuaDirs = { CustomSettings.luaDir, CustomSettings.toluaLuaDir };
        foreach (string dir in LuaDirs)
        {
            CopyLuaFiles(dir, tmpLuaPath);
        }

        //从临时lua目录找到lua文件并打包
        string[] dirs = Directory.GetDirectories(tmpLuaPath, "*", SearchOption.AllDirectories);
        foreach(string dir in dirs)
        {
            string name = dir.Replace(tmpLuaPath, string.Empty);
            name = name.Replace('\\', '_').Replace('/', '_');
            name = "lua/lua_" + name.ToLower() + ".unity3d";
            string path = "Assets" + dir.Replace(Application.dataPath, string.Empty);
            AddBuildList(name, "*.bytes", path);
        }
        AddBuildList("lua/lua.unity3d", "*.bytes", "Assets/TmpLua");
        AssetDatabase.Refresh();
    }

    static void AddBuildList(string bundleName,string pattern,string path)
    {
        string[] files = Directory.GetFiles(path, pattern);
        if (files.Length == 0) return;
        for (var i = 0; i < files.Length; i++)
            files[i] = files[i].Replace('\\', '/');
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;
        build.assetNames = files;
        abBuildList.Add(build);
        Debug.Log("abName : " + bundleName + "   资源路径 : " + path);
    }

    static void CopyLuaFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(sourceDir))
        {
            return;
        }

        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
        int len = sourceDir.Length;

        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
        {
            --len;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string str = files[i].Remove(0, len);
            string dest = destDir + "/" + str;
            if (appendext) dest += ".bytes";
            string dir = Path.GetDirectoryName(dest);
            Directory.CreateDirectory(dir);
            File.Copy(files[i], dest, true);
        }
    }

    static void GenMD5File()
    {
        string md5File = Application.streamingAssetsPath + "/files.txt";
        if (File.Exists(md5File))
            File.Delete(md5File);

        pathList.Clear();
        fileList.Clear();
        Recursive(Application.streamingAssetsPath);

        FileStream fs = new FileStream(md5File, FileMode.CreateNew);
        using(StreamWriter sw = new StreamWriter(fs))
        {
            foreach(string file in fileList)
            {
                if (file.EndsWith(".meta"))
                    continue;

                string md5 = Util.md5file(file);
                string value = file.Replace(Application.streamingAssetsPath+"/", string.Empty);
                sw.WriteLine(value + "|" + md5);
            }
        }
        fs.Close();
    }

    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            fileList.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            pathList.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}
