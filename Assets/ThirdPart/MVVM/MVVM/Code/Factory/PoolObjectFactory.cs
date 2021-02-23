using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjectFactory : IObjectFactory
{
    public object AcquireObject(string className)
    {
        throw new NotImplementedException();
    }

    public object AcquireObject(Type type)
    {
        throw new NotImplementedException();
    }

    public object AcquireObject<TInstance>() where TInstance : class, new()
    {
        throw new NotImplementedException();
    }

    public void ReleaseObject(object obj)
    {
        throw new NotImplementedException();
    }
}
