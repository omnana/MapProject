using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

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

    private void UpdateFollow()
    {
        if (FollowGo == null) return;

        var pos = transform.position;

        var followPos = FollowGo.transform.position;

        pos.z = DeepZ;

        followPos.z = DeepZ;

        var disSub = Vector2.Distance(pos, followPos);

        if (disSub < 0.1f) return;

        transform.position = Vector3.Lerp(pos, FollowGo.transform.position, FollowDelt * Time.deltaTime);
    }

    private void UpdateCameraArea()
    {

    }

    void Update()
    {
        UpdateFollow();

        UpdateCameraArea();
    }
}
