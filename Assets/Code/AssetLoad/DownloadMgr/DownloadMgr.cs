using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

public class DownloadMgr
{
    private const int MAX_THREAD_COUNT = 20;

    private static readonly object mlock = new object();

    private Queue<DownloadFileMac> readyList;

    private Dictionary<Thread, DownloadFileMac> runningList;

    private List<DownloadUnit> completeList;

    private List<DownloadFileMac> errorList;

    private DownloadMgr()
    {
        readyList = new Queue<DownloadFileMac>();

        runningList = new Dictionary<Thread, DownloadFileMac>();

        completeList = new List<DownloadUnit>();

        errorList = new List<DownloadFileMac>();
    }

    /// <summary>
    /// 异步会创建线程
    /// </summary>
    /// <param name="info"></param>
    public void DownloadAsync(DownloadUnit info)
    {
    }

    /// <summary>
    /// 同步不会调用回调函数，返回是否成功
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool DownloadSync(DownloadUnit info)
    {
        return false;
    }

    public void Update()
    {
    }
}
