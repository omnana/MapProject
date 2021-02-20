using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using System;
using Object = UnityEngine.Object;

public class ResourceService : BaseCtrl
{
    private AssetLoadMgr assetLoadMgr;


    public override void DispoWith()
    {
        base.DispoWith();
    }

    public ResourceService()
    {
        assetLoadMgr = ServiceLocator.Resolve<AssetLoadMgr>();
    }

    /// <summary>
    /// 加载模型
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public GameObject LoadModelSync(string assetName)
    {
        var obj = assetLoadMgr.LoadSync(assetName);

        if (obj != null)
        {
            return obj as GameObject;
        }

        return null;
    }

    /// <summary>
    /// 异步加载模型
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    public void LoadModelAsync(string assetName, Action<GameObject> callback)
    {
        if (string.IsNullOrEmpty(assetName)) return;

        assetLoadMgr.LoadAsync(assetName, (name, obj)=>
        {
            callback?.Invoke(obj as GameObject);
        });
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public T LoadAssetSync<T>(ResourceType type, string assetName) where T : Object
    {
        var obj = assetLoadMgr.LoadSync(assetName);

        if (obj != null)
        {
            return obj as T;
        }

        return null;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public void LoadAssetAsync<T>(ResourceType type, string assetName, Action<T> callback) where T : Object
    {
        assetLoadMgr.LoadAsync(assetName, (name, obj) =>
        {
            callback?.Invoke(obj as T);
        });
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public void UnLoadAsset(Object obj)
    {
        assetLoadMgr.Unload(obj);
    }
}
