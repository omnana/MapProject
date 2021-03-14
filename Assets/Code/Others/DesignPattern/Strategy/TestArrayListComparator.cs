using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

public class TestArrayListComparator : IComparer<object>
{
    public int Compare(object x, object y)
    {
        var xv = (int) x;
        var yv = (int) y;

        if (xv > yv) return 1;
        else if (xv < yv) return -1;
        else return 0;
    }
}
