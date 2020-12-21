using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCtr : MonoBehaviour
{
    private Camera mainCam;

    public static InputCtr Instance;

    public void Init()
    {
        Instance = this;

        mainCam = CameraFollow.Instance.MainCamera;
    }

    public Coordinate GetMouseCoord()
    {
        var mousePos = Input.mousePosition;

        mousePos.z = -mainCam.transform.position.z;

        Vector2 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        var coord = worldPos.Vector2ToCoord();

        //Debug.Log(coord);

        return coord;
    }
}
