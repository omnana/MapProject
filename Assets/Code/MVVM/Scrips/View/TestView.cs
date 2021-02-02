using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TestView : ViewBase<TestViewModel>
{

    public string TestName
    {
        get
        {
            return TestText.text;
        }
        set
        {
            TestText.text = value;
        }
    }


    public Text TestText;

    public InputField TestInput;

    public TestViewModel TestViewModel { get { return BindingContext; } }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Binder.Add<string>("TestName", OnTestNameValueChanged);

        Binder.Add<string>("TestInput", OnTestInputValueChanged);
    }

    public void OnTestNameValueChanged(string oldValue, string newValue)
    {
        TestText.text = newValue;
    }

    public void OnTestInputValueChanged(string oldValue, string newValue)
    {
        TestInput.text = newValue;
    }
}
