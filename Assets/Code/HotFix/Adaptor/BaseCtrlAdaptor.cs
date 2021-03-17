using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace Omnana
{
    /// <summary>
    /// 主UI适配器
    /// </summary>
    public class BaseCtrlAdaptor : CrossBindingAdaptor
    {

        public BaseCtrlAdaptor()
        {
        }

        public override Type BaseCLRType
        {
            get
            {
                return typeof(BaseCtrl);
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

        public class Adapter : BaseCtrl, CrossBindingAdaptorType
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

            public override void Loaded(ViewModelBase vm)
            {
                if (mLoaded == null)
                {
                    mLoaded = instance.Type.GetMethod("Loaded", 1);
                }

                if (mLoaded != null)
                {
                    appdomain.Invoke(mLoaded, instance, vm);
                }
                else
                {
                    base.Loaded(vm);
                }
            }

            static IMethod mDispoWith;

            public override void DispoWith()
            {
                if (mLoaded == null)
                {
                    mLoaded = instance.Type.GetMethod("DispoWith", 0);
                }

                if (mLoaded != null)
                {
                    appdomain.Invoke(mLoaded, instance, null);
                }
                else
                {
                    base.DispoWith();
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