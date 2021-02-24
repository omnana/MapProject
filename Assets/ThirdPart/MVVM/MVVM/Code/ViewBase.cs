using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IView<T> where T : ViewModelBase
{
    T BindingContext { get; set; }
}

public class ViewBase<T> : MonoBehaviour, IView<T> where T: ViewModelBase
{
    private bool _isBindingContextInitialized;

    public readonly BindableProperty<T> ViewModelProperty = new BindableProperty<T>();

    public PropertyBinder<T> Binder = new PropertyBinder<T>();

    public T BindingContext
    {
        get
        {
            return ViewModelProperty.Value;
        }
        set
        {
            if(!_isBindingContextInitialized)
            {
                OnInitialize();

                _isBindingContextInitialized = true;
            }

            ViewModelProperty.Value = value;
        }
    }

    public Action OnRevealed; // 显示完毕，额外操作


    protected virtual void OnBindingContextChanged(T oldViewModel, T newViewModel)
    {
        Binder.Unbind(oldViewModel);

        Binder.Bind(newViewModel);
    }


    /// <summary>
    /// 无所ViewModel的Value怎样变化，只对OnValueChanged事件监听(绑定)一次
    /// </summary>
    protected virtual void OnInitialize()
    {
        ViewModelProperty.OnValueChanged += OnBindingContextChanged;
    }

    /// <summary>
    /// 激活View
    /// </summary>
    public virtual void OnAppear()
    {
        gameObject.SetActive(true);

        BindingContext.OnStartReveal();
    }

    /// <summary>
    /// 显示，播放动画，或者直接打开
    /// </summary>
    public virtual void OnReveal(bool immitate)
    {
    }

    /// <summary>
    /// 开始隐藏
    /// </summary>
    public virtual void OnHide()
    {

    }

    /// <summary>
    /// 以动画形式慢慢隐藏，或者直接
    /// </summary>
    public virtual void OnHidden(bool immitate)
    {

    }

    /// <summary>
    /// 隐藏结束
    /// </summary>
    public virtual void OnDisappear()
    {

    }

    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void OnDestory()
    {

    }
}
