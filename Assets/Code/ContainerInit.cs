using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class ContainerInit : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);

        InitMgr();

        InitServices();

        InitCtrl();

        //TableMgrInit.Init(() => 
        //{
        //    GuiManager.Instance.OpenAsync<TestGui>();
        //});
    }

    private void InitCtrl()
    {
        ServiceLocator.RegisterSingleton<TestCtrl>();
    }

    private void InitServices()
    {
        // 基础服务
        ServiceContainer.AddService<RestService>(gameObject);
        ServiceContainer.AddService<ResourceService>(gameObject);
        // 业务服务
        ServiceContainer.AddService<TestServicer>(gameObject);
    }

    private void InitMgr()
    {
#if UNITY_EDITOR
        ServiceLocator.RegisterSingleton<EditorAssetLoadMgr>();
#endif
        ServiceLocator.RegisterSingleton<AssetBundleMgr>();
        ServiceLocator.RegisterSingleton<ResourcesLoadMgr>();
        ServiceLocator.RegisterSingleton<AssetLoadMgr>();
        ServiceLocator.RegisterSingleton<PrefabLoadMgr>();
        ServiceLocator.RegisterSingleton<DownloadMgr>();
    }
}
