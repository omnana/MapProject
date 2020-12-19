using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtr : MonoBehaviour
{
    public static CameraCtr Instance;

    public Camera MainCamera;

    public GameObject FollowGo;

    public float FollowDelt = 10f;

    public float DeepZ  = -10f;

    public void Init()
    {
        Instance = this;
    }

    public void SetFollow(GameObject obj)
    {
        FollowGo = obj;
    }

    void Update()
    {
        if (FollowGo == null) return;

        var pos = transform.position;

        var followPos = FollowGo.transform.position;

        pos.z = DeepZ;

        followPos.z = DeepZ;

        transform.position = Vector3.Lerp(pos, FollowGo.transform.position, FollowDelt * Time.deltaTime);
    }
}
