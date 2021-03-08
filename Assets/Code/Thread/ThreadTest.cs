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

    private static bool running = true;

    private int num = 0;

    private static int x, y = 0;
    private static int a, b = 0;

    void Start()
    {
        thread1 = new Thread(ThreadLoop);

        thread1.Start();

        //var thread2 = new Thread(ThreadLoop);

        //thread1.Start();
        //thread2.Start();

        //Thread.Sleep(1000);
    }


    private void ThreadLoop()
    {
        //while (running)
        //{
        //    num++;
        //    Debug.Log("running = " + num);
        //}

        //Debug.Log("end and num = " + num);
        //var i = 0;
        //for (; ; )
        //{
        //    i++;
        //    a = 0;
        //    b = 0;
        //    x = 0;
        //    y = 0;

        //    Thread t1 = new Thread(() => {
        //        a = 1;
        //        x = b;
        //    });

        //    Thread t2 = new Thread(() => {
        //        b = 1;
        //        y = a;
        //    });

        //    t1.Start();
        //    t2.Start();
        //    t1.Join();
        //    t2.Join();

        //    if (x == 0 && y == 0)
        //    {
        //        Debug.LogFormat("第{0}次  : x == 0 && y== 0");
        //        break;
        //    }
        //    else
        //    {

        //    }
        //}
    }

    private void Update()
    {
        debugText.text = thread1.ThreadState.ToString();

        if (Input.GetKeyDown(KeyCode.A))
        {
            thread1.Resume();
            //Debug.Log(Thread.CurrentThread.Name);
            Thread.ResetAbort();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("running is false");

            running = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //Debug.Log("running is false");
            thread1.Abort();
            //running = false;
        }
    }
}
