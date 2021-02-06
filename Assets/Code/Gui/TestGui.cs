using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine;
using AssetBundles;


public class TestGui : MonoBehaviour
{
    public TestView TestView;

    //List<Robot> Robots = new List<Robot>();

    //public SpriteAtlas spriteAtlas;

    public Image[] images;

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
        ServiceLocator.Resolve<ResourceService>().LoadModelAsync(TestView.TestInput.text, (obj) =>
       {
           if (obj != null)
           {
               var obj1 = Instantiate(obj);

               objQueue.Enqueue(obj1);
           }
       });
    }

    private void UnLoad_OnClick()
    {
        if (objQueue.Count == 0) return;

        var b = objQueue.Dequeue();

        Destroy(b);

        ServiceLocator.Resolve<ResourceService>().UnLoadAsset(ResourceType.Model);
    }

    private void Start()
    {
        //StartCoroutine(LoadImages());
    }

    private IEnumerator LoadImages()
    {
        //images[0].sprite = spriteAtlas.GetSprite("Btn_Bg");
        //images[1].sprite = spriteAtlas.GetSprite("ShareBtn");
        //images[2].sprite = spriteAtlas.GetSprite("RedPoint1");
        //images[3].sprite = spriteAtlas.GetSprite("RedPoint2");
        //images[4].sprite = spriteAtlas.GetSprite("StarLight");
        //images[5].sprite = spriteAtlas.GetSprite("StartGray");
        //images[6].sprite = spriteAtlas.GetSprite("selectAre_small");

        foreach (var m in images)
        {
            m.SetNativeSize();
        }

        yield return new WaitForEndOfFrame();

        gameObject.SetActive(true);
    }

    private int A(int i)
    {
        if (i == 1 || i == 2) return 1;

        return A(i - 1) + A(i - 2);
    }

    //void Start()
    //{
    //    var gameMgr = ServiceLocator.Resolve<TestCtrl>();

    //    TestView.TestViewModel.TestName.Value = gameMgr.GetData();

    //    TestView.TestViewModel.TestInput.Value = gameMgr.GetData();
    //}

    private void RefreshTest(object sender, MessageArgs<string> args)
    {
        TestView.TestViewModel.TestName.Value = args.Item;
    }
}
