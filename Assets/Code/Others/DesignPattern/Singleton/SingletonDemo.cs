using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDemo : MonoBehaviour
{
    void Start()
    {
        Singleton<TestMgr>.GetInstance().Init();
    }
}
