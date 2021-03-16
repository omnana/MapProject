using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Omnana;
using System.IO;
using System.Net;
using UnityEngine.Networking;

/// <summary>
/// 断点续传测试案例
/// </summary>
public class DownloadTest : MonoBehaviour
{
    private byte[] serverFile;

    //private int OneReadLen = 160 * 1024 * 1024; // 16kb
    private int OneReadLen = 16 * 1024; // 16kb

    private const int ReadWriteTimeOut = 2 * 1000;  // 超时等待时间

    private const int TimeOutWait = 5 * 1000;       // 超时等待时间

    private int totalSize;

    // 第一步：将本地资源转化为字节
    // 第二步：模拟断电续传
    private void Awake()
    {
        var url = "http://www.downza.cn/iopdfbhjl/182843?module=soft&id=182843&token=35c7bff4b9c87301d1c4f16f88ef4545&isxzq=0";

        GetWebFileSize(url);
    }

    /// <summary>
    /// HttpWebRequest支持的断点续传，协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator Download()
    {
        totalSize = GetWebFileSize(GPath.CDNUrl + "TeamCity-2020.2.exe");

        //var totalSize = GetWebFileSize(GPath.CDNUrl + "Cube.prefab");

        Debug.Log("totalSize = " + totalSize);

        var cdnUrl = GPath.CDNUrl + "TeamCity-2020.2.exe";

        var tempFile = Application.dataPath + "/DownloadTest/TeamCity-2020.2.temp";

        var savePath = Application.dataPath + "/DownloadTest/TeamCity-2020.2.exe";

        FileStream fs = null;

        long startPos = 0;

        if (File.Exists(tempFile))
        {
            fs = File.OpenWrite(tempFile);

            startPos = fs.Length;

            fs.Seek(startPos, SeekOrigin.Current); // 移动文件流中的当前指针

            if (startPos == totalSize)
            {
                fs.Flush();

                fs.Close();

                fs = null;

                if (File.Exists(savePath)) File.Delete(savePath);

                File.Move(tempFile, savePath);
            }
        }
        else
        {
            var direName = Path.GetDirectoryName(tempFile);

            if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);

            fs = new FileStream(tempFile, FileMode.Create);
        }

        HttpWebRequest request = null;

        HttpWebResponse respone = null;

        Stream ns = null;

        {
            request = WebRequest.Create(cdnUrl) as HttpWebRequest;

            request.Timeout = TimeOutWait;

            request.ReadWriteTimeout = ReadWriteTimeOut;

            // 断点续传
            if (startPos > 0) request.AddRange((int)startPos);

            respone = (HttpWebResponse)request.GetResponse();

            ns = respone.GetResponseStream();

            ns.ReadTimeout = TimeOutWait;

            var curTotalSize = respone.ContentLength;

            Debug.Log("curDownloadSize = " + curTotalSize);

            var curSize = startPos;

            if (curSize == curTotalSize)
            {
                fs.Flush();

                fs.Close();

                fs = null;

                Debug.Log("finish = " + curSize);

                if (File.Exists(savePath)) File.Delete(savePath);

                File.Move(tempFile, savePath);
            }
            else
            {
                var bytes = new byte[OneReadLen];

                var readSize = ns.Read(bytes, 0, OneReadLen); // 读取第一份数据

                while (readSize > 0)
                {
                    Debug.Log("curSize = " + curSize);

                    fs.Write(bytes, 0, readSize); // 将下载到的数据写入临时文件

                    curSize += readSize;

                    // 判断是否下载完成
                    // 下载完成将temp文件，改成正式文件
                    if (curSize == totalSize)
                    {
                        Debug.Log("finish = " + curSize);
                        fs.Flush();

                        fs.Close();

                        fs = null;

                        if (File.Exists(savePath)) File.Delete(savePath);

                        File.Move(tempFile, savePath);
                    }

                    // 往下继续读取
                    readSize = ns.Read(bytes, 0, OneReadLen);

                    yield return null;
                }
            }
        }
        //catch (WebException e)
        //{
        //    Debug.Log("获取文件长度出错：" + e.Message);
        //}
        //finally
        //{
        //    if (respone != null) { respone.Close(); respone = null; }
        //    if (request != null) { request.Abort(); request = null; }
        //}
    }

    private void Run()
    {
        //totalSize = GetWebFileSize(GPath.CDNUrl + "TeamCity-2020.2.exe");

        var totalSize = GetWebFileSize(GPath.CDNUrl + "1001000/Assets/Cube.prefab");

        var cdnUrl = GPath.CDNUrl + "TeamCity-2020.2.exe";

        var tempFile = Application.dataPath + "/DownloadTest/TeamCity-2020.2.temp";

        var savePath = Application.dataPath + "/DownloadTest/TeamCity-2020.2.exe";

        FileStream fs = null;

        long startPos = 0;

        if (File.Exists(tempFile))
        {
            fs = File.OpenWrite(tempFile);

            startPos = fs.Length;

            fs.Seek(startPos, SeekOrigin.Current); // 移动文件流中的当前指针

            if (startPos == totalSize)
            {
                fs.Flush();

                fs.Close();

                fs = null;

                if (File.Exists(savePath)) File.Delete(savePath);

                File.Move(tempFile, savePath);
            }
        }
        else
        {
            var direName = Path.GetDirectoryName(tempFile);

            if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);

            fs = new FileStream(tempFile, FileMode.Create);
        }

        HttpWebRequest request = null;

        HttpWebResponse respone = null;

        Stream ns = null;

        try
        {
            request = WebRequest.Create(cdnUrl) as HttpWebRequest;

            request.Timeout = TimeOutWait;

            request.ReadWriteTimeout = ReadWriteTimeOut;

            // 断点续传
            if (startPos > 0) request.AddRange((int)startPos);

            respone = (HttpWebResponse)request.GetResponse();

            ns = respone.GetResponseStream();

            ns.ReadTimeout = TimeOutWait;

            var curtotalSize = respone.ContentLength;

            Debug.Log("curDownloadSize = " + curtotalSize);

            var curSize = startPos;

            if (curSize == curtotalSize)
            {
                fs.Flush();

                fs.Close();

                fs = null;

                Debug.Log("finish = " + curSize);

                if (File.Exists(savePath)) File.Delete(savePath);

                File.Move(tempFile, savePath);
            }
            else
            {
                var bytes = new byte[OneReadLen];

                var readSize = ns.Read(bytes, 0, OneReadLen); // 读取第一份数据

                while (readSize > 0)
                {
                    fs.Write(bytes, 0, readSize); // 将下载到的数据写入临时文件

                    curSize += readSize;

                    // 判断是否下载完成
                    // 下载完成将temp文件，改成正式文件
                    if (curSize == totalSize)
                    {
                        Debug.Log("finish = " + curSize);
                        fs.Flush();

                        fs.Close();

                        fs = null;

                        if (File.Exists(savePath)) File.Delete(savePath);

                        File.Move(tempFile, savePath);
                    }

                    // 往下继续读取
                    readSize = ns.Read(bytes, 0, OneReadLen);
                }
            }
        }
        catch (WebException e)
        {
            Debug.Log("获取文件长度出错：" + e.Message);
        }
        finally
        {
            if (respone != null) { respone.Close(); respone = null; }
            if (request != null) { request.Abort(); request = null; }
        }
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
        }
        finally
        {
            if (respone != null) { respone.Close(); respone = null; }
            if (request != null) { request.Abort(); request = null; }
        }
        return length;
    }

}
