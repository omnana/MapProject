using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AssetBundles
{
    public enum AbObjStatus
    {
        None = 0,
        Ready = 1, // 准备加载
        Loading = 2, // 正在加载
        Loaded = 3, // 加载完成
        Unload = 4, // 待卸载
    }

    /// <summary>
    /// 资源管理
    /// </summary>
    public class AssetBundleMgr : BaseCtrl
    {
        private Dictionary<AbObjStatus, Dictionary<string, AssetBundleObject>> queueDic;

        private Dictionary<string, string[]> dependsDataDic;

        private List<AssetBundleObject> tempList;

        private const int Max_Loading_Count = 10;

        public AssetBundleMgr()
        {
            queueDic = new Dictionary<AbObjStatus, Dictionary<string, AssetBundleObject>>()
            {
                { AbObjStatus.Ready, new Dictionary<string, AssetBundleObject>()}, // 准备加载
                { AbObjStatus.Loading, new Dictionary<string, AssetBundleObject>()}, // 正在加载
                { AbObjStatus.Loaded, new Dictionary<string, AssetBundleObject>()}, // 加载成功
                { AbObjStatus.Unload, new Dictionary<string, AssetBundleObject>()}, // 准备卸载
            };

            dependsDataDic = new Dictionary<string, string[]>();

            tempList = new List<AssetBundleObject>();
        }

        /// <summary>
        /// 是否有该ab包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public bool ContainsAbObj(string abName)
        {
            return GetQueue(AbObjStatus.Loaded).ContainsKey(abName) 
                || GetQueue(AbObjStatus.Loading).ContainsKey(abName) 
                || GetQueue(AbObjStatus.Ready).ContainsKey(abName);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public AssetBundleObject LoadSync(string abName)
        {
            return LoadAssetBundleSync(abName);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="abName"></param>
        public void LoadAsync(string abName, AssetBundleLoadCallBack callFun)
        {
            LoadAssetBundleAsync(abName, callFun);
        }

        /// <summary>
        /// 加载主 mainfest
        /// </summary>
        public void LoadMainfest()
        {
            dependsDataDic.Clear();

            var mainfestFile = Utility.GetAssetMainfestPath();

            if (!File.Exists(mainfestFile))
            {
                Debug.LogError("未加载到Mainfest!!!");
                return;
            }

            var ab = AssetBundle.LoadFromFile(mainfestFile);

            if (ab == null)
            {
                string errormsg = string.Format("LoadMainfest ab NULL error !");

                Debug.LogError(errormsg);

                return;
            }

            var mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            if (mainfest == null)
            {
                string errormsg = string.Format("LoadMainfest NULL error !");

                Debug.LogError(errormsg);

                return;
            }

            foreach (string assetName in mainfest.GetAllAssetBundles())
            {
                string hashName = assetName.Replace(".ab", "");

                string[] dps = mainfest.GetAllDependencies(assetName);

                for (int i = 0; i < dps.Length; i++)
                {
                    dps[i] = dps[i].Replace(".ab", "");
                }

                dependsDataDic.Add(hashName, dps);
            }

            ab.Unload(true);

            ab = null;

            Debug.Log("AssetBundleLoadMgr dependsCount=" + dependsDataDic.Count);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public AssetBundleObject LoadAssetBundleSync(string abName)
        {
            AssetBundleObject abObj = null;

            var status = GetAbObjStatus(abName);

            if (status == AbObjStatus.None) //  未加载
            {
                abObj = CreateAbObj(abName, false);

                DoLoad(abObj, false);

                PutAbObjInQueue(AbObjStatus.Loaded, abObj);

                return abObj;
            }

            abObj = GetAbObjFromQueue(abName);

            if (abObj == null)
            {
                Debug.Log("AbObj no found!!! -> " + abName);

                return null;
            }

            abObj.RefCount++;

            foreach (var o in abObj.Depends) // 加载依赖项
            {
                LoadAssetBundleSync(o.HashName);
            }

            PutAbObjInQueue(AbObjStatus.Loaded, abObj);

            if (status == AbObjStatus.Loaded) return abObj; // 已加载

            if (status == AbObjStatus.Ready)
            {
                DoLoad(abObj, false);
            }
            else if (status == AbObjStatus.Loading) // 强制异步为同步加载
            {
                ForceLoadAsyncToSync(abObj);
            }

            DoLoadedCallFun(abObj, false);

            return abObj;
        }

        /// <summary>
        // 异步加载
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="callFun"></param>
        public AssetBundleObject LoadAssetBundleAsync(string abName, AssetBundleLoadCallBack callFun)
        {
            AssetBundleObject abObj = null;

            var status = GetAbObjStatus(abName);

            //  未加载过
            if (status == AbObjStatus.None)
            {
                abObj = CreateAbObj(abName);

                abObj.CallFunList.Add(callFun);

                // 移入准备队列
                PutAbObjInQueue(AbObjStatus.Ready, abObj);

                return abObj;
            }

            abObj = GetAbObjFromQueue(abName);

            if (abObj == null)
            {
                Debug.Log("AbObj no found!!! -> " + abName);

                return null;
            }

            DoDependsRef(abObj);

            if (status == AbObjStatus.Loaded) // 已加载
            {
                callFun?.Invoke(abObj.Main);
            }
            else
            {
                abObj.CallFunList.Add(callFun);
            }

            return abObj;
        }

        /// <summary>
        /// 创建新的异步资源包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject CreateAbObj(string abName, bool isAsync = true)
        {
            var abObj = new AssetBundleObject()
            {
                RefCount = 1,

                HashName = abName,

                DependLoadingCount = 0,
            };

            if (dependsDataDic.ContainsKey(abName))
            {
                var dps = dependsDataDic[abName];

                if (dps != null && dps.Length > 0)
                {
                    // 异步加载，需要加载完所有依赖项
                    if (isAsync) abObj.DependLoadingCount = dps.Length;

                    AssetBundleObject dpObj = null;

                    foreach (var d in dps)
                    {
                        if (!isAsync) // 同步
                        {
                            dpObj = LoadAssetBundleSync(d);
                        }
                        else
                        {
                            dpObj = LoadAssetBundleAsync(d, (ab) =>
                            {
                                if (abObj.DependLoadingCount <= 0) return;

                                abObj.DependLoadingCount--;
                            });
                        }

                        if (dpObj == null)
                        {
                            Debug.Log("curAbObj is null!!!");
                        }
                        else
                        {
                            abObj.Depends.Add(dpObj);
                        }
                    }
                }
            }

            return abObj;
        }

        /// <summary>
        /// 计算某个资源包的引用计数
        /// </summary>
        /// <param name="abObj"></param>
        private void DoDependsRef(AssetBundleObject abObj)
        {
            abObj.RefCount++;

            if (abObj.Depends.Count == 0) return;

            foreach (var dpObj in abObj.Depends)
            {
                DoDependsRef(dpObj); //递归依赖项，加载完
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName"></param>
        public void UnLoadAssetBundleAsync(string abName)
        {
            var abObj = GetAbObjFromQueue(abName);

            if(abObj == null)
            {
                string errormsg = string.Format("UnLoadAssetbundle error ! assetName:{0}", abName);

                Debug.LogError(errormsg);

                return;
            }

            if (abObj.RefCount == 0)
            {
                string errormsg = string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", abName);

                Debug.LogError(errormsg);

                return;
            }

            abObj.RefCount--;

            foreach (var dpObj in abObj.Depends)
            {
                UnLoadAssetBundleAsync(dpObj.HashName);
            }

            if (abObj.RefCount == 0)
            {
                PutAbObjInQueue(AbObjStatus.Unload, abObj);
            }
        }

        /// <summary>
        /// 强制异步为同步加载
        /// </summary>
        /// <param name="abObj"></param>
        private void ForceLoadAsyncToSync(AssetBundleObject abObj)
        {
            if (abObj.Request != null)
            {
                abObj.Main = abObj.Request.assetBundle;

                abObj.Request = null;
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="abObj"></param>
        /// <param name="isAsync">是否异步</param>
        private void DoLoad(AssetBundleObject abObj, bool isAsync = true)
        {
            string dir = Utility.GetAssetBundlePath(abObj.HashName);

            if (isAsync)
            {
                abObj.Request = AssetBundle.LoadFromFileAsync(dir);
            }
            else
            {
                abObj.Main = AssetBundle.LoadFromFile(dir);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abObj"></param>
        private void DoUnLoad(AssetBundleObject abObj)
        {
            // 这里用true，卸载Asset内存，实现指定卸载
            if (abObj.Main == null)
            {
                string errormsg = string.Format("LoadAssetbundle DoUnload error ! assetName:{0}", abObj.HashName);

                Debug.LogError(errormsg);

                return;
            }

            abObj.Main.Unload(true);

            abObj.Main = null;
        }

        /// <summary>
        /// 加载资源后的回调，（暂未提供服务器下载资源）
        /// </summary>
        /// <param name="abObj"></param>
        /// <param name="isAsync"></param>
        private void DoLoadedCallFun(AssetBundleObject abObj, bool isAsync = true)
        {
            if (abObj.Main == null) // 如果还没加载到ab包, 网上下载，先同步下载
            {
            }

            if (abObj.Main == null) // 未同步下载到，直接移除
            {
                PutAbObjInQueue(AbObjStatus.None, abObj);
            }

            if(abObj.Main == null && isAsync) // 异步下载
            {

            }

            foreach(var callback in abObj.CallFunList)
            {
                callback?.Invoke(abObj.Main);
            }

            Debug.Log(abObj.HashName);

            abObj.CallFunList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AbObjStatus GetAbObjStatus(string abName)
        {
            if (queueDic[AbObjStatus.Loaded].ContainsKey(abName)) // 已加载
                return AbObjStatus.Loaded;

            if (queueDic[AbObjStatus.Loading].ContainsKey(abName)) // 正在加载，异步改同步
                return AbObjStatus.Loading;

            if (queueDic[AbObjStatus.Ready].ContainsKey(abName)) // 准备加载，直接同步加载
                return AbObjStatus.Ready;

            if (queueDic[AbObjStatus.Unload].ContainsKey(abName)) // 待卸载
                return AbObjStatus.Unload;

            return AbObjStatus.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject GetAbObjFromQueue(string abName)
        {
            if (GetQueue(AbObjStatus.Ready).ContainsKey(abName))
                return GetQueue(AbObjStatus.Ready)[abName];

            if (queueDic[AbObjStatus.Loading].ContainsKey(abName))
                return GetQueue(AbObjStatus.Loading)[abName];

            if (queueDic[AbObjStatus.Loaded].ContainsKey(abName))
                return GetQueue(AbObjStatus.Loaded)[abName];

            if (queueDic[AbObjStatus.Unload].ContainsKey(abName))
                return GetQueue(AbObjStatus.Unload)[abName];

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Dictionary<string, AssetBundleObject> GetQueue(AbObjStatus status)
        {
            return status == AbObjStatus.None ? null : queueDic[status];
        }

        /// <summary>
        /// 将资源包加入某个队列
        /// </summary>
        /// <param name="status"></param>
        /// <param name="obj"></param>
        private void PutAbObjInQueue(AbObjStatus status, AssetBundleObject obj)
        {
            var hashName = obj.HashName;

            var curStatus = GetAbObjStatus(hashName);

            // 清除上一个状态
            if (curStatus != AbObjStatus.None) GetQueue(curStatus).Remove(hashName);

            if (status != AbObjStatus.None) GetQueue(status).Add(obj.HashName, obj);
        }

        /// <summary>
        /// 检测正在加载的资源状态
        /// </summary>
        public void UpdateLoading()
        {
            var queue = GetQueue(AbObjStatus.Loading);

            if (queue.Count == 0) return;

            tempList.Clear();

            foreach (var abObj in queue.Values)
            {
                if (abObj.DependLoadingCount == 0 && abObj.Request != null && abObj.Request.isDone)
                {
                    abObj.Main = abObj.Request.assetBundle;

                    tempList.Add(abObj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                DoLoadedCallFun(tempList[i]);

                PutAbObjInQueue(AbObjStatus.Loaded, tempList[i]);
            }
        }

        /// <summary>
        /// 加待加载资源，并将资源移入正在加载队列中
        /// </summary>
        public void UpdateReady()
        {
            var readyQueue = GetQueue(AbObjStatus.Ready);

            var loadingQueue = GetQueue(AbObjStatus.Loading);

            if (readyQueue.Count == 0 || loadingQueue.Count > Max_Loading_Count) return;

            foreach (var abObj in readyQueue.Values)
            {
                DoLoad(abObj); // 执行异步加载

                tempList.Add(abObj);

                if (loadingQueue.Count > Max_Loading_Count) break;
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                PutAbObjInQueue(AbObjStatus.Loading, tempList[i]);
            }
        }
        
        /// <summary>
        /// 检测待卸载队列
        /// </summary>
        public void UpdateUnLoad()
        {
            var queue = GetQueue(AbObjStatus.Unload);

            if (queue.Count == 0) return;

            tempList.Clear();

            foreach (var abObj in queue.Values)
            {
                if(abObj.RefCount == 0 && abObj.Main != null)
                {
                    DoUnLoad(abObj);

                    tempList.Add(abObj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                PutAbObjInQueue(AbObjStatus.None, tempList[i]);
            }
        }

        public void Update()
        {
            UpdateLoading();

            UpdateReady();

            UpdateUnLoad();
        }
    }
}