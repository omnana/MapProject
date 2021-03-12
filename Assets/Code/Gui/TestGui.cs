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

    protected override void OnInit()
    {
        base.OnInit();

        TestView.BindingContext = new TestViewModel();

        TestView.OnAppear();

        TestView.TestInput.onValueChanged.AddListener(n => { TestView.ExcuteSetInput(n); });

        TestBtn.onClick.AddListener(OnClick);

        UnLoadBtn.onClick.AddListener(UnLoad_OnClick);

        HttpGetBtn.onClick.AddListener(HttpGetBtn_OnClick);

        //var tree = new BSTTree();
        ////tree.Add(8);
        //tree.Add(3);
        //tree.Add(1);
        //tree.Add(4);
        ////tree.Add(5);
        ////tree.Add(15);
        ////tree.Add(9);
        ////tree.Add(2);

        //tree.MiddleOrderTraversal();
        //tree.Remove(1);
        //tree.Remove(3);
        //tree.Remove(4);
        ////tree.Remove(5);
        ////tree.Remove(4);
        ////tree.Remove(15);
        ////tree.Remove(9);
        ////tree.Remove(3);
        ////tree.Remove(8);
        //Debug.Log("-------------");
        //tree.MiddleOrderTraversal();
    }

    protected override void OnOpen()
    {
        base.Open();

        Debug.Log("123123");

        var data = TableMgrContainer.Resolve<TestModelMgr>().GetTableById(0);

        //Debug.Log(data.Test1);

        image.sprite = ServiceContainer.Resolve<ResourceService>().LoadSpriteFromAtlasSync("RedPoint1");

        image.SetNativeSize();
    }


    private void OnClick()
    {
        ServiceLocator.Resolve<PrefabLoadMgr>().LoadAsync(TestView.TestInput.text, (assetName, obj) =>
        {
            objQueue.Enqueue(obj);
        }, transform);
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
