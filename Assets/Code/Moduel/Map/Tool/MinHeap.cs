using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MinHeap1
{
    private int[] array;

    private int count = 0;


    public MinHeap1()
    {
        array = new int[1000];
    }

    public void Add(int a)
    {
        array[count++ + 1] = a;
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

            son *= 2;
        }
    }

    /// <summary>
    /// 初始化最大堆
    /// </summary>
    public void MaxHeapInit()
    {
        //for (int i = count / 2; i >= 1; i--)
        //{
        //    var dad = i;

        //    var son = 2 * dad;

        //    while (son <= count)
        //    {
        //        if (son + 1 <= count && array[son] < array[son + 1]) son++;

        //        if (array[dad] >= array[son]) break;

        //        Swap(array, dad, son);

        //        son *= 2;
        //    }
        //}
        for (int i = count / 2; i >= 1; i--)
        {
            // dad
            array[0] = array[i];

            int son = i * 2;

            while (son <= count)
            {
                if (son < count && array[son] < array[son + 1]) son++;

                if (array[0] >= array[son]) break;

                //array[son / 2] = array[son];
                Swap(array, son / 2, son);

                son *= 2;
            }

            //array[son / 2] = array[0];
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