using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamIEnumerable : IEnumerable
{
    public object[] values;

    public int startPoint;

    public SamIEnumerable(object[] values, int startPoint)
    {
        this.values = values;

        this.startPoint = startPoint;
    }

    public IEnumerator GetEnumerator()
    {
        return new SamIEnumrator(this);
    }
}
