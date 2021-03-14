using UnityEngine.UI;
using AssetBundles;
using UnityEngine.Events;
using UnityEngine;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotFix_ProjectMgr HotFix_ProjectMgr;

    private bool isFinish = false;

    private void Awake()
    {
        HotFix_ProjectMgr = ServiceLocator.Resolve<HotFix_ProjectMgr>();

        MessageAggregator<object>.Instance.Subscribe("DownloadFinish", DownloadFinish);
    }

    private void DownloadFinish(object sender, MessageArgs<object> args)
    {
        isFinish = true;
        //GuiManager.Instance.OpenAsync<TestGui>();
        //Destroy(gameObject);
    }

    private void Update()
    {
        if(HotFix_ProjectMgr != null)
            Slider.value = HotFix_ProjectMgr.DownloadProgress;

        if (isFinish && Input.GetKeyDown(KeyCode.A))
        {
            GuiManager.Instance.OpenAsync<TestGui>();
            Destroy(gameObject);
        }
    }
}
