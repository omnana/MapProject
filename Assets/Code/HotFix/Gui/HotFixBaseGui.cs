using UnityEngine;

/// <summary>
/// 给Gui做热更适配器
/// </summary>
public abstract class HotFixBaseGui
{
    public GameObject gameObject { get; private set; }

    public Transform transform { get; private set; }

    public Transform parent { get; private set; }

    public RectTransform rectTransform { get; private set; }

    public void SetGameObject(GameObject gameObject, Transform parent)
    {
        this.gameObject = gameObject;

        transform = gameObject.transform;

        this.parent = parent;

        if (parent != null) transform.SetParent(parent, false);
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnOpen()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnClose()
    {
    }

    public virtual void Destroy()
    {
    }
}
