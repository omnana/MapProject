using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchableEnumerator : IEnumerator
{
    private IEnumerator enumerator;

    private Action exceptionCallback;

    private bool subExceptionCatched = false;

    public CatchableEnumerator(IEnumerator tor, Action expCallback = null)
    {
        enumerator = tor;

        exceptionCallback = expCallback;
    }

    private void BindExceptionCallback(Action expCallback)
    {
        exceptionCallback = expCallback;
    }

    public object Current
    {
        get
        {
            if(enumerator.Current != null)
            {
                if (enumerator.Current is CatchableEnumerator caRator)
                {
                    caRator.BindExceptionCallback(SubExceptionCall);
                }
                else if (enumerator.Current is IEnumerator ieRator)
                {
                    return new CatchableEnumerator(ieRator, SubExceptionCall);
                }
            }

            return enumerator.Current;
        }
    }

    public bool MoveNext()
    {
        if (subExceptionCatched)
        {
            exceptionCallback?.Invoke();

            return false;
        }

        var result = false;

        try
        {
            result = enumerator.MoveNext();
        }
        catch
        {
            result = false;
            exceptionCallback?.Invoke();
        }

        return result;
    }

    public void Reset()
    {
        enumerator.Reset();
    }

    private void SubExceptionCall()
    {
        subExceptionCatched = true;
    }
}
