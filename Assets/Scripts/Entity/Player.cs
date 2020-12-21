using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Coordinate Coordinate;

    void Start()
    {
        
    }

    public void SetCoord(Coordinate coord)
    {
        Coordinate = coord;

        transform.position = coord.CoordinateToVector2();

        gameObject.SetActive(true);
    }
}
