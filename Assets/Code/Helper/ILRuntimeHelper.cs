using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System.IO;
using System;

namespace Omnana
{
    public class ILRuntimeHelper
    {
        public static ILRuntime.Runtime.Enviorment.AppDomain Appdomain { get; private set; }

        static MemoryStream fs;

        static MemoryStream p;

        public static IEnumerator LoadHotFix_ProjectAssembly(Action callback)
        {
            Appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            var webRequest = UnityWebRequest.Get(HotFixUtils.HotFixDllPath);

            yield return webRequest.SendWebRequest();

            byte[] dll = null;

            if (webRequest.isNetworkError)
            {
                Debug.Log("Download Error:" + webRequest.error);
            }
            else
            {
                dll = webRequest.downloadHandler.data;
            }

            webRequest.Dispose();

            // 正式版隐藏
            webRequest = UnityWebRequest.Get(HotFixUtils.HotFixPdbPath);

            yield return webRequest.SendWebRequest();

            byte[] pdb = null;

            if (webRequest.isNetworkError)
            {
                Debug.Log("Download Error:" + webRequest.error);
            }
            else
            {
                pdb = webRequest.downloadHandler.data;
            }

            webRequest.Dispose();

            fs = new MemoryStream(dll);

            p = new MemoryStream(pdb);

            try
            {
                Appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("加载热更DLL失败，请确保已经通过VS打开Assets/Samples/ILRuntime/DHotFix/HotFix.sln编译过热更DLL");
            }


#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            Appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

            if (Application.isEditor)
                Appdomain.DebugService.StartDebugService(56000);

            InitializeILRuntime();

            //请在生成了绑定代码后解除下面这行的注释
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(Appdomain);

            callback?.Invoke();
        }

        public static void Dispose()
        {
            fs.Dispose();

            p.Dispose();
        }

        private static void InitializeILRuntime()
        {
            RegisterCrossBindingAdaptors();

            RegisterDelegateConvertors();
        }

        private static void RegisterCrossBindingAdaptors()
        {
            Appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            Appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            Appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            Appdomain.RegisterCrossBindingAdaptor(new IGuiAdapter());
            Appdomain.RegisterCrossBindingAdaptor(new ServiceBaseAdaptor());
            Appdomain.RegisterCrossBindingAdaptor(new BaseCtrlAdaptor());
            Appdomain.RegisterCrossBindingAdaptor(new ViewModelBaseAdaptor());
        }

        private static void RegisterDelegateConvertors()
        {
            Appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((action) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((System.Action)action)();
                });
            });

            Appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<GameObject>>((action) =>
            {
                return new UnityEngine.Events.UnityAction<GameObject>((obj) =>
                {
                    ((System.Action<GameObject>)action)(obj);
                });
            });
        }
    }
}
