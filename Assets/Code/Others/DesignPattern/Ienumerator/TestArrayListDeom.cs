using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestArrayListDeom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ArrayList array = new ArrayList();

        MyArrayList testList = new MyArrayList();

        for (int i = 0; i < 10; i++)
        {
            testList.Add(Random.Range(0, 100));
        }

        foreach (var t in testList)
        {
            Debug.Log((t));
        }

        Debug.Log("after sort :");

        testList.Sort(new TestArrayListComparator());

        foreach (var t in testList)
        {
            Debug.Log((t));
        }
    }

}
