using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorData
{
    public int Id;

    public bool IsStraight;

    public bool IsVerticle;

    public List<RoomData> RoomData = new List<RoomData>();

    public CorridorData()
    {

    }
}
