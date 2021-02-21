using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void AssetsLoadCallback(string name, Object obj);

public class AssetObject
{
    public string AssetName;

    public int LockCallbackCount; // 记录回调当前数量，保证异步是下一帧回调

    public List<AssetsLoadCallback> CallbackList = new List<AssetsLoadCallback>(); //回调函数

    public int InstanceID; // asset的id

    public AsyncOperation Request; // 异步请求，AssetBundleRequest或ResourceRequest

    public Object Asset; // 加载的资源Asset

    public bool IsAbLoad; // 标识是否是ab资源加载的

    /// <summary>
    /// 是弱引用标识，为true时，表示这个资源可以在没有引用时卸载，否则常驻内存。常驻内存是指引用计数为0也不卸载。
    /// </summary>
    public bool IsWeak = true; // 是否是弱引用，用于预加载和释放

    public int RefCount; // 引用计数

    public int UnloadTick; // 卸载使用延迟卸载，UNLOAD_DELAY_TICK_BASE + _unloadList.Count
}
