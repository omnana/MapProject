using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace Omnana
{
    /// <summary>
    /// 主UI适配器
    /// </summary>
    public class ViewModelBaseAdaptor : CrossBindingAdaptor
    {

        public ViewModelBaseAdaptor()
        {
        }

        public override Type BaseCLRType
        {
            get
            {
                return typeof(ViewModelBase);
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

        public class Adapter : ViewModelBase, CrossBindingAdaptorType
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

            static IMethod mOnInitialize;

            protected override void OnInitialize()
            {
                if (mOnInitialize == null)
                {
                    mOnInitialize = instance.Type.GetMethod("OnInitialize", 0);
                }

                if (mOnInitialize != null)
                {
                    appdomain.Invoke(mOnInitialize, instance, null);
                }
                else
                {
                    base.OnInitialize();
                }
            }

            static IMethod mOnStartReveal;
            public override void OnStartReveal()
            {
                if (mOnStartReveal == null)
                {
                    mOnStartReveal = instance.Type.GetMethod("OnStartReveal", 0);
                }

                if (mOnStartReveal != null)
                {
                    appdomain.Invoke(mOnStartReveal, instance, null);
                }
                else
                {
                    base.OnStartReveal();
                }
            }

            static IMethod mOnStartHide;
            public override void OnStartHide()
            {
                if (mOnStartHide == null)
                {
                    mOnStartHide = instance.Type.GetMethod("OnStartHide", 0);
                }

                if (mOnStartHide != null)
                {
                    appdomain.Invoke(mOnStartHide, instance, null);
                }
                else
                {
                    base.OnStartHide();
                }
            }

            static IMethod mOnFinishReveal;
            public override void OnFinishReveal()
            {
                if (mOnFinishReveal == null)
                {
                    mOnFinishReveal = instance.Type.GetMethod("OnFinishReveal", 0);
                }

                if (mOnFinishReveal != null)
                {
                    appdomain.Invoke(mOnFinishReveal, instance, null);
                }
                else
                {
                    base.OnFinishReveal();
                }
            }

            static IMethod mOnFinishHide;
            public override void OnFinishHide()
            {
                if (mOnFinishHide == null)
                {
                    mOnFinishHide = instance.Type.GetMethod("OnFinishHide", 0);
                }

                if (mOnFinishHide != null)
                {
                    appdomain.Invoke(mOnFinishHide, instance, null);
                }
                else
                {
                    base.OnFinishHide();
                }
            }

            static IMethod mOnDestory;
            public override void OnDestory()
            {
                if (mOnDestory == null)
                {
                    mOnDestory = instance.Type.GetMethod("OnDestory", 0);
                }

                if (mOnDestory != null)
                {
                    appdomain.Invoke(mOnDestory, instance, null);
                }
                else
                {
                    base.OnDestory();
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