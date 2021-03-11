using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundles
{
    public delegate void AssetBundleLoadCallBack(AssetBundle ab);

    /// <summary>
    /// ab资源包
    /// </summary>
    public class AssetBundleObject
    {
        public string HashName; // hash标识符

        public int RefCount; // 引用计数

        public List<AssetBundleLoadCallBack> CallFunList = new List<AssetBundleLoadCallBack>(); //回调函数

        public AssetBundleCreateRequest Request; // 异步加载请求

        public AssetBundle AssetBundle; // 加载到的ab

        public int DependLoadingCount; // 依赖计数
   
        public List<AssetBundleObject> Depends = new List<AssetBundleObject>(); // 依赖项
    }
}