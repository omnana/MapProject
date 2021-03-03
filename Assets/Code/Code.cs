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

    private HotfixMgr hotfixMgr;

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

        hotfixMgr = ServiceLocator.Resolve<HotfixMgr>();

        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        // 热更完毕或者已下载
        hotfixMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
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
        hotfixMgr.Start();

        hotfixMgr.CheckCDNVersion();
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
        downloadMgr.Update();
    }
}
