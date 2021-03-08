using UnityEngine;
using AssetBundles;

public class Code : MonoBehaviour
{
    public MgrInit MgrInit;

    public ControllerInit ControllerInit;

    public ServiceContainerInit ServiceContainerInit;

    private AssetLoadMgr assetLoadMgr;

    private PrefabLoadMgr prefabLoadMgr;

    private DownloadMgr downloadMgr;

    private HotFix_ProjectMgr HotFix_ProjectMgr;

    private AssetBundleMgr assetBundleMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        MgrInit.Load();

        ControllerInit.Load();

        ServiceContainerInit.Load();

        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();

        prefabLoadMgr = ServiceLocator.Resolve<PrefabLoadMgr>();

        downloadMgr = ServiceLocator.Resolve<DownloadMgr>();

        HotFix_ProjectMgr = ServiceLocator.Resolve<HotFix_ProjectMgr>();

        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        // 热更完毕或者已下载
        HotFix_ProjectMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
        {
            assetBundleMgr.LoadMainfest();

            TableMgrLoader.StartLoad();

            TableMgrLoader.DownloadFinishCallabck = () =>
            {
                MessageAggregator<object>.Instance.Publish("DownloadFinish", this, null);
            };
        });
    }

    private void Start()
    {
        HotFix_ProjectMgr.Start();

        HotFix_ProjectMgr.CheckCDNVersion();
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
        downloadMgr.Update();
    }
}
