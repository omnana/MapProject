using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtr : MonoBehaviour
{
    public MapGenerator MapGenerator;

    public CameraFollow CameraFollow;

    public InputCtr InputCtr;

    public PlayerCtr PlayerCtr;

    private int gridNum;

    private bool isInit;

    private void Awake()
    {
        CameraFollow.Init();

        InputCtr.Init();

        PlayerCtr.Init();
    }

    void Start()
    {
        MapAreaMgr.Instance.LoadData();

        gridNum = GetGridMaxNum();

        MapGenerator.LoadMap(gridNum, LoadMapEnd);
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

        isInit = true;
    }

    private int GetGridMaxNum()
    {
        var rightUpCoord = CameraFollow.Corners[1].Vector3ToCoord();

        var leftLowerCoord = CameraFollow.Corners[2].Vector3ToCoord();

        var delt = Config.ShowRoomNumAdd;

        var leftDown = leftLowerCoord - new Coordinate(delt, delt);

        var rightUp = rightUpCoord + new Coordinate(delt, delt);

        return (rightUp.Y - leftDown.Y + 1) * (rightUp.X - leftDown.X + 1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var targetCoord = InputCtr.GetMouseCoord();

            var path = FindPathTool.FindPath(PlayerCtr.Player.Coordinate, targetCoord);
        }

        //if (!isInit) return;

        //var rightUpCoord = CameraFollow.Corners[1].Vector3ToCoord();

        //var leftLowerCoord = CameraFollow.Corners[2].Vector3ToCoord();

        //var delt = Config.ShowRoomNumAdd;

        //var leftDown = leftLowerCoord - new Coordinate(delt, delt);

        //var rightUp = rightUpCoord + new Coordinate(delt, delt);

        //if (leftDown.X < 0) leftDown.X = 0;

        //if (leftDown.Y < 0) leftDown.Y = 0;

        //if (rightUp.X > Config.MapAreaWidth) rightUp.X = Config.MapAreaWidth;

        //if (rightUp.Y > Config.MapAreaHeight) rightUp.Y = Config.MapAreaHeight;

        //var w = rightUp.X - leftDown.X + 1;

        //var h = rightUp.Y - leftDown.Y + 1;

        //Debug.Log("orign = " + leftDown.ToString());

        //Debug.LogFormat("w = {0}, h = {1}", w, h);

        //var data = MapAreaMgr.Instance.AllGrids;

        //var gridlist = MapGenerator.GridList;

        //var dataCount = w * h;

        //for (var i = 0; i < w; i++)
        //{
        //    for (var j = 0; j < h; j++)
        //    {
        //        if(data[i + leftDown.X, j + rightUp.Y] != null)
        //        {
        //            gridlist[i].SetData(data[i + leftDown.X, j + rightUp.Y]);
        //        }
        //    }
        //}

        //for (var i = 0; i < gridlist.Count; i++)
        //{
        //    if(i < dataCount)
        //    {
        //        var x = i % w + leftDown.X;
        //        var y = i / w + leftDown.Y;

        //        if(data[x, y] != null)
        //        {
        //            gridlist[i].SetData(data[x, y]);
        //        }
        //    }
        //    else if(gridlist[i].gameObject.activeSelf)
        //    {
        //        gridlist[i].gameObject.SetActive(false);
        //    }
        //}

        //Debug.Log(leftUpCoord.ToString());
        //Debug.Log(rightUpCoord.ToString());
        //Debug.Log(leftLowerCoord.ToString());
        //Debug.Log(rightLowerCoord.ToString());

        //Debug.Log(CameraFollow.Corners[0]);
        //Debug.Log(CameraFollow.Corners[1]);
        //Debug.Log(CameraFollow.Corners[2]);
        //Debug.Log(CameraFollow.Corners[3]);

        //Debug.Log("============================");
    }
}
