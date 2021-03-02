using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class GPath
{
    /// <summary>
    /// 
    /// </summary>
    public static string StreamingAssetsPath = Utility.GetAssetBundlePath() + "/";

    /// <summary>
    /// 
    /// </summary>
    public static string PersistentAssetsPath = Application.persistentDataPath + "/" + Utility.GetPlatformName() + "/";

    /// <summary>
    /// 版本文件名
    /// </summary>
    public static string VersionFileName = "Version.txt";

    /// <summary>
    /// 资源服务器路径
    /// </summary>
    public static string CDNUrl = "http://192.168.1.191:80/Test/" + Utility.GetPlatformName() + "/";
}
