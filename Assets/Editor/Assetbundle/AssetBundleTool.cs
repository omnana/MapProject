using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Omnana;
using System.Security.Cryptography;

public class AssetBundleTool
{
    //[MenuItem("AssetsBundle/打包安卓资源")]
    //static void BuildAndroidAssetBundles()
    //{
    //    BuildAllAssetBundles(BuildTarget.Android);
    //}

    //[MenuItem("AssetsBundle/打包苹果资源")]
    //static void BuildIosAssetBundles()
    //{
    //    BuildAllAssetBundles(BuildTarget.iOS);
    //}


    //[MenuItem("AssetsBundle/打包Window资源")]
    //static void BuildWindowAssetBundles()
    //{
    //    BuildAllAssetBundles(BuildTarget.StandaloneWindows64);
    //}

    public static void BuildAllAssetBundles(BuildTarget buildTarget) // 进行打包
    {
        string dir = Utility.GetAssetBundlePath();

        if (Directory.Exists(dir))
        {
            DeleteAllFile(dir);
        }

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (Directory.Exists(dir))
        {
            AssetBundleBuild[] builds = AssetBundleBuildMgr.Instance.Analyze();

            BuildPipeline.BuildAssetBundles(dir, builds, BuildAssetBundleOptions.None, buildTarget);
        }
    }

    public static bool DeleteAllFile(string fullPath)
    {
        //获取指定路径下面的所有资源文件  然后进行删除
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string FilePath = fullPath + "/" + files[i].Name;

                File.Delete(FilePath);
            }

            return true;
        }

        return false;
    }
}
