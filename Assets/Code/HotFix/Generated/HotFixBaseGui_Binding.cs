using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class HotFixBaseGui_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::HotFixBaseGui);
            args = new Type[]{};
            method = type.GetMethod("DoInit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoInit_0);
            args = new Type[]{};
            method = type.GetMethod("get_GameObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_GameObject_1);
            args = new Type[]{};
            method = type.GetMethod("get_Transform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Transform_2);
            args = new Type[]{};
            method = type.GetMethod("DoOpen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoOpen_3);
            args = new Type[]{};
            method = type.GetMethod("DoUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoUpdate_4);
            args = new Type[]{};
            method = type.GetMethod("DoClose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoClose_5);
            args = new Type[]{};
            method = type.GetMethod("DoDestroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoDestroy_6);


        }


        static StackObject* DoInit_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoInit();

            return __ret;
        }

        static StackObject* get_GameObject_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GameObject;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Transform_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Transform;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* DoOpen_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoOpen();

            return __ret;
        }

        static StackObject* DoUpdate_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoUpdate();

            return __ret;
        }

        static StackObject* DoClose_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoClose();

            return __ret;
        }

        static StackObject* DoDestroy_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HotFixBaseGui instance_of_this_method = (global::HotFixBaseGui)typeof(global::HotFixBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoDestroy();

            return __ret;
        }



    }
}
