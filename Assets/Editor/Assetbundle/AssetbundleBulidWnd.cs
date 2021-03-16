using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Omnana;
using System.Security.Cryptography;

public class AssetbundleBulidWnd : EditorWindow
{
    private Vector2 wndSize = new Vector2(300, 500);

    private static string versionCode;

    private BuildTarget buildTarget;

    public string[] options = new string[] {"Windows", "Ios", "Android" };

    public BuildTarget[] optionsIndex = new BuildTarget[] { BuildTarget.StandaloneWindows64, BuildTarget.iOS, BuildTarget.Android };

    private int index;

    [MenuItem("Tools/AssetsBundle/打包AssetsBundle资源")]
    private static void Open()
    {
        var wnd = GetWindow<AssetbundleBulidWnd>();

        wnd.OpenWnd(new Vector2(400, 300));
    }

    public void OpenWnd(Vector2 pos)
    {
        autoRepaintOnSceneChange = true;

        Show();

        position = new Rect(pos.x, pos.y, wndSize.x, wndSize.y);

        versionCode = "1001000";
    }

    private void OnGUI()
    {
        versionCode = EditorGUILayout.TextField("打包资源版本号：", versionCode);

        index = EditorGUILayout.Popup(index, options);

        var target = optionsIndex[index];

        if (GUI.Button(new Rect(wndSize.x * 0.25f - 25, wndSize.y * 0.75f, 50, 25), "确定"))
        {
            AssetBundleTool.BuildAllAssetBundles(target);

            BuildAllAssetBundlesConfigList(target);
        }

        if (GUI.Button(new Rect(wndSize.x * 0.5f + 25, wndSize.y * 0.75f, 50, 25), "取消"))
        {
            Close();
        }
    }

    /// <summary>
    /// 生成版本资源列表
    /// </summary>
    /// <param name="buildTarget"></param>
    private static void BuildAllAssetBundlesConfigList(BuildTarget buildTarget)
    {
        var fullPath = Application.streamingAssetsPath + "/" + Utility.GetPlatformForAssetBundles(buildTarget);

        if (Directory.Exists(fullPath))
        {
            var direction = new DirectoryInfo(fullPath);

            var files = direction.GetFiles("*", SearchOption.AllDirectories);

            var buffer = new byte[16 * 1024]; // 一次读取长度 16384 = 16 * 1024 kb

            var output = new byte[buffer.Length];

            int readLength = 0;//每次读取长度

            var fileVersionDatas = new List<FileVersionData>();

            var sb = new System.Text.StringBuilder(32);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                sb.Clear();

                var filePath = fullPath + "/" + files[i].Name;

                var inputStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var size = (int)inputStream.Length;

                var hashAlgorithm = new MD5CryptoServiceProvider();

                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0) // 计算MD5
                {
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }

                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);

                var retVal = hashAlgorithm.Hash;

                for (int j = 0; j < retVal.Length; j++)
                {
                    sb.Append(retVal[j].ToString("x2"));
                }

                hashAlgorithm.Clear();

                inputStream.Close();

                fileVersionDatas.Add(new FileVersionData()
                {
                    Name = files[i].Name,
                    Size = size,
                    Md5 = sb.ToString(),
                    Version = int.Parse(versionCode),
                });
            }

            var versionListPath = Utility.GetAssetBundlePath() + "/" + versionCode + ".txt";

            if (File.Exists(versionListPath)) File.Delete(versionListPath);

            sb.Clear();

            for (var i = 0; i < fileVersionDatas.Count; i++)
            {
                sb.AppendLine(fileVersionDatas[i].ToString());
            }

            File.WriteAllText(versionListPath, sb.ToString());
        }
    }
}
