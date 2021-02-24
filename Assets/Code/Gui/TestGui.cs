using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TestGui : BaseGui
{
    public TestView TestView;

    public Image image;

    public Button TestBtn;

    public Button UnLoadBtn;

    public Button HttpGetBtn;

    private Queue<GameObject> objQueue = new Queue<GameObject>();


    void Awake()
    {
        TestView.BindingContext = new TestViewModel();

        TestView.OnAppear();

        TestView.TestInput.onValueChanged.AddListener(n => { TestView.ExcuteSetInput(n); });

        TestBtn.onClick.AddListener(OnClick);

        UnLoadBtn.onClick.AddListener(UnLoad_OnClick);

        HttpGetBtn.onClick.AddListener(HttpGetBtn_OnClick);
    }

    private void Start()
    {
        var data = FiguresModelMgr.Ins.GetModelById(1);

        Debug.Log(data.Brief);

        image.sprite = ServiceContainer.Resolve<ResourceService>().LoadSpriteFromAtlasSync("RedPoint1");

        image.SetNativeSize();
    }

    private void OnClick()
    {
        //var prefab = ServiceLocator.Resolve<ResourceService>().LoadModelSync(TestView.TestInput.text);

        // if (prefab != null)
        // {
        //     var obj1 = Instantiate(prefab);

        //     objQueue.Enqueue(obj1);
        // }

        ServiceLocator.Resolve<PrefabLoadMgr>().LoadAsync(TestView.TestInput.text, (assetName, obj) =>
        {
            objQueue.Enqueue(obj);
        }, transform);

       //ServiceLocator.Resolve<ResourceService>().LoadModelAsync(TestView.TestInput.text, (obj) =>
       //{
       //    if (obj != null)
       //    {
       //        var obj1 = Instantiate(obj);

       //        objQueue.Enqueue(obj1);
       //    }
       //});
    }

    private void UnLoad_OnClick()
    {
        if (objQueue.Count == 0) return;

        var b = objQueue.Dequeue();

        var assetName = b.name;

        Destroy(b);

        ServiceLocator.Resolve<PrefabLoadMgr>().Destroy(b);
    }

    private void HttpGetBtn_OnClick()
    {
        TestView.ExcuteGetData();
    }
}
