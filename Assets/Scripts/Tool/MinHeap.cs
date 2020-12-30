using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MinHeap1
{
    private int[] array;

    private int count = 1;


    public MinHeap1()
    {
        array = new int[1000];
    }

    public void Add(int a)
    {
        array[count] = a;
        count++;
    }

    public void MaxHeapify(int[] arr, int start, int end)
    {
        var dad = start;

        var son = 2 * dad;

        while (son <= end)
        {
            if (son + 1 <= end && arr[son] < arr[son + 1]) son++;

            if (arr[dad] >= arr[son]) return;

            Swap(arr, dad, son);
        }
    }

    /// <summary>
    /// 初始化最大堆
    /// </summary>
    public void MaxHeapInit()
    {
        for(var i = count / 2; i > 0; i--)
        {
            MaxHeapify(array, i, count);
        }
    }

    /// <summary>
    /// 插入
    /// </summary>
    /// <param name="n"></param>
    public void Insert(int n)
    {
        Add(n);

        var i = count;

        while (i != 1 && n > array[i / 2])
        {
            array[i] = array[i / 2];

            i /= 2;
        }

        array[i] = n;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="n"></param>
    public void DeleteTop()
    {
        var top = array[count--];

        array[1] = array[count--];

        var i = 1;

        var son = i * 2;

        while (son <= count)
        {
            if (son < count && array[son] < array[son + 1]) son++;

            if (array[i] > array[son]) break;

            array[i] = array[son];

            son *= 2;
        }

        array[i] = top;
    }

    private void Swap(int[] arr, int i, int j)
    {
        var temp = arr[i];

        arr[i] = arr[j];

        arr[j] = temp;
    }
}