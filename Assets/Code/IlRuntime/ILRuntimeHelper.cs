using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ILRuntimeHelper
{
    public static ILRuntime.Runtime.Enviorment.AppDomain Appdomain { get; private set; }

    static MemoryStream fs;

    static MemoryStream p;

    public static IEnumerator LoadHotFix_ProjectAssembly(Action callback)
    {
        Appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        var webRequest = UnityWebRequest.Get(AssetBundles.Utility.GetAssetBundlePath("HotFix_Project.dll"));

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

        webRequest = UnityWebRequest.Get(AssetBundles.Utility.GetAssetBundlePath("HotFix_Project.pdb"));

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
            Debug.LogError("加载热更DLL失败，请确保已经通过VS打开Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln编译过热更DLL");
        }


#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        Appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

        if (Application.isEditor)
            Appdomain.DebugService.StartDebugService(56000);

        //Appdomain.RegisterCrossBindingAdaptor(new GuiAdapter());

        callback?.Invoke();
    }

    public static void Dispose()
    {
        fs.Dispose();

        p.Dispose();
    }
}
