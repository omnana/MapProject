using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class ContainerInit : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);

        InitServices();

        InitCtrl();

        InitMgr();
    }

    private void InitCtrl()
    {
        ServiceLocator.RegisterSingleton<TestCtrl>();
    }

    private void InitServices()
    {
        ServiceLocator.RegisterSingleton<ResourceService>();
    }

    private void InitMgr()
    {
        ServiceLocator.RegisterSingleton<AssetBundleMgr>();
        ServiceLocator.RegisterSingleton<ResourcesLoadMgr>();
        ServiceLocator.RegisterSingleton<EditorAssetLoadMgr>();
    }
}
