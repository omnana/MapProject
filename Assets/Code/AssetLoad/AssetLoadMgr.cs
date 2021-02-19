using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public enum AssetObjStatus
{
    None = 0,
    Loading = 1, // 正在加载
    Loaded = 2, // 加载完成
    Unload = 3, // 待卸载
}

public class AssetLoadMgr
{

    public static AssetLoadMgr Instance { get; private set; } = new AssetLoadMgr();

    public AssetBundleMgr AssetBundleMgr { get; set; }

    public EditorAssetLoadMgr EditorAssetLoadMgr { get; set; }

    public ResourcesLoadMgr ResourcesLoadMgr { get; set; }

    private Dictionary<AssetObjStatus, Dictionary<string, AssetObject>> assetObjDic = new Dictionary<AssetObjStatus, Dictionary<string, AssetObject>>();

    private List<AssetObject> loadedAsyncList; // 异步加载队列，延迟回调

    private List<AssetObject> tempLoadeds;

    private Dictionary<int, AssetObject> goInstanceIDList; //创建的实例对应的asset

    private int loadingIntervalCount;

    public AssetLoadMgr()
    {
        AssetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();

        EditorAssetLoadMgr = ServiceLocator.Resolve<EditorAssetLoadMgr>();

        ResourcesLoadMgr = ServiceLocator.Resolve<ResourcesLoadMgr>();

        assetObjDic = new Dictionary<AssetObjStatus, Dictionary<string, AssetObject>>()
        {
            {AssetObjStatus.Loading, new Dictionary<string, AssetObject>() },
            {AssetObjStatus.Loaded, new Dictionary<string, AssetObject>() },
            {AssetObjStatus.Unload, new Dictionary<string, AssetObject>() },
        };

        tempLoadeds = new List<AssetObject>();

#if UNITY_EDITOR
        EditorAssetLoadMgr.Init();
#else
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public bool IsAssetExist(string assetName)
    {
#if UNITY_EDITOR
        return EditorAssetLoadMgr.IsFileExist(assetName);
#else
        if (ResourcesLoadMgr.IsFileExist(assetName)) return true;
        return AssetBundleMgr.IsFileExist(assetName);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    public void PreLoad(string assetName)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private AssetObject CreateAssetObj(string assetName)
    {
        var assetObj = new AssetObject()
        {
            AssetName = assetName,
            RefCount = 1
        };

#if UNITY_EDITOR
        assetObj.Asset = EditorAssetLoadMgr.LoadSync(assetName);
#else
        if (AssetBundleMgr.IsFileExist(assetName))
        {
            assetObj.IsAbLoad = true;
            var ab = AssetBundleMgr.LoadSync(assetName);
            assetObj.Asset = ab.Main.LoadAsset(ab.Main.GetAllAssetNames()[0]);
            // 异步转同步，需要卸载异步的引用计数
            //AssetBundleMgr.Unload(assetName);
        }
        else
        {
            assetObj.IsAbLoad = false;
            assetObj.Asset = ResourcesLoadMgr.LoadSync(assetName);
        }
#endif
        return assetObj;
    }

    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="assetName"></param>
    public object LoadSync(string assetName)
    {
        if(!IsAssetExist(assetName))
        {
            return null;
        }

        var assetObj = GetAssetObj(assetName);

        var status = GetAssetObjStatus(assetName);

        if (status == AssetObjStatus.Loaded)
        {
            assetObj.RefCount++;

            return assetObj.Asset;
        }

        if (assetObj == null || status == AssetObjStatus.None)
        {
            assetObj = CreateAssetObj(assetName);
        }

        if (status == AssetObjStatus.Loading)
        {
#if UNITY_EDITOR
            assetObj.Asset = EditorAssetLoadMgr.LoadSync(assetName);
#else
            if(assetObj.IsAbLoad)
            {
                var ab = AssetBundleMgr.LoadSync(assetName);

                assetObj.Asset = ab.Main;

                // 异步转同步，需要卸载异步的引用计数
                //AssetBundleMgr.Unload(assetName);
            }
            else
            {
                assetObj.Asset = ResourcesLoadMgr.LoadSync(assetName);
            }
#endif

            if(assetObj.Asset == null)
            {
                PutAssetObInDic(AssetObjStatus.None, assetObj);

                return null;
            }

            assetObj.InstanceID = assetObj.Asset.GetInstanceID();

            goInstanceIDList.Add(assetObj.InstanceID, assetObj);

            PutAssetObInDic(AssetObjStatus.Loaded, assetObj);

            assetObj.RefCount++;

            return assetObj.Asset;
        }

        return null;
    }


    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="assetName"></param>
    public void LoadAsync(string assetName, AssetsLoadCallback callFun)
    {
        AssetObject assetObj = GetAssetObj(assetName);

        var status = GetAssetObjStatus(assetName);

        if(assetObj == null || status == AssetObjStatus.None)
        {
            assetObj = new AssetObject() { AssetName = assetName, };

            assetObj.CallbackList.Add(callFun);

#if UNITY_EDITOR

            PutAssetObInDic(AssetObjStatus.Loading, assetObj);

            assetObj.Request = EditorAssetLoadMgr.LoadAsync(assetName);

            ///////////// else
            if (AssetBundleMgr.IsFileExist(assetName))
            {
                assetObj.IsAbLoad = true;

                PutAssetObInDic(AssetObjStatus.Loading, assetObj);

                AssetBundleMgr.LoadAsync(assetName, (ab) =>
                {
                    if (GetAssetObjStatus(assetName) == AssetObjStatus.Loading && assetObj.Request == null && assetObj.Asset == null)
                    {
                        assetObj.Request = ab.LoadAssetAsync(ab.GetAllAssetNames()[0]);
                    }
                });
            }
            else if (ResourcesLoadMgr.IsFileExist(assetName))
            {
                assetObj.IsAbLoad = false;

                PutAssetObInDic(AssetObjStatus.Loading, assetObj);

                assetObj.Request = ResourcesLoadMgr.LoadAsync(assetName);
            }
#else

#endif
        }

        assetObj.CallbackList.Add(callFun);

        if(status == AssetObjStatus.Loaded)
        {
            loadedAsyncList.Add(assetObj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    public void Unload(string assetName)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdatePreload()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateLoadAsync()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateLoading()
    {
        var loadingList = GetDic(AssetObjStatus.Loading);

        if (loadingList.Count == 0) return;

        //检测加载完的
        tempLoadeds.Clear();

        foreach (var assetObj in loadingList.Values)
        {
            if (assetObj.Request != null && assetObj.Request.isDone)
            {
#if UNITY_EDITOR
                assetObj.Asset = (assetObj.Request as EditorAssetRequest).asset;

                if (assetObj.Asset == null) // 提取的资源失败，从加载列表删除
                {
                    PutAssetObInDic(AssetObjStatus.None, assetObj);
                    loadingList.Remove(assetObj.AssetName);
                    Debug.LogError("AssetsLoadMgr assetObj._asset Null " + assetObj.AssetName);
                    break;
                }
#else
                if (assetObj.IsAbLoad)
                    assetObj.Asset = (assetObj.Request as AssetBundleRequest).asset;
                else
                    assetObj.Asset = (assetObj.Request as ResourceRequest).asset;
#endif
                assetObj.InstanceID = assetObj.Asset.GetInstanceID();

                goInstanceIDList.Add(assetObj.InstanceID, assetObj);

                assetObj.Request = null;

                tempLoadeds.Add(assetObj);
            }
        }

        //回调中有可能对loadingList进行操作，先移动
        foreach (var assetObj in tempLoadeds)
        {
            PutAssetObInDic(AssetObjStatus.Loaded, assetObj);

            loadingIntervalCount++; //统计本轮加载的数量

            // 先锁定回调数量，保证异步成立
            assetObj.LockCallbackCount = assetObj.CallbackList.Count;
        }

        foreach (var assetObj in tempLoadeds)
        {
            DoAssetCallback(assetObj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateUnload()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    public void Update()
    {
        UpdatePreload();
        UpdateLoadAsync();
        UpdateLoading();
        UpdateUnload();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetObject"></param>
    private void DoLoad(AssetObject assetObject)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetObject"></param>
    private void DoAssetCallback(AssetObject assetObject)
    {
        if (assetObject.CallbackList.Count == 0) return;

        int count = assetObject.LockCallbackCount; //先提取count，保证回调中有加载需求不加载

        for (int i = 0; i < count; i++)
        {
            if (assetObject.CallbackList[i] != null)
            {
                assetObject.RefCount++; //每次回调，引用计数+1

                try
                {
                    assetObject.CallbackList[i](assetObject.AssetName, assetObject.Asset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        assetObject.CallbackList.RemoveRange(0, count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private AssetObjStatus GetAssetObjStatus(string assetName)
    {
        if (assetObjDic[AssetObjStatus.Loaded].ContainsKey(assetName)) // 已加载
            return AssetObjStatus.Loaded;

        if (assetObjDic[AssetObjStatus.Loading].ContainsKey(assetName)) // 正在加载，异步改同步
            return AssetObjStatus.Loading;

        if (assetObjDic[AssetObjStatus.Unload].ContainsKey(assetName)) // 待卸载
            return AssetObjStatus.Unload;

        return AssetObjStatus.None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private AssetObject GetAssetObj(string assetName)
    {
        if (assetObjDic[AssetObjStatus.Loaded].ContainsKey(assetName)) // 已加载
            return assetObjDic[AssetObjStatus.Loaded][assetName];

        if (assetObjDic[AssetObjStatus.Loading].ContainsKey(assetName)) // 正在加载，异步改同步
            return assetObjDic[AssetObjStatus.Loading][assetName];

        if (assetObjDic[AssetObjStatus.Unload].ContainsKey(assetName)) // 待卸载
            return assetObjDic[AssetObjStatus.Unload][assetName];

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private Dictionary<string, AssetObject> GetDic(AssetObjStatus status)
    {
        return status == AssetObjStatus.None ? null : assetObjDic[status];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="status"></param>
    /// <param name="obj"></param>
    private void PutAssetObInDic(AssetObjStatus status, AssetObject obj)
    {
        var assetName = obj.AssetName;

        var curStatus = GetAssetObjStatus(assetName);

        if (status == AssetObjStatus.None)
        {
            GetDic(curStatus).Remove(assetName);

            return;
        }

        // 清除上一个状态
        if (curStatus != AssetObjStatus.None) GetDic(curStatus).Remove(assetName);

        if (status != AssetObjStatus.None) GetDic(status).Add(obj.AssetName, obj);
    }
}
