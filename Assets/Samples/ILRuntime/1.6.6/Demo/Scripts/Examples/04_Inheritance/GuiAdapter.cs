using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

/// <summary>
/// 主UI适配器
/// </summary>
public class GuiAdapter : CrossBindingAdaptor
{
    static CrossBindingMethodInfo mInit = new CrossBindingMethodInfo("OnInit");
    static CrossBindingMethodInfo mOpen = new CrossBindingMethodInfo("Open");
    static CrossBindingMethodInfo mClose = new CrossBindingMethodInfo("OnClose");
    static CrossBindingMethodInfo mDestroy = new CrossBindingMethodInfo("Destroy");

    public GuiAdapter()
    {
    }

    public override Type BaseCLRType
    {
        get
        {
            return typeof(global::IlRuntimeBaseGui);
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

    public class Adapter : global::IlRuntimeBaseGui, CrossBindingAdaptorType
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

        public override void OnInit()
        {
            if (mInit.CheckShouldInvokeBase(this.instance))
                base.OnInit();
            else
                mInit.Invoke(this.instance);
        }

        public override void OnOpen()
        {
            if (mOpen.CheckShouldInvokeBase(this.instance))
                base.OnOpen();
            else
                mOpen.Invoke(this.instance);
        }

        public override void OnClose()
        {
            if (mClose.CheckShouldInvokeBase(this.instance))
                base.OnClose();
            else
                mClose.Invoke(this.instance);
        }

        public override void Destroy()
        {
            if (mDestroy.CheckShouldInvokeBase(this.instance))
                base.Destroy();
            else
                mDestroy.Invoke(this.instance);
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