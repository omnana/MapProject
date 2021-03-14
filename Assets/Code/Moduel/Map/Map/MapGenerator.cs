using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    //public Area areaPrefab;

    public GridMono gridPrefab;

    public List<GridMono> GridList;

    public List<Area> areaList;

    private void Awake()
    {

    }

    public void LoadMap(Action callback)
    {
        StartCoroutine(OnLoadMap(callback));
    }

    private IEnumerator OnLoadMap(Action callback)
    {
        var gridDatas = MapAreaMgr.Instance.AllGrids;

        //for (var i = 0; i < Config.MapAreaNum; i++)
        //{
        //    var area = Instantiate(areaPrefab);

        //    area.name = string.Format("Area_{0}", i);

        //    area.transform.SetParent(transform, false);

        //    area.Init(i);

        //    areaList.Add(area);
        //}

        var gridNum = Config.MapAreaWidth * Config.MapAreaHeight / Config.MapAreaNum * Config.MapAreaNum;

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
