using UnityEngine.UI;

namespace Omnana
{
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

        public void ExcuteGetData()
        {
            MessageAggregator<object>.Instance.Publish("GetData", this, new MessageArgs<object>(0));
        }

        public void ExcuteSetInput(string input)
        {
            MessageAggregator<string>.Instance.Publish("SetInput", this, new MessageArgs<string>(input));
        }
    }

}