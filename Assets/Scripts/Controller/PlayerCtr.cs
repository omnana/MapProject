using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtr : MonoBehaviour
{
    public static PlayerCtr Instance;

    public Player Player;

    public void Init()
    {
        Instance = this;
    }

    public void CreatePlayer(Coordinate coord)
    {
        Player.SetCoord(coord);
    }

    void Update()
    {
        
    }
}
