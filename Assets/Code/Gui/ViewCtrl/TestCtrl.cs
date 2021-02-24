using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCtrl : BaseCtrl
{
    public TestServicer TestServicer;

    private TestViewModel TestViewModel
    {
        get
        {
            return viewModel as TestViewModel;
        }
    }

    public TestCtrl()
    {
        TestServicer = ServiceContainer.Resolve<TestServicer>();
    }

    public async void GetData()
    {
        await TestServicer.Test();
    }

    public void SetInput(string input)
    {
        TestViewModel.TestName.Value = input;

        TestViewModel.TestInput.Value = input;
    }
}
