using System;

namespace Omnana
{
    public class TransientObjectFactory : IObjectFactory
    {
        public object AcquireObject(string className)
        {
            throw new NotImplementedException();
        }

        public object AcquireObject(Type type)
        {
            throw new NotImplementedException();
        }

        public object AcquireObject<TInstance>() where TInstance : class, new()
        {
            var instance = new TInstance();

            return instance;
        }

        public void ReleaseObject(object obj)
        {
            throw new NotImplementedException();
        }
    }
}