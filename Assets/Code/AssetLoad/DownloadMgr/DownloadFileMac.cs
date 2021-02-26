using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

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

        //State = DownloadMacState.ResetSize;

        //if (!ResetSize()) return;

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

            if (DownUnit.Size == 0) return false;
        }

        CurSize = 0;

        AllSize = DownUnit.Size;

        return true;
    }


    private bool CheckMd5()
    {
        if (string.IsNullOrEmpty(DownUnit.Md5)) return true; //不做校验，默认成功

        string md5 = GetMD5HashFromFile(DownUnit.SavePath);

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
        //打开上次下载的文件
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
                fs.Flush();

                fs.Close();

                fs = null;

                if (File.Exists(DownUnit.SavePath)) File.Delete(DownUnit.SavePath);

                File.Move(tempFile, DownUnit.SavePath);

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

            // 向服务器请求，获得服务器回应数据流
            if (startPos > 0) request.AddRange((int)startPos); // 设置Range值，断点续传

            respone = (HttpWebResponse)request.GetResponse();

            ns = respone.GetResponseStream();

            ns.ReadTimeout = TimeOutWait;

            var totalSize = respone.ContentLength;

            var curSize = startPos;

            if(curSize == totalSize)
            {
                fs.Flush();

                fs.Close();

                if (File.Exists(DownUnit.SavePath)) File.Delete(DownUnit.SavePath);

                File.Move(tempFile, DownUnit.SavePath);

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
                        fs.Flush();

                        fs.Close();

                        fs = null;

                        if (File.Exists(DownUnit.SavePath)) File.Delete(DownUnit.SavePath);

                        File.Move(tempFile, DownUnit.SavePath);
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

            Error = string.Format("Download Error {0}", ex.Message);
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

    private string GetMD5HashFromFile(string fileName)
    {
        byte[] buffer = new byte[Md5ReadLen];

        int readLength = 0;//每次读取长度

        var output = new byte[Md5ReadLen];

        using (Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0) // 计算MD5
                {
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }

                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);

                var retVal = hashAlgorithm.Hash;

                var sb = new System.Text.StringBuilder(32);

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                hashAlgorithm.Clear();

                inputStream.Close();

                return sb.ToString();
            }
        }
    }

    private int GetWebFileSize(string url)
    {
        return 0;
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
