using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject gridPrefab;

    public RoomData RoomData;


    public void SetData(RoomData data)
    {
        RoomData = data;

        var pos = data.Coordinate.CoordinateToVector2();

        name = string.Format("{0}_{1}", data.RoomType, data.Id);

        transform.localPosition = pos;

        for (var i = 0; i < data.Width; i++)
        {
            for (var j = 0; j < data.Height; j++)
            {
                var coord = new Coordinate(i, j);

                var worldCoord = coord + data.Coordinate;

                if (MapAreaMgr.Instance.CoordIsOccupy(worldCoord)) continue;

                var obj = Instantiate(gridPrefab);

                obj.transform.SetParent(transform, false);

                obj.transform.localPosition = new Vector3(i * Config.GridSize, j * Config.GridSize);

                var grid = obj.GetComponent<GridMono>();

                grid.Id = MapAreaMgr.Instance.GetGridId();

                if (data.RoomType == RoomType.Room)
                {
                    grid.SetType(GridType.Normal);
                }
                else if (data.RoomType == RoomType.Corridor)
                {
                    grid.SetType(GridType.Corridor);
                }

                grid.Coordinate = coord;

                MapAreaMgr.Instance.OccupyGrid(worldCoord);

                obj.SetActive(true);

            }
        }

        gameObject.SetActive(true);
    }
}
