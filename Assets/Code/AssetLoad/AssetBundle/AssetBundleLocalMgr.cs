using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AssetBundles
{
    /// <summary>
    /// 资源管理
    /// </summary>
    public class MyAssetBundleMgr : MonoBehaviour
    {
        public static MyAssetBundleMgr Instance { get; private set; }

        private List<AssetBundleObject> waitList;

        private Dictionary<string, AssetBundleObject> completeDic;

        private List<AssetBundleObject> unLoadList;

        private Dictionary<string, string[]> dependsDataDic;

        private Dictionary<string, List<string>> assetToAssetBundleDic;

        private HashSet<string> waittingList;

        private void Awake()
        {
            Instance = this;

            dependsDataDic = new Dictionary<string, string[]>();

            completeDic = new Dictionary<string, AssetBundleObject>();

            waitList = new List<AssetBundleObject>();

            unLoadList = new List<AssetBundleObject>();

            waittingList = new HashSet<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <returns></returns>
        public bool IsFileExist(string assetName)
        {
            return dependsDataDic.ContainsKey(assetName);
        }

        private string GetAssetBundlePath(string assetName)
        {
            return Utility.GetAssetBundlePath(assetName);
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
                string[] dps = mainfest.GetAllDependencies(assetName);

                for (int i = 0; i < dps.Length; i++)
                {
                    var dpAssetName = dps[i].Replace(".ab", "");

                    if (!assetToAssetBundleDic.ContainsKey(dpAssetName))
                    {
                        assetToAssetBundleDic.Add(dpAssetName, new List<string>());
                    }

                    assetToAssetBundleDic[dpAssetName].Add(assetName);
                }

                dependsDataDic.Add(assetName, dps);
            }

            ab.Unload(true);

            ab = null;

            Debug.Log("AssetBundleLoadMgr dependsCount = " + dependsDataDic.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <returns></returns>
        public T LoadSync<T>(string assetName) where T : Object
        {
            var obj = LoadAssetSync(assetName);

            if (obj != null && obj.AssetBundle != null)
            {
                if (assetToAssetBundleDic.ContainsKey(assetName))
                {
                    var assetbundleName = assetToAssetBundleDic[assetName];

                    //if(assetbundleName)

                    //return obj.AssetBundle.LoadAsset<T>(assetbundleName);
                }
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="callFunc"></param>
        public void LoadAsync(string assetName, AssetBundleLoadCallBack callFun)
        {
            StartCoroutine(LoadAssetAsync(assetName, callFun));
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetSync(string assetName)
        {
            AssetBundleObject obj = null;

            if (completeDic.ContainsKey(assetName))
            {
                completeDic[assetName].RefCount++;

                return completeDic[assetName];
            }

            obj = NewAssetbundleObj(assetName);

            var path = GetAssetBundlePath(obj.HashName);

            obj.AssetBundle = AssetBundle.LoadFromFile(path);

            if (dependsDataDic.ContainsKey(assetName))
            {
                var dps = dependsDataDic[assetName];

                var count = dps.Length;

                obj.DependLoadingCount = count;

                foreach (var d in dps)
                {
                    var dpAbObj = NewAssetbundleObj(d);

                    dpAbObj.AssetBundle = AssetBundle.LoadFromFile(path);

                    obj.Depends.Add(dpAbObj);
                }
            }

            return obj;
        }

        /// <summary>
        // 异步加载
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="callFun"></param>
        private IEnumerator LoadAssetAsync(string assetName, AssetBundleLoadCallBack callFun)
        {
            // 正在加载中，先等待
            if(waittingList.Contains(assetName)) yield return null;

            AssetBundleObject obj = null;

            if (completeDic.ContainsKey(assetName))
            {
                var abObj = completeDic[assetName];

                abObj.RefCount++;

                if (abObj.AssetBundle != null)
                {
                    callFun?.Invoke(abObj.AssetBundle);
                }

                yield break;
            }

            obj = NewAssetbundleObj(assetName);

            var path = GetAssetBundlePath(assetName);

            waittingList.Add(assetName);

            var request = AssetBundle.LoadFromFileAsync(path);

            yield return request;

            if (request != null && request.assetBundle != null)
            {
                obj.AssetBundle = request.assetBundle;

                completeDic.Add(obj.HashName, obj);

                waittingList.Remove(assetName);
            }

            if (dependsDataDic.ContainsKey(assetName))
            {
                var dps = dependsDataDic[assetName];

                var count = dps.Length;

                obj.DependLoadingCount = count;

                foreach (var d in dps)
                {
                    var dpAbObj = NewAssetbundleObj(d);

                    StartCoroutine(LoadAssetAsync(dpAbObj.HashName, (ab) =>
                    {
                        obj.DependLoadingCount--;

                        obj.Depends.Add(obj);
                    }));
                }
            }

            while (obj.DependLoadingCount > 0) yield return null;

            if (obj.AssetBundle != null)
            {
                callFun?.Invoke(obj.AssetBundle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private AssetBundleObject NewAssetbundleObj(string assetName)
        {
            return new AssetBundleObject()
            {
                RefCount = 1,
                HashName = string.Format("{0}.ab", assetName.ToLower()),
            };
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetbundleName"></param>
        public void UnLoadAssetBundleAsync(string assetbundleName)
        {
            AssetBundleObject obj = null;

            if (completeDic.ContainsKey(assetbundleName))
            {
                obj = completeDic[assetbundleName];
            }

            if(obj == null)
            {
                string errormsg = string.Format("UnLoadAssetbundle error ! assetName:{0}", assetbundleName);

                Debug.LogError(errormsg);

                return;
            }

            if (obj.RefCount == 0)
            {
                string errormsg = string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", assetbundleName);

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
                unLoadList.Add(obj);
            }
        }


        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="obj"></param>
        private void DoUnLoad(AssetBundleObject obj)
        {
            // 这里用true，卸载Asset内存，实现指定卸载
            if (obj.AssetBundle == null)
            {
                string errormsg = string.Format("LoadAssetbundle DoUnload error ! assetName:{0}", obj.HashName);

                Debug.LogError(errormsg);

                return;
            }

            obj.AssetBundle.Unload(true);

            obj.AssetBundle = null;
        }
        
        /// <summary>
        /// 检测待卸载队列
        /// </summary>
        private void UpdateUnLoad()
        {
            if (unLoadList.Count == 0) return;

            for (var i = 0; i < unLoadList.Count; i++)
            {
                DoUnLoad(unLoadList[i]);
            }

            unLoadList.Clear();
        }

        public void Update()
        {
            UpdateUnLoad();
        }
    }
}