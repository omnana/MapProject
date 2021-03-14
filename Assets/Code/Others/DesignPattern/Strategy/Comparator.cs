using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 策略模式
/// </summary>
/// <typeparam name="T"></typeparam>
public interface Comparator<T>
{
    int Compare<T>(T a1, T a2);
}
