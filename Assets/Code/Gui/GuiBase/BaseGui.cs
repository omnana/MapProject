using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseGui : MonoBehaviour
{
    /// 热更内容
    public IlRuntimeBaseGui LlRContent;

    private void Awake()
    {
    }

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
