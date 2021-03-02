using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/*
 在游戏开发中，热更并下载资源，对商业化游戏来说是一个必须的需求。
 而想要高效并稳定地下载文件一直是开发中的一个痛点我们能看到市面
 上，大量游戏都在下载上卡死，断网，重启无效等等问题。我们来总结
 一下要解决的问题：
（1）网络请求异常处理——断网、请求失败、请求超时、网络波动、下载到一半等问题
（2）文件读写异常处理——文件读失败、文件写失败、文件写一半等问题
（3）游戏进程异常处理——下载到一半、文件写一半、玩家退出等问题
（4）重启现场恢复处理——可恢复性，继续下载
（5）文件的下载正确性——文件长度、文件校验，文件可读性
（6）文件线程高效下载——多线程，异步回调文件
 */

public class DownloadMgr
{
    private const int MAX_THREAD_COUNT = 20;

    private static readonly object mlock = new object();

    private Queue<DownloadFileMac> readyList;

    private Dictionary<Thread, DownloadFileMac> runningList;

    private List<DownloadUnit> completeList;

    private List<DownloadFileMac> errorList;

    public DownloadMgr()
    {
        readyList = new Queue<DownloadFileMac>();

        runningList = new Dictionary<Thread, DownloadFileMac>();

        completeList = new List<DownloadUnit>();

        errorList = new List<DownloadFileMac>();

        //https解析的设置
        ServicePointManager.DefaultConnectionLimit = 100;

        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
    }

    private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    /// <summary>
    /// 异步会创建线程
    /// </summary>
    /// <param name="info"></param>
    public void DownloadAsync(DownloadUnit info)
    {
        if (info == null) return;

        var fileMac = new DownloadFileMac(info);

        lock (mlock)
        {
            readyList.Enqueue(fileMac);
        }

        if (runningList.Count < MAX_THREAD_COUNT)
        {
            var thread = new Thread(ThreadLoop);

            lock (mlock)
            {
                runningList.Add(thread, null);
            }

            thread.Start();
        }
    }

    /// <summary>
    /// 同步不会调用回调函数，返回是否成功
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool DownloadSync(DownloadUnit info)
    {
        if (info == null) return false;

        var mac = new DownloadFileMac(info);

        try // 同步下载尝试三次
        {
            mac.Run();
            if (mac.State == DownloadMacState.Complete) return true;
            mac.Run();
            if (mac.State == DownloadMacState.Complete) return true;
            mac.Run();
            if (mac.State == DownloadMacState.Complete) return true;
        }
        catch (Exception ex)
        {
            Debug.Log("Error DownloadSync " + mac.State + " " + mac.DownUnit.Name + " " + ex.Message + " " + ex.StackTrace);
        }

        return false;
    }

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="info"></param>
    public void DeleteDownload(DownloadUnit info)
    {
        lock (mlock)
        {
            info.IsDelete = true;
        }
    }

    //清理所有下载
    public void ClearAllDownloads()
    {
        lock (mlock)
        {
            foreach (var mac in readyList)
            {
                if (mac != null) mac.DownUnit.IsDelete = true;
            }

            foreach (var item in runningList)
            {
                if (item.Value != null) item.Value.DownUnit.IsDelete = true;
            }

            foreach (var unit in completeList)
            {
                if (unit != null) unit.IsDelete = true;
            }
        }
    }

