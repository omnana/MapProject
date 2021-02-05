using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using AssetBundles;

public class GuiManager : MonoBehaviour
{
    public static GuiManager Instance { get; private set; }

    public Transform UiRoot;

    private ResourceService resourceService;

    public virtual void Awake()
    {
        Instance = this;

        resourceService = ServiceLocator.Resolve<ResourceService>();
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

        var uiPrefab = resourceService.LoadAssetSync<GameObject>(ResourceType.Gui, uiName);

        if (uiPrefab != null)
        {
            var uiObj = Instantiate(uiPrefab);

            uiObj.transform.SetParent(UiRoot, false);

            uiObj.SetActive(true);
        }
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

        resourceService.LoadAssetAsync<GameObject>(ResourceType.Gui, uiName, (obj) =>
         {
             if (obj != null)
             {
                 var uiObj = Instantiate(obj);

                 uiObj.transform.SetParent(UiRoot, false);

                 uiObj.SetActive(true);
             }
         });
    }
}
