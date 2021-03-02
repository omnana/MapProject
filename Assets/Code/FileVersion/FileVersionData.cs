using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 版本文件单元
/// </summary>
public class FileVersionData
{
    public string Name;
    public int Size;
    public string Md5;
    public int Version;
    public bool InitPackage; //是否在初始包内 

    public override string ToString()
    {
        return Name + "\t" + Size + "\t" + Md5 + "\t" + Version;
    }

    public void InitData(string str)
    {
        var sps = str.Split('\t');

        Name = sps[0];

        Size = int.Parse(sps[1]);

        Md5 = sps[2];

        Version = int.Parse(sps[3]);

#if BIG_PACKAGE && !UNITY_ANDROID
        InitPackage = Version % 1000 == 0;
#endif
    }
}
