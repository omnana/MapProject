using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        
    }

    public void SetCoord(Coordinate coord)
    {
        transform.position = coord.CoordinateToVector2();

        gameObject.SetActive(true);
    }
}
