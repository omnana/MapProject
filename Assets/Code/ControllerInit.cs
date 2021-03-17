using UnityEngine;

namespace Omnana
{
    public class ControllerHelper
    {
        /// <summary>
        /// 主工程的Ctrl
        /// </summary>
        public static void Load()
        {
            ServiceLocator.RegisterSingleton<TestCtrl>();
        }
    }
}