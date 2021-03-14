using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjInfo : MonoBehaviour
{
    public int InstanceId = -1;

    public string AssetName = string.Empty;

    public void Init()
    {
        if (string.IsNullOrEmpty(AssetName)) return;

        //非空，说明通过克隆实例化，添加引用计数

        InstanceId = gameObject.GetInstanceID();

        ServiceLocator.Resolve<PrefabLoadMgr>().AddAssetRef(AssetName, gameObject);
    }

    void OnDestroy()
    {
        //被动销毁，保证引用计数正确
        ServiceLocator.Resolve<PrefabLoadMgr>().Destroy(gameObject);
    }
}
