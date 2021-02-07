using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using AssetBundles;

public class AssetBundleTool
{
    [MenuItem("AssetsBundle/打包安卓资源")]
    static void BuildAndroidAssetBundles()
    {
        BuildAllAssetBundles(BuildTarget.Android);
    }

    [MenuItem("AssetsBundle/打包苹果资源")]
    static void BuildIosAssetBundles()
    {
        BuildAllAssetBundles(BuildTarget.iOS);
    }


    [MenuItem("AssetsBundle/打包Window资源")]
    static void BuildWindowAssetBundles()
    {
        BuildAllAssetBundles(BuildTarget.StandaloneWindows64);
    }

    private static void BuildAllAssetBundles(BuildTarget buildTarget) // 进行打包
    {
        string dir = Utility.GetAssetBundlePath();

        if(Directory.Exists(dir))
        {
            DeleteAllFile(dir);
        }

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (Directory.Exists(dir))
        {
            // 参数一为打包到哪个路径，参数二压缩选项  参数三 平台的目标
            BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, buildTarget);
        }
    }


    [MenuItem("AssetsBundle/解析资源依赖关系")]
    private static void BuildAssetBundleDepedences()
    {
        AssetBundleBuildMgr.Instance.Analyze();
    }

    public static bool DeleteAllFile(string fullPath)
    {
        //获取指定路径下面的所有资源文件  然后进行删除
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

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
