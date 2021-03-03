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
    private byte[] serverFile;

    // 第一步：将本地资源转化为字节
    // 第二步：模拟断电续传
    private void Awake()
    {
        var serverPath = Application.streamingAssetsPath + "/Windows/cube_a.ab.manifest";

        var savePath = Application.dataPath + "/DownloadTest/downloadFile.ab.manifest";

        serverFile = File.ReadAllBytes(serverPath);

        var fs = new FileStream(savePath, FileMode.Create);

        fs.Write(serverFile, 0, serverFile.Length);

        fs.Flush();

        fs.Close();
    }

    public int GetFileSize()
    {
        return serverFile.Length;
    }

    private void Read(byte[] buffer, int offset, int count)
    {

    }
}
