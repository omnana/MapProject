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
    };
    
    public static Vector2 CoordinateToVector2(this Coordinate coord)
    {
        return new Vector2(coord.X * Config.GridSize, coord.Y * Config.GridSize);
    }

    public static Coordinate Vector2ToCoord(this Vector2 pos)
    {
        return new Coordinate(Mathf.CeilToInt(pos.x / Config.GridSize), Mathf.CeilToInt(pos.y / Config.GridSize));
    }
    public static Coordinate Vector3ToCoord(this Vector3 pos)
    {
        return new Coordinate(Mathf.CeilToInt(pos.x / Config.GridSize), Mathf.CeilToInt(pos.y / Config.GridSize));
    }

    public static Coordinate GetNeightbour(this Coordinate coord, int dir)
    {
        return coord + DirArray[dir];
    }
}