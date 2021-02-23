using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestViewModel : ViewModelBase
{
    public BindableProperty<string> TestName = new BindableProperty<string>();

    public BindableProperty<string> TestInput = new BindableProperty<string>();

    protected override void OnInitialize()
    {
        base.OnInitialize();

    }
}
