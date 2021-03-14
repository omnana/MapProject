using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例
/// </summary>
/// <typeparam name="T"></typeparam>
public static class Singleton <T> where T : Component
{
    private static T instance;

    public static T GetInstance()
    {
        if (instance == null)
        {
            var objName = typeof(T).ToString();

            var obj = new GameObject(objName);

            instance = obj.AddComponent<T>();
        }

        return instance;
    }
}
