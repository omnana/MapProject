﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortUtil
{
    /// <summary>
    /// 冒泡
    /// </summary>
    /// <param name="arr"></param>
    public static void PopSort(int[] arr)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            for (var j = 0; j < arr.Length - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    Swap(arr, j, j + 1);
                }
            }
        }
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <param name="arr"></param>
    public static void SelectSort(int[] arr)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            var max = arr[i];

            for (var j = i + 1; j < arr.Length; j++)
            {
                if (max > arr[j])
                {
                    Swap(arr, i, j);

                    max = arr[i];
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arr"></param>
    public static void InserSort(int[] arr)
    {
    }

    /// <summary>
    /// 快排
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public static void QuickSort(int[] arr, int left, int right)
    {
        if(left < right)
        {
            var mid = Partition(arr, left, right);

            QuickSort(arr, left, mid - 1);

            QuickSort(arr, mid + 1, right);
        }
    }

    private static int Partition(int[] arr, int left, int right)
    {
        var p = arr[left];

        while (left < right)
        {
            while (left < right && arr[right] > p) right--;

            Swap(arr, left, right);

            while (left < right && arr[left] < p) left++;

            Swap(arr, left, right);

            arr[left] = p;
        }

        return left;
    }

    private static void Swap(int[] arr, int i, int j)
    {
        var temp = arr[i];

        arr[i] = arr[j];

        arr[j] = temp;
    }
}
