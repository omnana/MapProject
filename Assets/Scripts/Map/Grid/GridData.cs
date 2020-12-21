using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public int Id;

    public GridType GridType;

    public Coordinate RoomCoord = Coordinate.Zero;

    public Coordinate WorldCoord = Coordinate.Zero;

    public bool IsWall;

    public GridData()
    {
        Id = MapAreaMgr.Instance.GetGridId();
    }
}
