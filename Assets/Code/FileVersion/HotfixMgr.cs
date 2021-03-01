using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 热更管理器
/// </summary>
public class HotfixMgr
{
    public enum HotfixState
    {
        None,  //正在为您初始化资源包
        ReadLocalVersion, //读取本地版本
        CheckCDNVersion,  //获取资源cdn版本
        DownloadVersionFile, //下载版本文件
        CompareAssetVersion, //比对所有版本文件
        DownloadFiles, //下载需要的资源文件
        SaveVersion, //保存版本
        Finished,   //更新完毕,祝你游戏愉快

        RequsetUpdatePackage,  //请求换包
        RequestDownloadFiles, //请求开始下载文件
        RequestErrorRestart, // error重新开始下载
    }

    public delegate void HotfixCallback(HotfixState state, string msg, Action<bool> callback);


    private FileVersionMgr fileVersionMgr;

    private DownloadMgr downloadMgr;

    private HotfixState currentState;

    private HotfixCallback hotfixCallback;

    private VersionCode clientVersion;

    private VersionCode serverVersion;

    public HotfixState CurrentState
    {
        get { return currentState; }
    }

    public VersionCode CurrentVersion
    {
        get { return clientVersion; }
    }

    public void RegisterRequsetCallback(HotfixCallback callback)
    {
        hotfixCallback = callback;
    }

    //启动前，请先调用RegisterRequsetCallback注册回调
    public void Start()
    {
        downloadMgr = ServiceLocator.Resolve<DownloadMgr>();

        fileVersionMgr = ServiceLocator.Resolve<FileVersionMgr>();

        if (hotfixCallback == null)
        {
            Debug.LogError("Null Error is _currentState");
            return;
        }

#if UNIRY_EDITOR
        //编辑器状态，不热更
        Finished();
#else
        ReadLocalVersion();
#endif
    }

    /// <summary>
    /// 读取本地版本号
    /// </summary>
    private void ReadLocalVersion()
    {
        currentState = HotfixState.ReadLocalVersion;

        var versionPath = GPath.PersistentAssetsPath + GPath.VersionFileName;

        int version;

        if (File.Exists(versionPath))
        {
            version = int.Parse(File.ReadAllText(versionPath));
        }
        else
        {
            version = int.Parse(Resources.Load<TextAsset>(GPath.VersionFileName).text);
        }

       clientVersion.Version = version;
    }

    /// <summary>
    /// 读取服务端版本号
    /// </summary>
    private void CheckCDNVersion()
    {
        DownloadMgr.Inst.ClearAllDownloads();

        currentState = HotfixState.CheckCDNVersion;

        string versionServerFile = GPath.PersistentAssetsPath + "versionServer.txt";

        if (File.Exists(versionServerFile)) File.Delete(versionServerFile);

        var versionUnit = new DownloadUnit()
        {
            Name = "version.txt",
            DownUrl = GPath.CDNUrl + GPath.VersionFileName,
            SavePath = versionServerFile
        };

        versionUnit.ErrorFun = (DownloadUnit downUnit, string msg) =>
        {
            Debug.LogWarning("CheckAssetVersion Download Error " + msg + "\n" + downUnit.DownUrl);
            DownloadMgr.Inst.DeleteDownload(versionUnit);
            UpdateError();
        };

        versionUnit.CompleteFun = (DownloadUnit downUnit) =>
        {
            if (!File.Exists(versionServerFile)) // 文件不存在，重新下载
            {
                UpdateError();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
                return;
            }

            string versionStr = File.ReadAllText(versionServerFile);

            if (!int.TryParse(versionStr, out int curVersion))
            {
                Debug.LogError("CheckAssetVersion version Error " + versionStr);
                UpdateError();
                return;
            }

            serverVersion.Version = curVersion;

            Debug.Log("本地版本:" + clientVersion + " 服务器版本：" + curVersion);

            if (serverVersion.MidVer < clientVersion.MidVer)
            {
                UpdateError();
            }
            else if (serverVersion.MidVer > clientVersion.MidVer) // 换包
            {
                hotfixCallback(HotfixState.RequestErrorRestart, "", (run) =>
                {

                });
            }
            else if (serverVersion.MinVer < clientVersion.MinVer)
            {
                Finished();
            }
            else DownloadVersionFile();
        };

        downloadMgr.DownloadAsync(versionUnit);
    }

