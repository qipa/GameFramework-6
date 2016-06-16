using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//开发阶段 资源放在Resources目录  以免每次打包AssetBundle
public class ResManager : MonoBehaviour 
{
    public static UnityEngine.Object Load(string path)
    {
        return Resources.Load(path);
    }
  
}
