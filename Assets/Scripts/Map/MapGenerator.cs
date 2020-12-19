using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public Room roomPrefab;

    public void LoadMap(Action callback)
    {
        StartCoroutine(OnLoadMap(callback));
    }

    private IEnumerator OnLoadMap(Action callback)
    {
        var roomDatas = MapAreaMgr.Instance.RoomDatas;

        for (var i = 0; i < roomDatas.Count; i++)
        {
            var roomObj = Instantiate(roomPrefab);

            roomObj.transform.SetParent(transform, false);

            var room = roomObj.GetComponent<Room>();

            room.SetData(roomDatas[i]);
        }

        var corridorDatas = MapAreaMgr.Instance.CorridorDatas;

        for (var i = 0; i < corridorDatas.Count; i++)
        {
            var roomObj = Instantiate(roomPrefab);

            roomObj.transform.SetParent(transform, false);

            var room = roomObj.GetComponent<Room>();

            room.SetData(corridorDatas[i]);
        }

        yield return new WaitForEndOfFrame();

        callback?.Invoke();
    }
}
