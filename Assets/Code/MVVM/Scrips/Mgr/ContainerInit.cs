using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInit : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);

        ServiceLocator.RegisterSingleton<TestCtrl>();
    }
}
