using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCtr : MonoBehaviour
{
    private Camera mainCam;

    public void Init()
    {
        mainCam = CameraCtr.Instance.MainCamera;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var mousePos = Input.mousePosition;

            Vector2 worldPos = mainCam.ScreenToWorldPoint(mousePos);

            var coord = worldPos.Vector2ToCoord();

            Debug.Log(coord);
        }
    }
}
