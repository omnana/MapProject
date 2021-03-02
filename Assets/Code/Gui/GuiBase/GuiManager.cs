using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    public void OpenSync<T>(string uiName = "")
    {
        if (string.IsNullOrEmpty(uiName))
        {
            uiName = typeof(T).ToString();
        }

        BaseGui gui = null;

        var uiObj = prefabLoadMgr.LoadSync(uiName, UiRoot);

        if (uiObj != null)
        {
            gui = uiObj.GetComponent<BaseGui>();

            gui.Init();

            uiObj.SetActive(true);
        }

        if (gui != null) gui.Open();
    }

    /// <summary>
    /// 异步打开
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiName"></param>
    public void OpenAsync<T>(string uiName = "")
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
                var uiObj = Instantiate(obj);

                uiObj.transform.SetParent(UiRoot, false);

                gui = uiObj.GetComponent<BaseGui>();

                gui.Init();

                uiObj.SetActive(true);
            }

            if (gui != null)
                gui.Open();

        }, UiRoot);
    }

    public void Close(BaseGui gui)
    {
        prefabLoadMgr.Destroy(gui.gameObject);
    }
}
