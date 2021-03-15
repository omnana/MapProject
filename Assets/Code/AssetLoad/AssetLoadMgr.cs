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

public class AssetLoadMgr : MonoBehaviour
{
    public AssetBundleMgr AssetBundleMgr { get; set; }

#if UNITY_EDITOR 
    public EditorAssetLoadMgr EditorAssetLoadMgr { get; set; }
#endif

    public ResourcesLoadMgr ResourcesLoadMgr { get; set; }

    private Dictionary<AssetObjStatus, Dictionary<string, AssetObject>> assetObjDic = new Dictionary<AssetObjStatus, Dictionary<string, AssetObject>>();

    private List<AssetObject> loadedAsyncList; // 异步加载队列，延迟回调

    private List<AssetObject> tempLoadeds;

    private Dictionary<int, AssetObject> goInstanceIDList; //创建的实例对应的asset

    private Queue<PreloadAssetObject> preloadedAsyncList; // 预加载队列

    private int loadingIntervalCount;

    private const int UNLOAD_DELAY_TICK_BASE = 60;

    private const int LOADING_INTERVAL_MAX_COUNT = 15;

    private void Awake()
    {
        AssetBundleMgr = Singleton<AssetBundleMgr>.GetInstance();

#if UNITY_EDITOR 
        EditorAssetLoadMgr = Singleton<EditorAssetLoadMgr>.GetInstance();
#endif

        ResourcesLoadMgr = Singleton<ResourcesLoadMgr>.GetInstance();

        assetObjDic = new Dictionary<AssetObjStatus, Dictionary<string, AssetObject>>()
        {
            {AssetObjStatus.Loading, new Dictionary<string, AssetObject>() },
            {AssetObjStatus.Loaded, new Dictionary<string, AssetObject>() },
            {AssetObjStatus.Unload, new Dictionary<string, AssetObject>() },
        };

        tempLoadeds = new List<AssetObject>();

        loadedAsyncList = new List<AssetObject>();

        goInstanceIDList = new Dictionary<int, AssetObject>();


        preloadedAsyncList = new Queue<PreloadAssetObject>();

#if UNITY_EDITOR 
        EditorAssetLoadMgr.Init();
#endif
        ResourcesLoadMgr.Init();

        AssetBundleMgr.Init();
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
    /// 同步加载
    /// </summary>
    /// <param name="assetName"></param>
    public Object LoadSync(string assetName)
    {
        assetName = assetName.ToLower();

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

        if (status == AssetObjStatus.Loading)
        {
            if (assetObj.Request == null)
            {
                if (assetObj.Request is AssetBundleRequest)
                {
                    assetObj.Asset = (assetObj.Request as AssetBundleRequest).asset;
                }
                else if (assetObj.Request is ResourceRequest)
                {
                    assetObj.Asset = (assetObj.Request as ResourceRequest).asset;
                }
                else if (assetObj.Request is EditorAssetRequest)
                {
                    assetObj.Asset = (assetObj.Request as EditorAssetRequest).asset;
                }
            }
            else
            {
#if UNITY_EDITOR 
                assetObj.Asset = EditorAssetLoadMgr.LoadSync(assetName);
#else
                if (assetObj.IsAbLoad)
            {
                var ab = AssetBundleMgr.LoadSync(assetName);

                assetObj.Asset = ab.AssetBundle;

                // 异步转同步，需要卸载异步的引用计数
                //AssetBundleMgr.Unload(assetName);
            }
            else
            {
                assetObj.Asset = ResourcesLoadMgr.LoadSync(assetName);
            }
#endif
            }

        }

        if (assetObj == null || status == AssetObjStatus.None)
        {
            assetObj = new AssetObject()
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
                assetObj.Asset = ab.AssetBundle.LoadAsset(ab.AssetBundle.GetAllAssetNames()[0]);
                // 异步转同步，需要卸载异步的引用计数
                //AssetBundleMgr.Unload(assetName);
            }
            else
            {
                assetObj.IsAbLoad = false;
                assetObj.Asset = ResourcesLoadMgr.LoadSync(assetName);
            }
#endif
        }

        if (assetObj.Asset == null)
        {
            PutAssetObInDic(AssetObjStatus.None, assetObj);

            Debug.LogError("asset is null -> " + assetObj.AssetName);

            return null;
        }

        assetObj.InstanceID = assetObj.Asset.GetInstanceID();

        goInstanceIDList.Add(assetObj.InstanceID, assetObj);

        PutAssetObInDic(AssetObjStatus.Loaded, assetObj);

        assetObj.RefCount++;

        return assetObj.Asset;
    }


    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="assetName"></param>
    public void LoadAsync(string assetName, AssetsLoadCallback callFun)
    {
        assetName = assetName.ToLower();

        AssetObject assetObj = GetAssetObj(assetName);

        var status = GetAssetObjStatus(assetName);

        if(assetObj == null || status == AssetObjStatus.None)
        {
            assetObj = new AssetObject() { AssetName = assetName, };

            PutAssetObInDic(AssetObjStatus.Loading, assetObj);

#if UNITY_EDITOR 
            assetObj.Asset = EditorAssetLoadMgr.LoadSync(assetName);
#else
            if (AssetBundleMgr.IsFileExist(assetName))
            {
                assetObj.IsAbLoad = true;

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

                assetObj.Request = ResourcesLoadMgr.LoadAsync(assetName);
            }
#endif
        }

        assetObj.CallbackList.Add(callFun);

        if(status == AssetObjStatus.Loaded)
        {
            loadedAsyncList.Add(assetObj);
        }
    }

