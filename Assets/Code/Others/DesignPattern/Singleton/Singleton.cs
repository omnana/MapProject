using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton <T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null && UIHelper.MgrRoot != null)
            {
                var objName = typeof(T).Name;

                var obj = new GameObject(objName);

                obj.transform.SetParent(UIHelper.MgrRoot, false);

                instance = obj.AddComponent<T>();
            }

            return instance;
        }
    }
}
