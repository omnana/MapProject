using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Food
{
    public override void Init()
    {
        Debug.Log("Apple Init");
    }

    public override void Update()
    {
        Debug.Log("Apple Update");
    }


    public override void Destroy()
    {
        Debug.Log("Apple Destroy");
    }
}
