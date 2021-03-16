using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Omnana
{
    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string AssetBuildPath = Application.dataPath + "/Art";

        public static string GetAssetMainfestPath()
        {
            return Application.streamingAssetsPath + "/" + GetPlatformName() + "/" + GetPlatformName();
        }

        public static string GetAssetBundlePath(string abName)
        {
            return string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, GetPlatformName(), abName);
        }

        public static string GetAssetBundlePath()
        {
            return Application.streamingAssetsPath + "/" + GetPlatformName();
        }

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "IOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }

        public static string GetMainfestFileVersionPath()
        {
            return string.Format("{0}/{1}/{2}.{3}", Application.streamingAssetsPath, GetPlatformName(), GetPlatformName(), "manifest");
        }
    }
}