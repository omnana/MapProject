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
    unsafe class IlRuntimeBaseGui_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::IlRuntimeBaseGui);
            args = new Type[]{};
            method = type.GetMethod("OnInit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnInit_0);
            args = new Type[]{};
            method = type.GetMethod("OnOpen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnOpen_1);
            args = new Type[]{};
            method = type.GetMethod("OnUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnUpdate_2);
            args = new Type[]{};
            method = type.GetMethod("OnClose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnClose_3);
            args = new Type[]{};
            method = type.GetMethod("Destroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Destroy_4);


        }


        static StackObject* OnInit_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::IlRuntimeBaseGui instance_of_this_method = (global::IlRuntimeBaseGui)typeof(global::IlRuntimeBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnInit();

            return __ret;
        }

        static StackObject* OnOpen_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::IlRuntimeBaseGui instance_of_this_method = (global::IlRuntimeBaseGui)typeof(global::IlRuntimeBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnOpen();

            return __ret;
        }

        static StackObject* OnUpdate_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::IlRuntimeBaseGui instance_of_this_method = (global::IlRuntimeBaseGui)typeof(global::IlRuntimeBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnUpdate();

            return __ret;
        }

        static StackObject* OnClose_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::IlRuntimeBaseGui instance_of_this_method = (global::IlRuntimeBaseGui)typeof(global::IlRuntimeBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnClose();

            return __ret;
        }

        static StackObject* Destroy_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::IlRuntimeBaseGui instance_of_this_method = (global::IlRuntimeBaseGui)typeof(global::IlRuntimeBaseGui).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Destroy();

            return __ret;
        }



    }
}
