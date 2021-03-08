using UnityEngine.UI;
using AssetBundles;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotFixProjectMgr HotFixProjectMgr;

    private void Awake()
    {
        HotFixProjectMgr = ServiceLocator.Resolve<HotFixProjectMgr>();

        MessageAggregator<object>.Instance.Subscribe("DownloadFinish", DownloadFinish);
    }

    private void DownloadFinish(object sender, MessageArgs<object> args)
    {
        GuiManager.Instance.OpenAsync<TestGui>();

        Destroy(gameObject);
    }

    private void Update()
    {
        if(HotFixProjectMgr != null)
            Slider.value = HotFixProjectMgr.DownloadProgress;
    }
}
