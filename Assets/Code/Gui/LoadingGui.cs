using UnityEngine.UI;
using AssetBundles;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotfixMgr hotfixMgr;

    private AssetBundleMgr assetBundleMgr;

    private void Start()
    {
        hotfixMgr = ServiceLocator.Resolve<HotfixMgr>();

        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        // 热更完毕或者已下载
        hotfixMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
        {
            assetBundleMgr.LoadMainfest();

            TableMgrInit.Load();

            GuiManager.Instance.OpenSync<TestGui>();

            Destroy(gameObject);

        });

        hotfixMgr.Start();

        hotfixMgr.CheckCDNVersion();
    }


    private void Update()
    {
        Slider.value = hotfixMgr.DownloadProgress;
    }
}
