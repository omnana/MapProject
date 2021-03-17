using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace Omnana
{
    /// <summary>
    /// 主UI适配器
    /// </summary>
    public class ServiceBaseAdaptor : CrossBindingAdaptor
    {

        public ServiceBaseAdaptor()
        {
        }

        public override Type BaseCLRType
        {
            get
            {
                return typeof(ServiceBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);

        }

        public class Adapter : ServiceBase, CrossBindingAdaptorType
        {
            ILTypeInstance instance;

            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            static IMethod mLoaded;

            public override void Loaded()
            {
                if (mLoaded == null)
                {
                    mLoaded = instance.Type.GetMethod("Loaded", 0);
                }

                if (mLoaded != null)
                {
                    appdomain.Invoke(mLoaded, instance, null);
                }
                else
                {
                    base.Loaded();
                }
            }

            static IMethod mSetup;

            public override void Setup()
            {
                if (mLoaded == null)
                {
                    mLoaded = instance.Type.GetMethod("Setup", 0);
                }

                if (mLoaded != null)
                {
                    appdomain.Invoke(mLoaded, instance, null);
                }
                else
                {
                    base.Setup();
                }
            }

            static IMethod mDispose;

            public override void Dispose()
            {
                if (mDispose == null)
                {
                    mDispose = instance.Type.GetMethod("Dispose", 0);
                }

                if (mDispose != null)
                {
                    appdomain.Invoke(mLoaded, instance, null);
                }
                else
                {
                    base.Dispose();
                }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }

        }
    }
}