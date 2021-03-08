using UnityEngine.UI;
using AssetBundles;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotFix_ProjectMgr HotFix_ProjectMgr;

    private void Awake()
    {
        HotFix_ProjectMgr = ServiceLocator.Resolve<HotFix_ProjectMgr>();

        MessageAggregator<object>.Instance.Subscribe("DownloadFinish", DownloadFinish);
    }

    private void DownloadFinish(object sender, MessageArgs<object> args)
    {
        GuiManager.Instance.OpenAsync<TestGui>();

        Destroy(gameObject);
    }

    private void Update()
    {
        if(HotFix_ProjectMgr != null)
            Slider.value = HotFix_ProjectMgr.DownloadProgress;
    }
}
