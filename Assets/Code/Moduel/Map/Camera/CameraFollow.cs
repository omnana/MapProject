﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    public Camera MainCamera;

    public GameObject FollowGo;

    public float FollowDelt = 5f;

    public float DeepZ  = -10f;

    public float cameraDistance = 12.0f;

    public Vector3[] Corners = new Vector3[4];

    public float ExtraValue = 10f;

    public void Init()
    {
        Instance = this;
    }

    public void SetFollow(GameObject obj)
    {
        FollowGo = obj;

        var followPos = FollowGo.transform.position;

        var pos = transform.position;

        pos.x = followPos.x;

        pos.y = followPos.y;

        transform.position = pos;
    }

    private void UpdateFollow()
    {
        if (!FollowGo) return;

        var pos = transform.position;

        var followPos = FollowGo.transform.position;

        pos.z = DeepZ;

        followPos.z = DeepZ;

        var disSub = Vector2.Distance(pos, followPos);

        if (disSub < 1f) return;

        transform.DOMove(followPos, 0.2f);
    }

    private void FindCorners()
    {
        if (MainCamera == null) return;

        cameraDistance = -MainCamera.transform.position.z;

        Vector3[] corners = GetCorners(cameraDistance);

#if UNITY_EDITOR
        Debug.DrawLine(corners[0], corners[1], Color.red);
        Debug.DrawLine(corners[1], corners[3], Color.red);
        Debug.DrawLine(corners[3], corners[2], Color.red);
        Debug.DrawLine(corners[2], corners[0], Color.red);
#endif

    }


    private Vector3[] GetCorners(float distance)
    {
        var tx = MainCamera.transform;

        float halfFOV = (MainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = MainCamera.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        // UpperLeft
        Corners[0] = tx.position - (tx.right * width);
        Corners[0] += tx.up * height;
        Corners[0] += tx.forward * distance;

        // UpperRight
        Corners[1] = tx.position + (tx.right * width);
        Corners[1] += tx.up * height;
        Corners[1] += tx.forward * distance;

        // LowerLeft
        Corners[2] = tx.position - (tx.right * width);
        Corners[2] -= tx.up * height;
        Corners[2] += tx.forward * distance;

        // LowerRight
        Corners[3] = tx.position + (tx.right * width);
        Corners[3] -= tx.up * height;
        Corners[3] += tx.forward * distance;

        return Corners;
    }

    void Update()
    {
        UpdateFollow();

        FindCorners();
    }
}
