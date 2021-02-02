using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCtrl : BaseCtrl
{
    public TestCtrl()
    {
        Debug.Log("TestCtrl init");
    }

    public string GetData()
    {
        return "TestCtrLData";
    }
}
