using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DonwloadErrorCallBack(DownloadUnit downUnit, string msg);

public delegate void DonwloadProgressCallBack(DownloadUnit downUnit, int curSize, int allSize);

public delegate void DonwloadCompleteCallBack(DownloadUnit downUnit);


public class DownloadUnit
{
    public string Name; //下载的文件，作为标识

    public string DownUrl; //远程地址

    public string SavePath; //本地地址

    public int Size; //文件长度,非必须

    public string Md5; //需要校验的md5，非必须

    public bool IsDelete; //用于清理正在下载的文件

    public DonwloadErrorCallBack ErrorFun;

    public DonwloadProgressCallBack ProgressFun;

    public DonwloadCompleteCallBack CompleteFun;
}
