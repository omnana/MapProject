using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using UnityEditor;

public class Code : MonoBehaviour
{
    private AssetLoadMgr assetLoadMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();
    }

    private void Start()
    {
        GuiManager.Instance.OpenAsync<TestGui>();
    }

    private void Update()
    {
        assetLoadMgr.Update();
    }
}