    /// <summary>
    /// 预加载，isWeak弱引用，true为使用过后会销毁，为false将不会销毁，慎用
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_isWeak"></param>
    public void PreLoad(string assetName, bool isWeak = true)
    {
        AssetObject assetObj = null;

        var status = GetAssetObjStatus(assetName);

        assetObj = GetDic(status)[assetName];

        if (assetObj != null)
        {
            assetObj.IsWeak = isWeak;

            var unLoadList = GetDic(AssetObjStatus.Unload);

            if (isWeak && assetObj.RefCount == 0)
            {
                PutAssetObInDic(AssetObjStatus.Unload, assetObj);
            }

            return;
        }

        preloadedAsyncList.Enqueue(new PreloadAssetObject()
        {
            AssetName = assetName,
            IsWeak = isWeak
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    public void Unload(Object obj)
    {
        if (obj == null) return;

        var instanceID = obj.GetInstanceID();

        if(!goInstanceIDList.ContainsKey(instanceID)) // 非从本类创建的资源，直接销毁即可
        {
            if (obj is GameObject)
            {
                Object.Destroy(obj);
            }
#if UNITY_EDITOR 
            else if(UnityEditor.EditorApplication.isPlaying)
            {

            }
#else
#endif
            return;
        }

        var assetObj = goInstanceIDList[instanceID];

        if (assetObj.InstanceID == instanceID) // obj不是GameObject，不销毁
        {
            assetObj.RefCount--;
        }
        else // error
        {
            string errormsg = string.Format("AssetsLoadMgr Destroy error ! assetName:{0}", assetObj.AssetName);
            Debug.LogError(errormsg);
            return;
        }

        if (assetObj.RefCount < 0)
        {
            string errormsg = string.Format("AssetsLoadMgr Destroy refCount error ! assetName:{0}", assetObj.AssetName);
            Debug.LogError(errormsg);
            return;
        }

        var unLoadList = GetDic(AssetObjStatus.Unload);

        if (assetObj.RefCount == 0 && !unLoadList.ContainsKey(assetObj.AssetName))
        {
            assetObj.UnloadTick = UNLOAD_DELAY_TICK_BASE + unLoadList.Count;

            unLoadList.Add(assetObj.AssetName, assetObj);
        }
    }

    /// <summary>
    /// 预加载
    /// 每次只取一个加载，直到加载完成，再去下一个
    /// </summary>
    private void UpdatePreload()
    {
        var loadingList = GetDic(AssetObjStatus.Loading);

        // 加载队列空闲才需要预加载
        if (loadingList.Count > 0 || preloadedAsyncList.Count == 0) return;

        // 从队列支取处取出一个，异步加载，直到加载完，在取下一个
        var plAssetObj = preloadedAsyncList.Peek();

        var status = GetAssetObjStatus(plAssetObj.AssetName);

        var assetObj = GetAssetObj(plAssetObj.AssetName);

        if(assetObj != null)
        {
            assetObj.IsWeak = plAssetObj.IsWeak;
        }
        else
        {
            LoadAsync(plAssetObj.AssetName, null);
        }

        if (status == AssetObjStatus.Loaded)
        {
            preloadedAsyncList.Dequeue();
        }
    }

    /// <summary>
    /// 加载异步，当调用已加载的资源，仍希望异步回调
    /// </summary>
    private void UpdateLoadAsync()
    {
        if (loadedAsyncList.Count == 0) return;

        int count = loadedAsyncList.Count;

        for (int i = 0; i < count; i++)
        {
            //先锁定回调数量，保证异步成立
            loadedAsyncList[i].LockCallbackCount = loadedAsyncList[i].CallbackList.Count;
        }

        for (int i = 0; i < count; i++)
        {
            DoAssetCallback(loadedAsyncList[i]);
        }

        loadedAsyncList.RemoveRange(0, count);

        var loadingList = GetDic(AssetObjStatus.Loading);

        if (loadingList.Count == 0 && loadingIntervalCount > LOADING_INTERVAL_MAX_COUNT)
        {
            //在连续的大量加载后，强制调用一次gc
            loadingIntervalCount = 0;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
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
#if UNITY_EDITOR 
            if(assetObj.Asset != null)
            {
                assetObj.InstanceID = assetObj.Asset.GetInstanceID();

                goInstanceIDList.Add(assetObj.InstanceID, assetObj);

                tempLoadeds.Add(assetObj);
            }
#else
            if (assetObj.Request != null && assetObj.Request.isDone)
            {
                if (assetObj.IsAbLoad)
                    assetObj.Asset = (assetObj.Request as AssetBundleRequest).asset;
                else
                    assetObj.Asset = (assetObj.Request as ResourceRequest).asset;

                assetObj.InstanceID = assetObj.Asset.GetInstanceID();

                goInstanceIDList.Add(assetObj.InstanceID, assetObj);

                assetObj.Request = null;

                tempLoadeds.Add(assetObj);
            }
#endif
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
        //遍历卸载，延迟卸载
        var unloadList = GetDic(AssetObjStatus.Unload);

        if (unloadList.Count == 0) return;

        tempLoadeds.Clear();

        foreach (var assetObj in unloadList.Values)
        {
            if (assetObj.IsWeak && assetObj.RefCount == 0 && assetObj.CallbackList.Count == 0) 
            {
                // 引用计数为0，且没有需要回调的函数，销毁
                if (assetObj.UnloadTick < 0)
                {
                    DoUnLoad(assetObj);

                    tempLoadeds.Add(assetObj);
                }
                else assetObj.UnloadTick--;
            }

            if (assetObj.RefCount > 0 || !assetObj.IsWeak)
            {
                // 引用计数增加（销毁期间有加载）
                tempLoadeds.Add(assetObj);
            }
        }

        foreach (var assetObj in tempLoadeds)
        {
            PutAssetObInDic(AssetObjStatus.None, assetObj);
        }
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

#if UNITY_EDITOR 

#else
        AssetBundleMgr.Update();
#endif
    }

    /// <summary>
    /// 执行卸载
    /// </summary>
    /// <param name="assetObject"></param>
    private void DoUnLoad(AssetObject assetObject)
    {
#if UNITY_EDITOR 
        EditorAssetLoadMgr.Unload(assetObject.Asset);
#else
        if (assetObject.IsAbLoad)
            AssetBundleMgr.UnLoadAssetBundleAsync(assetObject.AssetName);
        else
            ResourcesLoadMgr.Unload(assetObject.Asset);
#endif

        assetObject.Asset = null;

        if (goInstanceIDList.ContainsKey(assetObject.InstanceID))
        {
            goInstanceIDList.Remove(assetObject.InstanceID);
        }
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

        if (assetObjDic[AssetObjStatus.Loaded].ContainsKey(assetName))
            assetObjDic[AssetObjStatus.Loaded].Remove(assetName);

        if (assetObjDic[AssetObjStatus.Loading].ContainsKey(assetName))
            assetObjDic[AssetObjStatus.Loading].Remove(assetName);

        if (assetObjDic[AssetObjStatus.Unload].ContainsKey(assetName))
            assetObjDic[AssetObjStatus.Unload].Remove(assetName);

        if (status == AssetObjStatus.None) return;

        if (assetObjDic.ContainsKey(status))
            assetObjDic[status].Add(obj.AssetName, obj);
    }
}
