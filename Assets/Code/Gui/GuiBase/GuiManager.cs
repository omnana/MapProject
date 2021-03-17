using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class GuiManager : Singleton<GuiManager>
    {
        private Dictionary<string, BaseGui> guiDic;

        private HashSet<string> openList;

        private HashSet<string> closeList;

        private const int Buffer_Num = 4;

        private int maxOrder = 0;

        private void Awake()
        {
            guiDic = new Dictionary<string, BaseGui>();

            openList = new HashSet<string>();

            closeList = new HashSet<string>();
        }

        private void Update()
        {
            CheckGuiBuffer();
        }

        private void CheckGuiBuffer()
        {
            if (closeList.Count < Buffer_Num) return;

            foreach(var uiName in closeList)
            {
                if (guiDic.ContainsKey(uiName))
                {
                    PrefabLoadMgr.Instance.Destroy(guiDic[uiName].gameObject);

                    guiDic.Remove(uiName);
                }
            }

            closeList.Clear();
        }

        private void SetGuiSortingOrder(BaseGui gui)
        {
            if(gui.SortingOrder < maxOrder)
            {
                gui.SortingOrder = ++maxOrder;
            }
        }
        private BaseGui GetGui(string uiName)
        {
            if (guiDic.ContainsKey(uiName))
            {
                openList.Add(uiName);

                closeList.Remove(uiName);

                return guiDic[uiName];
            }

            return null;
        }

        private void CloseGui(string uiName)
        {
            if (guiDic.ContainsKey(uiName))
            {
                openList.Remove(uiName);

                closeList.Add(uiName);
            }
        }

        public void ClearAll()
        {
            foreach (var gui in guiDic.Values)
            {
                PrefabLoadMgr.Instance.Destroy(gui.gameObject);
            }

            openList.Clear();

            closeList.Clear();

            guiDic.Clear();
        }

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

            var gui = GetGui(uiName);

            if(gui != null)
            {
                gui.gameObject.SetActive(true);

                SetGuiSortingOrder(gui);

                gui.Open();

                return;
            }

            var obj = PrefabLoadMgr.Instance.LoadSync(uiName, UIHelper.UIRoot);

            if (obj != null)
            {
                var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                Debug.Log("typeName = " + typeName);

                var appdomain = ILRuntimeHelper.Appdomain;

                if (appdomain.LoadedTypes.ContainsKey(typeName))
                {
                    gui = obj.AddComponent<BaseGui>();

                    gui.HotFixContent = appdomain.Instantiate<HotFixBaseGui>(typeName);

                    gui.HotFixContent.SetGameObject(obj, UIHelper.UIRoot);
                }
                else
                {
                    gui = obj.AddComponent<T>();
                }

                obj.SetActive(true);

                guiDic.Add(uiName, gui);

                SetGuiSortingOrder(gui);

                gui.Init();
            }

            if (gui != null) gui.DoOpen();
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

            var gui = GetGui(uiName);

            if (gui != null)
            {
                gui.gameObject.SetActive(true);

                SetGuiSortingOrder(gui);

                gui.DoOpen();

                return;
            }

            PrefabLoadMgr.Instance.LoadAsync(uiName, (assetName, obj) =>
            {
                if (obj != null)
                {
                    obj.transform.SetParent(UIHelper.UIRoot, false);

                    var typeName = string.Format(HotFixUtils.HotFixGuiNameFormat, uiName);

                    Debug.Log("typeName = " + typeName);

                    var appdomain = ILRuntimeHelper.Appdomain;

                    if (appdomain.LoadedTypes.ContainsKey(typeName))
                    {
                        gui = obj.AddComponent<BaseGui>();

                        gui.HotFixContent = appdomain.Instantiate<HotFixBaseGui>(typeName);

                        gui.HotFixContent.SetGameObject(obj, UIHelper.UIRoot);
                    }
                    else
                    {
                        gui = obj.AddComponent<T>();
                    }

                    obj.SetActive(true);

                    SetGuiSortingOrder(gui);

                    guiDic.Add(uiName, gui);

                    gui.Init();
                }

                if (gui != null) gui.Open();

            }, UIHelper.UIRoot);
        }

        public void Close(BaseGui gui)
        {
            CloseGui(gui.name);

            gui.gameObject.SetActive(false);

            gui.Close();
        }

        #endregion
    }
}