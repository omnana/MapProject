using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionCode
{
    private int version;

    public int Version
    {
        get { return version; }
        set { version = value; }
    }

    /// <summary>
    /// 第几代
    /// </summary>
    public int MaxVer
    {
        get { return version / 1000000; }
    }

    /// <summary>
    /// 换包版本号
    /// </summary>
    public int MidVer
    {
        get { return version / 1000 % 1000; }
    }

    /// <summary>
    /// 资源版本号
    /// </summary>
    public int MinVer
    {
        get { return version % 1000; }
    }

    public override string ToString()
    {
        return MaxVer + "." + MidVer + "." + MinVer;
    }
}
