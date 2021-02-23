using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PrefabLoadCallback(string assetName, GameObject obj);

public class PrefabObject
{
    public string AssetName;

    public int LockCallbackCount; //记录回调当前数量，保证异步是下一帧回调

    public List<PrefabLoadCallback> CallbackList = new List<PrefabLoadCallback>();

    public List<Transform> CallParentList = new List<Transform>();

    public Object Asset;

    public int RefCount;

    public HashSet<int> GoInstanceIDSet = new HashSet<int>(); //实例化的GameObject引用列表
}
