using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Room = 0, // 正常

    Corridor = 1, // 走廊
}

[System.Serializable]
public class RoomData
{
    public RoomType RoomType;

    public int Id;

    public int Width;

    public int Height;

    public int Start;

    public int End;

    [SerializeField]
    public Coordinate Coordinate;

    public GridData[,] gridDatas;

    public Coordinate LeftDown
    {
        get
        {
            return Coordinate;
        }
    }

    public Coordinate RightUp
    {
        get
        {
            return Coordinate + new Coordinate(Width - 1, Height - 1);
        }
    }

    public RoomData()
    {

    }

    public void Build()
    {
        gridDatas = new GridData[Width, Height];

        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var roomCoord = new Coordinate(i, j);

                var worldCoord = Coordinate + roomCoord;

                if (RoomType == RoomType.Corridor)
                {
                    if (MapAreaMgr.Instance.HasGridInWorldCoord(worldCoord.X, worldCoord.Y)) continue;

                    var corridorGrid = new GridData()
                    {
                        GridType = GridType.Corridor,
                        RoomCoord = roomCoord,
                        WorldCoord = worldCoord,
                    };

                    MapAreaMgr.Instance.SetGrid(corridorGrid);

                    gridDatas[i, j] = corridorGrid;
                }
                else if (RoomType == RoomType.Room)
                {
                    var roomGrid = new GridData()
                    {
                        IsWall = Const.Random.Next(0, 10) > 7,
                        GridType = GridType.Normal,
                        RoomCoord = roomCoord,
                        WorldCoord = worldCoord,
                    };

                    MapAreaMgr.Instance.SetGrid(roomGrid);

                    gridDatas[i, j] = roomGrid;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Coordinate RandomACoord()
    {
        var x = Const.Random.Next(1, Width - 1);

        var y = Const.Random.Next(1, Height - 1);

        return new Coordinate(x, y) + Coordinate;
    }
}
