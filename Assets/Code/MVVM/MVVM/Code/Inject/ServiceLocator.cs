using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServiceLocator 
{
    public static Dictionary<Type, Func<object>> Container = new Dictionary<Type, Func<object>>();

    private static SingletonObjectFactory singletonObjectFactory = new SingletonObjectFactory();

    private static TransientObjectFactory transientObjectFactory = new TransientObjectFactory();

    private static PoolObjectFactory poolObjectFactory = new PoolObjectFactory();

    /// <summary>
    ///  对每一次请求，只返回唯一的实例
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public static void RegisterSingleton<TInterface, TInstance>() where TInstance : class, new()
    {
        Container.Add(typeof(TInterface), Lazy<TInstance>(FactoryType.Singleton));
    }

    /// <summary>
    ///  对每一次请求，只返回唯一的实例
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public static void RegisterSingleton<TInstance>() where TInstance : class, new()
    {
        Container.Add(typeof(TInstance), Lazy<TInstance>(FactoryType.Singleton));
    }

    /// <summary>
    ///  对每一次请求，返回不同的实例
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public static void RegisterTransient<TInterface, TInstance>() where TInstance : class, new()
    {
        Container.Add(typeof(TInterface), Lazy<TInstance>(FactoryType.Transient));
    }

    /// <summary>
    ///  对每一次请求，返回不同的实例
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public static void RegisterTransient<TInstance>() where TInstance : class, new()
    {
        Container.Add(typeof(TInstance), Lazy<TInstance>(FactoryType.Transient));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public static void RegisterPool<TInterface, TInstance>() where TInstance : class, new()
    {
        Container.Add(typeof(TInterface), Lazy<TInstance>(FactoryType.Pool));
    }


    /// <summary>
    /// 清空容器
    /// </summary>
    public static void Clear()
    {
        Container.Clear();
    }

    /// <summary>
    /// 从容器中获取一个实例
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static TInterface Resolve<TInterface>() where TInterface : class
    {
        return Resolve(typeof(TInterface)) as TInterface;
    }

    /// <summary>
    /// 从容器中获取一个实例
    /// </summary>
    /// <returns></returns>
    private static object Resolve(Type type)
    {
        if (!Container.ContainsKey(type))
        {
            return null;
        }
        return Container[type]();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="factoryType"></param>
    /// <returns></returns>
    public static Func<object> Lazy<TInstance>(FactoryType factoryType) where TInstance : class, new()
    {
        return () =>
        {
            switch(factoryType)
            {
                case FactoryType.Singleton:
                    return singletonObjectFactory.AcquireObject<TInstance>();
                case FactoryType.Pool:
                    return poolObjectFactory.AcquireObject<TInstance>();
                default:
                    return transientObjectFactory.AcquireObject<TInstance>();

            }
        };
    }
}
