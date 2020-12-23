using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Coordinate Coordinate;

    public GridNode Parent;

    public int G;

    public int H;

    public int F
    {
        get
        {
            return H + G;
        }
    }

    public GridNode(Coordinate coord)
    {
        Coordinate = coord;
    }
}

public class FindPathTool
{
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

        var openList = new List<GridNode>();

        var openSet = new HashSet<Coordinate>();

        var closeSet = new HashSet<Coordinate>();

        var curNode = new GridNode(start);

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
                    var g = curNode.G + 10;

                    var h = (int)(nCoord.Distance(end) * 10);

                    if (!openSet.Contains(nCoord))
                    {
                        var node = new GridNode(nCoord)
                        {
                            G = g,

                            H = h,

                            Parent = curNode,
                        };

                        openList.Add(node);

                        openSet.Add(nCoord);
                    }
                }
            }

            openList.Sort(ExcuteSort);

            if(openList.Count > 0)
            {
                curNode = openList[0];

                openList.RemoveAt(0);

                closeSet.Add(curNode.Coordinate);
            }
        }

        while (curNode != null)
        {
            path.Add(curNode.Coordinate);

            curNode = curNode.Parent;
        }

        path.Reverse();

        return path;
    }

    private static int ExcuteSort(GridNode n1, GridNode n2)
    {
        return n1.F.CompareTo(n2.F);
    }
}
