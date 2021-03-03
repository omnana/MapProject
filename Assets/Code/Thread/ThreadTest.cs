using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class ThreadTest : MonoBehaviour
{
    public class Test
    {
        public int Id;
    }

    public Text debugText;

    private int Num = 1;

    Thread thread1;

    private object mLock = new object();

    private List<Test> loadingList = new List<Test>();

    private List<Test> completeList = new List<Test>();

    void Start()
    {
        thread1 = new Thread(ThreadLoop);
        var thread2 = new Thread(ThreadLoop);

        thread1.Start();
        thread2.Start();
    }


    private void ThreadLoop()
    {
        Thread.Sleep(1000);
    }

    private void Update()
    {
        debugText.text = thread1.IsAlive.ToString();

        if (Input.GetKeyDown(KeyCode.A))
        {
            thread1.Start();
            Debug.Log(Thread.CurrentThread.Name);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //thread1.Abort();
            Num = 0;
            thread1.Abort();
        }
    }
}
