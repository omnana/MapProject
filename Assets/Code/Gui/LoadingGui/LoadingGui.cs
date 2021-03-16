using UnityEngine.UI;
using Omnana;
using UnityEngine.Events;
using UnityEngine;

public class LoadingGui : BaseGui
{
    public Slider Slider;

    private HotFixMgr hotFixMgr;

    private bool isFinish = false;

    private void Awake()
    {
        hotFixMgr = HotFixMgr.Instance;

        MessageAggregator<object>.Instance.Subscribe(MessageType.DownloadFinish, DownloadFinish);
    }

    private void DownloadFinish(object sender, MessageArgs<object> args)
    {
        isFinish = true;

        //Singleton<GuiManager>.GetInstance().OpenAsync<TestGui>();

        //Destroy(gameObject);
    }

    private void Update()
    {
        if(hotFixMgr != null)
            Slider.value = hotFixMgr.DownloadProgress;

        if (isFinish && Input.GetKeyDown(KeyCode.A))
        {
            GuiManager.Instance.OpenAsync<TestGui>();
             
            Destroy(gameObject);
        }
    }
}
