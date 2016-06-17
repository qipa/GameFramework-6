using System;
using System.IO;
using UnityEngine;

public class Log
{
    static StreamWriter ms_sw = null;
    static bool ms_first = true;

    static string GetLogPath()
    {
        if (Application.platform == RuntimePlatform.Android)
            return "/sdcard/zws_res/";
        else
            return Application.dataPath + "/Logs/";
    }

    static void WriteFile(string msg)
    {
        if (ms_first == true)
        {
            try
            {
                string path = GetLogPath();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //ms_sw = File.CreateText(path + "zws.txt");
                ms_sw = new StreamWriter(path + "zws.txt");
                ms_sw.AutoFlush = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }

            ms_first = false;
        }

        if (ms_sw != null)
        {
            ms_sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + msg);
            //ms_sw.Flush();
        }
    }

    public static void Start()
    {

    }

    public static void Stop()
    {
        if (ms_sw != null)
        {
            ms_sw.Close();
            ms_sw = null;
        }
    }

    public static void Info(string msg)
    {
        if ("" == msg) return;
        Debug.Log(msg);

        WriteFile("Info   :  " + msg);
    }

    public static void Error(string msg)
    {
        if ("" == msg) return;      
        Debug.LogError(msg);

        WriteFile("Error  :  " + msg);
    }

    public static void Warning(string msg)
    {
        if ("" == msg) return;
        Debug.LogWarning(msg);

        WriteFile("Warning:  " + msg);
    }

    public static void Exception(Exception excep)
    {
        Debug.LogException(excep);

        WriteFile("Exception:  " + excep.Message);
    }
}

