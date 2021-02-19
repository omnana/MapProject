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

        var mgr = ServiceLocator.Resolve<AssetLoadMgr>();

        assetLoadMgr = AssetLoadMgr.Instance;

    }

    private void Start()
    {
        GuiManager.Instance.OpenSync<TestGui>();
    }

    private void Update()
    {
        assetLoadMgr.Update();
    }
}
