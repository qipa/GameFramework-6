using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
public class GenerateCsvCode
{
     static string configPath = Application.dataPath + "/Resources/Config/";
     static string csvScriptPath = Application.dataPath + "/Script/CSV/Generate/";
    static char[] sp = { ',' };

    [MenuItem("Export/CSV_TO_C#")]
    public static void GenerateCode()
    {
        DirectoryInfo dir = new DirectoryInfo(configPath);
        if (!dir.Exists)
        {
            Debug.LogError("不存在目录 : " + configPath);
            return;
        }
        FindFiles(dir);
    }

    static void FindFiles(DirectoryInfo dir)
    {
        FileInfo[] fileInfo = dir.GetFiles();
        foreach(FileInfo file in fileInfo)
        {
            if (!file.Name.EndsWith(".csv"))
                continue;
            
            using(StreamReader sr = new StreamReader(new FileStream(file.FullName,FileMode.Open),Encoding.Default))
            {
                string desc = sr.ReadLine();        //字段注释
                string field = sr.ReadLine();       //字段名               
                string type = sr.ReadLine();        //字段类型

                string[] fields = field.Split(sp,System.StringSplitOptions.None);
                string[] descs = desc.Split(sp,System.StringSplitOptions.None);
                string[] types = type.Split(sp,System.StringSplitOptions.None);           
                Process(file.Name, fields, descs, types);
            }
           
        }
    }

    static void Process(string file,string[] fields,string[] descs,string[] types)
    {
        
        string fileName = file.Replace(".csv", "");
        string path = csvScriptPath + "CSV" + fileName + ".cs";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        Debug.Log("正在处理 : " + file + "  生成代码路径 : " + path);

        using (StreamWriter sw = new StreamWriter(new FileStream(path,FileMode.OpenOrCreate),Encoding.Default))
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//此代码根据 " + fileName + ".csv 自动生成，不要手动修改！！！！");
            sb.AppendLine("//工具菜单：Export/CSV_TO_C#    生成时间 ： " + System.DateTime.Now.ToString());
            sb.AppendLine("using System;");
            sb.AppendLine("public class CSV" + fileName);
            sb.AppendLine("{");

            for (int i = 0; i < fields.Length;++i )
            {
                string type = types[i];
                string field = fields[i];
                string desc = descs[i];
                sb.AppendLine("\tpublic " + type + " " + field + ";    //" + desc);
            }
            
            sb.AppendLine("}");

            sw.Write(sb.ToString());
        }
    }

    [MenuItem("Export/Test")]
    public static void Test()
    {
        CSVManager.LoadAllCsv();
    }
}

