using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Omnana
{   
    public class IGuiAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mDoInit_0 = new CrossBindingMethodInfo("DoInit");
        static CrossBindingMethodInfo mDoOpen_1 = new CrossBindingMethodInfo("DoOpen");
        static CrossBindingMethodInfo mDoUpdate_2 = new CrossBindingMethodInfo("DoUpdate");
        static CrossBindingMethodInfo mDoDestroy_3 = new CrossBindingMethodInfo("DoDestroy");
        static CrossBindingMethodInfo mDoClose_4 = new CrossBindingMethodInfo("DoClose");

        public override Type BaseCLRType
        {
            get
            {
                return typeof(IGui);
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

        public class Adapter : IGui, CrossBindingAdaptorType
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

            public void DoInit()
            {
                mDoInit_0.Invoke(this.instance);
            }

            public void DoOpen()
            {
                mDoOpen_1.Invoke(this.instance);
            }

            public void DoUpdate()
            {
                mDoUpdate_2.Invoke(this.instance);
            }

            public void DoDestroy()
            {
                mDoDestroy_3.Invoke(this.instance);
            }

            public void DoClose()
            {
                mDoClose_4.Invoke(this.instance);
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


