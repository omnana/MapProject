using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtr : MonoBehaviour
{
    public MapGenerator MapGenerator;

    public CameraFollow CameraFollow;

    public InputCtr InputCtr;

    public PlayerCtr PlayerCtr;

    private void Awake()
    {
        CameraFollow.Init();

        InputCtr.Init();

        PlayerCtr.Init();
    }

    void Start()
    {
        MapAreaMgr.Instance.LoadData();

        MapGenerator.LoadMap(LoadMapEnd);
    }

    private void LoadMapEnd()
    {
        Debug.Log("加载地图完毕");

        var bornRoom = MapAreaMgr.Instance.RandomARoom();

        Debug.LogFormat("出生房间Id = {0}", bornRoom.Id);

        var coord = bornRoom.RandomACoord();

        Debug.LogFormat("出生坐标 = {0}", coord.ToString());

        PlayerCtr.CreatePlayer(coord);

        CameraFollow.SetFollow(PlayerCtr.Player.gameObject);
    }

    void Update()
    {

    }
}
