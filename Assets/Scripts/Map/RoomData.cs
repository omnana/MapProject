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

    //private Grid[,] grids;

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
