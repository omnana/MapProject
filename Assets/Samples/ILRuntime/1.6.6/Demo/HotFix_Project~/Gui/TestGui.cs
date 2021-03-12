using UnityEngine.UI;
using UnityEngine;

namespace HotFix_Project.Gui
{
    public class TestGui : IlRuntimeBaseGui
    {
        private Text hotFixText;

        private Button hotFixBtn;

        private int count;

        public override void OnInit()
        {
            base.OnInit();

            Debug.Log("HotFix_Project : TestGui Init");

            hotFixText = transform.FindWidget<Text>("HotFixText");

            hotFixBtn = transform.FindWidget<Button>("HotFixButton");

            hotFixBtn.onClick.AddListener(HotFix_OnClick);
        }

        public override void OnOpen()
        {
            base.OnOpen();

            Debug.Log("HotFix_Project : TestGui Open");

            hotFixText.text = "来自热更的招呼 " + (count++);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //Debug.Log("HotFix_Project : TestGui Update");
        }

        public override void OnClose()
        {
            base.OnClose();
            Debug.Log("HotFix_Project : TestGui Close");
        }

        public override void Destroy()
        {
            base.Destroy();

            Debug.Log("HotFix_Project : TestGui Destroy");
        }

        private void HotFix_OnClick()
        {
            hotFixText.text = "来自热更的招呼 " + (count++);
        }
    }
}
