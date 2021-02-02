using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMono : MonoBehaviour
{
    public int Id;

    public GridType GridType;

    public Coordinate Coordinate;

    public GameObject Normal;

    public GameObject Corridor;

    public GameObject Wall;

    public Coordinate WorldCoord;

    private float gridSize;

    private void Awake()
    {
        gridSize = Config.GridSize;
    }

    public void SetData(GridData data)
    {
        GridType = data.GridType;

        Normal.SetActive(GridType == GridType.Normal);

        Corridor.SetActive(GridType == GridType.Corridor);

        Wall.SetActive(data.IsWall);

        transform.localPosition = data.WorldCoord.CoordinateToVector2();

        gameObject.SetActive(true);
    }
}
