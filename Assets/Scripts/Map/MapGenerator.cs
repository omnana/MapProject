using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public Room roomPrefab;

    public GridMono gridPrefab;

    public List<GridMono> GridList;

    private void Awake()
    {

    }

    public void LoadMap(int gridNum, Action callback)
    {
        StartCoroutine(OnLoadMap(gridNum, callback));
    }

    private IEnumerator OnLoadMap(int gridNum, Action callback)
    {
        var gridDatas = MapAreaMgr.Instance.AllGrids;

        for (var i = 0; i < Config.MapAreaWidth * Config.MapAreaHeight; i++)
        {
            var w = i % Config.MapAreaWidth;
            var h = i / Config.MapAreaWidth;

            if (gridDatas[w, h] != null)
            {
                var gridObj = Instantiate(gridPrefab);

                gridObj.transform.SetParent(transform, false);

                gridObj.SetData(gridDatas[w, h]);
            }
        }

        //var roomDatas = MapAreaMgr.Instance.RoomDatas;

        //for (var i = 0; i < roomDatas.Count; i++)
        //{
        //    var roomObj = Instantiate(roomPrefab);

        //    roomObj.transform.SetParent(transform, false);

        //    var room = roomObj.GetComponent<Room>();

        //    room.SetData(roomDatas[i]);
        //}

        //var corridorDatas = MapAreaMgr.Instance.CorridorDatas;

        //for (var i = 0; i < corridorDatas.Count; i++)
        //{
        //    var roomObj = Instantiate(roomPrefab);

        //    roomObj.transform.SetParent(transform, false);

        //    var room = roomObj.GetComponent<Room>();

        //    room.SetData(corridorDatas[i]);
        //}

        //for (var i = 0; i < gridNum; i++)
        //{
        //    var gridObj = Instantiate(gridPrefab);

        //    gridObj.transform.SetParent(transform, false);

        //    gridObj.gameObject.SetActive(false);

        //    GridList.Add(gridObj.GetComponent<GridMono>());
        //}

        yield return new WaitForEndOfFrame();

        callback?.Invoke();
    }


    private void UpdateView()
    {


    }
}
