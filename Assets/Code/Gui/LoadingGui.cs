using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingGui : BaseGui
{
    public LoadingView LoadingView;

    private void Awake()
    {
        LoadingView.BindingContext = new LoadingViewModel();

        LoadingView.OnAppear();
    }
}
