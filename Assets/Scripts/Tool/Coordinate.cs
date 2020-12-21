using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Coordinate
{
    public int X { get; set; }

    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinate Zero = new Coordinate(0, 0);

    public static Coordinate Down = new Coordinate(0, -1);

    public static Coordinate Up = new Coordinate(0, 1);

    public static Coordinate Left = new Coordinate(-1, 0);

    public static Coordinate Right = new Coordinate(1, 0);


    public override int GetHashCode()
    {
        return X ^ Y;
    }

    public override bool Equals(object obj)
    {
        var coord = obj as Coordinate;

        if (coord.X == X && coord.Y == Y) return true;

        return false;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Y);
    }

    public static Coordinate operator +(Coordinate c1, Coordinate c2)
    {
        return new Coordinate(c1.X + c2.X, c1.Y + c2.Y);
    }

    public static Coordinate operator -(Coordinate c1, Coordinate c2)
    {
        return new Coordinate(c1.X - c2.X, c1.Y - c2.Y);
    }

    public static bool operator ==(Coordinate c1, Coordinate c2)
    {
        return c1.Y == c2.Y && c2.X == c1.X;
    }

    public static bool operator !=(Coordinate c1, Coordinate c2)
    {
        return c1.Y != c2.Y || c2.X != c1.X;
    }
}