    private void ThreadLoop()
    {
        while (true)
        {
            DownloadFileMac mac = null;

            lock (mlock)
            {
                if (readyList.Count > 0)
                {
                    mac = readyList.Dequeue();

                    runningList[Thread.CurrentThread] = mac;

                    if (mac != null && mac.DownUnit.IsDelete) // 已经销毁，不提取运行，直接删除
                    {
                        runningList[Thread.CurrentThread] = null;

                        continue;
                    }
                }
            }

            // 已经没有需要下载的了
            if (mac == null) break;

            mac.Run();

            if (mac.State == DownloadMacState.Complete)
            {
                lock (mlock)
                {
                    completeList.Add(mac.DownUnit);

                    runningList[Thread.CurrentThread] = null;
                }

            }
            else if (mac.State == DownloadMacState.Error)
            {
                lock (mlock)
                {
                    readyList.Enqueue(mac);
                    // 防止失败频繁回调，只在特定次数回调
                    if (mac.IsNeedErrorCall())
                    {
                        errorList.Add(mac);
                    }
                }

                break;
            }
            else
            {
                //ThreadDebugLog.Log("Error DownloadMacState " + mac.State + " " + mac.DownUnit.name);
                break;
            }
        }
    }

    /// <summary>
    /// 回调完成
    /// </summary>
    private void UpdateComplete()
    {
        if (completeList.Count == 0) return;

        DownloadUnit[] completes = null;

        lock (mlock)
        {
            completes = completeList.ToArray();

            completeList.Clear();
        }

        for(var i = 0; i< completes.Length; i++)
        {
            var info = completes[i];

            if (info.IsDelete) continue;

            info.IsDelete = true;

            info.ProgressFun?.Invoke(info, info.Size, info.Size);

            if (info.CompleteFun != null)
            {
                try
                {
                    info.CompleteFun(info);
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateComplete : " + ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateError()
    {
        if (errorList.Count == 0) return;

        DownloadFileMac[] errorArr = null;

        lock (mlock)
        {
            errorArr = errorList.ToArray();

            errorList.Clear();
        }

        foreach (var mac in errorArr)
        {
            var info = mac.DownUnit;

            if (info.IsDelete) continue; //已经销毁，不进行回调

            if (info.ErrorFun != null)
            {
                info.ErrorFun(info, mac.Error);

                mac.Error = string.Empty;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateProgress()
    {
        if (runningList.Count == 0) return;

        var runArr = new List<DownloadFileMac>();

        lock (mlock)
        {
            foreach (var mac in runningList.Values)
            {
                if (mac != null) runArr.Add(mac);
            }
        }

        foreach (var mac in runArr)
        {
            var info = mac.DownUnit;

            if (info.IsDelete) continue; //已经销毁，不进行回调

            info.ProgressFun?.Invoke(info, mac.CurSize, mac.AllSize);
        }
    }

    /// <summary>
    /// 线程遍历
    /// 线程遍历就三个逻辑：关闭卡死线程、网络断开处理和开启新线程。
    /// 实现了线程的动态创建和销毁。
    /// </summary>
    private void UpdateThread()
    {
        if (readyList.Count == 0 && runningList.Count == 0) return;

        //关闭卡死的线程
        lock (mlock)
        {
            var threadList = new List<Thread>();

            foreach (var item in runningList)
            {
                if (item.Key.IsAlive) continue; // 已经在运行的跳过

                if (item.Value != null) readyList.Enqueue(item.Value);

                threadList.Add(item.Key);
            }

            foreach (var thread in threadList)
            {
                runningList.Remove(thread);

                thread.Abort();
            }
        }

        //网络断开处理
        if (!CheckNetworkActive()) return;

        //线程数量不足，创建
        if (runningList.Count >= MAX_THREAD_COUNT) return;


        if(readyList.Count > 0)
        {
            var thread = new Thread(ThreadLoop);

            lock (mlock)
            {
                runningList.Add(thread, null);
            }

            thread.Start();
        }
    }

    /// <summary>
    /// 是否有网络
    /// </summary>
    /// <returns></returns>
    private bool CheckNetworkActive()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) // 没有网络
        {
            return false;
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) // 234G网络
        {
            return true;
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) // wifi网络
        {
            return true;
        }

        return false;
    }

    public void Update()
    {
        UpdateComplete();
        UpdateProgress();
        UpdateError();
        UpdateThread();
    }
}
