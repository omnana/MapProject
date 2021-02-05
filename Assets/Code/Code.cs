using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class Code : MonoBehaviour
{
    private AssetBundleMgr assetBundleMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        assetBundleMgr.LoadMainfest();
    }

    private void Start()
    {
        GuiManager.Instance.OpenSync<TestGui>();
    }

    private void Update()
    {
        assetBundleMgr.Update();
    }
}
