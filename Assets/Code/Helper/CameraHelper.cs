using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper 
{
    private static Camera uiCamera;

    public static Camera UICamera
    {
        get
        {
            if (uiCamera == null)
            {
                uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            }

            return uiCamera;
        }
    }

}
