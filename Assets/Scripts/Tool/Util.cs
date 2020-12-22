using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    private static Coordinate[] DirArray = new Coordinate[]
    {
        Coordinate.Down,
        Coordinate.Left,
        Coordinate.Up,
        Coordinate.Right,
        Coordinate.LeftDown,
        Coordinate.LeftUp,
        Coordinate.RightUp,
        Coordinate.RightDown,
    };

    public static Vector2 CoordinateToVector2(this Coordinate coord)
    {
        return new Vector2(coord.X * Config.GridSize, coord.Y * Config.GridSize);
    }

    public static Coordinate Vector2ToCoord(this Vector2 pos)
    {
        return new Coordinate((int)(pos.x / Config.GridSize + 0.5), (int)(pos.y / Config.GridSize + 0.5));
    }

    public static Coordinate Vector3ToCoord(this Vector3 pos)
    {
        return new Coordinate((int)(pos.x / Config.GridSize + 0.5), (int)(pos.y / Config.GridSize + 0.5));
    }

    public static Coordinate GetNeightbour(this Coordinate coord, int dir)
    {
        return coord + DirArray[dir];
    }

    public static float Distance(this Coordinate coord, Coordinate end)
    {
        return Mathf.Sqrt(Mathf.Pow(coord.Y - end.Y, 2) + Mathf.Pow(coord.X - end.X, 2));
    }
}