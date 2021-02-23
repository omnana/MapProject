using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelBase
{
    private bool isInitialized;

    public ViewModelBase ParentViewModel { get; set; }

    public bool IsRevealed { get; private set; }

    public bool IsRevealInProgress { get; private set; }

    public bool IsHideInProgress { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void OnInitialize()
    {

    }

    /// <summary>
    /// 开始显示
    /// </summary>
    public virtual void OnStartReveal()
    {
        IsRevealInProgress = true;
        //在开始显示的时候进行初始化操作
        if (!isInitialized)
        {
            OnInitialize();
            isInitialized = true;
        }
    }

    /// <summary>
    /// 显示结束
    /// </summary>
    public virtual void OnFinishReveal()
    {
        IsRevealInProgress = false;
        IsRevealed = true;
    }

    /// <summary>
    /// 开始隐藏
    /// </summary>
    public virtual void OnStartHide()
    {
        IsHideInProgress = true;

    }

    /// <summary>
    /// 隐藏结束
    /// </summary>
    public virtual void OnFinishHide()
    {
        IsHideInProgress = false;
        IsRevealed = false;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void OnDestory()
    {

    }
}
