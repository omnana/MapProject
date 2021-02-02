using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashTool
{
    /// <summary>
    /// 加法Hash
    /// </summary>
    /// <param name="key"></param>
    /// <param name="prime"></param>
    /// <returns></returns>
    public static int AdditiveHash(string key, int prime)
    {
        int hash, i;
        for (hash = key.Length, i = 0; i < key.Length; i++)
            hash += key[i];
        return (hash % prime);
    }

    /// <summary>
    /// 位运算Hash
    /// </summary>
    /// <param name="key"></param>
    /// <param name="prime"></param>
    /// <returns></returns>
    public static int RotatingHash(string key, int prime)
    {
        int hash, i;
        for (hash = key.Length, i = 0; i < key.Length; ++i)
            hash = (hash << 4) ^ (hash >> 28) ^ key[i];
        return (hash % prime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int Bernstein(string key)
    {
        int hash = 0;
        int i;
        for (i = 0; i < key.Length; ++i) hash = 33 * hash + key[i];
        return hash;
    }

}
