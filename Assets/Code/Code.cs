using UnityEngine;
using AssetBundles;

public class Code : MonoBehaviour
{
    public ControllerInit ControllerInit;

    public ServiceContainerInit ServiceContainerInit;

    private HotFixMgr hotFixMgr;

    private AssetBundleMgr assetBundleMgr;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        ControllerInit.Load();

        ServiceContainerInit.Load();

        //hotFixMgr = Singleton<HotFixMgr>.GetInstance();

        Singleton<MyAssetBundleMgr>.GetInstance().LoadMainfest();

        TableHelper.DownloadFinishCallabck = () =>
        {
            StartCoroutine(ILRuntimeHelper.LoadHotFix_ProjectAssembly(() => { }));

            MessageAggregator<object>.Instance.Publish("DownloadFinish", this, null);
        };

        TableHelper.StartLoad();

        //// 热更完毕或者已下载
        //hotFixMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
        //{
        //    assetBundleMgr.LoadMainfest();

        //    Singleton<MyAssetBundleMgr>.GetInstance().LoadMainfest();

        //    TableMgrLoader.StartLoad();

        //    TableMgrLoader.DownloadFinishCallabck = () =>
        //    {
        //        StartCoroutine(ILRuntimeHelper.LoadHotFix_ProjectAssembly(() => { }));

        //        MessageAggregator<object>.Instance.Publish("DownloadFinish", this, null);
        //    };
        //});
    }

    private void Start()
    {
        //hotFixMgr.Start();

        //hotFixMgr.CheckCDNVersion();
    }
}
