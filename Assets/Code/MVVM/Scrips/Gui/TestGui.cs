using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine;

public class Robot
{
    public int Id;

    public Vector2 Pos;
}

public class TestGui : MonoBehaviour
{
    public TestView TestView;

    //List<Robot> Robots = new List<Robot>();


    public SpriteAtlas spriteAtlas;

    public Image[] images;

    void Awake()
    {
        TestView.BindingContext = new TestViewModel();

        MessageAggregator<string>.Instance.Subscribe("Test", RefreshTest);

        TestView.TestInput.onValueChanged.AddListener(n =>
        {
            MessageAggregator<string>.Instance.Publish("Test", this, new MessageArgs<string>(n));
        });

        //var dic = new SamDictonary<int, int>();

        //for (var i = 0; i < 100; i++)
        //{
        //    dic.Add(i, i * 10);
        //}

        //Debug.Log(dic.ContainsKey(2));

        //Debug.Log(dic.Remove(2));

        //Debug.Log(dic.ContainsKey(2));

        //Debug.Log();

        //Test();

        StartCoroutine(LoadImages());
    }

    private IEnumerator LoadImages()
    {
        images[0].sprite = spriteAtlas.GetSprite("Btn_Bg");
        images[1].sprite = spriteAtlas.GetSprite("ShareBtn");
        images[2].sprite = spriteAtlas.GetSprite("RedPoint1");
        images[3].sprite = spriteAtlas.GetSprite("RedPoint2");
        images[4].sprite = spriteAtlas.GetSprite("StarLight");
        images[5].sprite = spriteAtlas.GetSprite("StartGray");
        images[6].sprite = spriteAtlas.GetSprite("selectAre_small");

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

    void Start()
    {
        var gameMgr = ServiceLocator.Resolve<TestCtrl>();

        TestView.TestViewModel.TestName.Value = gameMgr.GetData();

        TestView.TestViewModel.TestInput.Value = gameMgr.GetData();
    }

    private void RefreshTest(object sender, MessageArgs<string> args)
    {
        TestView.TestViewModel.TestName.Value = args.Item;
    }
}
