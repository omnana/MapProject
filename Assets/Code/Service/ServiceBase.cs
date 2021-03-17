using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class ServiceBase : IServiceBase
    {
        public ServiceBase()
        {
            Loaded();
        }

        public virtual void Dispose()
        {
        }

        public virtual void Loaded()
        {
        }

        public virtual void Setup()
        {
        }
    }
}