using UnityEngine.UI;
using AssetBundles;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotfixMgr hotfixMgr;

    private void Start()
    {
        hotfixMgr = ServiceLocator.Resolve<HotfixMgr>();

        MessageAggregator<object>.Instance.Subscribe("DownloadFinish", DownloadFinish);
    }

    private void DownloadFinish(object sender, MessageArgs<object> args)
    {
        GuiManager.Instance.OpenAsync<TestGui>();

        Destroy(gameObject);
    }

    private void Update()
    {
        Slider.value = hotfixMgr.DownloadProgress;
    }
}
