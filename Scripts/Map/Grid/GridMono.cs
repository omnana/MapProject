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

    public void SetType(GridType type)
    {
        GridType = type;

        Normal.SetActive(type == GridType.Normal);

        Corridor.SetActive(type == GridType.Corridor);
    }
}
