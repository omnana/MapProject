using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseGui : MonoBehaviour
{
    /// 热更内容
    public IlRuntimeBaseGui LlRContent;

    protected bool NeedUpdate = true;

    private void Awake()
    {
    }

    private void Update()
    {
        if (!NeedUpdate) return;

        if (LlRContent != null)
        {
            LlRContent.OnUpdate();
        }
        else
        {
            OnUpdate();
        }
    }

    private void OnDestroy()
    {
        if (LlRContent != null)
        {
            LlRContent.Destroy();
        }
        else
        {
            Destroy();
        }
    }

    // 外部调用
    public void Init()
    {
        if(LlRContent != null)
        {
            LlRContent.OnInit();
        }
        else
        {
            OnInit();
        }
    }

    public void Open()
    {
        if (LlRContent != null)
        {
            LlRContent.OnOpen();
        }
        else
        {
            OnOpen();
        }
    }

    public void Close()
    {
        if (LlRContent != null)
        {
            LlRContent.OnClose();
        }
        else
        {
            OnClose();
        }
    }

    // 内部
    protected virtual void OnInit()
    {
        // 外部调用
    }

    protected virtual void OnOpen()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnClose()
    {
    }

    protected virtual void Destroy()
    {
    }
}
