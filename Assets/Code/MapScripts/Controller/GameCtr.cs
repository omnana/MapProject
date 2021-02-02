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

        var arr = new int[] { 2, 7, 5, 9, 3, 1 };
        //SortUtil.SelectSort(arr);
        //SortUtil.QuickSort(arr, 0, arr.Length - 1);
        //SortUtil.HeapSort(arr);

        //var maxHeap = new MinHeap1();

        //foreach (var a in arr) maxHeap.Add(a);

        //maxHeap.MaxHeapInit();

        //maxHeap.Insert(100);

        //maxHeap.DeleteTop();

        var arr1 = new string[] { "a", "bc" };
        var arr2 = new string[] { "ab", "c" };

        var a = ArrayStringsAreEqual(arr1, arr2);

        Debug.Log(Hash("asdasd"));
        Debug.Log(Hash("asdasds"));
        Debug.Log(Hash("asdasdss"));
        Debug.Log(Hash("asdasdsss"));
    }
    private int Hash(object key)
    {
        int h = key.GetHashCode();

        //return (h ^ (h >> 16)) & (10 - 1); //capicity 表示散列表的大小
        return h & 0x7FFFFFFF;
    }

    public bool ArrayStringsAreEqual(string[] word1, string[] word2)
    {
        var w1 = 0; var w2 = 0; var i = 0; var j = 0;

        while(w1 < word1.Length && w2 < word2.Length)
        {
            if (word1[w1][i] != word2[w2][j]) return false;

            if (++i == word1[w1].Length)
            {
                w1++;

                i = 0;
            }

            if (++j == word2[w2].Length)
            {
                w2++;

                j = 0;
            }
        }

        return w1 == word1.Length && w2 == word2.Length;
    }

    public bool HalvesAreAlike(string s)
    {
        var i = 0;

        var num1 = 0;

        var num2 = 0;

        var half = s.Length / 2;

        while (i < half)
        {
            if (IsVowel(s[i])) num1++;

            if (IsVowel(s[half + i])) num2++;

            i++;
        }

        return num1 == num2;
    }

    private bool IsVowel(char a)
    {
        return a == 'a' || a == 'e' || a == 'i' || a == 'o' || a == 'u' ||
            a == 'A' || a == 'E' || a == 'I' || a == 'O' || a == 'U';
    }


    void Start()
    {
        //MapAreaMgr.Instance.LoadData();

        //gridNum = GetGridMaxNum();

        //MapGenerator.LoadMap(LoadMapEnd);
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
