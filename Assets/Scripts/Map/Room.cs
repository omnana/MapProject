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
                var gridData = data.gridDatas[i, j];

                if (gridData == null) continue;

                var obj = Instantiate(gridPrefab);

                obj.transform.SetParent(transform, false);

                var grid = obj.GetComponent<GridMono>();

                grid.SetData(gridData);
            }
        }

        gameObject.SetActive(true);
    }
}
