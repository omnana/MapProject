using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using UnityEditor;

public class Code : MonoBehaviour
{
    private AssetLoadMgr assetLoadMgr;

    private PrefabLoadMgr prefabLoadMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();

        prefabLoadMgr = ServiceLocator.Resolve<PrefabLoadMgr>();
    }

    private void Start()
    {
        GuiManager.Instance.OpenAsync<TestGui>();
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
    }
}
