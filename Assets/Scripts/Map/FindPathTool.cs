using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Coordinate Coordinate;

    public GridNode Next;

    public float W;

    public float H;

    public float S
    {
        get
        {
            return H + W;
        }
    }

    public GridNode(Coordinate coord)
    {
        Coordinate = coord;
    }
}

public class FindPathTool
{
    public static List<Coordinate> FindPath(Coordinate start, Coordinate end, int limit = 1000)
    {
        var mapAreaMgr = MapAreaMgr.Instance;

        var path = new List<Coordinate>();

        var openList = new List<Coordinate>();

        var closeList = new HashSet<Coordinate>();

        var nodeDic = new Dictionary<Coordinate, GridNode>();

        var startNode = new GridNode(start);

        var curNode = startNode;

        nodeDic.Add(startNode.Coordinate, curNode);

        closeList.Add(curNode.Coordinate);

        var loopNum = 0;

        while (curNode != null && curNode.Coordinate != end)
        {
            loopNum++;
            if (loopNum > limit)
            {
                Debug.Log("死循环");
                break;
            }
            GridNode nextNode = null;

            for (var i = 0; i < 4; i++)
            {
                var nCoord = curNode.Coordinate.GetNeightbour(i);

                var grid = mapAreaMgr.GetGrid(nCoord);

                if (grid != null && !grid.IsWall && !closeList.Contains(nCoord))
                {
                    var w = curNode.W + 1;

                    var h = Mathf.Sqrt(Mathf.Pow(end.Y - nCoord.Y, 2) + Mathf.Pow(end.X - nCoord.X, 2));

                    if (!nodeDic.ContainsKey(nCoord))
                    {
                        var node = new GridNode(nCoord)
                        {
                            W = w,

                            H = h,
                        };

                        nodeDic.Add(nCoord, node);

                        if (nextNode != null)
                        {
                            if (nextNode.H > node.H)
                            {
                                nextNode = node;
                            }
                        }
                        else
                        {
                            nextNode = node;
                        }
                    }
                    else
                    {
                        var n = nodeDic[nCoord];

                        if (n.W < w)
                        {
                            n.Next = curNode;

                            curNode.W = n.W + 1;
                        }
                    }
                }
            }

            curNode.Next = nextNode;

            curNode = nextNode;

            if (!closeList.Contains(curNode.Coordinate))
            {
                closeList.Add(curNode.Coordinate);
            }
        }

        while (startNode != null)
        {
            path.Add(startNode.Coordinate);

            startNode = startNode.Next;
        }

        return path;
    }
}
