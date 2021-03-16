using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Omnana
{
    public class Code : MonoBehaviour
    {
        public ControllerInit ControllerInit;

        public ServiceContainerInit ServiceContainerInit;

        private HotFixMgr hotFixMgr;

        private AssetBundleMgr assetBundleMgr;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            ControllerInit.Load();

            ServiceContainerInit.Load();

            //hotFixMgr = Singleton<HotFixMgr>.GetInstance();

            //MessageAggregator<object>.Instance.Publish(MessageType.DownloadFinish, this, null);

        }

        private IEnumerator Start()
        {
            MyAssetBundleMgr.Instance.LoadMainfest();

            Debug.Log("加载资源Manifest。。。");

            var loadTableEnd = false;

            TableHelper.DownloadFinishCallabck = () => { loadTableEnd = true; };

            TableHelper.StartLoad();

            Debug.Log("加载配置表。。。");

            while (!loadTableEnd) yield return null;

            var loadHotFixEnd = false;

            StartCoroutine(ILRuntimeHelper.LoadHotFix_ProjectAssembly(() =>
            {
                loadHotFixEnd = true;
            }));

            Debug.Log("加载热更DLL。。。");

            while (!loadHotFixEnd) yield return null;

            //// 热更完毕或者已下载
            //hotFixMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
            //{
            //    assetBundleMgr.LoadMainfest();

            //    Singleton<MyAssetBundleMgr>.GetInstance().LoadMainfest();

            //    TableMgrLoader.StartLoad();

            //    TableMgrLoader.DownloadFinishCallabck = () =>
            //    {
            //        StartCoroutine(ILRuntimeHelper.LoadHotFix_ProjectAssembly(() => { }));

            //        MessageAggregator<object>.Instance.Publish("DownloadFinish", this, null);
            //    };
            //});

            //hotFixMgr.Start();

            //hotFixMgr.CheckCDNVersion();

            Debug.Log("加载完毕！！");

            loadEnd = true;

            //GuiManager.Instance.OpenAsync<TestGui>();
        }

        private bool loadEnd = false;

        private void Update()
        {
            if (loadEnd && Input.GetKeyDown(KeyCode.A))
            {
                GuiManager.Instance.OpenAsync<TestGui>();
            }
        }
    }
}