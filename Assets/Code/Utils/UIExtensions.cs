using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIExtensions
{
    public static T FindWidget<T>(this Transform transform, string widgetName) where T : UIBehaviour
    {
        var tra = transform.Find(widgetName);

        if (tra == null)
        {
            Debug.LogWarningFormat("昵称为{0}的部件未找到 ！！！", widgetName);

            return default;
        }

        return tra.GetComponent<T>();
    }

    public static void AddButtonOnClick(this Button btn, System.Action<GameObject> onClick)
    {
        btn.onClick.AddListener(() => { onClick?.Invoke(btn.gameObject); });
    }
}
