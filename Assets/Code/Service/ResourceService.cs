using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using System;
using Object = UnityEngine.Object;

public class ResourceService : BaseCtrl
{
    private AssetBundleMgr assetBundleMgr;

    private readonly Dictionary<ResourceType, string> ResourceTypeToNameDic = new Dictionary<ResourceType, string>()
    {
        {ResourceType.Gui,  "ui"},
        {ResourceType.Model,  "model"},
        {ResourceType.Texture,  "texture"},
        {ResourceType.Atlas,  "atlas"},
        {ResourceType.Scene,  "scene"},
    };

    public override void DispoWith()
    {
        base.DispoWith();
    }

    public ResourceService()
    {
        assetBundleMgr = ServiceLocator.Resolve<AssetBundleMgr>();
    }

    /// <summary>
    /// 加载模型
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public GameObject LoadModelSync(string assetName)
    {
        var abName = ResourceTypeToNameDic[ResourceType.Model];

        var abObj = assetBundleMgr.LoadAssetBundleSync(abName);

        if (abObj.Main != null)
        {
            return abObj.Main.LoadAsset(assetName) as GameObject;
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
        var abName = ResourceTypeToNameDic[ResourceType.Model];

        assetBundleMgr.LoadAssetBundleAsync(abName, (ab)=>
        {
            var asset = ab.LoadAsset(assetName);

            if (asset != null)
            {
                callback?.Invoke(asset as GameObject);
            }
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
        var abName = ResourceTypeToNameDic[type];

        var abObj = assetBundleMgr.LoadSync(abName);

        if (abObj.Main != null)
        {
            return abObj.Main.LoadAsset(assetName) as T;
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
        var abName = ResourceTypeToNameDic[type];

        assetBundleMgr.LoadAsync(abName, (ab)=> 
        {
            var asset = ab.LoadAsset(assetName);

            if(asset != null)
            {
                callback?.Invoke(asset as T);
            }
        });
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public void UnLoadAsset(ResourceType type)
    {
        var abName = ResourceTypeToNameDic[type];

        assetBundleMgr.UnLoadAssetBundleAsync(abName);
    }
}
