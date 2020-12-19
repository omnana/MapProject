using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea
{
    public int Id;

    public MapArea LeftChild;

    public MapArea RightChild;

    public int Wdith;

    public int Height;

    public Coordinate Coordinate;

    public RoomData RoomData;

    public MapArea(Coordinate coord, int w, int h)
    {
        Wdith = w;

        Height = h;

        Coordinate = coord;
    }

    /// <summary>
    /// 获取某个房间
    /// </summary>
    /// <returns></returns>
    public RoomData GetRoom()
    {
        if (RoomData != null) return RoomData;

        if (LeftChild == null && RightChild == null) return null;

        RoomData lRoom = null;

        RoomData rRoom = null;

        if (LeftChild != null) lRoom = LeftChild.GetRoom();

        if (RightChild != null) rRoom = RightChild.GetRoom();

        if (lRoom == null && rRoom == null) return null;

        if (lRoom != null) return lRoom;

        if (rRoom != null) return rRoom;

        if (Const.Random.Next(0, 10) > 4) return lRoom;

        return rRoom;
    }
}
