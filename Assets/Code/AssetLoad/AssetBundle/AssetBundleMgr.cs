using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AssetBundles
{
    public enum objStatus
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
        private Dictionary<objStatus, Dictionary<string, AssetBundleObject>> queueDic;

        private Dictionary<string, string[]> dependsDataDic;

        private List<AssetBundleObject> tempList;

        private const int Max_Loading_Count = 10;

        private MainfestVersion localMainfestVersion;

        public void Init()
        {
            queueDic = new Dictionary<objStatus, Dictionary<string, AssetBundleObject>>()
            {
                { objStatus.Ready, new Dictionary<string, AssetBundleObject>()}, // 准备加载
                { objStatus.Loading, new Dictionary<string, AssetBundleObject>()}, // 正在加载
                { objStatus.Loaded, new Dictionary<string, AssetBundleObject>()}, // 加载成功
                { objStatus.Unload, new Dictionary<string, AssetBundleObject>()}, // 准备卸载
            };

            dependsDataDic = new Dictionary<string, string[]>();

            tempList = new List<AssetBundleObject>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public bool IsFileExist(string abName)
        {
            return dependsDataDic.ContainsKey(abName);
        }

        /// <summary>
        /// 是否有该ab包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public bool Containsobj(string abName)
        {
            return GetQueue(objStatus.Loaded).ContainsKey(abName) 
                || GetQueue(objStatus.Loading).ContainsKey(abName) 
                || GetQueue(objStatus.Ready).ContainsKey(abName);
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

            var asset = ab.LoadAsset("AssetBundleManifest");

            var mainfest = asset as AssetBundleManifest;

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

            Debug.Log("AssetBundleLoadMgr dependsCount = " + dependsDataDic.Count);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetBundleSync(string abName)
        {
            AssetBundleObject obj = null;

            var status = GetobjStatus(abName);

            if (status == objStatus.None) //  未加载
            {
                obj = Createobj(abName, false);

                DoLoad(obj, false);

                PutobjInQueue(objStatus.Loaded, obj);

                return obj;
            }

            obj = GetobjFromQueue(abName);

            if (obj == null)
            {
                Debug.Log("obj no found!!! -> " + abName);

                return null;
            }

            obj.RefCount++;

            foreach (var o in obj.Depends) // 加载依赖项
            {
                LoadAssetBundleSync(o.HashName);
            }

            PutobjInQueue(objStatus.Loaded, obj);

            if (status == objStatus.Loaded) return obj; // 已加载

            if (status == objStatus.Ready)
            {
                DoLoad(obj, false);
            }
            else if (status == objStatus.Loading) // 强制异步为同步加载
            {
                ForceLoadAsyncToSync(obj);
            }

            DoLoadedCallFun(obj, false);

            return obj;
        }

        /// <summary>
        // 异步加载
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="callFun"></param>
        private AssetBundleObject LoadAssetBundleAsync(string abName, AssetBundleLoadCallBack callFun)
        {
            AssetBundleObject obj = null;

            var status = GetobjStatus(abName);

            //  未加载过
            if (status == objStatus.None)
            {
                obj = Createobj(abName);

                obj.CallFunList.Add(callFun);

                // 移入准备队列
                PutobjInQueue(objStatus.Ready, obj);

                return obj;
            }

            obj = GetobjFromQueue(abName);

            if (obj == null)
            {
                Debug.Log("obj no found!!! -> " + abName);

                return null;
            }

            DoDependsRef(obj);

            if (status == objStatus.Loaded) // 已加载
            {
                callFun?.Invoke(obj.Main);
            }
            else
            {
                obj.CallFunList.Add(callFun);
            }

            return obj;
        }

        /// <summary>
        /// 创建新的异步资源包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject Createobj(string abName, bool isAsync = true)
        {
            var obj = new AssetBundleObject()
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
                    if (isAsync) obj.DependLoadingCount = dps.Length;

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
                                if (obj.DependLoadingCount <= 0) return;

                                obj.DependLoadingCount--;
                            });
                        }

                        if (dpObj == null)
                        {
                            Debug.Log("curobj is null!!!");
                        }
                        else
                        {
                            obj.Depends.Add(dpObj);
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 计算某个资源包的引用计数
        /// </summary>
        /// <param name="obj"></param>
        private void DoDependsRef(AssetBundleObject obj)
        {
            obj.RefCount++;

            if (obj.Depends.Count == 0) return;

            foreach (var dpObj in obj.Depends)
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
            var obj = GetobjFromQueue(abName);

            if(obj == null)
            {
                string errormsg = string.Format("UnLoadAssetbundle error ! assetName:{0}", abName);

                Debug.LogError(errormsg);

                return;
            }

            if (obj.RefCount == 0)
            {
                string errormsg = string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", abName);

                Debug.LogError(errormsg);

                return;
            }

            obj.RefCount--;

            foreach (var dpObj in obj.Depends)
            {
                UnLoadAssetBundleAsync(dpObj.HashName);
            }

            if (obj.RefCount == 0)
            {
                PutobjInQueue(objStatus.Unload, obj);
            }
        }

        /// <summary>
        /// 强制异步为同步加载
        /// </summary>
        /// <param name="obj"></param>
        private void ForceLoadAsyncToSync(AssetBundleObject obj)
        {
            if (obj.Request != null)
            {
                obj.Main = obj.Request.assetBundle;

                obj.Request = null;
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isAsync">是否异步</param>
        private void DoLoad(AssetBundleObject obj, bool isAsync = true)
        {
            string assetName = obj.HashName.ToLower() + ".ab";
            string dir = Utility.GetAssetBundlePath(assetName);

            if (isAsync)
            {
                obj.Request = AssetBundle.LoadFromFileAsync(dir);
            }
            else
            {
                obj.Main = AssetBundle.LoadFromFile(dir);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="obj"></param>
        private void DoUnLoad(AssetBundleObject obj)
        {
            // 这里用true，卸载Asset内存，实现指定卸载
            if (obj.Main == null)
            {
                string errormsg = string.Format("LoadAssetbundle DoUnload error ! assetName:{0}", obj.HashName);

                Debug.LogError(errormsg);

                return;
            }

            obj.Main.Unload(true);

            obj.Main = null;
        }

        /// <summary>
        /// 加载资源后的回调，（暂未提供服务器下载资源）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isAsync"></param>
        private void DoLoadedCallFun(AssetBundleObject obj, bool isAsync = true)
        {
            if (obj.Main == null) // 如果还没加载到ab包, 网上下载，先同步下载
            {
            }

            if (obj.Main == null) // 未同步下载到，直接移除
            {
                PutobjInQueue(objStatus.None, obj);
            }

            if(obj.Main == null && isAsync) // 异步下载
            {

            }

            foreach(var callback in obj.CallFunList)
            {
                callback?.Invoke(obj.Main);
            }

            Debug.Log(obj.HashName);

            obj.CallFunList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private objStatus GetobjStatus(string abName)
        {
            if (queueDic[objStatus.Loaded].ContainsKey(abName)) // 已加载
                return objStatus.Loaded;

            if (queueDic[objStatus.Loading].ContainsKey(abName)) // 正在加载，异步改同步
                return objStatus.Loading;

            if (queueDic[objStatus.Ready].ContainsKey(abName)) // 准备加载，直接同步加载
                return objStatus.Ready;

            if (queueDic[objStatus.Unload].ContainsKey(abName)) // 待卸载
                return objStatus.Unload;

            return objStatus.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject GetobjFromQueue(string abName)
        {
            if (GetQueue(objStatus.Ready).ContainsKey(abName))
                return GetQueue(objStatus.Ready)[abName];

            if (queueDic[objStatus.Loading].ContainsKey(abName))
                return GetQueue(objStatus.Loading)[abName];

            if (queueDic[objStatus.Loaded].ContainsKey(abName))
                return GetQueue(objStatus.Loaded)[abName];

            if (queueDic[objStatus.Unload].ContainsKey(abName))
                return GetQueue(objStatus.Unload)[abName];

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Dictionary<string, AssetBundleObject> GetQueue(objStatus status)
        {
            return status == objStatus.None ? null : queueDic[status];
        }

        /// <summary>
        /// 将资源包加入某个队列
        /// </summary>
        /// <param name="status"></param>
        /// <param name="obj"></param>
        private void PutobjInQueue(objStatus status, AssetBundleObject obj)
        {
            var hashName = obj.HashName;

            var curStatus = GetobjStatus(hashName);

            // 清除上一个状态
            if (curStatus != objStatus.None) GetQueue(curStatus).Remove(hashName);

            if (status != objStatus.None) GetQueue(status).Add(obj.HashName, obj);
        }

        /// <summary>
        /// 检测正在加载的资源状态
        /// </summary>
        private void UpdateLoading()
        {
            var queue = GetQueue(objStatus.Loading);

            if (queue.Count == 0) return;

            tempList.Clear();

            foreach (var obj in queue.Values)
            {
                if (obj.DependLoadingCount == 0 && obj.Request != null && obj.Request.isDone)
                {
                    obj.Main = obj.Request.assetBundle;

                    tempList.Add(obj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                DoLoadedCallFun(tempList[i]);

                PutobjInQueue(objStatus.Loaded, tempList[i]);
            }
        }

        /// <summary>
        /// 加待加载资源，并将资源移入正在加载队列中
        /// </summary>
        private void UpdateReady()
        {
            var readyQueue = GetQueue(objStatus.Ready);

            var loadingQueue = GetQueue(objStatus.Loading);

            if (readyQueue.Count == 0 || loadingQueue.Count > Max_Loading_Count) return;

            foreach (var obj in readyQueue.Values)
            {
                DoLoad(obj); // 执行异步加载

                tempList.Add(obj);

                if (loadingQueue.Count > Max_Loading_Count) break;
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                PutobjInQueue(objStatus.Loading, tempList[i]);
            }
        }
        
        /// <summary>
        /// 检测待卸载队列
        /// </summary>
        private void UpdateUnLoad()
        {
            var queue = GetQueue(objStatus.Unload);

            if (queue.Count == 0) return;

            tempList.Clear();

            foreach (var obj in queue.Values)
            {
                if(obj.RefCount == 0 && obj.Main != null)
                {
                    DoUnLoad(obj);

                    tempList.Add(obj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                PutobjInQueue(objStatus.None, tempList[i]);
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