using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public GridMono gridPrefab;

    public List<GridMono> GridList;

    public void Init(int level)
    {
        GridList = new List<GridMono>();

        //var gridNum = Config.MapAreaWidth * Config.MapAreaHeight / Config.MapAreaNum * Config.MapAreaNum;

        //for (var i = 0; i < gridNum; i++)
        //{
        //    var grid = Instantiate(gridPrefab);

        //    grid.name = string.Format("Grid_{0}_{1}", level, i);

        //    grid.transform.SetParent(transform, false);

        //    grid.gameObject.SetActive(false);

        //    GridList.Add(grid);
        //}

        gameObject.SetActive(true);
    }
}
