using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AssetBundles;
using System.IO;

/// <summary>
/// 断点续传测试案例
/// </summary>
public class DownloadTest : MonoBehaviour
{
    // 第一步：将本地资源转化为字节
    // 第二步：模拟断电续传
    private void Start()
    {
        var bytes = GetFile();
    }

    private byte[] GetFile()
    {
        var obj = Resources.Load("Version.txt");

        return FileUtil.Serialize(obj);

    } 
}
