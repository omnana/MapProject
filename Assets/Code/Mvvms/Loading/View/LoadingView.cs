using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : ViewBase<LoadingViewModel>
{

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnBindingContextChanged(LoadingViewModel oldViewModel, LoadingViewModel newViewModel)
    {
        base.OnBindingContextChanged(oldViewModel, newViewModel);
    }
}
