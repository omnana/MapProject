using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableMgrContainer
{
    private static Dictionary<Type, object> container = new Dictionary<Type, object>();

    public static void RegisterSingleton(Type type)
    {
        var instance = Activator.CreateInstance(type);

        container.Add(type, instance);
    }

    public static TInterface Resolve<TInterface>() where TInterface : class
    {
        return container[typeof(TInterface)] as TInterface;
    }
}
