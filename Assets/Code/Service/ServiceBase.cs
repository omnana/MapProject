using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceBase : MonoBehaviour, IServiceBase
{
    public virtual void Dispose()
    {
    }

    public virtual void Loaded()
    {
    }

    public virtual void Setup()
    {
    }

    public virtual IEnumerator SetupAsync()
    {
        yield return null;
    }
}
