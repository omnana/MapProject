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

    private HotFixProjectMgr HotFixProjectMgr;

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

        HotFixProjectMgr = ServiceLocator.Resolve<HotFixProjectMgr>();

        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        // 热更完毕或者已下载
        HotFixProjectMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
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
        HotFixProjectMgr.Start();

        HotFixProjectMgr.CheckCDNVersion();
    }

    private void Update()
    {
        assetLoadMgr.Update();
        prefabLoadMgr.Update();
        downloadMgr.Update();
    }
}
