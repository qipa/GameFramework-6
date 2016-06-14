using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine;

public class CheckFileUTF8 : AssetPostprocessor
{
    static string basePath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths
        )
    {
        foreach(var path in importedAssets)
        {
            //只检查csv文件
            if( path.EndsWith(".csv") /*|| path.EndsWith(".txt") || path.EndsWith(".json") || path.EndsWith(".xml")*/)
            {
                string tmp = basePath + path;
                using(FileStream fs = new FileStream(tmp,FileMode.Open))
                {
                    if(GetFileEncode(fs) != Encoding.UTF8)  
                    {
                        Debug.LogError(tmp + "不是UTF-8格式");
                    }
                }
            }
        }
    }

    static Encoding GetFileEncode(FileStream fs)
    {
        BinaryReader br = new BinaryReader(fs);
        byte[] buff = br.ReadBytes(2);
        
        br.Close();
        fs.Close();
        if (buff.Length <= 0)
        {
            throw new Exception(fs.Name + "  文件为空");
        }

        if(buff[0] >= 0xEF)
        {
            if(buff[0] == 0xEF && buff[1] == 0xBB)
            {
                return Encoding.UTF8;
            }
            else if(buff[0] == 0xFE && buff[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }
            else if(buff[0] == 0xFF && buff[1] == 0xFE)
            {
                return Encoding.Unicode;
            }
            else
                return Encoding.Default;
        }
        else
            return Encoding.Default;
    }

    [MenuItem("Check/转换配置文件为UTF8编码")]
    public static void CheckUtf8()
    {
        string path = Application.dataPath + "/Resources/Config/";
        DirectoryInfo dir = new DirectoryInfo(path);

        FileInfo[] files = dir.GetFiles();
        foreach(var file in files)
        {
            if (file.FullName.EndsWith(".meta"))
                continue;
            using(FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                if(GetFileEncode(fs) != Encoding.UTF8)
                {
                    Debug.LogError(file.FullName + "不是UTF8格式");
                    ConvertToUTF8(file.FullName);
                }
            }
        }
        Debug.Log("转换完成");
    }

    public static void ConvertToUTF8(string path)
    {
        string text = File.ReadAllText(path, Encoding.Default);
        File.WriteAllText(path, text, Encoding.UTF8);
    }
}
