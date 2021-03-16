using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class PrefabLoadMgr : Singleton<PrefabLoadMgr>
    {
        private AssetLoadMgr assetLoadMgr;

        private Dictionary<string, PrefabObject> loadedList;

        private List<PrefabObject> loadedAsyncList;

        private Dictionary<int, PrefabObject> goInstanceIDList;

        public PrefabLoadMgr()
        {
            assetLoadMgr = AssetLoadMgr.Instance;

            loadedList = new Dictionary<string, PrefabObject>();

            loadedAsyncList = new List<PrefabObject>();

            goInstanceIDList = new Dictionary<int, PrefabObject>();
        }

        /// <summary>
        /// 异步实例化资源
        /// </summary>
        /// <param name="assetName"></param>
        public void LoadAsync(string assetName, PrefabLoadCallback callFun, Transform parent)
        {
            PrefabObject prefabObj = null;

            if (loadedList.ContainsKey(assetName))
            {
                prefabObj = loadedList[assetName];
                prefabObj.CallbackList.Add(callFun);
                prefabObj.CallParentList.Add(parent);
                prefabObj.RefCount++;

                if (prefabObj.Asset != null) loadedAsyncList.Add(prefabObj);

                return;
            }

            prefabObj = new PrefabObject() { AssetName = assetName, RefCount = 1 };
            prefabObj.CallbackList.Add(callFun);
            prefabObj.CallParentList.Add(parent);

            loadedList.Add(assetName, prefabObj);

            assetLoadMgr.LoadAsync(assetName, (string name, Object obj) =>
            {
                prefabObj.Asset = obj;

                prefabObj.LockCallbackCount = prefabObj.CallbackList.Count;

                DoInstanceAssetCallback(prefabObj);
            }
            );
        }

        /// <summary>
        /// 同步实例化资源
        /// </summary>
        /// <param name="assetName"></param>
        public GameObject LoadSync(string assetName, Transform parent)
        {
            PrefabObject prefabObj = null;

            if (loadedList.ContainsKey(assetName))
            {
                prefabObj = loadedList[assetName];

                prefabObj.RefCount++;

                if (prefabObj.Asset == null) // 说明在异步加载中，需要不影响异步加载,加载后要释放
                {
                    prefabObj.Asset = assetLoadMgr.LoadSync(assetName);

                    var newGo = InstanceAsset(prefabObj, parent);

                    assetLoadMgr.Unload(prefabObj.Asset);

                    prefabObj.Asset = null;

                    return newGo;
                }

                return InstanceAsset(prefabObj, parent);
            }

            prefabObj = new PrefabObject() { AssetName = assetName, RefCount = 1 };

            prefabObj.Asset = assetLoadMgr.LoadSync(assetName);

            loadedList.Add(assetName, prefabObj);

            return InstanceAsset(prefabObj, parent);
        }

        /// <summary>
        /// 执行实例化回调
        /// </summary>
        /// <param name="prefabObject"></param>
        private void DoInstanceAssetCallback(PrefabObject prefabObject)
        {
            if (prefabObject.CallbackList.Count == 0) return;

            var count = prefabObject.LockCallbackCount;
            var callbackList = prefabObject.CallbackList.GetRange(0, count);
            var callParentList = prefabObject.CallParentList.GetRange(0, count);

            prefabObject.LockCallbackCount = 0;
            prefabObject.CallbackList.RemoveRange(0, count);
            prefabObject.CallParentList.RemoveRange(0, count);

            for (int i = 0; i < count; i++)
            {
                if (callbackList[i] != null)
                {
                    var newObj = InstanceAsset(prefabObject, callParentList[i]);//prefab需要实例化

                    try
                    {
                        callbackList[i](prefabObject.AssetName, newObj);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }

                    //如果回调之后，节点挂在默认节点下，认为该节点无效，销毁
                    //if (newObj.transform.parent == assetParent.transform)
                    //Destroy(newObj);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabObj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private GameObject InstanceAsset(PrefabObject prefabObj, Transform parent)
        {
            var go = Object.Instantiate(prefabObj.Asset) as GameObject;

            go.name = go.name.Replace("(Clone)", "");

            var instanceID = go.GetInstanceID();

            var obgInfo = go.AddComponent<ObjInfo>();

            if (obgInfo != null)
            {
                obgInfo.Init();

                obgInfo.InstanceId = instanceID;

                obgInfo.AssetName = prefabObj.AssetName;
            }

            prefabObj.GoInstanceIDSet.Add(instanceID);

            goInstanceIDList.Add(instanceID, prefabObj);

            if (parent != null) go.transform.SetParent(parent, false);

            return go;
        }

        /// <summary>
        /// 卸载实例化Prefab
        /// </summary>
        /// <param name="assetName"></param>
        public void Destroy(GameObject obj)
        {
            if (obj == null) return;

            int instanceID = obj.GetInstanceID();

            if (!goInstanceIDList.ContainsKey(instanceID)) // 非从本类创建的资源，直接销毁即可
            {
                if (obj is GameObject) Object.Destroy(obj);
#if UNITY_EDITOR
                else if (UnityEditor.EditorApplication.isPlaying)
                {
                    Debug.LogError("PrefabLoadMgr destroy NoGameObject name=" + obj.name + " type=" + obj.GetType().Name);
                }
#else
			else Debug.LogError("PrefabLoadMgr destroy NoGameObject name=" + obj.name + " type=" + obj.GetType().Name);
#endif
                return;
            }

            var prefabObj = goInstanceIDList[instanceID];

            if (prefabObj.GoInstanceIDSet.Contains(instanceID)) // 实例化的GameObject
            {
                prefabObj.RefCount--;

                prefabObj.GoInstanceIDSet.Remove(instanceID);

                goInstanceIDList.Remove(instanceID);

                Object.Destroy(obj);
            }
            else
            {
                string errormsg = string.Format("PrefabLoadMgr Destroy error ! assetName:{0}", prefabObj.AssetName);

                Debug.LogError(errormsg);

                return;
            }

            if (prefabObj.RefCount < 0)
            {
                string errormsg = string.Format("PrefabLoadMgr Destroy refCount error ! assetName:{0}", prefabObj.AssetName);
                Debug.LogError(errormsg);
                return;
            }

            if (prefabObj.RefCount == 0)
            {
                loadedList.Remove(prefabObj.AssetName);

                assetLoadMgr.Unload(prefabObj.Asset);

                prefabObj.Asset = null;
            }
        }

        /// <summary>
        /// 用于解绑回调
        /// </summary>
        public void RemoveCallback(string assetName, PrefabLoadCallback callFun)
        {
            if (callFun == null) return;

            PrefabObject prefabObj = null;
            if (loadedList.ContainsKey(assetName))
                prefabObj = loadedList[assetName];

            if (prefabObj != null)
            {
                int index = prefabObj.CallbackList.IndexOf(callFun);

                if (index >= 0)
                {
                    prefabObj.RefCount--;
                    prefabObj.CallbackList.RemoveAt(index);
                    prefabObj.CallParentList.RemoveAt(index);

                    if (index < prefabObj.LockCallbackCount) // 说明是加载回调过程中解绑回调，需要降低lock个数
                    {
                        prefabObj.LockCallbackCount--;
                    }
                }

                if (prefabObj.RefCount < 0)
                {
                    string errormsg = string.Format("PrefabLoadMgr Destroy refCount error ! assetName:{0}", prefabObj.AssetName);
                    Debug.LogError(errormsg);
                    return;
                }

                if (prefabObj.RefCount == 0)
                {
                    loadedList.Remove(prefabObj.AssetName);

                    assetLoadMgr.Unload(prefabObj.Asset);

                    prefabObj.Asset = null;
                }
            }
        }

        /// <summary>
        /// 用于外部实例化，增加引用计数
        /// </summary>
        public void AddAssetRef(string assetName, GameObject gameObject)
        {
            if (!loadedList.ContainsKey(assetName)) return;

            var prefabObj = loadedList[assetName];

            int instanceID = gameObject.GetInstanceID();

            if (goInstanceIDList.ContainsKey(instanceID))
            {
                string errormsg = string.Format("PrefabLoadMgr AddAssetRef error ! assetName:{0}", assetName);

                Debug.LogError(errormsg);

                return;
            }

            prefabObj.RefCount++;

            prefabObj.GoInstanceIDSet.Add(instanceID);

            goInstanceIDList.Add(instanceID, prefabObj);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLoadedAsync()
        {
            if (loadedAsyncList.Count == 0) return;

            int count = loadedAsyncList.Count;

            for (int i = 0; i < count; i++)
            {
                loadedAsyncList[i].LockCallbackCount = loadedAsyncList[i].CallbackList.Count;
            }

            for (int i = 0; i < count; i++)
            {
                DoInstanceAssetCallback(loadedAsyncList[i]);
            }

            loadedAsyncList.RemoveRange(0, count);
        }

        private void Update()
        {
            UpdateLoadedAsync();
        }
    }
}