    /// <summary>
    /// 下载版本文件列表
    /// </summary>
    private void DownloadVersionFile()
    {
        currentState = HotfixState.DownloadVersionFile;

        string versionListServerFile = GPath.PersistentAssetsPath + serverVersion.ToString() + ".txt";
        if (File.Exists(versionListServerFile)) File.Delete(versionListServerFile);

        DownloadUnit versionListUnit = new DownloadUnit();
        versionListUnit.Name = serverVersion.ToString() + ".txt";
        versionListUnit.DownUrl = GPath.CDNUrl + versionListUnit.Name;
        versionListUnit.SavePath = versionListServerFile;
        versionListUnit.ErrorFun = (DownloadUnit downUnit, string msg) =>
        {
            Debug.LogWarning("CompareVersion Download Error " + msg + "\n" + downUnit.DownUrl);
            DownloadMgr.Inst.DeleteDownload(versionListUnit);
            UpdateError();
        };
        versionListUnit.CompleteFun = (DownloadUnit downUnit) =>
        {
            if (!File.Exists(versionListServerFile))
            {//文件不存在，重新下载
                UpdateError();
                return;
            }

            fileVersionMgr.InitVersionFile(versionListServerFile);

            var updateList = fileVersionMgr.FindUpdateFiles(clientVersion.Version);

            Debug.Log("版本文件数量:" + updateList.Count);
            if (updateList.Count > 0) StartDownloadList(updateList);
            else SaveVersion();
        };

        DownloadMgr.Inst.DownloadAsync(versionListUnit);
        Debug.Log("版本文件url:" + versionListUnit.DownUrl);
    }

    /// <summary>
    /// 下载版本资源
    /// </summary>
    /// <param name="updateList"></param>
    private void StartDownloadList(List<FileVersionData> updateList)
    {
        currentState = HotfixState.CompareAssetVersion;

        string saveRootPath = GPath.PersistentAssetsPath;
        string urlRootPath = GPath.CDNUrl;

        List<DownloadUnit> downloadList = new List<DownloadUnit>();
        Dictionary<string, int> downloadSizeList = new Dictionary<string, int>();
        int downloadCount = updateList.Count;
        int downloadMaxCount = downloadCount;

        int existAllSize = 0;
        int totalAllSize = 0;

        int downloadedFileSizes = 0;
        int downloadAllFileSize = 0;

        foreach (var fileData in updateList)
        {
            string savePath = saveRootPath + fileData.Name;
            if (File.Exists(savePath))
            {
                FileInfo fi = new FileInfo(savePath);
                if (fileData.Size == (int)fi.Length)
                {
                    //长度相等，可能是已经下载的
                    //downloadNameList.Add(fileData.Name, fileData.Name);
                    //existAllName += fileData.Name;
                }
                else
                {//长度不相等，需要重新下载
                    //Utils.Log("StartDownloadList Delete fileData.Size="+ fileData.Size + " fi.Length="+ fi.Length);
                    fi.Delete();
                    downloadSizeList.Add(fileData.Name, 0);
                    totalAllSize += fileData.Size;
                }
            }
            else
            {
                downloadSizeList.Add(fileData.Name, 0);
                totalAllSize += fileData.Size;
            }

            DownloadUnit downloadUnit = new DownloadUnit()
            {
                Name = fileData.Name,
                DownUrl = urlRootPath + fileData.Version + "/Assets/" + fileData.Name,
                Size = fileData.Size,
                Md5 = fileData.Md5
            };

            downloadUnit.ErrorFun = (DownloadUnit downUnit, string msg) =>
            {
                string errorMgs = "StartDownloadList Error " + downUnit.DownUrl + " " + msg + "\n";
                Debug.LogWarning(errorMgs);
            };

            downloadUnit.ProgressFun = (DownloadUnit downUnit, int curSize, int allSize) =>
            {
                downloadedFileSizes += curSize - downloadSizeList[downUnit.Name];
                downloadSizeList[downUnit.Name] = curSize;
            };

            downloadUnit.CompleteFun = (DownloadUnit downUnit) =>
            {
                downloadCount--;

                int percent = (downloadMaxCount - downloadCount) * 10 / downloadMaxCount;

                if (downloadCount == 0)
                {//下载完成
                    SaveVersion();
                }
            };

            downloadList.Add(downloadUnit);
        }

        downloadedFileSizes = 0;
        downloadAllFileSize = totalAllSize;

        //如果文件都存在，用已下载的作为长度，概率极低，为了表现进度特殊处理
        if (totalAllSize == 0)
        {
            downloadedFileSizes = 1;
            downloadAllFileSize = 1;
        }

        Debug.Log("下载文件总大小:" + totalAllSize);

        Action downloadFun = () =>
        {
            currentState = HotfixState.DownloadFiles;
            foreach (var downUnit in downloadList)
            {
                downloadMgr.DownloadAsync(downUnit);
            }
        };

        if (totalAllSize < 1024 * 1024) //<1MB
        {
            downloadFun();
        }
        else
        {
            hotfixCallback(HotfixState.RequestDownloadFiles, "游戏需要更新部分资源(" +
                (totalAllSize / 1024 / 1024) + "M),建议您在无线局域网环境下更新", (run) =>
                {
                    if (run == true) downloadFun();
                });
        }

    }

    private void SaveVersion()
    {
        currentState = HotfixState.SaveVersion;

        clientVersion.Version = serverVersion.Version + 1;

        string versionPath = GPath.PersistentAssetsPath + GPath.VersionFileName;

        File.WriteAllText(versionPath, clientVersion.Version.ToString());
    }

    public void Finished()
    {
        currentState = HotfixState.Finished;

        hotfixCallback(HotfixState.Finished, "", null);
    }

    private void UpdateError(string msg = "")
    {
        hotfixCallback(HotfixState.RequestErrorRestart, msg, (run) =>
        {
            if (run) Start();
        });
    }
}
