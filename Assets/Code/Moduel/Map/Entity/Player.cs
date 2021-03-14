using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Player : MonoBehaviour
{
    public Coordinate Coordinate;

    public float moveSpeed = 2f;

    private Coroutine walkCoroutine;

    void Awake()
    {

    }

    public void SetCoord(Coordinate coord)
    {
        Coordinate = coord;

        transform.position = coord.CoordinateToVector2();

        gameObject.SetActive(true);
    }

    public void SetPos(Vector2 pos)
    {
        var coord = pos.Vector2ToCoord();

        Coordinate = coord;
    }


    public void MoveTo(Coordinate coord, Action callback = null)
    {
        var pos = coord.CoordinateToVector2();

        transform.DOMove(pos, moveSpeed).onComplete = () => { callback?.Invoke(); };
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);

                walkCoroutine = null;
            }

            var targetCoord = InputCtr.Instance.GetMouseCoord();

            var path = FindPathTool.FindPath(Coordinate, targetCoord);

            if (path.Count > 0)
            {
                walkCoroutine = StartCoroutine(Walk(path));
            }
        }
    }

    private IEnumerator Walk(List<Coordinate> path)
    {
        var index = 1;

        var moving = false;

        while (index < path.Count)
        {
            moving = true;

            MoveTo(path[index], () => { moving = false; });

            while (moving)
            {
                SetPos(transform.position);

                yield return null;
            }

            index++;
        }

        walkCoroutine = null;
    }
}
