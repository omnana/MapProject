using System;

/// <summary>
/// 公共工厂接口
/// </summary>
public interface IObjectFactory
{
    object AcquireObject(string className);
    object AcquireObject(Type type);
    object AcquireObject<TInstance>() where TInstance : class, new();
    void ReleaseObject(object obj);
}