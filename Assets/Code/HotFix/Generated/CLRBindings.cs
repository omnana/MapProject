using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            HotFixBaseGui_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            Omnana_GameObjectRef_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            UIExtensions_Binding.Register(app);
            System_String_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            DG_Tweening_ShortcutExtensions_Binding.Register(app);
            Omnana_ServiceBase_Binding.Register(app);
            Omnana_BaseCtrl_Binding.Register(app);
            Omnana_BindableProperty_1_String_Binding.Register(app);
            Omnana_ServiceLocator_Binding.Register(app);
            Omnana_MessageAggregator_1_Object_Binding.Register(app);
            Omnana_MessageAggregator_1_String_Binding.Register(app);
            Omnana_MessageArgs_1_String_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector3_Binding_Binder = null;
        }
    }
}
