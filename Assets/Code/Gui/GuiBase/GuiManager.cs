using UnityEngine;

public class GuiManager : MonoBehaviour
{
    public static GuiManager Instance { get; private set; }

    public Transform UiRoot;

    private PrefabLoadMgr prefabLoadMgr;

    public virtual void Awake()
    {
        Instance = this;

        prefabLoadMgr = ServiceLocator.Resolve<PrefabLoadMgr>();
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

        var uiObj = prefabLoadMgr.LoadSync(uiName, UiRoot);

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

        prefabLoadMgr.LoadAsync(uiName, (assetName, obj) =>
        {
            BaseGui gui = null;

            if (obj != null)
            {
                obj.transform.SetParent(UiRoot, false);

                var typeName = string.Format("HotFix_Project.Gui.{0}", uiName);

                Debug.Log("typeName = " + typeName);

                var appdomain = ILRuntimeHelper.Appdomain;

                gui = obj.GetComponent<T>();

                if (gui == null) gui = obj.AddComponent<T>();

                obj.SetActive(true);

                if (appdomain.LoadedTypes.ContainsKey(typeName))
                {
                    gui.LlRContent = appdomain.Instantiate<IlRuntimeBaseGui>(typeName);

                    gui.LlRContent.SetGameObject(obj, UiRoot);
                }

                gui.Init();
            }

            if (gui != null) gui.Open();

        }, UiRoot);
    }

    public void Close(BaseGui gui)
    {
        gui.Close();

        prefabLoadMgr.Destroy(gui.gameObject);
    }
}
