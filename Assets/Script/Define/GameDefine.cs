using UnityEngine;
using System.Collections;

public class GameDefine 
{
    public const string AppName = "ZWS";
    public static bool UseBundle = true;    //是否Assetbundle模式
    public static string IP = "";
    public static int port = 0;
    public static bool UseAstar = true;
    public const string WebUrl = "http://localhost:6688/";      //测试更新地址
    public const bool UpdateRes = false;                        //暂不开启更新流程
}
