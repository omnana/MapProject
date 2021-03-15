using UnityEngine;

public class GuiManager : MonoBehaviour
{
    public virtual void Awake()
    {
    }

    /// <summary>
    /// 同步打开
    /// </summary>
    /// <param name="uiName"></param>
    public void OpenSync<T>(string uiName = "") where T : BaseGui
    {
        if (string.IsNullOrEmpty(uiName))
        {
            uiName = typeof(T).ToString();
        }

        BaseGui gui = null;

        var uiObj = Singleton<PrefabLoadMgr>.GetInstance().LoadSync(uiName, UIHelper.UIRoot);

        if (uiObj != null)
        {
            gui = uiObj.GetComponent<T>();

            uiObj.SetActive(true);

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
            uiName = typeof(T).ToString();
        }

        Singleton<PrefabLoadMgr>.GetInstance().LoadAsync(uiName, (assetName, obj) =>
        {
            BaseGui gui = null;

            if (obj != null)
            {
                obj.transform.SetParent(UIHelper.UIRoot, false);

                var typeName = string.Format("HotFix_Project.Gui.{0}", uiName);

                Debug.Log("typeName = " + typeName);

                var appdomain = ILRuntimeHelper.Appdomain;

                gui = obj.GetComponent<T>();

                if (gui == null) gui = obj.AddComponent<T>();

                obj.SetActive(true);

                if (appdomain.LoadedTypes.ContainsKey(typeName))
                {
                    gui.LlRContent = appdomain.Instantiate<IlRuntimeBaseGui>(typeName);

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

        Singleton<PrefabLoadMgr>.GetInstance().Destroy(gui.gameObject);
    }
}
