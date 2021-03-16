using UnityEngine;
namespace Omnana
{
    public class GuiManager : Singleton<GuiManager>
    {
        #region 主工程用

        /// <summary>
        /// 同步打开
        /// </summary>
        /// <param name="uiName"></param>
        public void OpenSync<T>(string uiName = "") where T : BaseGui
        {
            if (string.IsNullOrEmpty(uiName))
            {
                uiName = typeof(T).Name;
            }

            BaseGui gui = null;

            var obj = PrefabLoadMgr.Instance.LoadSync(uiName, UIHelper.UIRoot);

            if (obj != null)
            {
                var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                Debug.Log("typeName = " + typeName);

                var appdomain = ILRuntimeHelper.Appdomain;

                gui = obj.GetComponent<T>();

                if (gui == null) gui = obj.AddComponent<T>();

                obj.SetActive(true);

                if (appdomain.LoadedTypes.ContainsKey(typeName))
                {
                    gui.LlRContent = appdomain.Instantiate<HotFixBaseGui>(typeName);

                    gui.LlRContent.SetGameObject(obj, UIHelper.UIRoot);
                }

                gui.Init();
            }

            if (gui != null) gui.Open();
        }

        /// <summary>
        /// 异步打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiName"></param>
        public void OpenAsync<T>(string uiName = "") where T : BaseGui
        {
            if (string.IsNullOrEmpty(uiName))
            {
                uiName = typeof(T).Name;
            }

            PrefabLoadMgr.Instance.LoadAsync(uiName, (assetName, obj) =>
            {
                BaseGui gui = null;

                if (obj != null)
                {
                    obj.transform.SetParent(UIHelper.UIRoot, false);

                    var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                    Debug.Log("typeName = " + typeName);

                    var appdomain = ILRuntimeHelper.Appdomain;

                    gui = obj.GetComponent<T>();

                    if (gui == null) gui = obj.AddComponent<T>();

                    obj.SetActive(true);

                    if (appdomain.LoadedTypes.ContainsKey(typeName))
                    {
                        gui.LlRContent = appdomain.Instantiate<HotFixBaseGui>(typeName);

                        gui.LlRContent.SetGameObject(obj, UIHelper.UIRoot);
                    }

                    gui.Init();
                }

                if (gui != null) gui.Open();

            }, UIHelper.UIRoot);
        }

        public void Close(BaseGui gui)
        {
            gui.Close();

            PrefabLoadMgr.Instance.Destroy(gui.gameObject);
        }

        #endregion

        #region 热更工程用

        /// <summary>
        /// 直接从热更项目同步打开界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiName"></param>
        public void OpenSyncFormHotFix<T>(string uiName = "") where T : HotFixBaseGui
        {
            if (string.IsNullOrEmpty(uiName))
            {
                uiName = typeof(T).Name;
            }

            Debug.Log("界面昵称 = " + uiName);

            BaseGui gui = null;

            var obj = PrefabLoadMgr.Instance.LoadSync(uiName, UIHelper.UIRoot);

            if (obj != null)
            {
                var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                Debug.Log("typeName = " + typeName);

                var appdomain = ILRuntimeHelper.Appdomain;

                gui = obj.GetComponent<BaseGui>();

                if (gui == null) gui = obj.AddComponent<BaseGui>();

                obj.SetActive(true);

                if (appdomain.LoadedTypes.ContainsKey(typeName))
                {
                    gui.LlRContent = appdomain.Instantiate<T>(typeName);

                    gui.LlRContent.SetGameObject(obj, UIHelper.UIRoot);
                }

                gui.Init();
            }

            if (gui != null) gui.Open();
        }

        /// <summary>
        /// 从热更工程异步打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiName"></param>
        public void OpenAsyncFromHotFix<T>(string uiName = "") where T : HotFixBaseGui
        {
            if (string.IsNullOrEmpty(uiName))
            {
                uiName = typeof(T).Name;
            }

            Debug.Log("界面昵称 = " + uiName);

            PrefabLoadMgr.Instance.LoadAsync(uiName, (assetName, obj) =>
            {
                BaseGui gui = null;

                if (obj != null)
                {
                    obj.transform.SetParent(UIHelper.UIRoot, false);

                    var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                    Debug.Log("typeName = " + typeName);

                    var appdomain = ILRuntimeHelper.Appdomain;

                    gui = obj.GetComponent<BaseGui>();

                    if (gui == null) gui = obj.AddComponent<BaseGui>();

                    obj.SetActive(true);

                    if (appdomain.LoadedTypes.ContainsKey(typeName))
                    {
                        gui.LlRContent = appdomain.Instantiate<T>(typeName);

                        gui.LlRContent.SetGameObject(obj, UIHelper.UIRoot);
                    }

                    gui.Init();
                }

                if (gui != null) gui.Open();

            }, UIHelper.UIRoot);
        }

        public void CloseFromHotFix(HotFixBaseGui gui)
        {
            gui.OnClose();

            PrefabLoadMgr.Instance.Destroy(gui.gameObject);
        }

        #endregion
    }
}