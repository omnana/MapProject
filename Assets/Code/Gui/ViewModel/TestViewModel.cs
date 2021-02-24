using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestViewModel : ViewModelBase
{
    public BindableProperty<string> TestName = new BindableProperty<string>();

    public BindableProperty<string> TestInput = new BindableProperty<string>();

    private TestCtrl testCtrl
    {
        get
        {
            return (TestCtrl)controller;
        }
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        controller = ServiceLocator.Resolve<TestCtrl>();

        controller.Loaded(this);

        MessageAggregator<object>.Instance.Subscribe("GetData", ExcuteGetData);

        MessageAggregator<string>.Instance.Subscribe("SetInput", ExcuteSetInput);
    }

    private void ExcuteGetData(object sender, MessageArgs<object> args)
    {
        testCtrl.GetData();
    }

    private void ExcuteSetInput(object sender, MessageArgs<string> args)
    {
        testCtrl.SetInput(args.Item);
    }
}
