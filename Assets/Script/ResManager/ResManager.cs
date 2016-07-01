using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Text;
using System.IO;
public class ResManager : MonoBehaviour 
{
    static string[] m_variants = { };
    static AssetBundleManifest manifest;
    static AssetBundle assetbundle;
    static Dictionary<string, AssetBundle> bundles;
    static StringBuilder sb = new StringBuilder();
    public static T Load<T>(string assetName,string ext = ".prefab",string abName = "") where T : UnityEngine.Object
    {
        sb.Clear();
        // UseBundle为false仅限于在PC端开发时使用，旨在开发阶段时不用每次打包Assetsbundle
        //手机上，UseBundle一定要为true

        if(Application.isMobilePlatform || GameDefine.UseBundle)
        {
            //如果是多个文件打包成一个Assetbundle  如多个配置表打成了一个Assetbundle 
            //则，除了要指定AssetName，还要指定Assetbundle name
            
            string bundleName = string.IsNullOrEmpty(abName) ? assetName.ToLower() : abName.ToLower();

            AssetBundle bundle = LoadAssetBundle(bundleName);                  
            if(bundle == null)
            {
                Log.Error("加载AssetBundle失败：" + bundleName);
                return default(T);
            }

            int startIndex = assetName.LastIndexOf('/');
            if (startIndex > 0)
                assetName = assetName.Substring(startIndex + 1);
            return bundle.LoadAsset<T>(assetName.ToLower());
        }
        else
        {
            sb.Append("Assets/Res/");
            sb.Append(assetName);
            sb.Append(ext);
            return (T)AssetDatabase.LoadMainAssetAtPath(sb.ToString());
        }
    }

    public static void Init()
    {
        byte[] stream = null;
        string uri = string.Empty;
        bundles = new Dictionary<string, AssetBundle>();
        uri = Util.DataPath + "StreamingAssets";
        if(!File.Exists(uri))
            return;
        stream = File.ReadAllBytes(uri);
        assetbundle = AssetBundle.CreateFromMemoryImmediate(stream);
        manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }
  

    static AssetBundle LoadAssetBundle(string abName)
    {
        if (!abName.EndsWith(".unity3d"))
            abName += ".unity3d";

        AssetBundle bundle = null;
        if(!bundles.ContainsKey(abName))
        {
            byte[] stream = null;
            string uri = Util.DataPath + abName;
            LoadDependencies(abName);
            stream = File.ReadAllBytes(uri);
            bundle = AssetBundle.CreateFromMemoryImmediate(stream);
            bundles.Add(abName, bundle);
        }
        else
        {
            bundles.TryGetValue(abName, out bundle);
        }
        return bundle;
    }

    static void LoadDependencies(string abName)
    {
        if(manifest == null)
        {
            Log.Error("请先初始化AssetBundleManifest");
            return;
        }
        string[] denpendencies = manifest.GetAllDependencies(abName);
        if (denpendencies.Length == 0)
            return;
        for (int i = 0; i < denpendencies.Length; i++)
            denpendencies[i] = RemapVariantName(abName);
        for (int i = 0; i < denpendencies.Length; i++)
            LoadAssetBundle(denpendencies[i]);
    }

    static string RemapVariantName(string abName)
    {
        string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
        if (Array.IndexOf(bundlesWithVariant, abName) < 0)
            return abName;
        string[] split = abName.Split('.');
        int bestFit = int.MaxValue;
        int bestFitIndex = -1;

        for(int i = 0; i < bundlesWithVariant.Length;i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;
            int found = Array.IndexOf(m_variants, curSplit[1]);
            if(found != -1 && found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }
        if (bestFitIndex != -1)
            return bundlesWithVariant[bestFitIndex];
        else
            return abName;
    }
}
