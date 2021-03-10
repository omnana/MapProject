using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix_Project.Gui
{
    public class TestGui : IlRuntimeBaseGui
    {
        public override void OnInit()
        {
            base.OnInit();
            Debug.Log("HotFix_Project : TestGui OnInit");
        }

        public override void OnOpen()
        {
            base.OnOpen();
            Debug.Log("HotFix_Project : TestGui OnOpen");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnClose()
        {
            base.OnClose();
            Debug.Log("HotFix_Project : TestGui OnClose");
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("HotFix_Project : TestGui Destroy");
        }
    }
}
