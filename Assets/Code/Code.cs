using UnityEngine;

public class Code : MonoBehaviour
{
    public MgrInit MgrInit;

    public ControllerInit ControllerInit;

    public ServiceContainerInit ServiceContainerInit;

    private AssetLoadMgr assetLoadMgr;

    private PrefabLoadMgr prefabLoadMgr;

    private DownloadMgr downloadMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        MgrInit.Load();

        ControllerInit.Load();

        ServiceContainerInit.Load();

        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();

        prefabLoadMgr = ServiceLocator.Resolve<PrefabLoadMgr>();

        downloadMgr = ServiceLocator.Resolve<DownloadMgr>();
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
        downloadMgr.Update();
    }
}
