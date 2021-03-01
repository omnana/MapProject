using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingViewModel : ViewModelBase
{
    protected override void OnInitialize()
    {
        base.OnInitialize();

        controller = ServiceLocator.Resolve<LoadingCtrl>();

        controller.Loaded(this);
    }
}
