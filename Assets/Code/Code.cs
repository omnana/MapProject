using UnityEngine;

public class Code : MonoBehaviour
{
    private AssetLoadMgr assetLoadMgr;

    private PrefabLoadMgr prefabLoadMgr;

    private DownloadMgr downloadMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();

        prefabLoadMgr = ServiceLocator.Resolve<PrefabLoadMgr>();

        downloadMgr = ServiceLocator.Resolve<DownloadMgr>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
        downloadMgr.Update();
    }
}
