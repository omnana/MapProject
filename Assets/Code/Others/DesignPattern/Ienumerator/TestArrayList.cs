using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System;

namespace OmnanaTest
{
    /// <summary>
    /// 动态列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyArrayList : Ienumerator
    {
        public int Count { get; private set; }

        private object[] array;

        private const int MaxCount = 10;

        public MyArrayList() { }

        public void Add(object a)
        {
            if (array == null) array = new object[MaxCount];

            if (Count >= array.Length)
            {
                var newArray = new object[array.Length * 2];

                array.CopyTo(newArray, 0);

                array = newArray;
            }

            array[Count] = a;

            Count++;
        }

        public void Remove(object a)
        {
            if (array == null)
            {
                return;
            }

            var removeIndex = -1;

            for (int i = 0; i < Count; i++)
            {
                if (array[i] == a)
                {
                    removeIndex = i;

                    break;
                }
            }

            if (removeIndex != -1)
            {
                for (int i = removeIndex + 1; i < Count; i++)
                {
                    array[i - 1] = array[i];
                }

                Count--;

                array[Count] = null;
            }
        }

        #region  迭代器模式

        private int curIndex;

        public bool MoveNext()
        {
            return curIndex++ < Count;
        }

        public object Current
        {
            get
            {
                if (array == null || curIndex < 0 || curIndex > Count) return null;

                return array[curIndex - 1];
            }
        }

        public void Reset()
        {
            Count = 0;

            curIndex = 0;

            array = null;
        }

        public Ienumerator GetEnumerator()
        {
            curIndex = 0;
            return this;
        }

        #endregion

        public void Sort(IComparer<object> comparator)
        {
            if (array == null) return;

            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count - i - 1; j++)
                {
                    var c = comparator.Compare(array[j], array[j + 1]);

                    if (c == 1)
                    {
                        Swap(j, j + 1);
                    }
                }
            }
        }

        private void Swap(int i, int j)
        {
            var temp = array[i];

            array[i] = array[j];

            array[j] = temp;
        }

    }

}