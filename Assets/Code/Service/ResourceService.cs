using UnityEngine;
using Omnana;
using System;
using Object = UnityEngine.Object;

public class ResourceService : ServiceBase
{
    private AssetLoadMgr assetLoadMgr;

    private PrefabLoadMgr prefabLoadMgr;

    public override void Loaded()
    {
        base.Loaded();

        assetLoadMgr = AssetLoadMgr.Instance;

        prefabLoadMgr = PrefabLoadMgr.Instance;
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
    /// 从Common图集中，同步加载图片
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public Sprite LoadSpriteFromAtlasSync(string assetName)
    {
        if (string.IsNullOrEmpty(assetName)) return null;

        var obj = assetLoadMgr.LoadSync("common");

        if (obj != null)
        {
           var atlas = obj as UnityEngine.U2D.SpriteAtlas;

           return atlas.GetSprite(assetName);
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configName"></param>
    /// <returns></returns>
    public string LoadCsvSync(string csvName)
    {
        if (string.IsNullOrEmpty(csvName)) return null;

        var obj = assetLoadMgr.LoadSync(csvName);

        if (obj != null)
        {
            var content = obj.ToString();

            assetLoadMgr.Unload(obj);

            return content;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="callback"></param>
    public void LoadCsvAsync(string csvName, Action<string> callback)
    {
        if (string.IsNullOrEmpty(csvName)) return;

        assetLoadMgr.LoadAsync(csvName, (name, obj) =>
        {
            callback?.Invoke(obj.ToString());

            assetLoadMgr.Unload(obj);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configName"></param>
    /// <returns></returns>
    public TextAsset LoadTxtSync(string configName)
    {
        if (string.IsNullOrEmpty(configName)) return null;

        var obj = assetLoadMgr.LoadSync(configName);

        if (obj != null)
        {
            return obj as TextAsset;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="callback"></param>
    public void LoadTxtAsync(string configName, Action<string> callback)
    {
        if (string.IsNullOrEmpty(configName)) return;

        assetLoadMgr.LoadAsync(configName, (name, obj) =>
        {
            callback?.Invoke(obj.ToString());

            assetLoadMgr.Unload(obj);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    public void LoadAssetBytesAsync(string assetName, Action<byte[]> callback)
    {
        if (string.IsNullOrEmpty(assetName)) return;

        assetLoadMgr.LoadAsync(assetName, (name, obj) =>
        {
            //callback?.Invoke();

            assetLoadMgr.Unload(obj);
        });
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public T LoadAssetSync<T>(string assetName) where T : Object
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
    public void LoadAssetAsync<T>(string assetName, Action<T> callback) where T : Object
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
