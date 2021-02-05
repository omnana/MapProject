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
        private Dictionary<string, AssetBundleObject> readyDic; // 准备加载

        private Dictionary<string, AssetBundleObject> loadingDic; // 正在加载

        private Dictionary<string, AssetBundleObject> loadedDic; // 加载成功

        private Dictionary<string, AssetBundleObject> unLoadDic; // 准备卸载

        private Dictionary<string, string[]> dependsDataDic;

        private List<AssetBundleObject> tempList;

        private int Max_Loading_Count = 10;

        public AssetBundleMgr()
        {
            readyDic = new Dictionary<string, AssetBundleObject>();

            loadingDic = new Dictionary<string, AssetBundleObject>();

            loadedDic = new Dictionary<string, AssetBundleObject>();

            unLoadDic = new Dictionary<string, AssetBundleObject>();

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
            return readyDic.ContainsKey(abName) || loadedDic.ContainsKey(abName) || loadingDic.ContainsKey(abName);
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

            if(status == AbObjStatus.None) //  未加载
            {
                abObj = new AssetBundleObject()
                {
                    RefCount = 1,
                    HashName = abName,
                    DependLoadingCount = 0,
                };

                DoLoad(abObj, false);

                if (dependsDataDic.ContainsKey(abName))
                {
                    var dps = dependsDataDic[abName];

                    if (dps != null)
                    {
                        foreach (var d in dps)
                        {
                            var curAb = LoadAssetBundleSync(d);

                            abObj.Depends.Add(curAb);
                        }
                    }
                }

                loadedDic.Add(abName, abObj);

                return abObj;
            }

            abObj = GetAbObjFromCache(abName);

            if(abObj == null)
            {
                Debug.Log("AbObj no found!!! -> " + abName);

                return null;
            }

            abObj.RefCount++;

            foreach (var o in abObj.Depends) // 加载依赖项
            {
                LoadAssetBundleSync(o.HashName);
            }

            if(status == AbObjStatus.Loaded) return abObj; // 已加载

            if (status == AbObjStatus.Loading) // 正在加载，异步改同步
            {
                loadingDic.Remove(abName);
            }
            else if (status == AbObjStatus.Ready) // 准备加载，直接同步加载
            {
                DoLoad(abObj, false);

                readyDic.Remove(abName);
            }

            loadedDic.Add(abName, abObj);

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
                abObj = new AssetBundleObject()
                {
                    RefCount = 1,
                    HashName = abName,
                    DependLoadingCount = 0,
                };

                abObj.CallFunList.Add(callFun);

                if (dependsDataDic.ContainsKey(abName))
                {
                    var dps = dependsDataDic[abName];

                    if (dps != null && dps.Length > 0)
                    {
                        abObj.DependLoadingCount = dps.Length;

                        foreach (var d in dps)
                        {
                            var dpObj = LoadAssetBundleAsync(d, (ab) =>
                             {
                                 if (abObj.DependLoadingCount <= 0) return;

                                 abObj.Main = ab;

                                 abObj.DependLoadingCount--;

                                 if (abObj.DependLoadingCount == 0 && abObj.Request != null && abObj.Request.isDone)
                                 {
                                     DoLoadedCallFun(abObj);
                                 }
                             });

                            abObj.Depends.Add(dpObj);
                        }
                    }
                }

                // 当前加载数量小于阈值，直接加载，反之，移入待加载队列
                if (loadingDic.Count < Max_Loading_Count)
                {
                    DoLoad(abObj);

                    loadingDic.Add(abObj.HashName, abObj);
                }
                else
                {
                    readyDic.Add(abObj.HashName, abObj);
                }

                return abObj;
            }

            abObj = GetAbObjFromCache(abName);

            if (abObj == null)
            {
                Debug.Log("AbObj no found!!! -> " + abName);

                return null;
            }

            DoDependsRef(abObj);

            if (status == AbObjStatus.Loaded)
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
        /// 
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
            var abObj = GetAbObjFromCache(abName);

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
                unLoadDic.Add(abObj.HashName, abObj);
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

                if (abObj.Main == null)
                {
                    Debug.Log("未加载到ab包!!! -> " + abObj.HashName);
                }
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
        /// 加载资源后的回调，（未提供服务器下载资源）
        /// </summary>
        /// <param name="abObj"></param>
        /// <param name="isAsync"></param>
        private void DoLoadedCallFun(AssetBundleObject abObj, bool isAsync = true)
        {
            if (abObj.Request != null) // 异步加载, 直接强制同步加载
            {
                abObj.Main = abObj.Request.assetBundle;

                abObj.Request = null;

                loadingDic.Remove(abObj.HashName);

                loadedDic.Add(abObj.HashName, abObj);
            }

            if (abObj.Main == null) // 如果还没加载到ab包, 网上下载，先同步下载
            {
                var path = Utility.GetAssetBundlePath(abObj.HashName);
            }

            if (abObj.Main == null) // 未同步下载到，直接移除
            {
                if (loadedDic.ContainsKey(abObj.HashName))
                {
                    loadedDic.Remove(abObj.HashName);
                }
                else if (loadingDic.ContainsKey(abObj.HashName))
                {
                    loadingDic.Remove(abObj.HashName);
                }
            }

            if(abObj.Main == null && isAsync) // 异步下载
            {

            }

            foreach(var callback in abObj.CallFunList)
            {
                callback?.Invoke(abObj.Main);
            }

            abObj.CallFunList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AbObjStatus GetAbObjStatus(string abName)
        {
            if (loadedDic.ContainsKey(abName)) // 已加载
                return AbObjStatus.Loaded;

            if (loadingDic.ContainsKey(abName)) // 正在加载，异步改同步
                return AbObjStatus.Loading;

            if (readyDic.ContainsKey(abName)) // 准备加载，直接同步加载
                return AbObjStatus.Ready;

            if (unLoadDic.ContainsKey(abName)) // 待卸载
                return AbObjStatus.Unload;

            return AbObjStatus.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundleObject GetAbObjFromCache(string abName)
        {
            if (loadedDic.ContainsKey(abName))
                return loadedDic[abName];

            if (loadingDic.ContainsKey(abName))
                return loadingDic[abName];

            if (readyDic.ContainsKey(abName))
                return readyDic[abName];

            if (unLoadDic.ContainsKey(abName))
                return unLoadDic[abName];

            return null;
        }

        /// <summary>
        /// 检测正在加载的资源状态
        /// </summary>
        public void UpdateLoading()
        {
            if (loadingDic.Count == 0) return;

            tempList.Clear();

            foreach (var abObj in loadingDic.Values)
            {
                if (abObj.DependLoadingCount == 0 && abObj.Request != null && abObj.Request.isDone)
                {
                    tempList.Add(abObj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                DoLoadedCallFun(tempList[i]);
            }
        }

        /// <summary>
        /// 加待加载资源，并将资源移入正在加载队列中
        /// </summary>
        public void UpdateReady()
        {
            if (readyDic.Count == 0 || readyDic.Count > Max_Loading_Count) return;

            tempList.Clear();

            foreach (var abObj in readyDic.Values)
            {
                DoLoad(abObj);

                tempList.Add(abObj);

                loadingDic.Add(abObj.HashName, abObj);

                if (loadingDic.Count > Max_Loading_Count) break;
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                readyDic.Remove(tempList[i].HashName);
            }
        }
        
        /// <summary>
        /// 检测待卸载队列
        /// </summary>
        public void UpdateUnLoad()
        {
            if (unLoadDic.Count == 0) return;

            tempList.Clear();

            foreach (var abObj in unLoadDic.Values)
            {
                if(abObj.RefCount == 0 && abObj.Main != null)
                {
                    DoUnLoad(abObj);

                    loadedDic.Remove(abObj.HashName);

                    tempList.Add(abObj);
                }
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                unLoadDic.Remove(tempList[i].HashName);
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