using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamIEnumrator : IEnumerator
{
    private SamIEnumerable parent;

    private int position;

    public SamIEnumrator(SamIEnumerable parent)
    {
        this.parent = parent;

        position = -1;
    }

    public object Current
    {
        get
        {
            if(position == -1 || position == parent.values.Length)
            {
                throw new System.NotImplementedException();
            }

            int index = position + parent.startPoint;

            index %= parent.values.Length;

            return parent.values[index];
        }
    }


    public bool MoveNext()
    {
        if(position != parent.values.Length)
        {
            position++;
        }

        return position < parent.values.Length;
    }

    public void Reset()
    {
        position = -1;
    }
}
