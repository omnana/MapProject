using UnityEngine;
using Omnana;

/// <summary>
/// 给Gui做热更适配器
/// </summary>
public class HotFixBaseGui : IGui
{
    public GameObject GameObject { get; private set; }

    public Transform Transform { get; private set; }

    public Transform Parent { get; private set; }

    public RectTransform RectTransform { get; private set; }

    public void SetGameObject(GameObject gameObject, Transform parent)
    {
        GameObject = gameObject;

        Transform = gameObject.transform;

        Parent = parent;

        if (parent != null) Transform.SetParent(parent, false);
    }

    public virtual void DoInit()
    {
    }

    public virtual void DoOpen()
    {
    }

    public virtual void DoUpdate()
    {
    }

    public virtual void DoClose()
    {
    }

    public virtual void DoDestroy()
    {
    }
}
