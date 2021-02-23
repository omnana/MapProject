using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine;
using AssetBundles;


public class TestGui : BaseGui
{
    public TestView TestView;

    public Image image;

    public Button TestBtn;
    public Button TestBtn1;

    public Button UnLoadBtn;
    public Button UnLoadBtn1;

    private Queue<GameObject> objQueue = new Queue<GameObject>();


    void Awake()
    {
        TestView.BindingContext = new TestViewModel();

        MessageAggregator<string>.Instance.Subscribe("Test", RefreshTest);

        TestView.TestInput.onValueChanged.AddListener(n =>
        {
            MessageAggregator<string>.Instance.Publish("Test", this, new MessageArgs<string>(n));
        });

        TestBtn.onClick.AddListener(OnClick);

        UnLoadBtn.onClick.AddListener(UnLoad_OnClick);
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

    private void Start()
    {
        image.sprite = ServiceLocator.Resolve<ResourceService>().LoadSpriteFromAtlasSync("RedPoint1");
    }

   
    private int A(int i)
    {
        if (i == 1 || i == 2) return 1;

        return A(i - 1) + A(i - 2);
    }

    private void RefreshTest(object sender, MessageArgs<string> args)
    {
        TestView.TestViewModel.TestName.Value = args.Item;
    }
}
