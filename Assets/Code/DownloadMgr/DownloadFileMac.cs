using UnityEngine;
using System.Net;
using System.IO;
using System;

public enum DownloadMacState
{
    None,
    ResetSize,
    Download,
    Md5,
    Complete,
    Error,
}

public class DownloadFileMac
{
    private const int OneReadLen = 16 * 1024;       // 一次读取长度 16384 = 16*kb

    private const int Md5ReadLen = 16 * 1024;       // 一次读取长度 16384 = 16*kb

    private const int ReadWriteTimeOut = 2 * 1000;  // 超时等待时间

    private const int TimeOutWait = 5 * 1000;       // 超时等待时间

    public DownloadUnit DownUnit { get; private set; }

    public DownloadMacState State = DownloadMacState.None;

    public string Error { get; set; }

    public int CurSize { get; private set; }

    public int AllSize { get; private set; }

    public int TryCount { get; private set; }

    public DownloadFileMac(DownloadUnit unit)
    {
        DownUnit = unit;
    }

    public void Run()
    {
        TryCount++;

        State = DownloadMacState.ResetSize;

        if (!ResetSize()) return;

        State = DownloadMacState.Download;

        if (!DownLoad()) return;

        State = DownloadMacState.Md5;

        if (!CheckMd5()) // 校验失败，重下一次
        {
            State = DownloadMacState.Download;

            if (!DownLoad()) return;

            State = DownloadMacState.Md5;

            if (!CheckMd5()) return; // 两次都失败，文件有问题
        }

        State = DownloadMacState.Complete;
    }

    /// <summary>
    /// 重置文件长度
    /// </summary>
    /// <returns></returns>
    private bool ResetSize()
    {
        if(DownUnit.Size <= 0)
        {
            DownUnit.Size = GetWebFileSize(DownUnit.DownUrl);

            if (DownUnit.Size <= 0) return false;
        }

        CurSize = 0;

        AllSize = DownUnit.Size;

        return true;
    }


    private bool CheckMd5()
    {
        if (string.IsNullOrEmpty(DownUnit.Md5)) return true; //不做校验，默认成功

        string md5 = MD5Helper.GetMD5HashFromFile(DownUnit.SavePath);

        if (md5 != DownUnit.Md5)
        {
            File.Delete(DownUnit.SavePath);

            //ThreadDebugLog.Log("文件MD5校验出错：" + Unit.name);

            State = DownloadMacState.Error;

            Error = "Check MD5 Error ";

            return false;
        }

        return true;
    }

    private bool DownLoad()
    {
        // 打开上次下载的文件
        long startPos = 0;

        var tempFile = DownUnit.SavePath + ".temp";

        FileStream fs = null;

        if (File.Exists(DownUnit.SavePath)) // 文件已存在，跳过
        {
            CurSize = DownUnit.Size;

            return true;
        }
        else if (File.Exists(tempFile)) // 以及有部分下载到本地
        {
            fs = File.OpenWrite(tempFile);

            startPos = fs.Length;

            fs.Seek(startPos, SeekOrigin.Current); // 移动文件流中的当前指针

            if (startPos == DownUnit.Size) // 文件已经下载完，没改名字，结束
            {
                WriteEnd(fs, tempFile);

                CurSize = (int)startPos;

                return true;
            }
        }
        else
        {
            var direName = Path.GetDirectoryName(tempFile);

            if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);

            fs = new FileStream(tempFile, FileMode.Create);
        }

        // 下载逻辑

        HttpWebRequest request = null;

        HttpWebResponse respone = null;

        Stream ns = null;

        try
        {
            request = WebRequest.Create(DownUnit.DownUrl) as HttpWebRequest;

            request.ReadWriteTimeout = ReadWriteTimeOut;

            request.Timeout = TimeOutWait;

            // 断点续传
            if (startPos > 0) request.AddRange((int)startPos); // 设置Range值，断点续传

            respone = (HttpWebResponse)request.GetResponse();

            ns = respone.GetResponseStream();

            ns.ReadTimeout = TimeOutWait;

            var totalSize = respone.ContentLength;

            var curSize = startPos;

            if(curSize == totalSize)
            {
                WriteEnd(fs, tempFile);

                CurSize = (int)startPos;
            }
            else
            {
                var bytes = new byte[OneReadLen];

                var readSize = ns.Read(bytes, 0, OneReadLen); // 读取第一份数据

                while(readSize > 0)
                {
                    fs.Write(bytes, 0, readSize); // 将下载到的数据写入临时文件

                    curSize += readSize;

                    // 判断是否下载完成
                    // 下载完成将temp文件，改成正式文件
                    if (curSize == totalSize)
                    {
                        WriteEnd(fs, tempFile);
                    }

                    // 回调一下
                    CurSize = (int)curSize;

                    // 往下继续读取
                    readSize = ns.Read(bytes, 0, OneReadLen);
                }
            }
        }
        catch (Exception ex)
        {
            //下载失败，删除临时文件
            if (fs != null) { fs.Flush(); fs.Close(); fs = null; }

            if (File.Exists(tempFile))
                File.Delete(tempFile);

            if (File.Exists(DownUnit.SavePath))
                File.Delete(DownUnit.SavePath);

            State = DownloadMacState.Error;

            Error = string.Format("Download Error : {0}", ex.Message);

            Debug.LogError(Error);
        }
        finally
        {
            if (fs != null) { fs.Flush(); fs.Close(); fs = null; }

            if (ns != null) { ns.Close(); ns = null; }

            if (respone != null) { respone.Close(); respone = null; }

            if (request != null) { request.Abort(); request = null; }
        }

        if (State == DownloadMacState.Error) return false;

        return true;
    }


    private void WriteEnd(FileStream fs, string tempFile)
    {
        fs.Flush();

        fs.Close();

        fs = null;

        if (File.Exists(DownUnit.SavePath)) File.Delete(DownUnit.SavePath);

        File.Move(tempFile, DownUnit.SavePath);
    }

    private int GetWebFileSize(string url)
    {
        HttpWebRequest request = null;
        WebResponse respone = null;
        int length = 0;
        try
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = TimeOutWait;
            request.ReadWriteTimeout = ReadWriteTimeOut;
            //向服务器请求，获得服务器回应数据流
            respone = request.GetResponse();
            length = (int)respone.ContentLength;
        }
        catch (WebException e)
        {
            Debug.Log("获取文件长度出错：" + e.Message);
            State = DownloadMacState.Error;
            Error = "Request File Length Error " + e.Message;
        }
        finally
        {
            if (respone != null) { respone.Close(); respone = null; }
            if (request != null) { request.Abort(); request = null; }
        }
        return length;
    }

    /// <summary>
    /// 防止失败频繁回调，只在特定次数回调
    /// </summary>
    /// <returns></returns>
    public bool IsNeedErrorCall()
    {
        if (TryCount == 3 || TryCount == 10 || TryCount == 100)
            return true;

        return false;
    }
}
