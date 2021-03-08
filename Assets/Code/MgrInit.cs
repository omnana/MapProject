using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class MgrInit : MonoBehaviour
{
    public void Load()
    {
#if UNITY_EDITOR
        ServiceLocator.RegisterSingleton<EditorAssetLoadMgr>();
#endif
        ServiceLocator.RegisterSingleton<AssetBundleMgr>();
        ServiceLocator.RegisterSingleton<ResourcesLoadMgr>();
        ServiceLocator.RegisterSingleton<AssetLoadMgr>();
        ServiceLocator.RegisterSingleton<PrefabLoadMgr>();
        ServiceLocator.RegisterSingleton<DownloadMgr>();
        ServiceLocator.RegisterSingleton<FileVersionMgr>();
        ServiceLocator.RegisterSingleton<HotFixProjectMgr>();
    }
}
