using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class @int
{
    public Coordinate Coordinate;

    public @int Parent;

    public int G;

    public int H;

    public int F
    {
        get
        {
            return H + G;
        }
    }

    public @int(Coordinate coord)
    {
        Coordinate = coord;
    }
}

public class FindPathTool
{
    private static MinHeap minHeap = new MinHeap();

    /// <summary>
    /// 寻路
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public static List<Coordinate> FindPath(Coordinate start, Coordinate end, int limit = 5000)
    {
        var path = new List<Coordinate>();

        var openSet = new HashSet<Coordinate>();

        minHeap.Clear();

        var closeSet = new HashSet<Coordinate>();

        var curNode = new @int(start);

        closeSet.Add(curNode.Coordinate);

        var loopNum = 0;

        while (curNode != null && curNode.Coordinate != end)
        {
            loopNum++;

            if (loopNum > limit)
            {
                Debug.Log("死循环");
                return new List<Coordinate>();
            }

            for (var i = 0; i < 4; i++)
            {
                var nCoord = curNode.Coordinate.GetNeightbour(i);

                var grid = MapAreaMgr.Instance.GetGrid(nCoord);

                if (grid != null && !grid.IsWall && !closeSet.Contains(nCoord))
                {
                    var g = curNode.G + (i < 4 ? 10 : 14);

                    var h = (int)(nCoord.Distance(end) * 10);

                    if (!openSet.Contains(nCoord))
                    {
                        var node = new @int(nCoord)
                        {
                            G = g,

                            H = h,

                            Parent = curNode,
                        };

                        openSet.Add(nCoord);

                        minHeap.Add(node);
                    }
                }
            }

            curNode = minHeap.GetMin();

            closeSet.Add(curNode.Coordinate);
        }

        while (curNode != null)
        {
            path.Add(curNode.Coordinate);

            curNode = curNode.Parent;
        }

        path.Reverse();

        return path;
    }

    //private static int ExcuteSort(GridNode n1, GridNode n2)
    //{
    //    return n1.F.CompareTo(n2.F);
    //}


    /// <summary>
    /// 小顶堆
    /// </summary>
    public class MinHeap
    {
        private @int[] array;

        int count = 0;

        private HashSet<@int> haseSet;

        public MinHeap()
        {
            array = new @int[1000];

            haseSet = new HashSet<@int>();
        }

        public void Add(@int a)
        {
            array[count] = a;

            haseSet.Add(a);

            count++;
        }

        public void MaxHeapify(@int[] arr, int start, int end)
        {
            var dad = start;

            var son = 2 * dad + 1;

            while (son <= end)
            {
                if (son + 1 <= end && arr[son].F > arr[son + 1].F) son++;

                if (arr[dad].F <= arr[son].F) return;

                Swap(arr, dad, son);
            }
        }


        public @int GetMin()
        {
            for (var i = count / 2 - 1; i >= 0; i--)
            {
                MaxHeapify(array, i, count - 1);
            }

            Swap(array, 0, count - 1);

            var m = array[count - 1];

            array[count - 1] = null;

            count--;

            haseSet.Remove(m);

            return m;
        }

        public bool Contains(@int n)
        {
            return haseSet.Contains(n);
        }

        public void Clear()
        {
            haseSet.Clear();

            for (var i = 0; i < count; i++)
            {
                array[i] = null;
            }

            count = 0;
        }

        private void Swap(@int[] arr, int i, int j)
        {
            var temp = arr[i];

            arr[i] = arr[j];

            arr[j] = temp;
        }
    }

}
